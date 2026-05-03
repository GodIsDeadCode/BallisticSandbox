using System;

namespace BallisticSandbox.Infrastructure.DI.Exceptions
{
    public class BindingRegistrationException : Exception
    {
        public readonly Bind.BindData FailedBindData;

        public BindingRegistrationException(string message, Bind.BindData failedBindData, Exception innerException = null) : base(message, innerException)
        {
            FailedBindData = failedBindData;
        }

        public override string Message
        {
            get
            {
                return $"{base.Message}\nFailed to register binding for contract: {FailedBindData.ContractType.Name}, " +
                       $"implementation type: {FailedBindData.ImplementationType.Name}, identifier: {FailedBindData.Identifier}";
            }
        }
    }
}
