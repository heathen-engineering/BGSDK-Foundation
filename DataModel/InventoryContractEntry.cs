using System;
using System.Collections.Generic;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class InventoryContractEntry
    {
        public string contractAddress;
        public List<InventoryTokenEntry> tokenTypes;
    }
}
