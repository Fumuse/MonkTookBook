using System.Collections.Generic;
using System.IO;
using System.Xml;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace MonkTookBookTranslate
{
    [BepInPlugin("monktookbook.translate", "Monk Took Book — Translate", "1.1.0")]
    public class Plugin : BasePlugin
    {
        // Файл с кодом языка пишет установщик; английский — это выключенный загрузчик, плагин тогда не грузится
        const string LanguageFile = "MonkTookBook_Translate.language";
        static readonly Dictionary<string, string> Files = new() { ["ru"] = "Russian.xml", ["de"] = "German.xml" };

        internal static Dictionary<string, string> Strings = new();
        internal static ManualLogSource L;
        internal static string Dir;
        internal static string Language;

        public override void Load()
        {
            L = Log;
            Dir = Path.GetDirectoryName(typeof(Plugin).Assembly.Location);
            Language = ReadLanguage();
            var path = Path.Combine(Dir, Files[Language]);
            if (!File.Exists(path))
            {
                L.LogError("Не найден файл перевода: " + path);
                return;
            }

            // Тот же разбор, что и у игры: пробельные узлы вокруг CDATA отбрасываются
            var doc = new XmlDocument();
            doc.Load(path);
            foreach (XmlNode entry in doc.DocumentElement.ChildNodes)
            {
                var key = entry["Key"]?.InnerText;
                var value = entry["Value"]?.InnerText;
                if (!string.IsNullOrEmpty(key) && value != null)
                    Strings[key] = value;
            }

            var harmony = new Harmony("monktookbook.translate");
            harmony.PatchAll(typeof(Patches));
            harmony.PatchAll(typeof(DropCap));
            try { harmony.PatchAll(typeof(DropCap.Box)); }
            catch (System.Exception e) { L.LogWarning("Не удалось перехватить буквицы подсказок: " + e.Message); }
            // Подписи клавиш — отдельно: если хуки не встанут, перевод всё равно должен работать
            try { harmony.PatchAll(typeof(Keys)); }
            catch (System.Exception e) { L.LogWarning("Не удалось перехватить подписи клавиш: " + e.Message); }
            L.LogInfo($"Язык: {Language}, загружено строк перевода: {Strings.Count}");
        }

        static string ReadLanguage()
        {
            try
            {
                var path = Path.Combine(Paths.ConfigPath, LanguageFile);
                var code = File.Exists(path) ? File.ReadAllText(path).Trim().ToLowerInvariant() : "";
                if (Files.ContainsKey(code)) return code;
            }
            catch { }
            return "ru";
        }
    }

    [HarmonyPatch]
    static class Patches
    {
        // Игра заполняет словарь из English.xml — сразу после этого подменяем значения
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LocalisationManager), nameof(LocalisationManager.SetUpLocalisationData))]
        static void AfterSetUp(LocalisationManager __instance)
        {
            var data = __instance.localisationData;
            if (data == null) return;
            foreach (var kv in Plugin.Strings)
                data[kv.Key] = kv.Value;
            Plugin.L.LogInfo("Словарь локализации заменён: " + Plugin.Language);
            Fonts.Ensure();
        }

        // Шрифты подгружаются вместе со сценами — проверяем при появлении текстов
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LocKey), nameof(LocKey.SetText))]
        static void AfterLocKeySetText(LocKey __instance) => AfterLocKey(__instance);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LocKey), nameof(LocKey.SetTextWith))]
        static void AfterLocKeySetTextWith(LocKey __instance) => AfterLocKey(__instance);

        static void AfterLocKey(LocKey key)
        {
            Fonts.Ensure();
            try { Fit.Apply(key.locKey, key.UIText); }
            catch (System.Exception e) { Plugin.L.LogWarning("Fit: " + e.Message); }
        }

        // Страховка на случай, если словарь пересоздаётся без SetUpLocalisationData
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LocalisationManager), nameof(LocalisationManager.GetLocString))]
        static void AfterGetLocString(string __0, ref string __result)
        {
            if (__0 != null && Plugin.Strings.TryGetValue(__0, out var tr))
                __result = tr;
        }
    }
}
