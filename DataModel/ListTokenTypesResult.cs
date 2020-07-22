using System;
using System.Collections.Generic;

namespace HeathenEngineering.Arkane.DataModel
{
    [Serializable]
    public class ListTokenTypesResult : ArkaneBaseResult
    {
        public List<TokenCreateResponceData> result;
    }
}
