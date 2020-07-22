using HeathenEngineering;
using HeathenEngineering.Arkane.DataModel;
using System.Numerics;
using UnityEngine;

namespace HeathenEngineering.Arkane.Engine
{
    [CreateAssetMenu(menuName = "Arkane/Token")]
    public class Token : ScriptableObject
    {
        [HideInInspector]
        public bool UpdatedFromServer = false;
        [HideInInspector]
        public long UpdatedOn;
        [HideInInspector]
        public Contract contract;
        [HideInInspector]
        public DataModel.Token Data;

        public ulong id
        { get { return Data.id; } set { Data.id = value; } }
        public string systemName
        { get { return Data.name; } set { Data.name = value; } }
        public string description
        { get { return Data.description; } set { Data.description = value; } }
        public bool confirmed
        { get { return Data.confirmed; } set { Data.confirmed = value; } }
        public string address
        { get { return Data.contractAddress; } set { Data.contractAddress = value; } }
        public uint decimals
        { get { return Data.decimals; } set { Data.decimals = value; } }
        public BigInteger typeId
        { get { return Data.contractTypeId; } set { Data.contractTypeId = value; } }
        public bool isNonFungible
        { get { return Data.isNonFungible; } set { Data.isNonFungible = value; } }
        public string properties
        { get { return Data.properties; } set { Data.properties = value; } }
        public string backgroundColor
        { get { return Data.backgroundColor; } set { Data.backgroundColor = value; } }
        public string url
        { get { return Data.url; } set { Data.url = value; } }
        public string imagePreview
        { get { return Data.imagePreview; } set { Data.imagePreview = value; } }
        public string imageThumbnail
        { get { return Data.imageThumbnail; } set { Data.imageThumbnail = value; } }
        public string image
        { get { return Data.image; } set { Data.image = value; } }
        public TokenCreateRequestData TokenCreateRequestData => Data.ToTokenCreateRequestData();
        public TokenCreateResponceData TokenCreateResponceData => Data.ToTokenCreateResponceData();
    }
}
