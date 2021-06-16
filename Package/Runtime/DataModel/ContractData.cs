using System;
using UnityEngine;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public struct ContractData
    {
        /// <summary>
        /// id of the contract.
        /// </summary>
        [HideInInspector]
        public string id;
        /// <summary>
        /// name of the contract.
        /// </summary>
        [Tooltip("name of the contract.")]
        public string name;
        /// <summary>
        /// description of the contract.
        /// </summary>
        [Tooltip("description of the contract.")]
        public string description;
        /// <summary>
        /// Whether it’s been confirmed or not (on the blockchain).
        /// </summary>
        [HideInInspector]
        public bool confirmed;
        /// <summary>
        /// The chain the contract will be part of
        /// </summary>
        [Tooltip("The chain the contract will be part of")]
        public API.Wallets.SecretType secretType;
        /// <summary>
        /// Address on the blockchain.
        /// </summary>
        [HideInInspector]
        public string address;
        /// <summary>
        /// Symbol of the contract (e.g. GODS, CKITTY, STRK)
        /// </summary>
        [Tooltip("Symbol of the contract (e.g. GODS, CKITTY, STRK).")]
        public string symbol;
        /// <summary>
        /// Link to the website/application that issued this contract
        /// </summary>
        [HideInInspector]
        public string externalUrl;
        /// <summary>
        /// Link to an image (logo) of this contract.
        /// </summary>
        [Tooltip("Link to an image (logo) of this contract.")]
        public string image;
        /// <summary>
        /// Additional data used by the contract.
        /// </summary>
        [Tooltip("Additional data used by the contract.")]
        public TypeValuePair[] media;
    }
}
