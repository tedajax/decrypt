using System.Collections.Generic;

public class InjectionBinding
{
    protected System.Type boundType;
    protected object boundObject;

    public object BoundObject => boundObject;

    public void ToSingleton(object obj)
    {
        this.boundObject = obj;
    }
}

public class InjectionBinding<TBind> : InjectionBinding
{
    public InjectionBinding()
    {
        this.boundType = typeof(TBind);
    }

    public void ToSingleton<TObject>()
        where TObject : class, TBind, new()
    {
        this.boundObject = new TObject();
    }

    public void ToSingleton<TSingleton>(TSingleton obj)
        where TSingleton : class
    {
        this.boundObject = obj;
    }
}

public class InjectionContext
{
    private Dictionary<System.Type, InjectionBinding> singletonBindings = new Dictionary<System.Type, InjectionBinding>();

    public InjectionBinding<T> Bind<T>()
    {
        var result = new InjectionBinding<T>();
        singletonBindings.Add(typeof(T), result);
        return result;
    }

    public InjectionBinding Bind(System.Type bindingType)
    {
        var result = new InjectionBinding();
        singletonBindings.Add(bindingType, result);
        return result;
    }

    public object Get(System.Type type)
    {
        InjectionBinding binding;
        if (singletonBindings.TryGetValue(type, out binding))
        {
            return binding.BoundObject;
        }
        UnityEngine.Debug.LogError("No binding found for " + type.ToString());
        return null;
    }
}