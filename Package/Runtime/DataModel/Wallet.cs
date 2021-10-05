using System;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class Wallet
    {
        public string identifier;
        public string id;
        public string address;
        public string walletType;
        public string secretType;
        public DateTime createdAt;
        public bool archived;
        public string alias;
        public bool primary;
        public bool hasCustomPin;
        public WalletBallance balance;
    }
}
