using UnityEngine;
using System.Collections.Generic;

public class Systems
{
    private static Systems instance;
    public static Systems Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Systems();
                if (SystemsProxy.ProxyInstance == null)
                {
                    GameObject proxyObject = new GameObject("__systems_proxy");
                    instance.proxy = proxyObject.AddComponent<SystemsProxy>();
                    Object.DontDestroyOnLoad(proxyObject);
                }
                else
                {
                    instance.proxy = SystemsProxy.ProxyInstance;
                }
            }
            return instance;
        }
    }

    public static void Inject(object target)
    {
        Instance.injector.Inject(target);
    }

    private SystemsProxy proxy;
    private List<IGameSystem> registeredSystems = new List<IGameSystem>();
    private Dictionary<System.Type, IGameSystem> systemsByType = new Dictionary<System.Type, IGameSystem>();
    private IInjector injector;

    [Inject]
    public ISignalBus signalBus { get; set; }

    public static readonly string SignalAppLoad = "Systems.SignalAppLoad";
    public static readonly string SignalMapLoad = "Systems.SignalMapLoad";
    public static readonly string SignalMapUnload = "Systems.SignalMapUnload";

    public T GetSystem<T>()
        where T : class, IGameSystem
    {
        IGameSystem system;
        if (systemsByType.TryGetValue(typeof(T), out system))
        {
            return system as T;
        }
        return null;
    }

    public void SystemsInitialize()
    {
        InjectionContext context = new InjectionContext();

        var config = Resources.Load<ConfigData>("Data/ConfigData");

        context.Bind<ConfigData>().ToSingleton(config);

        List<IGameSystem> systems = new List<IGameSystem>()
        {
            new ParticipantSystem(),
            new SpawnSystem(),
        };

        registeredSystems.AddRange(systems);

        registeredSystems.ForEach((system) =>
        {
            systemsByType.Add(system.GetType(), system);
            context.Bind(system.GetType()).ToSingleton(system);
        });

        injector = new Injector(context);
        context.Bind<IInjector>().ToSingleton(injector);
        context.Bind<ISignalBus>().ToSingleton<SignalBus>();

        injector.Inject(this);

        registeredSystems.ForEach((system) =>
        {
            injector.Inject(system);
            (system as GameSystem).PostInjection();
        });

        foreach (IGameSystem system in registeredSystems)
        {
            if (system is IAppSystem appSystem)
            {
                appSystem.OnAppLoad();
            }
        }
    }

    public void OnUpdate()
    {
        foreach (IGameSystem system in registeredSystems)
        {
            if (system is IUpdateSystem updateSystem)
            {
                updateSystem.OnUpdate();
            }
        }
    }

    public void OnMapLoad()
    {
        foreach (IGameSystem system in registeredSystems)
        {
            if (system is IMapSystem mapSystem)
            {
                mapSystem.OnMapLoad();
            }
        }
    }

    public void OnMapUnload()
    {
        foreach (IGameSystem system in registeredSystems)
        {
            if (system is IMapSystem mapSystem)
            {
                mapSystem.OnMapUnload();
            }
        }
    }
}


// Base system interface
public interface IGameSystem : System.IDisposable
{
}

// Systems that respond to application level events.
public interface IAppSystem : IGameSystem
{
    void OnAppLoad();
}

// Systems that respond to game events like Update.
public interface IUpdateSystem : IGameSystem
{
    void OnUpdate();
}

// Systems that respond to new maps loading/unloading.
public interface IMapSystem : IGameSystem
{
    void OnMapLoad();
    void OnMapUnload();
}


public abstract class GameSystem : IGameSystem
{
    [Inject]
    public ConfigData config { get; set; }

    [Inject]
    public ISignalBus signalBus { get; set; }

    public GameSystem() { }

    public void PostInjection()
    {
        Initialize();
    }

    protected virtual void Initialize() { }
    public virtual void Dispose() { }

    protected TSystem GetSystem<TSystem>()
        where TSystem : class, IGameSystem
    {
        return Systems.Instance.GetSystem<TSystem>();
    }
}
