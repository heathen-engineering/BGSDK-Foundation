using HeathenEngineering.BGSDK.Engine;
using System;
using System.Collections;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace HeathenEngineering.BGSDK.DataModel
{

    /// <summary>
    /// Represents the complete set of Token data as you would recieve from a GetTokenType call
    /// </summary>
    [Serializable]
    public class TokenResponceData : TokenDefinition
    {
        [Tooltip("Address of the deployed token.")]
        public string contractAddress;
        [Tooltip("Internal id of the type")]
        public string id;
        [Tooltip("Blockchain-generated typeId.")]
        public BigInteger contractTypeId;
        [Tooltip("Whether or not the transaction has been confirmed.")]
        public bool confirmed;
        [HideInInspector]
        public string rawData;

        /// <summary>
        /// Synchronious call to fetch <see cref="TokenResponceData"/> for a given token in a given contract
        /// </summary>
        /// <param name="contractId">The id of the contract for which the token belongs</param>
        /// <param name="tokenId">The id of the token to return</param>
        /// <returns></returns>
        public static WebResults<TokenResponceData> GetTokenDefinition(string contractId, string tokenId)
        {
            UnityWebRequest tokenRequest = UnityWebRequest.Get(Settings.current.ConnectUri + "/" + contractId + "/token-types/" + tokenId);
            tokenRequest.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

            var asyncOpHandle = tokenRequest.SendWebRequest();

            while (!asyncOpHandle.isDone)
                ;

            return new WebResults<TokenResponceData>(tokenRequest);
        }

        /// <summary>
        /// An asynchronious and awaitable call to fetch <see cref="TokenResponceData"/> for a given token in a given contract
        /// </summary>
        /// <param name="contractId">The id of the contract for which the token belongs</param>
        /// <param name="tokenId">The id of the token to return</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> for the operation</returns>
        public static async Task<WebResults<TokenResponceData>> GetTokenDefinitionAsync(string contractId, string tokenId)
        {
            return await Task.Run(() => GetTokenDefinition(contractId, tokenId));
        }

        /// <summary>
        /// A coroutine ready call to fetch <see cref="TokenResponceData"/> for a given token in a given contract
        /// </summary>
        /// <param name="contractId">The id of the contract for which the token belongs</param>
        /// <param name="tokenId">The id of the token to return</param>
        /// <param name="callback">An action to be invoked when the call is completed</param>
        /// <returns></returns>
        public static IEnumerator GetTokenDefinitionCoroutine(string contractId, string tokenId, Action<WebResults<TokenResponceData>> callback)
        {
            UnityWebRequest tokenRequest = UnityWebRequest.Get(Settings.current.ConnectUri + "/" + contractId + "/token-types/" + tokenId);
            tokenRequest.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

            var asyncOpHandle = tokenRequest.SendWebRequest();

            while (!asyncOpHandle.isDone)
                yield return null;

            callback?.Invoke(new WebResults<TokenResponceData>(tokenRequest));
        }

        /// <summary>
        /// Synchronious call to fetch <see cref="TokenResponceData"/> for a given token in a given contract
        /// </summary>
        /// <param name="contractId">The id of the contract for which the token belongs</param>
        /// <param name="tokenId">The id of the token to return</param>
        /// <returns></returns>
        public static WebResults<TokenResponceData<T>> GetTokenDefinition<T>(string contractId, string tokenId)
        {
            UnityWebRequest tokenRequest = UnityWebRequest.Get(Settings.current.ConnectUri + "/" + contractId + "/token-types/" + tokenId);
            tokenRequest.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

            var asyncOpHandle = tokenRequest.SendWebRequest();

            while (!asyncOpHandle.isDone)
                ;

            return new WebResults<TokenResponceData<T>>(tokenRequest);
        }

        /// <summary>
        /// An asynchronious and awaitable call to fetch <see cref="TokenResponceData"/> for a given token in a given contract
        /// </summary>
        /// <param name="contractId">The id of the contract for which the token belongs</param>
        /// <param name="tokenId">The id of the token to return</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> for the operation</returns>
        public static async Task<WebResults<TokenResponceData<T>>> GetTokenDefinitionAsync<T>(string contractId, string tokenId)
        {
            return await Task.Run(() => GetTokenDefinition<T>(contractId, tokenId));
        }

        /// <summary>
        /// A coroutine ready call to fetch <see cref="TokenResponceData"/> for a given token in a given contract
        /// </summary>
        /// <param name="contractId">The id of the contract for which the token belongs</param>
        /// <param name="tokenId">The id of the token to return</param>
        /// <param name="callback">An action to be invoked when the call is completed</param>
        /// <returns></returns>
        public static IEnumerator GetTokenDefinitionCoroutine<T>(string contractId, string tokenId, Action<WebResults<TokenResponceData<T>>> callback)
        {
            UnityWebRequest tokenRequest = UnityWebRequest.Get(Settings.current.ConnectUri + "/" + contractId + "/token-types/" + tokenId);
            tokenRequest.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

            var asyncOpHandle = tokenRequest.SendWebRequest();

            while (!asyncOpHandle.isDone)
                yield return null;

            callback?.Invoke(new WebResults<TokenResponceData<T>>(tokenRequest));
        }

        public static TokenResponceData FromJson(string jsonData)
        {
            var def = JsonUtility.FromJson<TokenResponceData>(jsonData);
            return def;
        }

        public static TokenResponceData<T> FromJson<T>(string jsonData)
        {
            var def = JsonUtility.FromJson<TokenResponceData<T>>(jsonData);
            return def;
        }
    }

    [Serializable]
    public class TokenResponceData<T> : TokenResponceData
    {
        public T properties;
    }
}
