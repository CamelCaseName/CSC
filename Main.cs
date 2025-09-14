using CSC.Nodestuff;
using CSC.StoryItems;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace CSC
{
    public partial class Main : Form
    {
        public const int NodeSizeX = 200;
        public const int NodeSizeY = 50;
        private static readonly Dictionary<string, NodeStore> nodes = [];
        public bool MovingChild = false;
        private const int NodeCount = 100;
        public const string NoCharacter = "None";
        private readonly SolidBrush achievementNodeBrush;
        private readonly SolidBrush alternateTextNodeBrush;
        private readonly SolidBrush bgcNodeBrush;
        private readonly SolidBrush bgcResponseNodeBrush;
        private readonly SolidBrush characterGroupNodeBrush;
        private readonly SolidBrush clothingNodeBrush;
        private readonly SolidBrush criteriaGroupNodeBrush;
        private readonly SolidBrush criterionNodeBrush;
        private readonly SolidBrush cutsceneNodeBrush;
        private readonly SolidBrush defaultNodeBrush;
        private readonly SolidBrush dialogueNodeBrush;
        private readonly SolidBrush doorNodeBrush;
        private readonly SolidBrush eventNodeBrush;
        private readonly SolidBrush eventTriggerNodeBrush;
        private readonly SolidBrush HighlightNodeBrush;
        private readonly Pen highlightPen;
        private readonly SolidBrush inventoryNodeBrush;
        private readonly SolidBrush itemActionNodeBrush;
        private readonly SolidBrush itemGroupBehaviourNodeBrush;
        private readonly SolidBrush itemGroupInteractionNodeBrush;
        private readonly SolidBrush itemGroupNodeBrush;
        private readonly SolidBrush itemNodeBrush;
        private readonly List<int> layerYperX = [];
        private readonly Pen linePen;
        private readonly SolidBrush personalityNodeBrush;
        private readonly SolidBrush poseNodeBrush;
        private readonly SolidBrush propertyNodeBrush;
        private readonly SolidBrush questNodeBrush;
        private readonly SolidBrush responseNodeBrush;
        private readonly int scaleX = NodeSizeX + 40;
        private readonly int scaleY = NodeSizeY + 20;
        private readonly SolidBrush socialNodeBrush;
        private readonly SolidBrush stateNodeBrush;
        private readonly SolidBrush valueNodeBrush;
        private readonly List<Node> visited = [];
        private float AfterZoomMouseX;
        private float AfterZoomMouseY;
        private float BeforeZoomMouseX;
        private float BeforeZoomMouseY;
        private int counter = 0;
        private bool CurrentlyInPan = false;
        private PointF end;
        private Node highlightNode = Node.NullNode;
        private bool IsCtrlPressed;
        private bool IsShiftPressed;
        private Node lastNode = Node.NullNode;
        private Node movedNode;
        private Size OffsetFromDragClick = Size.Empty;
        private float OffsetX;
        private float OffsetY;
        private int runningTotal = 0;
        private float Scaling = 0.3f;
        private PointF start;
        private float StartPanOffsetX = 0f;
        private float StartPanOffsetY = 0f;
        private Font scaledFont = DefaultFont;
        RectangleF adjustedVisibleClipBounds = new();
        private RectangleF adjustedMouseClipBounds;

        public static string SelectedCharacter
        {
            get;


            private set;
        } = NoCharacter;

        public Main()
        {
            InitializeComponent();
            StoryTree.ExpandAll();
            Application.AddMessageFilter(new MouseMessageFilter());
            MouseMessageFilter.MouseMove += HandleMouseEvents;
            linePen = new Pen(Brushes.LightGray, 2)
            {
                EndCap = LineCap.Triangle,
                StartCap = LineCap.Round
            };
            highlightPen = new Pen(Brushes.Green, 3)
            {
                EndCap = LineCap.Triangle,
                StartCap = LineCap.Round
            };
            defaultNodeBrush = new SolidBrush(Color.FromArgb(255, 100, 100, 100));
            achievementNodeBrush = new SolidBrush(Color.FromArgb(255, 200, 10, 200));
            alternateTextNodeBrush = new SolidBrush(Color.FromArgb(255, 110, 120, 190));
            bgcNodeBrush = new SolidBrush(Color.FromArgb(255, 40, 190, 255));
            bgcResponseNodeBrush = new SolidBrush(Color.FromArgb(255, 150, 225, 255));
            characterGroupNodeBrush = new SolidBrush(Color.FromArgb(255, 190, 180, 130));
            clothingNodeBrush = new SolidBrush(Color.FromArgb(255, 115, 235, 30));
            criteriaGroupNodeBrush = new SolidBrush(Color.FromArgb(255, 150, 50, 50));
            criterionNodeBrush = new SolidBrush(Color.FromArgb(255, 220, 20, 20));
            cutsceneNodeBrush = new SolidBrush(Color.FromArgb(255, 235, 30, 160));
            dialogueNodeBrush = new SolidBrush(Color.FromArgb(255, 45, 60, 185));
            doorNodeBrush = new SolidBrush(Color.FromArgb(255, 200, 225, 65));
            eventNodeBrush = new SolidBrush(Color.FromArgb(255, 50, 150, 50));
            eventTriggerNodeBrush = new SolidBrush(Color.FromArgb(255, 20, 220, 20));
            inventoryNodeBrush = new SolidBrush(Color.FromArgb(255, 65, 225, 185));
            itemActionNodeBrush = new SolidBrush(Color.FromArgb(255, 85, 195, 195));
            itemGroupBehaviourNodeBrush = new SolidBrush(Color.FromArgb(255, 160, 200, 195));
            itemGroupInteractionNodeBrush = new SolidBrush(Color.FromArgb(255, 95, 120, 115));
            itemGroupNodeBrush = new SolidBrush(Color.FromArgb(255, 45, 190, 165));
            itemNodeBrush = new SolidBrush(Color.FromArgb(255, 45, 255, 255));
            personalityNodeBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 90));
            poseNodeBrush = new SolidBrush(Color.FromArgb(255, 255, 210, 90));
            propertyNodeBrush = new SolidBrush(Color.FromArgb(255, 255, 90, 150));
            questNodeBrush = new SolidBrush(Color.FromArgb(255, 150, 210, 155));
            responseNodeBrush = new SolidBrush(Color.FromArgb(255, 55, 155, 225));
            socialNodeBrush = new SolidBrush(Color.FromArgb(255, 255, 160, 90));
            stateNodeBrush = new SolidBrush(Color.FromArgb(255, 40, 190, 50));
            valueNodeBrush = new SolidBrush(Color.FromArgb(255, 40, 0, 190));
            HighlightNodeBrush = new SolidBrush(Color.DarkCyan);

            nodes.Add(NoCharacter, new());
        }

        public void HandleKeyBoard(object? sender, KeyEventArgs e)
        {
            //get the shift key state so we can determine later if we want to redraw the tree on node selection or not
            IsShiftPressed = e.KeyData == (Keys.ShiftKey | Keys.Shift);
            IsCtrlPressed = e.KeyData == (Keys.Control | Keys.ControlKey);
        }

        public void HandleMouseEvents(object? sender, MouseEventArgs e)
        {
            if (Main.ActiveForm is null)
            {
                return;
            }
            var pos = Graph.PointToClient(Cursor.Position);
            ScreenToGraph(pos.X, pos.Y, out float ScreenPosX, out float ScreenPosY);
            var ScreenPos = new Point((int)ScreenPosX, (int)ScreenPosY);
            //set old position for next frame/call
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (e.Clicks > 1)
                    {
                        //todo do whatever we do on double click
                    }
                    else
                    {
                        UpdateClickedNode(ScreenPosX, ScreenPosY, ScreenPos);
                    }
                    break;
                case MouseButtons.None:
                {
                    MovingChild = false;
                    EndPan();
                    UpdateHighlightNode(ScreenPos);
                }
                break;
                case MouseButtons.Right:
                {
                    UpdateClickedNode(ScreenPosX, ScreenPosY, ScreenPos);
                }
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
                    MovingChild = false;
                    EndPan();
                    UpdateHighlightNode(ScreenPos);
                }
                break;
            }

            if (MovingChild && movedNode != Node.NullNode)
            {
                movedNode.Position = ScreenPos + OffsetFromDragClick;
                Cursor = Cursors.SizeAll;
                Graph.Invalidate();
            }
            else if (highlightNode != Node.NullNode)
            {
                Cursor = Cursors.Hand;
            }
            else if (CurrentlyInPan)
            {
                Cursor = Cursors.Cross;
            }
            else
            {
                Cursor = Cursors.Arrow;
            }

            //everything else, scrolling for example
            if (e.Delta != 0)
            {
                //todo we need a limit here so we dont scroll out too far and make the texture hugeeee
                UpdateScaling(e);
                //redraw
                Graph.Invalidate();
            }
        }

        public void ScreenToGraph(float screenX, float screenY, out float graphX, out float graphY)
        {
            graphX = screenX / Scaling + OffsetX;
            graphY = screenY / Scaling + OffsetY;
        }

        private Node GetNodeAtPoint(Point mouseGraphLocation)
        {
            if (adjustedMouseClipBounds.Contains(mouseGraphLocation))
            {
                foreach (var key in nodes[SelectedCharacter].Positions[mouseGraphLocation])
                {
                    if (key.Rectangle.Contains(mouseGraphLocation))
                    {
                        return key;
                    }
                }
            }
            return Node.NullNode;
        }

        private void Add_Click(object sender, EventArgs e)
        {
            Node node = CreateNode(typeof(Criterion));

            nodes[NoCharacter].Add(node);

            lastNode = node;

            Graph.Invalidate();
        }

        private void AddChild_Click(object sender, EventArgs e)
        {
            Node node = CreateNode(typeof(Response));

            nodes[NoCharacter].AddChild(lastNode, node);

            lastNode = node;

            Graph.Invalidate();
        }

        private void AddParent_Click(object sender, EventArgs e)
        {
            Node node = CreateNode(typeof(Dialogue));

            nodes[NoCharacter].AddParent(lastNode, node);

            lastNode = node;

            Graph.Invalidate();
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

            var node = new Node(counter.ToString(), NodeType.Null, "blabla", item, nodes[NoCharacter].Positions)
            {
                Position = new PointF(x, y),
                Size = new SizeF(NodeSizeX, NodeSizeY),
                ID = item.GetType().Name + counter
            };

            counter++;
            return node;
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

                var newChilds = nodes[SelectedCharacter].Childs(currentChild);
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

        private void DrawNode(PaintEventArgs e, Node node, SolidBrush brush)
        {
            e.Graphics.FillPath(brush, RoundedRect(node.Rectangle, 10f));
            if (Scaling > 0.2f)
            {
                int length = 32;
                var scaledRect = GetScaledRect(e.Graphics, node.RectangleNonF, Scaling);
                while (TextRenderer.MeasureText(node.Text[..Math.Min(node.Text.Length, length)], scaledFont).Width > scaledRect.Size.Width)
                {
                    length--;
                }

                Color textColor = Color.White;
                if ((brush.Color.R * 0.299 + brush.Color.G * 0.587 + brush.Color.B * 0.114) > 186)
                {
                    textColor = Color.Black;
                }

                TextRenderer.DrawText(e.Graphics,
                                      node.Text[..Math.Min(node.Text.Length, length)],
                                      scaledFont,
                                      scaledRect,
                                      textColor,
                                      TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        //see https://stackoverflow.com/questions/33853434/how-to-draw-a-rounded-rectangle-in-c-sharp
        private static GraphicsPath RoundedRect(RectangleF bounds, float radius)
        {
            float diameter = radius * 2;
            SizeF size = new(diameter, diameter);
            RectangleF arc = new(bounds.Location, size);
            GraphicsPath path = new();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private void EndPan()
        {
            //end of pan
            if (CurrentlyInPan)
            {
                CurrentlyInPan = false;
            }
        }

        private SolidBrush GetNodeColor(NodeType type)
        {
            return type switch
            {
                NodeType.Null => defaultNodeBrush,
                NodeType.CharacterGroup => characterGroupNodeBrush,
                NodeType.Criterion => criterionNodeBrush,
                NodeType.ItemAction => itemActionNodeBrush,
                NodeType.ItemGroupBehaviour => itemGroupBehaviourNodeBrush,
                NodeType.ItemGroupInteraction => itemGroupInteractionNodeBrush,
                NodeType.Pose => poseNodeBrush,
                NodeType.Achievement => achievementNodeBrush,
                NodeType.BGC => bgcNodeBrush,
                NodeType.BGCResponse => bgcResponseNodeBrush,
                NodeType.Clothing => clothingNodeBrush,
                NodeType.CriteriaGroup => criteriaGroupNodeBrush,
                NodeType.Cutscene => cutsceneNodeBrush,
                NodeType.Dialogue => dialogueNodeBrush,
                NodeType.AlternateText => alternateTextNodeBrush,
                NodeType.Door => doorNodeBrush,
                NodeType.Event => eventNodeBrush,
                NodeType.EventTrigger => eventTriggerNodeBrush,
                NodeType.Inventory => inventoryNodeBrush,
                NodeType.Item => itemNodeBrush,
                NodeType.ItemGroup => itemGroupNodeBrush,
                NodeType.Personality => personalityNodeBrush,
                NodeType.Property => propertyNodeBrush,
                NodeType.Quest => questNodeBrush,
                NodeType.Response => responseNodeBrush,
                NodeType.Social => socialNodeBrush,
                NodeType.State => stateNodeBrush,
                NodeType.Value => valueNodeBrush,
                _ => defaultNodeBrush,
            };
        }

        private void GetStartingPos(out int x, out int y)
        {
            int xstep = 100;
            int ystep = 50;
            //~sidelength of the most square layout we can achieve witrh the number of Main.nodes we have
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

        private void Main_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.ToLowQuality();

            //update canvas transforms
            g.TranslateTransform(-OffsetX * Scaling, -OffsetY * Scaling);
            g.ScaleTransform(Scaling, Scaling);
            adjustedVisibleClipBounds = new(OffsetX - NodeSizeX,
                                            OffsetY - NodeSizeY,
                                            g.VisibleClipBounds.Width + NodeSizeX,
                                            g.VisibleClipBounds.Height + NodeSizeY);
            adjustedMouseClipBounds = new(OffsetX,
                                            OffsetY,
                                            g.VisibleClipBounds.Width,
                                            g.VisibleClipBounds.Height);

            if (Scaling < 0)
            {
                Debugger.Break();
            }
            scaledFont = GetScaledFont(g, DefaultFont, Scaling);

            //int c = 0;
            foreach (var node in nodes[SelectedCharacter].Positions[adjustedVisibleClipBounds])
            {
                //c++;
                DrawNode(e, node, GetNodeColor(node.Type));
            }
            //todo we need to cull here as well somehow
            foreach (var node in nodes[SelectedCharacter].Nodes)
            {
                var list = nodes[SelectedCharacter].Childs(node);
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
                var family = nodes[SelectedCharacter][highlightNode];
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

        private void OpenButton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();

            if (result != DialogResult.OK)
            {
                return;
            }

            string FilePath = dialog.FileName;

            if (File.Exists(FilePath))
            {
                string fileString = File.ReadAllText(FilePath);
                fileString = fileString.Replace('', ' ');

                NodeStore tempStore = new();
                //else create new
                try
                {
                    if (Path.GetExtension(FilePath) == ".story")
                    {
                        NodeLinker.DissectStory(JsonConvert.DeserializeObject<MainStory>(fileString) ?? new MainStory(), tempStore);
                    }
                    else
                    {
                        NodeLinker.DissectCharacter(JsonConvert.DeserializeObject<CharacterStory>(fileString) ?? new CharacterStory(), tempStore);
                    }
                }
                catch (JsonReaderException ex)
                {
                    Debug.WriteLine(ex.Message);
                    return;
                }
                catch (AggregateException ex)
                {
                    Debug.WriteLine(ex.Message);
                    return;
                }
                string FileName = Path.GetFileNameWithoutExtension(FilePath);

                //even link for single file, should be able to link most suff so it stays readable
                NodeLinker.Interlinknodes(tempStore);

                if (!nodes.TryAdd(FileName, tempStore))
                {
                    nodes[FileName] = tempStore;
                }
                SelectedCharacter = FileName;

                SetupStartPositions();

                Graph.Invalidate();

                StoryTree.Nodes[0].LastNode.Nodes.Add(new TreeNode(FileName));
                StoryTree.ExpandAll();
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            nodes.Clear();
            visited.Clear();
            Graph.Invalidate();
            counter = 0;
            SelectedCharacter = NoCharacter;
        }

        private void SetPanOffset(Point location)
        {
            StartPanOffsetX = location.X;
            StartPanOffsetY = location.Y;
        }

        private void SetupStartPositions()
        {
            layerYperX.Clear();
            layerYperX.Add(0);
            visited.Clear();

            //todo we need to investigate the tornadoization here

            int sideLengthY = (int)(Math.Sqrt(nodes[SelectedCharacter].Count) + 0.5);
            int zeroColoumn = 0;

            foreach (var key in nodes[SelectedCharacter].Nodes)
            {
                Family family = nodes[SelectedCharacter][key];
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

        private void Start_Click(object sender, EventArgs e)
        {
            Node node = CreateNode(typeof(Criterion));

            nodes[SelectedCharacter].Add(node);

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
                        nodes[SelectedCharacter].AddChild(lastNode, node);
                        break;
                    }
                    case 3:
                    {
                        node = CreateNode(typeof(Criterion));
                        nodes[SelectedCharacter].Add(node);
                        break;
                    }
                    case 10:
                    case 2:
                    case 1:
                    case 0:
                    {
                        node = CreateNode(typeof(Dialogue));
                        nodes[SelectedCharacter].AddParent(lastNode, node);
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

            Graph.Invalidate();
        }

        private void UpdateClickedNode(float ScreenPosX, float ScreenPosY, Point ScreenPos)
        {
            Node clickedNode = GetNodeAtPoint(ScreenPos);

            if (clickedNode == Node.NullNode)
            {
                return;
            }

            if (MouseButtons == MouseButtons.Right)
            {
                if (!MovingChild)
                {
                    movedNode = clickedNode;
                    MovingChild = true;
                    OffsetFromDragClick = new Size((int)(movedNode.Position.X - ScreenPosX), (int)(movedNode.Position.Y - ScreenPosY));
                }
            }
            else
            {
                MovingChild = false;
                SetSelectedObject(clickedNode);
                lastNode = clickedNode;
            }
        }

        private void SetSelectedObject(Node clickedNode)
        {
            //todo fill the propertyinspector with the required fields and dropdowns we need depending on the item
        }

        private void UpdateHighlightNode(Point ScreenPos)
        {
            var oldHighlight = highlightNode;
            highlightNode = GetNodeAtPoint(ScreenPos);
            if (highlightNode != oldHighlight)
            {
                Graph.Invalidate();
            }
        }

        private void UpdatePan(Point mouseLocation)
        {
            //start of pan
            if (!CurrentlyInPan)
            {
                CurrentlyInPan = true;
                //get current position in screen coordinates when we start to pan
                SetPanOffset(mouseLocation);
            }
            //in pan
            else if (CurrentlyInPan)
            {
                UpdatePanOffset(mouseLocation);
                //redraw
                Graph.Invalidate();
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

        //see https://stackoverflow.com/questions/8850528/how-to-apply-graphics-scale-and-translate-to-the-textrenderer
        private static Font GetScaledFont(Graphics g, Font f, float scale)
        {
            if (f.SizeInPoints * scale < 0)
            {
                Debugger.Break();
            }
            if (f.SizeInPoints * scale > 2000)
            {
                Debugger.Break();
            }

            return new Font(f.FontFamily,
                            f.SizeInPoints * scale,
                            f.Style,
                            GraphicsUnit.Point,
                            f.GdiCharSet,
                            f.GdiVerticalFont);
        }

        private static Rectangle GetScaledRect(Graphics g, Rectangle r, float scale)
        {
            return new Rectangle((int)Math.Ceiling(r.X * scale),
                                (int)Math.Ceiling(r.Y * scale),
                                (int)Math.Ceiling(r.Width * scale),
                                (int)Math.Ceiling(r.Height * scale));
        }
    }
}
