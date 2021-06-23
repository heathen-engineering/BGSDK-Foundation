using HeathenEngineering;
using HeathenEngineering.BGSDK.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Networking;

namespace HeathenEngineering.BGSDK.Engine
{
    [CreateAssetMenu(menuName = "Blockchain Game SDK/Token")]
    public class Token : ScriptableObject
    {
        #region JSON Results
        [Serializable]
        public class Result
        {
            public string id;
            public string typeId;
            public Metadata metadata;
            public string mineDate;
            public bool confirmed;
            public ulong amount;
            public string transactionHash;
        }

        [Serializable]
        public class Metadata
        {
            [Serializable]
            public class Contract
            {
                public string address;
                public string name;
                public string symbol;
                public string image;
                public string imageUrl;
                public string image_url;
                public string description;
                public string externalLink;
                public string external_link;
                public string externalUrl;
                public string external_url;
                public TypeValuePair[] media;
                public string type;
            }

            public string name;
            public string description;
            public string image;
            public string imagePreview;
            public string ImageThumbnail;
            public string backgroundColor;
            public string background_color;
            public string externalUrl;
            public string external_url;
            public TypeValuePair[] animationUrls;
            public TokenAttributes[] attributes;
            public Contract contract;
            public Contract asset_contract;
            public bool fungible;
        }

        [Serializable]
        public class ResultList : BGSDKBaseResult
        {
            public List<Result> result;
        }
        #endregion

        [HideInInspector]
        public bool UpdatedFromServer = false;
        [HideInInspector]
        public long UpdatedOn;
        
        public Contract contract;

        public string Id
        {
            get
            {
                return data == null ? "" : data.id;
            }
            set
            {
                if (data == null)
                    data = new TokenResponceData();
                data.id = value;
            }
        }
        public string SystemName
        {
            get
            {
                return data == null ? "" : data.name;
            }
            set
            {
                if (data == null)
                    data = new TokenResponceData();
                data.name = value;
            }
        }
        public string Description
        {
            get
            {
                return data == null ? "" : data.description;
            }
            set
            {
                if (data == null)
                    data = new TokenResponceData();
                data.description = value;
            }
        }
        public bool Confirmed
        {
            get
            {
                return data != null ? data.confirmed : false;
            }
            set
            {
                if (data == null)
                    data = new TokenResponceData();
                data.confirmed = value;
            }
        }
        public string Address
        { 
            get 
            { 
                return data == null ? "" : data.contractAddress; 
            } 
            set 
            {
                if (data == null)
                    data = new TokenResponceData(); 
                data.contractAddress = value; 
            }
        }
        public BigInteger TypeId
        { 
            get 
            { 
                return data != null ? data.contractTypeId : new BigInteger(); 
            } 
            set 
            {
                if (data == null)
                    data = new TokenResponceData(); 
                data.contractTypeId = value; 
            } 
        }
        public bool IsNonFungible
        { 
            get 
            { 
                return data != null ? !data.fungible : false; 
            } 
            set 
            {
                if (data == null)
                    data = new TokenResponceData();
                data.fungible = !value; 
            } 
        }
        public string Url
        { 
            get 
            { 
                return data == null ? "" : data.externalUrl; 
            } 
            set 
            {
                if (data == null)
                    data = new TokenResponceData(); 
                data.externalUrl = value; 
            }
        }
        public string Image
        { 
            get 
            { 
                return data == null ? "" : data.image; 
            }
            set 
            {
                if (data == null)
                    data = new TokenResponceData(); 
                data.image = value; 
            }
        }

        [SerializeField]
        private DataModel.TokenResponceData data;

        public void Set(WebResults<TokenResponceData> webResults)
        {
            data = webResults.result;
        }

        public TokenDefinition GetTokenDefinition()
        {
            return data;
        }

        public IEnumerator Get(Action<ResultList> callback)
        {
            if (BGSDKSettings.current == null)
            {
                callback(new ResultList() { hasError = true, message = "Attempted to call BGSDK.Wallets.NFTs with no BGSDK.Settings object applied." });
                yield return null;
            }
            else
            {
                if (BGSDKSettings.user == null)
                {
                    callback(new ResultList() { hasError = true, message = "BGSDKSettings.user required, null Settings.user provided.", result = null });
                    yield return null;
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(BGSDKSettings.current.GetTokenUri(contract) + "/" + Id + "/tokens");
                    www.SetRequestHeader("Authorization", BGSDKSettings.user.authentication.token_type + " " + BGSDKSettings.user.authentication.access_token);

                    var co = www.SendWebRequest();
                    while (!co.isDone)
                        yield return null;

                    if (!www.isNetworkError && !www.isHttpError)
                    {
                        var results = new ResultList();
                        try
                        {
                            string resultContent = www.downloadHandler.text;
                            results = JsonUtility.FromJson<ResultList>(Utilities.JSONArrayWrapper(resultContent));
                            results.message = "List NFTs complete.";
                            results.httpCode = www.responseCode;

                        }
                        catch (Exception ex)
                        {
                            results = null;
                            results.message = "An error occured while processing JSON results, see exception for more details.";
                            results.exception = ex;
                            results.httpCode = www.responseCode;
                        }
                        finally
                        {
                            callback(results);
                        }
                    }
                    else
                    {
                        callback(new ResultList() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while requesting NFTs." : " a HTTP error occured while requesting NFTs."), result = null, httpCode = www.responseCode });
                    }
                }
            }
        }

#if UNITY_EDITOR
        public string CreateTokenDefitionJson()
        {
            return data.ToJson();
        }
#endif
    }
}
