using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Aplikasi_Manajemen_Sampah
{
    public class BorderedPanel : Panel
    {
        public Color FillColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color BorderColor { get; set; } = Color.White;
        public int BorderRadius { get; set; } = 20;

        public BorderedPanel()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                          ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            this.Padding = new Padding(15, 10, 15, 10);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int radius = BorderRadius;
            Rectangle rect = this.ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;

            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();

            using (SolidBrush brush = new SolidBrush(FillColor))
            {
                e.Graphics.FillPath(brush, path);
            }

            using (Pen pen = new Pen(BorderColor, 1.5f))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }
    }
}