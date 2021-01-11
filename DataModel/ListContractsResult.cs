using System;
using System.Collections.Generic;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class ListContractsResult : BGSDKBaseResult
    {
        public List<DataModel.ContractData> result;
    }
}
