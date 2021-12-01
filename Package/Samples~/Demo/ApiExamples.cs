#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using HeathenEngineering.BGSDK.DataModel;
using UnityEngine;
using UnityEngine.UI;

namespace HeathenEngineering.BGSDK.Examples
{
    public class ApiExamples : MonoBehaviour
    {
        public InputField walletId;
        public InputField walletPin;
        public InputField walletAddress;
        public Dropdown createWalletType;
        public Dropdown listNFTWalletType;
        public SecretType chainType = SecretType.MATIC;

        [Header("Results")]
        public List<Wallet> walletData;
        public List<NFTBalanceResult.Token> tokenData;

        /// <summary>
        /// SERVER ONLY
        /// </summary>
        public void CreateWallet()
        {
#if UNITY_SERVER || UNITY_EDITOR
            if (!string.IsNullOrEmpty(walletId.text)
                && !string.IsNullOrEmpty(walletPin.text))
            {
                StartCoroutine(API.Server.Wallets.Create(walletPin.text, walletId.text, (SecretType)createWalletType.value, HandleCreateWalletResult));
            }
            else
            {
                Debug.LogWarning("You must provide a wallet ID and Pin in order to create a new wallet.");
            }
#else
            Debug.LogWarning("Attempted to call CreateWallet from a client build. This is not supported!");
#endif
        }

        /// <summary>
        /// SERVER ONLY
        /// </summary>
        public void FetchWallets()
        {
#if UNITY_SERVER || UNITY_EDITOR
            StartCoroutine(API.Server.Wallets.List(HandleListWalletResults));
#else
            Debug.LogWarning("Attempted to call FetchWallets from a client build. This is not supported!");
#endif
        }

        public void FetchNFTs()
        {
            StartCoroutine(API.Client.Wallets.NFTs(walletAddress.text, (SecretType)listNFTWalletType.value, null, HandleNFTBalanceResult));
        }

        private void HandleCreateWalletResult(ListWalletResult walletResult)
        {
            if (!walletResult.hasError)
            {
                walletData = walletResult.result;
                Debug.Log("Create Wallets Responce:\nReturned " + walletResult.result.Count.ToString() + " wallets. You can review the results in the inspector.");
            }
            else
            {
                Debug.Log("Create Wallets Responce:\nHas Error: " + walletResult.hasError + "\nMessage: " + walletResult.message + "\nCode:" + walletResult.httpCode);
            }
        }

        private void HandleNFTBalanceResult(NFTBalanceResult balanceResult)
        {
            if(!balanceResult.hasError)
            {
                tokenData = balanceResult.result;
                Debug.Log("List NFT Responce:\nReturned " + balanceResult.result.Count.ToString() + " NFTs. You can review the results in the inspector.");
            }
            else
            {
                Debug.Log("List NFT Responce:\nHas Error: " + balanceResult.hasError + "\nMessage: " + balanceResult.message + "\nCode:" + balanceResult.httpCode);
            }
        }

        private void HandleListWalletResults(ListWalletResult walletResult)
        {
            if (!walletResult.hasError)
            {
                walletData = walletResult.result;
                Debug.Log("List Wallets Responce:\nReturned " + walletResult.result.Count.ToString() + " wallets. You can review the results in the inspector.");
            }
            else
            {
                Debug.Log("List Wallets Responce:\nHas Error: " + walletResult.hasError + "\nMessage: " + walletResult.message + "\nCode:" + walletResult.httpCode);
            }
        }
    }
}
#endif