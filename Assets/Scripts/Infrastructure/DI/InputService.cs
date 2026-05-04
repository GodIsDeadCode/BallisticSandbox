namespace BallisticSandbox
{
    public class InputService : IInputService
    {
        public string s;

        public InputService(string s = null)
        {
            this.s = s;
        }
    }
}
