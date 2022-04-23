#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    [CustomEditor(typeof(BuildSettings))]
    public class BuildSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("DevWindowPath"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DevLinuxPath"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DevMacOSPath"));

            if (GUILayout.Button("Build Dev"))
            {
                var settings = serializedObject.targetObject as BuildSettings;
                if (settings)
                {
                    settings.BuildDev();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif