using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ConfigData))]
public class ConfigDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Config Data Editor"))
        {
            Open();
        }
    }

    [MenuItem("Decrypt/Config/Config")]
    [CanEditMultipleObjects]
    public static void Open()
    {
        DataEditorWindow.Open<ConfigDataEditorWindow>("Config Data");
    }

    [MenuItem("Decrypt/Create/Config")]
    public static void CreateAsset()
    {
        DataEditorWindow.CreateAssetType(typeof(ConfigData), "New Config Data", typeof(ConfigDataEditorWindow), "Config Data", "Assets/Decrypt/Data/");
    }
}
