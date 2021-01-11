#if UNITY_EDITOR
using HeathenEngineering.BGSDK.DataModel;
using HeathenEngineering.BGSDK.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace HeathenEngineering.BGSDK.Editor
{
    /// <summary>
    /// Only available in the Unity Editor!
    /// </summary>
    /// <remarks>
    /// <para>
    /// This funcitonality will not compile for builds and is only avialable within the Unity Editor
    /// </para>
    /// </remarks>
    public static partial class EditorUtilities
    {
        #region JSON String Classes
        /// <summary>
        /// JSON string helper class for Deploy Contract
        /// </summary>
        [Serializable]
        private class DeployContractModel
        {
            public string name;
            public string description;
        }
        #endregion

        /// <summary>
        /// <para>Tests the provided settings object for conflicts that could prevent processing.</para>
        /// </summary>
        /// <remarks>
        /// <para>Conditions tested for by this method include the following.</para>
        /// <list type="table">
        /// <listheader>
        /// <term>State</term>
        /// <term>Description</term>
        /// </listheader>
        /// <item><term>Empty Contracts</term><term>This is a contract with no tokens assigned to it.</term></item>
        /// <item><term>Orphaned Tokens</term><term>This is a token with no contract assigned to it.</term></item>
        /// <item><term>Empty Contract Names</term><term>This is a contract whoes name is empty.</term></item>
        /// <item><term>Empty Contract Description</term><term>This is a contract whoes description is empty.</term></item>
        /// <item><term>Empty Token Name</term><term>This is a token whoes name is empty.</term></item>
        /// <item><term>Empty Token Description</term><term>This is a token whoes description is empty.</term></item>
        /// <item><term>Duplicate Contract Name</term><term>This is a contract which shares a name with another contract.</term></item>
        /// <item><term>Duplicate Contract Address</term><term>This is a contract which shares an address with another contract.</term></item>
        /// <item><term>Duplicate Token Name</term><term>This is a token which shares a name with another token.</term></item>
        /// <item><term>Empty Model</term><term>This indicates that no valid contracts and tokens have been added to the tested model.</term></item>
        /// </list>
        /// </remarks>
        /// <param name="settings">The settings to be tested</param>
        /// <param name="message">The resulting messages</param>
        /// <returns>
        /// <list type="table">
        /// <listheader>
        /// <term>Type</term>
        /// <term>Description</term>
        /// </listheader>
        /// <item><term><see cref="ValidationStatus.Okay"/></term><term>No issues where found</term></item>
        /// <item><term><see cref="ValidationStatus.Warning"/></term><term>has an Empty Contract or Empty Contract Description or Empty Token Description or Duplicate Contract Name or Duplicate Token Name</term></item>
        /// <item><term><see cref="ValidationStatus.Error"/></term><term>has an Empty Model or Empty Token Name or Empty Contract Name or Orphaned Tokens or Duplicate Contract Address</term></item>
        /// </list>
        /// </returns>
        public static ValidationStatus ValidateSettingsModel(Settings settings, out string message, bool ignoreEmptyModel = false)
        {
            bool hasEmptyContract = false;
            bool hasOrphanedToken = false;
            bool hasEmptyContractName = false;
            bool hasEmptyContractDescription = false;
            bool hasEmptyTokenName = false;
            bool hasEmptyTokenDescription = false;
            bool hasDuplicateContractName = false;
            bool hasDuplicateContractAddress = false;
            bool hasDuplicateTokenName = false;
            bool hasEmptyModel = false;

            string EmptyContract = "One or more contracts have no tokens.";
            string OrphanedToken = "One or more tokens lack a contract reference.";
            string ContractName = "One or more contracts have no name.";
            string ContractDescription = "One or more contracts have no description.";
            string DuplicateContractName = "Duplicate contract names detected.";
            string DuplicateContractAddress = "Duplicate contract address detected.";
            string DuplicateTokenName = "Duplicate token names detected.";
            string TokenName = "One or more tokens have no name.";
            string TokenDescription = "One or more tokens have no description.";
            string EmptyModel = "No datamodel elements detected.";
            string NoErrors = "No errors detected.\nSee the BGSDK Manager window to commit this datamodel.";

            if (!ignoreEmptyModel)
            {
                if (settings.Contracts == null || settings.Contracts.Count < 1)
                    hasEmptyModel = true;
            }
            else
            {
                if (settings.Contracts == null)
                    settings.Contracts = new List<Engine.Contract>();
            }

            if (!hasEmptyModel)
            {
                foreach (var contract in settings.Contracts)
                {
                    if (contract == null)
                        continue;

                    if (!hasDuplicateContractAddress && settings.Contracts.Where(p => p.address != string.Empty && p.address == contract.address).Count() > 1)
                        hasDuplicateContractAddress = true;

                    if (!hasEmptyContract && (contract.Tokens == null || contract.Tokens.Count == 0))
                        hasEmptyContract = true;

                    if (!hasDuplicateContractName && settings.Contracts.Where(p => p.systemName == contract.systemName).Count() > 1)
                        hasDuplicateContractName = true;

                    if (!hasEmptyContractName && string.IsNullOrEmpty(contract.systemName))
                        hasEmptyContractName = true;

                    if (!hasEmptyContractDescription && string.IsNullOrEmpty(contract.description))
                        hasEmptyContractDescription = true;

                    foreach (var token in contract.Tokens)
                    {
                        if (token == null)
                            continue;

                        if (!hasDuplicateTokenName && contract.Tokens.Where(p => p.SystemName == token.SystemName).Count() > 1)
                            hasDuplicateTokenName = true;

                        if (!hasEmptyTokenName && string.IsNullOrEmpty(token.SystemName))
                            hasEmptyTokenName = true;

                        if (!hasEmptyTokenDescription && string.IsNullOrEmpty(token.Description))
                            hasEmptyTokenDescription = true;

                        if (!hasOrphanedToken && token.contract == null)
                            hasOrphanedToken = true;

                        if (string.IsNullOrEmpty(token.SystemName))
                            hasEmptyTokenName = true;

                        if (string.IsNullOrEmpty(token.Description))
                            hasEmptyTokenDescription = true;

                        if (token.contract != contract)
                            token.contract = contract;
                    }
                }
            }



            bool hasErrors = hasEmptyContract || hasOrphanedToken || hasEmptyContractName || hasEmptyContractDescription || hasEmptyTokenName || hasEmptyTokenDescription || hasEmptyModel;

            if (hasErrors)
            {
                message = "\n";
                if (hasEmptyContract)
                    message += EmptyContract + "\n";
                if (hasOrphanedToken)
                    message += OrphanedToken + "\n";
                if (hasEmptyContractName)
                    message += ContractName + "\n";
                if (hasEmptyContractDescription)
                    message += ContractDescription + "\n";
                if (hasEmptyTokenName)
                    message += TokenName + "\n";
                if (hasEmptyTokenDescription)
                    message += TokenDescription + "\n";
                if (hasDuplicateContractName)
                    message += DuplicateContractName + "\n";
                if (hasDuplicateTokenName)
                    message += DuplicateTokenName + "\n";
                if (hasDuplicateContractAddress)
                    message += DuplicateContractAddress + "\n";
                if (hasEmptyModel)
                    message += EmptyModel + "\n";

                message.Trim();
            }
            else
                message = NoErrors;

            if (hasEmptyModel || hasEmptyTokenName || hasEmptyContractName || hasOrphanedToken || hasDuplicateContractAddress)
                return ValidationStatus.Error;
            else if (hasEmptyContract || hasEmptyContractDescription || hasEmptyTokenDescription || hasDuplicateContractName || hasDuplicateTokenName)
                return ValidationStatus.Warning;
            else
                return ValidationStatus.Okay;
        }

        #region Depricated
        /// <summary>
        /// Consumes a <see cref="Settings"/> object updates it with the data found on service.
        /// </summary>
        /// <param name="settings">The settings to process ... these will be set as the active settings for the BGSDK API</param>
        /// <param name="identity">The identity of the user which will process the elements against the BGSDK service.</param>
        /// <returns></returns>
        public static IEnumerator SyncSettings()
        {
            //First insure we have a fresh token based on secret
            if (string.IsNullOrEmpty(Settings.current.AppId.clientSecret) || string.IsNullOrEmpty(Settings.current.AppId.clientId))
            {
                Debug.LogError("Failed to sync settings: you must populate the Client ID and Client Secret before you can sync settings.");
                yield return null;
            }
            else
            {
                var settings = Settings.current;

                var authenticated = false;

                WWWForm authForm = new WWWForm();
                authForm.AddField("grant_type", "client_credentials");
                authForm.AddField("client_id", Settings.current.AppId.clientId);
                authForm.AddField("client_secret", Settings.current.AppId.clientSecret);

                UnityWebRequest auth_www = UnityWebRequest.Post(Settings.current.AuthenticationUri, authForm);

                var ao = auth_www.SendWebRequest();

                while (!ao.isDone)
                {
                    yield return null;
                }

                if (!auth_www.isNetworkError && !auth_www.isHttpError)
                {
                    string resultContent = auth_www.downloadHandler.text;
                    if (Settings.user == null)
                        Settings.user = new Identity();
                    Settings.user.authentication = JsonUtility.FromJson<AuthenticationResponce>(resultContent);
                    Settings.user.authentication.not_before_policy = resultContent.Contains("not-before-policy:1");
                    Settings.user.authentication.Create();
                    authenticated = true;
                }
                else
                {
                    Debug.LogError((auth_www.isNetworkError ? "Error on authentication: Network Error." : "Error on authentication: HTTP Error.") + "\n" + auth_www.error);
                }

                if (authenticated)
                {
                    /**********************************************************************************
                     * First validate our model but skip the check on empty models ... this allows 
                     * the sync to be used to download the model from the service
                     **********************************************************************************/
                    var message = string.Empty;
                    if (ValidateSettingsModel(settings, out message, true) != ValidationStatus.Error)
                    {
                        foreach (var contract in settings.Contracts)
                        {
                            contract.UpdatedFromServer = false;
                            foreach (var token in contract.Tokens)
                            {
                                token.UpdatedFromServer = false;
                            }
                        }

                        //Good enough process the settings
                        Settings.current = settings;

                        /**********************************************************************************
                         * Next fetch a list of all contracts assoceated with the AppId recorded on 
                         * these settings
                         **********************************************************************************/

                        UnityWebRequest wwwContract = UnityWebRequest.Get(settings.ContractUri);
                        wwwContract.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

                        var co = wwwContract.SendWebRequest();
                        while (!co.isDone)
                            yield return null;

                        if (!wwwContract.isNetworkError && !wwwContract.isHttpError)
                        {
                            string resultContractContent = wwwContract.downloadHandler.text;
                            var existingContracts = JsonUtility.FromJson<ListContractsResult>(Utilities.JSONArrayWrapper(resultContractContent));

                            /**********************************************************************************
                            * Next for contract found try and match to a BGSDKContract object already recorded
                            * in our settings, if none is found to match then create a new one and store it to
                            * our settings.
                            **********************************************************************************/
                            foreach (var contractData in existingContracts.result)
                            {
                                #region Update for data existing on the backend service
                                //Try to match based on address ... this is the safest method
                                var arkaneContract = settings.Contracts.FirstOrDefault(p => p.Data.address == contractData.address);
                                if (arkaneContract != default(Engine.Contract))
                                {
                                    arkaneContract.Data = contractData;
                                    if (arkaneContract.name != contractData.name)
                                    {
                                        arkaneContract.name = contractData.name;

                                        if (arkaneContract.Tokens == null)
                                            arkaneContract.Tokens = new List<Engine.Token>();

                                        foreach (var token in arkaneContract.Tokens)
                                        {
                                            token.name = arkaneContract.name + " : " + token.SystemName;
                                        }
                                    }
                                    arkaneContract.UpdatedFromServer = true;
                                    arkaneContract.UpdatedOn = DateTime.Now.ToBinary();

                                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(settings));
                                }
                                else
                                {
                                    //Try to match based on ID ... note if there are any contracts in the back end system then the first contract with an ID of 0 is likely to match
                                    arkaneContract = settings.Contracts.FirstOrDefault(p => p.Data.id == contractData.id);
                                    if (arkaneContract != default(Engine.Contract))
                                    {
                                        arkaneContract.Data = contractData;
                                        if (arkaneContract.name != contractData.name)
                                        {
                                            arkaneContract.name = contractData.name;

                                            if (arkaneContract.Tokens == null)
                                                arkaneContract.Tokens = new List<Engine.Token>();

                                            foreach (var token in arkaneContract.Tokens)
                                            {
                                                token.name = arkaneContract.name + " : " + token.SystemName;
                                            }
                                        }
                                        arkaneContract.UpdatedFromServer = true;
                                        arkaneContract.UpdatedOn = DateTime.Now.ToBinary();

                                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(settings));
                                    }
                                    else
                                    {
                                        //Least reliable match method but should be tried all the same
                                        arkaneContract = settings.Contracts.FirstOrDefault(p => p.Data.name == contractData.name);
                                        if (arkaneContract != default(Engine.Contract))
                                        {
                                            arkaneContract.Data = contractData;
                                            if (arkaneContract.name != contractData.name)
                                            {
                                                arkaneContract.name = contractData.name;

                                                if (arkaneContract.Tokens == null)
                                                    arkaneContract.Tokens = new List<Engine.Token>();

                                                foreach (var token in arkaneContract.Tokens)
                                                {
                                                    token.name = arkaneContract.name + " : " + token.SystemName;
                                                }
                                            }
                                            arkaneContract.UpdatedFromServer = true;
                                            arkaneContract.UpdatedOn = DateTime.Now.ToBinary();
                                        }
                                        else
                                        {
                                            /**********************************************************************************
                                             * At this point we have failed to match through all methods and so we should just
                                             * create a new BGSDKContract object and store it in our settings.
                                             **********************************************************************************/

                                            arkaneContract = ScriptableObject.CreateInstance<Engine.Contract>();
                                            arkaneContract.name = contractData.name;
                                            arkaneContract.Data = contractData;
                                            arkaneContract.UpdatedFromServer = true;
                                            arkaneContract.UpdatedOn = DateTime.Now.ToBinary();

                                            string path = AssetDatabase.GetAssetPath(settings);
                                            if (path == "")
                                            {
                                                path = "Assets";
                                            }
                                            else if (Path.GetExtension(path) != "")
                                            {
                                                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(settings)), "");
                                            }

                                            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + contractData.name + ".asset");

                                            AssetDatabase.AddObjectToAsset(arkaneContract, settings);
                                            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(settings));

                                            settings.Contracts.Add(arkaneContract);
                                        }
                                    }
                                }
                                #endregion

                                /**********************************************************************************
                                 * Now that we have synced this contract to our settings object from the service
                                 * we need to look through all tokens on the service for this contract and either
                                 * sync them to the matching BGSDKToken object or create a new BGSDKToken object
                                 * to hold the data.
                                 **********************************************************************************/

                                #region Update token's for this contract that are on the backend service
                                UnityWebRequest wwwToken = UnityWebRequest.Get(settings.GetTokenUri(arkaneContract));
                                wwwToken.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

                                var to = wwwToken.SendWebRequest();
                                while (!to.isDone)
                                    yield return null;

                                if (!wwwToken.isNetworkError && !wwwToken.isHttpError)
                                {
                                    string resultTokenContent = wwwToken.downloadHandler.text;
                                    var tokenResults = JsonUtility.FromJson<ListTokenTypesResult>(Utilities.JSONArrayWrapper(resultTokenContent));
                                    Debug.Log("Found " + tokenResults.result.Count.ToString() + " tokens.");
                                    foreach (var tokenData in tokenResults.result)
                                    {
                                        Debug.Log("Found Token: " + tokenData.id);
                                        //Get the token so we can get the full data set for it
                                        UnityWebRequest wwwFullToken = UnityWebRequest.Get(settings.GetTokenUri(arkaneContract) + "/" + tokenData.id);
                                        wwwFullToken.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);

                                        var ftd = wwwFullToken.SendWebRequest();
                                        while (!ftd.isDone)
                                            yield return null;

                                        WebResults<TokenResponceData> webResult = new WebResults<TokenResponceData>(wwwFullToken);

                                        if (!webResult.isNetworkError && !webResult.isHttpError)
                                        {
                                            var arkaneToken = arkaneContract.Tokens.FirstOrDefault(p => p.Id == tokenData.id);
                                            if (arkaneToken != default(Engine.Token))
                                            {
                                                arkaneToken.Set(webResult);

                                                if (arkaneToken.name != arkaneContract.name + " : " + arkaneToken.SystemName)
                                                {
                                                    arkaneContract.name = arkaneContract.name + " : " + arkaneToken.SystemName;
                                                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(settings));
                                                }

                                                arkaneToken.UpdatedFromServer = true;
                                                arkaneToken.UpdatedOn = DateTime.Now.ToBinary();
                                            }
                                            else
                                            {
                                                arkaneToken = arkaneContract.Tokens.FirstOrDefault(p => p.SystemName == webResult.result.name && string.IsNullOrEmpty(p.Id));
                                                if (arkaneToken != default(Engine.Token))
                                                {
                                                    arkaneToken.Set(webResult);

                                                    if (arkaneToken.name != arkaneContract.name + " : " + arkaneToken.SystemName)
                                                    {
                                                        arkaneContract.name = arkaneContract.name + " : " + arkaneToken.SystemName;
                                                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(settings));
                                                    }

                                                    arkaneToken.UpdatedFromServer = true;
                                                    arkaneToken.UpdatedOn = DateTime.Now.ToBinary();
                                                }
                                                else
                                                {
                                                    /**********************************************************************************
                                                     * At this point we have failed to match the token to any existing BGSDKToken object
                                                     * so we will create a new one and store it in our settings
                                                     **********************************************************************************/

                                                    arkaneToken = ScriptableObject.CreateInstance<Engine.Token>();
                                                    arkaneToken.name = arkaneContract.name + " : " + webResult.result.name;
                                                    arkaneToken.Set(webResult);
                                                    arkaneToken.UpdatedFromServer = true;
                                                    arkaneToken.UpdatedOn = DateTime.Now.ToBinary();
                                                    arkaneContract.Tokens.Add(arkaneToken);

                                                    AssetDatabase.AddObjectToAsset(arkaneToken, arkaneContract);
                                                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(settings));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //TODO: handle fetch of token data error
                                            Debug.LogError("Failed to fetch full token data for Token Id = " + tokenData.id + " of contract Id = " + contractData.id);
                                        }
                                    }
                                }
                                else
                                {
                                    //TODO: handle error getting token list
                                    Debug.LogError("Failed to fetch token list contract Id = " + contractData.id);
                                }
                                #endregion

                                /**********************************************************************************
                                 * At this point we have synced all tokens for this contract from the service to 
                                 * the settings object
                                 * 
                                 * we now get a list of all the tokens that have not yet been synced for this contract
                                 * and sync them up to the service backend
                                 **********************************************************************************/

                                #region Add new tokens that are not yet on the backend service
                                var newTokens = arkaneContract.Tokens.Where(p => !p.UpdatedFromServer);

                                foreach (var token in newTokens)
                                {
                                    yield return CreateTokenType(arkaneContract, token, (result) =>
                                    {
                                        if(result.hasError)
                                        {
                                            Debug.LogError("Failed to create token [" + token.SystemName + "] for contract [" + arkaneContract.systemName + "], error:  " + result.httpCode + " message: " + result.message);
                                        }
                                        else
                                        {
                                            Debug.Log("Created token [" + token.SystemName + "] for contract [" + arkaneContract.systemName + "]");
                                        }
                                    });
                                }
                                #endregion
                            }

                            /**********************************************************************************
                             * At this point all contracts that already existed on the backend have been fully 
                             * synced. We now need to get all the contracts in our settings that have not yet
                             * been synced and process them against the server.
                             **********************************************************************************/
                            var newContracts = settings.Contracts.Where(p => !p.UpdatedFromServer);
                            foreach (var contract in newContracts)
                            {
                                DeployContractModel nContract = new DeployContractModel() { name = contract.Data.name, description = contract.Data.description };
                                var jsonString = JsonUtility.ToJson(nContract);

                                UnityWebRequest wwwCreateContract = UnityWebRequest.Put(Settings.current.ContractUri, jsonString);
                                wwwCreateContract.method = UnityWebRequest.kHttpVerbPOST;
                                wwwCreateContract.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);
                                wwwCreateContract.uploadHandler.contentType = "application/json;charset=UTF-8";
                                wwwCreateContract.SetRequestHeader("Content-Type", "application/json;charset=UTF-8");

                                var ccc = wwwCreateContract.SendWebRequest();
                                while (!ccc.isDone)
                                    yield return null;

                                if (!wwwCreateContract.isNetworkError && !wwwCreateContract.isHttpError)
                                {
                                    string resultContent = wwwCreateContract.downloadHandler.text;
                                    var result = JsonUtility.FromJson<DataModel.ContractData>(resultContent);

                                    contract.Data = result;
                                    contract.UpdatedFromServer = true;
                                    contract.UpdatedOn = DateTime.Now.ToBinary();

                                    /**********************************************************************************
                                     * Finally get all the BGSDKToken objects for this contract and create them on the 
                                     * server
                                     **********************************************************************************/
                                    foreach (var token in contract.Tokens)
                                    {
                                        yield return CreateTokenType(contract, token, (r) =>
                                        {
                                            if (r.hasError)
                                            {
                                                Debug.LogError("Failed to create token [" + token.SystemName + "] for contract [" + contract.systemName + "], error:  " + r.httpCode + " message: " + r.message);
                                            }
                                            else
                                            {
                                                Debug.Log("Created token [" + token.SystemName + "] for contract [" + contract.systemName + "]");
                                            }
                                        });
                                    }
                                }
                                else
                                {
                                    //TODO: handle error creating contract
                                    Debug.LogError("Failed to create contract: [Code: " + wwwCreateContract.responseCode + "] " + wwwCreateContract.error);
                                }


                            }
                        }
                        else
                        {
                            //TODO: notify the user that something went wrong while processing existing contracts.
                            Debug.LogError("Failed to fetch the list of contracts: [Code: " + wwwContract.responseCode + "] " + wwwContract.error);
                        }
                    }
                    else
                    {
                        //Some notable error occured in the validation step so report it to the end user
                        Debug.LogError("Model validation failed:\n" + message);

                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// <para>Deploying a new Token Contract. This token contract represents the game inventory (all items that can exist within the game).</para>
        /// <see href ="https://docs.arkane.network/pages/token-management.html">https://docs.arkane.network/pages/token-management.html</see>
        /// </summary>
        /// <param name="identity">The authenticated identity to operate with</param>
        /// <param name="name">The name of the new contract to deploy</param>
        /// <param name="description">A description of the contract to deploy</param>
        /// <param name="responce">A method that will be called on completion rather success or failure</param>
        /// <returns></returns>
        public static IEnumerator DeployContract(Identity identity, AppId app, string name, string description, Action<DataModel.ContractResult> responce)
        {
            //https://docs.arkane.network/pages/token-management.html

            if (string.IsNullOrEmpty(name))
            {
                responce(new DataModel.ContractResult() { hasError = true, message = "name required, null or empty name provided.", result = null });
                yield return null;
            }
            else if (identity == null)
            {
                responce(new DataModel.ContractResult() { hasError = true, message = "BGSDKIdentity required, null identity provided.", result = null });
                yield return null;
            }
            else
            {
                WWWForm form = new WWWForm();
                form.AddField("name", name);
                form.AddField("description", description);

                UnityWebRequest www = UnityWebRequest.Post(Settings.current.GetContractUri(app), form);

                www.SetRequestHeader("Authorization", identity.authentication.token_type + " " + identity.authentication.access_token);

                var co = www.SendWebRequest();
                while (!co.isDone)
                    yield return null;

                if (!www.isNetworkError && !www.isHttpError)
                {
                    string resultContent = www.downloadHandler.text;
                    var results = new DataModel.ContractResult();
                    results.result = JsonUtility.FromJson<DataModel.ContractData>(Utilities.JSONArrayWrapper(resultContent));
                    results.message = "Deploy Contract complete.";
                    results.httpCode = www.responseCode;
                    responce(results);
                }
                else
                {
                    responce(new DataModel.ContractResult() { hasError = true, message = "Error:" + (www.isNetworkError ? " a network error occured while attempting to deploy a contract." : " a HTTP error occured while attempting to deploy a contracts."), result = null, httpCode = www.responseCode });
                }
            }
        }

        /// <summary>
        /// <para>
        /// Before generating tokens, we need to define the type which will describe the token.
        /// </para>
        /// <see href ="https://docs-staging.arkane.network/pages/token-management.html#_create_token_type" />
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="app"></param>
        /// <param name="contract"></param>
        /// <param name="token"></param>
        /// <param name="responce"></param>
        /// <returns></returns>
        public static IEnumerator CreateTokenType(Engine.Contract contract, Token token, Action<DataModel.CreateTokenTypeResult> responce)
        {
            //Define a type of token
            yield return null;

            if (string.IsNullOrEmpty(Settings.current.AppId.clientSecret) || string.IsNullOrEmpty(Settings.current.AppId.clientId))
            {
                Debug.LogError("Failed to sync settings: you must populate the Client ID and Client Secret before you can sync settings.");
                yield return null;
            }
            else if (string.IsNullOrEmpty(token.SystemName))
            {
                responce(new DataModel.CreateTokenTypeResult() { hasError = true, message = "name required, null or empty name provided." });
                yield return null;
            }
            else
            {
                var request = new UnityWebRequest(Settings.current.DefineTokenTypeUri(contract), "POST");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(token.CreateTokenDefitionJson());
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Authorization", Settings.user.authentication.token_type + " " + Settings.user.authentication.access_token);
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();

                if (!request.isNetworkError && !request.isHttpError)
                {
                    string resultContent = request.downloadHandler.text;
                    var results = new DataModel.CreateTokenTypeResult();
                    results.result = JsonUtility.FromJson<TokenCreateResponceData>(resultContent);
                    results.message = "Define Token Type complete.";
                    results.httpCode = request.responseCode;
                    responce(results);
                }
                else
                {
                    responce(new DataModel.CreateTokenTypeResult() { hasError = true, message = "Error:" + (request.isNetworkError ? " a network error occured while attempting to define the token type." : " a HTTP error occured while attempting to define the token type."), httpCode = request.responseCode });
                }
            }
        }
    }
}
#endif