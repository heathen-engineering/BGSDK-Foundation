using System;
using System.Collections.Generic;

namespace HeathenEngineering.Arkane.DataModel
{
    [Serializable]
    public class ListTokenBalanceResult : ArkaneBaseResult
    {
        public List<TokenBalance> result;
    }
}
