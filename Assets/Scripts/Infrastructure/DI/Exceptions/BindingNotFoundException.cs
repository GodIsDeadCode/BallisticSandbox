using System;

namespace BallisticSandbox.Infrastructure.DI.Exceptions
{
    public class BindingNotFoundException : Exception
    {
        public readonly Type FailedContractType;
        public readonly Type FailedImplementationType;
        public readonly object FailedIdentifier;

        public BindingNotFoundException(string message, Type failedContractType, Type failedImplementationType = null, object failedIdentifier = null, Exception innerException = null)
            : base(message, innerException)
        {
            FailedContractType = failedContractType;
            FailedImplementationType = failedImplementationType;
            FailedIdentifier = failedIdentifier;
        }

        public override string Message
        {
            get
            {
                return $"{base.Message}\nFailed to resolve contract: {FailedContractType.Name}" +
                       (FailedImplementationType != null ? $", implementation: {FailedImplementationType.Name}" : "") +
                       (FailedIdentifier != null ? $", identifier: {FailedIdentifier}" : "");
            }
        }
    }
}
