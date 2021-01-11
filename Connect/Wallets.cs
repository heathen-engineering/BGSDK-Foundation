#if DONOTUSE
using System;
using HeathenEngineering.BGSDK.DataModel;
using HeathenEngineering.BGSDK.Engine;

/// <summary>
/// Wrapper for the BGSDK Connect endpoint
/// </summary>
/// <remarks>
/// <para>
/// For more information please see <see href="https://docs-staging.arkane.network/pages/buildingblocks.html#_arkane_connect">https://docs-staging.arkane.network/pages/buildingblocks.html#_arkane_connect</see>
/// </para>
/// <para>
/// BGSDK Connect is an endpoint specifically designed to perform common tasks with BGSDK wallets. Connect was created as a way to perform tasks which are
///<list type="bullet">
///<item>otherwise not possible due to security reasons, like creating signatures</item>
///<item>necessary by all applications, like linking wallets your client</item>
///</list>
/// </para>
/// </remarks>
namespace HeathenEngineering.BGSDK.Connect
{
    /// <summary>
    /// Wraps the BGSDK interface for wallets incuding User, App and Whitelable wallets.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Wallet funcitonality is discribed in the <see href="https://docs.arkane.network/pages/reference.html">https://docs.arkane.network/pages/reference.html</see> documentation.
    /// </para>
    /// </remarks>
    public static partial class Wallets
    {
        /// <summary>
        /// Wraps funcitons specific to UserWallets aka "normal" wallets.
        /// </summary>
        /// <remarks>
        /// <para>
        /// See <see href="https://docs.arkane.network/pages/reference.html#_wallet">https://docs.arkane.network/pages/reference.html#_wallet</see> for more information.
        /// </para>
        /// </remarks>
        public static partial class UserWallet
        {
            /// <summary>
            /// TBD
            /// </summary>
            /// <param name="wallet"></param>
            /// <param name="callback"></param>
            public static void Link(string redirectUri)
            {
                //TODO: Implament
            }

            /// <summary>
            /// TBD
            /// </summary>
            /// <param name="wallet"></param>
            /// <param name="callback"></param>
            /// <returns></returns>
            public static void Manage(Wallet wallet, Action<BGSDKBaseResult> callback)
            {
                //TODO: Implament
            }
        }

        /// <summary>
        /// Weaps funcitonality specific to the ApplicationWallets
        /// </summary>
        /// <remarks>
        /// <para>
        /// For more information please see <see href="https://docs.arkane.network/pages/reference.html#_application_wallet">https://docs.arkane.network/pages/reference.html#_application_wallet</see>
        /// </para>
        /// </remarks>
        public static partial class ApplicationWallet
        {
            /// <summary>
            /// TBD
            /// </summary>
            public static void Create()
            {
                //TODO: Implament
            }

            /// <summary>
            /// TBD
            /// </summary>
            public static void ExecuteTransaction()
            {
                //TODO: Implament
            }
        }

        /// <summary>
        /// Wraps funcitonality specific to Transactions
        /// </summary>
        /// <remarks>
        /// <para>
        /// For more information please see <see href="https://docs.arkane.network/pages/reference.html#_transactions">https://docs.arkane.network/pages/reference.html#_transactions</see>
        /// </para>
        /// </remarks>
        public static partial class Transactions
        {
            public static void NativeTransfer() { }
            public static void TokenTransfer() { }
            public static void GasTransfer() { }
            public static void NFTTransfer() { }
            public static void ContractExecution() { }

            public static partial class Ethereum
            {
                public static void ExecuteETH() { }
                public static void ExecuteERC20() { }
                public static void ExecuteERC721() { }
                public static void Sign() { }
            }

            public static partial class Gochain
            {
                public static void ExecuteGO() { }
                public static void ExecuteGO20() { }
                public static void Sign() { }
            }

            public static partial class Vechain
            {
                public static void ExecuteVET() { }
                public static void ExecuteVTHO() { }
                public static void ExecuteVIP180() { }
            }

            public static partial class Bitcoin
            {
                public static void ExecuteBitcoin() { }
            }

            public static partial class Litecoin
            {
                public static void ExecuteLitecoin() { }
            }

            public static partial class Tron
            {
                public static void ExecuteTRON() { }
                public static void ExecuteTRC10() { }
                public static void Sign() { }
            }

            public static partial class Aeternity
            {
                public static void ExecuteAEternity() { }
                public static void Sign() { }
            }

            public static partial class Neo
            {
                public static void ExecuteNEONative() { }
                public static void ExecuteNEOGas() { }
            }
        }
    }
}
#endif