using System;
using System.Collections.Generic;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class NFTBalanceResult : BGSDKBaseResult
    {
        [Serializable]
        public class Token
        {
            [Serializable]
            public class Contract
            {
                public string name;
                public string description;
                public string address;
                public string symbol;
                public string url;
                public string imageUrl;
                public TypeValuePair[] media;
                public string type;
                public bool verified;
            }

            public string name;
            public string id;            
            public string description;
            public string url;
            public string backgroundColor;
            public string imageUrl;
            public string imagePreviewUrl;
            public string imageThumbnailUrl;
            public string animationUrl;
            public TypeValuePair[] animationUrls;
            public bool fungible;
            public ulong maxSupply;
            public TokenAttributes[] attributes;

            public string TokenTypeId
            {
                get
                {
                    foreach(var attribute in attributes)
                    {
                        if (attribute.name == "tokenTypeId")
                            return attribute.value;
                    }

                    return "[unknown]";
                }
            }

            public Engine.Token TokenType
            {
                get
                {
                    var id = TokenTypeId;

                    if(Engine.BGSDKSettings.current != null && !string.IsNullOrEmpty(id))
                    {
                        foreach(var contract in Engine.BGSDKSettings.current.contracts)
                        {
                            foreach(var token in contract.tokens)
                            {
                                if (token.Id == id)
                                    return token;
                            }
                        }

                        return null;
                    }
                    else
                    {
                        if(Engine.BGSDKSettings.current == null)
                        {
                            UnityEngine.Debug.LogWarning("Failed to return Token reference, you must set an active BGSDKSettings object first!");
                        }
                        else if(string.IsNullOrEmpty(id))
                        {
                            UnityEngine.Debug.LogWarning("Failed to return Token reference, the token data in question has no tokenTypeId attribute!");
                        }

                        return null;
                    }
                }
            }
        }

        public List<Token> result;
    }
}
