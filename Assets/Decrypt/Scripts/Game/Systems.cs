using UnityEngine;
using System.Collections.Generic;

public class Systems : MonoBehaviour
{
    private static Systems instance;
    public static Systems Instance
    {
        get { return instance; }
    }

    [SerializeField]
    private ConfigData config;

    public ConfigData Config { get { return config; } }



    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }
}