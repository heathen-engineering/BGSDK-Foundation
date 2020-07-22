using System.Collections.Generic;
using HeathenEngineering.Arkane.DataModel;
using HeathenEngineering.Arkane.Engine;
using UnityEngine;

namespace HeathenEngineering.Arkane.Examples
{
    public class TestBehaviour : MonoBehaviour
    {
        public Settings settings;

        [Header("UI Elements")]
        public GameObject loginRoot;
        public UnityEngine.UI.InputField Username;
        public UnityEngine.UI.InputField Password;
        public UnityEngine.UI.Text ConsoleText;

        [Header("Results")]
        public Identity Identity = new Identity();
        public List<Wallet> wallets = new List<Wallet>();

        private void Start()
        {
            Settings.current = settings;

            if (wallets == null)
                wallets = new List<Wallet>();
            else
                wallets.Clear();

            ConsoleText.text = string.Empty;
        }

        public void Authenticate()
        {
            Identity = new Identity() { username = Username.text, password = Password.text };
            StartCoroutine(Editor.EditorUtilities.Authenticate(Identity, HandleAuthenticationResult));
        }

        private void HandleAuthenticationResult(AuthenticationResult result)
        {
            Debug.Log("Authenticate Responce:\nHas Error: " + result.hasError + "\nMessage:" + result.message);
            if (!result.hasError)
            {
                loginRoot.SetActive(false);
                ConsoleText.text = "<color=green><b>Authenticated</b></color>";
                StartCoroutine(API.Wallets.List(Identity, HandleWalletRefresh));
            }
            else
            {
                ConsoleText.text = "<color=red><b>Not Authenticated</b></color>\n" + result.message;
            }
        }

        private void HandleWalletRefresh(ListWalletResult result)
        {
            wallets.Clear();
            Debug.Log("List Wallets Responce:\nHas Error: " + result.hasError + "\nMessage:" + result.message);

            if(!result.hasError)
            {
                foreach(var wallet in result.result)
                {
                    wallets.Add(wallet);
                    Debug.Log(wallet.description + " : " + wallet.address);
                    ConsoleText.text += "\n\n<b>Wallet Alias: "+wallet.alias+"</b>\nType: " + wallet.walletType + "\nDescription: " + wallet.description;
                }
            }
        }
    }
}
