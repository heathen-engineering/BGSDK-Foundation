using System;
using System.Collections.Generic;

namespace HeathenEngineering.Arkane.DataModel
{
    [Serializable]
    public class InventoryContractEntry
    {
        public string contractAddress;
        public List<InventoryTokenEntry> tokenTypes;
    }
}
