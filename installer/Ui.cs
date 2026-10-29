using System.Globalization;

namespace MonkTookBookTranslate.Setup
{
    // Тексты окна установщика на языке Windows: русский, немецкий, иначе английский
    static class Ui
    {
        public static readonly string Lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
        {
            "ru" or "uk" or "be" or "kk" => "ru",
            "de" => "de",
            _ => "en",
        };

        static string T(string ru, string de, string en) => Lang == "ru" ? ru : Lang == "de" ? de : en;

        public static string Title => T("Monk Took Book — перевод", "Monk Took Book — Übersetzung", "Monk Took Book — Translation");
        public static string Subtitle(object v) => T($"Перевод · версия {v}", $"Übersetzung · Version {v}", $"Translation · version {v}");
        public static string GameFolder => T("Папка игры", "Spielordner", "Game folder");
        public static string Browse => T("Обзор…", "Durchsuchen…", "Browse…");
        public static string BrowseHint => T("Папка, где лежит MonkTookBook.exe", "Ordner mit MonkTookBook.exe", "Folder containing MonkTookBook.exe");
        public static string GameLanguage => T("Язык игры", "Spielsprache", "Game language");
        public static string Uninstall => T("Удалить перевод", "Übersetzung entfernen", "Remove translation");
        public static string UninstallConfirm => T("Удалить перевод и BepInEx из папки игры?\nИгра вернётся к английскому.",
            "Übersetzung und BepInEx aus dem Spielordner entfernen?\nDas Spiel ist dann wieder auf Englisch.",
            "Remove the translation and BepInEx from the game folder?\nThe game will be back to English.");
        public static string InstallDone => T("Перевод установлен. Можно запускать игру.", "Übersetzung installiert. Du kannst das Spiel starten.", "Translation installed. You can start the game.");
        public static string UpdateDone(object v) => T($"Установлена версия {v}. Можно запускать игру.", $"Version {v} installiert. Du kannst das Spiel starten.", $"Version {v} installed. You can start the game.");
        public static string Removed => T("Перевод удалён.", "Übersetzung entfernt.", "Translation removed.");
        public static string NotFound => T("Игра не найдена автоматически — укажите папку, где лежит MonkTookBook.exe.",
            "Spiel nicht automatisch gefunden – wähle den Ordner mit MonkTookBook.exe.",
            "Game not found automatically — choose the folder containing MonkTookBook.exe.");
        public static string NoExe => T("В этой папке нет MonkTookBook.exe.", "In diesem Ordner liegt keine MonkTookBook.exe.", "There is no MonkTookBook.exe in this folder.");
        public static string InstalledVersion(object v) => T($"Установлена версия {v}.", $"Installierte Version: {v}.", $"Installed version: {v}.");
        public static string LoaderOnly => T("BepInEx установлен, перевод — нет.", "BepInEx ist installiert, die Übersetzung nicht.", "BepInEx is installed, the translation is not.");
        public static string NotInstalled => T("Перевод ещё не установлен.", "Die Übersetzung ist noch nicht installiert.", "The translation is not installed yet.");
        public static string NewVersion(object v) => T($"Вышла новая версия {v} — её можно скачать и установить.",
            $"Version {v} ist erschienen – du kannst sie herunterladen und installieren.",
            $"Version {v} is out — you can download and install it.");
        public static string Latest => T("Это последняя версия.", "Das ist die neueste Version.", "This is the latest version.");
        public static string UpdateCheckFailed => T("Не удалось проверить обновления: ", "Update-Prüfung fehlgeschlagen: ", "Could not check for updates: ");
        public static string DownloadInstall(object v) => T($"Скачать и установить {v}", $"{v} herunterladen und installieren", $"Download and install {v}");
        public static string Install => T("Установить", "Installieren", "Install");
        public static string UpdateTo(object v) => T($"Обновить до {v}", $"Auf {v} aktualisieren", $"Update to {v}");
        public static string Reinstall => T("Переустановить", "Neu installieren", "Reinstall");
        public static string CloseGame => T("Закройте игру и попробуйте снова.", "Schließe das Spiel und versuche es erneut.", "Close the game and try again.");
        public static string NoAdmin => T("Без прав администратора в эту папку не записать.", "Ohne Administratorrechte kann in diesen Ordner nicht geschrieben werden.", "This folder cannot be written to without administrator rights.");
        public static string Error => T("Ошибка: ", "Fehler: ", "Error: ");
        public static string Yes => T("Да", "Ja", "Yes");
        public static string No => T("Нет", "Nein", "No");
        public static string Ok => T("Хорошо", "OK", "OK");
    }
}
