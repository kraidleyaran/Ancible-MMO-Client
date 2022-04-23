using System;

namespace Assets.Ancible_Tools.Scripts.System
{
    [Serializable]
    public class ConnectionSettings
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
    }
}