using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Data
{
    // File-backed preset storage · one XML file per preset under
    // <ModRoot>/Presets/<slug>.xml.
    //
    // XML is used over JSON because Bannerlord's base libraries already ship
    // XmlDocument / XmlReader — adding a JSON dependency (Newtonsoft) would need
    // an extra assembly reference and version-matching with TaleWorlds' bundled
    // copy. XML is also what Bannerlord itself uses for ModuleData, so the files
    // are grep-friendly for a modder.
    //
    // Preset XML schema:
    //   <UnifiedTroopManagerPreset name="..." created="ISO-8601">
    //     <Roster>
    //       <Troop id="imperial_recruit" count="30" />
    //       ...
    //     </Roster>
    //     <Formations>
    //       <Troop id="imperial_recruit" slots="0,1" />
    //       ...
    //     </Formations>
    //   </UnifiedTroopManagerPreset>
    public static class UTMPresetStore
    {
        private static string _presetDirCached;

        public static string PresetDir
        {
            get
            {
                if (_presetDirCached != null) return _presetDirCached;
                try
                {
                    // Resolve relative to this assembly:
                    //   <ModRoot>/bin/Win64_Shipping_Client/UnifiedTroopManager.dll
                    //     → <ModRoot>/Presets/
                    var dllPath = typeof(UTMPresetStore).Assembly.Location;
                    var binDir = Path.GetDirectoryName(dllPath);
                    var win64Parent = Path.GetDirectoryName(binDir);
                    var modRoot = Path.GetDirectoryName(win64Parent);
                    _presetDirCached = Path.Combine(modRoot, "Presets");
                    if (!Directory.Exists(_presetDirCached))
                        Directory.CreateDirectory(_presetDirCached);
                }
                catch (Exception ex)
                {
                    UTMLog.Exception("UTMPresetStore.PresetDir", ex);
                    _presetDirCached = null;
                }
                return _presetDirCached;
            }
        }

        public static List<UTMPreset> LoadAll()
        {
            var result = new List<UTMPreset>();
            try
            {
                var dir = PresetDir;
                if (dir == null) return result;
                var files = Directory.GetFiles(dir, "*.xml");
                foreach (var f in files)
                {
                    var p = LoadFromFile(f);
                    if (p != null) result.Add(p);
                }
                // Newest first — matches "last used probably wanted next" expectation.
                result.Sort((a, b) => b.Created.CompareTo(a.Created));
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMPresetStore.LoadAll", ex);
            }
            return result;
        }

        public static UTMPreset Load(string name)
        {
            try
            {
                var dir = PresetDir;
                if (dir == null) return null;
                var path = Path.Combine(dir, Slugify(name) + ".xml");
                if (!File.Exists(path)) return null;
                return LoadFromFile(path);
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMPresetStore.Load", ex);
                return null;
            }
        }

        public static bool Save(UTMPreset preset)
        {
            if (preset == null || string.IsNullOrWhiteSpace(preset.Name)) return false;
            try
            {
                var dir = PresetDir;
                if (dir == null) return false;
                var path = Path.Combine(dir, Slugify(preset.Name) + ".xml");

                var doc = new XmlDocument();
                var root = doc.CreateElement("UnifiedTroopManagerPreset");
                root.SetAttribute("name", preset.Name);
                root.SetAttribute("created", preset.Created.ToString("o", CultureInfo.InvariantCulture));
                doc.AppendChild(root);

                var rosterEl = doc.CreateElement("Roster");
                foreach (var kv in preset.Roster)
                {
                    var t = doc.CreateElement("Troop");
                    t.SetAttribute("id", kv.Key);
                    t.SetAttribute("count", kv.Value.ToString(CultureInfo.InvariantCulture));
                    rosterEl.AppendChild(t);
                }
                root.AppendChild(rosterEl);

                var formEl = doc.CreateElement("Formations");
                foreach (var kv in preset.Formations)
                {
                    if (kv.Value == null || kv.Value.Count == 0) continue;
                    var t = doc.CreateElement("Troop");
                    t.SetAttribute("id", kv.Key);
                    t.SetAttribute("slots", string.Join(",", kv.Value.Select(i => i.ToString(CultureInfo.InvariantCulture))));
                    formEl.AppendChild(t);
                }
                root.AppendChild(formEl);

                doc.Save(path);
                UTMLog.Info("Preset saved · " + preset.Name + " · " + preset.Roster.Count + " troops · " + preset.Formations.Count + " plans → " + path);
                return true;
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMPresetStore.Save", ex);
                return false;
            }
        }

        public static bool Delete(string name)
        {
            try
            {
                var dir = PresetDir;
                if (dir == null) return false;
                var path = Path.Combine(dir, Slugify(name) + ".xml");
                if (!File.Exists(path)) return false;
                File.Delete(path);
                UTMLog.Info("Preset deleted · " + name);
                return true;
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMPresetStore.Delete", ex);
                return false;
            }
        }

        private static UTMPreset LoadFromFile(string path)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(path);
                var root = doc.DocumentElement;
                if (root == null || root.Name != "UnifiedTroopManagerPreset") return null;

                var preset = new UTMPreset
                {
                    Name = root.GetAttribute("name"),
                    Created = DateTime.TryParse(root.GetAttribute("created"), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt)
                        ? dt
                        : File.GetCreationTime(path)
                };

                var rosterEl = root["Roster"];
                if (rosterEl != null)
                {
                    foreach (XmlNode n in rosterEl.ChildNodes)
                    {
                        if (!(n is XmlElement e) || e.Name != "Troop") continue;
                        var id = e.GetAttribute("id");
                        if (string.IsNullOrEmpty(id)) continue;
                        if (int.TryParse(e.GetAttribute("count"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
                            preset.Roster[id] = count;
                    }
                }

                var formEl = root["Formations"];
                if (formEl != null)
                {
                    foreach (XmlNode n in formEl.ChildNodes)
                    {
                        if (!(n is XmlElement e) || e.Name != "Troop") continue;
                        var id = e.GetAttribute("id");
                        if (string.IsNullOrEmpty(id)) continue;
                        var slotsStr = e.GetAttribute("slots");
                        if (string.IsNullOrEmpty(slotsStr)) continue;
                        var slots = new List<int>();
                        foreach (var tok in slotsStr.Split(','))
                        {
                            if (int.TryParse(tok.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var idx))
                                slots.Add(idx);
                        }
                        if (slots.Count > 0) preset.Formations[id] = slots;
                    }
                }

                return preset;
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMPresetStore.LoadFromFile(" + path + ")", ex);
                return null;
            }
        }

        // Slugify preset name into a filesystem-safe slug. Keeps ASCII letters/digits,
        // replaces runs of other characters with "_". Non-ASCII (e.g. CJK) collapses
        // to "_" too; the preset's display Name is preserved in the XML's name="…"
        // attribute, so Chinese names still render in the UI correctly even if the
        // on-disk filename is anglicized. If the slug ends up empty, fall back to
        // "preset_<hash>".
        public static string Slugify(string name)
        {
            if (string.IsNullOrEmpty(name)) return "preset_empty";
            var sb = new StringBuilder();
            bool lastWasSep = false;
            foreach (var ch in name)
            {
                if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9') || ch == '-')
                {
                    sb.Append(ch);
                    lastWasSep = false;
                }
                else
                {
                    if (!lastWasSep) { sb.Append('_'); lastWasSep = true; }
                }
            }
            var slug = sb.ToString().Trim('_');
            if (string.IsNullOrEmpty(slug))
                slug = "preset_" + (name.GetHashCode() & 0x7FFFFFFF).ToString(CultureInfo.InvariantCulture);
            return slug;
        }
    }
}
