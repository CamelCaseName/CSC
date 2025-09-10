using CSC.StoryItems;
using System.Drawing.Drawing2D;
using System.Net.Http.Headers;

namespace CSC
{
    public partial class Main : Form
    {
        private const int NOdeCount = 1000;
        int runningTotal = 0;
        public bool MovingChild = false;
        private Size OffsetFromDragClick = Size.Empty;
        private Control? movedNode;
        private int counter = 0;
        private Point start;
        private Point end;
        private readonly Pen linePen;

        private readonly NodeStore nodes = new();

        private Node lastNode = Node.NullNode;

        public Main()
        {
            InitializeComponent();
            linePen = new Pen(Brushes.Black, 2)
            {
                EndCap = LineCap.Triangle,
                StartCap = LineCap.Round
            };

        }

        private void Start_Click(object sender, EventArgs e)
        {
            Node node = CreateNode(typeof(Criterion));

            nodes.Add(node);

            lastNode = node;
            for (int i = 0; i < NOdeCount; i++)
            {
                switch (Random.Shared.Next(3))
                {
                    case 2:
                    {
                        node = CreateNode(typeof(Response));
                        nodes.AddChild(lastNode, node);
                        break;
                    }
                    case 1:
                    {
                        node = CreateNode(typeof(Criterion));
                        nodes.Add(node);
                        break;
                    }
                    case 0:
                    {
                        node = CreateNode(typeof(Dialogue));
                        nodes.AddParent(lastNode, node);
                        break;
                    }
                    default:
                        break;
                }

                if (Random.Shared.Next(3) == 1)
                {
                    lastNode = node;
                }
            }

            Invalidate();
        }

        private void Add_Click(object sender, EventArgs e)
        {
            Node node = CreateNode(typeof(Criterion));

            nodes.Add(node);

            lastNode = node;

            Invalidate();
        }

        private Node CreateNode(Type type)
        {
            object item;
            if (type == typeof(Dialogue))
            {
                item = new Dialogue
                {
                    ID = counter
                };
            }
            else if (type == typeof(Response))
            {
                item = new Response
                {
                    Order = counter
                };
            }
            else
            {
                item = new Criterion
                {
                    Order = counter
                };
            }

            GetStartingPos(out int x, out int y);

            Console.WriteLine($"spawned number {counter} at {x}|{y}");
            var control = new Button
            {
                Parent = this,
                Location = new Point(x, y),
                Name = type.Name + counter.ToString(),
                Size = new Size(75, 23),
                TabIndex = 1,
                Text = type.Name + counter.ToString(),
                UseVisualStyleBackColor = true,
                Enabled = true,
                Font = new Font("Segoe UI", 6.75F, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var node = new Node(control, counter.ToString(), NodeType.Criterion, "blabla", item);
            control.Click += (_, e) =>
            {
                Details.SelectedObject = item;
                lastNode = node;
            };
            control.MouseMove += Main_MouseMove;

            counter++;
            return node;
        }

        private void GetStartingPos(out int x, out int y)
        {
            int xstep = 100;
            int ystep = 50;
            //~sidelength of the most square layout we can achieve witrh the number of nodes we have
            int sideLength = (int)(Math.Sqrt(NOdeCount) + 0.5);
            //modulo of running total with sidelength gives x coords, repeating after sidelength
            //offset by halfe sidelength to center x
            x = runningTotal % sideLength - sideLength / 2;
            x += 17;
            x *= xstep;
            //running total divided by sidelength gives y coords,
            //increments after runningtotal increments sidelength times
            //offset by halfe sidelength to center y
            y = runningTotal / sideLength - sideLength / 2;
            y += 17;
            y *= ystep;
            //set position
            //increase running total
            runningTotal++;
        }

        private void AddChild_Click(object sender, EventArgs e)
        {
            Node node = CreateNode(typeof(Response));

            nodes.AddChild(lastNode, node);

            lastNode = node;

            Invalidate();
        }

        private void AddParent_Click(object sender, EventArgs e)
        {
            Node node = CreateNode(typeof(Dialogue));

            nodes.AddParent(lastNode, node);

            lastNode = node;

            Invalidate();
        }

        private void Main_Click(object sender, EventArgs e)
        {
            var pos = PointToClient(Cursor.Position);
            movedNode = GetChildAtPoint(pos + new Size(4, 4));
            movedNode ??= GetChildAtPoint(pos + new Size(4, -4));
            movedNode ??= GetChildAtPoint(pos + new Size(-4, 4));
            movedNode ??= GetChildAtPoint(pos + new Size(-4, -4));
            if (movedNode is not null)
            {
                if (movedNode.GetType() == typeof(ToolStrip))
                {
                    return;
                }

                MovingChild = !MovingChild;
                if (MovingChild)
                {
                    OffsetFromDragClick = new Size(movedNode.Location.X - pos.X, movedNode.Location.Y - pos.Y);
                }
            }
        }

        private void Main_DoubleClick(object sender, EventArgs e)
        {
            Details.Visible = !Details.Visible;
        }

        private void Main_MouseMove(object? sender, MouseEventArgs e)
        {
            if (MovingChild && movedNode is not null)
            {
                movedNode.Location = e.Location + OffsetFromDragClick;
                Invalidate();
            }
        }

        private void Main_Paint(object sender, PaintEventArgs e)
        {
            //if (!MovingChild)
            //{
            //    foreach (var node in nodes.Keys())
            //    {
            //        var list = nodes.Childs(node);
            //        if (list.Count > 0)
            //        {
            //            foreach (var item in list)
            //            {
            //                DrawEdge(e, node.control, item.control);
            //            }
            //        }
            //    }
            //}
            //else if(movedNode is not null)
            //{
            //    var family = nodes[nodes[movedNode]];
            //    if(family.Childs.Count > 0)
            //    {
            //        foreach(var item in family.Childs)
            //        {
            //            DrawEdge(e, movedNode, item.control);
            //        }
            //    }
            //    if (family.Parents.Count > 0)
            //    {
            //        foreach (var item in family.Parents)
            //        {
            //            DrawEdge(e, item.control, movedNode);
            //        }
            //    }
            //}

            foreach (var node in nodes.Keys())
            {
                var list = nodes.Childs(node);
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        DrawEdge(e, node.control, item.control);
                    }
                }
            }
        }

        private void DrawEdge(PaintEventArgs e, Control parent, Control child)
        {
            start = parent.Location + new Size(parent.Size.Width, parent.Size.Height / 2);
            end = child.Location + new Size(0, child.Size.Height / 2);

            Point controlStart;
            Point controlEnd;
            int controlEndY, controlStartY;

            int distanceX = Math.Abs(end.X - start.X);
            if (start.X < end.X)
            {
                controlStart = new Point((distanceX / 2) + start.X, start.Y);
                controlEnd = new Point(((end.X - start.X) / 2) + start.X, end.Y);
            }
            else
            {
                int distanceY = Math.Abs(end.Y - start.Y);
                if (start.Y > end.Y)
                {
                    controlStartY = start.Y - distanceY / 2;
                    controlEndY = end.Y + distanceY / 2;
                }
                else
                {
                    controlStartY = start.Y + distanceY / 2;
                    controlEndY = end.Y - distanceY / 2;
                }
                controlStart = new Point((start.X + distanceX / 2), controlStartY);
                controlEnd = new Point((end.X - distanceX / 2), controlEndY);
            }

            e.Graphics.DrawBezier(linePen, start, controlStart, controlEnd, end);
            //e.Graphics.DrawEllipse(Pens.Green, new Rectangle(controlStart, new Size(4, 4)));
            //e.Graphics.DrawEllipse(Pens.Red, new Rectangle(controlEnd, new Size(4, 4)));
        }
    }
}
