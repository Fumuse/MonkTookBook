using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonkTookBookTranslate.Setup
{
    // Новые версии берутся из последнего релиза на GitHub (не pre-release): тег vX.Y.Z и приложенный архив MonkTookBook_Translate.zip
    static class Updates
    {
        // Репозиторий вида "владелец/имя"; пусто — проверка обновлений выключена
        public const string Repo = "Fumuse/MonkTookBook";

        public class Release
        {
            public Version Version;
            public string ZipUrl;
            public string PageUrl;
        }

        static WebClient Client()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            var web = new WebClient();
            web.Headers[HttpRequestHeader.UserAgent] = "MonkTookBook_Translate_Setup";
            return web;
        }

        public static async Task<Release> Latest()
        {
            if (Repo.Length == 0) return null;
            using var web = Client();
            // /releases/latest пропускает черновики и pre-release — те для проверки, игрокам не предлагаются
            string json;
            try { json = await web.DownloadStringTaskAsync($"https://api.github.com/repos/{Repo}/releases/latest"); }
            // 404 — обычных релизов ещё нет
            catch (WebException e) when ((e.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound) { return null; }

            var tag = Regex.Match(json, "\"tag_name\"\\s*:\\s*\"v?([^\"]+)\"").Groups[1].Value;
            var zip = Regex.Match(json, "\"browser_download_url\"\\s*:\\s*\"([^\"]+\\.zip)\"").Groups[1].Value;
            if (!Version.TryParse(tag, out var version) || zip.Length == 0) return null;
            return new Release { Version = version, ZipUrl = zip, PageUrl = $"https://github.com/{Repo}/releases/tag/v{version}" };
        }

        public static async Task<string> Download(Release release, IProgress<int> progress)
        {
            var path = Path.Combine(Path.GetTempPath(), "MonkTookBook_Translate_" + release.Version + ".zip");
            using var web = Client();
            web.DownloadProgressChanged += (s, e) => progress.Report(e.ProgressPercentage);
            await web.DownloadFileTaskAsync(release.ZipUrl, path);
            return path;
        }
    }
}
