using CSC.StoryItems;
using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Net.Http.Headers;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CSC
{
    public partial class Main : Form
    {
        private static readonly System.Buffers.SearchValues<char> numbers = System.Buffers.SearchValues.Create("0123456789");
        private const int NodeCount = 100;
        //we have to do something to keep the texture size smaller than it is rn

        int runningTotal = 0;
        public bool MovingChild = false;
        private Size OffsetFromDragClick = Size.Empty;
        private Node movedNode;
        private int counter = 0;
        private PointF start;
        private PointF end;
        private readonly Pen linePen;
        private readonly Pen highlightPen;
        private readonly List<Node> visited = [];
        private readonly int scaleX = 120;
        private readonly int scaleY = 40;
        private float Scaling = 0.3f;
        private float StartPanOffsetX = 0f;
        private float StartPanOffsetY = 0f;
        private float OldMouseMovingPosX;
        private float OldMouseMovingPosY;
        private float AfterZoomMouseX;
        private float AfterZoomMouseY;
        private float BeforeZoomMouseX;
        private float BeforeZoomMouseY;
        private float OffsetX;
        private float OffsetY;
        private readonly int NodeSizeX = 70;
        private readonly int NodeSizeY = 30;
        private bool CurrentlyInPan = false;
        private readonly List<int> layerYperX = [];

        private readonly NodeStore nodes = new();

        private Node lastNode = Node.NullNode;
        private Node highlightNode = Node.NullNode;
        private bool IsShiftPressed;
        private bool IsCtrlPressed;
        private Cursor priorCursor = Cursors.Default;
        private readonly Brush nodeBrush;
        private readonly Brush HighlightNodeBrush;
        private readonly Brush FontBrush;

        public Main()
        {
            InitializeComponent();
            Application.AddMessageFilter(new MouseMessageFilter());
            MouseMessageFilter.MouseMove += HandleMouseEvents;
            linePen = new Pen(Brushes.Black, 2)
            {
                EndCap = LineCap.Triangle,
                StartCap = LineCap.Round
            };
            highlightPen = new Pen(Brushes.Green, 3)
            {
                EndCap = LineCap.Triangle,
                StartCap = LineCap.Round
            };
            nodeBrush = new SolidBrush(Color.FromArgb(255, 100, 100, 100));
            HighlightNodeBrush = Brushes.DarkCyan;
            FontBrush = Brushes.White;

        }

        private void Start_Click(object sender, EventArgs e)
        {
            Node node = CreateNode(typeof(Criterion));

            nodes.Add(node);

            lastNode = node;
            for (int i = 0; i < NodeCount; i++)
            {
                switch (Random.Shared.Next(20))
                {
                    case 9:
                    case 8:
                    case 7:
                    case 6:
                    case 5:
                    case 4:
                    case 19:
                    case 18:
                    case 17:
                    case 16:
                    case 15:
                    case 14:
                    case 13:
                    case 12:
                    case 11:
                    {
                        node = CreateNode(typeof(Response));
                        nodes.AddChild(lastNode, node);
                        break;
                    }
                    case 3:
                    {
                        node = CreateNode(typeof(Criterion));
                        nodes.Add(node);
                        break;
                    }
                    case 10:
                    case 2:
                    case 1:
                    case 0:
                    {
                        node = CreateNode(typeof(Dialogue));
                        nodes.AddParent(lastNode, node);
                        break;
                    }
                    default:
                        break;
                }

                if (Random.Shared.Next(5) < 3)
                {
                    lastNode = node;
                }
            }

            //position according to the child system
            SetupStartPositions();

            Invalidate();
        }

        private void SetupStartPositions()
        {
            layerYperX.Clear();
            layerYperX.Add(0);
            visited.Clear();

            int sideLengthY = (int)(Math.Sqrt(nodes.KeyNodes().Count) + 0.5) * 4;
            int zeroColoumn = 0;

            foreach (var key in nodes.KeyNodes())
            {
                Family family = nodes[key];
                if (family.Parents.Count == 0)
                {
                    //we have a root start node with no parents, start child search and layout from here
                    key.Position = new PointF((zeroColoumn + 1) * scaleX, layerYperX[zeroColoumn] * scaleY);
                    visited.Add(key);

                    if (family.Childs.Count == 0)
                    {
                        layerYperX[zeroColoumn]++;
                        if (layerYperX[zeroColoumn] > sideLengthY)
                        {
                            zeroColoumn = layerYperX.Count;
                            layerYperX.Add(0);
                            layerYperX.Add(0);
                        }
                        continue;
                    }
                    if (layerYperX.Count <= 1)
                    {
                        layerYperX.Add(layerYperX[zeroColoumn]);
                    }
                    else
                    {
                        layerYperX[zeroColoumn + 1] = Math.Max(layerYperX[zeroColoumn], layerYperX[zeroColoumn + 1]);
                    }
                    //Debug.WriteLine("on node " + key.control.Text + " we went to its children at y level " + layerYperX[0]);
                    DoAllChilds(zeroColoumn, zeroColoumn + 1, family.Childs);
                    layerYperX[zeroColoumn]++;

                }
            }
        }

        private void DoAllChilds(int zeroRow, int layerX, List<Node> childs)
        {
            foreach (var currentChild in childs)
            {
                if (visited.Contains(currentChild))
                {
                    continue;
                }

                currentChild.Position = new PointF((layerX + 1) * scaleX, layerYperX[layerX] * scaleY);
                //Debug.WriteLine("on node " + currentChild.control.Text + " we arrived at y level" + layerYperX[layerX]);
                visited.Add(currentChild);

                var newChilds = nodes.Childs(currentChild);
                if (newChilds.Count == 0)
                {
                    layerYperX[layerX]++;
                    continue;
                }
                else
                {
                    if (layerYperX.Count <= layerX + 1)
                    {
                        layerYperX.Add(layerYperX[layerX]);
                    }
                    else
                    {
                        layerYperX[layerX + 1] = Math.Max(layerYperX[layerX], layerYperX[layerX + 1]);
                    }
                    DoAllChilds(zeroRow, layerX + 1, newChilds);
                    layerYperX[layerX]++;
                    layerYperX[zeroRow] = Math.Max(layerYperX[zeroRow], layerYperX[layerX]);
                }
            }
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

            //Debug.WriteLine($"spawned number {counter} at {x}|{y}");

            var node = new Node(counter.ToString(), NodeType.Null, "blabla", item)
            {
                Position = new PointF(x, y),
                Size = new SizeF(NodeSizeX, NodeSizeY),
                ID = item.GetType().Name + counter
            };

            counter++;
            return node;
        }

        private void GetStartingPos(out int x, out int y)
        {
            int xstep = 100;
            int ystep = 50;
            //~sidelength of the most square layout we can achieve witrh the number of nodes we have
            int sideLength = (int)(Math.Sqrt(NodeCount) + 0.5);
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

        private void Main_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.ToLowQuality();

            //update canvas transforms
            g.TranslateTransform(-OffsetX * Scaling, -OffsetY * Scaling);
            g.ScaleTransform(Scaling, Scaling);
            RectangleF adjustedVisibleClipBounds = new(OffsetX - NodeSizeX, OffsetY - NodeSizeY, g.VisibleClipBounds.Width + NodeSizeX, g.VisibleClipBounds.Height + NodeSizeY);

            //int c = 0;
            foreach (var node in NodePositionSorting.Singleton[adjustedVisibleClipBounds])
            {
                //c++;
                DrawNode(e, node, nodeBrush);
            }

            foreach (var node in nodes.KeyNodes())
            {
                var list = nodes.Childs(node);
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        DrawEdge(e, node, item, linePen);
                    }
                }
            }

            if (highlightNode != Node.NullNode)
            {
                var family = nodes[highlightNode];
                if (family.Childs.Count > 0)
                {
                    foreach (var item in family.Childs)
                    {
                        DrawEdge(e, highlightNode, item, highlightPen);
                    }
                }
                if (family.Parents.Count > 0)
                {
                    foreach (var item in family.Parents)
                    {
                        DrawEdge(e, item, highlightNode, highlightPen);
                    }
                }
                DrawNode(e, highlightNode, HighlightNodeBrush);
            }
        }

        private void DrawNode(PaintEventArgs e, Node node, Brush brush)
        {
            e.Graphics.FillEllipse(brush, new RectangleF(node.Position, node.Size));
            if (Scaling > 0.2f)
            {
                e.Graphics.DrawString(node.ID[..3] + ".." + node.ID[node.ID.AsSpan().IndexOfAny(numbers)..], DefaultFont, FontBrush, new PointF(node.Position.X + 5, node.Position.Y + 7));
            }
        }

        private void DrawEdge(PaintEventArgs e, Node parent, Node child, Pen pen)
        {
            start = parent.Position + new SizeF(parent.Size.Width, parent.Size.Height / 2);
            end = child.Position + new SizeF(0, child.Size.Height / 2);

            PointF controlStart;
            PointF controlEnd;
            float controlEndY, controlStartY;

            float distanceX = MathF.Abs(end.X - start.X);
            if (start.X < end.X)
            {
                controlStart = new PointF((distanceX / 2) + start.X, start.Y);
                controlEnd = new PointF(((end.X - start.X) / 2) + start.X, end.Y);
            }
            else
            {
                float distanceY = MathF.Abs(end.Y - start.Y);
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
                controlStart = new PointF((start.X + distanceX / 2), controlStartY);
                controlEnd = new PointF((end.X - distanceX / 2), controlEndY);
            }

            e.Graphics.DrawBezier(pen, start, controlStart, controlEnd, end);
            //e.Graphics.DrawEllipse(Pens.Green, new Rectangle(controlStart, new Size(4, 4)));
            //e.Graphics.DrawEllipse(Pens.Red, new Rectangle(controlEnd, new Size(4, 4)));
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            nodes.Clear();
            visited.Clear();
            Invalidate();
            counter = 0;
        }

        public void HandleKeyBoard(object? sender, KeyEventArgs e)
        {
            //get the shift key state so we can determine later if we want to redraw the tree on node selection or not
            IsShiftPressed = e.KeyData == (Keys.ShiftKey | Keys.Shift);
            IsCtrlPressed = e.KeyData == (Keys.Control | Keys.ControlKey);
            if (!IsCtrlPressed)
            {
                movedNode = Node.NullNode;
                MovingChild = false;
            }
        }

        public void HandleMouseEvents(object? sender, MouseEventArgs e)
        {
            var pos = Main.ActiveForm!.PointToClient(Cursor.Position);
            ScreenToGraph(pos.X, pos.Y, out float ScreenPosX, out float ScreenPosY);
            var ScreenPos = new Point((int)ScreenPosX, (int)ScreenPosY);
            //set old position for next frame/call
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (e.Clicks > 1)
                    {
                        Details.Visible = !Details.Visible;
                    }
                    else
                    {
                        Node clickedNode = GetNodeAtPoint(ScreenPos);

                        if (clickedNode == Node.NullNode)
                        {
                            return;
                        }

                        if (IsCtrlPressed)
                        {
                            movedNode = clickedNode;
                            MovingChild = !MovingChild;
                            if (MovingChild)
                            {
                                OffsetFromDragClick = new Size((int)(movedNode.Position.X - ScreenPosX), (int)(movedNode.Position.Y - ScreenPosY));
                            }
                        }
                        else
                        {
                            Details.SelectedObject = clickedNode.Data;
                        }
                    }
                    break;
                case MouseButtons.None:
                {
                    EndPan();
                    var oldHighlight = highlightNode;
                    highlightNode = GetNodeAtPoint(ScreenPos);
                    if (highlightNode != oldHighlight)
                    {
                        Invalidate();
                    }
                }
                break;
                case MouseButtons.Right:
                    break;
                case MouseButtons.Middle:
                    UpdatePan(pos);
                    break;
                case MouseButtons.XButton1:
                    break;
                case MouseButtons.XButton2:
                    break;
                default:
                {
                    EndPan();
                    var oldHighlight = highlightNode;
                    highlightNode = GetNodeAtPoint(ScreenPos);
                    if (highlightNode != oldHighlight)
                    {
                        Invalidate();
                    }
                }
                break;
            }

            if (MovingChild && movedNode != Node.NullNode)
            {
                movedNode.Position = ScreenPos + OffsetFromDragClick;
                Invalidate();
            }


            //everything else, scrolling for example
            if (e.Delta != 0)
            {
                UpdateScaling(e);
                //redraw
                Invalidate();
            }

            //save mouse pos for next frame
            ScreenToGraph(pos.X, pos.Y, out OldMouseMovingPosX, out OldMouseMovingPosY);
        }

        private static Node GetNodeAtPoint(Point mouseGraphLocation)
        {
            foreach (var key in NodePositionSorting.Singleton[mouseGraphLocation])
            {
                if (new RectangleF(key.Position, key.Size).Contains(mouseGraphLocation))
                {
                    return key;
                }
            }
            return Node.NullNode;
        }

        private void EndPan()
        {
            //end of pan
            if (CurrentlyInPan)
            {
                CurrentlyInPan = false;
                Cursor = priorCursor;
            }
        }

        private void SetPanOffset(Point location)
        {
            StartPanOffsetX = location.X;
            StartPanOffsetY = location.Y;
        }

        private void UpdatePan(Point mouseLocation)
        {
            //start of pan
            if (!CurrentlyInPan)
            {
                CurrentlyInPan = true;
                //get current position in screen coordinates when we start to pan
                SetPanOffset(mouseLocation);
                priorCursor = Cursor;
                Cursor = Cursors.Cross;
            }
            //in pan
            else if (CurrentlyInPan)
            {
                UpdatePanOffset(mouseLocation);
                //redraw
                Invalidate();
            }
        }

        private void UpdatePanOffset(Point location)
        {
            //better scaling thanks to the one lone coder
            //https://www.youtube.com/watch?v=ZQ8qtAizis4
            //update opffset by the difference in screen coordinates we travelled so far
            OffsetX -= (location.X - StartPanOffsetX) / Scaling;
            OffsetY -= (location.Y - StartPanOffsetY) / Scaling;

            //replace old start by new start coordinates so we only look at one interval,
            //and do not accumulate the change in position
            StartPanOffsetX = location.X;
            StartPanOffsetY = location.Y;
        }

        private void UpdateScaling(MouseEventArgs e)
        {
            //get last mouse position in world space before the zoom so we can
            //offset back by the distance in world space we got shifted by zooming
            ScreenToGraph(e.X, e.Y, out BeforeZoomMouseX, out BeforeZoomMouseY);

            //WHEEL_DELTA = 120, as per windows documentation
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.mouseeventargs.delta?view=windowsdesktop-6.0
            if (e.Delta > 0)
            {
                Scaling *= 1.2f;
            }
            else if (e.Delta < 0)
            {
                Scaling *= 0.8f;
            }

            //capture mouse coordinates in world space again so we can calculate the offset cause by zooming and compensate
            ScreenToGraph(e.X, e.Y, out AfterZoomMouseX, out AfterZoomMouseY);

            //update pan offset by the distance caused by zooming
            OffsetX += BeforeZoomMouseX - AfterZoomMouseX;
            OffsetY += BeforeZoomMouseY - AfterZoomMouseY;
        }

        public void ScreenToGraph(float screenX, float screenY, out float graphX, out float graphY)
        {
            graphX = screenX / Scaling + OffsetX;
            graphY = screenY / Scaling + OffsetY;
        }
    }
}
