using System;

namespace HeathenEngineering.Arkane.DataModel
{
    [Serializable]
    public class ArkaneBaseResult
    {
        public bool hasError;
        public string message;
        public long httpCode;
        public Exception exception;
    }
}
