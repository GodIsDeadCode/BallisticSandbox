namespace BallisticSandbox.Infrastructure.DI.Bind
{
    public interface IBindingLifetimeState
    {
        IBindingConfigurationState AsSingleton();
        IBindingConfigurationState AsTransient();
        IBindingConfigurationState AsScoped();
    }
}
