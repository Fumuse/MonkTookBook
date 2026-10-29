# Monk Took Book — Translate

Перевод игры [Monk Took Book](https://store.steampowered.com/app/4022090/) на **русский** и **немецкий** ·
Übersetzung von Monk Took Book ins **Deutsche** und **Russische** · [Deutsch ↓](#deutsch)

**[⬇ MonkTookBook_Translate_Setup.exe](https://github.com/Fumuse/MonkTookBook/releases/latest)**

---

## Русский

Весь текст интерфейса, записки, обучение и подсказки, шрифты с кириллицей в стиле игры.
Язык (русский, немецкий или оригинальный английский) переключается в установщике.

### Установка

1. Скачайте **[MonkTookBook_Translate_Setup.exe](https://github.com/Fumuse/MonkTookBook/releases/latest)**
   со страницы последнего релиза.
2. Запустите его. Установщик сам найдёт игру в Steam. Если не нашёл, укажите папку,
   где лежит `MonkTookBook.exe`, кнопкой «Обзор…».
3. Нажмите **«Установить»** и запустите игру через Steam.

Первый запуск после установки дольше обычного: около минуты после нажатия «Играть» окно игры
не появляется — загрузчик модов готовит файлы под вашу версию игры. Подождите и не запускайте
игру повторно. Следующие запуски обычные.

> **Windows может предупредить** «Windows защитила ваш компьютер» или «Издатель неизвестен»:
> установщик не подписан платным сертификатом. Нажмите «Подробнее» → «Выполнить в любом случае».
> Если игра стоит в `Program Files`, установщик попросит права администратора: без них
> туда не записать.

### Язык, обновление, удаление

- **Язык** — в установщике, строка «Язык игры»: «Русский», «Deutsch» или «English» (игра должна быть закрыта).
  На английском игра запускается полностью оригинальной: загрузчик модов просто выключается,
  перевод остаётся на месте и возвращается одной кнопкой.
- **Обновление** — запустите установщик ещё раз: если вышла новая версия, появится кнопка
  «Скачать и установить».
- **Удаление** — кнопка **«Удалить перевод»**: игра вернётся к исходному виду.

<details>
<summary>Ручная установка из архива</summary>

1. Скачайте `MonkTookBook_Translate.zip` со [страницы релиза](https://github.com/Fumuse/MonkTookBook/releases/latest).
2. Steam → библиотека → Monk Took Book → ПКМ → Управление → Просмотреть локальные файлы.
3. Скопируйте в открывшуюся папку всё содержимое архива (папки `BepInEx`, `dotnet` и файлы
   `winhttp.dll`, `doorstop_config.ini`, `.doorstop_version`), согласитесь на объединение папок
   (если потребуется).
4. Запустите игру — она будет на русском.

Для удаления удалите из папки игры папки `BepInEx`, `dotnet` и файлы `winhttp.dll`,
`doorstop_config.ini`, `.doorstop_version`.
</details>

### Мультиплеер

Перевод работает только у вас на компьютере.

### Если что-то не так

- Игра не запускается или текст остался английским — [создайте issue](https://github.com/Fumuse/MonkTookBook/issues)
  и приложите файл `BepInEx\LogOutput.log` из папки игры.
- Если ставили поверх неудачной установки — удалите перевод установщиком и поставьте заново.
- После обновления самой игры часть новых строк может остаться на английском до выхода новой версии перевода.

---

## Deutsch

Der gesamte Text der Oberfläche, Notizen, Tutorial und Hinweise.
Die Sprache (Deutsch, Russisch oder das englische Original) wählst du im Installer.

### Installation

1. Lade **[MonkTookBook_Translate_Setup.exe](https://github.com/Fumuse/MonkTookBook/releases/latest)**
   von der Seite des neuesten Releases herunter.
2. Starte sie. Der Installer findet das Spiel in Steam selbst. Falls nicht, wähle mit
   „Durchsuchen…“ den Ordner mit `MonkTookBook.exe`.
3. Klicke auf **„Installieren“** und starte das Spiel über Steam.

Der erste Start nach der Installation dauert länger: Etwa eine Minute nach „Spielen“ erscheint
kein Spielfenster – der Mod-Loader bereitet Dateien für deine Spielversion vor. Warte ab und
starte das Spiel nicht erneut. Danach startet es wie gewohnt.

> **Windows warnt eventuell** „Der Computer wurde durch Windows geschützt“ oder „Unbekannter
> Herausgeber“: Der Installer ist nicht mit einem kostenpflichtigen Zertifikat signiert.
> Klicke auf „Weitere Informationen“ → „Trotzdem ausführen“. Liegt das Spiel unter
> `Program Files`, fragt der Installer nach Administratorrechten.

### Sprache, Update, Entfernen

- **Sprache** – im Installer unter „Spielsprache“: „Deutsch“, „Русский“ oder „English“
  (das Spiel muss geschlossen sein). Auf Englisch startet das Spiel komplett im Original:
  Der Mod-Loader wird nur abgeschaltet und lässt sich mit einem Klick wieder aktivieren.
- **Update** – starte den Installer erneut: Gibt es eine neue Version, erscheint
  „Herunterladen und installieren“.
- **Entfernen** – **„Übersetzung entfernen“** stellt den Originalzustand wieder her.

<details>
<summary>Manuelle Installation aus dem Archiv</summary>

1. Lade `MonkTookBook_Translate.zip` von der [Release-Seite](https://github.com/Fumuse/MonkTookBook/releases/latest).
2. Steam → Bibliothek → Monk Took Book → Rechtsklick → Verwalten → Lokale Dateien durchsuchen.
3. Kopiere den gesamten Archivinhalt in diesen Ordner (Ordner `BepInEx`, `dotnet` und die Dateien
   `winhttp.dll`, `doorstop_config.ini`, `.doorstop_version`) und bestätige das Zusammenführen
   der Ordner (falls gefragt).
4. Für Deutsch: Lege die Datei `BepInEx\config\MonkTookBook_Translate.language` mit dem Inhalt `de` an
   und ändere in `BepInEx\config\AutoTranslatorConfig.ini` die Zeile `Language=ru` zu `Language=de`.
5. Starte das Spiel.
</details>

### Mehrspieler

Die Übersetzung wirkt nur auf deinem PC.

### Probleme

- Das Spiel startet nicht oder der Text bleibt englisch – [eröffne ein Issue](https://github.com/Fumuse/MonkTookBook/issues)
  und hänge die Datei `BepInEx\LogOutput.log` aus dem Spielordner an.

---

## Состав · Bestandteile

- [BepInEx 6](https://github.com/BepInEx/BepInEx) — загрузчик модов · Mod-Loader (LGPL-2.1).
- [XUnity.AutoTranslator](https://github.com/bbepis/XUnity.AutoTranslator) — перевод отдельных строк · einzelne Texte (MIT).
- MonkTookBook_Translate — плагин перевода из этого репозитория · Übersetzungs-Plugin aus diesem Repository.
- Шрифты · Schriften: Alegreya, Kurale, Ponomar — SIL Open Font License 1.1.

Файлы самой игры не изменяются и в перевод не входят · Die Spieldateien werden nicht verändert und sind nicht enthalten.
