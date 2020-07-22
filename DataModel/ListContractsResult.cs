using System;
using System.Collections.Generic;

namespace HeathenEngineering.Arkane.DataModel
{
    [Serializable]
    public class ListContractsResult : ArkaneBaseResult
    {
        public List<DataModel.Contract> result;
    }
}
