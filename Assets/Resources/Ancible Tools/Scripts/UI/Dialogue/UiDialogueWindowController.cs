using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData.Client;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.Dialogue;
using Assets.Resources.Ancible_Tools.Scripts.UI.Windows;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Dialogue
{
    public class UiDialogueWindowController : UiBaseWindow
    {
        public override bool Movable => true;
        public override bool Blocking => true;

        public string OwnerId => _ownerId;

        [Header("Child References")]
        [SerializeField] private Text _titleText;
        [SerializeField] private Image _spriteIcon;
        [SerializeField] private Text _dialogueText;
        [SerializeField] private Text _pageText;
        [SerializeField] private Button _previousButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private RectTransform _content = null;
        [SerializeField] private VerticalLayoutGroup _verticalLayout = null;

        [Header("Prefab References")]
        [SerializeField] private UiDialogueAnswerController _answerTemplate;

        private int _dataIndex = 0;
        private DialogueData _data = null;
        private List<UiDialogueAnswerController> _answerControllers = new List<UiDialogueAnswerController>();
        private string _ownerId = string.Empty;

        void Awake()
        {
            SubscribeToMessages();
        }

        public void Setup(DialogueData data, string ownerId)
        {
            _dataIndex = 0;
            _data = data;
            if (!string.IsNullOrEmpty(ownerId) && (string.IsNullOrEmpty(_ownerId) || _ownerId != ownerId))
            {
                _ownerId = ownerId;
                var obj = ObjectManagerController.GetWorldObjectById(_ownerId);
                if (obj)
                {
                    ClientObjectData objData = null;
                    var queryObjDatMsg = MessageFactory.GenerateQueryNetworkObjectDataMsg();
                    queryObjDatMsg.DoAfter = (ownerData) => objData = ownerData;
                    gameObject.SendMessageTo(queryObjDatMsg, obj);
                    MessageFactory.CacheMessage(queryObjDatMsg);
                    if (objData != null)
                    {
                        _titleText.text = $"{objData.Name}";
                        var sprite = TraitFactoryController.GetSpriteTraitByName(objData.Sprite);
                        if (sprite)
                        {
                            _spriteIcon.sprite = sprite.Icon;
                        }
                        else
                        {
                            _spriteIcon.gameObject.SetActive(false);
                        }
                    }
                }

            }
            
            ShowDialogue(_data);
        }

        public void NextIndex()
        {
            if (_dataIndex < _data.Dialogue.Length - 1)
            {
                _dataIndex++;
                ShowDialogue(_data);
            }
        }

        public void PreviousIndex()
        {
            if (_dataIndex > 0)
            {
                _dataIndex--;
                ShowDialogue(_data);
            }
        }

        private void ShowDialogue(DialogueData data)
        {
            ClearAnswers();
            _dialogueText.text = data.Dialogue[_dataIndex];
            _pageText.text = $"{_dataIndex + 1} / {data.Dialogue.Length}";
            var textHeight = _dialogueText.GetHeightOfText(data.Dialogue[_dataIndex]);
            _dialogueText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textHeight);

            _nextButton.interactable = _dataIndex < data.Dialogue.Length - 1;
            _previousButton.interactable = _dataIndex > 0;
            if (data.Dialogue.Length <= 0 || _dataIndex >= data.Dialogue.Length -1)
            {
                for (var i = 0; i < data.Tree.Children.Length; i++)
                {
                    var controller = Instantiate(_answerTemplate, _content);
                    controller.Setup(data.Tree.Children[i]);
                    _answerControllers.Add(controller);
                }
            }

            var height = textHeight + _answerControllers.Sum(c => c.RectTransform.rect.height) + _verticalLayout.padding.top + (1 + _answerControllers.Count) * _verticalLayout.spacing;
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ClientFinishInteractionResultMessage>(ClientFinishInteractionResult);
        }

        private void ClientFinishInteractionResult(ClientFinishInteractionResultMessage msg)
        {
            Close();
        }

        private void ClearAnswers()
        {
            for (var i = 0; i < _answerControllers.Count; i++)
            {
                Destroy(_answerControllers[i].gameObject);
            }
            _answerControllers.Clear();
        }

        public override void Close()
        {
            ClientController.SendMessageToServer(new ClientFinishInteractionRequestMessage());
            base.Close();
        }
    }
}