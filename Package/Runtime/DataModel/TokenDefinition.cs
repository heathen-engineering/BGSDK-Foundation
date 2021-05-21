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
        /// <summary>
        /// Only applicable in case of a fungible token, this indicates the number of decimals the fungible token has
        /// </summary>
        [Tooltip("Only applicable in case of a fungible token, this indicates the number of decimals the fungible token has")]
        public uint decimals;
        public ulong currentSupply;
        public bool fungible;
        public bool burnable;

        /// <summary>
        /// The backgroundcolor of the image
        /// </summary>
        [Tooltip("The backgroundcolor of the image")]
        public string backgroundColor;
        /// <summary>
        /// The URL with more information about the token
        /// </summary>
        [Tooltip("The URL with more information about the token")]
        public string externalUrl;
        /// <summary>
        /// Image url of the token, 250x250, preferably svg
        /// </summary>
        [Tooltip("Image url of the token, 250x250, preferably svg")]
        public string imagePreview;
        /// <summary>
        /// Image url of the token, 128x128, preferably svg
        /// </summary>
        [Tooltip("Image url of the token, 128x128, preferably svg")]
        public string imageThumbnail;
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
