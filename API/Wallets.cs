using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using HeathenEngineering.Arkane.DataModel;
using HeathenEngineering.Arkane.Engine;

namespace HeathenEngineering.Arkane.API
{
    /// <summary>
    /// Wraps the Arkane interface for wallets incuding User, App and Whitelable wallets.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Wallet funcitonality is discribed in the <see href="https://docs.arkane.network/pages/reference.html">https://docs.arkane.network/pages/reference.html</see> documentation.
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
    public static partial class Wallets
    {
        /// <summary>
        /// Create a whitelabel wallet
        /// </summary>
        /// <remarks>
        /// <para>
        /// For more information see <see href="https://docs-staging.arkane.network/pages/whitelabel.html#_create_wallet_arkane_api">https://docs-staging.arkane.network/pages/whitelabel.html#_create_wallet_arkane_api</see>
        /// </para>
        /// </remarks>
        /// <param name="pincode"></param>
        /// <param name="alias"></param>
        /// <param name="description"></param>
        /// <param name="identifier"></param>
        /// <param name="secretType"></param>
        /// <param name="callback"></param>
        /// <returns>The Unity routine enumerator</returns>
        public static IEnumerator CreateWhitelableWallet(string pincode, string alias, string description, string identifier, string secretType, Action<ListWalletResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListWalletResult() { hasError = true, message = "Attempted to call Arkane.Wallets.CreateWhitelabelWallet with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {

                if (Settings.user == null)
                {
                    callback(new ListWalletResult() { hasError = true, message = "ArkaneSettings.user required, null Settings.user provided.\n Please initalize the Settings.user before calling CreateWhitelableWallet", result = null });
                    yield return null;
                }
                else
                {
                    WWWForm form = new WWWForm();
                    form.AddField("pincode", pincode);
                    form.AddField("alias", alias);
                    form.AddField("description", description);
                    form.AddField("identifier", identifier);
                    form.AddField("secretType", secretType);
                    form.AddField("walletType", "WHITE_LABEL");

                    UnityWebRequest www = UnityWebRequest.Post(Settings.current.WalletUri, form); ;
                    www.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

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
                        callback(new ListWalletResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while attempting creat a whitelable wallet." : " a HTTP error occured while attempting to creat a whitelable wallet."), result = null, httpCode = www.responseCode });
                    }
                }
            }
        }

        /// <summary>
        /// This allows applications to unlink a wallet from their app.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For more information see <see href="https://docs.arkane.network/pages/reference.html#_link_wallets_arkane_connect">https://docs.arkane.network/pages/reference.html#_link_wallets_arkane_connect</see>
        /// </para>
        /// </remarks>
        
        /// <param name="wallet"></param>
        /// <param name="callback"></param>
        /// <returns>The Unity routine enumerator</returns>
        public static IEnumerator Unlink(Wallet wallet, Action<ArkaneBaseResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ArkaneBaseResult() { hasError = true, message = "Attempted to call Arkane.Wallets.UserWallet.Unlink with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (Settings.user == null)
                {
                    callback(new ArkaneBaseResult() { hasError = true, message = "ArkaneSettings.user required, null Settings.user provided." });
                    yield return null;
                }
                else
                {
                    //TODO: Confirm with Arkane that its the address that should be used. This doesn't appear correct as the value the API expects is a GUID and address is a HEX value
                    UnityWebRequest www = UnityWebRequest.Delete(Settings.current.WalletUri + "/" + wallet.address + "/link");
                    www.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

                    var co = www.SendWebRequest();
                    while (!co.isDone)
                        yield return null;

                    if (!www.isNetworkError && !www.isHttpError)
                    {
                        callback(new ArkaneBaseResult() { hasError = false, message = "Unlink request completed.", httpCode = www.responseCode });
                    }
                    else
                    {
                        callback(new ArkaneBaseResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while processing an unlink request." : " a HTTP error occured while requesting the user's wallets."), httpCode = www.responseCode });
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
        /// <see href="https://docs.arkane.network/pages/reference.html#_list_wallets_arkane_api">https://docs.arkane.network/pages/reference.html#_list_wallets_arkane_api</see>
        /// </remarks>
        public static IEnumerator List(Action<ListWalletResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListWalletResult() { hasError = true, message = "Attempted to call Arkane.Wallets.UserWallet.List with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (Settings.user == null)
                {
                    callback(new ListWalletResult() { hasError = true, message = "ArkaneSettings.user required, null Settings.user provided.", result = null });
                    yield return null;
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(Settings.current.WalletUri);
                    www.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

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
        /// <see href="https://docs.arkane.network/pages/reference.html#get-specific-user-wallet">https://docs.arkane.network/pages/reference.html#get-specific-user-wallet</see>
        /// </remarks>
        public static IEnumerator Get(ulong walletId, Action<ListWalletResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListWalletResult() { hasError = true, message = "Attempted to call Arkane.Wallets.UserWallet.Get with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (Settings.user == null)
                {
                    callback(new ListWalletResult() { hasError = true, message = "ArkaneSettings.user required, null Settings.user provided.", result = null });
                    yield return null;
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(Settings.current.WalletUri + "/" + walletId.ToString());
                    www.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

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
        /// For more information please see <see href="https://docs-staging.arkane.network/pages/whitelabel.html#_update_wallet_arkane_api">https://docs-staging.arkane.network/pages/whitelabel.html#_update_wallet_arkane_api</see>
        /// </para>
        /// </remarks>
        /// <param name="walletId"></param>
        /// <param name="currentPincode"></param>
        /// <param name="newPincode"></param>
        /// <param name="callback"></param>
        /// <returns>The Unity routine enumerator</returns>
        public static IEnumerator UpdateWhitelableWalletPincode(ulong walletId, string currentPincode, string newPincode, Action<ListWalletResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListWalletResult() { hasError = true, message = "Attempted to call Arkane.Wallets.CreateWhitelabelWallet with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {

                if (Settings.user == null)
                {
                    callback(new ListWalletResult() { hasError = true, message = "ArkaneSettings.user required, null Settings.user provided.", result = null });
                    yield return null;
                }
                else
                {
                    WWWForm form = new WWWForm();
                    form.AddField("pincode", currentPincode);
                    form.AddField("newPincode", newPincode);

                    UnityWebRequest www = UnityWebRequest.Post(Settings.current.WalletUri + "/" + walletId.ToString() + "/security", form);
                    www.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

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
        /// For more information see <see href="https://docs.arkane.network/pages/reference.html#_native_balance_arkane_api">https://docs.arkane.network/pages/reference.html#_native_balance_arkane_api</see>
        /// </remarks>
        /// <param name="walletId"></param>
        /// <param name="callback"></param>
        /// <returns>The Unity routine enumerator</returns>
        public static IEnumerator NativeBalance(ulong walletId, Action<BalanceResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new BalanceResult() { hasError = true, message = "Attempted to call Arkane.Wallets.UserWallet.NativeBalance with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (Settings.user == null)
                {
                    callback(new BalanceResult() { hasError = true, message = "ArkaneSettings.user required, null Settings.user provided.", result = null });
                    yield return null;
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(Settings.current.WalletUri + "/" + walletId.ToString() + "/balance");
                    www.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

                    var co = www.SendWebRequest();
                    while (!co.isDone)
                        yield return null;

                    if (!www.isNetworkError && !www.isHttpError)
                    {
                        var results = new BalanceResult();
                        try
                        {
                            string resultContent = www.downloadHandler.text;
                            results.result = JsonUtility.FromJson<WalletBallance>(resultContent);
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
        /// Returns the balance of all tokens currently supported by Arkane.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For more details see <see href="https://docs.arkane.network/pages/reference.html#_token_balances_arkane_api">https://docs.arkane.network/pages/reference.html#_token_balances_arkane_api</see>
        /// </para>
        /// </remarks>
        /// <param name="walletId"></param>
        /// <param name="callback"></param>
        /// <returns>The Unity routine enumerator</returns>
        public static IEnumerator TokenBalance(ulong walletId, Action<ListTokenBalanceResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListTokenBalanceResult() { hasError = true, message = "Attempted to call Arkane.Wallets.UserWallet.TokenBalance with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (Settings.user == null)
                {
                    callback(new ListTokenBalanceResult() { hasError = true, message = "ArkaneSettings.user required, null Settings.user provided.", result = null });
                    yield return null;
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(Settings.current.WalletUri + "/" + walletId.ToString() + "/balance/tokens");
                    www.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

                    var co = www.SendWebRequest();
                    while (!co.isDone)
                        yield return null;

                    if (!www.isNetworkError && !www.isHttpError)
                    {
                        var results = new ListTokenBalanceResult();
                        try
                        {
                            string resultContent = www.downloadHandler.text;
                            results = JsonUtility.FromJson<ListTokenBalanceResult>(Utilities.JSONArrayWrapper(resultContent));
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
                        callback(new ListTokenBalanceResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while requesting the token balance from a wallet." : " a HTTP error occured while requesting the token balance from a wallet."), result = null, httpCode = www.responseCode });
                    }
                }
            }
        }

        /// <summary>
        /// Returns the token balance for a specified token (this can be any token).
        /// </summary>
        /// <remarks>
        /// <para>
        /// For more details see <see href="https://docs.arkane.network/pages/reference.html#_specific_token_balance_arkane_api">https://docs.arkane.network/pages/reference.html#_specific_token_balance_arkane_api</see>
        /// </para>
        /// </remarks>
        /// <param name="walletId"></param>
        /// <param name="tokenAddress"></param>
        /// <param name="callback"></param>
        /// <returns>The Unity routine enumerator</returns>
        public static IEnumerator SpecificTokenBalance(ulong walletId, string tokenAddress, Action<TokenBalanceResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new TokenBalanceResult() { hasError = true, message = "Attempted to call Arkane.Wallets.UserWallet.TokenBalance with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (Settings.user == null)
                {
                    callback(new TokenBalanceResult() { hasError = true, message = "ArkaneSettings.user required, null Settings.user provided.", result = null });
                    yield return null;
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(Settings.current.WalletUri + "/" + walletId.ToString() + "/balance/tokens/" + tokenAddress);
                    www.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

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
        /// Returns the list of NFT’s owned by a wallet
        /// </summary>
        /// <remarks>
        /// <para>
        /// Currently this functionallity is only supported for Ethereum wallets (ERC-721), other chains will follow
        /// <para>
        /// For more information please see <see href="https://docs.arkane.network/pages/reference.html#_list_non_fungible_tokens_nfts_arkane_api">https://docs.arkane.network/pages/reference.html#_list_non_fungible_tokens_nfts_arkane_api</see>
        /// </para>
        /// </para>
        /// </remarks>
        /// <param name="walletId"></param>
        /// <param name="optionalContractAddresses">List of contract addresses to filter for, if empty or null all will be returned. Can be null</param>
        /// <param name="callback"></param>
        /// <returns>The Unity routine enumerator</returns>
        public static IEnumerator ListNFTs(ulong walletId, List<string> optionalContractAddresses, Action<ListListedNFTTokenResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListListedNFTTokenResult() { hasError = true, message = "Attempted to call Arkane.Wallets.UserWallet.ListNFTs with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (Settings.user == null)
                {
                    callback(new ListListedNFTTokenResult() { hasError = true, message = "ArkaneSettings.user required, null Settings.user provided.", result = null });
                    yield return null;
                }
                else
                {
                    string address = Settings.current.WalletUri + "/" + walletId.ToString() + "/nonfungibles";

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
                    www.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

                    var co = www.SendWebRequest();
                    while (!co.isDone)
                        yield return null;

                    if (!www.isNetworkError && !www.isHttpError)
                    {
                        var results = new ListListedNFTTokenResult();
                        try
                        {
                            string resultContent = www.downloadHandler.text;
                            results = JsonUtility.FromJson<ListListedNFTTokenResult>(Utilities.JSONArrayWrapper(resultContent));
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
                        callback(new ListListedNFTTokenResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while requesting NFTs." : " a HTTP error occured while requesting NFTs."), result = null, httpCode = www.responseCode });
                    }
                }
            }
        }

        /// <summary>
        /// Returns the list of tokens owned by a wallet grouped by contract
        /// </summary>
        /// <remarks>
        /// <para>
        /// Currently this functionallity is only supported for MATIC wallets and items minted by Arkane
        /// <para>
        /// For more information please see <see href="https://docs-staging.arkane.network/pages/reference.html#_get_inventory_arkane_api">https://docs-staging.arkane.network/pages/reference.html#_get_inventory_arkane_api</see>
        /// </para>
        /// </para>
        /// </remarks>
        /// <param name="walletId"></param>
        /// <param name="optionalContractAddresses">List of contract addresses to filter for, if empty or null all will be returned. Can be null</param>
        /// <param name="callback"></param>
        /// <returns>The Unity routine enumerator</returns>
        public static IEnumerator GetInventory(ulong walletId, List<string> optionalContractAddresses, Action<ListInventoryResults> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListInventoryResults() { hasError = true, message = "Attempted to call Arkane.Wallets.UserWallet.ListNFTs with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (Settings.user == null)
                {
                    callback(new ListInventoryResults() { hasError = true, message = "ArkaneSettings.user required, null Settings.user provided.", result = null });
                    yield return null;
                }
                else
                {
                    string address = Settings.current.WalletUri + "/" + walletId.ToString() + "/inventory";

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
                    www.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

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
    }
}
