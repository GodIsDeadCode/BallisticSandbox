using System.Reflection;

namespace BallisticSandbox.Infrastructure.DI.Injection
{
    public class InjectableMethod
    {
        public readonly MethodInfo Method;
        public readonly ParameterInfo[] Parameters;
        public readonly object[] Identifiers;

        public InjectableMethod(MethodInfo method, ParameterInfo[] parameters, object[] identifiers)
        {
            Method = method;
            Parameters = parameters;
            Identifiers = identifiers;
        }
    }
}
