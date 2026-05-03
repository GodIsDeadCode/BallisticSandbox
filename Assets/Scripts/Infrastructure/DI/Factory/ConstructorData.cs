using System.Reflection;

namespace BallisticSandbox.Infrastructure.DI.Factory
{
    public class ConstructorData
    {
        public readonly ConstructorInfo Constructor;
        public readonly ParameterInfo[] Parameters;
        public readonly object[] Identifiers;

        public ConstructorData(ConstructorInfo constructor, ParameterInfo[] parameters, object[] identifiers)
        {
            Constructor = constructor;
            Parameters = parameters;
            Identifiers = identifiers;
        }
    }
}
