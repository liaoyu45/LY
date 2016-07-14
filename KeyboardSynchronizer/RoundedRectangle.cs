using System.Drawing;
using System.Drawing.Drawing2D;

namespace KeyboardSynchronizer {
    public static class Painter {
        public static GraphicsPath GetSmoothRectPath(Rectangle rect, int radius) {
            var arcRect = new Rectangle(rect.Location, new Size(radius, radius));
            var path = new GraphicsPath();
            path.AddArc(arcRect, 180, 90);
            arcRect.X = rect.Right - radius;
            path.AddArc(arcRect, 270, 90);
            path.AddLine(rect.Width, radius, rect.Width, rect.Height);
            path.AddLine(rect.Width, rect.Height, 0, rect.Height);
            path.CloseFigure();
            return path;
        }
    }
}
