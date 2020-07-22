using System;

namespace HeathenEngineering.Arkane.DataModel
{
    [Serializable]
    public class BalanceResult : ArkaneBaseResult
    {
        public WalletBallance result;
    }
}
