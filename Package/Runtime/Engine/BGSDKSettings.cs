﻿using HeathenEngineering.BGSDK.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeathenEngineering.BGSDK.Engine
{
    [CreateAssetMenu(menuName = "Blockchain Game SDK/Settings")]
    public class BGSDKSettings : ScriptableObject
    {
        public static BGSDKSettings current;
        public static Identity user;

        /// <summary>
        /// Endpoint to authenticate users with
        /// </summary>
        [FormerlySerializedAs("Authentication")]
        public DomainTarget authentication = new DomainTarget("https://login-staging.arkane.network", "https://login.arkane.network");
        
        [FormerlySerializedAs("walllet")]
        /// <summary>
        /// Wallet user interface endpoint
        /// </summary>
        public DomainTarget wallet = new DomainTarget("https://staging.arkane.network/", "https://app.arkane.network");
        /// <summary>
        /// Endpoint for API calls
        /// </summary>
        [FormerlySerializedAs("API")]
        public DomainTarget api = new DomainTarget("https://api-staging.arkane.network", "https://api.arkane.network");
        /// <summary>
        /// Endpoint used by the widget
        /// </summary>
        [FormerlySerializedAs("Connect")]
        public DomainTarget connect = new DomainTarget("https://connect-staging.arkane.network/auth/exchange", "https://connect.arkane.network/auth/exchange");
        [FormerlySerializedAs("Business")]
        public DomainTarget business = new DomainTarget("https://api-business-staging.arkane.network", "https://api-business.arkane.network");


        [Obsolete("Use useSettings instead.")]
        public bool UseStaging => useStaging;
        public bool useStaging = true;
        public AppId appId;
        //public AuthenticationMode AuthenticationMode = new AuthenticationMode("<client id>", "password");

        [FormerlySerializedAs("Contracts")]
        public List<Engine.Contract> contracts = new List<Engine.Contract>();
         
        public Engine.Contract this[int contractIndex] => contracts?[contractIndex];

        public Engine.Contract this[string contractId] => contracts?.FirstOrDefault(p => p.Id == contractId);

        public Engine.Contract FindContractByName(string contractName) => contracts?.FirstOrDefault(p => p.SystemName == contractName);

        public Engine.Token FindTokenById(string id)
        {
            foreach(var contract in contracts)
            {
                foreach(var token in contract.tokens)
                {
                    if (token.Id == id)
                        return token;
                }
            }

            return null;
        }

        public Engine.Token FindTokenByName(string name)
        {
            foreach (var contract in contracts)
            {
                foreach (var token in contract.tokens)
                {
                    if (token.SystemName == name)
                        return token;
                }
            }

            return null;
        }

        /// <summary>
        /// Used to authenticate the user to the BGSDK Network
        /// </summary>
        public string AuthenticationUri { get { return authentication[useStaging] + "/auth/realms/Arkane/protocol/openid-connect/token"; } }

        public string ConnectUri => connect[useStaging];

        /// <summary>
        /// Used to fetch the authenticated users wallets from the BGSDK Network
        /// </summary>
        public string WalletUri => api[useStaging] + "/api/wallets";

        /// <summary>
        /// Used to work against the business API for deploying, getting and working with BGSDK contracts.
        /// </summary>
        public string ContractUri => GetContractUri(appId);
        /// <summary>
        /// Used to work against the business API for listing and working with BGSDK applicaitons.
        /// </summary>
        public string AppsUri => business[useStaging] + "/api/apps";

        public string DefineTokenTypeUri(Engine.Contract forContract)
        {
            if (forContract == null)
                throw new NullReferenceException("A null contract was provided");
            else
                return ContractUri + "/" + forContract.Id.ToString() + "/token-types";
        }

        public string DefineTokenTypeUri(AppId onApp, Engine.Contract forContract)
        {
            if (forContract == null)
                throw new NullReferenceException("A null contract was provided");
            else
                return GetContractUri(onApp) + "/" + forContract.Id.ToString() + "/define-token-type";
        }

        public string MintTokenUri(Engine.Contract forContract)
        {
            if (forContract == null)
                throw new NullReferenceException("A null contract was provided");
            else
                return ContractUri + "/" + forContract.Id.ToString() + "/tokens";
        }

        public string MintTokenUri(AppId onApp, Engine.Contract forContract)
        {
            if (forContract == null)
                throw new NullReferenceException("A null contract was provided");
            else
                return GetContractUri(onApp) + "/" + forContract.Id.ToString() + "/tokens";
        }

        public string GetTokenUri(Engine.Contract forContract)
        {
            if (forContract == null)
                throw new NullReferenceException("A null contract was provided");
            else
                return ContractUri + "/" + forContract.Id + "/token-types";
        }

        public string GetTokenUri(long forContractId)
        {
            return ContractUri + "/" + forContractId.ToString() + "/token-types";
        }

        public string GetTokenUri(AppId onApp, Engine.Contract forContract)
        {
            if (forContract == null)
                throw new NullReferenceException("A null contract was provided");
            else
                return GetContractUri(onApp) + "/" + forContract.Id.ToString() + "/token-types";
        }

        public string GetTokenUri(AppId onApp, long forContractId)
        {
            return GetContractUri(onApp) + "/" + forContractId.ToString() + "/token-types";
        }

        /// <summary>
        /// Get the contract uri for a specific BGSDKApp
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public string GetContractUri(AppId app)
        {
            if (app == AppId.Invalid)
                throw new System.InvalidOperationException("Unable to construct a valid contract URI for an invalid app id.");

            return AppsUri + "/" + app.applicationId.ToString() + "/contracts";
        }
    }
}
