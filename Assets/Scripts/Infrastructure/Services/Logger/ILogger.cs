using System.Runtime.CompilerServices;

namespace BallisticSandbox.Infrastructure.Services.Logger
{
    public interface ILogger
    {
        void LogInfo(string message, string tag = null, [CallerMemberName] string member = "", [CallerFilePath] string file = "");
        void LogWarning(string message, string tag = null, [CallerMemberName] string member = "", [CallerFilePath] string file = "");
        void LogError(string message, string tag = null, [CallerMemberName] string member = "", [CallerFilePath] string file = "");
    }
}
