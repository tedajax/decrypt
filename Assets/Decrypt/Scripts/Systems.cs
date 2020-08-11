using UnityEngine;

public class Systems : MonoBehaviour
{
    public static Systems Instance { get; private set; }

    public ConfigData Config { get; private set; }

    public bool IsLoadingComplete { get; private set; } = false;
    public bool IsSceneSetup { get; private set; } = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(this);
            return;
        }

        LoadResources();
    }

    private void LoadResources()
    {
        Config = Resources.Load<ConfigData>("Data/ConfigData");
    }
}