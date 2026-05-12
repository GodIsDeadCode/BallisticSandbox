using System.Runtime.CompilerServices;
using UnityEngine;

namespace BallisticSandbox.Infrastructure.Services.Logger
{
    public class ConsoleLogger : ILogger
    {
        public void LogError(string message, string tag = null, [CallerMemberName]string member = "", [CallerFilePath] string file = "")
        {
            Debug.LogError(Format(message, tag, member, file));
        }

        public void LogInfo(string message, string tag = null, [CallerMemberName] string member = "", [CallerFilePath] string file = "")
        {
            Debug.Log(Format(message, tag, member, file));
        }

        public void LogWarning(string message, string tag = null, [CallerMemberName] string member = "", [CallerFilePath] string file = "")
        {
            Debug.LogWarning(Format(message, tag, member, file));
        }

        private static string Format(string message, string tag = null, string member = "", string file = "")
        {
            string className = System.IO.Path.GetFileNameWithoutExtension(file);
            string prefix = tag != null ? $"[{tag}]" : $"[{className}.{member}]";
            return $"{prefix} {message}";
        }
    }
}
