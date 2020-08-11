using UnityEngine;
using UnityEditor;
using System;

public class ConfigDataEditorWindow : SingletonDataEditorWindow
{
    protected override string FileName => "Decrypt/Data/ConfigData";

    protected override Type DataType => typeof(ConfigData);

    protected override string AssetTypeName => "ConfigData";

    protected override string Title => "Config Data Editor";

    ConfigData configData;

    protected override void updateData(ScriptableObject data)
    {
        base.updateData(data);
        configData = data as ConfigData;
    }

    protected override void draw()
    {
        EditorGUILayout.BeginVertical(EditorUtils.MainSettingsStyle);
        {
            EditorGUILayout.BeginVertical(EditorUtils.InternalPaneStyle);
            {
                configData.characterPrefab = EditorGUILayout.ObjectField(
                    "character prefab:",
                    configData.characterPrefab,
                    typeof(GameObject),
                    false) as GameObject;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorUtils.InternalPaneStyle);
            {
                configData.gravity = EditorGUILayout.FloatField("world gravity:", configData.gravity);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }
}