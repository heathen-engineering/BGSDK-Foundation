using HeathenEngineering;
using HeathenEngineering.BGSDK.DataModel;
using System.Numerics;
using UnityEngine;

namespace HeathenEngineering.BGSDK.Engine
{
    [CreateAssetMenu(menuName = "Blockchain Game SDK/Token")]
    public class Token : ScriptableObject
    {
        [HideInInspector]
        public bool UpdatedFromServer = false;
        [HideInInspector]
        public long UpdatedOn;
        [HideInInspector]
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
