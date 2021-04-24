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
            public string contractTokenId;
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

        public TokeProperties properties;

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
        public uint Decimals
        { 
            get 
            { 
                return data != null ? data.decimals : 0; 
            } 
            set 
            {
                if (data == null)
                    data = new TokenResponceData(); 
                data.decimals = value; 
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
                return data != null ? data.nft : false; 
            } 
            set 
            {
                if (data == null)
                    data = new TokenResponceData();
                data.nft = value; 
            } 
        }
        public string BackgroundColor
        { 
            get 
            { 
                return data == null ? "" : data.backgroundColor; 
            } 
            set 
            {
                if (data == null)
                    data = new TokenResponceData(); 
                data.backgroundColor = value; 
            }
        }
        public string Url
        { 
            get 
            { 
                return data == null ? "" : data.url; 
            } 
            set 
            {
                if (data == null)
                    data = new TokenResponceData(); 
                data.url = value; 
            }
        }
        public string ImagePreview
        { 
            get 
            {
                return data == null ? "" : data.imagePreview; 
            } 
            set 
            {
                if (data == null)
                    data = new TokenResponceData(); 
                data.imagePreview = value; 
            }
        }
        public string ImageThumbnail
        {
            get 
            { 
                return data == null ? "" : data.imageThumbnail;
            } 
            set 
            {
                if (data == null)
                    data = new TokenResponceData(); 
                data.imageThumbnail = value; 
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

        public void Set<T>(WebResults<TokenResponceData<T>> webResults)
        {
            data = webResults.result;

            if (properties != null && properties.DataType == typeof(T))
            {
                var prop = properties as TokenProperties<T>;
                prop.data = webResults.result.properties;
            }
        }

        public T GetProperties<T>()
        {
            if (properties != null && properties.DataType == typeof(T))
            {
                var prop = properties as TokenProperties<T>;
                return prop.data;
            }
            else
                return default;
        }

        public TokenDefinition GetTokenDefinition()
        {
            return data;
        }

        public TokenDefinition<T> GetTokenDefinition<T>()
        {
            TokenProperties<T> prop = null;
            if (properties != null && properties.DataType == typeof(T))
            {
                prop = properties as TokenProperties<T>;
            }

            var nDef = new TokenDefinition<T>
            {
                name = data.name,
                description = data.description,
                decimals = data.decimals,
                nft = data.nft,
                backgroundColor = data.backgroundColor,
                url = data.url,
                imagePreview = data.imagePreview,
                imageThumbnail = data.imageThumbnail,
                image = data.image,
            };

            if (prop != null)
                nDef.properties = prop.data;

            return nDef;
        }

        public IEnumerator Get(Action<ResultList> callback)
        {
            if (BGSDKSettings.current == null)
            {
                callback(new ResultList() { hasError = true, message = "Attempted to call BGSDK.Wallets.UserWallet.ListNFTs with no BGSDK.Settings object applied." });
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
            if (properties != null)
                return properties.ToJsonDef(data);
            else
                return data.ToJson();
        }
#endif
    }
}
