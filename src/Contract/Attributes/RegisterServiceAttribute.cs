namespace Contract;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class RegisterServiceAttribute : Attribute
{
    public Type ServiceType { get; }
    public ServiceLifetime Lifetime { get; }

    public RegisterServiceAttribute(Type serviceType, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        ServiceType = serviceType;
        Lifetime = lifetime;
    }
}
