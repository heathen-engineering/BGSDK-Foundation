using HeathenEngineering.BGSDK.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HeathenEngineering.BGSDK.Editor
{
    [CustomEditor(typeof(BGSDKSettings))]
    public class BGSDKSettingsEditor : UnityEditor.Editor
    {
        public VisualTreeAsset inspectorMarkup;
        public VisualTreeAsset contractMarkup;
        public VisualTreeAsset tokenMarkup;

        private static bool app = true;
        private static bool api = false;
        private static bool model = false;

        private string path;

        private UnityEditor.UIElements.ToolbarToggle tglApplication;
        private UnityEditor.UIElements.ToolbarToggle tglApi;
        private UnityEditor.UIElements.ToolbarToggle tglModel;
        private VisualElement contracts;
        private VisualElement appPage;
        private VisualElement apiPage;
        private VisualElement modelPage;

        private static List<IEnumerator> cooroutines;

        public override VisualElement CreateInspectorGUI()
        {
            var settings = target as BGSDKSettings;

            path = AssetDatabase.GetAssetPath(settings);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            var root = inspectorMarkup.CloneTree();

            tglApplication = root.Q<UnityEditor.UIElements.ToolbarToggle>("tglApplication");
            tglApi = root.Q<UnityEditor.UIElements.ToolbarToggle>("tglApi");
            tglModel = root.Q<UnityEditor.UIElements.ToolbarToggle>("tglModel");

            var addContract = root.Q<UnityEditor.UIElements.ToolbarButton>("addContract");
            addContract.clicked += () =>
            {
                BGSDK.Engine.Contract nContract = CreateInstance<Engine.Contract>();
                nContract.name = "New Contract";
                nContract.data.name = "New Contract";
                nContract.data.symbol = "NEWCONTRACT";
                nContract.data.secretType = "MATIC";

                nContract.updatedFromServer = false;

                if (settings.contracts == null)
                    settings.contracts = new List<BGSDK.Engine.Contract>();

                nContract.tokens = new List<Token>();
                string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + nContract.name + ".asset");
                AssetDatabase.CreateAsset(nContract, assetPathAndName);
                settings.contracts.Add(nContract);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(BGSDKSettings.current));
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                RebuildContractDisplay();
            };

            var syncData = root.Q<UnityEditor.UIElements.ToolbarButton>("syncButton");
            syncData.clicked += () =>
            {
                BGSDKSettings.current = settings;
                var syncProc = BGSDK.Editor.EditorUtilities.SyncSettings(RebuildContractDisplay);

                StartCoroutine(syncProc);
            };

            contracts = root.Q<VisualElement>("content");
            appPage = root.Q<VisualElement>("appPage");
            apiPage = root.Q<VisualElement>("apiPage");
            modelPage = root.Q<VisualElement>("modelPage");

            RebuildContractDisplay();

            tglApplication.value = app;
            tglApi.value = api;
            tglModel.value = model;

            appPage.style.display = app ? DisplayStyle.Flex : DisplayStyle.None;
            apiPage.style.display = api ? DisplayStyle.Flex : DisplayStyle.None;
            modelPage.style.display = model ? DisplayStyle.Flex : DisplayStyle.None;

            tglApplication.RegisterValueChangedCallback(HandleToggleApplicationChange);
            tglApi.RegisterValueChangedCallback(HandleToggleApiChange);
            tglModel.RegisterValueChangedCallback(HandleToggleModelChange);

            return root;
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
                    {
                        done.Add(e);
                        EditorUtility.SetDirty(target);
                    }
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

        private void RebuildContractDisplay()
        {
            var settings = target as BGSDKSettings;

            contracts.Clear();

            foreach(var contract in settings.contracts)
            {
                try
                {
                    var contractRoot = contractMarkup.CloneTree();
                    contracts.Add(contractRoot);
                    var label = contractRoot.Q<Label>("label");
                    var content = contractRoot.Q<VisualElement>("content");
                    var addToken = contractRoot.Q<UnityEditor.UIElements.ToolbarButton>("addToken");
                    var contractRemove = contractRoot.Q<UnityEditor.UIElements.ToolbarButton>("removeContract");
                    label.text = contract.SystemName;
                    addToken.clicked += () =>
                    {
                        BGSDK.Engine.Token nToken = CreateInstance<Engine.Token>();
                        nToken.name = contract.data.name + ": New Token";
                        nToken.SystemName = "New Token";
                        nToken.UpdatedFromServer = false;
                        nToken.contract = contract;

                        if (contract.tokens == null)
                            contract.tokens = new List<BGSDK.Engine.Token>();

                        contract.tokens.Add(nToken);
                        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + nToken.name + ".asset");
                        AssetDatabase.CreateAsset(nToken, assetPathAndName);
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(BGSDKSettings.current));
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        RebuildContractDisplay();
                        EditorUtility.SetDirty(target);
                    };
                    contractRemove.clicked += () =>
                    {
                        foreach (var token in contract.tokens)
                            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(token));

                        settings.contracts.Remove(contract);
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(contract));

                        RebuildContractDisplay();
                        EditorUtility.SetDirty(target);
                    };
                    

                    foreach (var token in contract.tokens)
                    {
                        var tokenRoot = tokenMarkup.CloneTree();
                        content.Add(tokenRoot);
                        var tokenLabel = tokenRoot.Q<Label>("label");
                        var tokenRemove = tokenRoot.Q<UnityEditor.UIElements.ToolbarButton>("removeToken");
                        tokenRemove.clicked += () =>
                        {
                            contract.tokens.Remove(token);
                            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(token));
                            RebuildContractDisplay();
                            EditorUtility.SetDirty(target);
                        };
                        tokenLabel.text = token.SystemName;
                    }
                }
                catch(Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        private void HandleToggleModelChange(ChangeEvent<bool> evt)
        {
            if (evt.newValue)
            {
                app = false;
                api = false;
                model = true;
                tglApplication.value = false;
                tglApi.value = false;
                appPage.style.display = DisplayStyle.None;
                apiPage.style.display = DisplayStyle.None;
                modelPage.style.display = DisplayStyle.Flex;
            }
        }

        private void HandleToggleApiChange(ChangeEvent<bool> evt)
        {
            if (evt.newValue)
            {
                app = false;
                api = true;
                model = false;
                tglApplication.value = false;
                tglModel.value = false;
                appPage.style.display = DisplayStyle.None;
                apiPage.style.display = DisplayStyle.Flex;
                modelPage.style.display = DisplayStyle.None;
            }
        }

        private void HandleToggleApplicationChange(ChangeEvent<bool> evt)
        {
            if (evt.newValue)
            {
                app = true;
                api = false;
                model = false;
                tglApi.value = false;
                tglModel.value = false;
                appPage.style.display = DisplayStyle.Flex;
                apiPage.style.display = DisplayStyle.None;
                modelPage.style.display = DisplayStyle.None;
            }
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
    }
}
