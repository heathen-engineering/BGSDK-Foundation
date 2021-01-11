using HeathenEngineering.BGSDK.DataModel;
using HeathenEngineering.BGSDK.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HeathenEngineering.BGSDK.Editor
{
    public class BGSDKManagerWindow : EditorWindow
    {
        public static BGSDKManagerWindow Instance;
        private static bool useUnityId = false;
        //private static bool useAppIdQuery = false;
        //private Vector2 scrollPos = new Vector2();
        //public GUISkin BGSDKSkin;
        private GUIStyle linkLableStyle;
        public Texture2D BGSDKLogo;
        public Texture2D ContractIcon;
        public Texture2D CreateContractIcon;
        public Texture2D RemoveContractIcon;
        public Texture2D TokenIcon;
        public Texture2D CreateTokenIcon;
        public Texture2D RemoveTokenIcon;

        const string settingsPath = "Heathen.BGSDK.Manager.Settings";

        public int tab = 0;
        private static List<IEnumerator> cooroutines;

        private BGSDK.Engine.Contract activeContract;
        private BGSDK.Engine.Token activeToken;
        private Vector2 scrollPos_NewsArea;
        private Vector2 scrollPos_ContractArea;
        private bool showingSecretValue = false;
        GUIStyle wordWrapTextField;

        [MenuItem("Window/BGSDK Manager")]
        public static void Init()
        {
            BGSDKManagerWindow window = EditorWindow.GetWindow<BGSDKManagerWindow>("BGSDK Manager", new Type[] { typeof(UnityEditor.SceneView) });
            if (Settings.user == null)
                Settings.user = new Identity();
            cooroutines = new List<IEnumerator>();
            window.Show();
            EditorApplication.update += window.EditorUpdate;
            window.wordWrapTextField = new GUIStyle(EditorStyles.textField);
            window.wordWrapTextField.wordWrap = true;
        }

        public BGSDKManagerWindow()
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
            linkLableStyle.normal.textColor = new Color(0.5f, 0.7877359f, 1f);

            if (wordWrapTextField == null)
            {
                wordWrapTextField = new GUIStyle(EditorStyles.textField);
                wordWrapTextField.wordWrap = true;
            }

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
            var currentPath = EditorPrefs.GetString(settingsPath);

            if (!string.IsNullOrEmpty(currentPath))
            {
                var target = AssetDatabase.LoadAssetAtPath<Settings>(currentPath);
                if (target != null)
                    Settings.current = target;
            }

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("Active Settings:", GUILayout.Width(100));
            if (Settings.current == null)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("New", EditorStyles.toolbarButton, GUILayout.Width(50)))
                {
                    GUI.FocusControl(null);
                    Settings.current = CreateAsset<Settings>("New BGSDK Settings");
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
                    Settings.current = CreateAsset<Settings>("New BGSDK Settings");
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
                    //Settings.current.AuthenticationMode = current.AuthenticationMode;
                }
                Settings.current = EditorGUILayout.ObjectField(GUIContent.none, Settings.current, typeof(Settings), false, GUILayout.Width(250)) as Settings;

                var path = AssetDatabase.GetAssetPath(Settings.current);
                EditorPrefs.SetString(settingsPath, path);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndHorizontal();
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
                var syncProc = BGSDK.Editor.EditorUtilities.SyncSettings();

                StartCoroutine(syncProc);
            }
            if (GUILayout.Button("Portal", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                GUI.FocusControl(null);
                Help.BrowseURL("https://business-staging.arkane.network/applications/" + Settings.current.AppId.applicationId + "/overview");
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
                    Settings.current.Contracts = new List<BGSDK.Engine.Contract>();

                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                var color = GUI.contentColor;
                EditorGUILayout.LabelField("Contracts", EditorStyles.whiteLabel, GUILayout.Width(200));
                GUI.contentColor = new Color(0.25f, 0.75f, 0.25f, 1);
                if (GUILayout.Button(CreateContractIcon, EditorStyles.toolbarButton, GUILayout.Width(25)))
                {
                    GUI.FocusControl(null);

                    BGSDK.Engine.Contract nContract = new BGSDK.Engine.Contract();
                    nContract.name = "New Contract";
                    nContract.systemName = "New Contract";
                    nContract.UpdatedFromServer = false;
                    nContract.Tokens = new List<BGSDK.Engine.Token>();
                    Settings.current.Contracts.Add(nContract);
                    EditorUtility.SetDirty(Settings.current);
                    AssetDatabase.AddObjectToAsset(nContract, Settings.current);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(nContract));

                    activeContract = nContract;
                    activeToken = null;
                }
                GUI.contentColor = color;
                EditorGUILayout.EndHorizontal(); ;

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

        private void DrawContractEntryDesigner(BGSDK.Engine.Contract contract)
        {
            if (contract == null)
                return;

            bool hasRemoved = false;

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField(new GUIContent(ContractIcon), GUILayout.Width(20));
            if (GUILayout.Toggle(activeContract == contract && activeToken == null, contract.systemName, EditorStyles.toolbarButton))
            {
                if (activeContract != contract || activeToken != null)
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
                BGSDK.Engine.Token nToken = new BGSDK.Engine.Token();
                nToken.name = contract.name + " : New Token";
                nToken.SystemName = "New Token";
                nToken.UpdatedFromServer = false;
                nToken.contract = contract;

                if (contract.Tokens == null)
                    contract.Tokens = new List<BGSDK.Engine.Token>();

                contract.Tokens.Add(nToken);
                AssetDatabase.AddObjectToAsset(nToken, contract);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Settings.current));

                activeToken = nToken;
            }
            GUI.contentColor = new Color(1, 0.50f, 0.50f, 1);
            if (GUILayout.Button(RemoveContractIcon, EditorStyles.toolbarButton, GUILayout.Width(25)))
            {
                GUI.FocusControl(null);
                if (EditorUtility.DisplayDialog("Delete Contract", "Are you sure you want to delete [" + contract.name + "] and all of its tokens.\n\nNote this will not remove a deployed contract from the backend service it only removes the contract from the configuraiton in your applicaiton.", "Delete", "Cancel"))
                {
                    if (contract.Tokens != null)
                    {
                        foreach (var token in contract.Tokens)
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
                contract.Tokens = new List<BGSDK.Engine.Token>();

            contract.Tokens.Sort((a, b) => { return a.SystemName.CompareTo(b.SystemName); });

            foreach (var token in contract.Tokens)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                EditorGUILayout.LabelField("", GUILayout.Width(20));
                EditorGUILayout.LabelField(new GUIContent(TokenIcon), GUILayout.Width(20));
                if (GUILayout.Toggle(activeContract == contract && activeToken == token, token.SystemName, EditorStyles.toolbarButton))
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
                    Settings.current.Contracts = new List<BGSDK.Engine.Contract>();

                if (activeContract != null && activeToken == null)
                {
                    DrawContractEditor();
                }
                else if (activeToken != null)
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
                EditorGUILayout.LabelField("Contract ID: " + activeContract.Data.id);
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
                        token.name = activeContract.name + " : " + token.SystemName;
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
                EditorGUILayout.LabelField(new GUIContent("Published Token Properties: " + activeToken.SystemName, "Token settings for " + activeToken.SystemName + " token."), EditorStyles.whiteLargeLabel);
                if (activeToken.IsNonFungible)
                    EditorGUILayout.LabelField(new GUIContent("Non Fungible", "Non fungible token definition, counts of this token are always a whole number typically used to represent items."));
                else
                    EditorGUILayout.LabelField("Fungible (" + (activeToken.Decimals == 0 ? "whole numbers" : (activeToken.Decimals.ToString() + " decimal places")) + ")");
                EditorGUILayout.LabelField(new GUIContent("Confirmed: " + (activeToken.Confirmed ? "Yes" : "No"), "Indicates rather or not the token has been confirmed on the chain. This will always be no untill the tokin is first deployed."));
                EditorGUILayout.SelectableLabel("Token ID: " + activeToken.Id.ToString());
                EditorGUILayout.SelectableLabel("Token Address: " + activeToken.Address);

                if (!string.IsNullOrEmpty(activeToken.Url))
                {
                    if (GUILayout.Button("Metadata URL: " + activeToken.Url, linkLableStyle))
                    {
                        Help.BrowseURL(activeToken.Url);
                    }
                }
                else
                    EditorGUILayout.LabelField("Metadata URL: [NULL]");

                EditorGUILayout.LabelField(new GUIContent("Description", "Discription of the token"));
                EditorGUILayout.SelectableLabel(activeToken.Description, EditorStyles.textArea, GUILayout.Height(110));
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                //EditorGUILayout.SelectableLabel("Properties: " + (string.IsNullOrEmpty(activeToken.Data.properties) ? "[NULL]" : activeToken.Data.properties));

                Color newCol;
                if (!string.IsNullOrEmpty(activeToken.BackgroundColor) && ColorUtility.TryParseHtmlString(activeToken.BackgroundColor, out newCol))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.SelectableLabel("Background Color: " + activeToken.BackgroundColor);
                    EditorGUILayout.ColorField(newCol);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.SelectableLabel("Background Color: [NULL]");
                }

                if (!string.IsNullOrEmpty(activeToken.Image))
                {
                    if (GUILayout.Button("Main Image URL: " + activeToken.Image, linkLableStyle))
                    {
                        Help.BrowseURL(activeToken.Image);
                    }
                }
                else
                    EditorGUILayout.LabelField("Main Image URL: [NULL]");

                if (!string.IsNullOrEmpty(activeToken.ImagePreview))
                {
                    if (GUILayout.Button("Preview Image URL: " + activeToken.ImagePreview, linkLableStyle))
                    {
                        Help.BrowseURL(activeToken.ImagePreview);
                    }
                }
                else
                    EditorGUILayout.LabelField("Preview Image URL: [NULL]");

                if (!string.IsNullOrEmpty(activeToken.ImageThumbnail))
                {
                    if (GUILayout.Button("Thumbnail Image URL: " + activeToken.ImageThumbnail, linkLableStyle))
                    {
                        Help.BrowseURL(activeToken.ImageThumbnail);
                    }
                }
                else
                    EditorGUILayout.LabelField("Thumbnail Image URL: [NULL]");
            }
            else
            {
                EditorGUILayout.LabelField(new GUIContent("New Token Properties: " + activeToken.SystemName, "Token settings for " + activeToken.SystemName + " token."), EditorStyles.whiteLargeLabel);
                var nameVal = EditorGUILayout.TextField(new GUIContent("Name", "The name to be assigned to the token."), activeToken.SystemName);
                var fungableValue = EditorGUILayout.Toggle(new GUIContent("Is Fungible", "If true then the token will be created as type fungible meaning that its value can be a fraction of a whole such as with currency values.\nIf false then this token is non fungible and counts of it are a whole number typical of items."), !activeToken.IsNonFungible);
                var decimalValue = activeToken.Decimals;
                if (fungableValue)
                {
                    decimalValue = System.Convert.ToUInt32(EditorGUILayout.IntField(new GUIContent("Decimals", "The number of decimal places the fungible token has."), System.Convert.ToInt32(decimalValue)));
                }
                var metadataUrl = EditorGUILayout.TextField(new GUIContent("Metadata URL", "The URL with more information about the token."), activeToken.Url);
                EditorGUILayout.LabelField(new GUIContent("Description", "Discription of the token"));
                var descVal = EditorGUILayout.TextArea(activeToken.Description, GUILayout.Height(110));
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                //var propertyString = EditorGUILayout.TextField(new GUIContent("Properties", "Free text field of extra properties assoceated with this token."), activeToken.Data.properties);

                Color newCol;
                string HTMLColorString;
                ColorUtility.TryParseHtmlString("#" + activeToken.BackgroundColor, out newCol);
                newCol = EditorGUILayout.ColorField(new GUIContent("Background Color", "Background color to be applied behing the image."), newCol);
                HTMLColorString = ColorUtility.ToHtmlStringRGBA(newCol);

                var imageString = EditorGUILayout.TextField(new GUIContent("Main Image URL", "The main image used for this token."), activeToken.Image);
                var previewImageString = EditorGUILayout.TextField(new GUIContent("Preview Image URL", "Preview image used for this token."), activeToken.ImagePreview);
                var thumbnailImageString = EditorGUILayout.TextField(new GUIContent("Thumbnail Image URL", "Thumbnail image used for this token."), activeToken.ImageThumbnail);

                if (nameVal != activeToken.SystemName
                    || descVal != activeToken.Description
                    || fungableValue == activeToken.IsNonFungible
                    || decimalValue != activeToken.Decimals
                    || metadataUrl != activeToken.Url
                    || HTMLColorString != activeToken.BackgroundColor
                    || imageString != activeToken.Image
                    || previewImageString != activeToken.ImagePreview
                    || thumbnailImageString != activeToken.ImageThumbnail)
                {
                    Undo.RecordObject(activeToken, "textEdit");
                    activeToken.name = activeContract.name + " : " + nameVal;
                    activeToken.SystemName = nameVal;
                    activeToken.Description = descVal;
                    activeToken.Decimals = decimalValue;
                    activeToken.Url = metadataUrl;
                    activeToken.BackgroundColor = HTMLColorString;
                    activeToken.Image = imageString;
                    activeToken.ImagePreview = previewImageString;
                    activeToken.ImageThumbnail = thumbnailImageString;
                    activeToken.IsNonFungible = !fungableValue;
                    EditorUtility.SetDirty(activeToken);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Settings.current));
                }
            }
        }

        private void DrawLoginArea()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(300), GUILayout.MinHeight(this.position.height - 45));
            try
            {

                if (Settings.user == null)
                    Settings.user = new Identity();

                EditorGUILayout.LabelField("Configuration", EditorStyles.whiteLargeLabel);
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                try
                {
                    EditorGUILayout.Space();
                    var r = EditorGUILayout.GetControlRect(false, GUILayout.Width(250), GUILayout.Height(250));
                    GUI.DrawTexture(r, BGSDKLogo, ScaleMode.ScaleToFit, true, 0);
                    EditorGUILayout.Space();
                }
                catch { }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                #region Manual Auth
                EditorGUILayout.LabelField("Client ID", EditorStyles.boldLabel);
                Settings.current.AppId.clientId = EditorGUILayout.TextField(GUIContent.none, Settings.current.AppId.clientId);

                EditorGUILayout.Space();
                showingSecretValue = EditorGUILayout.ToggleLeft("Secret", showingSecretValue, EditorStyles.boldLabel);
                if (showingSecretValue)
                    Settings.current.AppId.clientSecret = EditorGUILayout.TextField(GUIContent.none, Settings.current.AppId.clientSecret);
                else
                    Settings.current.AppId.clientSecret = EditorGUILayout.PasswordField(GUIContent.none, Settings.current.AppId.clientSecret);
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                #endregion

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                if (GUILayout.Button("Dashboard", GUILayout.Height(25)))
                {
                    GUI.FocusControl(null);
                    tab = 1;
                }
                if (GUILayout.Button("Asset Store", GUILayout.Height(25)))
                {
                    GUI.FocusControl(null);
                    Debug.Log("Coming Soon");
                }
                if (GUILayout.Button("Portal", GUILayout.Height(25)))
                {
                    GUI.FocusControl(null);
                    Help.BrowseURL("https://business-staging.arkane.network/applications/" + Settings.current.AppId.applicationId + "/overview");
                }
                if (GUILayout.Button("Marketplace", GUILayout.Height(25)))
                {
                    GUI.FocusControl(null);
                    Debug.Log("Coming Soon");
                }
                if (GUILayout.Button("Learning", GUILayout.Height(25)))
                {
                    GUI.FocusControl(null);
                    Debug.Log("Coming Soon");
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            catch
            { }
            EditorGUILayout.EndVertical();
            PlayerPrefs.Save();
        }

        private void DrawNewsArea()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(position.width - 310), GUILayout.MinHeight(position.height - 45));
            try
            {
                EditorGUILayout.LabelField("News", EditorStyles.whiteLargeLabel);
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                try
                {
                    EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(10));
                    EditorGUILayout.BeginVertical();
                    try
                    {
                        scrollPos_NewsArea = EditorGUILayout.BeginScrollView(scrollPos_NewsArea);
                        for (int i = 0; i < 50; i++)
                        {
                            DrawNewsItem("Example Article 2020-Jan-01"
                                , "This is an example of the first couple of lines being presented by the control. This can be shorter or longer as desired but would be keep to a resonable length and ended with a link or button to the source of the information."
                                , "http://www.google.com");
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    catch { }
                    EditorGUILayout.EndVertical();
                }
                catch { }
                EditorGUILayout.EndHorizontal();
            }
            catch { }
            EditorGUILayout.EndVertical();
        }

        private void DrawNewsItem(string title, string body, string url)
        {
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            try
            {
                EditorGUILayout.BeginHorizontal();
                try
                {
                    if (GUILayout.Button("Read More", EditorStyles.miniButton, GUILayout.Width(65)))
                    {
                        Help.BrowseURL(url);
                    }
                    EditorGUILayout.LabelField("Example Article 2020-Jan-01", EditorStyles.boldLabel);
                }
                catch { }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("This is an example of the first couple of lines being presented by the control. This can be shorter or longer as desired but would be keep to a resonable length and ended with a link or button to the source of the information.", EditorStyles.wordWrappedLabel);
            }
            catch { }
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
