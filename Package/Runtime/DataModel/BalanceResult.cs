using System;
using System.Collections.Generic;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class BalanceResult : BGSDKBaseResult
    {
        public WalletBallance result;
    }
}
