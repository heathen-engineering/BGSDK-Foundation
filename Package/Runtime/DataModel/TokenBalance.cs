using System;
using System.Numerics;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class TokenBalance
    {
        public string tokenAddress;
        public BigInteger rawBalance;
        public BigDecimal balance;
        public string type;
        public bool transferable;
        public string symbol;
        public string logo;
        public uint decimals;
    }
}
