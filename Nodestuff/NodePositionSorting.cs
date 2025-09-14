namespace CSC.Nodestuff
{
    public class NodePositionSorting
    {
        private const int GridSize = 1000;
        //4 lists for each quadrant so we can deal with negative coordinates
        private static readonly List<List<List<Node>>>[] Sorting = [[], [], [], []];

        public static NodePositionSorting Singleton = new();

        public List<Node> this[PointF position]
        {
            get
            {
                int scaledX = (int)(position.X / GridSize);
                int scaledY = (int)(position.Y / GridSize);

                TryFillListToSize(scaledX, scaledY);

                return Sorting[GetQuadrant(scaledX, scaledY)][Math.Abs(scaledX)][Math.Abs(scaledY)];
            }
        }

        public List<Node> this[RectangleF area]
        {
            get
            {
                List<Node> list = [];
                int scaledXLeft = (int)(area.Left / GridSize);
                int scaledYTop = (int)(area.Top / GridSize);
                int scaledXRight = (int)(area.Right / GridSize);
                int scaledYBottom = (int)(area.Bottom / GridSize);

                TryFillListToSize(scaledXRight, scaledYBottom);
                TryFillListToSize(scaledXRight, scaledYTop);
                TryFillListToSize(scaledXLeft, scaledYBottom);
                TryFillListToSize(scaledXLeft, scaledYTop);

                for (int x = scaledXLeft; x <= scaledXRight; x++)
                {
                    for (int y = scaledYTop; y <= scaledYBottom; y++)
                    {
                        //int quadrant = GetQuadrant(x, y);
                        //Debug.WriteLine(quadrant + " x -> " + Sorting[quadrant].Count + "[" + Math.Abs(x) + "] | " + Math.Abs(y));
                        //Debug.WriteLine(quadrant + " y -> " + Sorting[quadrant][Math.Abs(x)].Count + "[" + Math.Abs(y) + "]");
                        list.AddRange(Sorting[GetQuadrant(x, y)][Math.Abs(x)][Math.Abs(y)]);
                    }
                }

                return list;
            }
        }

        private static int GetQuadrant(int x, int y)
        {
            if (x < 0)
            {
                if (y < 0)
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (y < 0)
                {
                    return 3;
                }
                else
                {
                    return 0;
                }
            }
        }

        private static void TryFillListToSize(int scaledXRight, int scaledYBottom)
        {
            int quadrant = GetQuadrant(scaledXRight, scaledYBottom);

            scaledXRight = Math.Abs(scaledXRight) + 1;
            scaledYBottom = Math.Abs(scaledYBottom) + 1;

            if (Sorting[quadrant].Count <= scaledXRight + 2)
            {
                for (int i = 0; i <= scaledXRight + 1; i++)
                {
                    Sorting[quadrant].Add([]);
                }
            }

            for (int x = 0; x <= scaledXRight + 1; x++)
            {
                if (Sorting[quadrant][x].Count <= scaledYBottom + 2)
                {
                    for (int i = 0; i <= scaledYBottom + 1; i++)
                    {
                        Sorting[quadrant][x].Add([]);
                    }
                }
            }
        }

        public static void ClearNode(Node node)
        {
            int scaledXLeft = (int)(node.Position.X / GridSize);
            int scaledYTop = (int)(node.Position.Y / GridSize);
            int scaledXRight = (int)((node.Position.X + node.Size.Width) / GridSize);
            int scaledYBottom = (int)((node.Position.Y + node.Size.Height) / GridSize);

            TryFillListToSize(scaledXRight, scaledYBottom);

            for (int x = scaledXLeft; x <= scaledXRight; x++)
            {
                for (int y = scaledYTop; y <= scaledYBottom; y++)
                {
                    Sorting[GetQuadrant(x, y)][Math.Abs(x)][Math.Abs(y)].Remove(node);
                }
            }
        }

        public static void SetNode(Node node)
        {
            int scaledXLeft = (int)(node.Position.X / GridSize);
            int scaledYTop = (int)(node.Position.Y / GridSize);
            int scaledXRight = (int)((node.Position.X + node.Size.Width) / GridSize);
            int scaledYBottom = (int)((node.Position.Y + node.Size.Height) / GridSize);

            TryFillListToSize(scaledXRight, scaledYBottom);

            for (int x = scaledXLeft; x <= scaledXRight; x++)
            {
                for (int y = scaledYTop; y <= scaledYBottom; y++)
                {
                    Sorting[GetQuadrant(x, y)][Math.Abs(x)][Math.Abs(y)].Add(node);
                }
            }
        }

        internal static void Clear()
        {
            Sorting[0].Clear();
            Sorting[1].Clear();
            Sorting[2].Clear();
            Sorting[3].Clear();
        }
    }
}
