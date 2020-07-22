using System;
using System.Collections.Generic;

namespace HeathenEngineering.Arkane.DataModel
{
    [Serializable]
    public class ListWalletResult : ArkaneBaseResult
    {
        public List<Wallet> result;
    }
}
