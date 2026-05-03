using System;

namespace BallisticSandbox.Infrastructure.DI.Provider
{
    public interface IInstanceProvider
    {
        object GetInstance(Type contractType, object identifier = null);
        T GetInstance<T>(object identifier = null);
        bool TryGetInstance(Type contractType, object identifier, out object instance);
    }
}