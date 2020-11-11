using System;
using System.Numerics;
using UnityEngine;

namespace HeathenEngineering.Arkane.DataModel
{
    [Serializable]
    /// <summary>
    /// Represents the complete set of Token data as you would recieve from a GetTokenType call
    /// </summary>
    public struct TokenData
    {
        [Tooltip("Address of the deployed token.")]
        public string contractAddress;
        [Tooltip("Internal id of the type")]
        public ulong id;
        [Tooltip("Blockchain-generated typeId.")]
        public BigInteger contractTypeId;
        [Tooltip("Whether or not the transaction has been confirmed.")]
        public bool confirmed;
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
        /// <summary>
        /// Flag that indicates if this type is a non-fungible (true for non-fungible, false for fungible)
        /// </summary>
        [Tooltip("Flag that indicates if this type is a non-fungible (true for non-fungible, false for fungible)")]
        public bool isNonFungible;
        /// <summary>
        /// Free text field of extra properties for the token
        /// </summary>
        [Tooltip("Free text field of extra properties for the token")]
        public string properties;
        /// <summary>
        /// The backgroundcolor of the image
        /// </summary>
        [Tooltip("The backgroundcolor of the image")]
        public string backgroundColor;
        /// <summary>
        /// The URL with more information about the token
        /// </summary>
        [Tooltip("The URL with more information about the token")]
        public string url;
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
        
        public TokenCreateRequestData ToTokenCreateRequestData()
        {
            var result = new TokenCreateRequestData()
            {
                backgroundColor = backgroundColor,
                decimals = decimals,
                description = description,
                image = image,
                imagePreview = imagePreview,
                imageThumbnail = imageThumbnail,
                isNonFungible = isNonFungible,
                name = name,
                properties = properties,
                url = url
            };
            return result;
        }

        public void UpdateFromTokenCreateRequestData(TokenCreateRequestData value)
        {
            backgroundColor = value.backgroundColor;
            decimals = value.decimals;
            description = value.description;
            image = value.image;
            imagePreview = value.imagePreview;
            imageThumbnail = value.imageThumbnail;
            isNonFungible = value.isNonFungible;
            name = value.name;
            properties = value.properties;
            url = value.url;
        }

        public TokenCreateResponceData ToTokenCreateResponceData()
        {
            var result = new TokenCreateResponceData()
            {
                confirmed = confirmed,
                contractAddress = contractAddress,
                contractTypeId = contractTypeId,
                id = id,
                properties = properties
            };
            return result;
        }

        public void UpdateFromTokenCreateResponceData(TokenCreateResponceData value)
        {
            confirmed = value.confirmed;
            contractAddress = value.contractAddress;
            contractTypeId = value.contractTypeId;
            id = value.id;
            properties = value.properties;
        }
    }
    }
