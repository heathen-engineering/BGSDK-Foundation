using Boo.Lang;
using System;

namespace HeathenEngineering.Arkane.DataModel
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
