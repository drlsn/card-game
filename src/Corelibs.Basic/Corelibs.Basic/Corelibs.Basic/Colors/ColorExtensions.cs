using System.Drawing;

namespace Corelibs.Basic.Colors
{
    public static class ColorExtensions
    {
        public static string ToHexString(this Color color) =>
            "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");

        public static string ToRgbaString(this Color color) =>
            $"rgba({color.R}, {color.G}, {color.B}, {color.A / 255})";

        public static Color ToColor(this int? argb) =>
            Color.FromArgb(argb.Value);
    }
}
