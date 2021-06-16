using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using HeathenEngineering.BGSDK.DataModel;
using HeathenEngineering.BGSDK.Engine;
using System.Text;

namespace HeathenEngineering.BGSDK.API
{
#if UNITY_EDITOR
    /// <summary>
    /// Privileged features only work in restricted environments with restricted cradentials.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This code only compiles in Unity Editor and Unity Server (headless builds) it will not compile for a regular client build.
    /// </para>
    /// <para>
    /// Privileged subsclasses such as <see cref="TokenManagement.Privileged"/> use API calls that are not available to regular end users and will always result in an error when called by a non privlaged conneciton.
    /// </para>
    /// </remarks>
    public static class Privileged
    {
        private class MintNonFungibleRequest
        {
            public string typeId;
            public string[] destinations;
        }

        private class MintFungibleRequest
        {
            public int[] amounts;
            public string[] destinations;
        }

        /// <summary>
        /// Create a NFT for each destination provided
        /// </summary>
        /// <remarks>
        /// See <see href="https://docs.venly.io/api/api-products/nft-api/mint-nft"/> for details
        /// </remarks>
        /// <param name="token"></param>
        /// <param name="destinations">The wallets to add the token to</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator MintNonFungibleToken(Token token, string[] destinations, Action<BGSDKBaseResult> callback)
        {
            if (BGSDKSettings.current == null)
            {
                callback(new DataModel.BGSDKBaseResult() { hasError = true, message = "Attempted to call BGSDK.Tokens.MintNonFungibleToken with no BGSDK.Settings object applied." });
                yield return null;
            }
            else
            {
                if (BGSDKSettings.user == null)
                {
                    callback(new DataModel.BGSDKBaseResult() { hasError = true, message = "BGSDKIdentity required, null identity provided.\nPlease initalize Settings.user before calling Privileged.MintNonFungibleToken" });
                    yield return null;
                }
                else
                {
                    var data = new MintNonFungibleRequest()
                    {
                        typeId = token.TypeId.ToString(),
                        destinations = destinations
                    };

                    var request = new UnityWebRequest(BGSDKSettings.current.MintTokenUri(token.contract) + "/non-fungible", "POST");
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
                    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                    request.SetRequestHeader("Authorization", BGSDKSettings.user.authentication.token_type + " " + BGSDKSettings.user.authentication.access_token);
                    request.SetRequestHeader("Content-Type", "application/json");
                    var async = request.SendWebRequest();

                    while (!async.isDone)
                        yield return null;

                    if (!request.isNetworkError && !request.isHttpError)
                    {
                        callback(new DataModel.BGSDKBaseResult() { hasError = false, message = "Successful request to mint token!", httpCode = request.responseCode });
                    }
                    else
                    {
                        callback(new DataModel.BGSDKBaseResult() { hasError = true, message = "Error:" + (request.isNetworkError ? " a network error occured while attempting to mint a token." : " a HTTP error occured while attempting to mint a token."), httpCode = request.responseCode });
                    }
                }
            }
        }

        /// <summary>
        /// Create a fungible token for each destination provided
        /// </summary>
        /// <remarks>
        /// See <see href="https://docs.venly.io/api/api-products/nft-api/mint-fungible-nft"/> for details
        /// </remarks>
        /// <param name="token"></param>
        /// <param name="amounts">The amount of tokens to add to each wallet</param>
        /// <param name="destinations">The wallets to add the tokens to</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator MintFungibleToken(Token token, int[] amounts, string[] destinations, Action<BGSDKBaseResult> callback)
        {
            if (BGSDKSettings.current == null)
            {
                callback(new DataModel.BGSDKBaseResult() { hasError = true, message = "Attempted to call BGSDK.Tokens.MintFungibleToken with no BGSDK.Settings object applied." });
                yield return null;
            }
            else
            {
                if (BGSDKSettings.user == null)
                {
                    callback(new DataModel.BGSDKBaseResult() { hasError = true, message = "BGSDKIdentity required, null identity provided.\nPlease initalize Settings.user before calling Privileged.MintFungibleToken" });
                    yield return null;
                }
                else
                {
                    var data = new MintFungibleRequest()
                    {
                        amounts = amounts,
                        destinations = destinations
                    };

                    var request = new UnityWebRequest(BGSDKSettings.current.MintTokenUri(token.contract) + "/fungible/" + token.TypeId.ToString(), "POST");
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
                    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                    request.SetRequestHeader("Authorization", BGSDKSettings.user.authentication.token_type + " " + BGSDKSettings.user.authentication.access_token);
                    request.SetRequestHeader("Content-Type", "application/json");
                    var async = request.SendWebRequest();

                    while (!async.isDone)
                        yield return null;

                    if (!request.isNetworkError && !request.isHttpError)
                    {
                        callback(new DataModel.BGSDKBaseResult() { hasError = false, message = "Successful request to mint token!", httpCode = request.responseCode });
                    }
                    else
                    {
                        callback(new DataModel.BGSDKBaseResult() { hasError = true, message = "Error:" + (request.isNetworkError ? " a network error occured while attempting to mint a token." : " a HTTP error occured while attempting to mint a token."), httpCode = request.responseCode });
                    }
                }
            }
        }
    }
#endif
}
