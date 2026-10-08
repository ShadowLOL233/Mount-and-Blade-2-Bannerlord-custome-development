using System;
using System.IO;
using System.Reflection;

namespace UTMPatch.Util
{
    internal static class UTMPatchLog
    {
        private static readonly object _lock = new object();
        private static string _path;
        private static bool _ready;

        private static void Ensure()
        {
            if (_ready) return;
            lock (_lock)
            {
                if (_ready) return;
                try
                {
                    var dllDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    var modRoot = Directory.GetParent(dllDir)?.Parent?.FullName
                                  ?? Directory.GetParent(dllDir)?.FullName
                                  ?? dllDir;
                    var logsDir = Path.Combine(modRoot, "Logs");
                    Directory.CreateDirectory(logsDir);
                    _path = Path.Combine(logsDir, "log.txt");
                    File.AppendAllText(_path, "\n==== UTMPatch log started " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ====\n");
                }
                catch { _path = null; }
                _ready = true;
            }
        }

        public static void Info(string msg) => Write("INFO", msg);
        public static void Warn(string msg) => Write("WARN", msg);
        public static void Error(string msg) => Write("ERROR", msg);

        public static void Exception(string ctx, Exception ex)
            => Write("ERROR", ctx + " · " + ex.GetType().Name + " · " + ex.Message + "\n" + ex.StackTrace);

        private static void Write(string lvl, string msg)
        {
            Ensure();
            if (_path == null) return;
            try
            {
                lock (_lock)
                {
                    File.AppendAllText(_path,
                        DateTime.Now.ToString("HH:mm:ss.fff") + " [" + lvl + "] " + msg + "\n");
                }
            }
            catch { }
        }
    }
}
