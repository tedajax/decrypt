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
