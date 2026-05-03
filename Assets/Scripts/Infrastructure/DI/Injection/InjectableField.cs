using System.Reflection;

namespace BallisticSandbox.Infrastructure.DI.Injection
{
    public class InjectableField
    {
        public readonly FieldInfo Field;
        public readonly object Identifier;

        public InjectableField(FieldInfo field, object identifier)
        {
            Field = field;
            Identifier = identifier;
        }
    }
}
