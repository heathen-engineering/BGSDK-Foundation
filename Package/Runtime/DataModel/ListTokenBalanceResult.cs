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
        }

        public List<Token> result;
    }
}
