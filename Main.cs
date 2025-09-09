using CSC.StoryItems;
using System.Drawing.Drawing2D;

namespace CSC
{
    public partial class Main : Form
    {
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

        }

        private void Add_Click(object sender, EventArgs e)
        {
            Node node = CreateNode(typeof(Criterion));

            nodes.Add(node);

            lastNode = node;
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

            var control = new Button
            {
                Parent = this,
                Location = new Point(434, 147),
                Name = "button" + counter.ToString(),
                Size = new Size(75, 23),
                TabIndex = 1,
                Text = "button" + counter.ToString(),
                UseVisualStyleBackColor = true,
                Enabled = true
            };

            var node = new Node(control, counter.ToString(), NodeType.Criterion, "blabla", item);
            control.Click += (_, e) => { 
                Details.SelectedObject = item; 
                lastNode = node;
            };
            control.MouseMove += Main_MouseMove;


            counter++;
            return node;
        }

        private void AddChild_Click(object sender, EventArgs e)
        {
            Node node = CreateNode(typeof(Response));

            nodes.AddChild(lastNode, node);

            lastNode = node;
        }

        private void AddParent_Click(object sender, EventArgs e)
        {
            Node node = CreateNode(typeof(Dialogue));

            nodes.AddParent(lastNode, node);

            lastNode = node;
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
            //todo filter, duh
            //if (movedNode is not null)
            //{
            //    if (Childs.TryGetValue(movedNode, out var child))
            //    {
            //        DrawEdge(e, movedNode, child);
            //    }
            //    else
            //    {
            //        DrawEdge(e, movedNode, Parents[movedNode]);
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
