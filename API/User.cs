using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using HeathenEngineering.Arkane.DataModel;
using HeathenEngineering.Arkane.Engine;

namespace HeathenEngineering.Arkane.API
{
    public static partial class User
    {
        /// <summary>
        /// Returns more info about the connected user.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Form more information please see <see href="https://docs-staging.arkane.network/pages/reference.html#_user_profile_arkane_api">https://docs-staging.arkane.network/pages/reference.html#_user_profile_arkane_api</see>
        /// </para>
        /// </remarks>
        /// <param name="identity"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator GetProfile(Identity identity, Action<UserProfileResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new UserProfileResult() { hasError = true, message = "Attempted to call Arkane.User.GetProfile with no Arkane.Settings object applied." });
                yield return null;
            }
            else
            {
                if (identity == null)
                {
                    callback(new UserProfileResult() { hasError = true, message = "ArkaneIdentity required, null identity provided.", result = null });
                    yield return null;
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(Settings.current.API[Settings.current.UseStaging] + "/api/profile");
                    www.SetRequestHeader("Authorization", identity.authentication.token_type + " " + identity.authentication.access_token);

                    var co = www.SendWebRequest();
                    while (!co.isDone)
                        yield return null;

                    if (!www.isNetworkError && !www.isHttpError)
                    {
                        var results = new UserProfileResult();
                        try
                        {
                            string resultContent = www.downloadHandler.text;
                            results.result = JsonUtility.FromJson<UserProfile>(resultContent);
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
                        callback(new UserProfileResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while requesting the user's wallets." : " a HTTP error occured while requesting the user's wallets."), result = null, httpCode = www.responseCode });
                    }
                }
            }
        }
    }
}
