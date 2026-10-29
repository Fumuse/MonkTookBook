using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonkTookBookTranslate.Setup
{
    class MainForm : Form
    {
        readonly float k;
        readonly TextBox dirBox;
        readonly GameButton browse, main, uninstall;
        readonly GameButton[] langButtons;
        readonly TextLabel status, subtitle, link;
        readonly GoldProgress progress;

        readonly Version bundled;
        Version installed;
        Updates.Release latest;
        string latestError;
        readonly string autoAction;

        int S(int px) => (int)Math.Round(px * k);
        Rectangle R(int x, int y, int w, int h) => new(S(x), S(y), S(w), S(h));

        public MainForm(string dir, string action)
        {
            k = DeviceDpi / 96f;
            autoAction = action;
            Text = Ui.Title;
            AutoScaleMode = AutoScaleMode.None;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.Background;
            ClientSize = new Size(S(600), S(430));
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            using (var zip = Payload()) bundled = Game.ArchiveVersion(zip);

            var title = new TextLabel { Text = "Monk Took Book", Font = Theme.Deco(26), ForeColor = Theme.Parchment, Bounds = R(26, 14, 548, 52) };
            subtitle = new TextLabel { Text = Ui.Subtitle(bundled), Font = Theme.Body(12), ForeColor = Theme.Gold, Bounds = R(30, 66, 540, 26) };

            var panel = new ParchmentPanel { Bounds = R(24, 104, 552, 222) };
            var dirLabel = new TextLabel { Text = Ui.GameFolder, Font = Theme.Body(10.5f, FontStyle.Bold), ForeColor = Theme.InkMuted, Bounds = R(20, 14, 300, 22) };
            dirBox = new TextBox
            {
                Bounds = R(20, 40, 420, 28), BorderStyle = BorderStyle.FixedSingle,
                BackColor = Theme.ParchmentLight, ForeColor = Theme.Ink, Font = new Font("Segoe UI", 10),
            };
            browse = new GameButton { Text = Ui.Browse, Fill = Theme.ParchmentLight, Hover = Color.White, Font = Theme.Body(11), Bounds = R(448, 39, 86, 30) };
            var langLabel = new TextLabel { Text = Ui.GameLanguage, Font = Theme.Body(10.5f, FontStyle.Bold), ForeColor = Theme.InkMuted, Bounds = R(20, 88, 110, 22) };
            // Названия языков — каждое на своём языке, как принято в переключателях
            var names = new[] { "Русский", "Deutsch", "English" };
            langButtons = new GameButton[Game.Languages.Length];
            for (var i = 0; i < langButtons.Length; i++)
                langButtons[i] = new GameButton { Text = names[i], Font = Theme.Body(11), Bounds = R(130 + i * 110, 82, 102, 32) };
            status = new TextLabel { Font = Theme.Body(12), ForeColor = Theme.Ink, Bounds = R(20, 130, 514, 62) };
            progress = new GoldProgress { Bounds = R(20, 198, 514, 10), Visible = false };
            panel.Controls.AddRange(new Control[] { dirLabel, dirBox, browse, langLabel, status, progress });
            panel.Controls.AddRange(langButtons);

            main = new GameButton { Font = Theme.Body(13.5f), Bounds = R(24, 342, 316, 46) };
            uninstall = new GameButton { Text = Ui.Uninstall, Fill = Theme.Parchment, Hover = Theme.ParchmentLight, Font = Theme.Body(11.5f), Bounds = R(352, 342, 224, 46) };
            link = new TextLabel
            {
                Text = "github.com/" + Updates.Repo, Font = Theme.Body(10), ForeColor = Color.FromArgb(150, 140, 125),
                Bounds = R(24, 398, 300, 22), Cursor = Cursors.Hand,
            };

            Controls.AddRange(new Control[] { title, subtitle, panel, main, uninstall, link });

            dirBox.Text = dir ?? Game.Find() ?? "";
            dirBox.TextChanged += (s, e) => UpdateView();
            browse.Click += (s, e) => Browse();
            main.Click += async (s, e) => await (UpdateAvailable ? DownloadUpdate() : Install());
            uninstall.Click += async (s, e) =>
            {
                if (!Dialog.Show(this, Ui.UninstallConfirm, DialogKind.Question)) return;
                await Uninstall();
            };
            for (var i = 0; i < langButtons.Length; i++)
            {
                var code = Game.Languages[i];
                langButtons[i].Click += async (s, e) => await SetLanguage(code);
            }
            link.Click += (s, e) => Process.Start(latest?.PageUrl ?? $"https://github.com/{Updates.Repo}/releases/latest");

            UpdateView();
            Shown += async (s, e) =>
            {
                // Перезапуск с правами администратора: сразу делаем то, что выбрали до перезапуска
                switch (autoAction)
                {
                    case "install": await Install(); break;
                    case "update": await CheckUpdates(); if (latest != null) await DownloadUpdate(); break;
                    case "uninstall": await Uninstall(); break;
                    case string a when a != null && a.StartsWith("lang-"): await SetLanguage(a.Substring(5)); break;
                }
                await CheckUpdates();
            };
        }

        bool UpdateAvailable
        {
            get
            {
                var newest = installed != null && installed > bundled ? installed : bundled;
                return latest != null && latest.Version > newest;
            }
        }

        Task Install() => Run("install", () => Task.Run(() =>
        {
            using var zip = Payload();
            Game.Install(dirBox.Text, zip, Progress());
        }), Ui.InstallDone);

        Task SetLanguage(string code) => Run("lang-" + code,
            () => Task.Run(() => Game.SetLanguage(dirBox.Text, code)), null);

        Task DownloadUpdate() => Run("update", async () =>
        {
            var file = await Updates.Download(latest, Progress());
            await Task.Run(() =>
            {
                using var zip = File.OpenRead(file);
                Game.Install(dirBox.Text, zip, Progress());
            });
            File.Delete(file);
        }, Ui.UpdateDone(latest?.Version));

        Task Uninstall() => Run("uninstall", () => Task.Run(() => Game.Uninstall(dirBox.Text)), Ui.Removed);

        static Stream Payload() =>
            Assembly.GetExecutingAssembly().GetManifestResourceStream("payload.zip");

        IProgress<int> Progress() => new Progress<int>(p => progress.Value = p);

        async Task CheckUpdates()
        {
            try { latest = await Updates.Latest(); latestError = null; }
            catch (Exception e) { latestError = e.Message; }
            UpdateView();
        }

        void Browse()
        {
            using var dialog = new FolderBrowserDialog { Description = Ui.BrowseHint };
            if (Directory.Exists(dirBox.Text)) dialog.SelectedPath = dirBox.Text;
            if (dialog.ShowDialog(this) == DialogResult.OK) dirBox.Text = dialog.SelectedPath;
        }

        void UpdateView()
        {
            var dir = dirBox.Text;
            var ok = Game.IsGameDir(dir);
            installed = ok ? Game.InstalledVersion(dir) : null;

            var lines = new System.Collections.Generic.List<string>();
            if (!ok)
                lines.Add(dir.Length == 0
                    ? Ui.NotFound
                    : Ui.NoExe);
            else if (installed != null)
                lines.Add(Ui.InstalledVersion(installed));
            else
                lines.Add(Game.HasLoader(dir) ? Ui.LoaderOnly : Ui.NotInstalled);
            if (UpdateAvailable)
                lines.Add(Ui.NewVersion(latest.Version));
            else if (latest != null && installed != null && installed >= latest.Version)
                lines.Add(Ui.Latest);
            else if (latestError != null)
                lines.Add(Ui.UpdateCheckFailed + latestError);
            status.Text = string.Join("\n", lines);

            main.Enabled = ok;
            main.Text = UpdateAvailable ? Ui.DownloadInstall(latest.Version)
                : installed == null ? Ui.Install
                : bundled > installed ? Ui.UpdateTo(bundled)
                : Ui.Reinstall;
            uninstall.Enabled = ok && Game.HasLoader(dir);

            // Выбранный язык — золотая кнопка, остальные — светлые
            var current = ok ? Game.GetLanguage(dir) : null;
            for (var i = 0; i < langButtons.Length; i++)
            {
                var button = langButtons[i];
                var selected = Game.Languages[i] == current;
                button.Enabled = ok && installed != null;
                button.Fill = selected ? Theme.Gold : Theme.ParchmentLight;
                button.Hover = selected ? Theme.GoldHover : Color.White;
                button.Invalidate();
            }
        }

        async Task Run(string action, Func<Task> work, string done)
        {
            if (Game.IsRunning())
            {
                Dialog.Show(this, Ui.CloseGame, DialogKind.Warning);
                return;
            }
            if (!Game.CanWrite(dirBox.Text) && !Elevation.IsAdmin)
            {
                // Папка защищена (обычно Program Files) — перезапускаемся с правами администратора
                try
                {
                    Elevation.Restart(dirBox.Text, action);
                    Close();
                }
                catch (Win32Exception)
                {
                    Dialog.Show(this, Ui.NoAdmin, DialogKind.Warning);
                }
                return;
            }

            SetBusy(true);
            try
            {
                await work();
                // done == null — действие видно по самому окну (выбор языка), сообщение не нужно
                if (done != null) Dialog.Show(this, done, DialogKind.Info);
            }
            catch (Exception e)
            {
                Dialog.Show(this, Ui.Error + e.Message, DialogKind.Error);
            }
            finally
            {
                SetBusy(false);
                UpdateView();
            }
        }

        void SetBusy(bool busy)
        {
            progress.Value = 0;
            progress.Visible = busy;
            dirBox.Enabled = browse.Enabled = main.Enabled = uninstall.Enabled = !busy;
            foreach (var b in langButtons) b.Enabled = !busy;
            UseWaitCursor = busy;
        }
    }

    static class Elevation
    {
        public static bool IsAdmin =>
            new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent())
                .IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);

        public static void Restart(string dir, string action) =>
            Process.Start(new ProcessStartInfo(Application.ExecutablePath, $"--dir \"{dir.TrimEnd('\\')}\" --do {action}")
            {
                UseShellExecute = true,
                Verb = "runas",
            });
    }

    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string dir = null, action = null;
            for (var i = 0; i + 1 < args.Length; i++)
            {
                if (args[i] == "--dir") dir = args[++i];
                else if (args[i] == "--do") action = args[++i];
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Theme.Load();
            Application.Run(new MainForm(dir, action));
        }
    }
}
