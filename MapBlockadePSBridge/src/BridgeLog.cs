using System;
using System.IO;
using TaleWorlds.Library;

namespace MapBlockadePSBridge
{
    internal static class BridgeLog
    {
        private static readonly object _lock = new object();
        private static string _path;

        internal static void WriteFile(string msg)
        {
            try
            {
                EnsurePath();
                lock (_lock)
                {
                    File.AppendAllText(_path,
                        DateTime.Now.ToString("HH:mm:ss.fff") + " " + msg + Environment.NewLine);
                }
            }
            catch
            {
                // never let logging kill the mod
            }
        }

        internal static void Both(string msg)
        {
            WriteFile(msg);
            try
            {
                InformationManager.DisplayMessage(new InformationMessage("[PSBridge] " + msg));
            }
            catch
            {
                // DisplayMessage may not be safe outside campaign; ignore
            }
        }

        private static void EnsurePath()
        {
            if (_path != null) return;
            var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var dir = Path.Combine(docs, "Mount and Blade II Bannerlord", "Configs", "ModLogs");
            Directory.CreateDirectory(dir);
            _path = Path.Combine(dir, "PSBridge_" + DateTime.Today.ToString("yyyyMMdd") + ".log");
        }
    }
}
