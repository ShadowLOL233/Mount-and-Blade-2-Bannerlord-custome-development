using System;
using System.IO;
using System.Text;

namespace CheatsGuard
{
    // Rotating per-launch log at <ModRoot>/Logs/cheats_guard.log
    // Overwrites on each SubModule load so you always see the most recent run.
    // Also mirrors critical lines to in-game message bar so user sees status.
    internal static class LogFile
    {
        private static string _path;
        private static readonly object _lock = new object();

        public static void Init()
        {
            var root = Config.ModRoot;
            if (root == null) return;
            var logDir = Path.Combine(root, "Logs");
            try
            {
                Directory.CreateDirectory(logDir);
                _path = Path.Combine(logDir, "cheats_guard.log");
                File.WriteAllText(_path,
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] CheatsGuard log opened (VerboseLog={Config.VerboseLog} · SkipCount={Config.SkipSet.Count})\r\n",
                    Encoding.UTF8);
            }
            catch { _path = null; }
        }

        public static void Write(string line)
        {
            if (_path == null) return;
            lock (_lock)
            {
                try
                {
                    File.AppendAllText(_path,
                        $"[{DateTime.Now:HH:mm:ss.fff}] {line}\r\n",
                        Encoding.UTF8);
                }
                catch { }
            }
        }
    }
}
