using System;
using System.Collections.Generic;

public interface ISignalBus
{
    T GetSignal<T>(string signalName) where T : BaseSignal, new();
}

public class SignalBus : ISignalBus
{
    private Dictionary<int, BaseSignal> signalsByNameHash = new Dictionary<int, BaseSignal>();

    public T GetSignal<T>(string signalName)
        where T : BaseSignal, new()
    {
        int hash = signalName.GetHashCode();

        BaseSignal result;
        if (signalsByNameHash.TryGetValue(hash, out result))
        {
            return result as T;
        }
        result = new T();
        signalsByNameHash[hash] = result;
        return result as T;
    }
}