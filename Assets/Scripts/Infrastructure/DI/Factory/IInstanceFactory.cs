using BallisticSandbox.Infrastructure.DI.Utility;
using System;
using System.Collections.Generic;

namespace BallisticSandbox.Infrastructure.DI.Factory
{
    public interface IInstanceFactory
    {
        object CreateInstance(Type type, IReadOnlyDictionary<int, TypeValuePair> arguments = null);
        T CreateInstance<T>(IReadOnlyDictionary<int, TypeValuePair> arguments = null);
    }
}