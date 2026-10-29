using System.Collections.Generic;
using System.IO;
using Il2CppInterop.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace MonkTookBookTranslate
{
    // Ставит кириллические шрифты первыми в список запасных у каждого шрифта игры
    static class Fonts
    {
        // Подстроки имён шрифтов игры, для которых нужен стилизованный шрифт; остальным — книжный
        static readonly string[] Decorative = { "Uncial", "MedievalSharp", "MiddleAges", "Gore", "Alphakind" };

        public const string DropCapFont = "MiddleAgesDeco";

        static TMP_FontAsset body, deco, dropCap;
        static readonly HashSet<int> patched = new();
        static int lastFrame = -1, lastDropCapFrame = -1;
        static bool failed;

        public static void Ensure()
        {
            if (failed || Time.frameCount == lastFrame) return;
            lastFrame = Time.frameCount;

            if (body == null)
            {
                body = Load("Alegreya.ttf");
                deco = Load("Kurale-Regular.ttf");
                if (body == null || deco == null) { failed = true; return; }
                patched.Add(body.GetInstanceID());
                patched.Add(deco.GetInstanceID());

                dropCap = Load("Ponomar-Regular.ttf");
                if (dropCap != null) patched.Add(dropCap.GetInstanceID());
            }

            foreach (var obj in Resources.FindObjectsOfTypeAll(Il2CppType.Of<TMP_FontAsset>()))
                Patch(obj.TryCast<TMP_FontAsset>());
        }

        // Тег <font="MiddleAgesDeco SDF"> TMP разрешает в момент отрисовки: сначала по таблице
        // MaterialReferenceManager / TMP_ResourceManager, затем из Resources. Экземпляров шрифта может быть
        // несколько, и буквица бралась из того, к которому Ponomar не подключён. Подключаем ко всем.
        public static void EnsureDropCapFont()
        {
            if (Time.frameCount == lastDropCapFrame) return;
            lastDropCapFrame = Time.frameCount;
            Ensure();
            if (dropCap == null) return;

            const string name = DropCapFont + " SDF";
            PatchDropCap(Resources.Load(TMP_Settings.defaultFontAssetPath + name, Il2CppType.Of<TMP_FontAsset>())?.TryCast<TMP_FontAsset>(), "Resources");
            foreach (var hash in new[]
            {
                TMP_TextUtilities.GetSimpleHashCode(name),
                TMP_TextUtilities.GetHashCode(name),
                (int)TMP_TextUtilities.GetHashCodeCaseInSensitive(name),
                (int)TMP_TextUtilities.GetSimpleHashCodeLowercase(name),
            })
            {
                if (MaterialReferenceManager.TryGetFontAsset(hash, out var a)) PatchDropCap(a, "MaterialReferenceManager");
                if (TMP_ResourceManager.TryGetFontAssetByName(hash, out var b)) PatchDropCap(b, "TMP_ResourceManager");
            }
            foreach (var obj in Resources.FindObjectsOfTypeAll(Il2CppType.Of<TMP_FontAsset>()))
            {
                var font = obj.TryCast<TMP_FontAsset>();
                if (font != null && font.name.Contains(DropCapFont)) PatchDropCap(font, "загружен");
            }
        }

        static void PatchDropCap(TMP_FontAsset font, string source)
        {
            if (font == null) return;
            ClearForeignCyrillic(font);
            // Проверяем сам список, а не только отметку: игра может заменить запасные шрифты
            var list = font.fallbackFontAssetTable;
            if (list != null && list.Contains(dropCap)) return;
            patched.Remove(font.GetInstanceID());
            if (Patch(font))
                Plugin.L.LogInfo($"Буквицы: Ponomar подключён к {font.name} #{font.GetInstanceID()} ({source})");
        }

        // Найденную в запасном шрифте букву TMP запоминает в таблице шрифта буквиц и больше не перезаписывает.
        // Если «Н» однажды взялась из Alegreya, так и оставалось до конца игры. Своей кириллицы в MiddleAgesDeco
        // нет, поэтому все такие записи — запомненные; убираем те, что не из Ponomar, и TMP найдёт букву заново.
        static void ClearForeignCyrillic(TMP_FontAsset font)
        {
            var table = font.characterLookupTable;
            if (table == null) return;
            var stale = new List<uint>();
            foreach (var kv in table)
            {
                var unicode = kv.Key & 0xFFFFFF;
                if (unicode < 0x400 || unicode > 0x4FF) continue;
                var owner = kv.Value?.textAsset;
                if (owner == null || owner.GetInstanceID() != dropCap.GetInstanceID()) stale.Add(kv.Key);
            }
            foreach (var key in stale) table.Remove(key);
            if (stale.Count > 0)
                Plugin.L.LogInfo($"Буквицы: в {font.name} #{font.GetInstanceID()} сброшено запомненных букв: {stale.Count}");
        }

        static bool Patch(TMP_FontAsset font)
        {
            if (font == null || !patched.Add(font.GetInstanceID())) return false;

            // MiddleAgesDeco — шрифт буквиц; кириллические буквицы берутся из старославянского Ponomar
            var ru = font.name.Contains(DropCapFont) && dropCap != null ? dropCap
                : IsDecorative(font.name) ? deco : body;
            var list = font.fallbackFontAssetTable ?? new Il2CppSystem.Collections.Generic.List<TMP_FontAsset>();
            if (!list.Contains(ru)) list.Insert(0, ru);
            font.fallbackFontAssetTable = list;
            return true;
        }

        public static bool HasDropCap => dropCap != null;

        static bool IsDecorative(string name)
        {
            foreach (var d in Decorative)
                if (name.Contains(d)) return true;
            return false;
        }

        static TMP_FontAsset Load(string file)
        {
            var path = Path.Combine(Plugin.Dir, "fonts", file);
            if (!File.Exists(path))
            {
                Plugin.L.LogError("Не найден шрифт: " + path);
                return null;
            }
            // Динамический атлас: глифы добавляются по мере появления в тексте
            var font = TMP_FontAsset.CreateFontAsset(path, 0, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024);
            if (font == null)
            {
                Plugin.L.LogError("Не удалось загрузить шрифт: " + path);
                return null;
            }
            font.name = "RU " + Path.GetFileNameWithoutExtension(file);
            font.hideFlags = HideFlags.DontUnloadUnusedAsset;
            Plugin.L.LogInfo("Загружен шрифт " + font.name);
            return font;
        }
    }
}
