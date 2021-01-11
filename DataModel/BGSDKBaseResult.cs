using System;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class BGSDKBaseResult
    {
        public bool hasError;
        public string message;
        public long httpCode;
        public Exception exception;
    }
}
