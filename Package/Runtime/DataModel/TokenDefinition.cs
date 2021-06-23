using System;
using UnityEngine;

namespace HeathenEngineering.BGSDK.DataModel
{

    [Serializable]
    public class TokenDefinition
    {
        /// <summary>
        /// name of the token type
        /// </summary>
        [Tooltip("Name of the token type")]
        public string name;
        /// <summary>
        /// description of the token type
        /// </summary>
        [Tooltip("Description of the token type")]
        public string description;
        [HideInInspector]
        public ulong currentSupply;
        public bool fungible;
        public bool burnable;
        /// <summary>
        /// The URL with more information about the token
        /// </summary>
        [Tooltip("The URL with more information about the token")]
        public string externalUrl;
        /// <summary>
        /// Image url of the token, 2000x2000, preferably svg
        /// </summary>
        [Tooltip("Image url of the token, 2000x2000, preferably svg")]
        public string image;

        public TypeValuePair[] animationUrls;
        public TokenAttributes[] attributes;

        public virtual string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }

    public class TokenDefinition<T> : TokenDefinition
    {
        public T properties;

        public override string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
