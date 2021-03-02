using HeathenEngineering.BGSDK.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HeathenEngineering.BGSDK.Editor
{
    [CustomEditor(typeof(Settings))]
    public class BGSDKSettingsEditor : UnityEditor.Editor
    {
        public VisualTreeAsset inspectorMarkup;
        public VisualTreeAsset contractMarkup;
        public VisualTreeAsset tokenMarkup;

        private static bool app = true;
        private static bool api = false;
        private static bool model = false;

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
            var settings = target as Settings;
            var root = inspectorMarkup.CloneTree();

            tglApplication = root.Q<UnityEditor.UIElements.ToolbarToggle>("tglApplication");
            tglApi = root.Q<UnityEditor.UIElements.ToolbarToggle>("tglApi");
            tglModel = root.Q<UnityEditor.UIElements.ToolbarToggle>("tglModel");

            var addContract = root.Q<UnityEditor.UIElements.ToolbarButton>("addContract");
            addContract.clicked += () =>
            {
                BGSDK.Engine.Contract nContract = new BGSDK.Engine.Contract();
                nContract.name = "New Contract";
                nContract.UpdatedFromServer = false;

                if (settings.Contracts == null)
                    settings.Contracts = new List<BGSDK.Engine.Contract>();

                nContract.Tokens = new List<Token>();
                AssetDatabase.AddObjectToAsset(nContract, settings);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Settings.current));

                RebuildContractDisplay();
            };

            var syncData = root.Q<UnityEditor.UIElements.ToolbarButton>("syncButton");
            syncData.clicked += () =>
            {
                Settings.current = settings;
                var syncProc = BGSDK.Editor.EditorUtilities.SyncSettings();

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

        private void RebuildContractDisplay()
        {
            var settings = target as Settings;

            contracts.Clear();

            foreach(var contract in settings.Contracts)
            {
                try
                {
                    var contractRoot = contractMarkup.CloneTree();
                    contracts.Add(contractRoot);
                    var label = contractRoot.Q<Label>("label");
                    var content = contractRoot.Q<VisualElement>("content");
                    var addToken = contractRoot.Q<UnityEditor.UIElements.ToolbarButton>("addToken");
                    var contractRemove = contractRoot.Q<UnityEditor.UIElements.ToolbarButton>("removeContract");
                    label.text = contract.systemName;
                    addToken.clicked += () =>
                    {
                        BGSDK.Engine.Token nToken = new BGSDK.Engine.Token();
                        nToken.name = "New Token";
                        nToken.UpdatedFromServer = false;
                        nToken.contract = contract;

                        if (contract.Tokens == null)
                            contract.Tokens = new List<BGSDK.Engine.Token>();

                        contract.Tokens.Add(nToken);
                        AssetDatabase.AddObjectToAsset(nToken, settings);
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Settings.current));

                        RebuildContractDisplay();
                    };
                    contractRemove.clicked += () =>
                    {
                        foreach (var token in contract.Tokens)
                            DestroyImmediate(token, true);

                        settings.Contracts.Remove(contract);
                        DestroyImmediate(contract, true);

                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(settings));
                        RebuildContractDisplay();
                    };
                    

                    foreach (var token in contract.Tokens)
                    {
                        var tokenRoot = tokenMarkup.CloneTree();
                        content.Add(tokenRoot);
                        var tokenLabel = tokenRoot.Q<Label>("label");
                        var tokenRemove = tokenRoot.Q<UnityEditor.UIElements.ToolbarButton>("removeToken");
                        tokenRemove.clicked += () =>
                        {
                            Debug.Log("Remove Token");
                            contract.Tokens.Remove(token);
                            DestroyImmediate(token, true);

                            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(settings));
                            RebuildContractDisplay();
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
    }
}
