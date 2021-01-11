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

        public Properties properties;

        public string Id
        {
            get
            {
                return data?.id;
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
                return data?.name;
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
                return data?.description;
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
                return data?.contractAddress; 
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
                return data?.backgroundColor; 
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
                return data?.url; 
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
                return data?.imagePreview; 
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
                return data?.imageThumbnail;
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
                return data?.image; 
            }
            set 
            {
                if (data == null)
                    data = new TokenResponceData(); 
                data.image = value; 
            }
        }


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
                var prop = properties as Properties<T>;
                prop.data = webResults.result.properties;
            }
        }

        public T GetProperties<T>()
        {
            if (properties != null && properties.DataType == typeof(T))
            {
                var prop = properties as Properties<T>;
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
            Properties<T> prop = null;
            if (properties != null && properties.DataType == typeof(T))
            {
                prop = properties as Properties<T>;
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
