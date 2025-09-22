using CSC.Nodestuff;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace CSC
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
