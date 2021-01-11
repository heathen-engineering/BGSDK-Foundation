using System;
using UnityEngine;
using UnityEngine.Networking;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class WebResults<T>
    {
        public bool isHttpError;
        public bool isNetworkError;
        public string error;
        public long responseCode;
        public string rawResult;
        public T result;

        public WebResults() { }

        public WebResults(UnityWebRequest request)
        {
            isNetworkError = request.isNetworkError;
            isHttpError = request.isHttpError;
            error = request.error;
            responseCode = request.responseCode;

            if (!isNetworkError && !isNetworkError)
            {
                rawResult = request.downloadHandler.text;
                result = JsonUtility.FromJson<T>(rawResult);
            }
            else
                result = default;
        }
    }
}
