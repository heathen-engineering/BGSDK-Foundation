using System;
using System.Collections.Generic;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class ListInventoryResults : BGSDKBaseResult
    {
        public List<InventoryContractEntry> result;
    }
}
