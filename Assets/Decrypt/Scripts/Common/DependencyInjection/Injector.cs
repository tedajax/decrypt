using System;
using System.Reflection;
using System.Collections.Generic;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public class InjectAttribute : Attribute
{
    public InjectAttribute() { }
}

public interface IInjector
{
    void Inject(object target);
}

public class Injector : IInjector
{
    private InjectionContext context;

    public Injector(InjectionContext context)
    {
        this.context = context;
    }

    public void Inject(object target)
    {
        PropertyInfo[] properties = target.GetType().GetProperties();
        foreach (var property in properties)
        {
            Attribute injectAttribute = property.GetCustomAttribute(typeof(InjectAttribute));
            if (injectAttribute != null)
            {
                property.SetValue(target, context.Get(property.PropertyType));
            }
        }
    }
}