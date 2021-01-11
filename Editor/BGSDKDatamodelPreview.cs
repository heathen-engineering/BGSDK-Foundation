using HeathenEngineering.BGSDK.Engine;
using UnityEditor;
using UnityEngine;

namespace HeathenEngineering.BGSDK.Editor
{
    [CustomPreview(typeof(Settings))]
    class BGSDKDatamodelPreview : ObjectPreview
    {
        class BGSDKSettingsInfo
        {
            public GUIContent value;
        }

        class Styles
        {
            public GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            public GUIStyle componentName = new GUIStyle(EditorStyles.boldLabel);
            public GUIStyle disabledName = new GUIStyle(EditorStyles.miniLabel);

            public Styles()
            {
                Color fontColor = new Color(0.7f, 0.7f, 0.7f);
                labelStyle.padding.right += 20;
                labelStyle.normal.textColor = fontColor;
                labelStyle.active.textColor = fontColor;
                labelStyle.focused.textColor = fontColor;
                labelStyle.hover.textColor = fontColor;
                labelStyle.onNormal.textColor = fontColor;
                labelStyle.onActive.textColor = fontColor;
                labelStyle.onFocused.textColor = fontColor;
                labelStyle.onHover.textColor = fontColor;

                componentName.normal.textColor = fontColor;
                componentName.active.textColor = fontColor;
                componentName.focused.textColor = fontColor;
                componentName.hover.textColor = fontColor;
                componentName.onNormal.textColor = fontColor;
                componentName.onActive.textColor = fontColor;
                componentName.onFocused.textColor = fontColor;
                componentName.onHover.textColor = fontColor;

                disabledName.normal.textColor = fontColor;
                disabledName.active.textColor = fontColor;
                disabledName.focused.textColor = fontColor;
                disabledName.hover.textColor = fontColor;
                disabledName.onNormal.textColor = fontColor;
                disabledName.onActive.textColor = fontColor;
                disabledName.onFocused.textColor = fontColor;
                disabledName.onHover.textColor = fontColor;
            }
        }

        BGSDKSettingsInfo info;
        GUIContent title;
        Styles styles = new Styles();

        public override void Initialize(UnityEngine.Object[] targets)
        {
            base.Initialize(targets);
            GetSettingsInformation(target as Settings);
        }

        public override GUIContent GetPreviewTitle()
        {
            if (title == null)
            {
                title = new GUIContent("Status Information");
            }
            return title;
        }

        public override bool HasPreviewGUI()
        {
            return info != null;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            // refresh the data
            GetSettingsInformation(target as Settings);

            if (info == null)
                return;

            if (styles == null)
                styles = new Styles();

            //Apply padding
            RectOffset previewPadding = new RectOffset(-5, -5, -5, -5);
            Rect paddedr = previewPadding.Add(r);

            GUI.Label(paddedr, info.value, styles.componentName);
        }

        void GetSettingsInformation(Settings settings)
        {
            if (settings != null)
            {
                string message;
                EditorUtilities.ValidateSettingsModel(settings, out message);

                info = new BGSDKSettingsInfo() { value = new GUIContent(message) };
            }
        }
    }
}
