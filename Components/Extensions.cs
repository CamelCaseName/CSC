using CSC.Nodestuff;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Text;

namespace CSC.Components
{
    public static class Extensions
    {
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
            box.SelectedIndexChanged += (_, _) => {
                NodeLinker.UpdateLinks(node, node.FileName, nodes); Main.RedrawGraph();
            };
        }
    }

    public static class EEnum
    {
        public static string StringParse<T>(string value) where T : struct, Enum
        {
            if (Enum.TryParse(value, out T result))
            {
                return result.ToString()!;
            }
            else
            {
                return string.Empty;
            }
        }

        public static T CastParse<T>(string value) where T : struct, Enum
        {
            if (Enum.TryParse(value, out T result))
            {
                return result;
            }
            else
            {
                return default;
            }
        }
        public static string StringParse<T>(int value) where T : struct, Enum
        {
            if (Enum.IsDefined(typeof(T), value))
            {
                return Enum.GetName(typeof(T), value) ?? string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        public static T CastParse<T>(int value) where T : struct, Enum
        {
            if (Enum.IsDefined(typeof(T), value))
            {
                var name = Enum.GetName(typeof(T), value) ?? string.Empty;
                if (!string.IsNullOrEmpty(name))
                {
                    return Enum.Parse<T>(name);
                }
                return default;
            }
            else
            {
                return default;
            }
        }
    }

    public class NodeParentComparer(NodeStore store) : IComparer<Node>
    {
        private readonly NodeStore Store = store;

        public int Compare(Node? x, Node? y)
        {
            if (x is null)
            {
                return -1;
            }
            else if (y is null)
            {
                return 1;
            }
            return Store.Childs(y)?.Count ?? 0 - Store.Childs(x)?.Count ?? 0;
        }
    }

    public class NodeChildComparer(NodeStore store) : IComparer<Node>
    {
        private readonly NodeStore Store = store;

        public int Compare(Node? x, Node? y)
        {
            if (x is null)
            {
                return -1;
            }
            else if (y is null)
            {
                return 1;
            }
            return Store.Parents(x)?.Count ?? 0 - Store.Parents(y)?.Count ?? 0;
        }
    }
}
