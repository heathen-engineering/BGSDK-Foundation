using System;
using System.Numerics;
using UnityEngine;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public struct TokenCreateResponceData
    {
        [Tooltip("Address of the deployed token.")]
        public string contractAddress;
        [Tooltip("Internal id of the type")]
        public string id;
        [Tooltip("Blockchain-generated typeId.")]
        public BigInteger contractTypeId;
        [Tooltip("Whether or not the transaction has been confirmed.")]
        public bool confirmed;
        [Tooltip("The properties of the Token Type.")]
        public string properties;
    }
}
