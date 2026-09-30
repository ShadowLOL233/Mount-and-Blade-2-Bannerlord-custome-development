using System;
using System.IO;
using System.Reflection;
using TaleWorlds.Library;

namespace UnifiedTroopManager.Util
{
    // Dedicated log file per DESIGN §7.1 (D7).
    // Location: <ModRoot>/Logs/log.txt — the same pattern CheatsGuard uses so users always
    // find the file in a predictable place next to the mod install.
    public static class UTMLog
    {
        public enum Level { Info, Warn, Error, Debug }

        private const string LogFileName = "log.txt";

        private static readonly object _lock = new object();
        private static string _path;
        private static bool _debug;
        private static bool _initialized;

        public static void Init(bool debug = false)
        {
            lock (_lock)
            {
                _debug = debug;
                try
                {
                    string asmPath = Assembly.GetExecutingAssembly().Location;
                    string binDir = Path.GetDirectoryName(asmPath);
                    string modRoot = Directory.GetParent(binDir)?.Parent?.FullName;
                    if (modRoot == null) return;

                    string logDir = Path.Combine(modRoot, "Logs");
                    Directory.CreateDirectory(logDir);
                    _path = Path.Combine(logDir, LogFileName);
                    File.WriteAllText(_path,
                        "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] UnifiedTroopManager log started" + Environment.NewLine);
                    _initialized = true;
                }
                catch (Exception ex)
                {
                    InformationManager.DisplayMessage(new InformationMessage(
                        "[UTM] log init failed: " + ex.Message, Colors.Red));
                }
            }
        }

        public static void SetDebug(bool debug) { _debug = debug; }

        public static void Info(string msg)  { Write(Level.Info,  msg); }
        public static void Warn(string msg)  { Write(Level.Warn,  msg); }
        public static void Error(string msg) { Write(Level.Error, msg); }
        public static void Debug(string msg) { if (_debug) Write(Level.Debug, msg); }

        public static void Exception(string context, Exception ex)
        {
            Write(Level.Error, context + " · " + ex.GetType().Name + ": " + ex.Message);
        }

        private static void Write(Level lvl, string msg)
        {
            if (!_initialized || _path == null) return;
            try
            {
                string line = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] "
                    + lvl.ToString().ToUpperInvariant() + " · " + msg + Environment.NewLine;
                lock (_lock)
                {
                    File.AppendAllText(_path, line);
                }
            }
            catch { /* logging must never crash the game */ }
        }
    }
}
