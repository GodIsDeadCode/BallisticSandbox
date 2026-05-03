using System;

namespace BallisticSandbox.Infrastructure.DI.Bind
{
    public interface IBindingConfigurationState
    {
        IBindingConfigurationState WithIdentifier(object identifier);
        IBindingConfigurationState WithArgument(int position, Type type, object value);
        IBindingConfigurationState WithArguments(params Utility.TypeValuePair[] arguments);
        void CommitBinding();
    }
}
