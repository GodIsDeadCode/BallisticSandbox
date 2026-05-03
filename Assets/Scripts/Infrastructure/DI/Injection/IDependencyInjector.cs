namespace BallisticSandbox.Infrastructure.DI.Injection
{
    public interface IDependencyInjector
    {
        void Inject(object instance, InjectionType injectionType);
    }
}