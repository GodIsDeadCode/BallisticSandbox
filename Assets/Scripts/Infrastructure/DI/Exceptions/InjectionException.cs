using System;

namespace BallisticSandbox.Infrastructure.DI.Exceptions
{
    public class InjectionException : Exception
    {
        public readonly Type FailedType;

        public InjectionException(string message, Type failedType, Exception innerException) : base(message, innerException)
        {
            FailedType = failedType;
        }

        public override string Message
        {
            get
            {
                return $"{base.Message}\nFailed to inject into {FailedType.FullName}.";
            }
        }
    }
}
