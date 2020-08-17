using UnityEngine;

public class SystemsProxy : MonoBehaviour
{
    private static SystemsProxy instance;
    public static SystemsProxy ProxyInstance { get { return instance; } }

    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);

        Systems.Instance.SystemsInitialize();
    }

    void Start()
    {
        // For now assume we're in a real map... but we'll figure that part out later
        Systems.Instance.OnMapLoad();
    }

    void Update()
    {
        Systems.Instance.OnUpdate();
    }
}