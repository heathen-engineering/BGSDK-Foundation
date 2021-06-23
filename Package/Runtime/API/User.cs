using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using HeathenEngineering.BGSDK.DataModel;
using HeathenEngineering.BGSDK.Engine;

namespace HeathenEngineering.BGSDK.API
{
    public static class Client
    {
        /// <summary>
        /// A wrapper around BGSDK's User API
        /// </summary>
        /// <remarks>
        /// <para>
        /// For more detrails see <see href="https://docs-staging.venly.io/pages/reference.html#_user_profile_arkane_api">https://docs-staging.venly.io/pages/reference.html#_user_profile_arkane_api</see>.
        /// All functions of this class and child classes are designed to be used with Unity's StartCoroutine method.
        /// All funcitons of this class will take an Action as the final paramiter which is called when the process completes.
        /// Actions can be defined as a funciton in the calling script or can be passed as an expression.
        /// </para>
        /// <code>
        /// StartCoroutine(API.User.GetProfile(Identity, HandleProfileResult));
        /// </code>
        /// <para>
        /// or
        /// </para>
        /// <code>
        /// StartCoroutine(API.User.GetProfile(Identity, (resultObject) => 
        /// {
        ///     //TODO: handle the resultObject
        /// }));
        /// </code>
        /// <para>
        /// Additional code samples can be found in the Samples provided with the package.
        /// </para>
        /// </remarks>
        public static class User
        {
            /// <summary>
            /// Returns more info about the connected user.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Form more information please see <see href="https://docs-staging.venly.io/pages/reference.html#_user_profile_arkane_api">https://docs-staging.venly.io/pages/reference.html#_user_profile_arkane_api</see>
            /// </para>
            /// <para>
            /// The <see cref="UserProfileResult.result"/> is a <see cref="UserProfile"/> object containing details about the local user.
            /// This method assumes you have already authenticated via one of the available login methods such as <see cref="Login_Facebook(string, Action{AuthenticationResult})"/>
            /// </para>
            /// </remarks>
            /// <param name="callback"></param>
            /// <returns>The Unity routine enumerator</returns>
            /// <example>
            /// <para>
            /// How to call:
            /// </para>
            /// <code>
            /// StartCoroutine(HeathenEngineering.BGSDK.API.User.GetProfile(Identity, HandleProfileResult));
            /// </code>
            /// </example>
            public static IEnumerator GetProfile(Action<UserProfileResult> callback)
            {
                if (BGSDKSettings.current == null)
                {
                    callback(new UserProfileResult() { hasError = true, message = "Attempted to call BGSDK.User.GetProfile with no BGSDK.Settings object applied." });
                    yield return null;
                }
                else
                {
                    if (BGSDKSettings.user == null)
                    {
                        callback(new UserProfileResult() { hasError = true, message = "BGSDKIdentity required, null identity provided.\nPlease initalize Settings.user before calling GetProfile", result = null });
                        yield return null;
                    }
                    else
                    {
                        UnityWebRequest www = UnityWebRequest.Get(BGSDKSettings.current.api[BGSDKSettings.current.UseStaging] + "/api/profile");
                        www.SetRequestHeader("Authorization", BGSDKSettings.user.authentication.token_type + " " + BGSDKSettings.user.authentication.access_token);

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

            /// <summary>
            /// Records the authentication details aquired from some 3rd party source such as a trusted server
            /// </summary>
            /// <remarks>
            /// <para>
            /// This can be used when you have a trusted sources such as your game server or a 3rd party backend service provider handle user authentication against BGSDK.
            /// In this model you would gather the nessisary information from the user at the client level and would send this securly to your trusted 3rd party source.
            /// The 3rd party source would then perform any nessisary validation and finally authenticate against BGSDK's API aquiring the access token, expires in and refresh token/expires in values.
            /// Once the 3rd party has aquired this information it can securly pass it back to the client where you can feed that information into this method to enable this user to make further calls against BGSDK directly.
            /// </para>
            /// </remarks>
            /// <param name="createdAt">The date and time the token request was submited by the 3rd party source</param>
            /// <param name="accessToken">The token the 3rd party source aquired</param>
            /// <param name="expiresIn">The expires in value the 3rd party source aquired ... this is the number of seconds from <paramref name="createdAt"/> before the <paramref name="accessToken"/> expires.</param>
            /// <param name="refreshToken">The refresh token the 3rd party source aquired</param>
            /// <param name="refreshExpiresIn">The expires in value the 3rd party source aquired for the refresh token. this is the number of seconds from <paramref name="createdAt"/> before the <paramref name="refreshToken"/> expires.</param>
            public static void Login_3rdPartyAuthentication(DateTime createdAt, string accessToken, int expiresIn, string refreshToken, int refreshExpiresIn)
            {
                BGSDKSettings.user = new Identity()
                {
                    authentication = new AuthenticationResponce
                    {
                        CreatedAt = createdAt,
                        expires_in = expiresIn,
                        access_token = accessToken,
                        refresh_token = refreshToken,
                        refresh_expires_in = refreshExpiresIn,
                    }
                };
            }

            /// <summary>
            /// Exchanges a Facebook token for an BGSDK token enabling the client to make future calls against the BGSDK API
            /// </summary>
            /// <param name="token">The token provided to you via Facebook authentication</param>
            /// <param name="Callback">Called when the process is complete and indicates rather or not it was successful</param>
            public static IEnumerator Login_Facebook(string token, Action<AuthenticationResult> callback)
            {
                if (BGSDKSettings.current == null)
                {
                    callback(new AuthenticationResult() { hasError = true, message = "Attempted to call BGSDK.User.Login_Facebook with no BGSDK.Settings object applied." });
                    yield return null;
                }
                else
                {
                    WWWForm form = new WWWForm();
                    form.AddField("identityProvider", "FACEBOOK");
                    form.AddField("idpToken", token);
                    form.AddField("client_id", BGSDKSettings.current.appId.clientId);

                    UnityWebRequest www = UnityWebRequest.Post(BGSDKSettings.current.AuthenticationUri, form);

                    var ao = www.SendWebRequest();

                    while (!ao.isDone)
                    {
                        yield return null;
                    }

                    if (!www.isNetworkError && !www.isHttpError)
                    {
                        string resultContent = www.downloadHandler.text;
                        BGSDKSettings.user.authentication = JsonUtility.FromJson<AuthenticationResponce>(resultContent);
                        BGSDKSettings.user.authentication.not_before_policy = resultContent.Contains("not-before-policy:1");
                        BGSDKSettings.user.authentication.Create();
                        callback(new AuthenticationResult() { hasError = false, message = "Authentication complete.", httpCode = www.responseCode });
                    }
                    else
                    {
                        callback(new AuthenticationResult() { hasError = true, message = (www.isNetworkError ? "Error on authentication: Network Error." : "Error on authentication: HTTP Error.") + "\n" + www.error, httpCode = www.responseCode });
                    }
                }
            }
        }
    }
}
