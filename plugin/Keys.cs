using System.Collections.Generic;
using System.Text.RegularExpressions;
using HarmonyLib;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace MonkTookBookTranslate
{
    // Input System подписывает клавиши по текущей раскладке Windows («У» вместо E).
    // Возвращаем латинские названия физических клавиш.
    [HarmonyPatch]
    static class Keys
    {
        static readonly Regex Cyrillic = new("[А-Яа-яЁё]+");

        static readonly Dictionary<string, string> Punct = new()
        {
            ["semicolon"] = ";", ["quote"] = "'", ["comma"] = ",", ["period"] = ".",
            ["slash"] = "/", ["backslash"] = "\\", ["leftbracket"] = "[", ["rightbracket"] = "]",
            ["backquote"] = "`", ["minus"] = "-", ["equals"] = "=",
        };

        static string KeyName(string name)
        {
            var n = name.ToLowerInvariant();
            if (n.StartsWith("digit")) n = n.Substring(5);
            if (Punct.TryGetValue(n, out var p)) return p;
            return n.Length == 1 ? n.ToUpperInvariant() : name;
        }

        static void FixControl(InputControl control, ref string result)
        {
            if (result == null || !Cyrillic.IsMatch(result)) return;
            var key = control?.TryCast<KeyControl>();
            if (key != null) result = KeyName(key.keyCode.ToString());
        }

        static void FixPath(string path, ref string result)
        {
            if (result == null || path == null || !Cyrillic.IsMatch(result)) return;
            var i = path.IndexOf("<Keyboard>/", System.StringComparison.OrdinalIgnoreCase);
            if (i < 0) return;
            var name = KeyName(path.Substring(i + "<Keyboard>/".Length));
            result = Cyrillic.Replace(result, name);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InputControl), nameof(InputControl.displayName), MethodType.Getter)]
        static void DisplayName(InputControl __instance, ref string __result) => FixControl(__instance, ref __result);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InputControl), nameof(InputControl.shortDisplayName), MethodType.Getter)]
        static void ShortDisplayName(InputControl __instance, ref string __result) => FixControl(__instance, ref __result);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InputControlPath), nameof(InputControlPath.ToHumanReadableString),
            new[] { typeof(string), typeof(InputControlPath.HumanReadableStringOptions), typeof(InputControl) })]
        static void HumanReadable(string __0, ref string __result) => FixPath(__0, ref __result);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InputControlPath), nameof(InputControlPath.ToHumanReadableString),
            new[] { typeof(string), typeof(string), typeof(string), typeof(InputControlPath.HumanReadableStringOptions), typeof(InputControl) },
            new[] { ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal })]
        static void HumanReadableOut(string __0, ref string __result) => FixPath(__0, ref __result);
    }

    // Буквица: в MiddleAgesDeco нет кириллицы, её буквы рисуются запасным Ponomar (см. Fonts).
    // Без узоров оригинала он смотрится мельче — немного увеличиваем кегль кириллических буквиц.
    [HarmonyPatch]
    static class DropCap
    {
        const float SizeScale = 1.2f;

        static readonly Regex Deco = new("<font=\"MiddleAgesDeco SDF\">(.*?)</font>");
        static readonly Regex Tags = new("<[^>]*>");
        static readonly Regex Size = new(@"<size=([\d.]+)");

        static void Fix(ref string s)
        {
            if (s == null) return;
            Fonts.EnsureDropCapFont();
            if (!Fonts.HasDropCap) return;
            s = Deco.Replace(s, Replace);
        }

        static string Replace(Match m)
        {
            var inner = m.Groups[1].Value;
            var letter = Tags.Replace(inner, "");
            if (letter.Length == 0 || letter[0] < 'Ѐ' || letter[0] > 'ӿ') return m.Value;
            return m.Value.Replace(inner, Size.Replace(inner, ScaleSize));
        }

        static string ScaleSize(Match m)
        {
            var v = float.Parse(m.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
            return "<size=" + (v * SizeScale).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LocalisationManager), nameof(LocalisationManager.SetFirstLetterOfStringMedDeco))]
        static void Manager(ref string __result) => Fix(ref __result);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LocKey), nameof(LocKey.SetFirstLetterOfStringMedDeco))]
        static void Key(ref string __result) => Fix(ref __result);

        // Окна-подсказки делают буквицу своим методом. DialogBox ищем по имени:
        // прямая ссылка потянула бы сборку DevionGames.Utilities
        [HarmonyPatch]
        internal static class Box
        {
            static System.Reflection.MethodBase TargetMethod() =>
                AccessTools.Method(AccessTools.TypeByName("DialogBox"), "SetFirstLetterOfStringMedDeco");

            static void Postfix(ref string __result) => Fix(ref __result);
        }
    }
}
