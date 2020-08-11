using UnityEngine;
using UnityEditor;

public abstract class SingletonDataEditorWindow : DataEditorWindow
{
    public override void Initialize()
    {
        base.Initialize();

        if (data == null)
        {
            foreach (var dataType in DataTypes)
            {
                string[] assets = AssetDatabase.FindAssets("t: " + dataType.ToString());
                if (assets.Length > 0)
                {
                    ScriptableObject data = (ScriptableObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[0]), dataType);
                    if (data != null)
                    {
                        updateData(data);
                    }
                }
            }
        }
    }
}
