using System;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class BalanceResult : BGSDKBaseResult
    {
        public WalletBallance result;
    }
}
