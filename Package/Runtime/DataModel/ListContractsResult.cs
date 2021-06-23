using System;
using System.Collections.Generic;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class ListContractsResult : BGSDKBaseResult
    {
        [Serializable]
        public class RecieveContractModel
        {
            public string id;
            public string name;
            public string description;
            public bool confirmed;
            public string secretType;
            public string address;
            public string symbol;
            public string externalUrl;
            public string image;
            public TypeValuePair[] media;

            public ContractData ToContractData()
            {
                var data = new ContractData
                {
                    id = id,
                    name = name,
                    description = description,
                    confirmed = confirmed,
                    address = address,
                    symbol = symbol,
                    externalUrl = externalUrl,
                    image = image,
                    media = media,
                };

                switch (secretType)
                {
                    case "AVAC":
                        data.secretType = SecretType.AVAC;
                        break;
                    case "BSC":
                        data.secretType = SecretType.BSC;
                        break;
                    case "ETHEREUM":
                        data.secretType = SecretType.ETHEREUM;
                        break;
                    case "MATIC":
                        data.secretType = SecretType.MATIC;
                        break;
                }

                return data;
            }
        }

        public List<RecieveContractModel> result;
    }
}
