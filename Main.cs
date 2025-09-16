using CSC.Nodestuff;
using CSC.StoryItems;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using static CSC.StoryItems.StoryEnums;

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
        private readonly HatchBrush InterlinkedNodeBrush;
        private readonly SolidBrush ClickedNodeBrush;
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
        private float AfterZoomNodeX;
        private float AfterZoomNodeY;
        private float BeforeZoomNodeX;
        private float BeforeZoomNodeY;
        private int counter = 0;
        private bool CurrentlyInPan = false;
        private PointF end;
        private Node highlightNode = Node.NullNode;
        private bool IsCtrlPressed;
        private bool IsShiftPressed;
        private Node clickedNode = Node.NullNode;
        private Node movedNode = Node.NullNode;
        private Size OffsetFromDragClick = Size.Empty;
        private readonly Dictionary<string, float> OffsetX = [];
        private readonly Dictionary<string, float> OffsetY = [];
        private int runningTotal = 0;
        private readonly Dictionary<string, float> Scaling = [];
        private PointF start;
        private float StartPanOffsetX = 0f;
        private float StartPanOffsetY = 0f;
        private Font scaledFont = DefaultFont;
        RectangleF adjustedVisibleClipBounds = new();
        private RectangleF adjustedMouseClipBounds;
        public static string StoryName = NoCharacter;
        private static string selectedCharacter = NoCharacter;
        private static MainStory Story;
        private static Dictionary<string, CharacterStory> characterStories = new();

        //todo treat player as the story file
        public static string SelectedCharacter
        {
            get { return selectedCharacter; }

            private set
            {

                if (value == string.Empty || value is null)
                {
                    selectedCharacter = StoryName;
                }
                else
                {
                    selectedCharacter = value;
                }
            }
        }

        public Main()
        {
            InitializeComponent();
            StoryTree.ExpandAll();
            Application.AddMessageFilter(new MouseMessageFilter());
            MouseMessageFilter.MouseMove += HandleMouseEvents;
            linePen = new Pen(Brushes.LightGray, 0.3f)
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
            criterionNodeBrush = new SolidBrush(Color.FromArgb(255, 180, 20, 40));
            cutsceneNodeBrush = new SolidBrush(Color.FromArgb(255, 235, 30, 160));
            dialogueNodeBrush = new SolidBrush(Color.FromArgb(255, 45, 60, 185));
            doorNodeBrush = new SolidBrush(Color.FromArgb(255, 200, 225, 65));
            eventNodeBrush = new SolidBrush(Color.FromArgb(255, 50, 150, 50));
            eventTriggerNodeBrush = new SolidBrush(Color.FromArgb(255, 40, 120, 70));
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
            valueNodeBrush = new SolidBrush(Color.FromArgb(255, 120, 0, 150));
            HighlightNodeBrush = new SolidBrush(Color.DarkCyan);
            InterlinkedNodeBrush = new HatchBrush(HatchStyle.LightUpwardDiagonal, Color.DeepPink, BackColor);
            ClickedNodeBrush = new SolidBrush(Color.BlueViolet);

            nodes.Add(NoCharacter, new());
            Scaling.Add(NoCharacter, 0.3f);
            OffsetX.Add(NoCharacter, 0);
            OffsetY.Add(NoCharacter, 0);
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
                        //double click
                        UpdateDoubleClickTransition(ScreenPosX, ScreenPosY, ScreenPos);
                    }
                    else
                    {
                        _ = UpdateClickedNode(ScreenPosX, ScreenPosY, ScreenPos);
                        Graph.Invalidate();
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
                    _ = UpdateClickedNode(ScreenPosX, ScreenPosY, ScreenPos);
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
                UpdateScaling(e);
                //redraw
                Graph.Invalidate();
            }
        }

        private void UpdateDoubleClickTransition(float ScreenPosX, float ScreenPosY, Point ScreenPos)
        {
            var node = UpdateClickedNode(ScreenPosX, ScreenPosY, ScreenPos);
            if (node != Node.NullNode)
            {
                if (node.FileName != SelectedCharacter && node.FileName != "Player")
                {
                    if (node.FileName == StoryName)
                    {
                        StoryTree.SelectedNode = StoryTree.Nodes[0].FirstNode;
                    }
                    else
                    {
                        foreach (TreeNode treeNode in StoryTree.Nodes[0].LastNode.Nodes)
                        {
                            if (treeNode.Text == node.FileName)
                            {
                                StoryTree.SelectedNode = treeNode;
                                break;
                            }
                        }
                    }
                    SelectedCharacter = node.FileName;
                    CenterOnNode(node);
                }
            }
        }

        private void CenterOnNode(Node node)
        {
            Scaling[SelectedCharacter] = 1.5f;
            var clipWidth = Graph.Size.Width / Scaling[SelectedCharacter];
            var clipHeight = Graph.Size.Height / Scaling[SelectedCharacter];
            float x = node.Position.X - (clipWidth / 2) + (node.Size.Width / 2);
            float y = node.Position.Y - (clipHeight / 2) + (node.Size.Height / 2);
            OffsetX[SelectedCharacter] = x;
            OffsetY[SelectedCharacter] = y;
        }

        public void ScreenToGraph(float screenX, float screenY, out float graphX, out float graphY)
        {
            graphX = screenX / Scaling[SelectedCharacter] + OffsetX[SelectedCharacter];
            graphY = screenY / Scaling[SelectedCharacter] + OffsetY[SelectedCharacter];
        }

        public void GraphToScreen(float graphX, float graphY, out float screenX, out float screenY)
        {
            screenX = (graphX - OffsetX[SelectedCharacter]) * Scaling[SelectedCharacter];
            screenY = (graphY - OffsetY[SelectedCharacter]) * Scaling[SelectedCharacter];
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

        }

        private void AddChild_Click(object sender, EventArgs e)
        {

        }

        private void AddParent_Click(object sender, EventArgs e)
        {

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
            if (node == clickedNode)
            {
                if (node.FileName != SelectedCharacter)
                {
                    e.Graphics.FillPath(InterlinkedNodeBrush, RoundedRect(ScaleRect(node.Rectangle, 25), 18f));
                }
                e.Graphics.FillPath(ClickedNodeBrush, RoundedRect(ScaleRect(node.Rectangle, 15), 15f));
            }
            else if (node.FileName != SelectedCharacter)
            {
                e.Graphics.FillPath(InterlinkedNodeBrush, RoundedRect(ScaleRect(node.Rectangle, 15), 15f));
            }

            e.Graphics.FillPath(brush, RoundedRect(node.Rectangle, 10f));

            if (Scaling[SelectedCharacter] > 0.2f)
            {
                int length = 32;
                var scaledRect = GetScaledRect(e.Graphics, node.RectangleNonF, Scaling[SelectedCharacter]);
                while (TextRenderer.MeasureText(node.Text[..Math.Min(node.Text.Length, length)], scaledFont).Width > scaledRect.Size.Width)
                {
                    length--;
                }

                Color textColor = Color.White;
                if ((brush.Color.R * 0.299 + brush.Color.G * 0.587 + brush.Color.B * 0.114) > 150)
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

        private static RectangleF ScaleRect(RectangleF rect, float increase) => new(rect.X - (increase / 2), rect.Y - (increase / 2), rect.Width + increase, rect.Height + increase);

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

        private void Main_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.ToLowQuality();

            //update canvas transforms
            g.TranslateTransform(-OffsetX[SelectedCharacter] * Scaling[SelectedCharacter], -OffsetY[SelectedCharacter] * Scaling[SelectedCharacter]);
            g.ScaleTransform(Scaling[SelectedCharacter], Scaling[SelectedCharacter]);
            adjustedVisibleClipBounds = new(OffsetX[SelectedCharacter] - NodeSizeX,
                                            OffsetY[SelectedCharacter] - NodeSizeY,
                                            g.VisibleClipBounds.Width + NodeSizeX,
                                            g.VisibleClipBounds.Height + NodeSizeY);
            adjustedMouseClipBounds = new(OffsetX[SelectedCharacter],
                                            OffsetY[SelectedCharacter],
                                            g.VisibleClipBounds.Width,
                                            g.VisibleClipBounds.Height);

            if (Scaling[SelectedCharacter] < 0)
            {
                Debugger.Break();
            }
            scaledFont = GetScaledFont(g, DefaultFont, Scaling[SelectedCharacter]);

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
                LoadAllFilesIntoStore(FilePath);
            }
        }

        private void LoadAllFilesIntoStore(string FilePath)
        {
            foreach (var file in Directory.GetFiles(Path.GetDirectoryName(FilePath)!))
            {
                if (Path.GetExtension(file) == ".story")
                {
                    StoryName = Path.GetFileNameWithoutExtension(file);
                    LoadFileIntoStore(file);
                }
                else if (Path.GetExtension(file) == ".character")
                {
                    LoadFileIntoStore(file);
                }
            }

            NodeLinker.InterlinkBetweenFiles(nodes);

            SetupStartPositions();
        }

        private void LoadFileIntoStore(string FilePath)
        {
            string fileString = File.ReadAllText(FilePath);
            fileString = fileString.Replace('', ' ');

            NodeStore tempStore = new();
            //else create new
            try
            {
                if (Path.GetExtension(FilePath) == ".story")
                {
                    Story = JsonConvert.DeserializeObject<MainStory>(fileString) ?? new MainStory();
                    NodeLinker.DissectStory(Story, tempStore, Path.GetFileNameWithoutExtension(FilePath));
                }
                else
                {
                    CharacterStory story = JsonConvert.DeserializeObject<CharacterStory>(fileString) ?? new CharacterStory();
                    characterStories.Add(story.CharacterName!, story);
                    NodeLinker.DissectCharacter(story, tempStore);
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
            if (!Scaling.TryAdd(FileName, 0.3f))
            {
                Scaling[FileName] = 0.3f;
            }
            if (!OffsetX.TryAdd(FileName, 0))
            {
                OffsetX[FileName] = 0;
            }
            if (!OffsetY.TryAdd(FileName, 0))
            {
                OffsetY[FileName] = 0;
            }
            SelectedCharacter = FileName;

            Graph.Invalidate();

            if (Path.GetExtension(FilePath) == ".story")
            {
                StoryTree.Nodes[0].Nodes.Insert(0, new TreeNode(FileName));
                StoryTree.SelectedNode = StoryTree.Nodes[0].Nodes[0];
            }
            else
            {
                StoryTree.Nodes[0].LastNode.Nodes.Add(new TreeNode(FileName));
                StoryTree.SelectedNode = StoryTree.Nodes[0].LastNode.LastNode;
            }
            StoryTree.ExpandAll();
            return;
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

        //todo fix for stories, something is off. maybe also pull parents in in a second pass if they are too far away and have no other parent or sth. 
        //like we start with a root parent, then do all its childs, all their parents, all the childs childs and so on
        private void SetupStartPositions()
        {
            foreach (var store in nodes.Keys)
            {
                SelectedCharacter = store;
                layerYperX.Clear();
                layerYperX.Add(0);
                visited.Clear();

                int sideLengthY = (int)(Math.Sqrt(nodes[store].Count) + 0.5);
                int zeroColoumn = 0;

                foreach (var key in nodes[store].Nodes)
                {
                    Family family = nodes[store][key];
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
        }

        private void Start_Click(object sender, EventArgs e)
        {

        }

        private Node UpdateClickedNode(float ScreenPosX, float ScreenPosY, Point ScreenPos)
        {
            Node node = GetNodeAtPoint(ScreenPos);

            //todo only on drag move, right click alone spawns context menu with options to add criteria, event, alternate text depending on what you need
            //todo long term also do on button press/combination add the respective node
            if (MouseButtons == MouseButtons.Right)
            {
                if (node == Node.NullNode)
                {
                    return node;
                }
                if (!MovingChild)
                {
                    movedNode = node;
                    MovingChild = true;
                    OffsetFromDragClick = new Size((int)(movedNode.Position.X - ScreenPosX), (int)(movedNode.Position.Y - ScreenPosY));
                }
            }
            else
            {
                MovingChild = false;
                if (clickedNode != node && node != Node.NullNode)
                {
                    SetSelectedObject(node);
                }

                clickedNode = node;
            }
            return node;
        }

        private void SetSelectedObject(Node node)
        {
            //todo fill the propertyinspector with the required fields and dropdowns we need depending on the item
            PropertyInspector.Controls.Clear();
            PropertyInspector.SuspendLayout();
            PropertyInspector.ColumnCount = 1;
            if (node == Node.NullNode)
            {
                return;
            }

            switch (node.Type)
            {
                case NodeType.Criterion:
                {
                    PropertyInspector.RowCount = 1;
                    PropertyInspector.ColumnCount = 2;
                    Criterion criterion = (Criterion)node.Data!;

                    Label label = new()
                    {
                        Text = "Order",
                        TextAlign = ContentAlignment.MiddleRight,
                        Dock = DockStyle.Top,
                        ForeColor = Color.LightGray,
                        AutoSize = true,
                    };
                    PropertyInspector.Controls.Add(label);

                    NumericUpDown sortOrder = new()
                    {
                        //Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                        Location = new(0, PropertyInspector.Size.Height / 2),
                        Value = criterion.Order,
                        Dock = DockStyle.Left,
                        Width = 50
                    };
                    sortOrder.PerformLayout();
                    sortOrder.ValueChanged += (sender, values) => criterion.Order = (int)sortOrder.Value;
                    PropertyInspector.Controls.Add(sortOrder);

                    switch (criterion.CompareType)
                    {
                        case CompareTypes.CharacterFromCharacterGroup:
                            //todo
                            break;
                        case CompareTypes.Clothing:
                        {
                            PutCompareType(criterion);
                            PutCharacter1(criterion);

                            ComboBox clothing = new()
                            {
                                //Dock = DockStyle.Fill,
                                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                                Location = new(0, PropertyInspector.Size.Height / 2),
                                Dock = DockStyle.Fill,
                            };
                            clothing.Items.AddRange(Enum.GetNames(typeof(Clothes)));
                            clothing.SelectedIndex = int.Parse(criterion.Value!);
                            clothing.Select(clothing.SelectedItem?.ToString()?.Length ?? 0, 0);
                            clothing.PerformLayout();
                            clothing.SelectedIndexChanged += (sender, values) => criterion.Value = clothing.SelectedIndex!.ToString();
                            PropertyInspector.Controls.Add(clothing);

                            ComboBox set = new()
                            {
                                //Dock = DockStyle.Fill,
                                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                                Location = new(0, PropertyInspector.Size.Height / 2),
                                Dock = DockStyle.Fill,
                            };
                            set.Items.AddRange(Enum.GetNames(typeof(ClothingSet)));
                            set.SelectedIndex = criterion.Option!;
                            set.Select(set.SelectedItem?.ToString()?.Length ?? 0, 0);
                            set.PerformLayout();
                            set.SelectedIndexChanged += (sender, values) => criterion.Option = set.SelectedIndex!;
                            PropertyInspector.Controls.Add(set);

                            putBoolValue(criterion);

                            break;
                        }
                        case CompareTypes.CompareValues:
                        {
                            PutCompareType(criterion);
                            PutCharacter1(criterion);

                            ComboBox valueChar1 = new()
                            {
                                //Dock = DockStyle.Fill,
                                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                                Location = new(0, PropertyInspector.Size.Height / 2),
                                Dock = DockStyle.Fill,
                            };
                            if (criterion.Key == "Player")
                            {
                                valueChar1.Items.AddRange([.. Story.PlayerValues!]);
                            }
                            else
                            {
                                valueChar1.Items.AddRange([.. characterStories[criterion.Character!].StoryValues!]);
                            }
                            valueChar1.SelectedItem = criterion.Key!;
                            valueChar1.Select(valueChar1.SelectedItem?.ToString()?.Length ?? 0, 0);
                            valueChar1.PerformLayout();
                            valueChar1.SelectedIndexChanged += (sender, values) => criterion.Key = valueChar1.SelectedItem!.ToString();
                            PropertyInspector.Controls.Add(valueChar1);

                            ComboBox formula = new()
                            {
                                //Dock = DockStyle.Fill,
                                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                                Location = new(0, PropertyInspector.Size.Height / 2),
                                Dock = DockStyle.Fill,
                            };
                            formula.Items.AddRange(Enum.GetNames(typeof(ValueSpecificFormulas)));
                            formula.SelectedIndex = (int)criterion.ValueFormula!;
                            formula.Select(formula.SelectedItem?.ToString()?.Length ?? 0, 0);
                            formula.PerformLayout();
                            formula.SelectedIndexChanged += (sender, values) => criterion.ValueFormula = (ValueSpecificFormulas)formula.SelectedIndex!;
                            PropertyInspector.Controls.Add(formula);

                            PutCharacter2(criterion);

                            ComboBox valueChar2 = new()
                            {
                                //Dock = DockStyle.Fill,
                                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                                Location = new(0, PropertyInspector.Size.Height / 2),
                                Dock = DockStyle.Fill,
                            };
                            if (criterion.Key2 == "Player")
                            {
                                valueChar2.Items.AddRange([.. Story.PlayerValues!]);
                            }
                            else
                            {
                                valueChar2.Items.AddRange([.. characterStories[criterion.Character2!].StoryValues!]);
                            }
                            valueChar2.SelectedItem = criterion.Key2!;
                            valueChar2.Select(valueChar2.SelectedItem?.ToString()?.Length ?? 0, 0);
                            valueChar2.PerformLayout();
                            valueChar2.SelectedIndexChanged += (sender, values) => criterion.Key2 = valueChar2.SelectedItem!.ToString();
                            PropertyInspector.Controls.Add(valueChar2);

                            break;
                        }
                        case CompareTypes.CriteriaGroup:
                        {
                            PutCompareType(criterion);

                            ComboBox group = new()
                            {
                                //Dock = DockStyle.Fill,
                                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                                Location = new(0, PropertyInspector.Size.Height / 2),
                                Dock = DockStyle.Fill,
                            };
                            for (int i = 0; i < Story.CriteriaGroups!.Count; i++)
                            {
                                group.Items.Add(Story.CriteriaGroups[i].Name!);
                            }
                            group.SelectedItem = criterion.Value!;
                            group.SelectedIndex = group.SelectedIndex;
                            group.Select(group.SelectedItem?.ToString()?.Length ?? 0, 0);
                            group.PerformLayout();
                            group.SelectedIndexChanged += (sender, values) => criterion.Value = group.SelectedItem!.ToString();
                            PropertyInspector.Controls.Add(group);

                            putBoolValue(criterion);
                            break;
                        }
                        case CompareTypes.CutScene:
                        {
                            PutCompareType(criterion);

                            ComboBox cutscene = new()
                            {
                                //Dock = DockStyle.Fill,
                                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                                Location = new(0, PropertyInspector.Size.Height / 2),
                                Dock = DockStyle.Fill,
                            };
                            cutscene.Items.AddRange(Enum.GetNames(typeof(CutscenePlaying)));
                            cutscene.SelectedIndex = int.Parse(criterion.Value!) - 1;
                            cutscene.Select(cutscene.SelectedItem?.ToString()?.Length ?? 0, 0);
                            cutscene.PerformLayout();
                            cutscene.SelectedIndexChanged += (sender, values) => criterion.Value = (cutscene.SelectedIndex! + 1).ToString();
                            PropertyInspector.Controls.Add(cutscene);

                            putBoolValue(criterion);
                            break;
                        }
                        case CompareTypes.Dialogue:
                        {
                            PutCompareType(criterion);
                            PutCharacter1(criterion);

                            ComboBox dialogue = new()
                            {
                                //Dock = DockStyle.Fill,
                                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                                Location = new(0, PropertyInspector.Size.Height / 2),
                                Dock = DockStyle.Fill,
                            };
                            for (int i = 0; i < characterStories[criterion.Character!].Dialogues!.Count; i++)
                            {
                                dialogue.Items.Add(characterStories[criterion.Character!].Dialogues![i].ID.ToString());
                            }
                            dialogue.SelectedItem = criterion.Value!;
                            dialogue.PerformLayout();
                            dialogue.SelectedIndexChanged += (sender, values) => criterion.Value = dialogue.SelectedItem!.ToString();
                            PropertyInspector.Controls.Add(dialogue);

                            ComboBox status = new()
                            {
                                //Dock = DockStyle.Fill,
                                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                                Location = new(0, PropertyInspector.Size.Height / 2),
                                Dock = DockStyle.Fill,
                            };
                            status.Items.AddRange(Enum.GetNames(typeof(DialogueStatuses)));
                            status.SelectedItem = ((DialogueStatuses)criterion.Option!).ToString();
                            status.PerformLayout();
                            status.SelectedIndexChanged += (sender, values) => criterion.Option = (int)Enum.Parse<DialogueStatuses>(status.SelectedItem!.ToString()!);
                            PropertyInspector.Controls.Add(status);
                            break;
                        }
                        case CompareTypes.Distance:
                        {
                            PutCompareType(criterion);

                            TextBox obj1 = new()
                            {
                                Dock = DockStyle.Fill,
                                TextAlign = HorizontalAlignment.Center,
                                Text = criterion.Key
                            };
                            obj1.TextChanged += (_, args) => criterion.Key = obj1.Text;
                            PropertyInspector.Controls.Add(obj1);

                            TextBox obj2 = new()
                            {
                                Dock = DockStyle.Fill,
                                TextAlign = HorizontalAlignment.Center,
                                Text = criterion.Key
                            };
                            obj2.TextChanged += (_, args) => criterion.Key = obj2.Text;
                            PropertyInspector.Controls.Add(obj2);

                            ComboBox equ = new()
                            {
                                //Dock = DockStyle.Fill,
                                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                                Location = new(0, PropertyInspector.Size.Height / 2),
                                Dock = DockStyle.Fill,
                            };
                            equ.Items.AddRange(Enum.GetNames(typeof(ComparisonEquations)));
                            equ.SelectedIndex = int.Parse(criterion.Value!);
                            equ.Select(equ.SelectedItem?.ToString()?.Length ?? 0, 0);
                            equ.PerformLayout();
                            equ.SelectedIndexChanged += (sender, values) => criterion.Value = (equ.SelectedIndex).ToString();
                            PropertyInspector.Controls.Add(equ);

                            NumericUpDown option = new()
                            {
                                //Dock = DockStyle.Fill,
                                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                                Location = new(0, PropertyInspector.Size.Height / 2),
                                Value = criterion.Option,
                                Dock = DockStyle.Left,
                                Width = 50
                            };
                            option.PerformLayout();
                            option.ValueChanged += (sender, values) => criterion.Option = (int)option.Value;
                            PropertyInspector.Controls.Add(option);

                            break;
                        }
                        case CompareTypes.Door:
                            //todo do the rest here
                            break;
                        case CompareTypes.Gender:
                            break;
                        case CompareTypes.IntimacyPartner:
                            break;
                        case CompareTypes.IntimacyState:
                            break;
                        case CompareTypes.InZone:
                            break;
                        case CompareTypes.InVicinity:
                            break;
                        case CompareTypes.InVicinityAndVision:
                            break;
                        case CompareTypes.Item:
                            break;
                        case CompareTypes.IsBeingSpokenTo:
                            break;
                        case CompareTypes.IsAloneWithPlayer:
                            break;
                        case CompareTypes.IsDLCActive:
                            break;
                        case CompareTypes.IsOnlyInVicinityOf:
                            break;
                        case CompareTypes.IsOnlyInVisionOf:
                            break;
                        case CompareTypes.IsOnlyInVicinityAndVisionOf:
                            break;
                        case CompareTypes.IsCharacterEnabled:
                            break;
                        case CompareTypes.IsCurrentlyBeingUsed:
                            break;
                        case CompareTypes.IsCurrentlyUsing:
                            break;
                        case CompareTypes.IsExplicitGameVersion:
                            break;
                        case CompareTypes.IsGameUncensored:
                            break;
                        case CompareTypes.IsPackageInstalled:
                            break;
                        case CompareTypes.IsInFrontOf:
                            break;
                        case CompareTypes.IsInHouse:
                            break;
                        case CompareTypes.IsNewGame:
                            break;
                        case CompareTypes.IsZoneEmpty:
                            break;
                        case CompareTypes.ItemFromItemGroup:
                            break;
                        case CompareTypes.MetByPlayer:
                            break;
                        case CompareTypes.Personality:
                            break;
                        case CompareTypes.PlayerGender:
                            break;
                        case CompareTypes.PlayerInventory:
                            break;
                        case CompareTypes.PlayerPrefs:
                            break;
                        case CompareTypes.Posing:
                            break;
                        case CompareTypes.Property:
                            break;
                        case CompareTypes.Quest:
                            break;
                        case CompareTypes.SameZoneAs:
                            break;
                        case CompareTypes.ScreenFadeVisible:
                            break;
                        case CompareTypes.Social:
                            break;
                        case CompareTypes.State:
                            break;
                        case CompareTypes.Value:
                            break;
                        case CompareTypes.Vision:
                            break;
                        case CompareTypes.UseLegacyIntimacy:
                            break;
                        default:
                            break;
                    }
                    break;
                }
                case NodeType.BGC:
                {
                    PropertyInspector.RowCount = 2;
                    PropertyInspector.ColumnCount = 9;
                    Label label = new()
                    {
                        Text = node.FileName + "'s Background Chatter" + node.ID + " Speaking to:",
                        TextAlign = ContentAlignment.MiddleRight,
                        Dock = DockStyle.Fill,
                        ForeColor = Color.LightGray,
                    };
                    PropertyInspector.Controls.Add(label);
                    BackgroundChatter dialogue = ((BackgroundChatter)node.Data!);

                    ComboBox talkingTo = new()
                    {
                        //Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                        Location = new(0, PropertyInspector.Size.Height / 2),
                        Dock = DockStyle.Fill,
                    };
                    talkingTo.Items.AddRange(Enum.GetNames(typeof(StoryEnums.BGCCharacters)));
                    talkingTo.SelectedItem = dialogue.SpeakingTo;
                    talkingTo.SelectedText = string.Empty;
                    talkingTo.Select(talkingTo.SelectedItem?.ToString()?.Length ?? 0, 0);
                    talkingTo.PerformLayout();
                    talkingTo.SelectedIndexChanged += (sender, values) => dialogue.SpeakingTo = talkingTo.SelectedItem!.ToString();
                    PropertyInspector.Controls.Add(talkingTo);

                    label = new()
                    {
                        Text = "Importance:",
                        TextAlign = ContentAlignment.MiddleRight,
                        Dock = DockStyle.Fill,
                        ForeColor = Color.LightGray,
                    };
                    PropertyInspector.Controls.Add(label);

                    ComboBox importance = new()
                    {
                        //Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                        Location = new(0, PropertyInspector.Size.Height / 2),
                        Dock = DockStyle.Fill,
                    };
                    importance.Items.AddRange(Enum.GetNames(typeof(StoryEnums.Importance)));
                    importance.SelectedItem = dialogue.SpeakingTo;
                    importance.SelectedText = string.Empty;
                    importance.Select(importance.SelectedItem?.ToString()?.Length ?? 0, 0);
                    importance.PerformLayout();
                    importance.SelectedIndexChanged += (sender, values) => dialogue.SpeakingTo = importance.SelectedItem!.ToString();
                    PropertyInspector.Controls.Add(importance);

                    CheckBox checkBox = new()
                    {
                        Checked = dialogue.IsConversationStarter,
                        Dock = DockStyle.Fill,
                        Text = "Is conversation starter:",
                        CheckAlign = ContentAlignment.MiddleRight,
                        TextAlign = ContentAlignment.MiddleRight,
                        ForeColor = Color.LightGray,
                    };
                    checkBox.CheckedChanged += (_, args) => dialogue.IsConversationStarter = checkBox.Checked;
                    PropertyInspector.Controls.Add(checkBox);

                    checkBox = new()
                    {
                        Checked = dialogue.OverrideCombatRestriction,
                        Dock = DockStyle.Fill,
                        Text = "Override combat in vicinity:",
                        CheckAlign = ContentAlignment.MiddleRight,
                        TextAlign = ContentAlignment.MiddleRight,
                        ForeColor = Color.LightGray,
                    };
                    checkBox.CheckedChanged += (_, args) => dialogue.OverrideCombatRestriction = checkBox.Checked;
                    PropertyInspector.Controls.Add(checkBox);

                    checkBox = new()
                    {
                        Checked = dialogue.PlaySilently,
                        Dock = DockStyle.Fill,
                        Text = "Play voice acting at zero volume:",
                        CheckAlign = ContentAlignment.MiddleRight,
                        TextAlign = ContentAlignment.MiddleRight,
                        ForeColor = Color.LightGray,
                    };
                    checkBox.CheckedChanged += (_, args) => dialogue.PlaySilently = checkBox.Checked;
                    PropertyInspector.Controls.Add(checkBox);

                    label = new()
                    {
                        Text = "Paired Emote:",
                        TextAlign = ContentAlignment.MiddleRight,
                        Dock = DockStyle.Fill,
                        ForeColor = Color.LightGray,
                    };
                    PropertyInspector.Controls.Add(label);

                    ComboBox pairedEmote = new()
                    {
                        //Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                        Location = new(0, PropertyInspector.Size.Height / 2),
                        Dock = DockStyle.Fill,
                    };
                    pairedEmote.Items.AddRange(Enum.GetNames(typeof(StoryEnums.BGCEmotes)));
                    pairedEmote.SelectedItem = dialogue.PairedEmote;
                    pairedEmote.SelectedText = string.Empty;
                    pairedEmote.Select(pairedEmote.SelectedItem?.ToString()?.Length ?? 0, 0);
                    pairedEmote.PerformLayout();
                    pairedEmote.SelectedIndexChanged += (sender, values) => dialogue.PairedEmote = pairedEmote.SelectedItem!.ToString();
                    PropertyInspector.Controls.Add(pairedEmote);

                    TextBox text = new()
                    {
                        Text = dialogue.Text,
                        Multiline = true,
                        WordWrap = true,
                        ScrollBars = ScrollBars.Both,
                        Dock = DockStyle.Fill,
                        ForeColor = Color.LightGray,
                        BackColor = Color.FromArgb(255, 50, 50, 50),
                    };
                    text.TextChanged += (sender, values) => dialogue.Text = text.Text;
                    text.Select();
                    PropertyInspector.RowStyles[0].SizeType = SizeType.Absolute;
                    PropertyInspector.RowStyles[0].Height = 50;
                    PropertyInspector.Controls.Add(text, 0, 1);
                    PropertyInspector.SetColumnSpan(text, 9);
                    PropertyInspector.AutoSize = false;
                    break;
                }
                case NodeType.Dialogue:
                {
                    PropertyInspector.RowCount = 2;
                    PropertyInspector.ColumnCount = 5;
                    Label label = new()
                    {
                        Text = node.FileName + "'s Dialogue " + node.ID + "\n Talking to:",
                        TextAlign = ContentAlignment.MiddleRight,
                        Dock = DockStyle.Top,
                        ForeColor = Color.LightGray,
                        AutoSize = true,
                    };
                    PropertyInspector.Controls.Add(label);
                    Dialogue dialogue = ((Dialogue)node.Data!);

                    ComboBox talkingTo = new()
                    {
                        //Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                        Location = new(0, PropertyInspector.Size.Height / 2),
                        Dock = DockStyle.Top
                    };
                    talkingTo.Items.AddRange(Enum.GetNames(typeof(StoryEnums.Characters)));
                    talkingTo.SelectedItem = dialogue.SpeakingToCharacterName;
                    talkingTo.SelectedText = string.Empty;
                    talkingTo.Select(talkingTo.SelectedItem?.ToString()?.Length ?? 0, 0);
                    talkingTo.PerformLayout();
                    talkingTo.SelectedIndexChanged += (sender, values) => dialogue.SpeakingToCharacterName = talkingTo.SelectedItem!.ToString();
                    PropertyInspector.Controls.Add(talkingTo);

                    CheckBox checkBox = new()
                    {
                        Checked = dialogue.DoesNotCountAsMet,
                        Dock = DockStyle.Top,
                        Text = "Doesn't count as met:",
                        CheckAlign = ContentAlignment.MiddleRight,
                        TextAlign = ContentAlignment.MiddleRight,
                        ForeColor = Color.LightGray,
                    };
                    checkBox.CheckedChanged += (_, args) => dialogue.DoesNotCountAsMet = checkBox.Checked;
                    PropertyInspector.Controls.Add(checkBox);

                    checkBox = new()
                    {
                        Checked = dialogue.ShowGlobalResponses,
                        Dock = DockStyle.Top,
                        Text = "Show global repsonses:",
                        CheckAlign = ContentAlignment.MiddleRight,
                        TextAlign = ContentAlignment.MiddleRight,
                        ForeColor = Color.LightGray,
                    };
                    checkBox.CheckedChanged += (_, args) => dialogue.ShowGlobalResponses = checkBox.Checked;
                    PropertyInspector.Controls.Add(checkBox);

                    checkBox = new()
                    {
                        Checked = dialogue.ShowGlobalGoodByeResponses,
                        Dock = DockStyle.Top,
                        Text = "Use goodbye responses:",
                        CheckAlign = ContentAlignment.MiddleRight,
                        TextAlign = ContentAlignment.MiddleRight,
                        ForeColor = Color.LightGray,
                    };
                    checkBox.CheckedChanged += (_, args) => dialogue.ShowGlobalGoodByeResponses = checkBox.Checked;
                    PropertyInspector.Controls.Add(checkBox);

                    TextBox text = new()
                    {
                        Text = dialogue.Text,
                        Multiline = true,
                        WordWrap = true,
                        ScrollBars = ScrollBars.Both,
                        Dock = DockStyle.Fill,
                        ForeColor = Color.LightGray,
                        BackColor = Color.FromArgb(255, 50, 50, 50),
                    };
                    text.TextChanged += (sender, values) => dialogue.Text = text.Text;
                    text.Select();
                    PropertyInspector.RowStyles[0].SizeType = SizeType.Absolute;
                    PropertyInspector.RowStyles[0].Height = 35;
                    PropertyInspector.Controls.Add(text, 0, 1);
                    PropertyInspector.SetColumnSpan(text, 5);

                    break;
                }
                case NodeType.AlternateText:
                {
                    PropertyInspector.RowCount = 2;
                    PropertyInspector.ColumnCount = 3;
                    Label label = new()
                    {
                        Text = "Alternate text for " + node.FileName + "'s Dialogue " + node.ID + "\n Sort order:",
                        TextAlign = ContentAlignment.MiddleRight,
                        Dock = DockStyle.Top,
                        ForeColor = Color.LightGray,
                        AutoSize = true,
                    };
                    PropertyInspector.Controls.Add(label);
                    AlternateText alternate = ((AlternateText)node.Data!);

                    NumericUpDown sortOrder = new()
                    {
                        //Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                        Location = new(0, PropertyInspector.Size.Height / 2),
                        Value = alternate.Order,
                        Dock = DockStyle.Left,
                        Width = 50
                    };
                    sortOrder.PerformLayout();
                    sortOrder.ValueChanged += (sender, values) => alternate.Order = (int)sortOrder.Value;
                    PropertyInspector.Controls.Add(sortOrder);
                    TextBox text = new()
                    {
                        Text = alternate.Text,
                        Multiline = true,
                        WordWrap = true,
                        ScrollBars = ScrollBars.Both,
                        Dock = DockStyle.Fill,
                        ForeColor = Color.LightGray,
                        BackColor = Color.FromArgb(255, 50, 50, 50),
                    };
                    text.TextChanged += (sender, values) => alternate.Text = text.Text;
                    text.Select();
                    PropertyInspector.RowStyles[0].SizeType = SizeType.Absolute;
                    PropertyInspector.RowStyles[0].Height = 35;
                    PropertyInspector.Controls.Add(text, 0, 1);
                    PropertyInspector.SetColumnSpan(text, 3);
                    PropertyInspector.PerformLayout();
                    break;
                }
                case NodeType.Event:
                {

                    break;
                }
                case NodeType.EventTrigger:
                {

                    break;
                }
                case NodeType.Property:
                {

                    break;
                }
                case NodeType.Response:
                {

                    break;
                }
                case NodeType.Social:
                {

                    break;
                }
                case NodeType.Value:
                {
                    break;
                }
                case NodeType.Achievement:
                case NodeType.BGCResponse:
                case NodeType.CharacterGroup:
                case NodeType.Clothing:
                case NodeType.CriteriaGroup:
                case NodeType.Cutscene:
                case NodeType.Door:
                case NodeType.Inventory:
                case NodeType.Item:
                case NodeType.ItemAction:
                case NodeType.ItemGroup:
                case NodeType.ItemGroupBehaviour:
                case NodeType.ItemGroupInteraction:
                case NodeType.Null:
                case NodeType.Personality:
                case NodeType.Pose:
                case NodeType.Quest:
                case NodeType.State:
                default:
                {
                    PropertyInspector.RowCount = 1;
                    PropertyInspector.ColumnCount = 3;
                    Label label = new()
                    {
                        Text = node.Type.ToString(),
                        TextAlign = ContentAlignment.MiddleCenter,
                    };
                    PropertyInspector.Controls.Add(label, 0, 0);
                    label = new()
                    {
                        Text = node.ID,
                        TextAlign = ContentAlignment.MiddleCenter,
                    };
                    PropertyInspector.Controls.Add(label, 1, 0);
                    label = new()
                    {
                        Text = node.Text,
                        TextAlign = ContentAlignment.MiddleCenter,
                    };
                    PropertyInspector.Controls.Add(label, 2, 0);
                    break;
                }
            }

            foreach (ColumnStyle coloumn in PropertyInspector.ColumnStyles)
            {
                coloumn.SizeType = SizeType.Percent;
                coloumn.Width = (100 / PropertyInspector.ColumnCount) - 2;
            }

            PropertyInspector.ResumeLayout();

            for (int i = 0; i < PropertyInspector.Controls.Count; i++)
            {
                PropertyInspector.Controls[i].ForeColor = Color.LightGray;
                PropertyInspector.Controls[i].BackColor = Color.FromArgb(255, 50, 50, 50);

                if (PropertyInspector.Controls[i] is ComboBox box)
                {
                    if (box.SelectionLength > 0)
                    {
                        box.SelectionStart = box.SelectionLength;
                        box.SelectionLength = 1;
                    }
                }
            }

            void PutCompareType(Criterion criterion)
            {
                ComboBox compareType = new()
                {
                    //Dock = DockStyle.Fill,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                    Location = new(0, PropertyInspector.Size.Height / 2),
                    Dock = DockStyle.Fill,
                };
                compareType.Items.AddRange(Enum.GetNames(typeof(CompareTypes)));
                compareType.SelectedItem = criterion.CompareType.ToString();
                compareType.SelectedText = string.Empty;
                compareType.SelectionLength = 0;
                compareType.SelectionStart = 0;
                compareType.PerformLayout();
                compareType.SelectedIndexChanged += (sender, values) => criterion.CompareType = Enum.Parse<CompareTypes>((string)compareType.SelectedItem!);
                compareType.SelectedIndexChanged += (sender, values) => SetSelectedObject(node);
                PropertyInspector.Controls.Add(compareType);
            }

            void PutCharacter1(Criterion criterion)
            {
                ComboBox compareType = new()
                {
                    //Dock = DockStyle.Fill,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                    Location = new(0, PropertyInspector.Size.Height / 2),
                    Dock = DockStyle.Fill,
                };
                compareType.Items.AddRange(Enum.GetNames(typeof(Characters)));
                compareType.SelectedItem = criterion.Character;
                compareType.SelectionLength = 0;
                compareType.SelectionStart = 0;
                compareType.PerformLayout();
                compareType.SelectedIndexChanged += (sender, values) => criterion.Character = (string)compareType.SelectedItem!;
                PropertyInspector.Controls.Add(compareType);
            }

            void PutCharacter2(Criterion criterion)
            {
                ComboBox compareType = new()
                {
                    //Dock = DockStyle.Fill,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                    Location = new(0, PropertyInspector.Size.Height / 2),
                    Dock = DockStyle.Fill,
                };
                compareType.Items.AddRange(Enum.GetNames(typeof(Characters)));
                compareType.SelectedItem = criterion.Character2;
                compareType.SelectedText = string.Empty;
                compareType.SelectionLength = 0;
                compareType.SelectionStart = 0;
                compareType.PerformLayout();
                compareType.SelectedIndexChanged += (sender, values) => criterion.Character2 = (string)compareType.SelectedItem!;
                PropertyInspector.Controls.Add(compareType);
            }

            void putBoolValue(Criterion criterion)
            {
                ComboBox boolValue = new()
                {
                    //Dock = DockStyle.Fill,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                    Location = new(0, PropertyInspector.Size.Height / 2),
                    Dock = DockStyle.Fill,
                };
                boolValue.Items.AddRange(Enum.GetNames(typeof(BoolCritera)));
                boolValue.SelectedItem = criterion.BoolValue!.ToString();
                boolValue.SelectionLength = 0;
                boolValue.SelectionStart = 0;
                boolValue.PerformLayout();
                boolValue.SelectedIndexChanged += (sender, values) => criterion.BoolValue = Enum.Parse<BoolCritera>(boolValue.SelectedItem!.ToString()!);
                PropertyInspector.Controls.Add(boolValue);
            }
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
            OffsetX[SelectedCharacter] -= (location.X - StartPanOffsetX) / Scaling[SelectedCharacter];
            OffsetY[SelectedCharacter] -= (location.Y - StartPanOffsetY) / Scaling[SelectedCharacter];

            //replace old start by new start coordinates so we only look at one interval,
            //and do not accumulate the change in position
            StartPanOffsetX = location.X;
            StartPanOffsetY = location.Y;
        }

        private void UpdateScaling(MouseEventArgs e)
        {
            //get last mouse position in world space before the zoom so we can
            //offset back by the distance in world space we got shifted by zooming
            ScreenToGraph(e.X, e.Y, out BeforeZoomNodeX, out BeforeZoomNodeY);

            //WHEEL_DELTA = 120, as per windows documentation
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.mouseeventargs.delta?view=windowsdesktop-6.0
            if (e.Delta > 0)
            {
                if (Scaling[SelectedCharacter] < 10)
                {
                    Scaling[SelectedCharacter] *= 1.2f;
                }
            }
            else if (e.Delta < 0)
            {
                if (Scaling[SelectedCharacter] > 0.01f)
                {
                    Scaling[SelectedCharacter] *= 0.8f;
                }
            }

            //capture mouse coordinates in world space again so we can calculate the offset cause by zooming and compensate
            ScreenToGraph(e.X, e.Y, out AfterZoomNodeX, out AfterZoomNodeY);

            //update pan offset by the distance caused by zooming
            OffsetX[SelectedCharacter] += BeforeZoomNodeX - AfterZoomNodeX;
            OffsetY[SelectedCharacter] += BeforeZoomNodeY - AfterZoomNodeY;
        }

        //see https://stackoverflow.com/questions/8850528/how-to-apply-graphics-scale-and-translate-to-the-textrenderer
        private static Font GetScaledFont(Graphics g, Font f, float scale)
        {
            if (f.SizeInPoints * scale < 0)
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

        private void StoryTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is null)
            {
                return;
            }

            if (e.Node.Text != "Characters" && e.Node.Text != "Story Root")
            {
                Main.SelectedCharacter = e.Node.Text;
                Graph.Invalidate();
            }
        }
    }
}
