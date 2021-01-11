using System;
using System.Collections.Generic;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class ListWalletResult : BGSDKBaseResult
    {
        public List<Wallet> result;
    }
}
