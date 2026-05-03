using System;
using System.Collections.Generic;
using System.Linq;

namespace BallisticSandbox.Infrastructure.DI.Exceptions
{
    public class DependencyResolutionException : Exception
    {
        public readonly IReadOnlyList<Type> ResolutionStack;
        public readonly Type FailedType;

        public DependencyResolutionException(string message, Type failedType, IEnumerable<Type> resolutionStack, Exception innerException = null) : base(message, innerException)
        {
            FailedType = failedType;
            ResolutionStack = resolutionStack.Reverse().ToList();
        }

        public override string Message
        {
            get
            {
                string resolutionStack = string.Join(" --> ", ResolutionStack?.Select(t => t.Name));
                return $"{base.Message}\nResolution stack: {resolutionStack} --> {FailedType?.Name}";
            }
        }
    }
}
