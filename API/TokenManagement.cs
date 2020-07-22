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
    /// For more detrails see <see href="https://docs.arkane.network/pages/token-management.html#_token_management">https://docs.arkane.network/pages/token-management.html#_token_management</see>
    /// </remarks>
    public static partial class TokenManagement
    {
        /// <summary>
        /// List the available contracts for the configures app
        /// </summary>
        /// <param name="identity">The Arkane identity that for which you wish to list the available contracts of</param>
        /// <param name="callback">A method that will be called on completion rather success or failure</param>
        /// <returns></returns>
        public static IEnumerator ListContracts(Identity identity, Action<DataModel.ListContractsResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListContractsResult() { hasError = true, message = "Attempted to call Arkane.TokenManagement.ListContracts with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (identity == null)
                {
                    callback(new ListContractsResult() { hasError = true, message = "ArkaneIdentity required, null identity provided.", result = null });
                    yield return null;
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(Settings.current.ContractUri);
                    www.SetRequestHeader("Authorization", identity.authentication.token_type + " " + identity.authentication.access_token);

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
        /// <param name="identity">The users Arkane Identity</param>
        /// <param name="contract">The contract to get</param>
        /// <param name="callback">The method to call back into with the results.</param>
        /// <returns></returns>
        /// <remarks>
        /// <para>
        /// For more information please see <see href="https://docs.arkane.network/pages/token-management.html#_get_contract">https://docs.arkane.network/pages/token-management.html</see>
        /// </para>
        /// </remarks>
        public static IEnumerator GetContract(Identity identity, Engine.Contract contract, Action<DataModel.ContractResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ContractResult() { hasError = true, message = "Attempted to call Arkane.TokenManagement.GetContract with no Arkane.Settings object applied.", result = null });
                yield return null;
            }
            else
            {
                if (identity == null)
                {
                    callback(new ContractResult() { hasError = true, message = "ArkaneIdentity required, null identity provided.", result = null });
                    yield return null;
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(Settings.current.ContractUri + "/" + contract.id);
                    www.SetRequestHeader("Authorization", identity.authentication.token_type + " " + identity.authentication.access_token);

                    var co = www.SendWebRequest();
                    while (!co.isDone)
                        yield return null;

                    if (!www.isNetworkError && !www.isHttpError)
                    {
                        string resultContent = www.downloadHandler.text;
                        var results = new ContractResult();
                        results.result = JsonUtility.FromJson<DataModel.Contract>(resultContent);
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
        /// <param name="identity">The users Arkane Identity</param>
        /// <param name="contract">The contract to get</param>
        /// <param name="callback">The method to call back into with the results.</param>
        /// <returns></returns>
        public static IEnumerator ListTokenTypes(Identity identity, Engine.Contract contract, Action<DataModel.ListTokenTypesResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListTokenTypesResult() { hasError = true, message = "Attempted to call Arkane.TokenManagement.ListTokenTypes with no Arkane.Settings object applied.", result = null });
                yield return null;
            }
            else
            {
                UnityWebRequest www = UnityWebRequest.Get(Settings.current.GetTokenUri(contract));
                www.SetRequestHeader("Authorization", identity.authentication.token_type + " " + identity.authentication.access_token);

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
        /// <param name="identity">The users Arkane Identity</param>
        /// <param name="contract">The contract to get</param>
        /// <param name="tokenId">The id of the token to fetch</param>
        /// <param name="callback">The method to call back into with the results.</param>
        /// <returns></returns>
        public static IEnumerator GetTokenType(Identity identity, Engine.Contract contract, long tokenId, Action<DataModel.DefineTokenTypeResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new DataModel.DefineTokenTypeResult() { hasError = true, message = "Attempted to call Arkane.TokenManagement.GetTokenType with no Arkane.Settings object applied.", result = null });
                yield return null;
            }
            else
            {
                UnityWebRequest www = UnityWebRequest.Get(Settings.current.GetTokenUri(contract) + "/" + tokenId.ToString());
                www.SetRequestHeader("Authorization", identity.authentication.token_type + " " + identity.authentication.access_token);

                var co = www.SendWebRequest();
                while (!co.isDone)
                    yield return null;

                if (!www.isNetworkError && !www.isHttpError)
                {
                    string resultContent = www.downloadHandler.text;
                    var results = new DataModel.DefineTokenTypeResult();
                    results.result = JsonUtility.FromJson<DataModel.Token>(resultContent);
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
        /// TBD: Privileged features only work in restricted environments / with restricted cradentials
        /// Specifics of this restriction are being defined.
        /// </summary>
        public static partial class Privileged
        {
            /// <summary>
            /// Creates an instance of the indicated token 
            /// </summary>
            /// <remarks>
            /// <para>
            /// For more details please see <see href="https://docs.arkane.network/pages/token-management.html#_create_token">https://docs.arkane.network/pages/token-management.html#_create_token</see>
            /// </para>
            /// </remarks>
            /// <param name="identity">The identity of the user to execute this action</param>
            /// <param name="contract">The contract to execute the action against</param>
            /// <param name="token">The token to create an instnace of</param>
            /// <param name="amount">The amount of the token to create </param>
            /// <param name="callback">An action to execute when this process completes.</param>
            /// <returns></returns>
            public static IEnumerator CreateToken(Identity identity, Engine.Contract contract, Engine.Token token, int amount, Action<DataModel.ArkaneBaseResult> callback)
            {
                if (Settings.current == null)
                {
                    callback(new DataModel.ArkaneBaseResult() { hasError = true, message = "Attempted to call Arkane.TokenManagement.CreateToken with no Arkane.Settings object applied." });
                    yield return null;
                }
                else
                {
                    if (identity == null)
                    {
                        callback(new DataModel.ArkaneBaseResult() { hasError = true, message = "ArkaneIdentity required, null identity provided." });
                        yield return null;
                    }
                    else
                    {
                        WWWForm form = new WWWForm();
                        form.AddField("amount", amount);
                        form.AddField("typeId", token.Data.id.ToString());

                        UnityWebRequest www = UnityWebRequest.Post(Settings.current.CreateTokenUri(contract), form);
                        www.SetRequestHeader("Authorization", identity.authentication.token_type + " " + identity.authentication.access_token);

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
}
