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
                    case "AETERNITY":
                        data.secretType = API.Wallets.SecretType.AETERNITY;
                        break;
                    case "AVAC":
                        data.secretType = API.Wallets.SecretType.AVAC;
                        break;
                    case "BITCOIN":
                        data.secretType = API.Wallets.SecretType.BITCOIN;
                        break;
                    case "BSC":
                        data.secretType = API.Wallets.SecretType.BSC;
                        break;
                    case "ETHEREUM":
                        data.secretType = API.Wallets.SecretType.ETHEREUM;
                        break;
                    case "GOCHAIN":
                        data.secretType = API.Wallets.SecretType.GOCHAIN;
                        break;
                    case "LITECOIN":
                        data.secretType = API.Wallets.SecretType.LITECOIN;
                        break;
                    case "MATIC":
                        data.secretType = API.Wallets.SecretType.MATIC;
                        break;
                    case "NEO":
                        data.secretType = API.Wallets.SecretType.NEO;
                        break;
                    case "TRON":
                        data.secretType = API.Wallets.SecretType.TRON;
                        break;
                    case "VECHAIN":
                        data.secretType = API.Wallets.SecretType.VECHAIN;
                        break;
                }

                return data;
            }
        }

        public List<RecieveContractModel> result;
    }
}
