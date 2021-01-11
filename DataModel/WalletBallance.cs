using System;
using System.Numerics;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class WalletBallance
    {
        public bool available;
        public string secretType;
        public BigDecimal balance;
        public BigDecimal gasBalance;
        public string symbol;
        public string gasSymbol;
        public BigInteger rawBalance;
        public BigInteger rawGasBalance;
        public ulong decimals;
    }
}
