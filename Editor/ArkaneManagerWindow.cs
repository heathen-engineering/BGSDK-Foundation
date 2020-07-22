using HeathenEngineering.Arkane.DataModel;
using HeathenEngineering.Arkane.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HeathenEngineering.Arkane.Editor
{
    public class ArkaneManagerWindow : EditorWindow
    {
        public static ArkaneManagerWindow Instance;
        private static bool useUnityId = false;
        //private static bool useAppIdQuery = false;
        //private Vector2 scrollPos = new Vector2();
        //public GUISkin ArkaneSkin;
        private GUIStyle linkLableStyle;
        public Texture2D ArkaneLogo;
        public Texture2D ContractIcon;
        public Texture2D CreateContractIcon;
        public Texture2D RemoveContractIcon;
        public Texture2D TokenIcon;
        public Texture2D CreateTokenIcon;
        public Texture2D RemoveTokenIcon;

        const string settingsPath = "Heathen.Arkane.Manager.Settings";
        const string username = "Heathen.Arkane.Manager.Username";
        const string accessToken = "Heathen.Arkane.Manager.AccessToken";
        const string tokenType = "Heathen.Arkane.Manager.TokenType";
        const string tokenExperation = "Heathen.Arkane.Manager.TokenExperation";
        const string refreshToken = "Heathen.Arkane.Manager.RefreshToken";
        const string refreshExperation = "Heathen.Arkane.Manager.RefreshExperation";

        public int tab = 0;
        private static Identity current;
        private static string storedAccessToken;
        private static string storedTokenType;
        private static DateTime storedTokenExperation;
        private static List<IEnumerator> cooroutines;
        private static string storedRefreshToken;
        private static DateTime storedRefreshExperation;

        private Arkane.Engine.Contract activeContract;
        private Arkane.Engine.Token activeToken;
        private Vector2 scrollPos_NewsArea;
        private Vector2 scrollPos_ContractArea;
        private bool authenticated = false;
        private List<AppId> AppIds;

        //Fake app id index
        private int appIdIndex = 0;
        
        GUIStyle wordWrapTextField;

        #region Arkane Editor Wraps
        public void LogOn()
        {
            var e = Arkane.Editor.EditorUtilities.Authenticate(current, (result) =>
            {
                if (result.hasError)
                {
                    Debug.LogError(result.message);
                }
                else
                {
                    PlayerPrefs.SetString(accessToken, current.authentication.access_token);
                    PlayerPrefs.SetString(tokenType, current.authentication.token_type);
                    PlayerPrefs.SetString(tokenExperation, current.authentication.ExpiresAt.ToBinary().ToString());
                    PlayerPrefs.SetString(refreshToken, current.authentication.refresh_token);
                    PlayerPrefs.SetString(refreshExperation, current.authentication.RefreshExpiresAt.ToBinary().ToString());
                    Debug.Log(result.message);
                }
            });

            StartCoroutine(e);
        }
        
        #endregion

        [MenuItem("Window/Arkane Manager")]
        public static void Init()
        {
            ArkaneManagerWindow window = EditorWindow.GetWindow<ArkaneManagerWindow>("Arkane Manager", new Type[] { typeof(UnityEditor.SceneView) });
            if (current == null)
                current = new Identity();
            cooroutines = new List<IEnumerator>();
            window.TryApplySettings();
            window.Show();
            EditorApplication.update += window.EditorUpdate;
            window.wordWrapTextField = new GUIStyle(EditorStyles.textField);
            window.wordWrapTextField.wordWrap = true;
        }

        public ArkaneManagerWindow()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void OnGUI()
        {
            linkLableStyle = new GUIStyle(EditorStyles.label);
            linkLableStyle.fontStyle = FontStyle.Italic;
            linkLableStyle.normal.textColor = Color.blue;

            if (wordWrapTextField == null)
            {
                wordWrapTextField = new GUIStyle(EditorStyles.textField);
                wordWrapTextField.wordWrap = true;
            }

            FetchStoredToken();
            TryApplySettings();
            DrawSettingsArea();

            switch (tab)
            {
                case 0:
                    DrawHomePage();
                    break;
                case 1:
                    DrawDashoard();
                    break;
                default:
                    tab = 0;
                    DrawHomePage();
                    break;
            }
        }

        private void DrawSettingsArea()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("Active Settings:", GUILayout.Width(100));
            if (Settings.current == null)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("New", EditorStyles.toolbarButton, GUILayout.Width(50)))
                {
                    GUI.FocusControl(null);
                    Settings.current = CreateAsset<Settings>("New Arkane Settings");
                }
                Settings.current = EditorGUILayout.ObjectField(GUIContent.none, Settings.current, typeof(Settings), false, GUILayout.Width(250)) as Settings;
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("New", EditorStyles.toolbarButton, GUILayout.Width(50)))
                {
                    GUI.FocusControl(null);
                    Settings.current = CreateAsset<Settings>("New Arkane Settings");
                }
                if (GUILayout.Button("Clone", EditorStyles.toolbarButton, GUILayout.Width(50)))
                {
                    GUI.FocusControl(null);
                    var current = Settings.current;
                    Settings.current = CreateAsset<Settings>("(Clone) " + Settings.current.name.Replace("(Clone) ", ""));
                    Settings.current.Authentication = new DomainTarget(current.Authentication.Staging, current.Authentication.Production);
                    Settings.current.Business = new DomainTarget(current.Business.Staging, current.Business.Production);
                    Settings.current.API = new DomainTarget(current.API.Staging, current.API.Production);
                    Settings.current.UseStaging = current.UseStaging;
                    Settings.current.AppId = current.AppId;
                    Settings.current.AuthenticationMode = current.AuthenticationMode;
                }
                Settings.current = EditorGUILayout.ObjectField(GUIContent.none, Settings.current, typeof(Settings), false, GUILayout.Width(250)) as Settings;

                var path = AssetDatabase.GetAssetPath(Settings.current);
                PlayerPrefs.SetString(settingsPath, path);

                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndHorizontal();
            PlayerPrefs.Save();
        }

        private void DrawDashboardMenu()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Home", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                GUI.FocusControl(null);
                tab = 0;
            }
            if (GUILayout.Button("Sync", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                GUI.FocusControl(null);
                var syncProc = Arkane.Editor.EditorUtilities.SyncSettings(Settings.current, current);

                StartCoroutine(syncProc);
            }
            if (GUILayout.Button("Portal", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                GUI.FocusControl(null);
                Help.BrowseURL("https://business-staging.arkane.network/applications/" + Settings.current.AppId.id + "/overview");
            }
            if (GUILayout.Button("Help", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawHomePage()
        {
            EditorGUILayout.BeginHorizontal();
            DrawLoginArea();
            DrawNewsArea();
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawDashoard()
        {
            DrawDashboardMenu();
            EditorGUILayout.BeginHorizontal();
            DrawContractListArea();
            DrawEditorArea();
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawContractListArea()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(250), GUILayout.ExpandHeight(true));
            if (Settings.current != null)
            {
                if (Settings.current.Contracts == null)
                    Settings.current.Contracts = new List<Arkane.Engine.Contract>();

                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                var color = GUI.contentColor;
                EditorGUILayout.LabelField("Contracts", EditorStyles.whiteLabel, GUILayout.Width(200));
                GUI.contentColor = new Color(0.25f, 0.75f, 0.25f, 1);
                if (GUILayout.Button(CreateContractIcon, EditorStyles.toolbarButton, GUILayout.Width(25)))
                {
                    GUI.FocusControl(null);

                    Arkane.Engine.Contract nContract = new Arkane.Engine.Contract();
                    nContract.name = "New Contract";
                    nContract.systemName = "New Contract";
                    nContract.UpdatedFromServer = false;
                    nContract.Tokens = new List<Arkane.Engine.Token>();
                    Settings.current.Contracts.Add(nContract);
                    EditorUtility.SetDirty(Settings.current);
                    AssetDatabase.AddObjectToAsset(nContract, Settings.current);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(nContract));

                    activeContract = nContract;
                    activeToken = null;
                }
                GUI.contentColor = color;
                EditorGUILayout.EndHorizontal();;

                if (Settings.current.Contracts.Count > 0)
                {
                    if (activeContract == null)
                        activeContract = Settings.current.Contracts[0];

                    scrollPos_ContractArea = EditorGUILayout.BeginScrollView(scrollPos_ContractArea);
                    foreach (var con in Settings.current.Contracts)
                    {
                        DrawContractEntryDesigner(con);
                    }
                    EditorGUILayout.EndScrollView();

                    Settings.current.Contracts.RemoveAll(p => p == null);
                }
            }
            else
                tab = 0;
            EditorGUILayout.EndVertical();
        }
        
        private void DrawContractEntryDesigner(Arkane.Engine.Contract contract)
        {
            if (contract == null)
                return;

            bool hasRemoved = false;

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField(new GUIContent(ContractIcon), GUILayout.Width(20));            
            if (GUILayout.Toggle(activeContract == contract && activeToken == null, contract.systemName, EditorStyles.toolbarButton))
            {
                if(activeContract != contract || activeToken != null)
                {
                    GUI.FocusControl(null);
                }

                activeContract = contract;
                activeToken = null;
            }

            var color = GUI.contentColor;
            GUI.contentColor = new Color(0.25f, 0.75f, 0.25f, 1);
            if (GUILayout.Button(CreateTokenIcon, EditorStyles.toolbarButton, GUILayout.Width(25)))
            {
                GUI.FocusControl(null);
                Arkane.Engine.Token nToken = new Arkane.Engine.Token();
                nToken.name = contract.name + " : New Token";
                nToken.systemName = "New Token";
                nToken.UpdatedFromServer = false;
                nToken.contract = contract;

                if (contract.Tokens == null)
                    contract.Tokens = new List<Arkane.Engine.Token>();

                contract.Tokens.Add(nToken);
                AssetDatabase.AddObjectToAsset(nToken, contract);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Settings.current));

                activeToken = nToken;
            }
            GUI.contentColor = new Color(1,0.50f,0.50f,1);
            if (GUILayout.Button(RemoveContractIcon, EditorStyles.toolbarButton, GUILayout.Width(25)))
            {
                GUI.FocusControl(null);
                if (EditorUtility.DisplayDialog("Delete Contract", "Are you sure you want to delete [" + contract.name + "] and all of its tokens.\n\nNote this will not remove a deployed contract from the backend service it only removes the contract from the configuraiton in your applicaiton.", "Delete", "Cancel"))
                {
                    if(contract.Tokens != null)
                    {
                        foreach(var token in contract.Tokens)
                        {
                            DestroyImmediate(token, true);
                            hasRemoved = true;
                        }
                    }

                    DestroyImmediate(contract, true);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Settings.current));
                }
            }
            GUI.contentColor = color;
            EditorGUILayout.EndHorizontal();

            if (hasRemoved)
                return;

            if (contract.Tokens == null)
                contract.Tokens = new List<Arkane.Engine.Token>();

            contract.Tokens.Sort((a, b) => { return a.systemName.CompareTo(b.systemName); });

            foreach(var token in contract.Tokens)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                EditorGUILayout.LabelField("", GUILayout.Width(20));
                EditorGUILayout.LabelField(new GUIContent(TokenIcon), GUILayout.Width(20));
                if (GUILayout.Toggle(activeContract == contract && activeToken == token, token.Data.name, EditorStyles.toolbarButton))
                {
                    if (activeToken != token)
                        GUI.FocusControl(null);

                    activeContract = token.contract;
                    activeToken = token;
                }
                GUI.contentColor = new Color(1, 0.5f, 0.5f, 1);
                if (GUILayout.Button(RemoveTokenIcon, EditorStyles.toolbarButton, GUILayout.Width(25)))
                {
                    GUI.FocusControl(null);
                    if (EditorUtility.DisplayDialog("Delete Token", "Are you sure you want to delete [" + contract.name + "].\n\nNote this will not remove a deployed token from the backend service it only removes the token from the configuraiton in your applicaiton.", "Delete", "Cancel"))
                    {
                        DestroyImmediate(token, true);
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Settings.current));
                        hasRemoved = true;
                    }
                }
                GUI.contentColor = color;

                EditorGUILayout.EndHorizontal();
            }

            if (hasRemoved)
                contract.Tokens.RemoveAll(p => p == null);
        }

        private void DrawEditorArea()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            if (Settings.current != null)
            {
                if (Settings.current.Contracts == null)
                    Settings.current.Contracts = new List<Arkane.Engine.Contract>();

                if (activeContract != null && activeToken == null)
                {
                    DrawContractEditor();
                }
                else if(activeToken != null)
                {
                    DrawTokenEditor();
                }
                else
                {
                    EditorGUILayout.LabelField("No Object Selected", EditorStyles.whiteLargeLabel);
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    EditorGUILayout.TextField("Name", "");
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Description");
                    EditorGUILayout.TextArea("", GUILayout.Height(110));
                }
            }
            else
                tab = 0;

            EditorGUILayout.EndVertical();
        }

        private void DrawContractEditor()
        {
            if (activeContract.UpdatedFromServer)
            {
                EditorGUILayout.LabelField(new GUIContent("Published Contract Properties: " + activeContract.systemName, "Contract settings for " + activeContract.systemName), EditorStyles.whiteLargeLabel);
                EditorGUILayout.LabelField(new GUIContent("Confirmed: " + (activeContract.Data.confirmed ? "Yes" : "No"), "Indicates rather or not the contract has been confirmed on the chain. This will always be no untill the contract is first deployed."));
                EditorGUILayout.LabelField("Contract Address: " + activeContract.Data.address);
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Description");
                EditorGUILayout.SelectableLabel(activeContract.description, EditorStyles.textArea, GUILayout.Height(110));
            }
            else
            {
                EditorGUILayout.LabelField("Contract Properties: " + activeContract.systemName, EditorStyles.whiteLargeLabel);
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                var nameVal = EditorGUILayout.TextField("Name", activeContract.systemName);
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Description");
                var descVal = EditorGUILayout.TextArea(activeContract.description, GUILayout.Height(110));

                if (nameVal != activeContract.systemName
                    || descVal != activeContract.description)
                {
                    Undo.RecordObject(activeContract, "textEdit");
                    activeContract.name = nameVal;
                    activeContract.systemName = nameVal;
                    activeContract.description = descVal;
                    EditorUtility.SetDirty(activeContract);

                    foreach (var token in activeContract.Tokens)
                    {
                        token.name = activeContract.name + " : " + token.systemName;
                        EditorUtility.SetDirty(token);
                    }

                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Settings.current));
                }
            }
        }

        private void DrawTokenEditor()
        {
            if (activeToken.UpdatedFromServer)
            {
                EditorGUILayout.LabelField(new GUIContent("Published Token Properties: " + activeToken.Data.name, "Token settings for " + activeToken.Data.name + " token."), EditorStyles.whiteLargeLabel);
                if (activeToken.Data.isNonFungible)
                    EditorGUILayout.LabelField(new GUIContent("Non Fungible", "Non fungible token definition, counts of this token are always a whole number typically used to represent items."));
                else
                    EditorGUILayout.LabelField(new GUIContent("Fungible (" + activeToken.Data.decimals.ToString() + ")", "Fungible token definition supporting " + activeToken.Data.decimals.ToString() + " decimal places, counts of this token are decimal and can represent fractional values, typically used for currencies."));
                EditorGUILayout.LabelField(new GUIContent("Confirmed: " + (activeToken.Data.confirmed ? "Yes" : "No"), "Indicates rather or not the token has been confirmed on the chain. This will always be no untill the tokin is first deployed."));
                EditorGUILayout.LabelField("Type ID: " + activeToken.Data.contractTypeId.ToString());
                EditorGUILayout.LabelField("Token Address: " + activeToken.Data.contractAddress);

                if (!string.IsNullOrEmpty(activeToken.Data.url))
                {
                    if (GUILayout.Button("Metadata URL: " + activeToken.Data.url, linkLableStyle))
                    {
                        Help.BrowseURL(activeToken.Data.url);
                    }
                }
                else
                    EditorGUILayout.LabelField("Metadata URL: [NULL]");

                EditorGUILayout.LabelField(new GUIContent("Description", "Discription of the token"));
                EditorGUILayout.SelectableLabel(activeToken.description, EditorStyles.textArea, GUILayout.Height(110));
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Properties: " + (string.IsNullOrEmpty(activeToken.Data.properties) ? "[NULL]" : activeToken.Data.properties));

                Color newCol;
                if (!string.IsNullOrEmpty(activeToken.Data.backgroundColor) && ColorUtility.TryParseHtmlString(activeToken.Data.backgroundColor, out newCol))
                {
                    EditorGUILayout.ColorField("Background Color: " + activeToken.Data.backgroundColor, newCol);
                }
                else
                {
                    EditorGUILayout.LabelField("Background Color: [NULL]");
                }

                if (!string.IsNullOrEmpty(activeToken.Data.image))
                {
                    if (GUILayout.Button("Main Image URL: " + activeToken.Data.image, linkLableStyle))
                    {
                        Help.BrowseURL(activeToken.Data.image);
                    }
                }
                else
                    EditorGUILayout.LabelField("Main Image URL: [NULL]");

                if (!string.IsNullOrEmpty(activeToken.Data.imagePreview))
                {
                    if (GUILayout.Button("Preview Image URL: " + activeToken.Data.imagePreview, linkLableStyle))
                    {
                        Help.BrowseURL(activeToken.Data.imagePreview);
                    }
                }
                else
                    EditorGUILayout.LabelField("Preview Image URL: [NULL]");

                if (!string.IsNullOrEmpty(activeToken.Data.imageThumbnail))
                {
                    if (GUILayout.Button("Thumbnail Image URL: " + activeToken.Data.imageThumbnail, linkLableStyle))
                    {
                        Help.BrowseURL(activeToken.Data.imageThumbnail);
                    }
                }
                else
                    EditorGUILayout.LabelField("Thumbnail Image URL: [NULL]");
            }
            else
            {
                EditorGUILayout.LabelField(new GUIContent("New Token Properties: " + activeToken.Data.name, "Token settings for " + activeToken.Data.name + " token."), EditorStyles.whiteLargeLabel);
                var nameVal = EditorGUILayout.TextField(new GUIContent("Name", "The name to be assigned to the token."), activeToken.Data.name);
                var fungableValue = EditorGUILayout.Toggle(new GUIContent("Is Fungible", "If true then the token will be created as type fungible meaning that its value can be a fraction of a whole such as with currency values.\nIf false then this token is non fungible and counts of it are a whole number typical of items."), !activeToken.Data.isNonFungible);
                var decimalValue = activeToken.Data.decimals;
                if(fungableValue)
                {
                    decimalValue = System.Convert.ToUInt32(EditorGUILayout.IntField(new GUIContent("Decimals", "The number of decimal places the fungible token has."), System.Convert.ToInt32(decimalValue)));
                }
                var metadataUrl = EditorGUILayout.TextField(new GUIContent("Metadata URL", "The URL with more information about the token."), activeToken.Data.url);
                EditorGUILayout.LabelField(new GUIContent("Description", "Discription of the token"));
                var descVal = EditorGUILayout.TextArea(activeToken.Data.description, GUILayout.Height(110));
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                var propertyString = EditorGUILayout.TextField(new GUIContent("Properties", "Free text field of extra properties assoceated with this token."), activeToken.Data.properties);

                Color newCol = Color.white;
                var HTMLColorString = activeToken.Data.backgroundColor;
                ColorUtility.TryParseHtmlString(activeToken.Data.backgroundColor, out newCol);
                newCol = EditorGUILayout.ColorField(new GUIContent("Background Color", "Background color to be applied behing the image."), newCol);
                HTMLColorString = ColorUtility.ToHtmlStringRGBA(newCol);

                var imageString = EditorGUILayout.TextField(new GUIContent("Main Image URL", "The main image used for this token."), activeToken.Data.image);
                var previewImageString = EditorGUILayout.TextField(new GUIContent("Preview Image URL", "Preview image used for this token."), activeToken.Data.imagePreview);
                var thumbnailImageString = EditorGUILayout.TextField(new GUIContent("Thumbnail Image URL", "Thumbnail image used for this token."), activeToken.Data.imageThumbnail);

                if (nameVal != activeToken.Data.name
                    || descVal != activeToken.Data.description
                    || fungableValue == activeToken.Data.isNonFungible
                    || decimalValue != activeToken.Data.decimals
                    || metadataUrl != activeToken.Data.url
                    || propertyString != activeToken.Data.properties
                    || HTMLColorString != activeToken.Data.backgroundColor
                    || imageString != activeToken.Data.image
                    || previewImageString != activeToken.Data.imagePreview
                    || thumbnailImageString != activeToken.Data.imageThumbnail)
                {
                    Undo.RecordObject(activeToken, "textEdit");
                    activeToken.name = activeContract.name + " : " + nameVal;
                    activeToken.Data.name = nameVal;
                    activeToken.Data.description = descVal;
                    activeToken.Data.isNonFungible = fungableValue;
                    activeToken.Data.decimals = decimalValue;
                    activeToken.Data.url = metadataUrl;
                    activeToken.Data.properties = propertyString;
                    activeToken.Data.backgroundColor = HTMLColorString;
                    activeToken.Data.image = imageString;
                    activeToken.Data.imagePreview = previewImageString;
                    activeToken.Data.imageThumbnail = thumbnailImageString;
                    activeToken.Data.isNonFungible = !fungableValue;
                    EditorUtility.SetDirty(activeToken);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Settings.current));
                }
            }
        }
                        
        private void DrawLoginArea()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox,GUILayout.Width(300), GUILayout.MinHeight(this.position.height - 45));
            if (current == null)
                current = new Identity();

            //TODO: Remove this once we get the Unity ID integraiton
            current.password = "Jodi@01092019";

            EditorGUILayout.LabelField("Configuration", EditorStyles.whiteLargeLabel);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            var r = EditorGUILayout.GetControlRect(false, GUILayout.Width(250), GUILayout.Height(250));
            GUI.DrawTexture(r, ArkaneLogo, ScaleMode.ScaleToFit, true, 0);
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (!useUnityId)
            {
                #region Manual Auth
                EditorGUILayout.LabelField("Username", EditorStyles.boldLabel);
                var usernameValue = "";
                if (PlayerPrefs.HasKey(username))
                {
                    usernameValue = PlayerPrefs.GetString(username);
                }
                usernameValue = EditorGUILayout.TextField(GUIContent.none, usernameValue);
                current.username = usernameValue;
                PlayerPrefs.SetString(username, usernameValue);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Password", EditorStyles.boldLabel);
                current.password = EditorGUILayout.PasswordField(GUIContent.none, current.password);
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Sign Up", GUILayout.Height(25)))
                {
                }
                EditorGUILayout.LabelField("", GUILayout.Width(25));
                if (GUILayout.Button("Login", GUILayout.Height(25)))
                {
                    if (Settings.current == null)
                    {
                        Debug.LogError("You must provide a Arkane Settings object before you can authenticate!");
                    }
                    else
                    {
                        LogOn();
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                #endregion
            }

            if (authenticated)
            {
                if (AppIds == null)
                {
                    AppIds = new List<AppId>();

                    var e = Arkane.Editor.EditorUtilities.ListApplications(current, (result) =>
                    {
                        if(!result.hasError)
                        {
                            AppIds.Clear();
                            AppIds.AddRange(result.result);

                            foreach (var a in AppIds)
                            {
                                if (string.IsNullOrEmpty(a.name))
                                {
                                    a.name = "<< No Name >> client id: " + a.clientId;
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError(result.message);
                        }
                    });

                    StartCoroutine(e);
                }

                //If we have authenticated with Unity ID
                if (Settings.current != null)
                {

                    if (AppIds.Count > 0)
                    {
                        string[] options = new string[AppIds.Count + 2];
                        options[0] = "<< Refresh List >>";
                        options[1] = "<< Create New Application >>";
                        for (int i = 0; i < AppIds.Count; i++)
                        {
                            options[2 + i] = AppIds[i].name;
                        }

                        appIdIndex = EditorGUILayout.Popup(appIdIndex, options);
                        if (appIdIndex == 0)
                        {
                            appIdIndex = 2;
                            var e = Arkane.Editor.EditorUtilities.ListApplications(current, (result) =>
                            {
                                if (!result.hasError)
                                {
                                    AppIds.Clear();
                                    AppIds.AddRange(result.result);
                                    
                                    foreach(var a in AppIds)
                                    {
                                        if(string.IsNullOrEmpty( a.name))
                                        {
                                            a.name = "<< No Name >> client id: " + a.clientId;
                                        }
                                    }

                                    Debug.Log("Returned " + AppIds.Count + " applications");
                                }
                                else
                                {
                                    Debug.LogError(result.message + "\nHTTP Code: " + result.httpCode);
                                }
                            });

                            StartCoroutine(e);
                        }
                        else if (appIdIndex == 1)
                        {
                            appIdIndex = 2;
                            if (Settings.current.UseStaging)
                                Help.BrowseURL("https://business-staging.arkane.network/applications");
                            else
                                Help.BrowseURL("https://business.arkane.network/applications");
                        }
                        else
                        {
                            Settings.current.AppId = AppIds[appIdIndex - 2];
                        }
                    }
                    else
                    {
                        //TODO: tell the user they have no app Ids so they need to refresh the list after they have made an applicaiton on Arkane Network
                        string[] options = new string[] { "No Application Found", "<< Refresh List >>", "<< Create New Application >>" };

                        appIdIndex = EditorGUILayout.Popup(appIdIndex, options);

                        if (appIdIndex == 1)
                        {
                            appIdIndex = 0;
                            var e = Arkane.Editor.EditorUtilities.ListApplications(current, (result) =>
                            {
                                if (!result.hasError)
                                {
                                    AppIds.Clear();
                                    AppIds.AddRange(result.result);

                                    foreach (var a in AppIds)
                                    {
                                        if (string.IsNullOrEmpty(a.name))
                                        {
                                            a.name = "<< No Name >> client id: " + a.clientId;
                                        }
                                    }

                                    Debug.Log("Returned " + AppIds.Count + " applications");
                                }
                                else
                                {
                                    Debug.LogError(result.message + "\nHTTP Code: " + result.httpCode);
                                }
                            });

                            StartCoroutine(e);
                        }
                        else if (appIdIndex == 2)
                        {
                            appIdIndex = 0;
                            if (Settings.current.UseStaging)
                                Help.BrowseURL("https://business-staging.arkane.network/applications");
                            else
                                Help.BrowseURL("https://business.arkane.network/applications");
                        }
                    }

                }
            }
            else
            {
                EditorGUILayout.LabelField("App ID", EditorStyles.boldLabel);
                Settings.current.AppId.id = EditorGUILayout.TextField(GUIContent.none, Settings.current.AppId.id);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (GUILayout.Button("Dashboard", GUILayout.Height(25)))
            {
                tab = 1;
            }
            if (GUILayout.Button("Asset Store", GUILayout.Height(25)))
            {
            }
            if (GUILayout.Button("Portal", GUILayout.Height(25)))
            {
            }
            if (GUILayout.Button("Marketplace", GUILayout.Height(25)))
            {
            }
            if (GUILayout.Button("Learning", GUILayout.Height(25)))
            {
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            PlayerPrefs.Save();
        }
        
        private void DrawNewsArea()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(position.width - 310), GUILayout.MinHeight(position.height - 45));
            EditorGUILayout.LabelField("News", EditorStyles.whiteLargeLabel);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(10));
            EditorGUILayout.BeginVertical();
            scrollPos_NewsArea = EditorGUILayout.BeginScrollView(scrollPos_NewsArea);
            for (int i = 0; i < 50; i++)
            {
                DrawNewsItem("Example Article 2020-Jan-01"
                    , "This is an example of the first couple of lines being presented by the control. This can be shorter or longer as desired but would be keep to a resonable length and ended with a link or button to the source of the information."
                    , "http://www.google.com");
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawNewsItem(string title, string body, string url)
        {
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Read More", EditorStyles.miniButton, GUILayout.Width(65)))
            {
                Help.BrowseURL(url);
            }
            EditorGUILayout.LabelField("Example Article 2020-Jan-01", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("This is an example of the first couple of lines being presented by the control. This can be shorter or longer as desired but would be keep to a resonable length and ended with a link or button to the source of the information.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        
        private T CreateAsset<T>(string name) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }

        private void TryApplySettings()
        {
            if (Settings.current == null && PlayerPrefs.HasKey(settingsPath))
            {
                var target = AssetDatabase.LoadAssetAtPath<Settings>(PlayerPrefs.GetString(settingsPath));
                Settings.current = target;
            }
        }

        private void FetchStoredToken()
        {
            if (PlayerPrefs.HasKey(accessToken) && PlayerPrefs.HasKey(tokenType) && PlayerPrefs.HasKey(tokenExperation) && PlayerPrefs.HasKey(refreshExperation) && PlayerPrefs.HasKey(refreshToken))
            {
                storedAccessToken = PlayerPrefs.GetString(accessToken);
                storedTokenType = PlayerPrefs.GetString(tokenType);
                storedRefreshToken = PlayerPrefs.GetString(refreshToken);

                var expBuffer = PlayerPrefs.GetString(tokenExperation);
                long timeBuffer;
                var tResult = false;
                if (long.TryParse(expBuffer, out timeBuffer))
                {
                    storedTokenExperation = DateTime.FromBinary(timeBuffer);

                    //If the token is set to expire in 120 seconds then mark it as a failed token
                    if (storedTokenExperation > DateTime.Now - new TimeSpan(0, 2, 0))
                    {
                        authenticated = true;
                        titleContent.text = "Arkane Manager (Online)";
                        tResult = true;

                        if (current == null)
                            current = new Identity();

                        if (current.authentication == null)
                            current.authentication = new AuthenticationResponce();

                        current.authentication.access_token = storedAccessToken;
                        current.authentication.token_type = storedTokenType;
                        current.authentication.refresh_token = storedRefreshToken;
                    }
                }

                expBuffer = PlayerPrefs.GetString(refreshExperation);
                if (long.TryParse(expBuffer, out timeBuffer))
                {
                    storedRefreshExperation = DateTime.FromBinary(timeBuffer);

                    //If the token is failed e.g. has or will expire in 120 seconds then try and refresh it ... assuming our refresh token is valid
                    if (!tResult)
                    {
                        if (storedRefreshExperation > DateTime.Now)
                        {

                            var e = Arkane.Editor.EditorUtilities.Authenticate(current, (result) =>
                            {
                                if (result.hasError)
                                {
                                    Debug.LogError(result.message);
                                }
                                else
                                {
                                    PlayerPrefs.SetString(accessToken, current.authentication.access_token);
                                    PlayerPrefs.SetString(tokenType, current.authentication.token_type);
                                    PlayerPrefs.SetString(tokenExperation, current.authentication.ExpiresAt.ToBinary().ToString());
                                    PlayerPrefs.SetString(refreshToken, current.authentication.refresh_token);
                                    PlayerPrefs.SetString(refreshExperation, current.authentication.RefreshExpiresAt.ToBinary().ToString());
                                    Debug.Log(result.message);
                                }
                            });
                            authenticated = false;
                            titleContent.text = "Arkane Manager (Refreshing)";
                            StartCoroutine(e);

                            
                        }
                        else
                        {
                            authenticated = false;
                            titleContent.text = "Arkane Manager (Offline)";
                        }
                    }
                }
            }
            else
            {
                authenticated = false;
                titleContent.text = "Arkane Manager (Offline)";
            }
        }

        private void StartCoroutine(IEnumerator handle)
        {
            if (cooroutines == null)
            {
                EditorApplication.update -= EditorUpdate;
                EditorApplication.update += EditorUpdate;
                cooroutines = new List<IEnumerator>();
            }

            cooroutines.Add(handle);
        }


        public void EditorUpdate()
        {
            List<IEnumerator> done = new List<IEnumerator>();

            if (cooroutines != null)
            {
                foreach (var e in cooroutines)
                {
                    if (!e.MoveNext())
                        done.Add(e);
                    else
                    {
                        if (e.Current != null)
                            Debug.Log(e.Current.ToString());
                    }
                }
            }

            foreach (var d in done)
                cooroutines.Remove(d);
        }
    }
}
