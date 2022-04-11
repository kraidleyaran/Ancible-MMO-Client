#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server
{
    [CustomEditor(typeof(ServerExportSettings))]
    public class ServerExportSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("TraitFolderPath"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TraitSaveDataPath"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("MapFolderPath"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("MapSaveDataPath"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("MapSpawnSaveDataPath"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterClassFolderPath"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterClassSaveDataPath"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("AbilityFolderPath"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AbilitySaveDataPath"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("ObjectTemplateFolderPath"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ObjectTemplateSavePath"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("ItemFolderPath"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ItemSavePath"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("LootTableFolderPath"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LootTableSavePath"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("WorldBonusFolderPath"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("WorldBonusSavePath"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("TalentFolderPath"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TalentSavePath"));

            if (GUILayout.Button("Export Data"))
            {
                var settings = serializedObject.targetObject as ServerExportSettings;
                if (settings)
                {
                    settings.ExportData();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif