using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using HeathenEngineering.Arkane.DataModel;
using HeathenEngineering.Arkane.Engine;

namespace HeathenEngineering.Arkane.API
{
    /// <summary>
    /// A wrapper around Arkane's Token Management API
    /// </summary>
    /// <remarks>
    /// <para>
    /// For more detrails see <see href="https://docs.arkane.network/pages/token-management.html#_token_management">https://docs.arkane.network/pages/token-management.html#_token_management</see>.
    /// All functions of this class and child classes are designed to be used with Unity's StartCoroutine method.
    /// All funcitons of this class will take an Action as the final paramiter which is called when the process completes.
    /// Actions can be defined as a funciton in the calling script or can be passed as an expression.
    /// </para>
    /// <code>
    /// StartCoroutine(API.TokenManagement.GetContract(Identity, contract, HandleGetContractResults));
    /// </code>
    /// <para>
    /// or
    /// </para>
    /// <code>
    /// StartCoroutine(API.TokenManagement.GetContract(Identity, contract, (resultObject) => 
    /// {
    ///     //TODO: handle the resultObject
    /// }));
    /// </code>
    /// <para>
    /// Additional code samples can be found in the Samples provided with the package.
    /// </para>
    /// </remarks>
    public static partial class TokenManagement
    {
        /// <summary>
        /// List the available contracts for the configured app.
        /// </summary>
        /// <param name="callback">A method that will be called on completion rather success or failure</param>
        /// <returns>The Unity routine enumerator</returns>
        /// <remarks>
        /// <para>
        /// On completion the callback will be invoked passing a <see cref="ListContractsResult"/> object as a paramiter.
        /// The <see cref="ListContractsResult.result"/> is a collection of <see cref="ContractData"/> objects.
        /// For more information please see <see href="https://docs.arkane.network/pages/token-management.html#_list_contracts">https://docs.arkane.network/pages/token-management.html</see>
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// How to call:
        /// </para>
        /// <code>
        /// StartCoroutine(HeathenEngineering.Arkane.API.TokenManagement.ListContracts(Identity, HandleListContractResults));
        /// </code>
        /// </example>
        public static IEnumerator ListContracts(Action<DataModel.ListContractsResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListContractsResult() { hasError = true, message = "Attempted to call Arkane.TokenManagement.ListContracts with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (Settings.user == null)
                {
                    callback(new ListContractsResult() { hasError = true, message = "ArkaneIdentity required, null identity provided.\nPlease initalize the Settings.user variable before calling ListContracts.", result = null });
                    yield return null;
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(Settings.current.ContractUri);
                    www.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

                    var co = www.SendWebRequest();
                    while (!co.isDone)
                        yield return null;

                    if (!www.isNetworkError && !www.isHttpError)
                    {
                        string resultContent = www.downloadHandler.text;
                        var results = JsonUtility.FromJson<ListContractsResult>(resultContent);
                        results.message = "List Contracts request complete.";
                        results.httpCode = www.responseCode;
                        callback(results);
                    }
                    else
                    {
                        callback(new ListContractsResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while requesting the user's wallets." : " a HTTP error occured while requesting the user's wallets."), result = null, httpCode = www.responseCode });
                    }
                }
            }
        }

        /// <summary>
        /// Fetch details about a specific contract for the current app Id
        /// </summary>
        /// <param name="contract">The contract to get</param>
        /// <param name="callback">The method to call back into with the results.</param>
        /// <returns>The Unity routine enumerator</returns>
        /// <remarks>
        /// <para>
        /// For more information please see <see href="https://docs.arkane.network/pages/token-management.html#_get_contract">https://docs.arkane.network/pages/token-management.html</see>
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// How to call:
        /// </para>
        /// <code>
        /// StartCoroutine(API.TokenManagement.GetContract(Identity, contract, HandleGetContractResults));
        /// </code>
        /// </example>
        public static IEnumerator GetContract(Engine.Contract contract, Action<ContractResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ContractResult() { hasError = true, message = "Attempted to call Arkane.TokenManagement.GetContract with no Arkane.Settings object applied.", result = null });
                yield return null;
            }
            else
            {
                if (Settings.user == null)
                {
                    callback(new ContractResult() { hasError = true, message = "ArkaneIdentity required, null identity provided.\nPlease initalize the Settings.user variable before calling GetContract", result = null });
                    yield return null;
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(Settings.current.ContractUri + "/" + contract.id);
                    www.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

                    var co = www.SendWebRequest();
                    while (!co.isDone)
                        yield return null;

                    if (!www.isNetworkError && !www.isHttpError)
                    {
                        string resultContent = www.downloadHandler.text;
                        var results = new ContractResult();
                        results.result = JsonUtility.FromJson<DataModel.ContractData>(resultContent);
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
        /// <see href="https://docs.arkane.network/pages/token-management.html">https://docs.arkane.network/pages/token-management.html</see>
        /// </summary>
        /// <param name="contract">The contract to get</param>
        /// <param name="callback">The method to call back into with the results.</param>
        /// <returns>The Unity routine enumerator</returns>
        /// <example>
        /// <para>
        /// How to call:
        /// </para>
        /// <code>
        /// StartCoroutine(API.TokenManagement.ListTokenTypes(Identity, contract, HandleListTokenTypeResults));
        /// </code>
        /// </example>
        public static IEnumerator ListTokenTypes(Contract contract, Action<ListTokenTypesResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListTokenTypesResult() { hasError = true, message = "Attempted to call Arkane.TokenManagement.ListTokenTypes with no Arkane.Settings object applied.", result = null });
                yield return null;
            }
            else if(Settings.user == null)
            {
                callback(new ListTokenTypesResult() { hasError = true, message = "ArkaneIdentity required, null identity provided.\nPlease initalize the Settings.user variable before calling ListTokenTypes", result = null });
                yield return null;
            }
            else
            {
                UnityWebRequest www = UnityWebRequest.Get(Settings.current.GetTokenUri(contract));
                www.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

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
        /// <see href="https://docs.arkane.network/pages/token-management.html">https://docs.arkane.network/pages/token-management.html</see>
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
        /// StartCoroutine(API.TokenManagement.GetTokenType(Identity, contract, tokenId, HandleGetTokenTypeResults));
        /// </code>
        /// </example>
        public static IEnumerator GetTokenType(Contract contract, long tokenId, Action<DataModel.DefineTokenTypeResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new DataModel.DefineTokenTypeResult() { hasError = true, message = "Attempted to call Arkane.TokenManagement.GetTokenType with no Arkane.Settings object applied.", result = null });
                yield return null;
            }
            else if (Settings.user == null)
            {
                callback(new DefineTokenTypeResult() { hasError = true, message = "ArkaneIdentity required, null identity provided.\nPlease initalize the Settings.user variable before calling GetTokenType", result = null });
                yield return null;
            }
            else
            {
                UnityWebRequest www = UnityWebRequest.Get(Settings.current.GetTokenUri(contract) + "/" + tokenId.ToString());
                www.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

                var co = www.SendWebRequest();
                while (!co.isDone)
                    yield return null;

                if (!www.isNetworkError && !www.isHttpError)
                {
                    string resultContent = www.downloadHandler.text;
                    var results = new DataModel.DefineTokenTypeResult();
                    results.result = JsonUtility.FromJson<DataModel.TokenData>(resultContent);
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

#if UNITY_EDITOR || UNITY_SERVER
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
        public static partial class Privileged
        {
            /// <summary>
            /// Creates an instance of the indicated token 
            /// </summary>
            /// <remarks>
            /// <para>
            /// For more details please see <see href="https://docs.arkane.network/pages/token-management.html#_create_token">https://docs.arkane.network/pages/token-management.html#_create_token</see>
            /// </para>
            /// <para>
            /// This requests an instance of the <paramref name="token"/> be generated, the new instance will be owned by the server or developer wallet assoceated with the Arkane app and can then be transfered to the appropreate user.
            /// </para>
            /// </remarks>
            /// <param name="identity">The identity of the user to execute this action</param>
            /// <param name="contract">The contract to execute the action against</param>
            /// <param name="token">The token to create an instnace of</param>
            /// <param name="amount">The amount of the token to create </param>
            /// <param name="callback">An action to execute when this process completes.</param>
            /// <returns>The Unity routine enumerator</returns>
            public static IEnumerator CreateToken(Contract contract, Token token, int amount, Action<ArkaneBaseResult> callback)
            {
                if (Settings.current == null)
                {
                    callback(new DataModel.ArkaneBaseResult() { hasError = true, message = "Attempted to call Arkane.TokenManagement.CreateToken with no Arkane.Settings object applied." });
                    yield return null;
                }
                else
                {
                    if (Settings.user == null)
                    {
                        callback(new DataModel.ArkaneBaseResult() { hasError = true, message = "ArkaneIdentity required, null identity provided.\nPlease initalize Settings.user before calling Privileged.CreateToken" });
                        yield return null;
                    }
                    else
                    {
                        WWWForm form = new WWWForm();
                        form.AddField("amount", amount);
                        form.AddField("typeId", token.Data.id.ToString());

                        UnityWebRequest www = UnityWebRequest.Post(Settings.current.CreateTokenUri(contract), form);
                        www.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

                        var co = www.SendWebRequest();
                        while (!co.isDone)
                            yield return null;

                        if (!www.isNetworkError && !www.isHttpError)
                        {
                            callback(new DataModel.ArkaneBaseResult() { hasError = false, message = "Successful request to create token!", httpCode = www.responseCode });
                        }
                        else
                        {
                            callback(new DataModel.ArkaneBaseResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while attempting to create a token." : " a HTTP error occured while attempting to create a token."), httpCode = www.responseCode });
                        }
                    }
                }
            }
        }
    }
#endif
}
