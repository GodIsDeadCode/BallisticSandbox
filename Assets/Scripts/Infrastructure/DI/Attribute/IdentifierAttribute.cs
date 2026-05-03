
namespace BallisticSandbox.Infrastructure.DI.Attribute
{
    [System.AttributeUsage(System.AttributeTargets.Parameter | System.AttributeTargets.Field | System.AttributeTargets.Property, AllowMultiple = false)]
    public class IdentifierAttribute : System.Attribute
    {
        public readonly object Identifier;

        public IdentifierAttribute(object identifier)
        {
            Identifier = identifier;
        }
    }
}
