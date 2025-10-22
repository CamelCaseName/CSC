using CSC.Nodestuff;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Text;

namespace CSC.Glue
{
    public static class Extensions
    {

        public static string Enumize(this string s)
        {
            return s.Replace(" ", "").Replace("'", "").Replace("-", "").Replace("(", "").Replace(")", "");
        }

        public static Control? FindFocusedControl(this Control control)
        {

            if (control.Focused)
            {
                return control;
            }

            foreach (Control item in control.Controls)
            {
                var focus = item.FindFocusedControl();
                if (focus != null)
                {
                    return focus;
                }
            }
            return null;
        }
        public static void ToLowQuality(this Graphics graphics)
        {
            graphics.InterpolationMode = InterpolationMode.Low;
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.CompositingMode = CompositingMode.SourceOver;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
            graphics.PixelOffsetMode = PixelOffsetMode.None;
        }

        public static void ExtendToIndex<T>(this List<T> list, int newLastIndex, T item)
        {
            for (int i = list.Count; i <= newLastIndex; i++)
            {
                list.Add(item);
            }
        }

        public static string ListRepresentation(this List<string> list)
        {
            StringBuilder sb = new();
            foreach (string item in list)
            {
                sb.Append(item);
                sb.Append(", ");
            }
            return sb.ToString();
        }

        public static Color Times(this Color color, float f)
        {
            var r = color.R;
            var g = color.G;
            var b = color.B;
            return Color.FromArgb(255, (int)(r * f), (int)(g * f), (int)(b * f));
        }

        public static void AddComboBoxHandler(this ComboBox box, Node node, NodeStore nodes, EventHandler handler)
        {
            box.SelectedIndexChanged += handler;
            box.SelectedIndexChanged += (_, _) =>
            {
                NodeLinker.UpdateLinks(node, node.FileName, nodes);
                Main.RedrawGraph();
            };
        }
    }
}
