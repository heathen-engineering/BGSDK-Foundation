#if UNITY_SERVER || UNITY_EDITOR
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using HeathenEngineering.BGSDK.DataModel;
using HeathenEngineering.BGSDK.Engine;
using System.Collections.Generic;

namespace HeathenEngineering.BGSDK.API
{
    public static class Server
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
        public static class Tokens
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
                else if (BGSDKSettings.user == null)
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
        }

        /// <summary>
        /// Wraps the BGSDK interface for wallets incuding User, App and Whitelable wallets.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Wallet funcitonality is discribed in the <see href="https://docs.venly.io/pages/reference.html">https://docs.venly.io/pages/reference.html</see> documentation.
        /// All functions of this class and child classes are designed to be used with Unity's StartCoroutine method.
        /// All funcitons of this class will take an Action as the final paramiter which is called when the process completes.
        /// Actions can be defined as a funciton in the calling script or can be passed as an expression.
        /// </para>
        /// <code>
        /// StartCoroutine(API.Wallets.Get(Settings.user, walletId, HandleResults));
        /// </code>
        /// <para>
        /// or
        /// </para>
        /// <code>
        /// StartCoroutine(API.Wallets.Get(Settings.user, walletId, (resultObject) => 
        /// {
        ///     //TODO: handle the resultObject
        /// }));
        /// </code>
        /// <para>
        /// Additional code samples can be found in the Samples provided with the package.
        /// </para>
        /// </remarks>
        public static class Wallets
        {
            [HideInInspector]
            public class CreateWalletModel
            {
                public string walletType;
                public string secretType;
                public string identifier;
                public string pincode;
            }

            public enum Type
            {
                WHITE_LABEL,
                UNRECOVERABLE_WHITE_LABEL
            }

            /// <summary>
            /// Create a new white label style wallet for the user
            /// </summary>
            /// <remarks>
            /// See <see href="https://docs.venly.io/api/api-products/wallet-api/create-wallet"/> for more details
            /// </remarks>
            /// <param name="pincode">[Required] The pin that will encrypt and decrypt the wallet</param>
            /// <param name="identifier">[Optional] An identifier that can be used to query or group wallets</param>
            /// <param name="description">[Optional] A description to describe the wallet.</param>
            /// <param name="chain">The blockchain on which to create the wallet</param>
            /// <param name="type">Define if the wallet is recoverable or unrecoverable</param>
            /// <param name="callback"></param>
            /// <returns></returns>
            public static IEnumerator Create(string pincode, string identifier, SecretType chain, Action<ListWalletResult> callback)
            {
                if (BGSDKSettings.current == null)
                {
                    callback(new ListWalletResult() { hasError = true, message = "Attempted to call BGSDK.Wallets.CreateWhitelabelWallet with no BGSDK.Settings object applied." });
                    yield return null;
                }
                else
                {

                    if (BGSDKSettings.user == null)
                    {
                        callback(new ListWalletResult() { hasError = true, message = "BGSDKSettings.user required, null Settings.user provided.\n Please initalize the Settings.user before calling CreateWhitelableWallet", result = null });
                        yield return null;
                    }
                    else
                    {
                        var walletModel = new CreateWalletModel
                        {
                            walletType = "WHITE_LABEL",
                            identifier = identifier,
                            pincode = pincode,
                        };

                        switch (chain)
                        {
                            case SecretType.AVAC:
                                walletModel.secretType = "AVAC";
                                break;
                            case SecretType.BSC:
                                walletModel.secretType = "BSC";
                                break;
                            case SecretType.ETHEREUM:
                                walletModel.secretType = "ETHEREUM";
                                break;
                            case SecretType.MATIC:
                                walletModel.secretType = "MATIC";
                                break;
                        }

                        var jsonString = JsonUtility.ToJson(walletModel);

                        UnityWebRequest www = UnityWebRequest.Put(BGSDKSettings.current.WalletUri, jsonString);
                        www.method = UnityWebRequest.kHttpVerbPOST;
                        www.SetRequestHeader("Authorization", BGSDKSettings.user.authentication.token_type + " " + BGSDKSettings.user.authentication.access_token);
                        www.uploadHandler.contentType = "application/json;charset=UTF-8";
                        www.SetRequestHeader("Content-Type", "application/json;charset=UTF-8");

                        var co = www.SendWebRequest();
                        while (!co.isDone)
                            yield return null;

                        if (!www.isNetworkError && !www.isHttpError)
                        {
                            var results = new ListWalletResult();
                            try
                            {
                                string resultContent = www.downloadHandler.text;
                                results.result = new System.Collections.Generic.List<Wallet>();
                                results.result.Add(JsonUtility.FromJson<Wallet>(Utilities.JSONArrayWrapper(resultContent)));
                                results.message = "Create wallet complete.";
                                results.httpCode = www.responseCode;
                            }
                            catch (Exception ex)
                            {
                                results = null;
                                results.message = "An error occured while processing JSON results, see exception for more details.";
                                results.exception = ex;
                                results.httpCode = www.responseCode;
                            }
                            finally
                            {
                                callback(results);
                            }
                        }
                        else
                        {
                            callback(new ListWalletResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while attempting creat wallet." : " a HTTP error occured while attempting to creat wallet."), result = null, httpCode = www.responseCode });
                        }
                    }
                }
            }

            /// <summary>
            /// Gets the user wallets available to the authorized Settings.user
            /// </summary>
            /// <param name="callback">A method pointer to handle the results of the query</param>
            /// <returns>The Unity routine enumerator</returns>
            /// <remarks>
            /// <see href="https://docs.venly.io/pages/reference.html#_list_wallets_arkane_api">https://docs.venly.io/pages/reference.html#_list_wallets_arkane_api</see>
            /// </remarks>
            public static IEnumerator List(Action<ListWalletResult> callback)
            {
                if (BGSDKSettings.current == null)
                {
                    callback(new ListWalletResult() { hasError = true, message = "Attempted to call BGSDK.Wallets.List with no BGSDK.Settings object applied." });
                    yield return null;
                }
                else
                {
                    if (BGSDKSettings.user == null)
                    {
                        callback(new ListWalletResult() { hasError = true, message = "BGSDKSettings.user required, null Settings.user provided.", result = null });
                        yield return null;
                    }
                    else
                    {
                        UnityWebRequest www = UnityWebRequest.Get(BGSDKSettings.current.WalletUri);
                        www.SetRequestHeader("Authorization", BGSDKSettings.user.authentication.token_type + " " + BGSDKSettings.user.authentication.access_token);

                        var co = www.SendWebRequest();
                        while (!co.isDone)
                            yield return null;

                        if (!www.isNetworkError && !www.isHttpError)
                        {
                            var results = new ListWalletResult();
                            try
                            {
                                string resultContent = www.downloadHandler.text;
                                results = JsonUtility.FromJson<ListWalletResult>(Utilities.JSONArrayWrapper(resultContent));
                                results.message = "Wallet refresh complete.";
                                results.httpCode = www.responseCode;

                            }
                            catch (Exception ex)
                            {
                                results = null;
                                results.message = "An error occured while processing JSON results, see exception for more details.";
                                results.exception = ex;
                                results.httpCode = www.responseCode;
                            }
                            finally
                            {
                                callback(results);
                            }
                        }
                        else
                        {
                            callback(new ListWalletResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while requesting the user's wallets." : " a HTTP error occured while requesting the user's wallets."), result = null, httpCode = www.responseCode });
                        }
                    }
                }
            }

            /// <summary>
            /// Gets a user wallet as available to the authorized Settings.user
            /// </summary>
            /// <param name="Settings.user">The Settings.user to query for</param>
            /// <param name="callback">A method pointer to handle the results of the query</param>
            /// <returns>The Unity routine enumerator</returns>
            /// <remarks>
            /// <see href="https://docs.venly.io/pages/reference.html#get-specific-user-wallet">https://docs.venly.io/pages/reference.html#get-specific-user-wallet</see>
            /// </remarks>
            public static IEnumerator Get(string walletId, Action<ListWalletResult> callback)
            {
                if (BGSDKSettings.current == null)
                {
                    callback(new ListWalletResult() { hasError = true, message = "Attempted to call BGSDK.Wallets.UserWallet.Get with no BGSDK.Settings object applied." });
                    yield return null;
                }
                else
                {
                    if (BGSDKSettings.user == null)
                    {
                        callback(new ListWalletResult() { hasError = true, message = "BGSDKSettings.user required, null Settings.user provided.", result = null });
                        yield return null;
                    }
                    else
                    {
                        UnityWebRequest www = UnityWebRequest.Get(BGSDKSettings.current.WalletUri + "/" + walletId);
                        www.SetRequestHeader("Authorization", BGSDKSettings.user.authentication.token_type + " " + BGSDKSettings.user.authentication.access_token);

                        var co = www.SendWebRequest();
                        while (!co.isDone)
                            yield return null;

                        if (!www.isNetworkError && !www.isHttpError)
                        {
                            var results = new ListWalletResult();
                            try
                            {
                                string resultContent = www.downloadHandler.text;
                                results.result = new System.Collections.Generic.List<Wallet>();
                                results.result.Add(JsonUtility.FromJson<Wallet>(resultContent));
                                results.message = "Wallet refresh complete.";
                                results.httpCode = www.responseCode;
                            }
                            catch (Exception ex)
                            {
                                results = null;
                                results.message = "An error occured while processing JSON results, see exception for more details.";
                                results.exception = ex;
                                results.httpCode = www.responseCode;
                            }
                            finally
                            {
                                callback(results);
                            }
                        }
                        else
                        {
                            callback(new ListWalletResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while requesting a user's wallet." : " a HTTP error occured while requesting a user's wallet."), result = null, httpCode = www.responseCode });
                        }
                    }
                }
            }

            /// <summary>
            /// Endpoint that allows updating the details of a wallet (ex. pincode).
            /// </summary>
            /// <remarks>
            /// <para>
            /// For more information please see <see href="https://docs-staging.venly.io/pages/whitelabel.html#_update_wallet_arkane_api">https://docs-staging.venly.io/pages/whitelabel.html#_update_wallet_arkane_api</see>
            /// </para>
            /// </remarks>
            /// <param name="walletId"></param>
            /// <param name="currentPincode"></param>
            /// <param name="newPincode"></param>
            /// <param name="callback"></param>
            /// <returns>The Unity routine enumerator</returns>
            public static IEnumerator UpdatePincode(string walletId, string currentPincode, string newPincode, Action<ListWalletResult> callback)
            {
                if (BGSDKSettings.current == null)
                {
                    callback(new ListWalletResult() { hasError = true, message = "Attempted to call BGSDK.Wallets.CreateWhitelabelWallet with no BGSDK.Settings object applied." });
                    yield return null;
                }
                else
                {

                    if (BGSDKSettings.user == null)
                    {
                        callback(new ListWalletResult() { hasError = true, message = "BGSDKSettings.user required, null Settings.user provided.", result = null });
                        yield return null;
                    }
                    else
                    {
                        WWWForm form = new WWWForm();
                        form.AddField("pincode", currentPincode);
                        form.AddField("newPincode", newPincode);

                        UnityWebRequest www = UnityWebRequest.Post(BGSDKSettings.current.WalletUri + "/" + walletId + "/security", form);
                        www.SetRequestHeader("Authorization", BGSDKSettings.user.authentication.token_type + " " + BGSDKSettings.user.authentication.access_token);

                        var co = www.SendWebRequest();
                        while (!co.isDone)
                            yield return null;

                        if (!www.isNetworkError && !www.isHttpError)
                        {
                            var results = new ListWalletResult();
                            try
                            {
                                string resultContent = www.downloadHandler.text;
                                results.result = new System.Collections.Generic.List<Wallet>();
                                results.result.Add(JsonUtility.FromJson<Wallet>(Utilities.JSONArrayWrapper(resultContent)));
                                results.message = "Update wallet complete.";
                                results.httpCode = www.responseCode;
                            }
                            catch (Exception ex)
                            {
                                results = null;
                                results.message = "An error occured while processing JSON results, see exception for more details.";
                                results.exception = ex;
                                results.httpCode = www.responseCode;
                            }
                            finally
                            {
                                callback(results);
                            }
                        }
                        else
                        {
                            callback(new ListWalletResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while attempting creat a whitelable wallet." : " a HTTP error occured while attempting to creat a whitelable wallet."), result = null, httpCode = www.responseCode });
                        }
                    }
                }
            }

            /// <summary>
            /// Returns the "native" balance for a wallet. This is the balance of the native token used by the chain. Ex. ETH for Ethereum.
            /// </summary>
            /// <remarks>
            /// For more information see <see href="https://docs.venly.io/pages/reference.html#_native_balance_arkane_api">https://docs.venly.io/pages/reference.html#_native_balance_arkane_api</see>
            /// </remarks>
            /// <param name="walletId"></param>
            /// <param name="callback"></param>
            /// <returns>The Unity routine enumerator</returns>
            public static IEnumerator Balance(string walletId, Action<BalanceResult> callback)
            {
                if (BGSDKSettings.current == null)
                {
                    callback(new BalanceResult() { hasError = true, message = "Attempted to call BGSDK.Wallets.UserWallet.NativeBalance with no BGSDK.Settings object applied." });
                    yield return null;
                }
                else
                {
                    if (BGSDKSettings.user == null)
                    {
                        callback(new BalanceResult() { hasError = true, message = "BGSDKSettings.user required, null Settings.user provided.", result = null });
                        yield return null;
                    }
                    else
                    {
                        UnityWebRequest www = UnityWebRequest.Get(BGSDKSettings.current.WalletUri + "/" + walletId + "/balance");
                        www.SetRequestHeader("Authorization", BGSDKSettings.user.authentication.token_type + " " + BGSDKSettings.user.authentication.access_token);

                        var co = www.SendWebRequest();
                        while (!co.isDone)
                            yield return null;

                        if (!www.isNetworkError && !www.isHttpError)
                        {
                            var results = new BalanceResult();
                            try
                            {
                                string resultContent = www.downloadHandler.text;
                                results = JsonUtility.FromJson<BalanceResult>(resultContent);
                                results.message = "Wallet balance updated.";
                                results.httpCode = www.responseCode;
                            }
                            catch (Exception ex)
                            {
                                results = null;
                                results.message = "An error occured while processing JSON results, see exception for more details.";
                                results.exception = ex;
                                results.httpCode = www.responseCode;
                            }
                            finally
                            {
                                callback(results);
                            }
                        }
                        else
                        {
                            callback(new BalanceResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while requesting a user's wallet." : " a HTTP error occured while requesting a user's wallet."), result = null, httpCode = www.responseCode });
                        }
                    }
                }
            }

            /// <summary>
            /// Returns the balance of all tokens currently supported by BGSDK.
            /// </summary>
            /// <remarks>
            /// <para>
            /// For more details see <see href="https://docs.venly.io/pages/reference.html#_token_balances_arkane_api">https://docs.venly.io/pages/reference.html#_token_balances_arkane_api</see>
            /// </para>
            /// </remarks>
            /// <param name="walletId"></param>
            /// <param name="callback"></param>
            /// <returns>The Unity routine enumerator</returns>
            public static IEnumerator TokenBalance(string walletId, Action<TokenBalanceResult> callback)
            {
                if (BGSDKSettings.current == null)
                {
                    callback(new TokenBalanceResult() { hasError = true, message = "Attempted to call BGSDK.Wallets.UserWallet.TokenBalance with no BGSDK.Settings object applied." });
                    yield return null;
                }
                else
                {
                    if (BGSDKSettings.user == null)
                    {
                        callback(new TokenBalanceResult() { hasError = true, message = "BGSDKSettings.user required, null Settings.user provided.", result = null });
                        yield return null;
                    }
                    else
                    {
                        UnityWebRequest www = UnityWebRequest.Get(BGSDKSettings.current.WalletUri + "/" + walletId + "/balance/tokens");
                        www.SetRequestHeader("Authorization", BGSDKSettings.user.authentication.token_type + " " + BGSDKSettings.user.authentication.access_token);

                        var co = www.SendWebRequest();
                        while (!co.isDone)
                            yield return null;

                        if (!www.isNetworkError && !www.isHttpError)
                        {
                            var results = new TokenBalanceResult();
                            try
                            {
                                string resultContent = www.downloadHandler.text;
                                results = JsonUtility.FromJson<TokenBalanceResult>(Utilities.JSONArrayWrapper(resultContent));
                                results.message = "Fetch token balance complete.";
                                results.httpCode = www.responseCode;

                            }
                            catch (Exception ex)
                            {
                                results = null;
                                results.message = "An error occured while processing JSON results, see exception for more details.";
                                results.exception = ex;
                                results.httpCode = www.responseCode;
                            }
                            finally
                            {
                                callback(results);
                            }
                        }
                        else
                        {
                            callback(new TokenBalanceResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while requesting the token balance from a wallet." : " a HTTP error occured while requesting the token balance from a wallet."), result = null, httpCode = www.responseCode });
                        }
                    }
                }
            }

            /// <summary>
            /// Returns the token balance for a specified token (this can be any token).
            /// </summary>
            /// <remarks>
            /// <para>
            /// For more details see <see href="https://docs.venly.io/pages/reference.html#_specific_token_balance_arkane_api">https://docs.venly.io/pages/reference.html#_specific_token_balance_arkane_api</see>
            /// </para>
            /// </remarks>
            /// <param name="walletId"></param>
            /// <param name="tokenAddress"></param>
            /// <param name="callback"></param>
            /// <returns>The Unity routine enumerator</returns>
            public static IEnumerator SpecificTokenBalance(string walletId, string tokenAddress, Action<TokenBalanceResult> callback)
            {
                if (BGSDKSettings.current == null)
                {
                    callback(new TokenBalanceResult() { hasError = true, message = "Attempted to call BGSDK.Wallets.UserWallet.TokenBalance with no BGSDK.Settings object applied." });
                    yield return null;
                }
                else
                {
                    if (BGSDKSettings.user == null)
                    {
                        callback(new TokenBalanceResult() { hasError = true, message = "BGSDKSettings.user required, null Settings.user provided.", result = null });
                        yield return null;
                    }
                    else
                    {
                        UnityWebRequest www = UnityWebRequest.Get(BGSDKSettings.current.WalletUri + "/" + walletId + "/balance/tokens/" + tokenAddress);
                        www.SetRequestHeader("Authorization", BGSDKSettings.user.authentication.token_type + " " + BGSDKSettings.user.authentication.access_token);

                        var co = www.SendWebRequest();
                        while (!co.isDone)
                            yield return null;

                        if (!www.isNetworkError && !www.isHttpError)
                        {
                            var results = new TokenBalanceResult();
                            try
                            {
                                string resultContent = www.downloadHandler.text;
                                results.result = JsonUtility.FromJson<TokenBalance>(resultContent);
                                results.message = "Fetch token balance complete.";
                                results.httpCode = www.responseCode;

                            }
                            catch (Exception ex)
                            {
                                results = null;
                                results.message = "An error occured while processing JSON results, see exception for more details.";
                                results.exception = ex;
                                results.httpCode = www.responseCode;
                            }
                            finally
                            {
                                callback(results);
                            }
                        }
                        else
                        {
                            callback(new TokenBalanceResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while requesting the token balance from a wallet." : " a HTTP error occured while requesting the token balance from a wallet."), result = null, httpCode = www.responseCode });
                        }
                    }
                }
            }

            /// <summary>
            /// NFTs can be queried either by wallet ID or by wallet address, if required multiple NFT contract addresses can be passed as a query parameter to act as a filter. 
            /// </summary>
            /// <remarks>
            /// For more information please see <see href="https://docs.venly.io/api/api-products/wallet-api/retrieve-non-fungible-tokens"/>
            /// </remarks>
            /// <param name="walletId"></param>
            /// <param name="optionalContractAddresses">List of contract addresses to filter for, if empty or null all will be returned. Can be null</param>
            /// <param name="callback"></param>
            /// <returns>The Unity routine enumerator</returns>
            public static IEnumerator NFTs(string walletId, SecretType chain, List<string> optionalContractAddresses, Action<NFTBalanceResult> callback)
            {
                if (BGSDKSettings.current == null)
                {
                    callback(new NFTBalanceResult()
                    {
                        hasError = true,
                        message = "Attempted to call BGSDK.Wallets.UserWallet.ListNFTs with no BGSDK.Settings object applied."
                    });
                    yield return null;
                }
                else
                {
                    if (BGSDKSettings.user == null)
                    {
                        callback(new NFTBalanceResult()
                        {
                            hasError = true,
                            message = "BGSDKSettings.user required, null Settings.user provided."
                        });
                        yield return null;
                    }
                    else
                    {
                        string address = BGSDKSettings.current.WalletUri + "/" + chain.ToString() + "/" + walletId + "/nonfungibles";

                        if (optionalContractAddresses != null && optionalContractAddresses.Count > 0)
                        {
                            address += "?";
                            for (int i = 0; i < optionalContractAddresses.Count; i++)
                            {
                                if (i == 0)
                                    address += "contract-addresses=" + optionalContractAddresses[i];
                                else
                                    address += "&contract-addresses=" + optionalContractAddresses[i];
                            }
                        }

                        UnityWebRequest www = UnityWebRequest.Get(address);
                        www.SetRequestHeader("Authorization", BGSDKSettings.user.authentication.token_type + " " + BGSDKSettings.user.authentication.access_token);

                        var co = www.SendWebRequest();
                        while (!co.isDone)
                            yield return null;

                        if (!www.isNetworkError && !www.isHttpError)
                        {
                            var results = new NFTBalanceResult();
                            try
                            {
                                string resultContent = www.downloadHandler.text;
                                results = JsonUtility.FromJson<NFTBalanceResult>(resultContent);
                                results.message = "List NFTs complete.";
                                results.httpCode = www.responseCode;

                            }
                            catch (Exception ex)
                            {
                                results = null;
                                results.message = "An error occured while processing JSON results, see exception for more details.";
                                results.exception = ex;
                                results.httpCode = www.responseCode;
                            }
                            finally
                            {
                                callback(results);
                            }
                        }
                        else
                        {
                            callback(new NFTBalanceResult()
                            {
                                hasError = true,
                                message = "Error:" + (www.isNetworkError ? " a network error occured while requesting NFTs." : " a HTTP error occured while requesting NFTs."),
                                httpCode = www.responseCode
                            });
                        }
                    }
                }
            }

            #region Legacy
#if false

        /// <param name="wallet"></param>
        /// <param name="callback"></param>
        /// <returns>The Unity routine enumerator</returns>
        public static IEnumerator Unlink(Wallet wallet, Action<BGSDKBaseResult> callback)
        {
            if (BGSDKSettings.current == null)
            {
                callback(new BGSDKBaseResult() { hasError = true, message = "Attempted to call BGSDK.Wallets.UserWallet.Unlink with no BGSDK.Settings object applied." });
                yield return null;
            }
            else
            {
                if (BGSDKSettings.user == null)
                {
                    callback(new BGSDKBaseResult() { hasError = true, message = "BGSDKSettings.user required, null Settings.user provided." });
                    yield return null;
                }
                else
                {
                    //TODO: Confirm with BGSDK that its the address that should be used. This doesn't appear correct as the value the API expects is a GUID and address is a HEX value
                    UnityWebRequest www = UnityWebRequest.Delete(BGSDKSettings.current.WalletUri + "/" + wallet.address + "/link");
                    www.SetRequestHeader("Authorization", BGSDKSettings.user.authentication.token_type + " " + BGSDKSettings.user.authentication.access_token);

                    var co = www.SendWebRequest();
                    while (!co.isDone)
                        yield return null;

                    if (!www.isNetworkError && !www.isHttpError)
                    {
                        callback(new BGSDKBaseResult() { hasError = false, message = "Unlink request completed.", httpCode = www.responseCode });
                    }
                    else
                    {
                        callback(new BGSDKBaseResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while processing an unlink request." : " a HTTP error occured while requesting the user's wallets."), httpCode = www.responseCode });
                    }
                }
            }
        }

        /// <summary>
        /// Returns the list of tokens owned by a wallet grouped by contract
        /// </summary>
        /// <remarks>
        /// <para>
        /// Currently this functionallity is only supported for MATIC wallets and items minted by BGSDK
        /// <para>
        /// For more information please see <see href="https://docs-staging.venly.io/pages/reference.html#_get_inventory_arkane_api">https://docs-staging.venly.io/pages/reference.html#_get_inventory_arkane_api</see>
        /// </para>
        /// </para>
        /// </remarks>
        /// <param name="walletId"></param>
        /// <param name="optionalContractAddresses">List of contract addresses to filter for, if empty or null all will be returned. Can be null</param>
        /// <param name="callback"></param>
        /// <returns>The Unity routine enumerator</returns>
        public static IEnumerator GetInventory(string walletId, List<string> optionalContractAddresses, Action<ListInventoryResults> callback)
        {
            if (BGSDKSettings.current == null)
            {
                callback(new ListInventoryResults() { hasError = true, message = "Attempted to call BGSDK.Wallets.UserWallet.ListNFTs with no BGSDK.Settings object applied." });
                yield return null;
            }
            else
            {
                if (BGSDKSettings.user == null)
                {
                    callback(new ListInventoryResults() { hasError = true, message = "BGSDKSettings.user required, null Settings.user provided.", result = null });
                    yield return null;
                }
                else
                {
                    string address = BGSDKSettings.current.WalletUri + "/" + walletId + "/inventory";

                    if (optionalContractAddresses != null && optionalContractAddresses.Count > 0)
                    {
                        address += "?";
                        for (int i = 0; i < optionalContractAddresses.Count; i++)
                        {
                            if (i == 0)
                                address += "contract-addresses=" + optionalContractAddresses[i];
                            else
                                address += "&contract-addresses=" + optionalContractAddresses[i];
                        }
                    }

                    UnityWebRequest www = UnityWebRequest.Get(address);
                    www.SetRequestHeader("Authorization", BGSDKSettings.user.authentication.token_type + " " + BGSDKSettings.user.authentication.access_token);

                    var co = www.SendWebRequest();
                    while (!co.isDone)
                        yield return null;

                    if (!www.isNetworkError && !www.isHttpError)
                    {
                        var results = new ListInventoryResults();
                        try
                        {
                            string resultContent = www.downloadHandler.text;
                            results = JsonUtility.FromJson<ListInventoryResults>(Utilities.JSONArrayWrapper(resultContent));
                            results.message = "List NFTs complete.";
                            results.httpCode = www.responseCode;

                        }
                        catch (Exception ex)
                        {
                            results = null;
                            results.message = "An error occured while processing JSON results, see exception for more details.";
                            results.exception = ex;
                            results.httpCode = www.responseCode;
                        }
                        finally
                        {
                            callback(results);
                        }
                    }
                    else
                    {
                        callback(new ListInventoryResults() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while requesting inventory." : " a HTTP error occured while requesting inventory."), result = null, httpCode = www.responseCode });
                    }
                }
            }
        }
#endif
            #endregion
        }
    }
}
#endif