using System;
using UnityEngine;

namespace HeathenEngineering.Arkane.DataModel
{
    [Serializable]
    public struct ContractData
    {
        /// <summary>
        /// id of the contract.
        /// </summary>
        [Tooltip("id of the contract.")]
        public ulong id;
        /// <summary>
        /// name of the contract.
        /// </summary>
        [Tooltip("name of the contract.")]
        public string name;
        /// <summary>
        /// description of the contract.
        /// </summary>
        [Tooltip("description of the contract.")]
        public string description;
        /// <summary>
        /// Whether it’s been confirmed or not (on the blockchain).
        /// </summary>
        [Tooltip("Whether it’s been confirmed or not (on the blockchain).")]
        public bool confirmed;
        /// <summary>
        /// Address on the blockchain.
        /// </summary>
        [Tooltip("Address on the blockchain.")]
        public string address;
        /// <summary>
        /// Symbol of the contract (e.g. GODS, CKITTY, STRK)
        /// </summary>
        [Tooltip("Symbol of the contract (e.g. GODS, CKITTY, STRK).")]
        public string symbol;
        /// <summary>
        /// Link to the website/application that issued this contract
        /// </summary>
        [Tooltip("Link to the website/application that issued this contract.")]
        public string url;
        /// <summary>
        /// Link to an image (logo) of this contract.
        /// </summary>
        [Tooltip("Link to an image (logo) of this contract.")]
        public string imageUrl;
        /// <summary>
        /// The type of the contract (ex. ERC721)
        /// </summary>
        [Tooltip("The type of the contract (ex. ERC721).")]
        public string type;
    }
}


//public class TestBehaviour : MonoBehaviour
//{
//    public Settings settings;

//    [Header("UI Elements")]
//    public GameObject loginRoot;
//    public UnityEngine.UI.InputField Username;
//    public UnityEngine.UI.InputField Password;
//    public UnityEngine.UI.Text ConsoleText;

//    [Header("Results")]
//    public Identity Identity = new Identity();
//    public List<Wallet> wallets = new List<Wallet>();

//    private void Start()
//    {
//        Settings.current = settings;

//        if (wallets == null)
//            wallets = new List<Wallet>();
//        else
//            wallets.Clear();

//        ConsoleText.text = string.Empty;
//    }

//    public void Authenticate()
//    {
//        Identity = new Identity() { username = Username.text, password = Password.text };
//        StartCoroutine(Editor.EditorUtilities.Authenticate(Identity, HandleAuthenticationResult));
//    }

//    public void FetchContracts()
//    {
//        StartCoroutine(API.TokenManagement.ListContracts(Identity, HandleListContractResults));
//    }

//    private void HandleListContractResults(ListContractsResult contractsResult)
//    {
//        Debug.Log("List Contracts Responce:\nHas Error: " + contractsResult.hasError + "\nMessage: " + contractsResult.message);
//    }

//    private void HandleAuthenticationResult(AuthenticationResult authenticationResult)
//    {
//        Debug.Log("Authenticate Responce:\nHas Error: " + authenticationResult.hasError + "\nMessage:" + authenticationResult.message);
//        if (!authenticationResult.hasError)
//        {
//            loginRoot.SetActive(false);
//            ConsoleText.text = "<color=green><b>Authenticated</b></color>";
//            StartCoroutine(API.Wallets.List(Identity, HandleWalletRefresh));
//        }
//        else
//        {
//            ConsoleText.text = "<color=red><b>Not Authenticated</b></color>\n" + authenticationResult.message;
//        }
//    }

//    private void HandleWalletRefresh(ListWalletResult walletResult)
//    {
//        wallets.Clear();
//        Debug.Log("List Wallets Responce:\nHas Error: " + walletResult.hasError + "\nMessage:" + walletResult.message);

//        if(!walletResult.hasError)
//        {
//            foreach(var wallet in walletResult.result)
//            {
//                wallets.Add(wallet);
//                Debug.Log(wallet.description + " : " + wallet.address);
//                ConsoleText.text += "\n\n<b>Wallet Alias: "+wallet.alias+"</b>\nType: " + wallet.walletType + "\nDescription: " + wallet.description;
//            }
//        }
//    }
//}