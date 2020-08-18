using UnityEngine;

public abstract class DecryptComponent : MonoBehaviour, System.IDisposable
{
    [Inject]
    public ISignalBus signalBus { get; set; }

    protected virtual void Awake()
    {
        Systems.Inject(this);
    }

    public virtual void Dispose() { }
}