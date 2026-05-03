using System.Reflection;

namespace BallisticSandbox.Infrastructure.DI.Injection
{
    public class InjectableProperty
    {
        public readonly PropertyInfo Property;
        public readonly object Identifier;

        public InjectableProperty(PropertyInfo property, object identifier)
        {
            Property = property;
            Identifier = identifier;
        }
    }
}
