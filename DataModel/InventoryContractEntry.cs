using Boo.Lang;
using System;

namespace HeathenEngineering.Arkane.DataModel
{
    [Serializable]
    public class InventoryContractEntry
    {
        public string contractAddress;
        public List<InventoryTokenEntry> tokenTypes;
    }
}
