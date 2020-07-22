using System;
using System.Collections.Generic;

namespace HeathenEngineering.Arkane.DataModel
{
    [Serializable]
    public class ListInventoryResults : ArkaneBaseResult
    {
        public List<InventoryContractEntry> result;
    }
}
