using System;
using System.Drawing;
using System.Windows.Forms;

namespace MonkTookBookTranslate.Setup
{
    enum DialogKind { Info, Warning, Error, Question }

    // Окно сообщения в оформлении установщика вместо стандартного MessageBox
    class Dialog : Form
    {
        public static bool Show(IWin32Window owner, string message, DialogKind kind)
        {
            using var d = new Dialog(message, kind);
            return d.ShowDialog(owner) == DialogResult.Yes;
        }

        readonly float k;
        DialogResult acceptResult = DialogResult.OK;
        int S(int px) => (int)Math.Round(px * k);

        Dialog(string message, DialogKind kind)
        {
            k = DeviceDpi / 96f;
            Text = Ui.Title;
            AutoScaleMode = AutoScaleMode.None;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Theme.Background;
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var font = Theme.Body(12.5f);
            var textWidth = S(390);
            int textHeight;
            using (var g = CreateGraphics())
                textHeight = (int)Math.Ceiling(g.MeasureString(message, font, textWidth).Height) + S(4);
            textHeight = Math.Max(textHeight, S(28));

            var panel = new ParchmentPanel { Bounds = new Rectangle(S(16), S(16), S(458), textHeight + S(40)) };
            var accent = new Panel
            {
                Bounds = new Rectangle(S(18), S(20), S(5), textHeight),
                BackColor = kind switch
                {
                    DialogKind.Info => Theme.Green,
                    DialogKind.Error => Theme.Red,
                    _ => Theme.Gold,
                },
            };
            var text = new TextLabel { Text = message, Font = font, ForeColor = Theme.Ink, Bounds = new Rectangle(S(38), S(20), textWidth + S(20), textHeight) };
            panel.Controls.AddRange(new Control[] { accent, text });

            var top = panel.Bottom + S(14);
            Controls.Add(panel);
            if (kind == DialogKind.Question)
            {
                AddButton(Ui.Yes, DialogResult.Yes, new Rectangle(S(254), top, S(104), S(38)), Theme.Gold, Theme.GoldHover, true);
                AddButton(Ui.No, DialogResult.No, new Rectangle(S(370), top, S(104), S(38)), Theme.Parchment, Theme.ParchmentLight, false);
            }
            else
                AddButton(Ui.Ok, DialogResult.OK, new Rectangle(S(354), top, S(120), S(38)), Theme.Gold, Theme.GoldHover, true);

            ClientSize = new Size(S(490), top + S(38) + S(16));
        }

        void AddButton(string text, DialogResult result, Rectangle bounds, Color fill, Color hover, bool accept)
        {
            var b = new GameButton { Text = text, Font = Theme.Body(12), Bounds = bounds, Fill = fill, Hover = hover };
            b.Click += (s, e) => { DialogResult = result; Close(); };
            Controls.Add(b);
            if (!accept) return;
            acceptResult = result;
            Shown += (s, e) => b.Focus();
        }

        protected override bool ProcessDialogKey(Keys key)
        {
            // Enter — первая (основная) кнопка, Esc — закрыть
            if (key == Keys.Enter) { DialogResult = acceptResult; Close(); return true; }
            if (key == Keys.Escape) { DialogResult = DialogResult.No; Close(); return true; }
            return base.ProcessDialogKey(key);
        }
    }
}
