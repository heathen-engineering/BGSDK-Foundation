using HeathenEngineering.BGSDK.Engine;
using UnityEditor;
using UnityEngine;

namespace HeathenEngineering.BGSDK.Editor
{
    [CustomEditor(typeof(Settings))]
    public class BGSDKSettingsEditor : UnityEditor.Editor
    {
        SerializedProperty Authentication;
        SerializedProperty Business;
        SerializedProperty API;

        SerializedProperty UseStaging;
        SerializedProperty AppId;
        SerializedProperty AuthenticationMode;

        private bool domainSettings = false;
        private bool configSettings = false;
        private bool modelSettings = true;

        void OnEnable()
        {
            Authentication = serializedObject.FindProperty("Authentication");
            Business = serializedObject.FindProperty("Business");
            API = serializedObject.FindProperty("API");

            UseStaging = serializedObject.FindProperty("UseStaging");
            //Secret = serializedObject.FindProperty("Secret");
            AppId = serializedObject.FindProperty("AppId");
            AuthenticationMode = serializedObject.FindProperty("AuthenticationMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDomainSettings();
            if (Authenticate())
            {
                DrawConfigSettings();
            }
            else
            {
                //User is not authenticated so help them fix that
            }
            serializedObject.ApplyModifiedProperties();
        }

        private bool Authenticate()
        {
            return true;
        }

        private void DrawDomainSettings()
        {
            domainSettings = EditorGUILayout.Foldout(domainSettings, "Domain Settings");
            if (domainSettings)
            {
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(Authentication, true);
                EditorGUILayout.PropertyField(Business, true);
                EditorGUILayout.PropertyField(API, true);
                EditorGUI.indentLevel = indent;
            }
        }

        private void DrawConfigSettings()
        {
            configSettings = EditorGUILayout.Foldout(configSettings, "Config Settings");
            if (configSettings)
            {
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(AppId, new GUIContent("Application ID", "The identity and client details for the BGSDK App to connect to.") , true);
                //EditorGUILayout.PropertyField(AuthenticationMode, new GUIContent("Authentication Settings", "Details used to adjust the method of authentication with BGSDK."), true);
                //EditorGUILayout.PropertyField(Secret);
                EditorGUILayout.PropertyField(UseStaging);
                EditorGUI.indentLevel = indent;
            }
        }
    }
}
