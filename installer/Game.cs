using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace MonkTookBookTranslate.Setup
{
    // Поиск игры, установка перевода из архива, выбор языка и удаление
    static class Game
    {
        const string GameFolder = "Monk Took Book";
        const string Exe = "MonkTookBook.exe";
        const string PluginDir = @"BepInEx\plugins\MonkTookBookTranslate";
        const string VersionFile = PluginDir + @"\version.txt";

        // Всё, что русификатор кладёт в папку игры
        static readonly string[] Dirs = { "BepInEx", "dotnet" };
        static readonly string[] Files = { Loader, LoaderOff, "doorstop_config.ini", ".doorstop_version", "changelog.txt" };

        // Через winhttp.dll Windows подгружает BepInEx. Английский язык = загрузчик выключен (файл переименован):
        // игра запускается полностью оригинальной, без смеси русских и английских надписей.
        const string Loader = "winhttp.dll";
        const string LoaderOff = "winhttp.dll.off";

        public static bool IsGameDir(string dir) =>
            !string.IsNullOrEmpty(dir) && File.Exists(Path.Combine(dir, Exe));

        public static string Find()
        {
            foreach (var lib in SteamLibraries())
            {
                var dir = Path.Combine(lib, "steamapps", "common", GameFolder);
                if (IsGameDir(dir)) return dir;
            }
            return null;
        }

        static IEnumerable<string> SteamLibraries()
        {
            var steam = new List<string>();
            foreach (var (root, key, value) in new[]
            {
                (Registry.CurrentUser, @"Software\Valve\Steam", "SteamPath"),
                (Registry.LocalMachine, @"SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath"),
                (Registry.LocalMachine, @"SOFTWARE\Valve\Steam", "InstallPath"),
            })
            {
                try
                {
                    using var k = root.OpenSubKey(key);
                    if (k?.GetValue(value) is string path && Directory.Exists(path))
                        steam.Add(path.Replace('/', '\\'));
                }
                catch { }
            }

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var s in steam)
            {
                if (seen.Add(s)) yield return s;
                var vdf = Path.Combine(s, "steamapps", "libraryfolders.vdf");
                if (!File.Exists(vdf)) continue;
                foreach (Match m in Regex.Matches(File.ReadAllText(vdf), "\"path\"\\s+\"([^\"]+)\""))
                {
                    var lib = m.Groups[1].Value.Replace(@"\\", @"\");
                    if (seen.Add(lib)) yield return lib;
                }
            }
        }

        public static bool IsRunning() =>
            Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Exe)).Length > 0;

        public static Version InstalledVersion(string dir)
        {
            var path = Path.Combine(dir, VersionFile);
            if (!File.Exists(path)) return null;
            return Version.TryParse(File.ReadAllText(path).Trim(), out var v) ? v : null;
        }

        public static bool CanWrite(string dir)
        {
            try
            {
                var probe = Path.Combine(dir, ".ru_write_test");
                File.WriteAllText(probe, "");
                File.Delete(probe);
                return true;
            }
            catch { return false; }
        }

        public static bool HasLoader(string dir) =>
            File.Exists(Path.Combine(dir, Loader)) || File.Exists(Path.Combine(dir, LoaderOff));

        // Язык перевода для плагина и XUnity.AutoTranslator; английский = выключенный загрузчик
        const string LanguageFile = @"BepInEx\config\MonkTookBook_Translate.language";
        const string XuaConfig = @"BepInEx\config\AutoTranslatorConfig.ini";
        public static readonly string[] Languages = { "ru", "en" };

        const string DefaultLanguage = "ru";

        public static string GetLanguage(string dir)
        {
            if (!File.Exists(Path.Combine(dir, Loader)) && File.Exists(Path.Combine(dir, LoaderOff))) return "en";
            return SavedLanguage(dir);
        }

        static string SavedLanguage(string dir)
        {
            try
            {
                var path = Path.Combine(dir, LanguageFile);
                var code = File.Exists(path) ? File.ReadAllText(path).Trim() : "";
                if (code == "ru") return code;
            }
            catch { }
            return DefaultLanguage;
        }

        public static void SetLanguage(string dir, string code)
        {
            if (code == "en")
            {
                MoveLoader(dir, false);
                return;
            }
            Directory.CreateDirectory(Path.Combine(dir, @"BepInEx\config"));
            File.WriteAllText(Path.Combine(dir, LanguageFile), code);
            // XUA берёт строки вне таблицы игры из BepInEx\Translation\<язык>
            var xua = Path.Combine(dir, XuaConfig);
            if (File.Exists(xua))
                File.WriteAllText(xua, Regex.Replace(File.ReadAllText(xua), @"(?m)^Language=[^\r\n]*", "Language=" + code));
            MoveLoader(dir, true);
        }

        static void MoveLoader(string dir, bool on)
        {
            var enabled = Path.Combine(dir, Loader);
            var disabled = Path.Combine(dir, LoaderOff);
            var (from, to) = on ? (disabled, enabled) : (enabled, disabled);
            if (!File.Exists(from)) return;
            if (File.Exists(to)) File.Delete(to);
            File.Move(from, to);
        }

        public static Version ArchiveVersion(Stream zip)
        {
            using var archive = new ZipArchive(zip, ZipArchiveMode.Read, true);
            var entry = archive.GetEntry(VersionFile.Replace('\\', '/'));
            if (entry == null) return null;
            using var r = new StreamReader(entry.Open());
            return Version.TryParse(r.ReadToEnd().Trim(), out var v) ? v : null;
        }

        public static void Install(string dir, Stream zip, IProgress<int> progress)
        {
            var root = Path.GetFullPath(dir).TrimEnd('\\') + "\\";
            // Архив перезапишет конфиги — запоминаем выбранный перевод (английский после установки не оставляем)
            var language = SavedLanguage(root);
            using var archive = new ZipArchive(zip, ZipArchiveMode.Read, true);
            // Папку плагина ставим начисто, чтобы не оставались файлы прошлых версий (например, заменённые шрифты).
            // Настройки игрока лежат в BepInEx\config и не затрагиваются.
            // MonkTookBookRU — прежнее имя папки плагина: два плагина сразу мешали бы друг другу
            foreach (var old in new[] { PluginDir, @"BepInEx\plugins\MonkTookBookRU" })
            {
                var plugin = Path.Combine(root, old);
                if (Directory.Exists(plugin)) Directory.Delete(plugin, true);
            }
            var entries = archive.Entries;
            for (var i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                progress?.Report(i * 100 / entries.Count);
                // Инструкция для ручной установки в папке игры не нужна
                if (e.FullName.EndsWith("/") || e.FullName == "README.txt") continue;
                var target = Path.GetFullPath(Path.Combine(root, e.FullName));
                if (!target.StartsWith(root, StringComparison.OrdinalIgnoreCase)) continue;
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                e.ExtractToFile(target, true);
            }
            var off = Path.Combine(root, LoaderOff);
            if (File.Exists(off)) File.Delete(off);
            SetLanguage(root, language);
            progress?.Report(100);
        }

        public static void Uninstall(string dir)
        {
            foreach (var d in Dirs)
            {
                var path = Path.Combine(dir, d);
                if (Directory.Exists(path)) Directory.Delete(path, true);
            }
            foreach (var f in Files)
            {
                var path = Path.Combine(dir, f);
                if (File.Exists(path)) File.Delete(path);
            }
        }
    }
}
