using System;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class ListedNFTToken
    {
        public string id;
        public string tokenId;
        public string owner;
        public string name;
        public string description;
        public string url;
        public string backgroundColor;
        public string imageUrl;
        public string imagePreviewUrl;
        public string imageThumbnailUrl;
        public DataModel.ContractData contract;
    }
}
