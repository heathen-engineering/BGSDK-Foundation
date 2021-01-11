using System;

namespace HeathenEngineering.BGSDK.DataModel
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
