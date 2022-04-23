using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData;
using Assets.Resources.Ancible_Tools.Scripts.UI;
using Assets.Resources.Ancible_Tools.Scripts.UI.Alerts;
using Battlehub.Dispatcher;
using MessageBusLib;
using MLAPI.Cryptography.KeyExchanges;
using Telepathy;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class ClientController : MonoBehaviour
    {
        public static string ClientId { get; set; }
        public static int TimeBetweenMessages => _instance._checkEvery;

        private static ClientController _instance = null;

        [SerializeField] private string _ipAddress;
        [SerializeField] private int _port = 42069;
        [SerializeField] private int _checkEvery = 150;
        [SerializeField] private string _connectionSettingsFileName = string.Empty;

        [Header("Dev Settings")]
        [SerializeField] private string _devIp;
        [SerializeField] private int _devPort = 0;

        private Thread _messagingThread = null;
        private Client _client = null;
        private bool _active = false;

        private byte[] _key = new byte[0];

        private List<ClientMessage> _outgoing = new List<ClientMessage>();

        public ClientController(string connectionSettingsFileName)
        {
            _connectionSettingsFileName = connectionSettingsFileName;
        }

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            var connectionSettingsPath = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}{_connectionSettingsFileName}.{DataExtensions.JSON}";
            if (File.Exists($"{connectionSettingsPath}"))
            {
                var json = File.ReadAllText(connectionSettingsPath);
                var connectionSettings = AncibleUtils.FromJson<ConnectionSettings>(json);
                if (connectionSettings != null)
                {
                    _ipAddress = connectionSettings.IpAddress;
                    _port = connectionSettings.Port;
                }
                else
                {
                    connectionSettings = new ConnectionSettings { IpAddress = _ipAddress, Port = _port };
                    json = AncibleUtils.ConverToJson(connectionSettings);
                    File.WriteAllText($"{connectionSettingsPath}", json);
                }
            }
            else
            {
                var connectionSettings = new ConnectionSettings { IpAddress = _ipAddress, Port = _port };
                var json = AncibleUtils.ConverToJson(connectionSettings);
                File.WriteAllText($"{connectionSettingsPath}", json);
            }
#if UNITY_EDITOR
            _ipAddress = _devIp;
            _port = _devPort;
#endif
            SubscribeToMessages();
        }

        void Start()
        {
            _client = new Client();
            _messagingThread = new Thread(() =>
            {
                _active = true;
                var checkEvery = _checkEvery;
                while (_active)
                {
                    //SendMessagesToServer();
                    CheckMessages();
                    Thread.Sleep(checkEvery);
                }

                _messagingThread = null;
            });
            _messagingThread.Start();
            _client.MaxMessageSize = 500000;
            _client.Connect(_ipAddress, _port);
        }

        public static void SendMessageToServer(ClientMessage message, bool useClientIdAsFilter = true)
        {
            message.Sender = null;
            message.ClientId = ClientId;
            if (useClientIdAsFilter)
            {
                message.Filter = ClientId;
            }

            _instance._client.Send(message.ConvertToJson());
            //_instance._outgoing.Add(message);
        }

        private void SendMessagesToServer()
        {
            if (_client.Connected)
            {
                var messages = _outgoing.ToArray();
                _outgoing.Clear();
                for (var i = 0; i < messages.Length; i++)
                {
                    _client.Send(messages[i].ConvertToJson());
                }
            }

        }

        private void CheckMessages()
        {
            while (_client.GetNextMessage(out var netMsg))
            {
                switch (netMsg.eventType)
                {
                    case Telepathy.EventType.Connected:
                        Dispatcher.Current.BeginInvoke(() =>
                        {
                            DataController.SetWorldState(WorldState.Connected);
                            UiServerStatusTextController.SetText("Connected. Registering with server");
                        });
                        break;
                    case Telepathy.EventType.Data:
                        var clientMsg = netMsg.data.GenerateMessageFromJson();
                        if (clientMsg != null)
                        {
                            if (string.IsNullOrEmpty(clientMsg.Filter))
                            {
                                Dispatcher.Current.BeginInvoke(() =>
                                {
                                    gameObject.SendMessage(clientMsg);
                                });
                            }
                            else
                            {
                                Dispatcher.Current.BeginInvoke(() =>
                                {
                                    gameObject.SendMessageWithFilter(clientMsg, clientMsg.Filter);
                                });
                            }
                        }
                        break;
                    case Telepathy.EventType.Disconnected:
                        Dispatcher.Current.BeginInvoke(() =>
                        {
                            DataController.SetWorldState(WorldState.Disconnected);
                            UiServerStatusTextController.SetText("Disconnected.");
                        });
                        break;
                }
            }
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ClientRegisterResultMessage>(ClientRegisterResult);
            gameObject.Subscribe<LoginMessage>(Login);
            gameObject.Subscribe<RegisterKeyMessage>(RegisterKey);
            gameObject.Subscribe<ClientLoginResultMessage>(ClientLoginResult);
            gameObject.Subscribe<RefreshPlayerCharacterListMessage>(RefreshPlayerCharacterList);
            gameObject.Subscribe<ClientEnterWorldWithCharacterResultMessage>(ClientEnterCharacterResult);
            gameObject.Subscribe<ClientPlayerRespawnMessage>(ClientPlayerRespawn);
            gameObject.Subscribe<ClientLeaveWorldResultMessage>(ClientLeaveWorldResult);
        }

        private void ClientRegisterResult(ClientRegisterResultMessage msg)
        {
            if (msg.Success)
            {
                UiServerStatusTextController.SetText("Registration succesful. Setting up menu...");
                _key = msg.Key;
                ClientId = msg.ClientId;
                gameObject.SendMessage(ShowLoginMessage.INSTANCE);
                UiServerStatusTextController.CloseText();
            }
            else
            {
                UiServerStatusTextController.SetText(msg.Message);
                DataController.SetWorldState(WorldState.Disconnected);
            }
        }

        private void Login(LoginMessage msg)
        {
            UiServerStatusTextController.SetText("Logging in...");
            try
            {
                var key = new ECDiffieHellman();
                var authKey = new ECDiffieHellman(key.GetPrivateKey()).GetSharedSecretRaw(_key);
                var secureLogin =
                    AncibleCrypto.Encrypt(new SecureLogin { Username = msg.Username, Password = msg.Password }.ToJson(),
                        authKey, out var iv);
                var clientLoginRequestMsg = new ClientLoginRequestMessage
                {
                    ClientId = ClientId,
                    Iv = iv,
                    Key = key.GetPublicKey(),
                    Login = secureLogin
                };
                SendMessageToServer(clientLoginRequestMsg, false);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Exception while logging in: {ex}");
                gameObject.SendMessage(ShowLoginMessage.INSTANCE);
                UiServerStatusTextController.SetText("Error while trying to log in to server. Please try again.", true);
            }
        }

        private void RegisterKey(RegisterKeyMessage msg)
        {
            UiServerStatusTextController.SetText("Registering...");
            try
            {
                var key = new ECDiffieHellman();
                var authKey = new ECDiffieHellman(key.GetPrivateKey()).GetSharedSecretRaw(_key);
                var secureLogin =
                    AncibleCrypto.Encrypt(new SecureLogin {Username = msg.Username, Password = msg.Password}.ToJson(),
                        authKey, out var iv);
                SendMessageToServer(new ClientClaimKeyRequestMessage
                {
                    GameKey = msg.GameKey,
                    Login = secureLogin,
                    Iv = iv,
                    Key = key.GetPublicKey()
                }, false);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Exception while registering key - {ex}");
                UiServerStatusTextController.SetText($"Error while trying to register game key. Please try again.", true);
            }
        }

        private void ClientLoginResult(ClientLoginResultMessage msg)
        {
            if (msg.Success)
            {
                DataController.SetWorldState(WorldState.LoggedIn);
                UiServerStatusTextController.SetText("Login succesfull. Requesting characters...");
                SendMessageToServer(new ClientCharacterRequestMessage());
                gameObject.SendMessage(HideLoginMessage.INSTANCE);
            }
            else
            {
                gameObject.SendMessage(ShowLoginMessage.INSTANCE);
                UiServerStatusTextController.SetText(msg.Message, true);
            }
        }

        private void RefreshPlayerCharacterList(RefreshPlayerCharacterListMessage msg)
        {
            gameObject.SendMessage(ShowPlayerCharacterListMessage.INSTANCE);
            UiServerStatusTextController.CloseText();
        }

        private void ClientEnterCharacterResult(ClientEnterWorldWithCharacterResultMessage msg)
        {
            if (msg.Success)
            {
                UiServerStatusTextController.CloseText();
                StartCoroutine(StaticMethods.WaitForFrames(1, () =>
                {
                    DataController.SetWorldState(WorldState.Active);
                }));
            }
            else
            {
                UiServerStatusTextController.SetText(msg.Message, true);
                gameObject.SendMessage(ShowPlayerCharacterListMessage.INSTANCE);
            }
        }

        private void ClientPlayerRespawn(ClientPlayerRespawnMessage msg)
        {
            UiServerStatusTextController.CloseText();
        }

        private void ClientLeaveWorldResult(ClientLeaveWorldResultMessage msg)
        {
            if (msg.Success)
            {
                UiServerStatusTextController.SetText("Leave Request Successful - Requesting character list...");
            }
            else
            {
                UiServerStatusTextController.SetText($"Error while leaving world - {msg.Message}. Requesting character list...", true);
            }
            SendMessageToServer(new ClientCharacterRequestMessage());
        }

        void OnDestroy()
        {
            _active = false;
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}