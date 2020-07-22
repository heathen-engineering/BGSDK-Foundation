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
        /// <param name="identity"></param>
        /// <param name="pincode"></param>
        /// <param name="alias"></param>
        /// <param name="description"></param>
        /// <param name="identifier"></param>
        /// <param name="secretType"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator CreateWhitelableWallet(Identity identity, string pincode, string alias, string description, string identifier, string secretType, Action<ListWalletResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListWalletResult() { hasError = true, message = "Attempted to call Arkane.Wallets.CreateWhitelabelWallet with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {

                if (identity == null)
                {
                    callback(new ListWalletResult() { hasError = true, message = "ArkaneIdentity required, null identity provided.", result = null });
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
                    www.SetRequestHeader("Authorization", identity.authentication.token_type + " " + identity.authentication.access_token);

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
        /// <param name="identity"></param>
        /// <param name="wallet"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator Unlink(Identity identity, Wallet wallet, Action<ArkaneBaseResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ArkaneBaseResult() { hasError = true, message = "Attempted to call Arkane.Wallets.UserWallet.Unlink with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (identity == null)
                {
                    callback(new ArkaneBaseResult() { hasError = true, message = "ArkaneIdentity required, null identity provided." });
                    yield return null;
                }
                else
                {
                    //TODO: Confirm with Arkane that its the address that should be used. This doesn't appear correct as the value the API expects is a GUID and address is a HEX value
                    UnityWebRequest www = UnityWebRequest.Delete(Settings.current.WalletUri + "/" + wallet.address + "/link");
                    www.SetRequestHeader("Authorization", identity.authentication.token_type + " " + identity.authentication.access_token);

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
        /// Gets the user wallets available to the authorized identity
        /// </summary>
        /// <param name="identity">The identity to query for</param>
        /// <param name="callback">A method pointer to handle the results of the query</param>
        /// <returns></returns>
        /// <remarks>
        /// <see href="https://docs.arkane.network/pages/reference.html#_list_wallets_arkane_api">https://docs.arkane.network/pages/reference.html#_list_wallets_arkane_api</see>
        /// </remarks>
        /// <example>
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// <para>
        /// A simple example class in the form of a MonoBehaviour which authenticates on Start and then returns the available wallets.
        /// </para>
        /// Example Output:
        /// <para>
        /// Has Error: false
        /// Authentication complete.
        /// Has Error: false
        /// Wallet refresh complete.
        /// Wallet A : [address of wallet A]
        /// Wallet B : [address of wallet B]
        /// </para>
        /// </description>
        /// <code>
        /// namespace HeathenEngineering.ArkaneExamples
        /// {
        ///    public class ExampleBehaviour : MonoBehaviour
        ///    {
        ///        public ArkaneSettings settings;
        ///        public ArkaneIdentity Identity = new ArkaneIdentity();
        ///
        ///        private void Start()
        ///        {
        ///            Arkane.Settings = settings;
        ///            StartCoroutine(Arkane.RefreshAuthenticate(Identity, HandleAuthenticationResult));
        ///        }
        ///
        ///        private void HandleAuthenticationResult(AuthenticationResult result)
        ///        {
        ///            Debug.Log("Has Error: " + result.hasError + "\nMessage:" + result.message);
        ///            StartCoroutine(Arkane.Wallets.UserWallet.List(Identity, HandleWalletRefresh));
        ///        }
        ///
        ///        private void HandleWalletRefresh(ListWalletResult result)
        ///        {
        ///            Debug.Log("Has Error: " + result.hasError + "\nMessage:" + result.message);
        ///
        ///            if (!result.hasError)
        ///            {
        ///                foreach (var wallet in result.result)
        ///                {
        ///                    Debug.Log(wallet.description + " : " + wallet.address);
        ///                }
        ///            }
        ///        }
        ///    }
        /// }
        /// </code>
        /// </item>
        /// </list>
        /// </example>
        public static IEnumerator List(Identity identity, Action<ListWalletResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListWalletResult() { hasError = true, message = "Attempted to call Arkane.Wallets.UserWallet.List with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (identity == null)
                {
                    callback(new ListWalletResult() { hasError = true, message = "ArkaneIdentity required, null identity provided.", result = null });
                    yield return null;
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(Settings.current.WalletUri);
                    www.SetRequestHeader("Authorization", identity.authentication.token_type + " " + identity.authentication.access_token);

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
        /// Gets a user wallet as available to the authorized identity
        /// </summary>
        /// <param name="identity">The identity to query for</param>
        /// <param name="callback">A method pointer to handle the results of the query</param>
        /// <returns></returns>
        /// <remarks>
        /// <see href="https://docs.arkane.network/pages/reference.html#get-specific-user-wallet">https://docs.arkane.network/pages/reference.html#get-specific-user-wallet</see>
        /// </remarks>
        /// <example>
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// <para>
        /// A simple example class in the form of a MonoBehaviour which authenticates on Start and then returns the available wallets.
        /// </para>
        /// Example Output:
        /// <para>
        /// Has Error: false
        /// Authentication complete.
        /// Has Error: false
        /// Wallet refresh complete.
        /// Wallet A : [address of wallet A]
        /// Wallet B : [address of wallet B]
        /// </para>
        /// </description>
        /// <code>
        /// namespace HeathenEngineering.ArkaneExamples
        /// {
        ///    public class ExampleBehaviour : MonoBehaviour
        ///    {
        ///        public ArkaneSettings settings;
        ///        public ArkaneIdentity Identity = new ArkaneIdentity();
        ///        public ulong walletId;
        ///
        ///        private void Start()
        ///        {
        ///            Arkane.Settings = settings;
        ///            StartCoroutine(Arkane.RefreshAuthenticate(Identity, HandleAuthenticationResult));
        ///        }
        ///
        ///        private void HandleAuthenticationResult(AuthenticationResult result)
        ///        {
        ///            Debug.Log("Has Error: " + result.hasError + "\nMessage:" + result.message);
        ///            StartCoroutine(Arkane.Wallets.UserWallet.Get(Identity, walletId, HandleWalletRefresh));
        ///        }
        ///
        ///        private void HandleWalletRefresh(ListWalletResult result)
        ///        {
        ///            Debug.Log("Has Error: " + result.hasError + "\nMessage:" + result.message);
        ///
        ///            if (!result.hasError)
        ///            {
        ///                //This will only return 1 wallet but uses the same return responce model as List so we need to loop
        ///                foreach (var wallet in result.result)
        ///                {
        ///                    Debug.Log(wallet.description + " : " + wallet.address);
        ///                }
        ///            }
        ///        }
        ///    }
        /// }
        /// </code>
        /// </item>
        /// </list>
        /// </example>
        public static IEnumerator Get(Identity identity, ulong walletId, Action<ListWalletResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListWalletResult() { hasError = true, message = "Attempted to call Arkane.Wallets.UserWallet.Get with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (identity == null)
                {
                    callback(new ListWalletResult() { hasError = true, message = "ArkaneIdentity required, null identity provided.", result = null });
                    yield return null;
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(Settings.current.WalletUri + "/" + walletId.ToString());
                    www.SetRequestHeader("Authorization", identity.authentication.token_type + " " + identity.authentication.access_token);

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
        /// <param name="identity"></param>
        /// <param name="walletId"></param>
        /// <param name="currentPincode"></param>
        /// <param name="newPincode"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator UpdateWhitelableWalletPincode(Identity identity, ulong walletId, string currentPincode, string newPincode, Action<ListWalletResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListWalletResult() { hasError = true, message = "Attempted to call Arkane.Wallets.CreateWhitelabelWallet with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {

                if (identity == null)
                {
                    callback(new ListWalletResult() { hasError = true, message = "ArkaneIdentity required, null identity provided.", result = null });
                    yield return null;
                }
                else
                {
                    WWWForm form = new WWWForm();
                    form.AddField("pincode", currentPincode);
                    form.AddField("newPincode", newPincode);

                    UnityWebRequest www = UnityWebRequest.Post(Settings.current.WalletUri + "/" + walletId.ToString() + "/security", form);
                    www.SetRequestHeader("Authorization", identity.authentication.token_type + " " + identity.authentication.access_token);

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
        /// <param name="identity"></param>
        /// <param name="walletId"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator NativeBalance(Identity identity, ulong walletId, Action<BalanceResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new BalanceResult() { hasError = true, message = "Attempted to call Arkane.Wallets.UserWallet.NativeBalance with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (identity == null)
                {
                    callback(new BalanceResult() { hasError = true, message = "ArkaneIdentity required, null identity provided.", result = null });
                    yield return null;
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(Settings.current.WalletUri + "/" + walletId.ToString() + "/balance");
                    www.SetRequestHeader("Authorization", identity.authentication.token_type + " " + identity.authentication.access_token);

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
        /// <param name="identity"></param>
        /// <param name="walletId"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator TokenBalance(Identity identity, ulong walletId, Action<ListTokenBalanceResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListTokenBalanceResult() { hasError = true, message = "Attempted to call Arkane.Wallets.UserWallet.TokenBalance with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (identity == null)
                {
                    callback(new ListTokenBalanceResult() { hasError = true, message = "ArkaneIdentity required, null identity provided.", result = null });
                    yield return null;
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(Settings.current.WalletUri + "/" + walletId.ToString() + "/balance/tokens");
                    www.SetRequestHeader("Authorization", identity.authentication.token_type + " " + identity.authentication.access_token);

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
        /// <param name="identity"></param>
        /// <param name="walletId"></param>
        /// <param name="tokenAddress"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator SpecificTokenBalance(Identity identity, ulong walletId, string tokenAddress, Action<TokenBalanceResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new TokenBalanceResult() { hasError = true, message = "Attempted to call Arkane.Wallets.UserWallet.TokenBalance with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (identity == null)
                {
                    callback(new TokenBalanceResult() { hasError = true, message = "ArkaneIdentity required, null identity provided.", result = null });
                    yield return null;
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(Settings.current.WalletUri + "/" + walletId.ToString() + "/balance/tokens/" + tokenAddress);
                    www.SetRequestHeader("Authorization", identity.authentication.token_type + " " + identity.authentication.access_token);

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
        /// <param name="identity"></param>
        /// <param name="walletId"></param>
        /// <param name="optionalContractAddresses">List of contract addresses to filter for, if empty or null all will be returned. Can be null</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator ListNFTs(Identity identity, ulong walletId, List<string> optionalContractAddresses, Action<ListListedNFTTokenResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListListedNFTTokenResult() { hasError = true, message = "Attempted to call Arkane.Wallets.UserWallet.ListNFTs with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (identity == null)
                {
                    callback(new ListListedNFTTokenResult() { hasError = true, message = "ArkaneIdentity required, null identity provided.", result = null });
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
                    www.SetRequestHeader("Authorization", identity.authentication.token_type + " " + identity.authentication.access_token);

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
        /// <param name="identity"></param>
        /// <param name="walletId"></param>
        /// <param name="optionalContractAddresses">List of contract addresses to filter for, if empty or null all will be returned. Can be null</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator GetInventory(Identity identity, ulong walletId, List<string> optionalContractAddresses, Action<ListInventoryResults> callback)
        {
            if (Settings.current == null)
            {
                callback(new ListInventoryResults() { hasError = true, message = "Attempted to call Arkane.Wallets.UserWallet.ListNFTs with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (identity == null)
                {
                    callback(new ListInventoryResults() { hasError = true, message = "ArkaneIdentity required, null identity provided.", result = null });
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
                    www.SetRequestHeader("Authorization", identity.authentication.token_type + " " + identity.authentication.access_token);

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
