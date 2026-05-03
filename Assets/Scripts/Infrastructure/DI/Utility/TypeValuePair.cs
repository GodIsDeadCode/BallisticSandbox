
namespace BallisticSandbox.Infrastructure.DI.Utility
{
    public readonly struct TypeValuePair
    {
        public readonly System.Type Type;
        public readonly object Value;

        public TypeValuePair(System.Type type, object value)
        {
            Type = type;
            Value = value;
        }
    }
}
