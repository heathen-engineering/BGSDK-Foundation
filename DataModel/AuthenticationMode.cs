using System;

namespace HeathenEngineering.Arkane.DataModel
{
    [Serializable]
    public struct AuthenticationMode
    {
        public string ClientId;
        public string GrantType;

        public AuthenticationMode(string clientId, string grantType)
        {
            ClientId = clientId;
            GrantType = grantType;
        }
    }
}
