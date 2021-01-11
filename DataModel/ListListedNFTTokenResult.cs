using System;
using System.Collections.Generic;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class ListListedNFTTokenResult : BGSDKBaseResult
    {
        public List<ListedNFTToken> result;
    }
}
