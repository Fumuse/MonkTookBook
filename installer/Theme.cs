using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MonkTookBookTranslate.Setup
{
    // Оформление в духе интерфейса игры: тёмный фон, пергамент, золотые кнопки, шрифты русификатора
    static class Theme
    {
        public static readonly Color Background = Color.FromArgb(36, 30, 27);
        public static readonly Color Parchment = Color.FromArgb(192, 188, 168);
        public static readonly Color ParchmentLight = Color.FromArgb(214, 210, 192);
        public static readonly Color Ink = Color.FromArgb(43, 38, 34);
        public static readonly Color InkMuted = Color.FromArgb(96, 88, 78);
        public static readonly Color Gold = Color.FromArgb(200, 166, 87);
        public static readonly Color GoldHover = Color.FromArgb(218, 186, 110);
        public static readonly Color Red = Color.FromArgb(124, 51, 51);
        public static readonly Color Green = Color.FromArgb(77, 104, 58);

        static readonly PrivateFontCollection fonts = new();
        static FontFamily body, deco;

        public static void Load()
        {
            body = Add("Alegreya.ttf") ?? FontFamily.GenericSerif;
            deco = Add("Kurale-Regular.ttf") ?? body;
        }

        static FontFamily Add(string name)
        {
            try
            {
                using var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
                if (s == null) return null;
                var data = new byte[s.Length];
                s.Read(data, 0, data.Length);
                var ptr = Marshal.AllocCoTaskMem(data.Length);
                Marshal.Copy(data, 0, ptr, data.Length);
                var before = fonts.Families.Length;
                fonts.AddMemoryFont(ptr, data.Length);
                return fonts.Families.Length > before ? fonts.Families[fonts.Families.Length - 1] : null;
            }
            catch { return null; }
        }

        public static Font Body(float size, FontStyle style = FontStyle.Regular) => new(body, size, style);
        public static Font Deco(float size) => new(deco, size);

        public static void Text(Graphics g, string text, Font font, Color color, Rectangle rect,
            StringAlignment align = StringAlignment.Near, StringAlignment line = StringAlignment.Near)
        {
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using var brush = new SolidBrush(color);
            using var format = new StringFormat { Alignment = align, LineAlignment = line, Trimming = StringTrimming.EllipsisWord };
            g.DrawString(text, font, brush, rect, format);
        }
    }

    // Текст, нарисованный через GDI+ — обычные Label не видят шрифты, загруженные из памяти
    class TextLabel : Control
    {
        public StringAlignment Align = StringAlignment.Near;

        public TextLabel()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            BackColor = Color.Transparent;
        }

        protected override void OnTextChanged(EventArgs e) { base.OnTextChanged(e); Invalidate(); }

        protected override void OnPaint(PaintEventArgs e) =>
            Theme.Text(e.Graphics, Text, Font, ForeColor, ClientRectangle, Align);
    }

    // Панель-пергамент с тёмной рамкой и «гвоздиками» по углам, как у окон игры
    class ParchmentPanel : Panel
    {
        public ParchmentPanel()
        {
            DoubleBuffered = true;
            BackColor = Theme.Parchment;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(Theme.Ink, 2);
            g.DrawRectangle(pen, 1, 1, Width - 3, Height - 3);
            using var brush = new SolidBrush(Theme.Ink);
            foreach (var (x, y) in new[] { (0, 0), (Width - 1, 0), (0, Height - 1), (Width - 1, Height - 1) })
                g.FillEllipse(brush, x - 5, y - 5, 10, 10);
        }
    }

    class GameButton : Control
    {
        public Color Fill = Theme.Gold, Hover = Theme.GoldHover;
        bool hover, down;

        public GameButton()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
            Cursor = Cursors.Hand;
            ForeColor = Theme.Ink;
        }

        protected override void OnMouseEnter(EventArgs e) { hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { hover = down = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { down = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { down = false; Invalidate(); base.OnMouseUp(e); }
        protected override void OnTextChanged(EventArgs e) { base.OnTextChanged(e); Invalidate(); }
        protected override void OnEnabledChanged(EventArgs e) { base.OnEnabledChanged(e); Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var fill = !Enabled ? Color.FromArgb(120, Fill) : hover ? Hover : Fill;
            g.Clear(Parent?.BackColor ?? Theme.Background);
            var r = new Rectangle(0, 0, Width - 1, Height - 1);
            if (down) r.Offset(0, 1);
            using (var b = new SolidBrush(fill)) g.FillRectangle(b, r);
            using (var p = new Pen(Theme.Ink, 2)) g.DrawRectangle(p, r.X + 1, r.Y + 1, r.Width - 2, r.Height - 2);
            var color = Enabled ? ForeColor : Color.FromArgb(140, ForeColor);
            Theme.Text(g, Text, Font, color, r, StringAlignment.Center, StringAlignment.Center);
        }
    }

    // Тонкая золотая полоска прогресса
    class GoldProgress : Control
    {
        int value;
        public int Value { get => value; set { this.value = Math.Max(0, Math.Min(100, value)); Invalidate(); } }

        public GoldProgress() => SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Theme.ParchmentLight);
            using var b = new SolidBrush(Theme.Gold);
            e.Graphics.FillRectangle(b, 0, 0, Width * value / 100, Height);
            using var p = new Pen(Theme.Ink);
            e.Graphics.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
        }
    }
}
