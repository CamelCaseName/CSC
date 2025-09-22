using CSC.Nodestuff;
using CSC.StoryItems;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;
using static CSC.StoryItems.StoryEnums;

namespace CSC;

public partial class Main : Form
{
    public const int NodeSizeX = 200;
    public const int NodeSizeY = 50;
    private static readonly Dictionary<string, NodeStore> nodes = [];
    public bool MovingChild = false;
    public const string NoCharacter = "None";
    public const string Player = "Player";
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
    private readonly SolidBrush NodeToLinkNextBrush;
    private readonly HatchBrush InterlinkedNodeBrush;
    private readonly SolidBrush ClickedNodeBrush;
    private readonly Pen highlightPen;
    private readonly SolidBrush inventoryNodeBrush;
    private readonly SolidBrush itemActionNodeBrush;
    private readonly SolidBrush itemGroupBehaviourNodeBrush;
    private readonly SolidBrush itemGroupInteractionNodeBrush;
    private readonly SolidBrush itemGroupNodeBrush;
    private readonly SolidBrush itemNodeBrush;
    private readonly List<int> maxYperX = [];
    private readonly Pen linePen;
    private readonly SolidBrush personalityNodeBrush;
    private readonly SolidBrush poseNodeBrush;
    private readonly SolidBrush propertyNodeBrush;
    private readonly SolidBrush questNodeBrush;
    private readonly SolidBrush responseNodeBrush;
    private readonly int scaleX = NodeSizeX * 2;
    private readonly int scaleY = NodeSizeY * 2;
    private readonly SolidBrush socialNodeBrush;
    private readonly SolidBrush stateNodeBrush;
    private readonly SolidBrush valueNodeBrush;
    private readonly List<Node> visited = [];
    private float AfterZoomNodeX;
    private float AfterZoomNodeY;
    private float BeforeZoomNodeX;
    private float BeforeZoomNodeY;
    private bool CurrentlyInPan = false;
    private PointF end;
    private Node highlightNode = Node.NullNode;
    private bool IsCtrlPressed;
    private bool IsShiftPressed;
    private Node clickedNode = Node.NullNode;
    private Node nodeToLinkToNext = Node.NullNode;
    private Node movedNode = Node.NullNode;
    private Size OffsetFromDragClick = Size.Empty;
    private readonly Dictionary<string, float> OffsetX = [];
    private readonly Dictionary<string, float> OffsetY = [];
    private readonly Dictionary<string, float> Scaling = [];
    private PointF start;
    private float StartPanOffsetX = 0f;
    private float StartPanOffsetY = 0f;
    private Font scaledFont = DefaultFont;
    RectangleF adjustedVisibleClipBounds = new();
    private RectangleF adjustedMouseClipBounds;
    public static string StoryName { get; private set; } = NoCharacter;
    private static string selectedCharacter = NoCharacter;
    private static MainStory Story = new();
    private static readonly Dictionary<string, CharacterStory> characterStories = [];

    public static string SelectedCharacter
    {
        get
        {
            if (selectedCharacter == NoCharacter || selectedCharacter == StoryName)
            {
                return Player;
            }
            else
            {
                return selectedCharacter;
            }
        }

        private set
        {

            if (value == string.Empty || value is null)
            {
                selectedCharacter = Player;
            }
            else
            {
                selectedCharacter = value;
            }
        }
    }

    public bool RightHasJustBeenClicked { get; private set; }

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
        NodeToLinkNextBrush = new SolidBrush(Color.LightGray);

        NodeSpawnBox.Items.AddRange(Enum.GetNames<SpawnableNodeType>());

        nodes.Add(Player, new());
        Scaling.Add(Player, 0.3f);
        OffsetX.Add(Player, 0);
        OffsetY.Add(Player, 0);
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

        if (e.KeyData == Keys.Space && !NodeSpawnBox.Enabled)
        {
            if (ActiveForm is null)
            {
                return;
            }
            var pos = Graph.PointToClient(Cursor.Position);
            ScreenToGraph(pos.X, pos.Y, out float ScreenPosX, out float ScreenPosY);
            if (adjustedMouseClipBounds.Contains(new PointF(ScreenPosX, ScreenPosY)) && Graph.Focused)
            {
                ShowNodeSpawnBox();
            }
        }
        else if (e.KeyData == Keys.Escape && NodeSpawnBox.Enabled)
        {
            NodeSpawnBox.Enabled = false;
            NodeSpawnBox.Visible = false;
        }
    }

    private void ShowNodeSpawnBox()
    {
        //only allow node types that make sense depending on the selected node type
        NodeSpawnBox.Items.Clear();
        if (clickedNode != Node.NullNode)
        {
            switch (clickedNode.Type)
            {
                //itemaction uswith criterialist event eventtrigger alternatetext
                //response dialogue bgc item itemgroup
                case NodeType.Criterion:
                {
                    NodeSpawnBox.Items.AddRange([NodeType.ItemAction, NodeType.UseWith, NodeType.CriteriaGroup, NodeType.Event, NodeType.EventTrigger, NodeType.AlternateText, NodeType.Response, NodeType.Dialogue, NodeType.BGC, NodeType.Item, NodeType.ItemGroup]);
                    break;
                }

                //item and event and criterion
                case NodeType.ItemAction:
                case NodeType.ItemGroupBehaviour:
                case NodeType.ItemGroupInteraction:
                case NodeType.Inventory:
                case NodeType.Item:
                case NodeType.ItemGroup:
                case NodeType.UseWith:
                {
                    NodeSpawnBox.Items.AddRange([NodeType.Item, NodeType.Event, NodeType.Criterion]);
                    break;
                }

                //event
                case NodeType.Pose:
                case NodeType.Achievement:
                {
                    NodeSpawnBox.Items.Add(NodeType.Event);
                    break;
                }

                //event bgcresponse
                case NodeType.BGC:
                {
                    NodeSpawnBox.Items.AddRange([NodeType.BGCResponse, NodeType.Event, NodeType.Criterion]);
                    break;
                }

                //bgc
                case NodeType.BGCResponse:
                {
                    NodeSpawnBox.Items.Add(NodeType.BGC);
                    break;
                }
                //criteria dialogue
                case NodeType.Response:
                {
                    NodeSpawnBox.Items.AddRange([NodeType.Dialogue, NodeType.Event, NodeType.Criterion]);
                    break;
                }

                case NodeType.Dialogue:
                {
                    NodeSpawnBox.Items.AddRange([NodeType.Event, NodeType.Criterion, NodeType.Response]);
                    break;
                }
                //event criterion
                case NodeType.CharacterGroup:
                case NodeType.AlternateText:
                case NodeType.EventTrigger:
                case NodeType.Value:
                case NodeType.Clothing:
                case NodeType.Personality:
                case NodeType.Quest:
                case NodeType.Social:
                case NodeType.State:
                case NodeType.Property:
                case NodeType.Cutscene:
                case NodeType.Door:
                case NodeType.CriteriaGroup:
                {
                    NodeSpawnBox.Items.AddRange([NodeType.Event, NodeType.Criterion]);
                    break;
                }
                case NodeType.Event:
                default:
                {
                    NodeSpawnBox.Items.AddRange(Enum.GetNames<SpawnableNodeType>());
                    break;
                }
            }
        }
        else
        {
            NodeSpawnBox.Items.AddRange(Enum.GetNames<SpawnableNodeType>());
        }

        NodeSpawnBox.Enabled = true;
        NodeSpawnBox.Visible = true;
        NodeSpawnBox.Focus();
    }

    public void HandleMouseEvents(object? sender, MouseEventArgs e)
    {
        if (ActiveForm is null)
        {
            return;
        }
        var pos = Graph.PointToClient(Cursor.Position);
        ScreenToGraph(pos.X, pos.Y, out float ScreenPosX, out float ScreenPosY);
        var ScreenPos = new PointF(ScreenPosX, ScreenPosY);
        //set old position for next frame/call
        switch (e.Button)
        {
            case MouseButtons.Left:
                if (e.Clicks > 1)
                {
                    //double click
                    UpdateDoubleClickTransition(ScreenPos);
                    Graph.Focus();
                }
                else
                {
                    _ = UpdateClickedNode(ScreenPos);
                    Graph.Invalidate();
                    if (adjustedMouseClipBounds.Contains(ScreenPos))
                    {
                        Graph.Focus();
                    }
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
                _ = UpdateClickedNode(ScreenPos);
                Graph.Focus();
            }
            break;
            case MouseButtons.Middle:
                UpdatePan(pos);
                Graph.Focus();
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

    private void UpdateDoubleClickTransition(PointF ScreenPos)
    {
        var node = UpdateClickedNode(ScreenPos);
        if (node != Node.NullNode)
        {
            if (node.FileName != SelectedCharacter && node.FileName != Player)
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

    private void CenterOnNode(Node node, float newScale = 1.5f)
    {
        Scaling[SelectedCharacter] = newScale;
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

    private Node GetNodeAtPoint(PointF mouseGraphLocation)
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

    //todo make objects with two different events (dialogues, items, etc) bigger and have the
    //start or stop events paths come out of a different spor lower or higher on the node, maybe even with label
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

        //int c = 0;
        foreach (var node in nodes[SelectedCharacter].Positions[adjustedVisibleClipBounds])
        {
            //c++;
            DrawNode(e, node, GetNodeColor(node.Type));
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

        if (nodeToLinkToNext != Node.NullNode)
        {
            DrawNode(e, nodeToLinkToNext, NodeToLinkNextBrush);
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
        string FileName;
        //else create new
        try
        {
            if (Path.GetExtension(FilePath) == ".story")
            {
                Story = JsonConvert.DeserializeObject<MainStory>(fileString) ?? new MainStory();
                NodeLinker.DissectStory(Story, tempStore, Path.GetFileNameWithoutExtension(FilePath));
                FileName = Player;
            }
            else
            {
                CharacterStory story = JsonConvert.DeserializeObject<CharacterStory>(fileString) ?? new CharacterStory();
                characterStories.Add(story.CharacterName!, story);
                NodeLinker.DissectCharacter(story, tempStore);
                FileName = story.CharacterName!;

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
            StoryTree.Nodes[0].Nodes.Insert(0, new TreeNode(Path.GetFileNameWithoutExtension(FilePath)));
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
        SelectedCharacter = NoCharacter;
    }

    private void SetPanOffset(Point location)
    {
        StartPanOffsetX = location.X;
        StartPanOffsetY = location.Y;
    }

    private void SetupStartPositions()
    {
        foreach (var store in nodes.Keys)
        {
            if (nodes[store].Nodes.Count <= 0)
            {
                continue;
            }

            SelectedCharacter = store;

            int sideLengthY = (int)(Math.Sqrt(nodes[store].Count) + 0.5);

            maxYperX.Clear();
            var intX = 30;
            var intY = 1;
            for (int i = 0; i < intX; i++)
            {
                maxYperX.Add(1);
            }

            visited.Clear();
            for (int i = 0; i < maxYperX.Count; i++)
            {
                maxYperX[i] = 1;
            }

            //todo getting somewhere, larger sets still need some love
            foreach (var key in nodes[store].Nodes)
            {
                Family family = nodes[store][key];
                if (family.Parents.Count != 0)
                {
                    continue;
                }

                if (visited.Contains(key))
                {
                    continue;
                }

                if (family.Childs.Count > 0)
                {
                    intX = maxYperX.Count + 1;
                }
                else
                {
                    intX += 1;
                }

                for (int i = maxYperX.Count; i <= intX; i++)
                {
                    maxYperX.Add(1);
                }

                Queue<Node> toExplore = [];
                Queue<int> layerX = [];
                Queue<int> layerY = [];
                toExplore.Enqueue(key);
                layerX.Enqueue(intX);
                layerY.Enqueue(1);

                while (toExplore.Count > 0)
                {
                    var node = toExplore.Dequeue();
                    intX = layerX.Dequeue();
                    intY = layerY.Dequeue();

                    if (visited.Contains(node))
                    {
                        continue;
                    }
                    else
                    {
                        visited.Add(node);
                    }

                    var childs = nodes[store].Childs(node);
                    childs.Sort(new NodeChildComparer(nodes[store]));
                    var parents = nodes[store].Parents(node);
                    parents.Sort(new NodeParentComparer(nodes[store]));

                    int newParentsX = intX - (parents.Count / 3) - 1;
                    int newChildX = intX + (childs.Count / 3) + 1;
                    for (int i = maxYperX.Count; i <= newChildX; i++)
                    {
                        maxYperX.Add(1);
                    }

                    //todo investigate here
                    if (maxYperX[intX] < intY)
                    {
                        maxYperX[intX] = intY;
                    }
                    else
                    {
                        maxYperX[intX]++;
                    }

                    node.Position = new PointF(intX * scaleX, Math.Max(intY, maxYperX[intX]) * scaleY);

                    if (parents.Count > 0)
                    {
                        foreach (var item in parents)
                        {
                            if (visited.Contains(item))
                            {
                                continue;
                            }

                            layerX.Enqueue(newParentsX);
                            layerY.Enqueue(Math.Max(intY++, maxYperX[newParentsX]));
                            toExplore.Enqueue(item);
                        }
                    }

                    if (childs.Count > 0)
                    {
                        foreach (var item in childs)
                        {
                            if (visited.Contains(item))
                            {
                                continue;
                            }

                            layerX.Enqueue(newChildX);
                            layerY.Enqueue(Math.Max(intY++, maxYperX[newChildX]));
                            toExplore.Enqueue(item);
                        }
                    }
                }
            }

            CenterOnNode(nodes[store].Nodes.First(), 0.8f);
        }
    }

    private void Start_Click(object sender, EventArgs e)
    {

    }

    private Node UpdateClickedNode(PointF ScreenPos)
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

            if (RightHasJustBeenClicked && !MovingChild)
            {
                movedNode = node;
                MovingChild = true;
                OffsetFromDragClick = new Size((int)(movedNode.Position.X - ScreenPos.X), (int)(movedNode.Position.Y - ScreenPos.Y));
            }
            else if (!RightHasJustBeenClicked)
            {
                RightHasJustBeenClicked = true;
            }
        }
        else
        {
            if (RightHasJustBeenClicked && !MovingChild)
            {
                //right click only no move, spawn context
                SpawnContextMenu(node, ScreenPos);
            }

            MovingChild = false;
            if (node != Node.NullNode)
            {
                if (IsCtrlPressed
                    && node.Type is NodeType.Event or NodeType.Criterion
                    && (node.Data != null && node.Data?.GetType() != typeof(MissingreferenceInfo)))
                {
                    nodeToLinkToNext = node;
                    if (highlightNode == node)
                    {
                        highlightNode = Node.NullNode;
                    }
                }
                else if (!IsCtrlPressed && nodeToLinkToNext != Node.NullNode)
                {
                    AddNodeToNextClicked(node);
                    ShowProperties(node);
                }
                else if (clickedNode != node)
                {
                    ShowProperties(node);
                }
            }
            else
            {
                nodeToLinkToNext = Node.NullNode;
            }

            clickedNode = node;
        }
        return node;
    }

    private void SpawnContextMenu(Node node, PointF screenPos)
    {

        //todo
    }

    private void AddNodeToNextClicked(Node addToThis)
    {
        if (addToThis.Data is null || addToThis.Data?.GetType() == typeof(MissingreferenceInfo))
        {
            return;
        }

        bool linked = false;

        if (nodeToLinkToNext.DataType == typeof(Criterion))
        {
            if (addToThis.DataType == typeof(ItemAction))
            {
                ((ItemAction)addToThis.Data!).Criteria!.Add((Criterion)nodeToLinkToNext.Data!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(UseWith))
            {
                ((UseWith)addToThis.Data!).Criteria!.Add((Criterion)nodeToLinkToNext.Data!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(CriteriaList1))
            {
                ((CriteriaList1)addToThis.Data!).CriteriaList!.Add((Criterion)nodeToLinkToNext.Data!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(GameEvent))
            {
                ((GameEvent)addToThis.Data!).Criteria!.Add((Criterion)nodeToLinkToNext.Data!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(EventTrigger))
            {
                ((EventTrigger)addToThis.Data!).Critera!.Add((Criterion)nodeToLinkToNext.Data!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(AlternateText))
            {
                ((AlternateText)addToThis.Data!).Critera!.Add((Criterion)nodeToLinkToNext.Data!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(Response))
            {
                ((Response)addToThis.Data!).ResponseCriteria!.Add((Criterion)nodeToLinkToNext.Data!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(BackgroundChatter))
            {
                ((BackgroundChatter)addToThis.Data!).Critera!.Add((Criterion)nodeToLinkToNext.Data!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(ItemInteraction))
            {
                ((ItemInteraction)addToThis.Data!).Critera!.Add((Criterion)nodeToLinkToNext.Data!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(ItemGroupInteraction))
            {
                ((ItemGroupInteraction)addToThis.Data!).Critera!.Add((Criterion)nodeToLinkToNext.Data!);
                linked = true;
            }

            if (linked)
            {
                nodes[addToThis.FileName].AddParent(addToThis, nodeToLinkToNext);
            }
        }
        else if (nodeToLinkToNext.DataType == typeof(GameEvent))
        {
            if (addToThis.DataType == typeof(ItemAction))
            {
                ((ItemAction)addToThis.Data!).OnTakeActionEvents!.Add((GameEvent)nodeToLinkToNext.Data!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(UseWith))
            {
                ((UseWith)addToThis.Data!).OnSuccessEvents!.Add((GameEvent)nodeToLinkToNext.Data!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(EventTrigger))
            {
                ((EventTrigger)addToThis.Data!).Events!.Add((GameEvent)nodeToLinkToNext.Data!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(MainStory))
            {
                ((MainStory)addToThis.Data!).GameStartEvents!.Add((GameEvent)nodeToLinkToNext.Data!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(Response))
            {
                ((Response)addToThis.Data!).ResponseEvents!.Add((GameEvent)nodeToLinkToNext.Data!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(Dialogue))
            {
                var result = MessageBox.Show("Add as StartEvent? Hit yes for StartEvent, no for CloseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ((Dialogue)addToThis.Data!).StartEvents!.Add((GameEvent)nodeToLinkToNext.Data!);
                    linked = true;
                }
                else if (result == DialogResult.No)
                {
                    ((Dialogue)addToThis.Data!).CloseEvents!.Add((GameEvent)nodeToLinkToNext.Data!);
                    linked = true;
                }
            }
            else if (addToThis.DataType == typeof(BackgroundChatter))
            {
                ((BackgroundChatter)addToThis.Data!).StartEvents!.Add((GameEvent)nodeToLinkToNext.Data!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(ItemInteraction))
            {
                var result = MessageBox.Show("Add as OnAcceptEvent? Hit yes for OnAcceptEvent, no for OnRefuseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ((ItemInteraction)addToThis.Data!).OnAcceptEvents!.Add((GameEvent)nodeToLinkToNext.Data!);
                    linked = true;
                }
                else if (result == DialogResult.No)
                {
                    ((ItemInteraction)addToThis.Data!).OnRefuseEvents!.Add((GameEvent)nodeToLinkToNext.Data!);
                    linked = true;
                }
            }
            else if (addToThis.DataType == typeof(ItemGroupInteraction))
            {
                var result = MessageBox.Show("Add as OnAcceptEvent? Hit yes for OnAcceptEvent, no for OnRefuseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ((ItemGroupInteraction)addToThis.Data!).OnAcceptEvents!.Add((GameEvent)nodeToLinkToNext.Data!);
                    linked = true;
                }
                else if (result == DialogResult.No)
                {
                    ((ItemGroupInteraction)addToThis.Data!).OnRefuseEvents!.Add((GameEvent)nodeToLinkToNext.Data!);
                    linked = true;
                }
            }

            if (linked)
            {
                nodes[addToThis.FileName].AddChild(addToThis, nodeToLinkToNext);
            }
        }

        //todo also do for dialogues and responses and alternate texts...

        nodeToLinkToNext = Node.NullNode;
    }

    //todo implement updating of node references on changing values!!!
    //todo implement drag/drop setting of node data like item name or sth
    private void ShowProperties(Node node)
    {
        PropertyInspector.Controls.Clear();
        PropertyInspector.SuspendLayout();
        PropertyInspector.ColumnCount = 1;
        PropertyInspector.RowCount = 1;
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
                    Text = "Order:",
                    TextAlign = ContentAlignment.MiddleRight,
                    Dock = DockStyle.Top,
                    ForeColor = Color.LightGray,
                    Height = 30
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
                sortOrder.ValueChanged += (_, _) => criterion.Order = (int)sortOrder.Value;
                PropertyInspector.Controls.Add(sortOrder);

                switch (criterion.CompareType)
                {
                    case CompareTypes.CharacterFromCharacterGroup:
                    {
                        //todo
                        break;
                    }
                    case CompareTypes.Clothing:
                    {
                        PutCompareType(criterion, node);
                        PutCharacter1(criterion);

                        ComboBox clothing = GetComboBox();
                        clothing.Items.AddRange(Enum.GetNames(typeof(Clothes)));
                        if (int.TryParse(criterion.Value!, out int res))
                        {
                            clothing.SelectedIndex = res;
                        }
                        else
                        {
                            clothing.SelectedIndex = 0;
                        }
                        clothing.Select(clothing.SelectedItem?.ToString()?.Length ?? 0, 0);
                        clothing.PerformLayout();
                        clothing.SelectedIndexChanged += (_, _) => criterion.Value = clothing.SelectedIndex!.ToString();
                        PropertyInspector.Controls.Add(clothing);

                        ComboBox set = GetComboBox();
                        set.Items.AddRange(Enum.GetNames(typeof(ClothingSet)));
                        set.SelectedIndex = criterion.Option!;
                        set.Select(set.SelectedItem?.ToString()?.Length ?? 0, 0);
                        set.PerformLayout();
                        set.SelectedIndexChanged += (_, _) => criterion.Option = set.SelectedIndex!;
                        PropertyInspector.Controls.Add(set);

                        PutBoolValue(criterion);

                        break;
                    }
                    case CompareTypes.CoinFlip:
                    {
                        PutCompareType(criterion, node);
                        break;
                    }
                    case CompareTypes.CompareValues:
                    {
                        PutCompareType(criterion, node);
                        PutCharacter1(criterion);

                        ComboBox valueChar1 = GetComboBox();
                        if (criterion.Key == Player)
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
                        valueChar1.SelectedIndexChanged += (_, _) => criterion.Key = valueChar1.SelectedItem!.ToString();
                        PropertyInspector.Controls.Add(valueChar1);

                        ComboBox formula = GetComboBox();
                        formula.Items.AddRange(Enum.GetNames(typeof(ValueSpecificFormulas)));
                        formula.SelectedIndex = (int)criterion.ValueFormula!;
                        formula.Select(formula.SelectedItem?.ToString()?.Length ?? 0, 0);
                        formula.PerformLayout();
                        formula.SelectedIndexChanged += (_, _) => criterion.ValueFormula = (ValueSpecificFormulas)formula.SelectedIndex!;
                        PropertyInspector.Controls.Add(formula);

                        PutCharacter2(criterion);

                        ComboBox valueChar2 = GetComboBox();
                        if (criterion.Key2 == Player)
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
                        valueChar2.SelectedIndexChanged += (_, _) => criterion.Key2 = valueChar2.SelectedItem!.ToString();
                        PropertyInspector.Controls.Add(valueChar2);

                        break;
                    }
                    case CompareTypes.CriteriaGroup:
                    {
                        PutCompareType(criterion, node);

                        ComboBox group = GetComboBox();
                        for (int i = 0; i < Story.CriteriaGroups!.Count; i++)
                        {
                            group.Items.Add(Story.CriteriaGroups[i].Name!);
                        }
                        group.SelectedItem = criterion.Value!;
                        group.SelectedIndex = group.SelectedIndex;
                        group.Select(group.SelectedItem?.ToString()?.Length ?? 0, 0);
                        group.PerformLayout();
                        group.SelectedIndexChanged += (_, _) => criterion.Value = group.SelectedItem!.ToString();
                        PropertyInspector.Controls.Add(group);

                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.CutScene:
                    {
                        PutCompareType(criterion, node);

                        ComboBox cutscene = GetComboBox();
                        cutscene.Items.AddRange(Enum.GetNames(typeof(CutscenePlaying)));
                        if (int.TryParse(criterion.Value!, out int res))
                        {
                            cutscene.SelectedIndex = res - 1;
                        }
                        else
                        {
                            cutscene.SelectedIndex = 0;
                        }
                        cutscene.PerformLayout();
                        cutscene.SelectedIndexChanged += (_, _) => criterion.Value = (cutscene.SelectedIndex! + 1).ToString();
                        PropertyInspector.Controls.Add(cutscene);

                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.Dialogue:
                    {
                        PutCompareType(criterion, node);
                        PutCharacter1(criterion);

                        ComboBox dialogue = GetComboBox();
                        for (int i = 0; i < characterStories[criterion.Character!].Dialogues!.Count; i++)
                        {
                            dialogue.Items.Add(characterStories[criterion.Character!].Dialogues![i].ID.ToString());
                        }
                        dialogue.SelectedItem = criterion.Value!;
                        dialogue.PerformLayout();
                        dialogue.SelectedIndexChanged += (_, _) => criterion.Value = dialogue.SelectedItem!.ToString();
                        PropertyInspector.Controls.Add(dialogue);

                        ComboBox status = GetComboBox();
                        status.Items.AddRange(Enum.GetNames(typeof(DialogueStatuses)));
                        status.SelectedItem = ((DialogueStatuses)criterion.Option!).ToString();
                        status.PerformLayout();
                        status.SelectedIndexChanged += (_, _) => criterion.Option = (int)Enum.Parse<DialogueStatuses>(status.SelectedItem!.ToString()!);
                        PropertyInspector.Controls.Add(status);
                        break;
                    }
                    case CompareTypes.Distance:
                    {
                        PutCompareType(criterion, node);

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
                            Text = criterion.Key2
                        };
                        obj2.TextChanged += (_, args) => criterion.Key2 = obj2.Text;
                        PropertyInspector.Controls.Add(obj2);

                        PutComparison(criterion);

                        PutNumericOption(criterion);

                        break;
                    }
                    case CompareTypes.Door:
                    {

                        PutCompareType(criterion, node);

                        ComboBox door = GetComboBox();
                        door.Items.AddRange(Enum.GetNames(typeof(Doors)));
                        door.SelectedItem = criterion.Key?.Replace(" ", "");
                        door.PerformLayout();
                        door.SelectedIndexChanged += (_, _) => criterion.Key = door.SelectedItem!.ToString();
                        PropertyInspector.Controls.Add(door);

                        ComboBox doorstate = GetComboBox();
                        doorstate.Items.AddRange(Enum.GetNames(typeof(DoorOptionValues)));
                        doorstate.SelectedIndex = criterion.Option!;
                        doorstate.PerformLayout();
                        doorstate.SelectedIndexChanged += (_, _) => criterion.Option = (doorstate.SelectedIndex);
                        PropertyInspector.Controls.Add(doorstate);
                        break;
                    }
                    case CompareTypes.Gender:
                    {
                        //todo
                        break;
                    }
                    case CompareTypes.IntimacyPartner:
                    {
                        PutCompareType(criterion, node);
                        PutCharacter1(criterion);

                        PutEquals(criterion);

                        ComboBox character = GetComboBox();
                        character.Items.AddRange(Enum.GetNames(typeof(IntimateCharacters)));
                        character.SelectedItem = criterion.Value;
                        character.SelectionLength = 0;
                        character.SelectionStart = 0;
                        character.PerformLayout();
                        character.SelectedIndexChanged += (_, _) => criterion.Value = (string)character.SelectedItem!;
                        PropertyInspector.Controls.Add(character);

                        break;
                    }
                    case CompareTypes.IntimacyState:
                    {
                        PutCompareType(criterion, node);
                        PutCharacter1(criterion);

                        PutEquals(criterion);

                        ComboBox state = GetComboBox();
                        state.Items.AddRange(Enum.GetNames(typeof(SexualActs)));
                        state.SelectedItem = criterion.Value;
                        state.SelectionLength = 0;
                        state.SelectionStart = 0;
                        state.PerformLayout();
                        state.SelectedIndexChanged += (_, _) => criterion.Value = (string)state.SelectedItem!;
                        PropertyInspector.Controls.Add(state);

                        break;
                    }
                    case CompareTypes.InZone:
                    {
                        PutCompareType(criterion, node);
                        PutCharacter1(criterion);
                        PutZone(criterion);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.InVicinity:
                    case CompareTypes.InVicinityAndVision:
                    case CompareTypes.IsOnlyInVicinityOf:
                    case CompareTypes.IsOnlyInVisionOf:
                    case CompareTypes.IsOnlyInVicinityAndVisionOf:
                    {
                        PutCharacter1(criterion);
                        PutCompareType(criterion, node);
                        PutCharacter2(criterion);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.Item:
                    {
                        PutCompareType(criterion, node);

                        PutItem(criterion);

                        ComboBox state = GetComboBox();
                        state.Items.AddRange(Enum.GetNames(typeof(ItemComparisonTypes)));
                        state.SelectedItem = criterion.ItemComparison.ToString();
                        state.SelectionLength = 0;
                        state.SelectionStart = 0;
                        state.PerformLayout();
                        state.SelectedIndexChanged += (_, _) => criterion.ItemComparison = Enum.Parse<ItemComparisonTypes>(state.SelectedItem!.ToString()!);
                        PropertyInspector.Controls.Add(state);

                        break;
                    }
                    case CompareTypes.IsBeingSpokenTo:
                    {
                        PutCompareType(criterion, node);
                        PutCharacter1(criterion);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.IsAloneWithPlayer:
                    {
                        PutCharacter1(criterion);
                        PutCompareType(criterion, node);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.IsDLCActive:
                    {
                        //todo
                        break;
                    }
                    case CompareTypes.IsCharacterEnabled:
                    {
                        PutCompareType(criterion, node);
                        PutCharacter1(criterion);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.IsCurrentlyBeingUsed:
                    {
                        PutCompareType(criterion, node);
                        PutItem(criterion);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.IsCurrentlyUsing:
                    {
                        PutCompareType(criterion, node);
                        PutCharacter1(criterion);
                        PutItem(criterion);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.IsExplicitGameVersion:
                    {
                        PutCompareType(criterion, node);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.IsGameUncensored:
                    {
                        PutCompareType(criterion, node);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.IsPackageInstalled:
                    {
                        //todo
                        break;
                    }
                    case CompareTypes.IsInFrontOf:
                    {
                        PutCharacter1(criterion);
                        PutCompareType(criterion, node);
                        PutCharacter2(criterion);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.IsInHouse:
                    {
                        PutCharacter1(criterion);
                        PutCompareType(criterion, node);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.IsNewGame:
                    {
                        PutCompareType(criterion, node);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.IsZoneEmpty:
                    {
                        PutCompareType(criterion, node);
                        PutZone(criterion);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.ItemFromItemGroup:
                    {
                        //todo
                        break;
                    }
                    case CompareTypes.MetByPlayer:
                    {
                        PutCompareType(criterion, node);
                        PutCharacter1(criterion);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.Personality:
                    {
                        PutCompareType(criterion, node);
                        PutCharacter1(criterion);

                        ComboBox trait = GetComboBox();
                        trait.Items.AddRange(Enum.GetNames(typeof(PersonalityTraits)));
                        if (int.TryParse(criterion.Key!, out int res))
                        {
                            trait.SelectedIndex = res;
                        }
                        else
                        {
                            trait.SelectedIndex = 0;
                        }
                        trait.PerformLayout();
                        trait.SelectedIndexChanged += (_, _) => criterion.Key = trait.SelectedIndex.ToString();
                        PropertyInspector.Controls.Add(trait);

                        PutComparison(criterion);
                        PutNumericValue(criterion);

                        break;
                    }
                    case CompareTypes.PlayerGender:
                    {
                        PutCompareType(criterion, node);

                        ComboBox gender = GetComboBox();
                        gender.Items.AddRange(Enum.GetNames(typeof(Gender)));
                        gender.SelectedItem = criterion.Value;
                        gender.PerformLayout();
                        gender.SelectedIndexChanged += (_, _) => criterion.Value = gender.SelectedItem!.ToString();
                        PropertyInspector.Controls.Add(gender);

                        break;
                    }
                    case CompareTypes.PlayerInventory:
                    {
                        PutCompareType(criterion, node);

                        ComboBox state = GetComboBox();
                        state.Items.AddRange(Enum.GetNames(typeof(PlayerInventoryOptions)));
                        state.SelectedItem = criterion.PlayerInventoryOption.ToString();
                        state.PerformLayout();
                        state.SelectedIndexChanged += (_, _) => criterion.PlayerInventoryOption = Enum.Parse<PlayerInventoryOptions>(state.SelectedItem!.ToString()!);
                        PropertyInspector.Controls.Add(state);

                        PutItem(criterion);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.PlayerPrefs:
                    {
                        PutCompareType(criterion, node);

                        ComboBox pref = GetComboBox();
                        pref.Items.AddRange(Enum.GetNames(typeof(PlayerPrefs)));
                        pref.SelectedItem = criterion.Key;
                        pref.PerformLayout();
                        pref.SelectedIndexChanged += (_, _) => criterion.Key = pref.SelectedItem!.ToString();
                        PropertyInspector.Controls.Add(pref);

                        PutComparison(criterion);
                        PutTextValue(criterion);

                        break;
                    }
                    case CompareTypes.Posing:
                    {
                        PutCompareType(criterion, node);
                        PutCharacter1(criterion);

                        ComboBox option = GetComboBox();
                        option.Items.AddRange(Enum.GetNames(typeof(PoseOptions)));
                        option.SelectedItem = criterion.PoseOption.ToString();
                        option.PerformLayout();
                        option.SelectedIndexChanged += (_, _) => criterion.PoseOption = Enum.Parse<PoseOptions>(option.SelectedItem!.ToString()!);
                        option.SelectedIndexChanged += (_, _) => ShowProperties(node);
                        PropertyInspector.Controls.Add(option);

                        if (criterion.PoseOption == PoseOptions.CurrentPose)
                        {
                            PutEquals(criterion);

                            ComboBox pose = GetComboBox();
                            pose.Items.AddRange(Enum.GetNames(typeof(Poses)));
                            pose.SelectedItem = Enum.Parse<Poses>(criterion.Value!).ToString();
                            pose.PerformLayout();
                            pose.SelectedIndexChanged += (sender, values) => criterion.Value = ((int)Enum.Parse<Poses>(pose.SelectedItem!.ToString()!)).ToString();
                            PropertyInspector.Controls.Add(pose);
                        }
                        else
                        {
                            PutBoolValue(criterion);
                        }
                        break;
                    }
                    case CompareTypes.Property:
                    {
                        PutCompareType(criterion, node);
                        PutCharacter1(criterion);

                        ComboBox property = GetComboBox();
                        property.Items.AddRange(Enum.GetNames(typeof(InteractiveProperties)));
                        property.SelectedItem = Enum.Parse<InteractiveProperties>(criterion.Value!).ToString();
                        property.PerformLayout();
                        property.SelectedIndexChanged += (sender, values) => criterion.Value = ((int)Enum.Parse<InteractiveProperties>(property.SelectedItem!.ToString()!)).ToString();
                        PropertyInspector.Controls.Add(property);

                        PutBoolValue(criterion);

                        break;
                    }
                    case CompareTypes.Quest:
                    {
                        PutCompareType(criterion, node);

                        ComboBox quest = GetComboBox();
                        foreach (string key in characterStories.Keys)
                        {
                            for (int i = 0; i < characterStories[key].Quests!.Count; i++)
                            {
                                quest.Items.Add(characterStories[key].Quests![i].Name!);
                            }
                        }
                        quest.SelectedItem = criterion.Key!;
                        quest.PerformLayout();
                        quest.SelectedIndexChanged += (_, _) =>
                        {
                            //todo implement quest store or sth
                            criterion.Key2 = quest.SelectedItem!.ToString();
                            foreach (string key in characterStories.Keys)
                            {
                                for (int i = 0; i < characterStories[key].Quests!.Count; i++)
                                {
                                    if (characterStories[key].Quests![i].Name == criterion.Key2)
                                    {
                                        criterion.Key = characterStories[key].Quests![i].ID;
                                    }
                                }
                            }
                        };
                        PropertyInspector.Controls.Add(quest);

                        PutEquals(criterion);

                        ComboBox obtained = GetComboBox();
                        obtained.Items.AddRange(Enum.GetNames(typeof(QuestStatus)));
                        obtained.SelectedItem = Enum.Parse<QuestStatus>(criterion.Value!).ToString();
                        obtained.PerformLayout();
                        obtained.SelectedIndexChanged += (sender, values) => criterion.Value = ((int)Enum.Parse<QuestStatus>(obtained.SelectedItem!.ToString()!)).ToString();
                        PropertyInspector.Controls.Add(obtained);
                        break;
                    }
                    case CompareTypes.SameZoneAs:
                    {
                        PutCharacter1(criterion);
                        PutCompareType(criterion, node);
                        PutCharacter2(criterion);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.ScreenFadeVisible:
                    {
                        PutCompareType(criterion, node);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.Social:
                    {
                        PutCompareType(criterion, node);
                        PutCharacter1(criterion);

                        ComboBox social = GetComboBox();
                        social.Items.AddRange(Enum.GetNames(typeof(SocialStatuses)));
                        social.SelectedItem = criterion.SocialStatus!.ToString();
                        social.PerformLayout();
                        social.SelectedIndexChanged += (sender, values) => criterion.SocialStatus = Enum.Parse<SocialStatuses>(social.SelectedItem!.ToString()!);
                        PropertyInspector.Controls.Add(social);

                        PutComparison(criterion);
                        PutNumericValue(criterion);

                        break;
                    }
                    case CompareTypes.State:
                    {
                        PutCompareType(criterion, node);
                        PutCharacter1(criterion);

                        ComboBox state = GetComboBox();
                        state.Items.AddRange(Enum.GetNames(typeof(InteractiveStates)));
                        state.SelectedItem = Enum.Parse<InteractiveStates>(criterion.Value!).ToString();
                        state.PerformLayout();
                        state.SelectedIndexChanged += (sender, values) => criterion.Value = ((int)Enum.Parse<InteractiveStates>(state.SelectedItem!.ToString()!)).ToString();
                        PropertyInspector.Controls.Add(state);

                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.Value:
                    {
                        PutCompareType(criterion, node);
                        PutCharacter1(criterion).SelectedIndexChanged += (_, _) => ShowProperties(node);

                        ComboBox value = GetComboBox();
                        if (criterion.Character == Player)
                        {
                            for (int i = 0; i < Story.PlayerValues!.Count; i++)
                            {
                                value.Items.Add(Story.PlayerValues![i]);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < characterStories[criterion.Character!].StoryValues!.Count; i++)
                            {
                                value.Items.Add(characterStories[criterion.Character!].StoryValues![i]);
                            }
                        }
                        value.SelectedItem = criterion.Key!;
                        value.PerformLayout();
                        value.SelectedIndexChanged += (sender, values) => criterion.Key = value.SelectedItem!.ToString();
                        PropertyInspector.Controls.Add(value);

                        PutComparison(criterion);

                        TextBox obj2 = new()
                        {
                            Dock = DockStyle.Fill,
                            TextAlign = HorizontalAlignment.Center,
                            Text = criterion.Value
                        };
                        obj2.TextChanged += (_, args) => criterion.Value = obj2.Text;
                        PropertyInspector.Controls.Add(obj2);

                        break;
                    }
                    case CompareTypes.Vision:
                    {
                        PutCharacter1(criterion);
                        PutCompareType(criterion, node);
                        PutCharacter2(criterion);
                        PutBoolValue(criterion);
                        break;
                    }
                    case CompareTypes.UseLegacyIntimacy:
                    {
                        PutCompareType(criterion, node);
                        PutBoolValue(criterion);
                        break;
                    }
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
                    Text = node.FileName + "'s\n Background Chatter" + node.ID + "\nSpeaking to:",
                    TextAlign = ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.LightGray,
                    AutoSize = true,
                };
                PropertyInspector.Controls.Add(label);
                BackgroundChatter dialogue = ((BackgroundChatter)node.Data!);

                ComboBox talkingTo = GetComboBox();
                talkingTo.Items.AddRange(Enum.GetNames(typeof(StoryEnums.AnybodyCharacters)));
                talkingTo.SelectedItem = dialogue.SpeakingTo;
                talkingTo.SelectedIndexChanged += (_, _) => dialogue.SpeakingTo = talkingTo.SelectedItem!.ToString();
                PropertyInspector.Controls.Add(talkingTo);

                label = new()
                {
                    Text = "Importance:",
                    TextAlign = ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.LightGray,
                    AutoSize = true,
                };
                PropertyInspector.Controls.Add(label);

                ComboBox importance = GetComboBox();
                importance.Items.AddRange(Enum.GetNames(typeof(StoryEnums.Importance)));
                importance.SelectedItem = dialogue.SpeakingTo;
                importance.SelectedIndexChanged += (_, _) => dialogue.SpeakingTo = importance.SelectedItem!.ToString();
                PropertyInspector.Controls.Add(importance);

                CheckBox checkBox = new()
                {
                    Checked = dialogue.IsConversationStarter,
                    Dock = DockStyle.Fill,
                    Text = "Is conversation starter:",
                    CheckAlign = ContentAlignment.MiddleRight,
                    TextAlign = ContentAlignment.MiddleRight,
                    ForeColor = Color.LightGray,
                    AutoSize = true,
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
                    AutoSize = true,
                };
                checkBox.CheckedChanged += (_, args) => dialogue.OverrideCombatRestriction = checkBox.Checked;
                PropertyInspector.Controls.Add(checkBox);

                checkBox = new()
                {
                    Checked = dialogue.PlaySilently,
                    Dock = DockStyle.Fill,
                    Text = "Silence Voice Acting:",
                    CheckAlign = ContentAlignment.MiddleRight,
                    TextAlign = ContentAlignment.MiddleRight,
                    ForeColor = Color.LightGray,
                    AutoSize = true,
                };
                checkBox.CheckedChanged += (_, args) => dialogue.PlaySilently = checkBox.Checked;
                PropertyInspector.Controls.Add(checkBox);

                label = new()
                {
                    Text = "Paired Emote:",
                    TextAlign = ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.LightGray,
                    AutoSize = true,
                };
                PropertyInspector.Controls.Add(label);

                ComboBox pairedEmote = GetComboBox();
                pairedEmote.Items.AddRange(Enum.GetNames(typeof(StoryEnums.BGCEmotes)));
                pairedEmote.SelectedItem = dialogue.PairedEmote;
                pairedEmote.SelectedIndexChanged += (_, _) => dialogue.PairedEmote = pairedEmote.SelectedItem!.ToString();
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
                text.TextChanged += (_, _) => dialogue.Text = text.Text;
                text.Select();
                PropertyInspector.RowStyles[0].SizeType = SizeType.Absolute;
                PropertyInspector.RowStyles[0].Height = 55;
                PropertyInspector.Controls.Add(text, 0, 1);
                PropertyInspector.SetColumnSpan(text, 9);
                PropertyInspector.AutoSize = false;
                break;
            }
            case NodeType.Dialogue:
            {
                PropertyInspector.RowCount = 1;
                Label label = new()
                {
                    Text = node.FileName + "'s Dialogue " + node.ID + "\n Talking to:",
                    TextAlign = ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.LightGray,
                    AutoSize = true,
                };
                PropertyInspector.Controls.Add(label);
                if (node.Data?.GetType() != typeof(Dialogue))
                {
                    Label label2 = new()
                    {
                        Text = "No data on this node",
                        TextAlign = ContentAlignment.MiddleLeft,
                        Dock = DockStyle.Fill,
                        ForeColor = Color.LightGray
                    };
                    PropertyInspector.Controls.Add(label2);
                    break;
                }
                PropertyInspector.RowCount = 2;
                Dialogue dialogue = ((Dialogue)node.Data!);

                ComboBox talkingTo = GetComboBox();
                talkingTo.Items.AddRange(Enum.GetNames(typeof(StoryEnums.Characters)));
                talkingTo.SelectedItem = dialogue.SpeakingToCharacterName;
                talkingTo.SelectedText = string.Empty;
                talkingTo.Select(talkingTo.SelectedItem?.ToString()?.Length ?? 0, 0);
                talkingTo.PerformLayout();
                talkingTo.SelectedIndexChanged += (_, _) => dialogue.SpeakingToCharacterName = talkingTo.SelectedItem!.ToString();
                PropertyInspector.Controls.Add(talkingTo);

                CheckBox checkBox = new()
                {
                    Checked = dialogue.DoesNotCountAsMet,
                    Dock = DockStyle.Fill,
                    Text = "Doesn't count as met:",
                    CheckAlign = ContentAlignment.MiddleRight,
                    TextAlign = ContentAlignment.MiddleRight,
                    ForeColor = Color.LightGray,
                    AutoSize = true,
                };
                checkBox.CheckedChanged += (_, args) => dialogue.DoesNotCountAsMet = checkBox.Checked;
                PropertyInspector.Controls.Add(checkBox);

                checkBox = new()
                {
                    Checked = dialogue.ShowGlobalResponses,
                    Dock = DockStyle.Fill,
                    Text = "Show global repsonses:",
                    CheckAlign = ContentAlignment.MiddleRight,
                    TextAlign = ContentAlignment.MiddleRight,
                    ForeColor = Color.LightGray,
                    AutoSize = true,
                };
                checkBox.CheckedChanged += (_, args) => dialogue.ShowGlobalResponses = checkBox.Checked;
                PropertyInspector.Controls.Add(checkBox);

                checkBox = new()
                {
                    Checked = dialogue.ShowGlobalGoodByeResponses,
                    Dock = DockStyle.Fill,
                    Text = "Use goodbye responses:",
                    CheckAlign = ContentAlignment.MiddleRight,
                    TextAlign = ContentAlignment.MiddleRight,
                    ForeColor = Color.LightGray,
                    AutoSize = true,
                };
                checkBox.CheckedChanged += (_, args) => dialogue.ShowGlobalGoodByeResponses = checkBox.Checked;
                PropertyInspector.Controls.Add(checkBox);

                checkBox = new()
                {
                    Checked = dialogue.IsDynamic,
                    Dock = DockStyle.Fill,
                    Text = "Auto Immersive:",
                    CheckAlign = ContentAlignment.MiddleRight,
                    TextAlign = ContentAlignment.MiddleRight,
                    ForeColor = Color.LightGray,
                    AutoSize = true,
                };
                checkBox.CheckedChanged += (_, args) => dialogue.IsDynamic = checkBox.Checked;
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
                text.TextChanged += (_, _) => dialogue.Text = text.Text;
                text.Select();
                PropertyInspector.RowStyles[0].SizeType = SizeType.Absolute;
                PropertyInspector.RowStyles[0].Height = 35;
                PropertyInspector.Controls.Add(text, 0, 1);
                PropertyInspector.SetColumnSpan(text, 6);

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
                sortOrder.ValueChanged += (_, _) => alternate.Order = (int)sortOrder.Value;
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
                text.TextChanged += (_, _) => alternate.Text = text.Text;
                text.Select();
                PropertyInspector.RowStyles[0].SizeType = SizeType.Absolute;
                PropertyInspector.RowStyles[0].Height = 35;
                PropertyInspector.Controls.Add(text, 0, 1);
                PropertyInspector.SetColumnSpan(text, 3);
                break;
            }
            case NodeType.Event:
            {
                if (node.Data?.GetType() != typeof(GameEvent))
                {
                    Label label2 = new()
                    {
                        Text = "No data on this node",
                        TextAlign = ContentAlignment.MiddleLeft,
                        Dock = DockStyle.Top,
                        ForeColor = Color.LightGray,
                        Height = 30,
                    };
                    PropertyInspector.Controls.Add(label2);
                    break;
                }

                PropertyInspector.RowCount = 2;
                PropertyInspector.ColumnCount = 5;

                GameEvent gevent = (GameEvent)node.Data;

                Label label = new()
                {
                    Text = "Order:",
                    TextAlign = ContentAlignment.TopRight,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.LightGray,
                };
                PropertyInspector.Controls.Add(label, 0, 0);

                NumericUpDown sortOrder = new()
                {
                    //Dock = DockStyle.Fill,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                    Location = new(0, PropertyInspector.Size.Height / 2),
                    Value = gevent.SortOrder,
                    Dock = DockStyle.Left,
                    Width = 50
                };
                sortOrder.ValueChanged += (_, _) => gevent.SortOrder = (int)sortOrder.Value;
                PropertyInspector.Controls.Add(sortOrder, 1, 0);

                ComboBox type = GetComboBox();
                type.Items.AddRange(Enum.GetNames(typeof(GameEvents)));
                type.SelectedItem = gevent.EventType.ToString();
                type.SelectedIndexChanged += (_, _) => gevent.EventType = Enum.Parse<GameEvents>(type.SelectedItem.ToString()!);
                PropertyInspector.Controls.Add(type, 2, 0);

                Label label3 = new()
                {
                    Text = "Delay:",
                    TextAlign = ContentAlignment.TopRight,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.LightGray,
                };
                PropertyInspector.Controls.Add(label3, 3, 0);

                NumericUpDown delay = new()
                {
                    //Dock = DockStyle.Fill,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                    Location = new(0, PropertyInspector.Size.Height / 2),
                    Value = (decimal)gevent.Delay,
                    Dock = DockStyle.Left,
                    DecimalPlaces = 2,
                };
                delay.ValueChanged += (_, _) => gevent.Delay = (double)delay.Value;
                PropertyInspector.Controls.Add(delay, 4, 0);

                switch (gevent.EventType)
                {
                    case GameEvents.AddForce:
                    {
                        ComboBox characters = GetComboBox();
                        characters.Items.AddRange(Enum.GetNames(typeof(Characters)));
                        characters.SelectedItem = gevent.Value!;
                        characters.SelectedIndexChanged += (_, _) => gevent.Value = characters.SelectedItem.ToString()!;
                        PropertyInspector.Controls.Add(characters);

                        //todo fill other event types
                        break;
                    }
                    case GameEvents.AllowPlayerSave:
                        break;
                    case GameEvents.ChangeBodyScale:
                        break;
                    case GameEvents.CharacterFromCharacterGroup:
                        break;
                    case GameEvents.CharacterFunction:
                        break;
                    case GameEvents.Clothing:
                        break;
                    case GameEvents.Combat:
                        break;
                    case GameEvents.CombineValue:
                        break;
                    case GameEvents.CutScene:
                        break;
                    case GameEvents.Dialogue:
                        break;
                    case GameEvents.DisableNPC:
                        break;
                    case GameEvents.DisplayGameMessage:
                        break;
                    case GameEvents.Door:
                        break;
                    case GameEvents.Emote:
                        break;
                    case GameEvents.EnableNPC:
                        break;
                    case GameEvents.EventTriggers:
                        break;
                    case GameEvents.FadeIn:
                        break;
                    case GameEvents.FadeOut:
                        break;
                    case GameEvents.IKReach:
                        break;
                    case GameEvents.Intimacy:
                        break;
                    case GameEvents.Item:
                        break;
                    case GameEvents.ItemFromItemGroup:
                        break;
                    case GameEvents.LookAt:
                        break;
                    case GameEvents.Personality:
                        break;
                    case GameEvents.Property:
                        break;
                    case GameEvents.MatchValue:
                        break;
                    case GameEvents.ModifyValue:
                        break;
                    case GameEvents.Player:
                        break;
                    case GameEvents.PlaySoundboardClip:
                        break;
                    case GameEvents.Pose:
                        break;
                    case GameEvents.Quest:
                        break;
                    case GameEvents.RandomizeIntValue:
                        break;
                    case GameEvents.ResetReactionCooldown:
                        break;
                    case GameEvents.Roaming:
                        break;
                    case GameEvents.SendEvent:
                        break;
                    case GameEvents.SetPlayerPref:
                        break;
                    case GameEvents.Social:
                        break;
                    case GameEvents.State:

                        break;
                    case GameEvents.TriggerBGC:
                        break;
                    case GameEvents.Turn:
                        break;
                    case GameEvents.TurnInstantly:
                        break;
                    case GameEvents.UnlockAchievement:
                        break;
                    case GameEvents.WalkTo:
                        break;
                    case GameEvents.WarpOverTime:
                        break;
                    case GameEvents.WarpTo:
                        break;
                    case GameEvents.None:
                        break;
                    default:
                        break;
                }

                break;
            }
            case NodeType.EventTrigger:
            {
                if (node.Data?.GetType() != typeof(EventTrigger))
                {
                    Label label2 = new()
                    {
                        Text = "No data on this node",
                        TextAlign = ContentAlignment.MiddleLeft,
                        Dock = DockStyle.Top,
                        ForeColor = Color.LightGray,
                        Height = 30
                    };
                    PropertyInspector.Controls.Add(label2);
                    break;
                }
                EventTrigger eventTrigger = ((EventTrigger)node.Data!);

                PropertyInspector.RowCount = 2;

                Label label = new()
                {
                    Text = "Name:",
                    TextAlign = ContentAlignment.TopRight,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.LightGray,
                    Height = 30,
                };
                PropertyInspector.Controls.Add(label, 0, 0);

                TextBox customName = new()
                {
                    Text = eventTrigger.Name,
                    Multiline = true,
                    WordWrap = true,
                    ScrollBars = ScrollBars.Both,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.LightGray,
                    BackColor = Color.FromArgb(255, 50, 50, 50),
                };
                customName.TextChanged += (_, _) => eventTrigger.Name = customName.Text;
                PropertyInspector.Controls.Add(customName, 1, 0);
                PropertyInspector.SetColumnSpan(customName, 4);

                switch (eventTrigger.Type)
                {
                    case EventTypes.CaughtHavingSex:
                    case EventTypes.CaughtMasturbating:
                    case EventTypes.Dies:
                    case EventTypes.EjaculatesOnMe:
                    case EventTypes.EntersVicinity:
                    case EventTypes.EntersVision:
                    case EventTypes.ExitsVision:
                    case EventTypes.ExposesChest:
                    case EventTypes.ExposesGenitals:
                    case EventTypes.FallsOver:
                    case EventTypes.FinishedPopulatingMainDialogueText:
                    case EventTypes.GetsKnockedOut:
                    case EventTypes.GropesMyAss:
                    case EventTypes.GropesMyBreast:
                    case EventTypes.ImpactsGround:
                    case EventTypes.ImpactsWall:
                    case EventTypes.IsBottomless:
                    case EventTypes.IsDancing:
                    case EventTypes.IsNaked:
                    case EventTypes.IsTopless:
                    case EventTypes.Orgasms:
                    case EventTypes.PeesOnMe:
                    case EventTypes.PhoneBlindedMe:
                    case EventTypes.PopperedMe:
                    case EventTypes.ScoredBeerPongPoint:
                    case EventTypes.StartedLapDance:
                    case EventTypes.StartedPeeing:
                    case EventTypes.CombatModeToggled:
                    case EventTypes.StoppedPeeing:
                    case EventTypes.VapesOnMe:
                    case EventTypes.OnAnyItemAcceptFallback:
                    case EventTypes.OnAnyItemRefuseFallback:
                    {
                        PutCharacter(eventTrigger);
                        PutStartCondition(node, eventTrigger);
                        break;
                    }
                    case EventTypes.EntersZone:
                    {
                        PutCharacter(eventTrigger);
                        PutStartCondition(node, eventTrigger);

                        ComboBox zone = GetComboBox();
                        zone.Items.AddRange(Enum.GetNames(typeof(ZoneEnums)));
                        zone.SelectedItem = eventTrigger.Value;
                        zone.PerformLayout();
                        zone.SelectedIndexChanged += (_, _) => eventTrigger.Value = (string)zone.SelectedItem!;
                        PropertyInspector.Controls.Add(zone);

                        break;
                    }
                    case EventTypes.ReachesTarget:
                    {
                        PutCharacter(eventTrigger);
                        PutStartCondition(node, eventTrigger);

                        ComboBox targetType = GetComboBox();
                        targetType.Items.AddRange(Enum.GetNames(typeof(LocationTargetOption)));
                        targetType.SelectedItem = eventTrigger.LocationTargetOption.ToString();
                        targetType.PerformLayout();
                        targetType.SelectedIndexChanged += (_, _) => eventTrigger.LocationTargetOption = Enum.Parse<LocationTargetOption>(targetType.SelectedItem!.ToString()!);
                        targetType.SelectedIndexChanged += (_, _) => ShowProperties(node);
                        PropertyInspector.Controls.Add(targetType);

                        switch (eventTrigger.LocationTargetOption)
                        {
                            case LocationTargetOption.MoveTarget:
                            {
                                ComboBox target = GetComboBox();
                                target.Items.AddRange(Enum.GetNames(typeof(MoveTargets)));
                                target.SelectedItem = eventTrigger.Value!.Replace(" ", "");
                                target.PerformLayout();
                                target.SelectedIndexChanged += (_, _) => eventTrigger.Value = target.SelectedItem!.ToString()!;
                                PropertyInspector.Controls.Add(target);
                                break;
                            }
                            case LocationTargetOption.Character:
                            {
                                PutCharacterValue(eventTrigger);
                                break;
                            }
                            case LocationTargetOption.Item:
                            {
                                PutItemValue(eventTrigger);
                                break;
                            }
                            default:
                                break;
                        }

                        break;
                    }
                    case EventTypes.IsBlockedByLockedDoor:
                    {
                        PutCharacter(eventTrigger);
                        PutStartCondition(node, eventTrigger);

                        TextBox door = new()
                        {
                            Text = eventTrigger.Value,
                            Multiline = true,
                            WordWrap = true,
                            ScrollBars = ScrollBars.Both,
                            Dock = DockStyle.Fill,
                            ForeColor = Color.LightGray,
                            BackColor = Color.FromArgb(255, 50, 50, 50),
                        };
                        door.TextChanged += (_, _) => eventTrigger.Value = door.Text;
                        PropertyInspector.Controls.Add(door);

                        break;
                    }
                    case EventTypes.IsAttacked:
                    {
                        PutCharacter(eventTrigger);
                        PutStartCondition(node, eventTrigger);
                        PutCharacterValue(eventTrigger);
                        break;
                    }
                    case EventTypes.GetsHitWithProjectile:
                    {
                        PutCharacter(eventTrigger);
                        PutStartCondition(node, eventTrigger);
                        PutItemKey(eventTrigger);

                        Label inlabel = new()
                        {
                            Text = "in the:",
                            TextAlign = ContentAlignment.MiddleRight,
                            Dock = DockStyle.Top,
                            ForeColor = Color.LightGray,
                            Height = 30,
                        };
                        PropertyInspector.Controls.Add(inlabel);

                        ComboBox zone = GetComboBox();
                        zone.Items.AddRange(Enum.GetNames(typeof(BodyRegion)));
                        zone.SelectedItem = ((BodyRegion)int.Parse(eventTrigger.Value!)).ToString();
                        zone.PerformLayout();
                        zone.SelectedIndexChanged += (_, _) => eventTrigger.Value = ((int)Enum.Parse<BodyRegion>(zone.SelectedItem!.ToString()!)).ToString();
                        PropertyInspector.Controls.Add(zone);

                        break;
                    }
                    case EventTypes.StartedIntimacyAct:
                    {
                        //todo
                        break;
                    }
                    case EventTypes.PlayerGrabsItem:
                    {
                        //todo
                        break;
                    }
                    case EventTypes.PlayerReleasesItem:
                    {
                        //todo
                        break;
                    }
                    case EventTypes.Periodically:
                    {
                        PutStartCondition(node, eventTrigger);

                        Label inlabel = new()
                        {
                            Text = "every:",
                            TextAlign = ContentAlignment.MiddleRight,
                            Dock = DockStyle.Top,
                            ForeColor = Color.LightGray,
                            Height = 30,
                        };
                        PropertyInspector.Controls.Add(inlabel);

                        NumericUpDown option = new()
                        {
                            //Dock = DockStyle.Fill,
                            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                            Location = new(0, PropertyInspector.Size.Height / 2),
                            Dock = DockStyle.Left,
                            Width = 50,
                            Value = (decimal)eventTrigger.UpdateIteration
                        };
                        option.PerformLayout();
                        option.ValueChanged += (_, _) => eventTrigger.UpdateIteration = (double)option.Value;
                        PropertyInspector.Controls.Add(option);

                        Label seconds = new()
                        {
                            Text = "seconds",
                            TextAlign = ContentAlignment.MiddleRight,
                            Dock = DockStyle.Top,
                            ForeColor = Color.LightGray,
                            Height = 30,
                        };
                        PropertyInspector.Controls.Add(seconds);
                        break;
                    }
                    case EventTypes.OnItemFunction:
                    {
                        //todo
                        break;
                    }
                    case EventTypes.PokedByVibrator:
                    {
                        //todo
                        break;
                    }
                    case EventTypes.PeesOnItem:
                    {
                        PutCharacter(eventTrigger);
                        PutStartCondition(node, eventTrigger);
                        PutItemValue(eventTrigger);
                        break;
                    }
                    case EventTypes.PlayerThrowsItem:
                    {
                        PutCharacter(eventTrigger);
                        PutStartCondition(node, eventTrigger);
                        PutItemValue(eventTrigger);
                        break;
                    }
                    case EventTypes.StartedUsingActionItem:
                    case EventTypes.StoppedUsingActionItem:
                    {
                        //todo
                        break;
                    }
                    case EventTypes.OnFriendshipIncreaseWith:
                    case EventTypes.OnFriendshipDecreaseWith:
                    case EventTypes.OnRomanceDecreaseWith:
                    case EventTypes.OnRomanceIncreaseWith:
                    case EventTypes.PlayerInteractsWithCharacter:
                    {
                        PutStartCondition(node, eventTrigger);
                        PutCharacter(eventTrigger);
                        break;
                    }
                    case EventTypes.PlayerInteractsWithItem:
                    {
                        //todo
                        break;
                    }
                    case EventTypes.OnAfterCutSceneEnds:
                    {
                        PutStartCondition(node, eventTrigger);

                        ComboBox cutscene = GetComboBox();
                        cutscene.Items.AddRange(Enum.GetNames(typeof(Cutscenes)));
                        cutscene.SelectedItem = eventTrigger.Value;
                        cutscene.PerformLayout();
                        cutscene.SelectedIndexChanged += (_, _) => eventTrigger.Value = cutscene.SelectedItem!.ToString();
                        PropertyInspector.Controls.Add(cutscene);

                        break;
                    }
                    default:
                    case EventTypes.GameStarts:
                    case EventTypes.Ejaculates:
                    case EventTypes.PlayerInventoryOpened:
                    case EventTypes.PlayerInventoryClosed:
                    case EventTypes.PlayerTookCameraPhoto:
                    case EventTypes.PlayerOpportunityWindowOpened:
                    case EventTypes.Never:
                    case EventTypes.None:
                    case EventTypes.OnScreenFadeInComplete:
                    case EventTypes.OnScreenFadeOutComplete:
                    {
                        PutStartCondition(node, eventTrigger);
                        break;
                    }
                }

                break;
            }
            case NodeType.Response:
            {
                PropertyInspector.RowCount = 2;
                if (node.Data?.GetType() != typeof(Response))
                {
                    Label label2 = new()
                    {
                        Text = "No data on this node",
                        TextAlign = ContentAlignment.MiddleLeft,
                        Dock = DockStyle.Top,
                        ForeColor = Color.LightGray,
                        Height = 30
                    };
                    PropertyInspector.Controls.Add(label2);
                    break;
                }
                Response response = ((Response)node.Data!);

                PropertyInspector.ColumnCount = 6;

                Label label = new()
                {
                    Text = "Trigger New Dialogue:\n\nResponse Text:",
                    TextAlign = ContentAlignment.MiddleRight,
                    Dock = DockStyle.Top,
                    ForeColor = Color.LightGray,
                    Height = 49,
                    Width = 100
                };
                PropertyInspector.Controls.Add(label);

                ComboBox dialogue = GetComboBox();
                dialogue.Items.Add("Do not trigger new dialogue");
                for (int i = 0; i < characterStories[node.FileName].Dialogues!.Count; i++)
                {
                    dialogue.Items.Add(characterStories[node.FileName].Dialogues![i].ID.ToString());
                }
                dialogue.SelectedItem = response.Next.ToString()!;
                dialogue.PerformLayout();
                dialogue.SelectedIndexChanged += (_, _) =>
                {
                    if (int.TryParse(dialogue.SelectedItem!.ToString()!, out int res))
                    {
                        response.Next = res;
                    }
                    else
                    {
                        response.Next = 0;
                    }
                    ShowProperties(node);
                };
                PropertyInspector.Controls.Add(dialogue);

                Label label3 = new()
                {
                    Text = characterStories[node.FileName].Dialogues!.Find((dialog) => dialog.ID.ToString() == dialogue.SelectedItem?.ToString())?.Text ?? "No text on dialogue",
                    TextAlign = ContentAlignment.TopLeft,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.LightGray,
                };
                PropertyInspector.Controls.Add(label3);

                Label label4 = new()
                {
                    Text = "Display Order",
                    TextAlign = ContentAlignment.MiddleRight,
                    Dock = DockStyle.Top,
                    ForeColor = Color.LightGray,
                };
                PropertyInspector.Controls.Add(label4);

                NumericUpDown option = new()
                {
                    //Dock = DockStyle.Fill,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                    Location = new(0, PropertyInspector.Size.Height / 2),
                    Value = response.Order,
                    Dock = DockStyle.Left,
                    Width = 50
                };
                option.PerformLayout();
                option.ValueChanged += (_, _) => response.Order = (int)option.Value;
                PropertyInspector.Controls.Add(option);

                CheckBox checkBox = new()
                {
                    Checked = response.AlwaysDisplay,
                    Dock = DockStyle.Top,
                    Text = "Always Available:",
                    CheckAlign = ContentAlignment.MiddleRight,
                    TextAlign = ContentAlignment.MiddleRight,
                    ForeColor = Color.LightGray,
                    AutoSize = true,
                };
                checkBox.CheckedChanged += (_, args) => response.AlwaysDisplay = checkBox.Checked;
                PropertyInspector.Controls.Add(checkBox);

                TextBox text = new()
                {
                    Text = response.Text,
                    Multiline = true,
                    WordWrap = true,
                    ScrollBars = ScrollBars.Both,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.LightGray,
                    BackColor = Color.FromArgb(255, 50, 50, 50),
                };
                text.TextChanged += (_, _) => response.Text = text.Text;
                text.Select();
                PropertyInspector.RowStyles[0].SizeType = SizeType.Absolute;
                PropertyInspector.RowStyles[0].Height = 50;
                PropertyInspector.Controls.Add(text, 0, 1);
                PropertyInspector.SetColumnSpan(text, 6);
                break;
            }
            case NodeType.Value:
            {
                Label label = new()
                {
                    Text = node.FileName + "'s value:",
                    TextAlign = ContentAlignment.MiddleRight,
                    Dock = DockStyle.Top,
                    ForeColor = Color.LightGray,
                    Height = 30,
                };
                PropertyInspector.Controls.Add(label);

                if (node.Data?.GetType() == typeof(Value))
                {
                    TextBox obj2 = new()
                    {
                        Dock = DockStyle.Fill,
                        TextAlign = HorizontalAlignment.Center,
                        ForeColor = Color.LightGray,
                        Text = ((Value)node.Data).value,
                        AutoSize = true,
                    };
                    obj2.TextChanged += (_, args) => ((Value)node.Data).value = obj2.Text;
                    PropertyInspector.Controls.Add(obj2);
                }
                else
                {
                    Label label2 = new()
                    {
                        Text = "No data on this node!",
                        TextAlign = ContentAlignment.MiddleLeft,
                        Dock = DockStyle.Top,
                        ForeColor = Color.LightGray,
                        Height = 30
                    };
                    PropertyInspector.Controls.Add(label2);
                }
                break;
            }
            case NodeType.Personality:
            {
                Label label = new()
                {
                    Text = node.FileName + "'s " + node.ID,
                    TextAlign = ContentAlignment.MiddleRight,
                    Dock = DockStyle.Top,
                    ForeColor = Color.LightGray,
                    Height = 30
                };
                PropertyInspector.Controls.Add(label);

                if (node.Data?.GetType() == typeof(Trait))
                {
                    NumericUpDown option = new()
                    {
                        Dock = DockStyle.Top,
                        ForeColor = Color.LightGray,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                        Location = new(0, PropertyInspector.Size.Height / 2),
                        Value = ((Trait)(node.Data)).Value,
                        Maximum = 100,
                        Minimum = -100,
                        Width = 50
                    };
                    option.PerformLayout();
                    option.ValueChanged += (_, _) => ((Trait)(node.Data)).Value = (int)option.Value;
                    PropertyInspector.Controls.Add(option);
                }
                else
                {
                    Label label2 = new()
                    {
                        Text = "No data on this node!",
                        TextAlign = ContentAlignment.MiddleLeft,
                        Dock = DockStyle.Top,
                        ForeColor = Color.LightGray,
                        Height = 30
                    };
                    PropertyInspector.Controls.Add(label2);
                }
                break;
            }
            case NodeType.State:
            case NodeType.Property:
            case NodeType.Social:
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
            case NodeType.Pose:
            case NodeType.Quest:
            default:
            {
                PropertyInspector.RowCount = 1;
                PropertyInspector.ColumnCount = 3;
                Label label = new()
                {
                    Text = node.Type.ToString(),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Top,
                    ForeColor = Color.LightGray,
                    AutoSize = true,
                };
                PropertyInspector.Controls.Add(label);
                label = new()
                {
                    Text = node.ID,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Top,
                    ForeColor = Color.LightGray,
                    AutoSize = true,
                };
                PropertyInspector.Controls.Add(label);
                label = new()
                {
                    Text = node.Text,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Top,
                    ForeColor = Color.LightGray,
                    AutoSize = true,
                };
                PropertyInspector.Controls.Add(label);
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
    }

    private void PutItemValue(EventTrigger eventTrigger)
    {
        ComboBox item = GetComboBox();
        item.Items.AddRange(Enum.GetNames(typeof(Items)));
        item.SelectedItem = eventTrigger.Value;
        item.PerformLayout();
        item.SelectedIndexChanged += (_, _) => eventTrigger.Value = item.SelectedItem!.ToString()!;
        PropertyInspector.Controls.Add(item);
    }

    private void PutItemKey(EventTrigger eventTrigger)
    {
        ComboBox item = GetComboBox();
        item.Items.AddRange(Enum.GetNames(typeof(Items)));
        item.SelectedItem = eventTrigger.Key;
        item.PerformLayout();
        item.SelectedIndexChanged += (_, _) => eventTrigger.Key = item.SelectedItem!.ToString()!;
        PropertyInspector.Controls.Add(item);
    }

    private void PutCharacterValue(EventTrigger eventTrigger)
    {
        ComboBox character = GetComboBox();
        character.Items.AddRange(Enum.GetNames(typeof(AnybodyCharacters)));
        character.SelectedItem = eventTrigger.Value;
        character.PerformLayout();
        character.SelectedIndexChanged += (_, _) => eventTrigger.Value = character.SelectedItem!.ToString();
        PropertyInspector.Controls.Add(character);
    }

    private ComboBox PutCharacter(EventTrigger eventTrigger)
    {
        ComboBox character = GetComboBox();
        character.Items.AddRange(Enum.GetNames(typeof(AnybodyCharacters)));
        character.SelectedItem = eventTrigger.CharacterToReactTo;
        character.PerformLayout();
        character.SelectedIndexChanged += (_, _) => eventTrigger.CharacterToReactTo = character.SelectedItem!.ToString();
        PropertyInspector.Controls.Add(character);
        return character;
    }

    private void PutStartCondition(Node node, EventTrigger eventTrigger)
    {
        Label label3 = new()
        {
            Text = "Trigger when:",
            TextAlign = ContentAlignment.TopRight,
            Dock = DockStyle.Top,
            ForeColor = Color.LightGray,
            Height = 30,
        };
        PropertyInspector.Controls.Add(label3);

        ComboBox startCondition = GetComboBox();
        startCondition.Items.AddRange(Enum.GetNames<EventTypes>());
        startCondition.SelectedItem = eventTrigger.Type.ToString()!;
        startCondition.PerformLayout();
        startCondition.SelectedIndexChanged += (_, _) =>
        {
            if (Enum.TryParse(startCondition.SelectedItem!.ToString()!, out EventTypes res))
            {
                eventTrigger.Type = res;
            }
            else
            {
                eventTrigger.Type = EventTypes.None;
            }
            ShowProperties(node);
        };
        PropertyInspector.Controls.Add(startCondition);
    }

    private void PutTextValue(Criterion criterion)
    {
        TextBox obj2 = new()
        {
            Dock = DockStyle.Fill,
            TextAlign = HorizontalAlignment.Center,
            Text = criterion.Value
        };
        obj2.TextChanged += (_, args) => criterion.Value = obj2.Text;
        PropertyInspector.Controls.Add(obj2);
    }

    private void PutNumericOption(Criterion criterion)
    {
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
        option.ValueChanged += (_, _) => criterion.Option = (int)option.Value;
        PropertyInspector.Controls.Add(option);
    }

    private void PutNumericValue(Criterion criterion)
    {
        NumericUpDown option = new()
        {
            //Dock = DockStyle.Fill,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
            Location = new(0, PropertyInspector.Size.Height / 2),
            Dock = DockStyle.Left,
            Width = 50
        };
        if (int.TryParse(criterion.Value!, out int res))
        {
            option.Value = res;
        }
        else
        {
            option.Value = 0;
        }
        option.PerformLayout();
        option.ValueChanged += (_, _) => criterion.Value = option.Value.ToString();
        PropertyInspector.Controls.Add(option);
    }

    private void PutComparison(Criterion criterion)
    {
        ComboBox equ = GetComboBox();
        equ.Items.AddRange(Enum.GetNames(typeof(ComparisonEquations)));
        if (int.TryParse(criterion.Value!, out int res))
        {
            equ.SelectedIndex = res;
        }
        else
        {
            equ.SelectedIndex = 0;
        }
        equ.Select(equ.SelectedItem?.ToString()?.Length ?? 0, 0);
        equ.PerformLayout();
        equ.SelectedIndexChanged += (_, _) => criterion.Value = (equ.SelectedIndex).ToString();
        PropertyInspector.Controls.Add(equ);
    }

    private void PutZone(Criterion criterion)
    {
        ComboBox zone = GetComboBox();
        zone.Items.AddRange(Enum.GetNames(typeof(ZoneEnums)));
        zone.SelectedItem = criterion.Key;
        zone.SelectionLength = 0;
        zone.SelectionStart = 0;
        zone.PerformLayout();
        zone.SelectedIndexChanged += (_, _) => criterion.Key = (string)zone.SelectedItem!;
        PropertyInspector.Controls.Add(zone);
    }

    private void PutCompareType(Criterion criterion, Node node)
    {
        ComboBox compareType = GetComboBox();
        compareType.Items.AddRange(Enum.GetNames(typeof(CompareTypes)));
        compareType.SelectedItem = criterion.CompareType.ToString();
        compareType.SelectedText = string.Empty;
        compareType.SelectionLength = 0;
        compareType.SelectionStart = 0;
        compareType.PerformLayout();
        compareType.SelectedIndexChanged += (_, _) => criterion.CompareType = Enum.Parse<CompareTypes>((string)compareType.SelectedItem!);
        compareType.SelectedIndexChanged += (_, _) => ShowProperties(node);
        PropertyInspector.Controls.Add(compareType);
    }

    private ComboBox PutCharacter1(Criterion criterion)
    {
        ComboBox compareType = GetComboBox();
        compareType.Items.AddRange(Enum.GetNames(typeof(Characters)));
        compareType.SelectedItem = criterion.Character;
        compareType.SelectionLength = 0;
        compareType.SelectionStart = 0;
        compareType.PerformLayout();
        compareType.SelectedIndexChanged += (_, _) => criterion.Character = (string)compareType.SelectedItem!;
        PropertyInspector.Controls.Add(compareType);
        return compareType;
    }

    private void PutCharacter2(Criterion criterion)
    {
        ComboBox compareType = GetComboBox();
        compareType.Items.AddRange(Enum.GetNames(typeof(Characters)));
        compareType.SelectedItem = criterion.Character2;
        compareType.SelectedText = string.Empty;
        compareType.SelectionLength = 0;
        compareType.SelectionStart = 0;
        compareType.PerformLayout();
        compareType.SelectedIndexChanged += (_, _) => criterion.Character2 = (string)compareType.SelectedItem!;
        PropertyInspector.Controls.Add(compareType);
    }

    private void PutBoolValue(Criterion criterion)
    {
        ComboBox boolValue = GetComboBox();
        boolValue.Items.AddRange(Enum.GetNames(typeof(BoolCritera)));
        boolValue.SelectedItem = criterion.BoolValue!.ToString();
        boolValue.SelectionLength = 0;
        boolValue.SelectionStart = 0;
        boolValue.PerformLayout();
        boolValue.SelectedIndexChanged += (_, _) => criterion.BoolValue = Enum.Parse<BoolCritera>(boolValue.SelectedItem!.ToString()!);
        PropertyInspector.Controls.Add(boolValue);
    }

    private void PutItem(Criterion criterion)
    {
        ComboBox item = GetComboBox();
        item.Items.AddRange(Enum.GetNames<Items>());
        item.SelectedItem = criterion.Key!.Replace(" ", "");
        item.PerformLayout();
        item.SelectedIndexChanged += (_, _) => criterion.Key = item.SelectedItem!.ToString();
        PropertyInspector.Controls.Add(item);
    }

    private void PutEquals(Criterion criterion)
    {
        //todo make one single and shared new constructor for getting this type of combobox...
        ComboBox equals = GetComboBox();
        equals.Items.AddRange(Enum.GetNames(typeof(EqualsValues)));
        equals.SelectedIndex = (int)criterion.EqualsValue!;
        equals.PerformLayout();
        equals.SelectedIndexChanged += (_, _) => criterion.EqualsValue = (EqualsValues?)equals.SelectedIndex;
        PropertyInspector.Controls.Add(equals);
    }

    private static ComboBox GetComboBox() => new()
    {
        Dock = DockStyle.Fill,
        AutoCompleteMode = AutoCompleteMode.SuggestAppend,
        AutoCompleteSource = AutoCompleteSource.ListItems,
        DropDownStyle = ComboBoxStyle.DropDownList,
    };

    private void UpdateHighlightNode(PointF ScreenPos)
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
            SelectedCharacter = e.Node.Text;
            Graph.Invalidate();
        }
    }

    private void SpawnNodeFromSpaceSpawner(object sender, EventArgs e)
    {
        SpawnableNodeType selectedType = Enum.Parse<SpawnableNodeType>(NodeSpawnBox.SelectedItem?.ToString()!);

        if (!NodeSpawnBox.Enabled)
        {
            return;
        }
        NodeSpawnBox.Enabled = false;
        NodeSpawnBox.Visible = false;

        Node newNode = Node.NullNode;

        string character = SelectedCharacter;

        if (clickedNode != Node.NullNode)
        {
            character = clickedNode.FileName;
        }

        switch (selectedType)
        {
            case SpawnableNodeType.Criterion:
            {
                string id = Guid.NewGuid().ToString();
                newNode = new Node(id, NodeType.Criterion, string.Empty, nodes[character].Positions)
                {
                    Data = new Criterion() { Character = character },
                    DataType = typeof(Criterion),
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    if (clickedNode.DataType == typeof(ItemAction))
                    {
                        ((ItemAction)clickedNode.Data!).Criteria.Add((Criterion)newNode.Data);
                        nodes[character].AddParent(clickedNode, newNode);
                    }
                    else if (clickedNode.DataType == typeof(UseWith))
                    {
                        ((UseWith)clickedNode.Data!).Criteria.Add((Criterion)newNode.Data);
                        nodes[character].AddParent(clickedNode, newNode);
                    }
                    else if (clickedNode.DataType == typeof(CriteriaList1))
                    {
                        ((CriteriaList1)clickedNode.Data!).CriteriaList.Add((Criterion)newNode.Data);
                        nodes[character].AddParent(clickedNode, newNode);
                    }
                    else if (clickedNode.DataType == typeof(GameEvent))
                    {
                        ((GameEvent)clickedNode.Data!).Criteria.Add((Criterion)newNode.Data);
                        nodes[character].AddParent(clickedNode, newNode);
                    }
                    else if (clickedNode.DataType == typeof(EventTrigger))
                    {
                        ((EventTrigger)clickedNode.Data!).Critera.Add((Criterion)newNode.Data);
                        nodes[character].AddParent(clickedNode, newNode);
                    }
                    else if (clickedNode.DataType == typeof(AlternateText))
                    {
                        ((AlternateText)clickedNode.Data!).Critera.Add((Criterion)newNode.Data);
                        nodes[character].AddParent(clickedNode, newNode);
                    }
                    else if (clickedNode.DataType == typeof(Response))
                    {
                        ((Response)clickedNode.Data!).ResponseCriteria.Add((Criterion)newNode.Data);
                        nodes[character].AddParent(clickedNode, newNode);
                    }
                    else if (clickedNode.DataType == typeof(Dialogue))
                    {
                        ((Dialogue)clickedNode.Data!).DynamicDialogueCriteria.Add((Criterion)newNode.Data);
                        nodes[character].AddParent(clickedNode, newNode);
                    }
                    else if (clickedNode.DataType == typeof(BackgroundChatter))
                    {
                        ((BackgroundChatter)clickedNode.Data!).Critera.Add((Criterion)newNode.Data);
                        nodes[character].AddParent(clickedNode, newNode);
                    }
                    else if (clickedNode.DataType == typeof(ItemInteraction))
                    {
                        ((ItemInteraction)clickedNode.Data!).Critera.Add((Criterion)newNode.Data);
                        nodes[character].AddParent(clickedNode, newNode);
                    }
                    else if (clickedNode.DataType == typeof(ItemGroupInteraction))
                    {
                        ((ItemGroupInteraction)clickedNode.Data!).Critera.Add((Criterion)newNode.Data);
                        nodes[character].AddParent(clickedNode, newNode);
                    }
                }

                break;
            }
            case SpawnableNodeType.ItemAction:
            {
                string id = "action";
                newNode = new Node(id, NodeType.ItemAction, string.Empty, nodes[character].Positions)
                {
                    Data = new ItemAction() { ActionName = id },
                    DataType = typeof(ItemAction),
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    if (clickedNode.DataType == typeof(Criterion))
                    {
                        ((ItemAction)newNode.Data!).Criteria.Add((Criterion)clickedNode.Data!);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(GameEvent))
                    {
                        ((ItemAction)newNode.Data!).OnTakeActionEvents.Add((GameEvent)clickedNode.Data!);
                        nodes[character].AddChild(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(ItemOverride))
                    {
                        ((ItemOverride)clickedNode.Data!).ItemActions.Add((ItemAction)newNode.Data!);
                        nodes[character].AddChild(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(ItemGroupBehavior))
                    {
                        ((ItemGroupBehavior)clickedNode.Data!).ItemActions.Add((ItemAction)newNode.Data!);
                        nodes[character].AddChild(newNode, clickedNode);
                    }
                }

                break;
            }
            case SpawnableNodeType.Achievement:
            {
                var guid = Guid.NewGuid().ToString();
                newNode = new Node(guid, NodeType.Achievement, string.Empty, nodes[character].Positions)
                {
                    Data = new Achievement() { Id = guid },
                    DataType = typeof(Achievement),
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    if (clickedNode.DataType == typeof(GameEvent))
                    {
                        GameEvent gameEvent = ((GameEvent)clickedNode.Data!);
                        gameEvent.Value = ((Achievement)newNode.Data!).Id;
                        gameEvent.EventType = GameEvents.UnlockAchievement;
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                }

                break;
            }
            case SpawnableNodeType.BGC:
            {
                newNode = new Node("BGC" + (characterStories[character].BackgroundChatter!.Count + 1).ToString(), NodeType.BGC, string.Empty, nodes[character].Positions)
                {
                    Data = new BackgroundChatter() { Id = characterStories[character].BackgroundChatter!.Count + 1 },
                    DataType = typeof(BackgroundChatter),
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    if (clickedNode.DataType == typeof(Criterion))
                    {
                        ((BackgroundChatter)newNode.Data!).Critera.Add((Criterion)clickedNode.Data!);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(GameEvent))
                    {
                        ((BackgroundChatter)newNode.Data!).StartEvents.Add((GameEvent)clickedNode.Data!);
                        nodes[character].AddChild(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(BackgroundChatterResponse))
                    {
                        ((BackgroundChatter)newNode.Data!).Responses.Add((BackgroundChatterResponse)clickedNode.Data!);
                        nodes[character].AddChild(newNode, clickedNode);
                    }
                }

                break;
            }
            case SpawnableNodeType.BGCResponse:
            {
                newNode = new Node(Guid.NewGuid().ToString(), NodeType.BGCResponse, string.Empty, nodes[character].Positions)
                {
                    Data = new BackgroundChatterResponse() { Label = "response:" },
                    DataType = typeof(BackgroundChatterResponse),
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    if (clickedNode.DataType == typeof(BackgroundChatter))
                    {
                        ((BackgroundChatter)clickedNode.Data!).Responses.Add((BackgroundChatterResponse)clickedNode.Data!);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                }

                break;
            }
            case SpawnableNodeType.CriteriaGroup:
            {
                string id = Guid.NewGuid().ToString();
                newNode = new Node(id, NodeType.CriteriaGroup, string.Empty, nodes[character].Positions)
                {
                    Data = new CriteriaGroup() { Name = id },
                    DataType = typeof(CriteriaGroup),
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    if (clickedNode.DataType == typeof(Criterion))
                    {
                        ((CriteriaGroup)newNode.Data!).CriteriaList[0].CriteriaList.Add((Criterion)clickedNode.Data!);
                        nodes[character].AddChild(newNode, clickedNode);
                    }
                }

                break;
            }
            case SpawnableNodeType.Dialogue:
            {
                int id = (characterStories[character].Dialogues!.Count + 1);
                newNode = new Node(id.ToString(), NodeType.Dialogue, string.Empty, nodes[character].Positions)
                {
                    Data = new Dialogue() { ID = id },
                    DataType = typeof(Dialogue),
                    FileName = character,
                };
                nodes[character].Add(newNode);
                characterStories[character].Dialogues!.Add((Dialogue)newNode.Data!);

                if (clickedNode != Node.NullNode)
                {
                    if (clickedNode.DataType == typeof(Criterion))
                    {
                        ((Dialogue)newNode.Data!).DynamicDialogueCriteria.Add((Criterion)clickedNode.Data!);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(GameEvent))
                    {
                        ((Dialogue)newNode.Data!).StartEvents.Add((GameEvent)clickedNode.Data!);
                        nodes[character].AddChild(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(Response))
                    {
                        ((Response)clickedNode.Data!).Next = ((Dialogue)newNode.Data!).ID;
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                }

                break;
            }
            case SpawnableNodeType.AlternateText:
            {
                string id = Guid.NewGuid().ToString();
                newNode = new Node(id, NodeType.AlternateText, string.Empty, nodes[character].Positions)
                {
                    Data = new AlternateText() { },
                    DataType = typeof(AlternateText),
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    if (clickedNode.DataType == typeof(Dialogue))
                    {
                        ((Dialogue)clickedNode.Data!).AlternateTexts.Add((AlternateText)newNode.Data!);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(Criterion))
                    {
                        ((AlternateText)newNode.Data!).Critera.Add((Criterion)clickedNode.Data!);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                }

                break;
            }
            case SpawnableNodeType.Event:
            {
                string id = Guid.NewGuid().ToString();
                newNode = new Node(id, NodeType.Event, string.Empty, nodes[character].Positions)
                {
                    Data = new GameEvent() { Character = character },
                    DataType = typeof(GameEvent),
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    GameEvent gameEvent = ((GameEvent)newNode.Data!);
                    if (clickedNode.DataType == typeof(Criterion))
                    {
                        gameEvent.Criteria.Add((Criterion)clickedNode.Data!);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(ItemAction))
                    {
                        ((ItemAction)clickedNode.Data!).OnTakeActionEvents.Add(gameEvent);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(UseWith))
                    {
                        ((UseWith)clickedNode.Data!).OnSuccessEvents.Add(gameEvent);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(EventTrigger))
                    {
                        ((EventTrigger)clickedNode.Data!).Events.Add(gameEvent);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(Response))
                    {
                        ((Response)clickedNode.Data!).ResponseEvents.Add(gameEvent);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(Dialogue))
                    {
                        ((Dialogue)clickedNode.Data!).StartEvents.Add(gameEvent);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(BackgroundChatter))
                    {
                        ((BackgroundChatter)clickedNode.Data!).StartEvents.Add(gameEvent);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(ItemInteraction))
                    {
                        ((ItemInteraction)clickedNode.Data!).OnAcceptEvents.Add(gameEvent);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(ItemGroupInteraction))
                    {
                        ((ItemGroupInteraction)clickedNode.Data!).OnAcceptEvents.Add(gameEvent);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                }

                break;
            }
            case SpawnableNodeType.EventTrigger:
            {
                string id = Guid.NewGuid().ToString();
                newNode = new Node(id, NodeType.EventTrigger, string.Empty, nodes[character].Positions)
                {
                    Data = new EventTrigger() { Id = id },
                    DataType = typeof(EventTrigger),
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    if (clickedNode.DataType == typeof(GameEvent))
                    {
                        ((EventTrigger)newNode.Data!).Events.Add((GameEvent)clickedNode.Data!);
                        nodes[character].AddChild(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(MainStory))
                    {
                        ((MainStory)clickedNode.Data!).PlayerReactions.Add((EventTrigger)newNode.Data!);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(CharacterStory))
                    {
                        ((CharacterStory)clickedNode.Data!).Reactions.Add((EventTrigger)newNode.Data!);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                }

                break;
            }
            case SpawnableNodeType.Item:
            {
                string id = "item name";
                newNode = new Node(id, NodeType.Item, string.Empty, nodes[character].Positions)
                {
                    Data = new ItemOverride() { ItemName = id },
                    DataType = typeof(ItemOverride),
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    if (clickedNode.DataType == typeof(ItemAction))
                    {
                        ((ItemOverride)newNode.Data!).ItemActions.Add((ItemAction)clickedNode.Data!);
                        nodes[character].AddChild(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(ItemGroup))
                    {
                        ((ItemGroup)clickedNode.Data!).ItemsInGroup.Add(((ItemOverride)newNode.Data!).ItemName!);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                }

                break;
            }
            case SpawnableNodeType.ItemGroup:
            {
                string id = Guid.NewGuid().ToString();
                newNode = new Node(id, NodeType.ItemGroup, string.Empty, nodes[character].Positions)
                {
                    Data = new ItemGroup() { Id = id },
                    DataType = typeof(ItemGroup),
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    if (clickedNode.DataType == typeof(ItemOverride))
                    {
                        ((ItemGroup)newNode.Data!).ItemsInGroup.Add(((ItemOverride)clickedNode.Data!).ItemName!);
                        nodes[character].AddChild(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(GameEvent))
                    {
                        ((GameEvent)clickedNode.Data!).EventType = GameEvents.ItemFromItemGroup;
                        ((GameEvent)clickedNode.Data!).Value = ((ItemGroup)newNode.Data!).Id;
                        nodes[character].AddChild(newNode, clickedNode);
                    }
                }

                break;
            }
            case SpawnableNodeType.Quest:
            {
                string id = Guid.NewGuid().ToString();
                newNode = new Node(id, NodeType.Quest, string.Empty, nodes[character].Positions)
                {
                    Data = new Quest() { CharacterName = character, ID = id },
                    DataType = typeof(Quest),
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    if (clickedNode.DataType == typeof(GameEvent))
                    {
                        ((GameEvent)clickedNode.Data!).EventType = GameEvents.Quest;
                        ((GameEvent)clickedNode.Data!).Key = ((Quest)newNode.Data!).ID;
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(Criterion))
                    {
                        ((Criterion)clickedNode.Data!).CompareType = CompareTypes.Quest;
                        ((Criterion)clickedNode.Data!).Key = ((Quest)newNode.Data!).ID;
                        nodes[character].AddChild(newNode, clickedNode);
                    }
                }

                break;
            }
            case SpawnableNodeType.Response:
            {
                string id = Guid.NewGuid().ToString();
                newNode = new Node(id, NodeType.Response, string.Empty, nodes[character].Positions)
                {
                    Data = new Response() { Id = id },
                    DataType = typeof(Response),
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    if (clickedNode.DataType == typeof(Dialogue))
                    {
                        ((Dialogue)clickedNode.Data!).Responses.Add((Response)newNode.Data!);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(Criterion))
                    {
                        ((Response)newNode.Data!).ResponseCriteria.Add((Criterion)clickedNode.Data!);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(GameEvent))
                    {
                        ((Response)newNode.Data!).ResponseEvents.Add((GameEvent)clickedNode.Data!);
                        nodes[character].AddChild(newNode, clickedNode);
                    }
                }

                break;
            }
            case SpawnableNodeType.Value:
            {
                string id = "ValueName";
                newNode = new Node(id, NodeType.Value, string.Empty, nodes[character].Positions)
                {
                    Data = new Value() { value = id },
                    DataType = typeof(Value),
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    if (clickedNode.DataType == typeof(Criterion))
                    {
                        ((Criterion)clickedNode.Data!).CompareType = CompareTypes.Value;
                        ((Criterion)clickedNode.Data!).Key = ((Value)newNode.Data!).value;
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(GameEvent))
                    {
                        ((GameEvent)clickedNode.Data!).EventType = GameEvents.ModifyValue;
                        ((GameEvent)clickedNode.Data!).Key = ((Value)newNode.Data!).value;
                        nodes[character].AddChild(newNode, clickedNode);
                    }
                }

                break;
            }
            case SpawnableNodeType.UseWith:
            {
                string id = "use on item:";
                newNode = new Node(id, NodeType.UseWith, string.Empty, nodes[character].Positions)
                {
                    Data = new UseWith() { ItemName = id },
                    DataType = typeof(UseWith),
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    if (clickedNode.DataType == typeof(ItemOverride))
                    {
                        ((ItemOverride)clickedNode.Data!).UseWiths.Add((UseWith)newNode.Data!);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                    else if (clickedNode.DataType == typeof(Criterion))
                    {
                        ((UseWith)newNode.Data!).Criteria.Add((Criterion)clickedNode.Data!);
                        nodes[character].AddParent(newNode, clickedNode);
                    }
                }

                break;
            }
            default:
            {
                break;
            }
        }

        if (ActiveForm is null)
        {
            return;
        }

        var pos = Graph.PointToClient(Cursor.Position);
        ScreenToGraph(pos.X, pos.Y, out float ScreenPosX, out float ScreenPosY);
        ScreenPosX -= NodeSizeX / 2;
        ScreenPosY -= NodeSizeY / 2;
        var ScreenPos = new PointF(ScreenPosX, ScreenPosY);

        newNode.Position = ScreenPos;

        clickedNode = newNode;
        ShowProperties(newNode);

        Graph.Invalidate();
        Graph.Focus();
    }
}
