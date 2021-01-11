using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using HeathenEngineering.BGSDK.DataModel;
using System;

namespace HeathenEngineering.BGSDK.Engine
{
    public class BlockchainEngineBootstrapper : MonoBehaviour
    {
        public Settings settings;

        void Start()
        {
            Settings.current = settings;

            if(!string.IsNullOrEmpty(settings.AppId.clientSecret))
            {

            }
        }

        /// <summary>
        /// Exchanges a Facebook token for an BGSDK token enabling the client to make future calls against the BGSDK APIs
        /// </summary>
        /// <param name="token">The token provided to you via Facebook authentication</param>
        /// <param name="Callback">Called when the process is complete and indicates rather or not it was successful</param>
        public static IEnumerator ClientSecretAuthentication(Action<AuthenticationResult> callback)
        {
            if (Settings.current == null)
            {
                callback(new AuthenticationResult() { hasError = true, message = "Attempted to call BGSDK.User.Authenticate with no BGSDK.Settings object applied." });
                yield return null;
            }
            else
            {
                WWWForm form = new WWWForm();
                form.AddField("grant_type", "client_credentials");
                form.AddField("client_id", Settings.current.AppId.clientId);
                form.AddField("client_secret", Settings.current.AppId.clientSecret);

                UnityWebRequest www = UnityWebRequest.Post(Settings.current.AuthenticationUri, form);

                var ao = www.SendWebRequest();

                while (!ao.isDone)
                {
                    yield return null;
                }

                if (!www.isNetworkError && !www.isHttpError)
                {
                    string resultContent = www.downloadHandler.text;
                    if (Settings.user == null)
                        Settings.user = new Identity();
                    Settings.user.authentication = JsonUtility.FromJson<AuthenticationResponce>(resultContent);
                    Settings.user.authentication.not_before_policy = resultContent.Contains("not-before-policy:1");
                    Settings.user.authentication.Create();
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
