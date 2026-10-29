using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;

namespace MonkTookBookTranslate
{
    // Русский текст длиннее английского, а рамки у игры фиксированные.
    // Однословные надписи («Управление», «Настройки») переносились посреди слова,
    // а длинные тексты в рамках вылезали за края — даём шрифту ужаться.
    static class Fit
    {
        const float MinScale = 0.6f;
        const int ShortText = 20;

        // Многострочные тексты в рамках фиксированного размера
        static readonly HashSet<string> Boxed = new() { "_txt_HostAdvisoryText" };

        static readonly Regex Tags = new("<[^>]*>");
        static readonly Regex Cyrillic = new("[А-Яа-яЁё]");

        public static void Apply(string key, TMP_Text text)
        {
            if (text == null || text.enableAutoSizing) return;
            var visible = Tags.Replace(text.text ?? "", "").Trim();
            if (visible.Length == 0) return;
            // Английские надписи (непереведённые) не трогаем
            if (!Cyrillic.IsMatch(visible)) return;

            if (key == null || !Boxed.Contains(key))
            {
                // Кнопки и заголовки: одно слово или короткая надпись вроде «Воистину (R)»
                if (visible.Length > ShortText)
                    foreach (var c in visible)
                        if (char.IsWhiteSpace(c)) return;
                text.textWrappingMode = TextWrappingModes.NoWrap;
            }

            var size = text.fontSize;
            text.fontSizeMax = size;
            text.fontSizeMin = size * MinScale;
            text.enableAutoSizing = true;
        }
    }
}
