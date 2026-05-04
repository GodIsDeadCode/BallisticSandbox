using UnityEngine;
using BallisticSandbox.Infrastructure.DI.Container;
using BallisticSandbox.Infrastructure.DI.Context;
using BallisticSandbox.Infrastructure.DI.Attribute;

namespace BallisticSandbox
{
    public class Test : MonoBehaviour
    {
        [Inject]
        private IInputService input;

        [Inject]
        private void Constructor(IInputService input)
        {
            this.input = input;
        }

        private void Start()
        {
            ProjectContext.Instance.DependencyContainer.Inject(this, Infrastructure.DI.Injection.InjectionType.Fields);
            Debug.Log(input);
        }
    }
}
