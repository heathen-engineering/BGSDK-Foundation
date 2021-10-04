#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using HeathenEngineering.BGSDK.DataModel;
using HeathenEngineering.BGSDK.Engine;
using UnityEngine;

namespace HeathenEngineering.BGSDK.Examples
{
    public class ApiExamples : MonoBehaviour
    {
        public string walletAddress;
        public SecretType chainType = SecretType.MATIC;

        [Header("Results")]
        public List<Wallet> walletData;
        public List<NFTBalanceResult.Token> tokenData;

        public void FetchWallets()
        {
            StartCoroutine(API.Server.Wallets.List(HandleWalletResults));
        }

        public void FetchNFTs()
        {
            StartCoroutine(API.Server.Wallets.NFTs(walletAddress, chainType, null, HandleNFTBalanceResult));
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

        private void HandleWalletResults(ListWalletResult walletResult)
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