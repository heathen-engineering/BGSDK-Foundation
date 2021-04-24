using System;
using System.Collections.Generic;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class InventoryTokenEntry
    {
        public bool fungible;
        public ulong tokenTypeId;
        public BigDecimal balance;
        public List<string> tokenIds;
    }
}
