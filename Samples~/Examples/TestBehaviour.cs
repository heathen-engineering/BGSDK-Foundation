using System;
using System.Collections.Generic;
using HeathenEngineering.BGSDK.DataModel;
using HeathenEngineering.BGSDK.Engine;
using UnityEngine;

namespace HeathenEngineering.BGSDK.Examples
{
    public class TestBehaviour : MonoBehaviour
    {
        [Serializable]
        public class TestProperties
        {
            public string type;
            public int level;
            public string category;
        }

        [Serializable]
        public class TestTokenDef : TokenDefinition<TestProperties>
        { }

        //Your BGSDK settings... should be set in the insepctor
        public Settings settings;
        //Your target contract... should be set in the inspector
        public Contract contract;

        [Header("UI Elements")]
        public string Username;
        public string Password;

        [Header("Results")]
        public ContractData ContractData;

        private void Start()
        {
            Settings.current = settings;
        }

        public void TestJson()
        {
            var nToken = new TokenDefinition<TestProperties>();
            nToken.name = "Test name";
            nToken.description = "Test description";
            nToken.nft = true;
            nToken.properties = new TestProperties
            {
                type = "test type",
                level = 32,
                category = "test category"
            };

            var result = JsonUtility.ToJson(nToken);
            Debug.Log(result);
        }

        public void FetchContracts()
        {
            StartCoroutine(API.TokenManagement.GetContract(contract, HandleGetContractResults));
        }

        private void HandleGetContractResults(ContractResult contractResult)
        {
            Debug.Log("List Contracts Responce:\nHas Error: " + contractResult.hasError + "\nMessage: " + contractResult.message);
            if(!contractResult.hasError)
            {
                if (contractResult.result.HasValue)
                    ContractData = contractResult.result.Value;
                else
                {
                    //TODO: Handle no contract found
                }
            }
            else
            {
                //TODO: Handle your errors
            }
        }

    }
}
