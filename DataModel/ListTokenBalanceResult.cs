using System;
using System.Collections.Generic;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class ListTokenBalanceResult : BGSDKBaseResult
    {
        public List<TokenBalance> result;
    }
}
