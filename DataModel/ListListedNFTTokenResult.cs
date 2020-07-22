using System;
using System.Collections.Generic;

namespace HeathenEngineering.Arkane.DataModel
{
    [Serializable]
    public class ListListedNFTTokenResult : ArkaneBaseResult
    {
        public List<ListedNFTToken> result;
    }
}
