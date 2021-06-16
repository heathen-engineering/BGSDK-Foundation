using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using HeathenEngineering.BGSDK.DataModel;
using HeathenEngineering.BGSDK.Engine;

namespace HeathenEngineering.BGSDK.API
{
    /// <summary>
    /// A wrapper around BGSDK's Token Management API
    /// </summary>
    /// <remarks>
    /// <para>
    /// For more detrails see <see href="https://docs.venly.io/pages/token-management.html#_token_management">https://docs.venly.io/pages/token-management.html#_token_management</see>.
    /// All functions of this class and child classes are designed to be used with Unity's StartCoroutine method.
    /// All funcitons of this class will take an Action as the final paramiter which is called when the process completes.
    /// Actions can be defined as a funciton in the calling script or can be passed as an expression.
    /// </para>
    /// <code>
    /// StartCoroutine(API.Tokens.GetContract(Identity, contract, HandleGetContractResults));
    /// </code>
    /// <para>
    /// or
    /// </para>
    /// <code>
    /// StartCoroutine(API.Tokens.GetContract(Identity, contract, (resultObject) => 
    /// {
    ///     //TODO: handle the resultObject
    /// }));
    /// </code>
    /// <para>
    /// Additional code samples can be found in the Samples provided with the package.
    /// </para>
    /// </remarks>
    public static partial class Tokens
    {
        /// <summary>
        /// Fetch details about a specific contract for the current app Id
        /// </summary>
        /// <param name="contract">The contract to get</param>
        /// <param name="callback">The method to call back into with the results.</param>
        /// <returns>The Unity routine enumerator</returns>
        /// <remarks>
        /// <para>
        /// For more information please see <see href="https://docs.venly.io/pages/token-management.html#_get_contract">https://docs.venly.io/pages/token-management.html</see>
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// How to call:
        /// </para>
        /// <code>
        /// StartCoroutine(API.Tokens.GetContract(Identity, contract, HandleGetContractResults));
        /// </code>
        /// </example>
        public static IEnumerator GetContract(Engine.Contract contract, Action<ContractResult> callback)
        {
            if (BGSDKSettings.current == null)
            {
                callback(new ContractResult() { hasError = true, message = "Attempted to call BGSDK.Tokens.GetContract with no BGSDK.Settings object applied.", result = null });
                yield return null;
            }
            else
            {
                if (BGSDKSettings.user == null)
                {
                    callback(new ContractResult() { hasError = true, message = "BGSDKIdentity required, null identity provided.\nPlease initalize the Settings.user variable before calling GetContract", result = null });
                    yield return null;
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(BGSDKSettings.current.ContractUri + "/" + contract.Id);
                    www.SetRequestHeader("Authorization", BGSDKSettings.user.authentication.token_type + " " + BGSDKSettings.user.authentication.access_token);

                    var co = www.SendWebRequest();
                    while (!co.isDone)
                        yield return null;

                    if (!www.isNetworkError && !www.isHttpError)
                    {
                        string resultContent = www.downloadHandler.text;
                        var results = new ContractResult();
                        results.result = JsonUtility.FromJson<ListContractsResult.RecieveContractModel>(resultContent)?.ToContractData();
                        results.message = "Get Contract complete.";
                        results.httpCode = www.responseCode;
                        callback(results);
                    }
                    else
                    {
                        callback(new ContractResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while requesting the contract." : " a HTTP error occured while requesting the contract."), result = null, httpCode = www.responseCode });
                    }
                }
            }
        }

        /// <summary>
        /// <para>Returns the list of available token types for the indicated contract</para>
        /// <see href="https://docs.venly.io/pages/token-management.html">https://docs.venly.io/pages/token-management.html</see>
        /// </summary>
        /// <param name="contract">The contract to get</param>
        /// <param name="callback">The method to call back into with the results.</param>
        /// <returns>The Unity routine enumerator</returns>
        /// <example>
        /// <para>
        /// How to call:
        /// </para>
        /// <code>
        /// StartCoroutine(API.Tokens.ListTokenTypes(Identity, contract, HandleListTokenTypeResults));
        /// </code>
        /// </example>
        public static IEnumerator ListTokenTypes(Contract contract, Action<ListTokenTypesResult> callback)
        {
            if (BGSDKSettings.current == null)
            {
                callback(new ListTokenTypesResult() { hasError = true, message = "Attempted to call BGSDK.Tokens.ListTokenTypes with no BGSDK.Settings object applied.", result = null });
                yield return null;
            }
            else if(BGSDKSettings.user == null)
            {
                callback(new ListTokenTypesResult() { hasError = true, message = "BGSDKIdentity required, null identity provided.\nPlease initalize the Settings.user variable before calling ListTokenTypes", result = null });
                yield return null;
            }
            else
            {
                UnityWebRequest www = UnityWebRequest.Get(BGSDKSettings.current.GetTokenUri(contract));
                www.SetRequestHeader("Authorization", BGSDKSettings.user.authentication.token_type + " " + BGSDKSettings.user.authentication.access_token);

                var co = www.SendWebRequest();
                while (!co.isDone)
                    yield return null;

                if (!www.isNetworkError && !www.isHttpError)
                {
                    string resultContent = www.downloadHandler.text;
                    var results = new ListTokenTypesResult();
                    results = JsonUtility.FromJson<ListTokenTypesResult>(Utilities.JSONArrayWrapper(resultContent));
                    results.message = "List Token Types complete.";
                    results.httpCode = www.responseCode;
                    callback(results);
                }
                else
                {
                    callback(new ListTokenTypesResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while requesting the list of available token types." : " a HTTP error occured while requesting the list of availabel token types."), result = null, httpCode = www.responseCode });
                }
            }
        }

        /// <summary>
        /// <para>Returns the definition of the indicated token</para>
        /// <see href="https://docs.venly.io/pages/token-management.html">https://docs.venly.io/pages/token-management.html</see>
        /// </summary>
        /// <param name="contract">The contract to get</param>
        /// <param name="tokenId">The id of the token to fetch</param>
        /// <param name="callback">The method to call back into with the results.</param>
        /// <returns>The Unity routine enumerator</returns>
        /// <example>
        /// <para>
        /// How to call:
        /// </para>
        /// <code>
        /// StartCoroutine(API.Tokens.GetTokenType(Identity, contract, tokenId, HandleGetTokenTypeResults));
        /// </code>
        /// </example>
        public static IEnumerator GetTokenType(Contract contract, string tokenId, Action<DataModel.DefineTokenTypeResult> callback)
        {
            if (BGSDKSettings.current == null)
            {
                callback(new DataModel.DefineTokenTypeResult() { hasError = true, message = "Attempted to call BGSDK.Tokens.GetTokenType with no BGSDK.Settings object applied.", result = null });
                yield return null;
            }
            else if (BGSDKSettings.user == null)
            {
                callback(new DefineTokenTypeResult() { hasError = true, message = "BGSDKIdentity required, null identity provided.\nPlease initalize the Settings.user variable before calling GetTokenType", result = null });
                yield return null;
            }
            else
            {
                UnityWebRequest www = UnityWebRequest.Get(BGSDKSettings.current.GetTokenUri(contract) + "/" + tokenId);
                www.SetRequestHeader("Authorization", BGSDKSettings.user.authentication.token_type + " " + BGSDKSettings.user.authentication.access_token);

                var co = www.SendWebRequest();
                while (!co.isDone)
                    yield return null;

                if (!www.isNetworkError && !www.isHttpError)
                {
                    string resultContent = www.downloadHandler.text;
                    var results = new DataModel.DefineTokenTypeResult();
                    results.result = JsonUtility.FromJson<DataModel.TokenResponceData>(resultContent);
                    results.message = "List Token Types complete.";
                    results.httpCode = www.responseCode;
                    callback(results);
                }
                else
                {
                    callback(new DataModel.DefineTokenTypeResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while requesting the definition of token " + tokenId + "." : " a HTTP error occured while requesting the definition of token " + tokenId + "."), result = null, httpCode = www.responseCode });
                }
            }
        }

        /// <summary>
        /// Returns tokens for given token type
        /// </summary>
        /// <param name="token">The token type to query</param>
        /// <param name="callback">The callback to invoke with the results</param>
        /// <returns></returns>
        public static IEnumerator GetTokens(Token token, Action<Token.ResultList> callback) => token.Get(callback);

#if UNITY_EDITOR
    }
#endif
}
