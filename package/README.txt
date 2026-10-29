Monk Took Book — Translate (русский / Deutsch)
=============================================
https://github.com/Fumuse/MonkTookBook

--- РУССКИЙ ---

УСТАНОВКА
Проще всего — запустить MonkTookBook_Translate_Setup.exe: он сам найдёт игру, установит,
обновит или удалит перевод и переключит язык. Ниже — ручная установка из архива.

1. Steam → библиотека → Monk Took Book → ПКМ → Управление → Просмотреть локальные файлы.
   Откроется папка игры (там лежит MonkTookBook.exe).
2. Скопируйте в эту папку всё содержимое архива (папки BepInEx, dotnet и файлы
   winhttp.dll, doorstop_config.ini, .doorstop_version). Согласитесь на объединение папок
   (если потребуется).
3. Запустите игру через Steam — она будет на русском.

Первый запуск дольше обычного: около минуты после нажатия «Играть» окно игры не появляется —
загрузчик модов готовит файлы под вашу версию игры. Подождите и не запускайте игру повторно.
Интернет для этого не нужен. Следующие запуски обычные.

Если ставите поверх неудачной установки — сначала удалите папку BepInEx целиком.

ПЕРЕКЛЮЧЕНИЕ ЯЗЫКА
В установщике — строка «Язык игры»: «Русский», «Deutsch» или «English».
Вручную: переименуйте winhttp.dll в winhttp.dll.off — игра запустится на английском
(полностью оригинальной). Чтобы вернуть перевод, верните файлу имя winhttp.dll.

УДАЛЕНИЕ
Удалите из папки игры: папки BepInEx и dotnet, файлы winhttp.dll, doorstop_config.ini,
.doorstop_version, winhttp.dll.off (если есть).

МУЛЬТИПЛЕЕР
Перевод работает только у вас на компьютере.

ЕСЛИ ЧТО-ТО НЕ ТАК
- Игра не запускается или текст на английском — пришлите файл BepInEx\LogOutput.log.
- После обновления игры перевод может частично слететь: новые строки будут на английском.

--- DEUTSCH ---

INSTALLATION
Am einfachsten startest du MonkTookBook_Translate_Setup.exe: Der Installer findet das Spiel,
installiert, aktualisiert oder entfernt die Übersetzung und wechselt die Sprache.
Manuelle Installation aus dem Archiv:

1. Steam → Bibliothek → Monk Took Book → Rechtsklick → Verwalten → Lokale Dateien durchsuchen.
2. Kopiere den gesamten Archivinhalt in diesen Ordner (Ordner BepInEx, dotnet und die Dateien
   winhttp.dll, doorstop_config.ini, .doorstop_version) und bestätige das Zusammenführen
   der Ordner (falls gefragt).
3. Lege die Datei BepInEx\config\MonkTookBook_Translate.language mit dem Inhalt de an
   und ändere in BepInEx\config\AutoTranslatorConfig.ini die Zeile Language=ru zu Language=de.
4. Starte das Spiel über Steam.

Der erste Start dauert länger: Etwa eine Minute nach „Spielen“ erscheint kein Spielfenster –
der Mod-Loader bereitet Dateien für deine Spielversion vor. Warte ab und starte das Spiel
nicht erneut. Danach startet es wie gewohnt.

SPRACHE
Im Installer unter „Spielsprache“. Manuell: Benenne winhttp.dll in winhttp.dll.off um –
das Spiel startet dann im englischen Original.

ENTFERNEN
Lösche im Spielordner die Ordner BepInEx und dotnet sowie winhttp.dll, doorstop_config.ini,
.doorstop_version und winhttp.dll.off (falls vorhanden).

MEHRSPIELER
Die Übersetzung wirkt nur auf deinem PC.

PROBLEME
Das Spiel startet nicht oder der Text bleibt englisch – schick die Datei BepInEx\LogOutput.log.

--- BESTANDTEILE / ИЗ ЧЕГО СОСТОИТ ---
- BepInEx 6 (IL2CPP), LGPL-2.1: https://github.com/BepInEx/BepInEx
- XUnity.AutoTranslator, MIT: https://github.com/bbepis/XUnity.AutoTranslator
- MonkTookBook_Translate (BepInEx\plugins\MonkTookBookTranslate)
- Alegreya, Kurale, Ponomar — SIL Open Font License 1.1 (BepInEx\plugins\MonkTookBookTranslate\fonts)

Файлы самой игры в архиве отсутствуют и не изменяются.
Die Spieldateien sind nicht enthalten und werden nicht verändert.
