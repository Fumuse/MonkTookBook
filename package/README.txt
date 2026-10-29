Monk Took Book — Translate (русский)
====================================
https://github.com/Fumuse/MonkTookBook

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
В установщике — строка «Язык игры»: «Русский» или «English».
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

--- ИЗ ЧЕГО СОСТОИТ ---
- BepInEx 6 (IL2CPP), LGPL-2.1: https://github.com/BepInEx/BepInEx
- XUnity.AutoTranslator, MIT: https://github.com/bbepis/XUnity.AutoTranslator
- MonkTookBook_Translate (BepInEx\plugins\MonkTookBookTranslate)
- Alegreya, Kurale, Ponomar — SIL Open Font License 1.1 (BepInEx\plugins\MonkTookBookTranslate\fonts)

Файлы самой игры в архиве отсутствуют и не изменяются.
