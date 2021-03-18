using System;
using System.Collections.Generic;
using HeathenEngineering.BGSDK.DataModel;
using HeathenEngineering.BGSDK.Engine;
using UnityEngine;

namespace HeathenEngineering.BGSDK.Examples
{
    public class ApiExamples : MonoBehaviour
    {
        //Your target contract... should be set in the inspector
        public Contract contract;

        [Header("Results")]
        public ContractData contractData;
        public List<Wallet> walletData;

        public void FetchContracts()
        {
            StartCoroutine(API.TokenManagement.GetContract(contract, HandleGetContractResults));
        }

        public void FetchWallets()
        {
            StartCoroutine(API.Wallets.List(HandleWalletResults));
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
                Debug.Log("List Wallets Responce:\nHas Error: " + walletResult.hasError + "\nMessage: " + walletResult.message);
            }
        }

        private void HandleGetContractResults(ContractResult contractResult)
        {
            
            if(!contractResult.hasError)
            {
                if (contractResult.result.HasValue)
                {
                    contractData = contractResult.result.Value;
                    Debug.Log("List Contracts Responce:\nContract Data Found ... please view the inspector.");
                }
                else
                {
                    //TODO: Handle no contract found
                    Debug.Log("List Contracts Responce:\nNo contract found.");
                }
            }
            else
            {
                //TODO: Handle your errors
                Debug.Log("List Contracts Responce:\nHas Error: " + contractResult.hasError + "\nMessage: " + contractResult.message);
            }
        }
    }
}
