using System;
using System.Collections.Generic;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class ListTokenTypesResult : BGSDKBaseResult
    {
        public List<TokenCreateResponceData> result;
    }
}
