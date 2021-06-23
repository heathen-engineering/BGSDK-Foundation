using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using HeathenEngineering.BGSDK.DataModel;
using System;

namespace HeathenEngineering.BGSDK.Engine
{
    [AddComponentMenu("BGSDK Behaviour")]
    public class BGSDKBehaviour : MonoBehaviour
    {
        public BGSDKSettings settings;
#if UNITY_SERVER || UNITY_EDITOR
        public bool LoginClientSecret = false;
#endif

        [Header("Events")]
        public BGSDKAuthenticationResultEvent authenticationResponce;

        void Start()
        {
            BGSDKSettings.current = settings;
#if UNITY_SERVER || UNITY_EDITOR
            if (LoginClientSecret && !string.IsNullOrEmpty(settings.appId.clientSecret))
            {
                StartCoroutine(ClientSecretAuthentication(HandleSecretAuthenticationResponce));
            }
#endif
        }

        #region Server
#if UNITY_SERVER || UNITY_EDITOR
        /// <summary>
        /// Logs in via the client secret ... this is only usable by servers and the Unity Editor and will not have an user data
        /// </summary>
        /// <param name="token">The token provided to you via Facebook authentication</param>
        /// <param name="Callback">Called when the process is complete and indicates rather or not it was successful</param>
        public static IEnumerator ClientSecretAuthentication(Action<AuthenticationResult> callback)
        {
            if (BGSDKSettings.current == null)
            {
                callback(new AuthenticationResult() { hasError = true, message = "Attempted to call BGSDK.User.Authenticate with no BGSDK.Settings object applied." });
                yield return null;
            }
            else
            {
                WWWForm form = new WWWForm();
                form.AddField("grant_type", "client_credentials");
                form.AddField("client_id", BGSDKSettings.current.appId.clientId);
                form.AddField("client_secret", BGSDKSettings.current.appId.clientSecret);

                UnityWebRequest www = UnityWebRequest.Post(BGSDKSettings.current.AuthenticationUri, form);

                var ao = www.SendWebRequest();

                while (!ao.isDone)
                {
                    yield return null;
                }

                if (!www.isNetworkError && !www.isHttpError)
                {
                    string resultContent = www.downloadHandler.text;
                    if (BGSDKSettings.user == null)
                        BGSDKSettings.user = new Identity();
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

        private void HandleSecretAuthenticationResponce(AuthenticationResult authResult)
        {
            if (authResult.hasError)
            {
                Debug.LogError("Authentication Result:\nError " + authResult.message);
            }
            else
            {
                Debug.Log("Authentication Complete");
            }

            authenticationResponce.Invoke(authResult);
        }
#endif
        #endregion

        #region Client
#if !UNITY_SERVER
        public void FacebookLogin(string FacebookToken)
        {
            API.Client.User.Login_Facebook(FacebookToken, HandleSecretAuthenticationResponce);
        }
#endif
        #endregion
    }
}
