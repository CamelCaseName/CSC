using CSC.Nodestuff;
using CSC.StoryItems;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security.Policy;
using System.Windows.Forms;
using System.Xml.Linq;
using static CSC.StoryItems.StoryEnums;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace CSC;

public partial class Main : Form
{
    private bool CurrentlyInPan = false;
    private bool IsCtrlPressed;
    private bool IsShiftPressed;
    private const TextFormatFlags TextFlags = TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.LeftAndRightPadding;
    private float AfterZoomNodeX;
    private float AfterZoomNodeY;
    private float BeforeZoomNodeX;
    private float BeforeZoomNodeY;
    private float StartPanOffsetX = 0f;
    private float StartPanOffsetY = 0f;
    private Font scaledFont = new(DefaultFont.FontFamily, 8f);
    private Node clickedNode;
    private Node highlightNode;
    private Node movedNode;
    private Node nodeToLinkFrom;
    private readonly Dictionary<string, float> OffsetX = [];
    private readonly Dictionary<string, float> OffsetY = [];
    private readonly Dictionary<string, float> Scaling = [];
    private readonly HatchBrush InterlinkedNodeBrush;
    private readonly int scaleX = (int)(NodeSizeX * 1.5f);
    private readonly int scaleY = (int)(NodeSizeY * 1.5f);
    private readonly List<int> maxYperX = [];
    private readonly List<Node> visited = [];
    private readonly Pen clickedLinePen;
    private readonly Pen highlightPen;
    private readonly Pen linePen;
    private readonly Pen circlePen;
    private readonly Pen nodeToLinkPen;
    private readonly SolidBrush ClickedNodeBrush;
    private readonly SolidBrush HighlightNodeBrush;
    private readonly SolidBrush NodeToLinkNextBrush;
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
    private readonly SolidBrush inventoryNodeBrush;
    private readonly SolidBrush itemActionNodeBrush;
    private readonly SolidBrush itemGroupBehaviourNodeBrush;
    private readonly SolidBrush itemGroupInteractionNodeBrush;
    private readonly SolidBrush itemGroupNodeBrush;
    private readonly SolidBrush itemNodeBrush;
    private readonly SolidBrush personalityNodeBrush;
    private readonly SolidBrush poseNodeBrush;
    private readonly SolidBrush propertyNodeBrush;
    private readonly SolidBrush questNodeBrush;
    private readonly SolidBrush responseNodeBrush;
    private readonly SolidBrush socialNodeBrush;
    private readonly SolidBrush stateNodeBrush;
    private readonly SolidBrush valueNodeBrush;
    private readonly SolidBrush darkachievementNodeBrush;
    private readonly SolidBrush darkalternateTextNodeBrush;
    private readonly SolidBrush darkbgcNodeBrush;
    private readonly SolidBrush darkbgcResponseNodeBrush;
    private readonly SolidBrush darkcharacterGroupNodeBrush;
    private readonly SolidBrush darkclothingNodeBrush;
    private readonly SolidBrush darkcriteriaGroupNodeBrush;
    private readonly SolidBrush darkcriterionNodeBrush;
    private readonly SolidBrush darkcutsceneNodeBrush;
    private readonly SolidBrush darkdefaultNodeBrush;
    private readonly SolidBrush darkdialogueNodeBrush;
    private readonly SolidBrush darkdoorNodeBrush;
    private readonly SolidBrush darkeventNodeBrush;
    private readonly SolidBrush darkeventTriggerNodeBrush;
    private readonly SolidBrush darkinventoryNodeBrush;
    private readonly SolidBrush darkitemActionNodeBrush;
    private readonly SolidBrush darkitemGroupBehaviourNodeBrush;
    private readonly SolidBrush darkitemGroupInteractionNodeBrush;
    private readonly SolidBrush darkitemGroupNodeBrush;
    private readonly SolidBrush darkitemNodeBrush;
    private readonly SolidBrush darkpersonalityNodeBrush;
    private readonly SolidBrush darkposeNodeBrush;
    private readonly SolidBrush darkpropertyNodeBrush;
    private readonly SolidBrush darkquestNodeBrush;
    private readonly SolidBrush darkresponseNodeBrush;
    private readonly SolidBrush darksocialNodeBrush;
    private readonly SolidBrush darkstateNodeBrush;
    private readonly SolidBrush darkvalueNodeBrush;
    private RectangleF adjustedMouseClipBounds;
    private SizeF OffsetFromDragClick = SizeF.Empty;
    private SizeF CircleSize = new(15, 15);
    CachedBitmap? oldGraph;
    private static MainStory Story = new();
    public static readonly Dictionary<string, CharacterStory> Stories = [];
    private static readonly Dictionary<string, NodeStore> nodes = [];
    private static string selectedCharacter = NoCharacter;
    public bool MovingChild = false;
    public const int NodeSizeX = 200;
    public const int NodeSizeY = 50;
    public const string NoCharacter = "None";
    public const string Player = "Player";
    private const string Anybody = "Anybody";

    public static string StoryName { get; private set; } = NoCharacter;
    RectangleF adjustedVisibleClipBounds = new();
    Point oldMousePosBeforeSpawnWindow = Point.Empty;

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

    public int RightClickFrameCounter { get; private set; } = 0;

    public Main()
    {
        InitializeComponent();
        StoryTree.ExpandAll();
        Application.AddMessageFilter(new MouseMessageFilter());
        MouseMessageFilter.MouseMove += HandleMouseEvents;
        linePen = new Pen(Color.FromArgb(75, 75, 75), 0.2f)
        {
            EndCap = LineCap.Triangle,
            StartCap = LineCap.Round
        };
        circlePen = new Pen(Color.FromArgb(75, 75, 75), 0.5f)
        {
            EndCap = LineCap.Triangle,
            StartCap = LineCap.Round
        };
        clickedLinePen = new Pen(Brushes.LightGray, 3)
        {
            EndCap = LineCap.Triangle,
            StartCap = LineCap.Round
        };
        highlightPen = new Pen(Brushes.DeepPink, 3)
        {
            EndCap = LineCap.Triangle,
            StartCap = LineCap.Round
        };
        nodeToLinkPen = new Pen(Brushes.LightCyan, 3)
        {
            EndCap = LineCap.Triangle,
            StartCap = LineCap.Round
        };
        defaultNodeBrush = new SolidBrush(Color.FromArgb(100, 100, 100));
        achievementNodeBrush = new SolidBrush(Color.FromArgb(200, 10, 200));
        alternateTextNodeBrush = new SolidBrush(Color.FromArgb(110, 120, 190));
        bgcNodeBrush = new SolidBrush(Color.FromArgb(40, 190, 255));
        bgcResponseNodeBrush = new SolidBrush(Color.FromArgb(150, 225, 255));
        characterGroupNodeBrush = new SolidBrush(Color.FromArgb(190, 180, 130));
        clothingNodeBrush = new SolidBrush(Color.FromArgb(115, 235, 30));
        criteriaGroupNodeBrush = new SolidBrush(Color.FromArgb(150, 50, 50));
        criterionNodeBrush = new SolidBrush(Color.FromArgb(180, 20, 40));
        cutsceneNodeBrush = new SolidBrush(Color.FromArgb(235, 30, 160));
        dialogueNodeBrush = new SolidBrush(Color.FromArgb(45, 60, 185));
        doorNodeBrush = new SolidBrush(Color.FromArgb(200, 225, 65));
        eventNodeBrush = new SolidBrush(Color.FromArgb(50, 150, 50));
        eventTriggerNodeBrush = new SolidBrush(Color.FromArgb(40, 120, 70));
        inventoryNodeBrush = new SolidBrush(Color.FromArgb(65, 225, 185));
        itemActionNodeBrush = new SolidBrush(Color.FromArgb(85, 195, 195));
        itemGroupBehaviourNodeBrush = new SolidBrush(Color.FromArgb(160, 200, 195));
        itemGroupInteractionNodeBrush = new SolidBrush(Color.FromArgb(95, 120, 115));
        itemGroupNodeBrush = new SolidBrush(Color.FromArgb(45, 190, 165));
        itemNodeBrush = new SolidBrush(Color.FromArgb(45, 255, 255));
        personalityNodeBrush = new SolidBrush(Color.FromArgb(255, 255, 90));
        poseNodeBrush = new SolidBrush(Color.FromArgb(255, 210, 90));
        propertyNodeBrush = new SolidBrush(Color.FromArgb(255, 90, 150));
        questNodeBrush = new SolidBrush(Color.FromArgb(150, 210, 155));
        responseNodeBrush = new SolidBrush(Color.FromArgb(55, 155, 225));
        socialNodeBrush = new SolidBrush(Color.FromArgb(255, 160, 90));
        stateNodeBrush = new SolidBrush(Color.FromArgb(40, 190, 50));
        valueNodeBrush = new SolidBrush(Color.FromArgb(120, 0, 150));
        //darker color variants
        float darkening = 0.18f;
        darkdefaultNodeBrush = new SolidBrush(defaultNodeBrush.Color.Times(darkening));
        darkachievementNodeBrush = new SolidBrush(achievementNodeBrush.Color.Times(darkening));
        darkalternateTextNodeBrush = new SolidBrush(alternateTextNodeBrush.Color.Times(darkening));
        darkbgcNodeBrush = new SolidBrush(bgcNodeBrush.Color.Times(darkening));
        darkbgcResponseNodeBrush = new SolidBrush(bgcResponseNodeBrush.Color.Times(darkening));
        darkcharacterGroupNodeBrush = new SolidBrush(characterGroupNodeBrush.Color.Times(darkening));
        darkclothingNodeBrush = new SolidBrush(clothingNodeBrush.Color.Times(darkening));
        darkcriteriaGroupNodeBrush = new SolidBrush(criteriaGroupNodeBrush.Color.Times(darkening));
        darkcriterionNodeBrush = new SolidBrush(criterionNodeBrush.Color.Times(darkening));
        darkcutsceneNodeBrush = new SolidBrush(cutsceneNodeBrush.Color.Times(darkening));
        darkdialogueNodeBrush = new SolidBrush(dialogueNodeBrush.Color.Times(darkening));
        darkdoorNodeBrush = new SolidBrush(doorNodeBrush.Color.Times(darkening));
        darkeventNodeBrush = new SolidBrush(eventNodeBrush.Color.Times(darkening));
        darkeventTriggerNodeBrush = new SolidBrush(eventTriggerNodeBrush.Color.Times(darkening));
        darkinventoryNodeBrush = new SolidBrush(inventoryNodeBrush.Color.Times(darkening));
        darkitemActionNodeBrush = new SolidBrush(itemActionNodeBrush.Color.Times(darkening));
        darkitemGroupBehaviourNodeBrush = new SolidBrush(itemGroupBehaviourNodeBrush.Color.Times(darkening));
        darkitemGroupInteractionNodeBrush = new SolidBrush(itemGroupInteractionNodeBrush.Color.Times(darkening));
        darkitemGroupNodeBrush = new SolidBrush(itemGroupNodeBrush.Color.Times(darkening));
        darkitemNodeBrush = new SolidBrush(itemNodeBrush.Color.Times(darkening));
        darkpersonalityNodeBrush = new SolidBrush(personalityNodeBrush.Color.Times(darkening));
        darkposeNodeBrush = new SolidBrush(poseNodeBrush.Color.Times(darkening));
        darkpropertyNodeBrush = new SolidBrush(propertyNodeBrush.Color.Times(darkening));
        darkquestNodeBrush = new SolidBrush(questNodeBrush.Color.Times(darkening));
        darkresponseNodeBrush = new SolidBrush(responseNodeBrush.Color.Times(darkening));
        darksocialNodeBrush = new SolidBrush(socialNodeBrush.Color.Times(darkening));
        darkstateNodeBrush = new SolidBrush(stateNodeBrush.Color.Times(darkening));
        darkvalueNodeBrush = new SolidBrush(valueNodeBrush.Color.Times(darkening));

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

        clickedNode = Node.NullNode;
        highlightNode = Node.NullNode;
        movedNode = Node.NullNode;
        nodeToLinkFrom = Node.NullNode;
    }

    //todo add selection
    //todo add option to isolate/pull all childs and parents close
    //todo add info when trying to link incompatible notes
    //todo add search
    //todo add new file creation
    //todo add file export
    //todo use the csc dll to populate the rest of the events and criteria i havent done yet, 
    //maybe turn it into a store where you put in the type and get out the relevant fields in order
    //this can then be used for linking as well
    //todo unify all node creation so its always the same
    //todo add grouping

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
        else if (e.KeyData == Keys.Escape)
        {
            NodeSpawnBox.Enabled = false;
            NodeSpawnBox.Visible = false;
            nodeToLinkFrom = Node.NullNode;
            oldMousePosBeforeSpawnWindow = Point.Empty;
            ClearOverlayBitmap();
            Graph.Focus();
            Graph.Invalidate();
        }
        else if (e.KeyData == Keys.Delete)
        {
            TryDeleteNode();
        }
    }

    private void TryDeleteNode()
    {
        Node removedNode = Node.NullNode;
        if (highlightNode != Node.NullNode)
        {
            if (MessageBox.Show("Delete " + highlightNode.Text[..Math.Min(highlightNode.Text.Length, 20)] + "[...]?", "Delete for real?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (nodeToLinkFrom == highlightNode)
                {
                    nodeToLinkFrom = Node.NullNode;
                }
                if (clickedNode == highlightNode)
                {
                    clickedNode = Node.NullNode;
                }

                removedNode = highlightNode;
                highlightNode = Node.NullNode;
            }
        }
        else if (clickedNode != Node.NullNode)
        {
            if (MessageBox.Show("Delete " + clickedNode.Text[..Math.Min(clickedNode.Text.Length, 20)] + "[...]?", "Delete for real?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (nodeToLinkFrom == clickedNode)
                {
                    nodeToLinkFrom = Node.NullNode;
                }
                if (highlightNode == clickedNode)
                {
                    highlightNode = Node.NullNode;
                }

                removedNode = clickedNode;
                clickedNode = Node.NullNode;
            }
        }

        var family = nodes[SelectedCharacter][removedNode];
        List<Node> childs = [.. family.Childs];
        List<Node> parents = [.. family.Parents];

        foreach (var child in childs)
        {
            Unlink(removedNode, child, true);
        }
        foreach (var parent in parents)
        {
            Unlink(parent, removedNode, true);
        }
        nodes[SelectedCharacter].Remove(removedNode);

        foreach (var child in childs)
        {
            NodeLinker.UpdateLinks(child, SelectedCharacter, nodes[SelectedCharacter]);
        }
        foreach (var parent in parents)
        {
            NodeLinker.UpdateLinks(parent, SelectedCharacter, nodes[SelectedCharacter]);
        }
        Graph.Invalidate();
    }

    private void ShowNodeSpawnBox()
    {

        oldMousePosBeforeSpawnWindow = Graph.PointToClient(Cursor.Position);

        //only allow node types that make sense depending on the selected node type
        NodeSpawnBox.Items.Clear();
        NodeSpawnBox.Items.AddRange(GetSpawnableNodeTypes());

        NodeSpawnBox.Enabled = true;
        NodeSpawnBox.Visible = true;
        NodeSpawnBox.Focus();
    }

    private object[] GetSpawnableNodeTypes()
    {
        if (clickedNode != Node.NullNode)
        {
            switch (clickedNode.Type)
            {
                //itemaction uswith criterialist event eventtrigger alternatetext
                //response dialogue bgc item itemgroup
                case NodeType.Criterion:
                {
                    return [NodeType.ItemAction, NodeType.UseWith, NodeType.CriteriaGroup, NodeType.GameEvent, NodeType.EventTrigger, NodeType.AlternateText, NodeType.Response, NodeType.Dialogue, NodeType.BGC, NodeType.StoryItem, NodeType.ItemGroup, NodeType.Value];
                }

                //item and event and criterion
                case NodeType.ItemAction:
                case NodeType.ItemInteraction:
                case NodeType.ItemGroupBehaviour:
                case NodeType.ItemGroupInteraction:
                case NodeType.Inventory:
                case NodeType.StoryItem:
                case NodeType.ItemGroup:
                case NodeType.UseWith:
                {
                    return [NodeType.StoryItem, NodeType.GameEvent, NodeType.Criterion];
                }

                //event
                case NodeType.Pose:
                case NodeType.Achievement:
                {
                    return [NodeType.GameEvent];
                }

                //event bgcresponse
                case NodeType.BGC:
                {
                    return [NodeType.BGCResponse, NodeType.GameEvent, NodeType.Criterion];
                }

                //bgc
                case NodeType.BGCResponse:
                {
                    return [NodeType.BGC];
                }
                //criteria dialogue
                case NodeType.Response:
                {
                    return [NodeType.Dialogue, NodeType.GameEvent, NodeType.Criterion];
                }

                case NodeType.Dialogue:
                {
                    return [NodeType.GameEvent, NodeType.Criterion, NodeType.Response];
                }
                case NodeType.Value:
                {
                    return [NodeType.GameEvent, NodeType.Criterion, NodeType.Value];
                }
                //event criterion
                case NodeType.CharacterGroup:
                case NodeType.AlternateText:
                case NodeType.EventTrigger:
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
                    return [NodeType.GameEvent, NodeType.Criterion];
                }
                case NodeType.GameEvent:
                default:
                {
                    return [NodeType.Criterion, NodeType.ItemAction, NodeType.Achievement, NodeType.BGC, NodeType.BGCResponse, NodeType.CriteriaGroup, NodeType.Dialogue, NodeType.AlternateText, NodeType.GameEvent, NodeType.EventTrigger, NodeType.StoryItem, NodeType.ItemGroup, NodeType.Quest, NodeType.UseWith, NodeType.Response, NodeType.Value];
                }
            }
        }
        else
        {
            return [NodeType.Criterion, NodeType.ItemAction, NodeType.Achievement, NodeType.BGC, NodeType.BGCResponse, NodeType.CriteriaGroup, NodeType.Dialogue, NodeType.AlternateText, NodeType.GameEvent, NodeType.EventTrigger, NodeType.StoryItem, NodeType.ItemGroup, NodeType.Quest, NodeType.UseWith, NodeType.Response, NodeType.Value];
        }
    }

    public void HandleMouseEvents(object? sender, MouseEventArgs e)
    {
        if (ActiveForm is null)
        {
            return;
        }
        var pos = Graph.PointToClient(Cursor.Position);
        ScreenToGraph(pos.X, pos.Y, out float GraphPosX, out float GraphPosY);
        var graphPos = new PointF(GraphPosX, GraphPosY);

        if (MovingChild && movedNode != Node.NullNode)
        {
            movedNode.Position = graphPos + OffsetFromDragClick;
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

        if (!adjustedMouseClipBounds.Contains(graphPos))
        {
            return;
        }
        else if (e.Button != MouseButtons.None)
        {
            Graph.Focus();
        }
        else if (e.Button == MouseButtons.None && MouseButtons == MouseButtons.Middle)
        {
            UpdatePan(pos);
            return;
        }
        else if (e.Button == MouseButtons.None && MouseButtons == MouseButtons.Right)
        {
            UpdateRightClick(graphPos);
            return;
        }

        //everything else, scrolling for example
        if (e.Delta != 0)
        {
            ClearOverlayBitmap();
            UpdateScaling(e);
            //redraw
            Graph.Invalidate();
            TryCreateOverlayBitmap();
        }

        //set old position for next frame/call
        switch (e.Button)
        {
            case MouseButtons.Left:
            {
                Graph.Focus();
                RightClickFrameCounter = 0;
                if (e.Clicks > 1)
                {
                    //double click
                    UpdateDoubleClickTransition(graphPos);
                    Graph.Focus();
                }
                else if (e.Clicks == 1)
                {
                    _ = UpdateClickedNode(graphPos);
                    Graph.Invalidate();
                }
                break;
            }
            case MouseButtons.None:
            {
                if (!MovingChild && RightClickFrameCounter > 0)
                {
                    _ = UpdateClickedNode(graphPos);
                    //right click only no move, spawn context
                    SpawnContextMenu(graphPos);
                }
                if (MouseButtons != MouseButtons.Right)
                {
                    RightClickFrameCounter = 0;
                    MovingChild = false;
                }

                EndPan();
                UpdateHighlightNode(graphPos);

                //we are only moving the mouse so we can go ahead and cache the latest state and draw over that with the connecting line if we need to
                TryCreateOverlayBitmap();
            }
            break;
            case MouseButtons.Right:
            {
                if (e.Clicks > 0)
                {
                    UpdateRightClick(graphPos);
                }
            }
            break;
            case MouseButtons.Middle:
            {
                if (e.Clicks > 0)
                {
                    UpdatePan(pos);
                    Graph.Focus();
                }
                break;
            }
            case MouseButtons.XButton1:
                break;
            case MouseButtons.XButton2:
                break;
            default:
            {
                if (!MovingChild && RightClickFrameCounter > 0)
                {
                    _ = UpdateClickedNode(graphPos);
                    //right click only no move, spawn context
                    SpawnContextMenu(graphPos);
                }
                if (MouseButtons != MouseButtons.Right)
                {
                    RightClickFrameCounter = 0;
                    MovingChild = false;
                }
                EndPan();
                UpdateHighlightNode(graphPos);
                TryCreateOverlayBitmap();
            }
            break;
        }

        void UpdateRightClick(PointF graphPos)
        {
            RightClickFrameCounter++;
            Debug.WriteLine(RightClickFrameCounter);
            if (!MovingChild && RightClickFrameCounter > 1)
            {
                Node node = UpdateClickedNode(graphPos);
                movedNode = node;
                MovingChild = true;
                OffsetFromDragClick = new SizeF((movedNode.Position.X - graphPos.X), (movedNode.Position.Y - graphPos.Y));
                Debug.WriteLine(MovingChild);
            }
            Graph.Focus();
        }
    }

    private void TryCreateOverlayBitmap()
    {
        if (nodeToLinkFrom != Node.NullNode)
        {
            if (oldGraph is null)
            {
                //this is correct
                var map = new Bitmap(Graph.Width, Graph.Height);
                Graph.DrawToBitmap(map, Graph.ClientRectangle);
                oldGraph = new CachedBitmap(map, Graphics.FromHwnd(Handle));
                map.Save("tmp.bmp");
            }
            Graph.Invalidate();
        }
        else
        {
            ClearOverlayBitmap();
        }
    }

    private void ClearOverlayBitmap()
    {
        if (oldGraph is not null)
        {
            oldGraph.Dispose();
            oldGraph = null;
            Graph.Invalidate();
        }
    }

    public static void RedrawGraph()
    {
        ((Main?)(ActiveForm))?.Graph.Invalidate();
    }

    public static void ClearNodePos(Node node)
    {
        nodes[SelectedCharacter].Positions.ClearNode(node);
    }

    public static void SetNodePos(Node node)
    {
        nodes[SelectedCharacter].Positions.SetNode(node);
    }

    public static void ClearNodePos(Node node, string file)
    {
        nodes[file].Positions.ClearNode(node);
    }

    public static void SetNodePos(Node node, string file)
    {
        nodes[file].Positions.SetNode(node);
    }

    private void UpdateDoubleClickTransition(PointF ScreenPos)
    {
        var node = UpdateClickedNode(ScreenPos);
        if (node != Node.NullNode)
        {
            if (node.FileName != SelectedCharacter)
            {
                if (node.FileName == StoryName || node.FileName == Player || node.FileName == Anybody)
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

    private Node TryGetNodeLinkStart(PointF mouseGraphLocation)
    {
        if (adjustedMouseClipBounds.Contains(mouseGraphLocation))
        {
            foreach (var node in nodes[SelectedCharacter].Positions[mouseGraphLocation])
            {
                GetLinkCircleRects(node, out RectangleF leftRect, out RectangleF rightRect);
                float circleCenterX = leftRect.X + leftRect.Width / 2;
                float circleCenterRightX = rightRect.X + rightRect.Width / 2;
                float circleCenterY = leftRect.Y + leftRect.Height / 2;
                float DistanceLeft = MathF.Sqrt(MathF.Pow(mouseGraphLocation.X - circleCenterX, 2) + MathF.Pow(mouseGraphLocation.Y - circleCenterY, 2));
                float DistanceRight = MathF.Sqrt(MathF.Pow(mouseGraphLocation.X - (circleCenterRightX), 2) + MathF.Pow(mouseGraphLocation.Y - circleCenterY, 2));

                if (DistanceLeft < CircleSize.Width / 2)
                {
                    return node;
                }
                else if (DistanceRight < CircleSize.Width / 2)
                {
                    return node;
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

    private void DrawEdge(Graphics g, Node parent, Node child, Pen pen, PointF start = default, PointF end = default)
    {
        int third = 0;

        third = GetEdgeStartHeight(parent, child, third);
        if (third == 0)
        {
            third = GetEdgeStartHeight(child, parent, third);
        }
        if (start == default)
        {
            start = GetStartHeightFromThird(parent, parent.Position, third);
        }
        if (end == default)
        {
            end = child.Position + new SizeF(0, child.Size.Height / 2);
        }

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

        g.DrawBezier(pen, start, controlStart, controlEnd, end);
        //e.Graphics.DrawEllipse(Pens.Green, new Rectangle(controlStart, new Size(4, 4)));
        //e.Graphics.DrawEllipse(Pens.Red, new Rectangle(controlEnd, new Size(4, 4)));
    }

    private static PointF GetStartHeightFromThird(Node parent, PointF start, int third)
    {
        if (third == 0)
        {
            start = start + new SizeF(parent.Size.Width, parent.Size.Height / 2);
        }
        else if (third == 1)
        {
            start = start + new SizeF(parent.Size.Width, parent.Size.Height / 5);
        }
        else if (third == 2)
        {
            start = start + new SizeF(parent.Size.Width, parent.Size.Height / 5 * 4);
        }

        return start;
    }

    private static int GetEdgeStartHeight(Node parent, Node child, int third)
    {
        if (parent.DataType == typeof(Dialogue) && child.DataType == typeof(GameEvent))
        {
            if (parent.Data<Dialogue>()!.StartEvents.Contains(child.Data<GameEvent>()!))
            {
                third = 1;
            }
            else if (parent.Data<Dialogue>()!.CloseEvents.Contains(child.Data<GameEvent>()!))
            {
                third = 2;
            }
        }
        else if (parent.DataType == typeof(ItemInteraction) && child.DataType == typeof(GameEvent))
        {
            if (parent.Data<ItemInteraction>()!.OnAcceptEvents.Contains(child.Data<GameEvent>()!))
            {
                third = 1;
            }
            else if (parent.Data<ItemInteraction>()!.OnRefuseEvents.Contains(child.Data<GameEvent>()!))
            {
                third = 2;
            }
        }
        else if (parent.DataType == typeof(ItemGroupInteraction) && child.DataType == typeof(GameEvent))
        {
            if (parent.Data<ItemGroupInteraction>()!.OnAcceptEvents.Contains(child.Data<GameEvent>()!))
            {
                third = 1;
            }
            else if (parent.Data<ItemGroupInteraction>()!.OnRefuseEvents.Contains(child.Data<GameEvent>()!))
            {
                third = 2;
            }
        }

        return third;
    }

    private void DrawNode(Graphics g, Node node, SolidBrush brush, bool lightText = false)
    {
        if (Scaling[SelectedCharacter] > 0.28f)
        {
            GetLinkCircleRects(node, out RectangleF leftRect, out RectangleF rightRect);

            g.DrawEllipse(circlePen, leftRect);
            g.DrawEllipse(circlePen, rightRect);
        }
        if (node == clickedNode)
        {
            lightText = true;
            if (node.FileName != SelectedCharacter)
            {
                lightText = true;
                g.FillPath(InterlinkedNodeBrush, RoundedRect(ScaleRect(node.Rectangle, 25), 18f));
            }
            g.FillPath(ClickedNodeBrush, RoundedRect(ScaleRect(node.Rectangle, 15), 15f));
        }
        else if (node.FileName != SelectedCharacter)
        {
            g.FillPath(InterlinkedNodeBrush, RoundedRect(ScaleRect(node.Rectangle, 15), 15f));
        }

        g.FillPath(brush, RoundedRect(node.Rectangle, 10f));

        if (Scaling[SelectedCharacter] > 0.28f)
        {
            var scaledRect = GetScaledRect(g, node.RectangleNonF, Scaling[SelectedCharacter]);
            scaledRect.Location += new Size(3, 3);
            scaledRect.Size -= new Size(6, 6);

            Color textColor = lightText ? Color.White : Color.DarkGray;
            if ((brush.Color.R * 0.299 + brush.Color.G * 0.587 + brush.Color.B * 0.114) > 150)
            {
                textColor = Color.Black;
            }

            TextRenderer.DrawText(g,
                                  node.Text[..Math.Min(node.Text.Length, 100)],
                                  scaledFont,
                                  scaledRect,
                                  textColor,
                                  TextFlags);
        }
    }

    private void GetLinkCircleRects(Node node, out RectangleF leftRect, out RectangleF rightRect)
    {
        leftRect = new RectangleF(node.Position + new SizeF(-CircleSize.Width / 2, (node.Size.Height / 2) - CircleSize.Width / 2), CircleSize);
        rightRect = new RectangleF(node.Position + new SizeF(node.Size.Width - CircleSize.Width / 2, (node.Size.Height / 2) - CircleSize.Width / 2), CircleSize);
        if (node.FileName != SelectedCharacter)
        {
            if (node == clickedNode)
            {
                leftRect.Location -= new SizeF(25 / 2, 0);
                rightRect.Location += new SizeF(25 / 2, 0);
            }
            else
            {
                leftRect.Location -= new SizeF(15 / 2, 0);
                rightRect.Location += new SizeF(15 / 2, 0);
            }
        }
        else if (node == clickedNode)
        {
            leftRect.Location -= new SizeF(15 / 2, 0);
            rightRect.Location += new SizeF(15 / 2, 0);
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
            TryCreateOverlayBitmap();
        }
    }

    private SolidBrush GetNodeColor(NodeType type, bool light)
    {
        return type switch
        {
            NodeType.Null => light ? defaultNodeBrush : darkdefaultNodeBrush,
            NodeType.CharacterGroup => light ? characterGroupNodeBrush : darkcharacterGroupNodeBrush,
            NodeType.Criterion => light ? criterionNodeBrush : darkcriterionNodeBrush,
            NodeType.ItemAction => light ? itemActionNodeBrush : darkitemActionNodeBrush,
            NodeType.ItemGroupBehaviour => light ? itemGroupBehaviourNodeBrush : darkitemGroupBehaviourNodeBrush,
            NodeType.ItemGroupInteraction => light ? itemGroupInteractionNodeBrush : darkitemGroupInteractionNodeBrush,
            NodeType.Pose => light ? poseNodeBrush : darkposeNodeBrush,
            NodeType.Achievement => light ? achievementNodeBrush : darkachievementNodeBrush,
            NodeType.BGC => light ? bgcNodeBrush : darkbgcNodeBrush,
            NodeType.BGCResponse => light ? bgcResponseNodeBrush : darkbgcResponseNodeBrush,
            NodeType.Clothing => light ? clothingNodeBrush : darkclothingNodeBrush,
            NodeType.CriteriaGroup => light ? criteriaGroupNodeBrush : darkcriteriaGroupNodeBrush,
            NodeType.Cutscene => light ? cutsceneNodeBrush : darkcutsceneNodeBrush,
            NodeType.Dialogue => light ? dialogueNodeBrush : darkdialogueNodeBrush,
            NodeType.AlternateText => light ? alternateTextNodeBrush : darkalternateTextNodeBrush,
            NodeType.Door => light ? doorNodeBrush : darkdoorNodeBrush,
            NodeType.GameEvent => light ? eventNodeBrush : darkeventNodeBrush,
            NodeType.EventTrigger => light ? eventTriggerNodeBrush : darkeventTriggerNodeBrush,
            NodeType.Inventory => light ? inventoryNodeBrush : darkinventoryNodeBrush,
            NodeType.StoryItem => light ? itemNodeBrush : darkitemNodeBrush,
            NodeType.ItemGroup => light ? itemGroupNodeBrush : darkitemGroupNodeBrush,
            NodeType.Personality => light ? personalityNodeBrush : darkpersonalityNodeBrush,
            NodeType.Property => light ? propertyNodeBrush : darkpropertyNodeBrush,
            NodeType.Quest => light ? questNodeBrush : darkquestNodeBrush,
            NodeType.Response => light ? responseNodeBrush : darkresponseNodeBrush,
            NodeType.Social => light ? socialNodeBrush : darksocialNodeBrush,
            NodeType.State => light ? stateNodeBrush : darkstateNodeBrush,
            NodeType.Value => light ? valueNodeBrush : darkvalueNodeBrush,
            _ => defaultNodeBrush,
        };
    }

    private void Main_Paint(object sender, PaintEventArgs e)
    {
        if (selectedCharacter == NoCharacter)
        {
            return;
        }
        var g = e.Graphics;

        if (oldGraph is null || nodeToLinkFrom == Node.NullNode)
        {
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
            scaledFont = GetScaledFont(g, new(DefaultFont.FontFamily, 8f), Scaling[SelectedCharacter]);

            DrawAllNodes(g);
        }
        else
        {
            g.DrawCachedBitmap(oldGraph, 0, 0);
            DrawLinkToNextEdge(g);
        }
    }

    private void DrawLinkToNextEdge(Graphics g)
    {
        Point pos;
        if (oldMousePosBeforeSpawnWindow != Point.Empty)
        {
            pos = oldMousePosBeforeSpawnWindow;
        }
        else
        {
            pos = Graph.PointToClient(Cursor.Position);
        }

        GraphToScreen(nodeToLinkFrom.Position.X, nodeToLinkFrom.Position.Y, out float screenX, out float ScreenY);
        var screenPos = new PointF(screenX, ScreenY);

        screenPos += new SizeF(nodeToLinkFrom.Size.Width * Scaling[nodeToLinkFrom.FileName], (nodeToLinkFrom.Size.Height / 2) * Scaling[nodeToLinkFrom.FileName]);

        DrawEdge(g, nodeToLinkFrom, Node.NullNode, nodeToLinkPen, start: screenPos, end: pos);
    }

    private void DrawAllNodes(Graphics g)
    {
        foreach (var node in nodes[SelectedCharacter].Nodes)
        {
            var list = nodes[SelectedCharacter].Childs(node);
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    DrawEdge(g, node, item, linePen);
                }
            }
        }

        if (clickedNode != Node.NullNode)
        {
            var family = nodes[SelectedCharacter][clickedNode];
            if (family.Childs.Count > 0)
            {
                foreach (var item in family.Childs)
                {
                    DrawEdge(g, clickedNode, item, clickedLinePen);
                }
            }
            if (family.Parents.Count > 0)
            {
                foreach (var item in family.Parents)
                {
                    DrawEdge(g, item, clickedNode, clickedLinePen);
                }
            }

            foreach (var node in nodes[SelectedCharacter].Positions[adjustedVisibleClipBounds])
            {
                //c++;
                bool light = family.Childs.Contains(node) || family.Parents.Contains(node) || clickedNode == node;
                DrawNode(g, node, GetNodeColor(node.Type, light), light);
            }
        }
        else
        {
            foreach (var node in nodes[SelectedCharacter].Positions[adjustedVisibleClipBounds])
            {
                //c++;
                DrawNode(g, node, GetNodeColor(node.Type, false));
            }
        }

        if (highlightNode != Node.NullNode)
        {
            var family = nodes[SelectedCharacter][highlightNode];
            if (family.Childs.Count > 0)
            {
                foreach (var item in family.Childs)
                {
                    DrawEdge(g, highlightNode, item, highlightPen);
                }
            }
            if (family.Parents.Count > 0)
            {
                foreach (var item in family.Parents)
                {
                    DrawEdge(g, item, highlightNode, highlightPen);
                }
            }
            DrawNode(g, highlightNode, HighlightNodeBrush, true);
        }

        if (nodeToLinkFrom != Node.NullNode)
        {
            DrawNode(g, nodeToLinkFrom, NodeToLinkNextBrush, true);
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

        Graph.Invalidate();

        StoryTree.SelectedNode = StoryTree.Nodes[0].Nodes[1].Nodes[Stories.Count - 1];
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
                FileName = Player;
            }
            else
            {
                CharacterStory story = JsonConvert.DeserializeObject<CharacterStory>(fileString) ?? new CharacterStory();
                FileName = story.CharacterName!;
                Stories.Add(FileName, story);

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

        if (FileName == Player)
        {
            NodeLinker.DissectStory(Story, tempStore, Path.GetFileNameWithoutExtension(FilePath));
        }
        else
        {
            NodeLinker.DissectCharacter(Stories[FileName], tempStore);
        }
        //even link for single file, should be able to link most suff so it stays readable
        NodeLinker.Interlinknodes(tempStore, FileName);

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
            var intX = 100;
            maxYperX.ExtendToIndex(intX, 1);

            visited.Clear();
            for (int i = 0; i < maxYperX.Count; i++)
            {
                maxYperX[i] = 1;
            }

            NodeStore nodeStore = nodes[store];
            var nodeList = nodeStore.Nodes;
            int ParentEdgeMaxStartValue = 1;

            nodeList.Sort(new NodeParentComparer(nodes[store]));
            foreach (var key in nodeList)
            {
                Family family = nodeStore[key];
                if (family.Parents.Count > ParentEdgeMaxStartValue || visited.Contains(key))
                {
                    continue;
                }

                if (family.Childs.Count > 0)
                {
                    intX = maxYperX.Count + 1;
                }
                else
                {
                    intX += 1 + ParentEdgeMaxStartValue;
                }
                maxYperX.ExtendToIndex(intX, 1);

                Queue<Node> toExplore = [];
                Queue<int> layerX = [];
                toExplore.Enqueue(key);
                layerX.Enqueue(intX);

                //Debug.WriteLine($"starting on {key.ID} at {intX}|{1}");

                while (toExplore.Count > 0)
                {
                    var node = toExplore.Dequeue();
                    intX = layerX.Dequeue();

                    if (visited.Contains(node))
                    {
                        continue;
                    }
                    else
                    {
                        visited.Add(node);
                    }

                    var childs = nodeStore.Childs(node);
                    childs.Sort(new NodeChildComparer(nodeStore));
                    var parents = nodeStore.Parents(node);
                    parents.Sort(new NodeParentComparer(nodeStore));

                    int newParentsX = intX - (parents.Count / 3) - 1;
                    newParentsX = Math.Max(0, newParentsX);
                    int newChildX = intX + (childs.Count / 3) + 1;
                    maxYperX.ExtendToIndex(newChildX, maxYperX[intX]);

                    int rest = (int)float.Round(node.Size.Height / NodeSizeY);
                    rest = rest < 0 ? 1 : rest;

                    node.Position = new PointF(intX * scaleX, maxYperX[intX] * scaleY);

                    maxYperX[intX] += rest;

                    if (parents.Count > 0)
                    {
                        if (maxYperX[newParentsX] < maxYperX[intX] - rest)
                        {
                            maxYperX[newParentsX] = maxYperX[intX] - rest;
                        }

                        foreach (var item in parents)
                        {
                            if (visited.Contains(item))
                            {
                                continue;
                            }

                            layerX.Enqueue(newParentsX);
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

    private Node UpdateClickedNode(PointF graphPos)
    {
        Node node = GetNodeAtPoint(graphPos);

        MovingChild = false;
        if (node != Node.NullNode)
        {
            if (IsCtrlPressed && node.DataType != typeof(MissingReferenceInfo))
            {
                nodeToLinkFrom = node;
                if (highlightNode == node)
                {
                    highlightNode = Node.NullNode;
                }
            }
            else if (!IsCtrlPressed && nodeToLinkFrom != Node.NullNode)
            {
                if (nodes[SelectedCharacter].AreConnected(nodeToLinkFrom, node))
                {
                    Unlink(nodeToLinkFrom, node);
                }
                else
                {
                    Link(nodeToLinkFrom, node);
                }

                nodeToLinkFrom = Node.NullNode;
                oldMousePosBeforeSpawnWindow = Point.Empty;

                ShowProperties(node);
                ClearOverlayBitmap();
            }
            else if (clickedNode != node)
            {
                ShowProperties(node);
            }
        }
        else
        {
            var circleNode = TryGetNodeLinkStart(graphPos);
            if (circleNode != Node.NullNode)
            {
                if (nodeToLinkFrom == Node.NullNode)
                {
                    nodeToLinkFrom = circleNode;
                }
                else if (nodeToLinkFrom != Node.NullNode)
                {
                    Link(nodeToLinkFrom, circleNode);

                    nodeToLinkFrom = Node.NullNode;
                    oldMousePosBeforeSpawnWindow = Point.Empty;

                    ShowProperties(node);
                    ClearOverlayBitmap();
                }
            }
            else if (nodeToLinkFrom != Node.NullNode)
            {
                ShowNodeSpawnBox();
            }
        }

        clickedNode = node;
        return node;
    }

    private void SpawnContextMenu(PointF graphPos)
    {
        GraphToScreen(graphPos.X, graphPos.Y, out float screenPosX, out float screenPosY);
        Point ScreenPos = new((int)screenPosX, (int)screenPosY);

        NodeContext.Items.Clear();

        foreach (var item in GetSpawnableNodeTypes())
        {
            var button = new ToolStripMenuItem(item.ToString(), null, onClick: (_, _) =>
            {
                string character = GetProbableCharacter();
                var newNode = GetNodeFromSpawnableType((Enum.Parse<SpawnableNodeType>(item.ToString()!)), character);
                SetUpNewlySpawnedNode(newNode, graphPos);
            })
            {
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                AutoSize = true,
                BackColor = Color.FromArgb(64, 64, 64),
                ForeColor = Color.LightGray,
            };
            button.MouseEnter += (_, _) => button.ForeColor = Color.Black;
            button.MouseLeave += (_, _) => button.ForeColor = Color.LightGray;
            NodeContext.Items.Add(button);
        }

        NodeContext.Show(Graph, ScreenPos);
    }

    //todo needs some more linking for criteria or events with other node types so we auto populate. should reuse the code from the node spawning#
    private void Link(Node addFrom, Node addToThis)
    {
        if (addToThis.DataType == typeof(MissingReferenceInfo))
        {
            return;
        }

        bool linked = false;

        if (addFrom.DataType == typeof(Criterion))
        {
            if (addToThis.DataType == typeof(ItemAction))
            {
                addToThis.Data<ItemAction>()!.Criteria!.Add(addFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(UseWith))
            {
                addToThis.Data<UseWith>()!.Criteria!.Add(addFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(CriteriaList1))
            {
                addToThis.Data<CriteriaList1>()!.CriteriaList!.Add(addFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(GameEvent))
            {
                addToThis.Data<GameEvent>()!.Criteria!.Add(addFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(EventTrigger))
            {
                addToThis.Data<EventTrigger>()!.Critera!.Add(addFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(AlternateText))
            {
                addToThis.Data<AlternateText>()!.Critera!.Add(addFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(Response))
            {
                addToThis.Data<Response>()!.ResponseCriteria!.Add(addFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(BackgroundChatter))
            {
                addToThis.Data<BackgroundChatter>()!.Critera!.Add(addFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(ItemInteraction))
            {
                addToThis.Data<ItemInteraction>()!.Critera!.Add(addFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(ItemGroupInteraction))
            {
                addToThis.Data<ItemGroupInteraction>()!.Critera!.Add(addFrom.Data<Criterion>()!);
                linked = true;
            }

            if (linked)
            {
                nodes[addToThis.FileName].AddParent(addToThis, addFrom);
            }
        }
        else if (addFrom.DataType == typeof(GameEvent))
        {
            if (addToThis.DataType == typeof(ItemAction))
            {
                addToThis.Data<ItemAction>()!.OnTakeActionEvents!.Add(addFrom.Data<GameEvent>()!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(UseWith))
            {
                addToThis.Data<UseWith>()!.OnSuccessEvents!.Add(addFrom.Data<GameEvent>()!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(EventTrigger))
            {
                addToThis.Data<EventTrigger>()!.Events!.Add(addFrom.Data<GameEvent>()!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(MainStory))
            {
                addToThis.Data<MainStory>()!.GameStartEvents!.Add(addFrom.Data<GameEvent>()!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(Response))
            {
                addToThis.Data<Response>()!.ResponseEvents!.Add(addFrom.Data<GameEvent>()!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(Dialogue))
            {
                var result = MessageBox.Show("Add as StartEvent? Hit yes for StartEvent, no for CloseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    addToThis.Data<Dialogue>()!.StartEvents!.Add(addFrom.Data<GameEvent>()!);
                    linked = true;
                }
                else if (result == DialogResult.No)
                {
                    addToThis.Data<Dialogue>()!.CloseEvents!.Add(addFrom.Data<GameEvent>()!);
                    linked = true;
                }
            }
            else if (addToThis.DataType == typeof(BackgroundChatter))
            {
                addToThis.Data<BackgroundChatter>()!.StartEvents!.Add(addFrom.Data<GameEvent>()!);
                linked = true;
            }
            else if (addToThis.DataType == typeof(ItemInteraction))
            {
                var result = MessageBox.Show("Add as OnAcceptEvent? Hit yes for OnAcceptEvent, no for OnRefuseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    addToThis.Data<ItemInteraction>()!.OnAcceptEvents!.Add(addFrom.Data<GameEvent>()!);
                    linked = true;
                }
                else if (result == DialogResult.No)
                {
                    addToThis.Data<ItemInteraction>()!.OnRefuseEvents!.Add(addFrom.Data<GameEvent>()!);
                    linked = true;
                }
            }
            else if (addToThis.DataType == typeof(ItemGroupInteraction))
            {
                var result = MessageBox.Show("Add as OnAcceptEvent? Hit yes for OnAcceptEvent, no for OnRefuseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    addToThis.Data<ItemGroupInteraction>()!.OnAcceptEvents!.Add(addFrom.Data<GameEvent>()!);
                    linked = true;
                }
                else if (result == DialogResult.No)
                {
                    addToThis.Data<ItemGroupInteraction>()!.OnRefuseEvents!.Add(addFrom.Data<GameEvent>()!);
                    linked = true;
                }
            }
            else if (addToThis.DataType == typeof(Criterion))
            {
                addFrom.Data<GameEvent>()!.Criteria.Add(addToThis.Data<Criterion>()!);
                nodes[addToThis.FileName].AddParent(addToThis, addFrom);
            }

            if (linked)
            {
                nodes[addToThis.FileName].AddChild(addToThis, addFrom);
            }
        }
        else if (addFrom.DataType == typeof(ItemAction))
        {
            if (addToThis.DataType == typeof(ItemOverride))
            {
                addToThis.Data<ItemOverride>()!.ItemActions.Add(addFrom.Data<ItemAction>()!);
                nodes[addToThis.FileName].AddChild(addToThis, addFrom);
            }
            else if (addToThis.DataType == typeof(ItemGroupBehavior))
            {
                addToThis.Data<ItemGroupBehavior>()!.ItemActions.Add(addFrom.Data<ItemAction>()!);
                nodes[addToThis.FileName].AddChild(addToThis, addFrom);
            }
            else if (addToThis.DataType == typeof(Criterion))
            {
                addFrom.Data<ItemAction>()!.Criteria.Add(addToThis.Data<Criterion>()!);
                nodes[addToThis.FileName].AddChild(addToThis, addFrom);
            }
            else if (addToThis.DataType == typeof(GameEvent))
            {
                addFrom.Data<ItemAction>()!.OnTakeActionEvents.Add(addToThis.Data<GameEvent>()!);
                nodes[addToThis.FileName].AddParent(addToThis, addFrom);
            }
        }
        else if (addFrom.DataType == typeof(UseWith))
        {
            if (addToThis.DataType == typeof(ItemOverride))
            {
                addToThis.Data<ItemOverride>()!.UseWiths.Add(addFrom.Data<UseWith>()!);
                nodes[addToThis.FileName].AddChild(addToThis, addFrom);
            }
            else if (addToThis.DataType == typeof(ItemGroupBehavior))
            {
                addToThis.Data<ItemGroupBehavior>()!.UseWiths.Add(addFrom.Data<UseWith>()!);
                nodes[addToThis.FileName].AddChild(addToThis, addFrom);
            }
            else if (addToThis.DataType == typeof(Criterion))
            {
                addFrom.Data<UseWith>()!.Criteria.Add(addToThis.Data<Criterion>()!);
                nodes[addToThis.FileName].AddChild(addToThis, addFrom);
            }
            else if (addToThis.DataType == typeof(GameEvent))
            {
                addFrom.Data<UseWith>()!.OnSuccessEvents.Add(addToThis.Data<GameEvent>()!);
                nodes[addToThis.FileName].AddParent(addToThis, addFrom);
            }
        }
        else if (addFrom.DataType == typeof(ItemOverride))
        {
            if (addToThis.DataType == typeof(ItemAction))
            {
                addFrom.Data<ItemOverride>()!.ItemActions.Add(addToThis.Data<ItemAction>()!);
                nodes[addToThis.FileName].AddParent(addToThis, addFrom);
            }
            else if (addToThis.DataType == typeof(UseWith))
            {
                addFrom.Data<ItemOverride>()!.UseWiths.Add(addToThis.Data<UseWith>()!);
                nodes[addToThis.FileName].AddParent(addToThis, addFrom);
            }
        }
        else if (addFrom.DataType == typeof(ItemGroupBehavior))
        {
            if (addToThis.DataType == typeof(ItemAction))
            {
                addFrom.Data<ItemGroupBehavior>()!.ItemActions.Add(addToThis.Data<ItemAction>()!);
                nodes[addToThis.FileName].AddParent(addToThis, addFrom);
            }
            else if (addToThis.DataType == typeof(UseWith))
            {
                addFrom.Data<ItemGroupBehavior>()!.UseWiths.Add(addToThis.Data<UseWith>()!);
                nodes[addToThis.FileName].AddParent(addToThis, addFrom);
            }
        }
        else if (addFrom.DataType == typeof(Achievement))
        {
            //todo
        }
        else if (addFrom.DataType == typeof(CriteriaList1))
        {
            //todo
        }
        else if (addFrom.DataType == typeof(CriteriaGroup))
        {
            //todo
        }
        else if (addFrom.DataType == typeof(ItemGroup))
        {
            //todo
        }
        else if (addFrom.DataType == typeof(EventTrigger))
        {
            if (addToThis.DataType == typeof(Criterion))
            {
                addFrom.Data<EventTrigger>()!.Critera.Add(addToThis.Data<Criterion>()!);
                nodes[addToThis.FileName].AddChild(addToThis, addFrom);
            }
            else if (addToThis.DataType == typeof(GameEvent))
            {
                addFrom.Data<EventTrigger>()!.Events.Add(addToThis.Data<GameEvent>()!);
                nodes[addToThis.FileName].AddParent(addToThis, addFrom);
            }
        }
        else if (addFrom.DataType == typeof(CharacterGroup))
        {
            //todo
        }
        else if (addFrom.DataType == typeof(AlternateText))
        {
            if (addToThis.DataType == typeof(Criterion))
            {
                addFrom.Data<AlternateText>()!.Critera.Add(addToThis.Data<Criterion>()!);
                nodes[addToThis.FileName].AddChild(addToThis, addFrom);
            }
            else if (addToThis.DataType == typeof(Dialogue))
            {
                addToThis.Data<Dialogue>()!.AlternateTexts.Add(addFrom.Data<AlternateText>()!);
                nodes[addToThis.FileName].AddChild(addToThis, addFrom);
            }
        }
        else if (addFrom.DataType == typeof(Response))
        {
            if (addToThis.DataType == typeof(Criterion))
            {
                addFrom.Data<Response>()!.ResponseCriteria.Add(addToThis.Data<Criterion>()!);
                nodes[addToThis.FileName].AddChild(addToThis, addFrom);
            }
            else if (addToThis.DataType == typeof(GameEvent))
            {
                addFrom.Data<Response>()!.ResponseEvents.Add(addToThis.Data<GameEvent>()!);
                nodes[addToThis.FileName].AddParent(addToThis, addFrom);
            }
            else if (addToThis.DataType == typeof(Dialogue))
            {
                var result = MessageBox.Show("Lead to this dialogue from the response? Hit yes for that, no to add the response as a normal response to this dialogue", "Select Response place", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    addFrom.Data<Response>()!.Next = addToThis.Data<Dialogue>()!.ID;
                    nodes[addToThis.FileName].AddParent(addToThis, addFrom);
                }
                else if (result == DialogResult.No)
                {
                    addToThis.Data<Dialogue>()!.Responses.Add(addFrom.Data<Response>()!);
                    nodes[addToThis.FileName].AddChild(addToThis, addFrom);
                }
            }
        }
        else if (addFrom.DataType == typeof(Dialogue))
        {
            if (addToThis.DataType == typeof(GameEvent))
            {
                var result = MessageBox.Show("Add as StartEvent? Hit yes for StartEvent, no for CloseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    addFrom.Data<Dialogue>()!.StartEvents!.Add(addToThis.Data<GameEvent>()!);
                    nodes[addToThis.FileName].AddParent(addToThis, addFrom);
                }
                else if (result == DialogResult.No)
                {
                    addFrom.Data<Dialogue>()!.CloseEvents!.Add(addToThis.Data<GameEvent>()!);
                    nodes[addToThis.FileName].AddParent(addToThis, addFrom);
                }
            }
            else if (addToThis.DataType == typeof(Response))
            {
                var result = MessageBox.Show("Add as a response? Hit yes for Response, no for the response leading to this dialogue", "Select Response place", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    addFrom.Data<Dialogue>()!.Responses.Add(addToThis.Data<Response>()!);
                    nodes[addToThis.FileName].AddParent(addToThis, addFrom);
                }
                else if (result == DialogResult.No)
                {
                    addToThis.Data<Response>()!.Next = addFrom.Data<Dialogue>()!.ID;
                    nodes[addToThis.FileName].AddChild(addToThis, addFrom);
                }
            }
            else if (addToThis.DataType == typeof(AlternateText))
            {
                addFrom.Data<Dialogue>()!.AlternateTexts.Add(addToThis.Data<AlternateText>()!);
                nodes[addToThis.FileName].AddParent(addToThis, addFrom);
            }
        }
        else if (addFrom.DataType == typeof(BackgroundChatter))
        {
            if (addToThis.DataType == typeof(Criterion))
            {
                addFrom.Data<BackgroundChatter>()!.Critera.Add(addToThis.Data<Criterion>()!);
                nodes[addToThis.FileName].AddChild(addToThis, addFrom);
            }
            else if (addToThis.DataType == typeof(GameEvent))
            {
                addFrom.Data<BackgroundChatter>()!.StartEvents.Add(addToThis.Data<GameEvent>()!);
                nodes[addToThis.FileName].AddParent(addToThis, addFrom);
            }
            else if (addToThis.DataType == typeof(BackgroundChatterResponse))
            {
                addFrom.Data<BackgroundChatter>()!.Responses.Add(addToThis.Data<BackgroundChatterResponse>()!);
                nodes[addToThis.FileName].AddParent(addToThis, addFrom);
            }
        }
        else if (addFrom.DataType == typeof(BackgroundChatterResponse))
        {
            if (addToThis.DataType == typeof(BackgroundChatter))
            {
                addToThis.Data<BackgroundChatter>()!.Responses.Add(addFrom.Data<BackgroundChatterResponse>()!);
                nodes[addToThis.FileName].AddChild(addToThis, addFrom);
            }
        }
        else if (addFrom.DataType == typeof(Trait))
        {
            //todo
        }
        else if (addFrom.DataType == typeof(ExtendedDetail))
        {
            //todo
        }
        else if (addFrom.DataType == typeof(Quest))
        {
            //todo
        }
        else if (addFrom.DataType == typeof(ItemInteraction))
        {
            //todo
        }
        else if (addFrom.DataType == typeof(ItemGroupInteraction))
        {
            //todo
        }
        else if (addFrom.DataType == typeof(Value))
        {
            //todo
        }

        NodeLinker.UpdateLinks(addToThis, SelectedCharacter, nodes[selectedCharacter]);

        addFrom = Node.NullNode;
    }

    private void Unlink(Node removeFrom, Node removeThis, bool removeAll = false)
    {
        if (removeThis.DataType == typeof(MissingReferenceInfo))
        {
            return;
        }

        bool linked = false;

        if (removeFrom.DataType == typeof(Criterion))
        {
            if (removeThis.DataType == typeof(ItemAction))
            {
                removeThis.Data<ItemAction>()!.Criteria!.Remove(removeFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (removeThis.DataType == typeof(UseWith))
            {
                removeThis.Data<UseWith>()!.Criteria!.Remove(removeFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (removeThis.DataType == typeof(CriteriaList1))
            {
                removeThis.Data<CriteriaList1>()!.CriteriaList!.Remove(removeFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (removeThis.DataType == typeof(GameEvent))
            {
                removeThis.Data<GameEvent>()!.Criteria!.Remove(removeFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (removeThis.DataType == typeof(EventTrigger))
            {
                removeThis.Data<EventTrigger>()!.Critera!.Remove(removeFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (removeThis.DataType == typeof(AlternateText))
            {
                removeThis.Data<AlternateText>()!.Critera!.Remove(removeFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (removeThis.DataType == typeof(Response))
            {
                removeThis.Data<Response>()!.ResponseCriteria!.Remove(removeFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (removeThis.DataType == typeof(BackgroundChatter))
            {
                removeThis.Data<BackgroundChatter>()!.Critera!.Remove(removeFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (removeThis.DataType == typeof(ItemInteraction))
            {
                removeThis.Data<ItemInteraction>()!.Critera!.Remove(removeFrom.Data<Criterion>()!);
                linked = true;
            }
            else if (removeThis.DataType == typeof(ItemGroupInteraction))
            {
                removeThis.Data<ItemGroupInteraction>()!.Critera!.Remove(removeFrom.Data<Criterion>()!);
                linked = true;
            }

            if (linked)
            {
                nodes[removeThis.FileName].RemoveParent(removeThis, removeFrom);
            }
        }
        else if (removeFrom.DataType == typeof(GameEvent))
        {
            if (removeThis.DataType == typeof(ItemAction))
            {
                removeThis.Data<ItemAction>()!.OnTakeActionEvents!.Remove(removeFrom.Data<GameEvent>()!);
                linked = true;
            }
            else if (removeThis.DataType == typeof(UseWith))
            {
                removeThis.Data<UseWith>()!.OnSuccessEvents!.Remove(removeFrom.Data<GameEvent>()!);
                linked = true;
            }
            else if (removeThis.DataType == typeof(EventTrigger))
            {
                removeThis.Data<EventTrigger>()!.Events!.Remove(removeFrom.Data<GameEvent>()!);
                linked = true;
            }
            else if (removeThis.DataType == typeof(MainStory))
            {
                removeThis.Data<MainStory>()!.GameStartEvents!.Remove(removeFrom.Data<GameEvent>()!);
                linked = true;
            }
            else if (removeThis.DataType == typeof(Response))
            {
                removeThis.Data<Response>()!.ResponseEvents!.Remove(removeFrom.Data<GameEvent>()!);
                linked = true;
            }
            else if (removeThis.DataType == typeof(Dialogue))
            {
                DialogResult result = DialogResult.None;
                if (!removeAll)
                {
                    result = MessageBox.Show("Remove as StartEvent? Hit yes for StartEvent, no for CloseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }
                if (result == DialogResult.Yes || removeAll)
                {
                    removeThis.Data<Dialogue>()!.StartEvents!.Remove(removeFrom.Data<GameEvent>()!);
                    linked = true;
                }
                if (result == DialogResult.No || removeAll)
                {
                    removeThis.Data<Dialogue>()!.CloseEvents!.Remove(removeFrom.Data<GameEvent>()!);
                    linked = true;
                }
            }
            else if (removeThis.DataType == typeof(BackgroundChatter))
            {
                removeThis.Data<BackgroundChatter>()!.StartEvents!.Remove(removeFrom.Data<GameEvent>()!);
                linked = true;
            }
            else if (removeThis.DataType == typeof(ItemInteraction))
            {
                DialogResult result = DialogResult.None;
                if (!removeAll)
                {
                    result = MessageBox.Show("Remove as OnAcceptEvent? Hit yes for OnAcceptEvent, no for OnRefuseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }
                if (result == DialogResult.Yes || removeAll)
                {
                    removeThis.Data<ItemInteraction>()!.OnAcceptEvents!.Remove(removeFrom.Data<GameEvent>()!);
                    linked = true;
                }
                if (result == DialogResult.No || removeAll)
                {
                    removeThis.Data<ItemInteraction>()!.OnRefuseEvents!.Remove(removeFrom.Data<GameEvent>()!);
                    linked = true;
                }
            }
            else if (removeThis.DataType == typeof(ItemGroupInteraction))
            {
                DialogResult result = DialogResult.None;
                if (!removeAll)
                {
                    result = MessageBox.Show("Remove as OnAcceptEvent? Hit yes for OnAcceptEvent, no for OnRefuseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }
                if (result == DialogResult.Yes || removeAll)
                {
                    removeThis.Data<ItemGroupInteraction>()!.OnAcceptEvents!.Remove(removeFrom.Data<GameEvent>()!);
                    linked = true;
                }
                if (result == DialogResult.No || removeAll)
                {
                    removeThis.Data<ItemGroupInteraction>()!.OnRefuseEvents!.Remove(removeFrom.Data<GameEvent>()!);
                    linked = true;
                }
            }
            else if (removeThis.DataType == typeof(Criterion))
            {
                removeFrom.Data<GameEvent>()!.Criteria.Remove(removeThis.Data<Criterion>()!);
                nodes[removeThis.FileName].RemoveParent(removeThis, removeFrom);
            }

            if (linked)
            {
                nodes[removeThis.FileName].RemoveChild(removeThis, removeFrom);
            }
        }
        else if (removeFrom.DataType == typeof(ItemAction))
        {
            if (removeThis.DataType == typeof(ItemOverride))
            {
                removeThis.Data<ItemOverride>()!.ItemActions.Remove(removeFrom.Data<ItemAction>()!);
                nodes[removeThis.FileName].RemoveChild(removeThis, removeFrom);
            }
            else if (removeThis.DataType == typeof(ItemGroupBehavior))
            {
                removeThis.Data<ItemGroupBehavior>()!.ItemActions.Remove(removeFrom.Data<ItemAction>()!);
                nodes[removeThis.FileName].RemoveChild(removeThis, removeFrom);
            }
            else if (removeThis.DataType == typeof(Criterion))
            {
                removeFrom.Data<ItemAction>()!.Criteria.Remove(removeThis.Data<Criterion>()!);
                nodes[removeThis.FileName].RemoveChild(removeThis, removeFrom);
            }
            else if (removeThis.DataType == typeof(GameEvent))
            {
                removeFrom.Data<ItemAction>()!.OnTakeActionEvents.Remove(removeThis.Data<GameEvent>()!);
                nodes[removeThis.FileName].RemoveParent(removeThis, removeFrom);
            }
        }
        else if (removeFrom.DataType == typeof(UseWith))
        {
            if (removeThis.DataType == typeof(ItemOverride))
            {
                removeThis.Data<ItemOverride>()!.UseWiths.Remove(removeFrom.Data<UseWith>()!);
                nodes[removeThis.FileName].RemoveChild(removeThis, removeFrom);
            }
            else if (removeThis.DataType == typeof(ItemGroupBehavior))
            {
                removeThis.Data<ItemGroupBehavior>()!.UseWiths.Remove(removeFrom.Data<UseWith>()!);
                nodes[removeThis.FileName].RemoveChild(removeThis, removeFrom);
            }
            else if (removeThis.DataType == typeof(Criterion))
            {
                removeFrom.Data<UseWith>()!.Criteria.Remove(removeThis.Data<Criterion>()!);
                nodes[removeThis.FileName].RemoveChild(removeThis, removeFrom);
            }
            else if (removeThis.DataType == typeof(GameEvent))
            {
                removeFrom.Data<UseWith>()!.OnSuccessEvents.Remove(removeThis.Data<GameEvent>()!);
                nodes[removeThis.FileName].RemoveParent(removeThis, removeFrom);
            }
        }
        else if (removeFrom.DataType == typeof(ItemOverride))
        {
            if (removeThis.DataType == typeof(ItemAction))
            {
                removeFrom.Data<ItemOverride>()!.ItemActions.Remove(removeThis.Data<ItemAction>()!);
                nodes[removeThis.FileName].RemoveParent(removeThis, removeFrom);
            }
            else if (removeThis.DataType == typeof(UseWith))
            {
                removeFrom.Data<ItemOverride>()!.UseWiths.Remove(removeThis.Data<UseWith>()!);
                nodes[removeThis.FileName].RemoveParent(removeThis, removeFrom);
            }
        }
        else if (removeFrom.DataType == typeof(ItemGroupBehavior))
        {
            if (removeThis.DataType == typeof(ItemAction))
            {
                removeFrom.Data<ItemGroupBehavior>()!.ItemActions.Remove(removeThis.Data<ItemAction>()!);
                nodes[removeThis.FileName].RemoveParent(removeThis, removeFrom);
            }
            else if (removeThis.DataType == typeof(UseWith))
            {
                removeFrom.Data<ItemGroupBehavior>()!.UseWiths.Remove(removeThis.Data<UseWith>()!);
                nodes[removeThis.FileName].RemoveParent(removeThis, removeFrom);
            }
        }
        else if (removeFrom.DataType == typeof(Achievement))
        {
            //todo
        }
        else if (removeFrom.DataType == typeof(CriteriaList1))
        {
            //todo
        }
        else if (removeFrom.DataType == typeof(CriteriaGroup))
        {
            //todo
        }
        else if (removeFrom.DataType == typeof(ItemGroup))
        {
            //todo
        }
        else if (removeFrom.DataType == typeof(EventTrigger))
        {
            if (removeThis.DataType == typeof(Criterion))
            {
                removeFrom.Data<EventTrigger>()!.Critera.Remove(removeThis.Data<Criterion>()!);
                nodes[removeThis.FileName].RemoveChild(removeThis, removeFrom);
            }
            else if (removeThis.DataType == typeof(GameEvent))
            {
                removeFrom.Data<EventTrigger>()!.Events.Remove(removeThis.Data<GameEvent>()!);
                nodes[removeThis.FileName].RemoveParent(removeThis, removeFrom);
            }
        }
        else if (removeFrom.DataType == typeof(CharacterGroup))
        {
            //todo
        }
        else if (removeFrom.DataType == typeof(AlternateText))
        {
            if (removeThis.DataType == typeof(Criterion))
            {
                removeFrom.Data<AlternateText>()!.Critera.Remove(removeThis.Data<Criterion>()!);
                nodes[removeThis.FileName].RemoveChild(removeThis, removeFrom);
            }
            else if (removeThis.DataType == typeof(Dialogue))
            {
                removeThis.Data<Dialogue>()!.AlternateTexts.Remove(removeFrom.Data<AlternateText>()!);
                nodes[removeThis.FileName].RemoveChild(removeThis, removeFrom);
            }
        }
        else if (removeFrom.DataType == typeof(Response))
        {
            if (removeThis.DataType == typeof(Criterion))
            {
                removeFrom.Data<Response>()!.ResponseCriteria.Remove(removeThis.Data<Criterion>()!);
                nodes[removeThis.FileName].RemoveChild(removeThis, removeFrom);
            }
            else if (removeThis.DataType == typeof(GameEvent))
            {
                removeFrom.Data<Response>()!.ResponseEvents.Remove(removeThis.Data<GameEvent>()!);
                nodes[removeThis.FileName].RemoveParent(removeThis, removeFrom);
            }
            else if (removeThis.DataType == typeof(Dialogue))
            {
                DialogResult result = DialogResult.None;
                if (!removeAll)
                {
                    result = MessageBox.Show("Lead to this dialogue from the response? Hit yes for that, no to Remove the response as a normal response to this dialogue", "Select Response place", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }
                if (result == DialogResult.Yes || removeAll)
                {
                    removeFrom.Data<Response>()!.Next = 0;
                    nodes[removeThis.FileName].RemoveParent(removeThis, removeFrom);
                }
                if (result == DialogResult.No || removeAll)
                {
                    removeThis.Data<Dialogue>()!.Responses.Remove(removeFrom.Data<Response>()!);
                    nodes[removeThis.FileName].RemoveChild(removeThis, removeFrom);
                }
            }
        }
        else if (removeFrom.DataType == typeof(Dialogue))
        {
            if (removeThis.DataType == typeof(GameEvent))
            {
                DialogResult result = DialogResult.None;
                if (!removeAll)
                {
                    result = MessageBox.Show("Remove as StartEvent? Hit yes for StartEvent, no for CloseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }
                if (result == DialogResult.Yes || removeAll)
                {
                    removeFrom.Data<Dialogue>()!.StartEvents!.Remove(removeThis.Data<GameEvent>()!);
                    nodes[removeThis.FileName].RemoveParent(removeThis, removeFrom);
                }
                if (result == DialogResult.No || removeAll)
                {
                    removeFrom.Data<Dialogue>()!.CloseEvents!.Remove(removeThis.Data<GameEvent>()!);
                    nodes[removeThis.FileName].RemoveParent(removeThis, removeFrom);
                }
            }
            else if (removeThis.DataType == typeof(Response))
            {
                DialogResult result = DialogResult.None;
                if (!removeAll)
                {
                    result = MessageBox.Show("Remove as a response? Hit yes for Response, no for the response leading to this dialogue", "Select Response place", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }
                if (result == DialogResult.Yes || removeAll)
                {
                    removeFrom.Data<Dialogue>()!.Responses.Remove(removeThis.Data<Response>()!);
                    nodes[removeThis.FileName].RemoveParent(removeThis, removeFrom);
                }
                if (result == DialogResult.No || removeAll)
                {
                    removeThis.Data<Response>()!.Next = 0;
                    nodes[removeThis.FileName].RemoveChild(removeThis, removeFrom);
                }
            }
            else if (removeThis.DataType == typeof(AlternateText))
            {
                removeFrom.Data<Dialogue>()!.AlternateTexts.Remove(removeThis.Data<AlternateText>()!);
                nodes[removeThis.FileName].RemoveParent(removeThis, removeFrom);
            }
        }
        else if (removeFrom.DataType == typeof(BackgroundChatter))
        {
            if (removeThis.DataType == typeof(Criterion))
            {
                removeFrom.Data<BackgroundChatter>()!.Critera.Remove(removeThis.Data<Criterion>()!);
                nodes[removeThis.FileName].RemoveChild(removeThis, removeFrom);
            }
            else if (removeThis.DataType == typeof(GameEvent))
            {
                removeFrom.Data<BackgroundChatter>()!.StartEvents.Remove(removeThis.Data<GameEvent>()!);
                nodes[removeThis.FileName].RemoveParent(removeThis, removeFrom);
            }
            else if (removeThis.DataType == typeof(BackgroundChatterResponse))
            {
                removeFrom.Data<BackgroundChatter>()!.Responses.Remove(removeThis.Data<BackgroundChatterResponse>()!);
                nodes[removeThis.FileName].RemoveParent(removeThis, removeFrom);
            }
        }
        else if (removeFrom.DataType == typeof(BackgroundChatterResponse))
        {
            if (removeThis.DataType == typeof(BackgroundChatter))
            {
                removeThis.Data<BackgroundChatter>()!.Responses.Remove(removeFrom.Data<BackgroundChatterResponse>()!);
                nodes[removeThis.FileName].RemoveChild(removeThis, removeFrom);
            }
        }
        else if (removeFrom.DataType == typeof(Trait))
        {
            //todo
        }
        else if (removeFrom.DataType == typeof(ExtendedDetail))
        {
            //todo
        }
        else if (removeFrom.DataType == typeof(Quest))
        {
            //todo
        }
        else if (removeFrom.DataType == typeof(ItemInteraction))
        {
            //todo
        }
        else if (removeFrom.DataType == typeof(ItemGroupInteraction))
        {
            //todo
        }
        else if (removeFrom.DataType == typeof(Value))
        {
            //todo
        }

        NodeLinker.UpdateLinks(removeThis, SelectedCharacter, nodes[selectedCharacter]);
    }

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
                Criterion criterion = node.Data<Criterion>()!;

                Label label = GetLabel("Order:");
                PropertyInspector.Controls.Add(label);

                NumericUpDown sortOrder = GetNumericUpDown(criterion.Order);
                sortOrder.DecimalPlaces = 0;
                sortOrder.Minimum = 0;
                sortOrder.ValueChanged += (_, _) => criterion.Order = (int)sortOrder.Value;
                sortOrder.ValueChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
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
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);

                        ComboBox clothing = GetComboBox(node);
                        clothing.Items.AddRange(Enum.GetNames(typeof(Clothes)));
                        if (int.TryParse(criterion.Value!, out int res))
                        {
                            if (res < clothing.Items.Count && res >= 1)
                            {
                                clothing.SelectedIndex = res - 1;
                            }
                            else
                            {
                                clothing.SelectedIndex = 0;
                                criterion.Value = 0.ToString();
                            }
                        }
                        else
                        {
                            clothing.SelectedIndex = 0;
                            criterion.Value = 0.ToString();
                        }
                        clothing.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Value = clothing.SelectedIndex!.ToString());
                        PropertyInspector.Controls.Add(clothing);

                        ComboBox set = GetComboBox(node);
                        set.Items.AddRange(Enum.GetNames(typeof(ClothingSet)));
                        set.SelectedIndex = criterion.Option!;
                        set.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Option = set.SelectedIndex!);
                        PropertyInspector.Controls.Add(set);

                        PutBoolValue(node, criterion);

                        break;
                    }
                    case CompareTypes.CoinFlip:
                    {
                        PutCompareType(node, criterion);
                        break;
                    }
                    case CompareTypes.CompareValues:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);

                        ComboBox valueChar1 = GetComboBox(node);
                        if (criterion.Key == Player)
                        {
                            valueChar1.Items.AddRange([.. Story.PlayerValues!]);
                        }
                        else
                        {
                            valueChar1.Items.AddRange([.. Stories[criterion.Character!].StoryValues!]);
                        }
                        valueChar1.SelectedItem = criterion.Key!;
                        valueChar1.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Key = valueChar1.SelectedItem!.ToString());
                        PropertyInspector.Controls.Add(valueChar1);

                        ComboBox formula = GetComboBox(node);
                        formula.Items.AddRange(Enum.GetNames(typeof(ValueSpecificFormulas)));
                        formula.SelectedIndex = (int)criterion.ValueFormula!;
                        formula.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.ValueFormula = (ValueSpecificFormulas)formula.SelectedIndex!);
                        PropertyInspector.Controls.Add(formula);

                        PutCharacter2(node, criterion);

                        ComboBox valueChar2 = GetComboBox(node);
                        if (criterion.Key2 == Player)
                        {
                            valueChar2.Items.AddRange([.. Story.PlayerValues!]);
                        }
                        else
                        {
                            valueChar2.Items.AddRange([.. Stories[criterion.Character2!].StoryValues!]);
                        }
                        valueChar2.SelectedItem = criterion.Key2!;
                        valueChar2.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Key2 = valueChar2.SelectedItem!.ToString());
                        PropertyInspector.Controls.Add(valueChar2);

                        break;
                    }
                    case CompareTypes.CriteriaGroup:
                    {
                        PutCompareType(node, criterion);

                        ComboBox group = GetComboBox(node);
                        for (int i = 0; i < Story.CriteriaGroups!.Count; i++)
                        {
                            group.Items.Add(Story.CriteriaGroups[i].Name!);
                        }
                        group.SelectedItem = criterion.Value!;
                        group.SelectedIndex = group.SelectedIndex;
                        group.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Value = group.SelectedItem!.ToString());
                        PropertyInspector.Controls.Add(group);

                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.CutScene:
                    {
                        PutCompareType(node, criterion);

                        ComboBox cutscene = GetComboBox(node);
                        cutscene.Items.AddRange(Enum.GetNames(typeof(CutscenePlaying)));
                        if (int.TryParse(criterion.Value!, out int res))
                        {
                            if (res < cutscene.Items.Count && res >= 1)
                            {
                                cutscene.SelectedIndex = res - 1;
                            }
                            else
                            {
                                cutscene.SelectedIndex = 0;
                                criterion.Value = 0.ToString();
                            }
                        }
                        else
                        {
                            cutscene.SelectedIndex = 0;
                            criterion.Value = 0.ToString();
                        }
                        cutscene.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Value = (cutscene.SelectedIndex! + 1).ToString());
                        PropertyInspector.Controls.Add(cutscene);

                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.Dialogue:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);

                        ComboBox dialogue = GetComboBox(node);
                        for (int i = 0; i < Stories[criterion.Character!].Dialogues!.Count; i++)
                        {
                            dialogue.Items.Add(Stories[criterion.Character!].Dialogues![i].ID.ToString());
                        }
                        dialogue.SelectedItem = criterion.Value!;
                        dialogue.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Value = dialogue.SelectedItem!.ToString());
                        PropertyInspector.Controls.Add(dialogue);

                        ComboBox status = GetComboBox(node);
                        status.Items.AddRange(Enum.GetNames(typeof(DialogueStatuses)));
                        status.SelectedItem = ((DialogueStatuses)criterion.Option!).ToString();
                        status.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Option = (int)Enum.Parse<DialogueStatuses>(status.SelectedItem!.ToString()!));
                        PropertyInspector.Controls.Add(status);
                        break;
                    }
                    case CompareTypes.Distance:
                    {
                        PutCompareType(node, criterion);

                        TextBox obj1 = new()
                        {
                            Dock = DockStyle.Fill,
                            TextAlign = HorizontalAlignment.Center,
                            Text = criterion.Key
                        };
                        obj1.TextChanged += (_, args) => criterion.Key = obj1.Text;
                        obj1.TextChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
                        PropertyInspector.Controls.Add(obj1);

                        TextBox obj2 = new()
                        {
                            Dock = DockStyle.Fill,
                            TextAlign = HorizontalAlignment.Center,
                            Text = criterion.Key2
                        };
                        obj2.TextChanged += (_, args) => criterion.Key2 = obj2.Text;
                        obj2.TextChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
                        PropertyInspector.Controls.Add(obj2);

                        PutComparison(node, criterion);

                        PutNumericOption(node, criterion);

                        break;
                    }
                    case CompareTypes.Door:
                    {

                        PutCompareType(node, criterion);

                        ComboBox door = GetComboBox(node);
                        door.Items.AddRange(Enum.GetNames(typeof(Doors)));
                        door.SelectedItem = criterion.Key?.Replace(" ", "");
                        door.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Key = door.SelectedItem!.ToString());
                        PropertyInspector.Controls.Add(door);

                        ComboBox doorstate = GetComboBox(node);
                        doorstate.Items.AddRange(Enum.GetNames(typeof(DoorOptionValues)));
                        doorstate.SelectedIndex = criterion.Option!;
                        doorstate.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Option = (doorstate.SelectedIndex));
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
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);

                        PutEquals(node, criterion);

                        ComboBox character = GetComboBox(node);
                        character.Items.AddRange(Enum.GetNames(typeof(IntimateCharacters)));
                        character.SelectedItem = criterion.Value;
                        character.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Value = (string)character.SelectedItem!);
                        PropertyInspector.Controls.Add(character);

                        break;
                    }
                    case CompareTypes.IntimacyState:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);

                        PutEquals(node, criterion);

                        ComboBox state = GetComboBox(node);
                        state.Items.AddRange(Enum.GetNames(typeof(SexualActs)));
                        state.SelectedItem = criterion.Value;
                        state.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Value = (string)state.SelectedItem!);
                        PropertyInspector.Controls.Add(state);

                        break;
                    }
                    case CompareTypes.InZone:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);
                        PutZone(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.InVicinity:
                    case CompareTypes.InVicinityAndVision:
                    case CompareTypes.IsOnlyInVicinityOf:
                    case CompareTypes.IsOnlyInVisionOf:
                    case CompareTypes.IsOnlyInVicinityAndVisionOf:
                    {
                        PutCharacter1(node, criterion);
                        PutCompareType(node, criterion);
                        PutCharacter2(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.Item:
                    {
                        PutCompareType(node, criterion);

                        PutItem(node, criterion);

                        ComboBox state = GetComboBox(node);
                        state.Items.AddRange(Enum.GetNames(typeof(ItemComparisonTypes)));
                        state.SelectedItem = criterion.ItemComparison.ToString();
                        state.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.ItemComparison = Enum.Parse<ItemComparisonTypes>(state.SelectedItem!.ToString()!));
                        PropertyInspector.Controls.Add(state);

                        break;
                    }
                    case CompareTypes.IsBeingSpokenTo:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.IsAloneWithPlayer:
                    {
                        PutCharacter1(node, criterion);
                        PutCompareType(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.IsDLCActive:
                    {
                        //todo
                        break;
                    }
                    case CompareTypes.IsCharacterEnabled:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.IsCurrentlyBeingUsed:
                    {
                        PutCompareType(node, criterion);
                        PutItem(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.IsCurrentlyUsing:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);
                        PutItem(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.IsExplicitGameVersion:
                    {
                        PutCompareType(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.IsGameUncensored:
                    {
                        PutCompareType(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.IsPackageInstalled:
                    {
                        //todo
                        break;
                    }
                    case CompareTypes.IsInFrontOf:
                    {
                        PutCharacter1(node, criterion);
                        PutCompareType(node, criterion);
                        PutCharacter2(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.IsInHouse:
                    {
                        PutCharacter1(node, criterion);
                        PutCompareType(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.IsNewGame:
                    {
                        PutCompareType(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.IsZoneEmpty:
                    {
                        PutCompareType(node, criterion);
                        PutZone(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.ItemFromItemGroup:
                    {
                        //todo
                        break;
                    }
                    case CompareTypes.MetByPlayer:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.Personality:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);

                        ComboBox trait = GetComboBox(node);
                        trait.Items.AddRange(Enum.GetNames(typeof(PersonalityTraits)));
                        if (int.TryParse(criterion.Key!, out int res))
                        {
                            if (res < trait.Items.Count && res >= 1)
                            {
                                trait.SelectedIndex = res - 1;
                            }
                            else
                            {
                                trait.SelectedIndex = 0;
                                criterion.Key = 0.ToString();
                            }
                        }
                        else
                        {
                            trait.SelectedIndex = 0;
                            criterion.Key = 0.ToString();
                        }
                        trait.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Key = trait.SelectedIndex.ToString());
                        PropertyInspector.Controls.Add(trait);

                        PutComparison(node, criterion);
                        PutNumericValue(node, criterion);

                        break;
                    }
                    case CompareTypes.PlayerGender:
                    {
                        PutCompareType(node, criterion);

                        ComboBox gender = GetComboBox(node);
                        gender.Items.AddRange(Enum.GetNames(typeof(Gender)));
                        gender.SelectedItem = criterion.Value;
                        gender.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Value = gender.SelectedItem!.ToString());
                        PropertyInspector.Controls.Add(gender);

                        break;
                    }
                    case CompareTypes.PlayerInventory:
                    {
                        PutCompareType(node, criterion);

                        ComboBox state = GetComboBox(node);
                        state.Items.AddRange(Enum.GetNames(typeof(PlayerInventoryOptions)));
                        state.SelectedItem = criterion.PlayerInventoryOption.ToString();
                        state.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.PlayerInventoryOption = Enum.Parse<PlayerInventoryOptions>(state.SelectedItem!.ToString()!));
                        PropertyInspector.Controls.Add(state);

                        PutItem(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.PlayerPrefs:
                    {
                        PutCompareType(node, criterion);

                        ComboBox pref = GetComboBox(node);
                        pref.Items.AddRange(Enum.GetNames(typeof(PlayerPrefs)));
                        pref.SelectedItem = criterion.Key;
                        pref.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Key = pref.SelectedItem!.ToString());
                        PropertyInspector.Controls.Add(pref);

                        PutComparison(node, criterion);
                        PutTextValue(node, criterion);

                        break;
                    }
                    case CompareTypes.Posing:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);

                        ComboBox option = GetComboBox(node);
                        option.Items.AddRange(Enum.GetNames(typeof(PoseOptions)));
                        option.SelectedItem = criterion.PoseOption.ToString();
                        option.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.PoseOption = Enum.Parse<PoseOptions>(option.SelectedItem!.ToString()!));
                        option.SelectedIndexChanged += (_, _) => ShowProperties(node);
                        PropertyInspector.Controls.Add(option);

                        if (criterion.PoseOption == PoseOptions.CurrentPose)
                        {
                            PutEquals(node, criterion);

                            ComboBox pose = GetComboBox(node);
                            pose.Items.AddRange(Enum.GetNames(typeof(Poses)));
                            pose.SelectedItem = Enum.Parse<Poses>(criterion.Value!).ToString();
                            pose.AddComboBoxHandler(node, nodes[SelectedCharacter], (sender, values) => criterion.Value = ((int)Enum.Parse<Poses>(pose.SelectedItem!.ToString()!)).ToString());
                            PropertyInspector.Controls.Add(pose);
                        }
                        else
                        {
                            PutBoolValue(node, criterion);
                        }
                        break;
                    }
                    case CompareTypes.Property:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);

                        ComboBox property = GetComboBox(node);
                        property.Items.AddRange(Enum.GetNames(typeof(InteractiveProperties)));
                        property.SelectedItem = Enum.Parse<InteractiveProperties>(criterion.Value!).ToString();
                        property.AddComboBoxHandler(node, nodes[SelectedCharacter], (sender, values) => criterion.Value = ((int)Enum.Parse<InteractiveProperties>(property.SelectedItem!.ToString()!)).ToString());
                        PropertyInspector.Controls.Add(property);

                        PutBoolValue(node, criterion);

                        break;
                    }
                    case CompareTypes.Quest:
                    {
                        PutCompareType(node, criterion);

                        ComboBox quest = GetComboBox(node);
                        foreach (string key in Stories.Keys)
                        {
                            for (int i = 0; i < Stories[key].Quests!.Count; i++)
                            {
                                quest.Items.Add(Stories[key].Quests![i].Name!);
                            }
                        }
                        quest.SelectedItem = criterion.Key!;
                        quest.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) =>
                        {
                            //todo implement quest store or sth
                            criterion.Key2 = quest.SelectedItem!.ToString();
                            foreach (string key in Stories.Keys)
                            {
                                for (int i = 0; i < Stories[key].Quests!.Count; i++)
                                {
                                    if (Stories[key].Quests![i].Name == criterion.Key2)
                                    {
                                        criterion.Key = Stories[key].Quests![i].ID;
                                    }
                                }
                            }
                        });
                        PropertyInspector.Controls.Add(quest);

                        PutEquals(node, criterion);

                        ComboBox obtained = GetComboBox(node);
                        obtained.Items.AddRange(Enum.GetNames(typeof(QuestStatus)));
                        obtained.SelectedItem = Enum.Parse<QuestStatus>(criterion.Value!).ToString();
                        obtained.AddComboBoxHandler(node, nodes[SelectedCharacter], (sender, values) => criterion.Value = ((int)Enum.Parse<QuestStatus>(obtained.SelectedItem!.ToString()!)).ToString());
                        PropertyInspector.Controls.Add(obtained);
                        break;
                    }
                    case CompareTypes.SameZoneAs:
                    {
                        PutCharacter1(node, criterion);
                        PutCompareType(node, criterion);
                        PutCharacter2(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.ScreenFadeVisible:
                    {
                        PutCompareType(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.Social:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);

                        ComboBox social = GetComboBox(node);
                        social.Items.AddRange(Enum.GetNames(typeof(SocialStatuses)));
                        social.SelectedItem = criterion.SocialStatus!.ToString();
                        social.AddComboBoxHandler(node, nodes[SelectedCharacter], (sender, values) => criterion.SocialStatus = Enum.Parse<SocialStatuses>(social.SelectedItem!.ToString()!));
                        PropertyInspector.Controls.Add(social);

                        PutComparison(node, criterion);
                        PutNumericValue(node, criterion);

                        break;
                    }
                    case CompareTypes.State:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);

                        ComboBox state = GetComboBox(node);
                        state.Items.AddRange(Enum.GetNames(typeof(InteractiveStates)));
                        state.SelectedItem = Enum.Parse<InteractiveStates>(criterion.Value!).ToString();
                        state.AddComboBoxHandler(node, nodes[SelectedCharacter], (sender, values) => criterion.Value = ((int)Enum.Parse<InteractiveStates>(state.SelectedItem!.ToString()!)).ToString());
                        PropertyInspector.Controls.Add(state);

                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.Value:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion).SelectedIndexChanged += (_, _) => ShowProperties(node);

                        ComboBox value = GetComboBox(node);
                        if (criterion.Character == Player)
                        {
                            for (int i = 0; i < Story.PlayerValues!.Count; i++)
                            {
                                value.Items.Add(Story.PlayerValues![i]);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < Stories[criterion.Character!].StoryValues!.Count; i++)
                            {
                                value.Items.Add(Stories[criterion.Character!].StoryValues![i]);
                            }
                        }
                        value.SelectedItem = criterion.Key!;
                        value.AddComboBoxHandler(node, nodes[SelectedCharacter], (sender, values) => criterion.Key = value.SelectedItem!.ToString());
                        PropertyInspector.Controls.Add(value);

                        PutComparison(node, criterion);

                        TextBox obj2 = new()
                        {
                            Dock = DockStyle.Fill,
                            TextAlign = HorizontalAlignment.Center,
                            Text = criterion.Value
                        };
                        obj2.TextChanged += (_, args) => criterion.Value = obj2.Text;
                        obj2.TextChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
                        PropertyInspector.Controls.Add(obj2);

                        break;
                    }
                    case CompareTypes.Vision:
                    {
                        PutCharacter1(node, criterion);
                        PutCompareType(node, criterion);
                        PutCharacter2(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }
                    case CompareTypes.UseLegacyIntimacy:
                    {
                        PutCompareType(node, criterion);
                        PutBoolValue(node, criterion);
                        break;
                    }

                    case CompareTypes.Never:
                    case CompareTypes.None:
                    default:
                    {
                        PutCompareType(node, criterion);
                        break;
                    }
                }
                break;
            }
            case NodeType.BGC:
            {
                PropertyInspector.RowCount = 2;
                PropertyInspector.ColumnCount = 9;

                Label label = GetLabel(node.FileName + "'s\n Background Chatter" + node.ID + "\nSpeaking to:");
                PropertyInspector.Controls.Add(label);

                BackgroundChatter dialogue = node.Data<BackgroundChatter>()!;

                ComboBox talkingTo = GetComboBox(node);
                talkingTo.Items.AddRange(Enum.GetNames(typeof(StoryEnums.AnybodyCharacters)));
                talkingTo.SelectedItem = dialogue.SpeakingTo;
                talkingTo.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => dialogue.SpeakingTo = talkingTo.SelectedItem!.ToString());
                PropertyInspector.Controls.Add(talkingTo);

                label = GetLabel("Importance:");
                PropertyInspector.Controls.Add(label);

                ComboBox importance = GetComboBox(node);
                importance.Items.AddRange(Enum.GetNames(typeof(StoryEnums.Importance)));
                importance.SelectedItem = dialogue.SpeakingTo;
                importance.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => dialogue.SpeakingTo = importance.SelectedItem!.ToString());
                PropertyInspector.Controls.Add(importance);

                CheckBox checkBox = GetCheckbox("Is conversation starter:", dialogue.IsConversationStarter);
                checkBox.CheckedChanged += (_, args) => dialogue.IsConversationStarter = checkBox.Checked;
                PropertyInspector.Controls.Add(checkBox);

                checkBox = GetCheckbox("Override combat in vicinity:", dialogue.OverrideCombatRestriction);
                checkBox.CheckedChanged += (_, args) => dialogue.OverrideCombatRestriction = checkBox.Checked;
                PropertyInspector.Controls.Add(checkBox);

                checkBox = GetCheckbox("Silence Voice Acting:", dialogue.PlaySilently);
                checkBox.CheckedChanged += (_, args) => dialogue.PlaySilently = checkBox.Checked;
                PropertyInspector.Controls.Add(checkBox);

                label = GetLabel("Paired Emote:");
                PropertyInspector.Controls.Add(label);

                ComboBox pairedEmote = GetComboBox(node);
                pairedEmote.Items.AddRange(Enum.GetNames(typeof(StoryEnums.BGCEmotes)));
                pairedEmote.SelectedItem = dialogue.PairedEmote;
                pairedEmote.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => dialogue.PairedEmote = pairedEmote.SelectedItem!.ToString());
                PropertyInspector.Controls.Add(pairedEmote);

                TextBox text = new()
                {
                    Text = dialogue.Text,
                    Multiline = true,
                    WordWrap = true,
                    ScrollBars = ScrollBars.Both,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.LightGray,
                    BackColor = Color.FromArgb(50, 50, 50),
                };
                text.TextChanged += (_, _) => dialogue.Text = text.Text;
                text.TextChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
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
                Label label = GetLabel(node.FileName + "'s Dialogue " + node.ID + "\n Talking to:");
                PropertyInspector.Controls.Add(label);
                if (node.Data<Dialogue>() is null)
                {
                    Label label2 = GetLabel("No data on this node", ContentAlignment.TopCenter);
                    PropertyInspector.Controls.Add(label2);
                    break;
                }
                PropertyInspector.RowCount = 2;
                Dialogue dialogue = node.Data<Dialogue>()!;

                ComboBox talkingTo = GetComboBox(node);
                talkingTo.Items.AddRange(Enum.GetNames(typeof(StoryEnums.Characters)));
                talkingTo.SelectedItem = dialogue.SpeakingToCharacterName;
                talkingTo.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => dialogue.SpeakingToCharacterName = talkingTo.SelectedItem!.ToString());
                PropertyInspector.Controls.Add(talkingTo);

                CheckBox checkBox = GetCheckbox("Doesn't count as met:", dialogue.DoesNotCountAsMet);
                checkBox.CheckedChanged += (_, args) => dialogue.DoesNotCountAsMet = checkBox.Checked;
                PropertyInspector.Controls.Add(checkBox);

                checkBox = GetCheckbox("Show global repsonses:", dialogue.ShowGlobalResponses);
                checkBox.CheckedChanged += (_, args) => dialogue.ShowGlobalResponses = checkBox.Checked;
                PropertyInspector.Controls.Add(checkBox);

                checkBox = GetCheckbox("Use goodbye responses:", dialogue.ShowGlobalGoodByeResponses);
                checkBox.CheckedChanged += (_, args) => dialogue.ShowGlobalGoodByeResponses = checkBox.Checked;
                PropertyInspector.Controls.Add(checkBox);

                checkBox = GetCheckbox("Auto Immersive:", dialogue.IsDynamic);
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
                    BackColor = Color.FromArgb(50, 50, 50),
                };
                text.TextChanged += (_, _) => dialogue.Text = text.Text;
                text.TextChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
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

                Label label = GetLabel("Alternate text for " + node.FileName + "'s Dialogue " + node.ID + "\n Sort order:");
                PropertyInspector.Controls.Add(label);
                AlternateText alternate = node.Data<AlternateText>()!;

                NumericUpDown sortOrder = GetNumericUpDown(alternate.Order);
                sortOrder.DecimalPlaces = 0;
                sortOrder.Minimum = 0;
                sortOrder.ValueChanged += (_, _) => alternate.Order = (int)sortOrder.Value;
                sortOrder.ValueChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
                PropertyInspector.Controls.Add(sortOrder);

                TextBox text = new()
                {
                    Text = alternate.Text,
                    Multiline = true,
                    WordWrap = true,
                    ScrollBars = ScrollBars.Both,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.LightGray,
                    BackColor = Color.FromArgb(50, 50, 50),
                };
                text.TextChanged += (_, _) => alternate.Text = text.Text;
                text.TextChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
                text.Select();

                PropertyInspector.RowStyles[0].SizeType = SizeType.Absolute;
                PropertyInspector.RowStyles[0].Height = 35;
                PropertyInspector.Controls.Add(text, 0, 1);
                PropertyInspector.SetColumnSpan(text, 3);
                break;
            }
            case NodeType.GameEvent:
            {
                if (node.Data<GameEvent>() is null)
                {
                    Label label2 = GetLabel("No data on this node", ContentAlignment.TopCenter);
                    PropertyInspector.Controls.Add(label2);
                    break;
                }

                PropertyInspector.RowCount = 2;
                PropertyInspector.ColumnCount = 5;

                GameEvent gevent = node.Data<GameEvent>()!;

                Label label = GetLabel("Order:");
                PropertyInspector.Controls.Add(label, 0, 0);

                NumericUpDown sortOrder = GetNumericUpDown(gevent.SortOrder);
                sortOrder.DecimalPlaces = 0;
                sortOrder.Minimum = 0;
                sortOrder.ValueChanged += (_, _) => gevent.SortOrder = (int)sortOrder.Value;
                sortOrder.ValueChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
                PropertyInspector.Controls.Add(sortOrder, 1, 0);

                ComboBox type = GetComboBox(node);
                type.Items.AddRange(Enum.GetNames(typeof(GameEvents)));
                type.SelectedItem = gevent.EventType.ToString();
                type.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.EventType = Enum.Parse<GameEvents>(type.SelectedItem.ToString()!));
                PropertyInspector.Controls.Add(type, 2, 0);

                Label label3 = GetLabel("Delay:");
                PropertyInspector.Controls.Add(label3, 3, 0);

                NumericUpDown delay = GetNumericUpDown((float)gevent.Delay);
                delay.ValueChanged += (_, _) => gevent.Delay = (double)delay.Value;
                delay.ValueChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
                PropertyInspector.Controls.Add(delay, 4, 0);

                switch (gevent.EventType)
                {
                    case GameEvents.AddForce:
                    {
                        ComboBox characters = GetComboBox(node);
                        characters.Items.AddRange(Enum.GetNames(typeof(Characters)));
                        characters.SelectedItem = gevent.Value!;
                        characters.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Value = characters.SelectedItem.ToString()!);
                        PropertyInspector.Controls.Add(characters);

                        //todo
                        break;
                    }
                    case GameEvents.AllowPlayerSave:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.ChangeBodyScale:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.CharacterFromCharacterGroup:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.CharacterFunction:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.Clothing:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.Combat:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.CombineValue:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.CutScene:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.Dialogue:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.DisableNPC:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.DisplayGameMessage:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.Door:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.Emote:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.EnableNPC:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.EventTriggers:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.FadeIn:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.FadeOut:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.IKReach:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.Intimacy:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.Item:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.ItemFromItemGroup:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.LookAt:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.Personality:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.Property:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.MatchValue:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.ModifyValue:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.Player:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.PlaySoundboardClip:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.Pose:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.Quest:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.RandomizeIntValue:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.ResetReactionCooldown:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.Roaming:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.SendEvent:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.SetPlayerPref:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.Social:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.State:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.TriggerBGC:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.Turn:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.TurnInstantly:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.UnlockAchievement:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.WalkTo:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.WarpOverTime:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.WarpTo:
                    {
                        //todo
                        break;
                    }
                    case GameEvents.None:
                    {
                        //todo
                        break;
                    }
                    default:
                    {
                        //todo
                        break;
                    }
                }

                break;
            }
            case NodeType.EventTrigger:
            {
                if (node.Data<EventTrigger>() is null)
                {
                    Label label2 = GetLabel("No data on this node", ContentAlignment.TopCenter);
                    PropertyInspector.Controls.Add(label2);
                    break;
                }
                EventTrigger eventTrigger = node.Data<EventTrigger>()!;

                PropertyInspector.RowCount = 2;

                Label label = GetLabel("Name:");
                PropertyInspector.Controls.Add(label, 0, 0);

                TextBox customName = new()
                {
                    Text = eventTrigger.Name,
                    Multiline = true,
                    WordWrap = true,
                    ScrollBars = ScrollBars.Both,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.LightGray,
                    BackColor = Color.FromArgb(50, 50, 50),
                };
                customName.TextChanged += (_, _) => eventTrigger.Name = customName.Text;
                customName.TextChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
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
                        PutCharacter(node, eventTrigger);
                        PutStartCondition(node, eventTrigger);
                        break;
                    }
                    case EventTypes.EntersZone:
                    {
                        PutCharacter(node, eventTrigger);
                        PutStartCondition(node, eventTrigger);

                        ComboBox zone = GetComboBox(node);
                        zone.Items.AddRange(Enum.GetNames(typeof(ZoneEnums)));
                        zone.SelectedItem = eventTrigger.Value;
                        zone.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.Value = (string)zone.SelectedItem!);
                        PropertyInspector.Controls.Add(zone);

                        break;
                    }
                    case EventTypes.ReachesTarget:
                    {
                        PutCharacter(node, eventTrigger);
                        PutStartCondition(node, eventTrigger);

                        ComboBox targetType = GetComboBox(node);
                        targetType.Items.AddRange(Enum.GetNames(typeof(LocationTargetOption)));
                        targetType.SelectedItem = eventTrigger.LocationTargetOption.ToString();
                        targetType.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.LocationTargetOption = Enum.Parse<LocationTargetOption>(targetType.SelectedItem!.ToString()!));
                        targetType.SelectedIndexChanged += (_, _) => ShowProperties(node);
                        PropertyInspector.Controls.Add(targetType);

                        switch (eventTrigger.LocationTargetOption)
                        {
                            case LocationTargetOption.MoveTarget:
                            {
                                ComboBox target = GetComboBox(node);
                                target.Items.AddRange(Enum.GetNames(typeof(MoveTargets)));
                                target.SelectedItem = eventTrigger.Value!.Replace(" ", "");
                                target.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.Value = target.SelectedItem!.ToString()!);
                                PropertyInspector.Controls.Add(target);
                                break;
                            }
                            case LocationTargetOption.Character:
                            {
                                PutCharacterValue(node, eventTrigger);
                                break;
                            }
                            case LocationTargetOption.Item:
                            {
                                PutItemValue(node, eventTrigger);
                                break;
                            }
                            default:
                                break;
                        }

                        break;
                    }
                    case EventTypes.IsBlockedByLockedDoor:
                    {
                        PutCharacter(node, eventTrigger);
                        PutStartCondition(node, eventTrigger);

                        TextBox door = new()
                        {
                            Text = eventTrigger.Value,
                            Multiline = true,
                            WordWrap = true,
                            ScrollBars = ScrollBars.Both,
                            Dock = DockStyle.Fill,
                            ForeColor = Color.LightGray,
                            BackColor = Color.FromArgb(50, 50, 50),
                        };
                        door.TextChanged += (_, _) => eventTrigger.Value = door.Text;
                        door.TextChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
                        PropertyInspector.Controls.Add(door);

                        break;
                    }
                    case EventTypes.IsAttacked:
                    {
                        PutCharacter(node, eventTrigger);
                        PutStartCondition(node, eventTrigger);
                        PutCharacterValue(node, eventTrigger);
                        break;
                    }
                    case EventTypes.GetsHitWithProjectile:
                    {
                        PutCharacter(node, eventTrigger);
                        PutStartCondition(node, eventTrigger);
                        PutItemkey(node, eventTrigger);

                        Label inlabel = GetLabel("in the:");
                        PropertyInspector.Controls.Add(inlabel);

                        ComboBox zone = GetComboBox(node);
                        zone.Items.AddRange(Enum.GetNames(typeof(BodyRegion)));
                        zone.SelectedItem = ((BodyRegion)int.Parse(eventTrigger.Value!)).ToString();
                        zone.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.Value = ((int)Enum.Parse<BodyRegion>(zone.SelectedItem!.ToString()!)).ToString());
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

                        Label inlabel = GetLabel("every:");
                        PropertyInspector.Controls.Add(inlabel);

                        NumericUpDown option = GetNumericUpDown((float)eventTrigger.UpdateIteration);
                        option.ValueChanged += (_, _) => eventTrigger.UpdateIteration = (double)option.Value;
                        option.ValueChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
                        PropertyInspector.Controls.Add(option);

                        Label seconds = GetLabel("seconds");
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
                        PutCharacter(node, eventTrigger);
                        PutStartCondition(node, eventTrigger);
                        PutItemValue(node, eventTrigger);
                        break;
                    }
                    case EventTypes.PlayerThrowsItem:
                    {
                        PutCharacter(node, eventTrigger);
                        PutStartCondition(node, eventTrigger);
                        PutItemValue(node, eventTrigger);
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
                        PutCharacter(node, eventTrigger);
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

                        ComboBox cutscene = GetComboBox(node);
                        cutscene.Items.AddRange(Enum.GetNames(typeof(Cutscenes)));
                        cutscene.SelectedItem = eventTrigger.Value;
                        cutscene.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.Value = cutscene.SelectedItem!.ToString());
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
                if (node.Data<Response>() is null)
                {
                    Label label2 = GetLabel("No data on this node", ContentAlignment.TopCenter);
                    PropertyInspector.Controls.Add(label2);
                    break;
                }
                Response response = node.Data<Response>()!;

                PropertyInspector.ColumnCount = 6;

                Label label = GetLabel("Trigger New Dialogue:\n\nResponse Text:");
                PropertyInspector.Controls.Add(label);

                ComboBox dialogue = GetComboBox(node);
                dialogue.Items.Add("Do not trigger new dialogue");
                for (int i = 0; i < Stories[node.FileName].Dialogues!.Count; i++)
                {
                    dialogue.Items.Add(Stories[node.FileName].Dialogues![i].ID.ToString());
                }
                dialogue.SelectedItem = response.Next.ToString()!;
                dialogue.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) =>
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
                });
                PropertyInspector.Controls.Add(dialogue);

                Label label3 = GetLabel(Stories[node.FileName].Dialogues!.Find((dialog) => dialog.ID.ToString() == dialogue.SelectedItem?.ToString())?.Text ?? "No text on dialogue");
                PropertyInspector.Controls.Add(label3);

                Label label4 = GetLabel("Display Order");
                PropertyInspector.Controls.Add(label4);

                NumericUpDown option = GetNumericUpDown(response.Order);
                option.DecimalPlaces = 0;
                option.Minimum = 0;
                option.ValueChanged += (_, _) => response.Order = (int)option.Value;
                option.ValueChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
                PropertyInspector.Controls.Add(option);

                CheckBox checkBox = GetCheckbox("Always Available:", response.AlwaysDisplay);
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
                    BackColor = Color.FromArgb(50, 50, 50),
                };
                text.TextChanged += (_, _) => response.Text = text.Text;
                text.TextChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
                text.Select();
                PropertyInspector.RowStyles[0].SizeType = SizeType.Absolute;
                PropertyInspector.RowStyles[0].Height = 50;
                PropertyInspector.Controls.Add(text, 0, 1);
                PropertyInspector.SetColumnSpan(text, 6);
                break;
            }
            case NodeType.Value:
            {
                Label label = GetLabel(node.FileName + "'s value:");
                PropertyInspector.Controls.Add(label);

                if (node.Data<Value>() is not null)
                {
                    TextBox obj2 = new()
                    {
                        Dock = DockStyle.Fill,
                        TextAlign = HorizontalAlignment.Center,
                        ForeColor = Color.LightGray,
                        Text = node.Data<Value>()!.value,
                        AutoSize = true,
                    };
                    obj2.TextChanged += (_, args) => node.Data<Value>()!.value = obj2.Text;
                    obj2.TextChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
                    PropertyInspector.Controls.Add(obj2);
                }
                else
                {
                    Label label2 = GetLabel("No data on this node!", ContentAlignment.TopCenter);
                    PropertyInspector.Controls.Add(label2);
                }
                break;
            }
            case NodeType.Personality:
            {
                Label label = GetLabel(node.FileName + "'s " + node.ID);
                PropertyInspector.Controls.Add(label);

                if (node.Data<Trait>() is not null)
                {
                    NumericUpDown option = GetNumericUpDown(node.Data<Trait>()!.Value);
                    option.Maximum = 100;
                    option.Minimum = -100;
                    option.DecimalPlaces = 0;
                    option.ValueChanged += (_, _) => node.Data<Trait>()!.Value = (int)option.Value;
                    option.ValueChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
                    PropertyInspector.Controls.Add(option);
                }
                else
                {
                    Label label2 = GetLabel("No data on this node!", ContentAlignment.TopCenter);
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
            case NodeType.StoryItem:
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
                Label label = GetLabel(node.Type.ToString(), ContentAlignment.TopCenter);
                PropertyInspector.Controls.Add(label);
                label = GetLabel(node.ID, ContentAlignment.TopCenter);
                PropertyInspector.Controls.Add(label);
                label = GetLabel(node.Text, ContentAlignment.TopCenter);
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
            PropertyInspector.Controls[i].BackColor = Color.FromArgb(50, 50, 50);

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

    private static NumericUpDown GetNumericUpDown(float start)
    {
        NumericUpDown sortOrder = new()
        {
            //Dock = DockStyle.Fill,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
            Value = (decimal)start,
            Dock = DockStyle.Left,
            Width = 50
        };
        return sortOrder;
    }

    private static Label GetLabel(string text, ContentAlignment alignment = ContentAlignment.TopRight)
    {
        return new()
        {
            Text = text,
            TextAlign = alignment,
            Dock = DockStyle.Top,
            ForeColor = Color.LightGray,
            AutoSize = true,
        };
    }

    private static CheckBox GetCheckbox(string text, bool ischecked)
    {
        return new()
        {
            Checked = ischecked,
            Dock = DockStyle.Fill,
            Text = text,
            CheckAlign = ContentAlignment.MiddleRight,
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = Color.LightGray,
            AutoSize = true,
        };
    }

    private void PutItemValue(Node node, EventTrigger eventTrigger)
    {
        ComboBox item = GetComboBox(node);
        item.Items.AddRange(Enum.GetNames(typeof(Items)));
        item.SelectedItem = eventTrigger.Value;
        item.PerformLayout();
        item.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.Value = item.SelectedItem!.ToString()!);
        PropertyInspector.Controls.Add(item);
    }

    private void PutItemkey(Node node, EventTrigger eventTrigger)
    {
        ComboBox item = GetComboBox(node);
        item.Items.AddRange(Enum.GetNames(typeof(Items)));
        item.SelectedItem = eventTrigger.Key;
        item.PerformLayout();
        item.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.Key = item.SelectedItem!.ToString()!);
        PropertyInspector.Controls.Add(item);
    }

    private void PutCharacterValue(Node node, EventTrigger eventTrigger)
    {
        ComboBox character = GetComboBox(node);
        character.Items.AddRange(Enum.GetNames(typeof(AnybodyCharacters)));
        character.SelectedItem = eventTrigger.Value;
        character.PerformLayout();
        character.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.Value = character.SelectedItem!.ToString());
        PropertyInspector.Controls.Add(character);
    }

    private ComboBox PutCharacter(Node node, EventTrigger eventTrigger)
    {
        ComboBox character = GetComboBox(node);
        character.Items.AddRange(Enum.GetNames(typeof(AnybodyCharacters)));
        character.SelectedItem = eventTrigger.CharacterToReactTo;
        character.PerformLayout();
        character.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.CharacterToReactTo = character.SelectedItem!.ToString());
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

        ComboBox startCondition = GetComboBox(node);
        startCondition.Items.AddRange(Enum.GetNames<EventTypes>());
        startCondition.SelectedItem = eventTrigger.Type.ToString()!;
        startCondition.PerformLayout();
        startCondition.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) =>
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
        });
        PropertyInspector.Controls.Add(startCondition);
    }

    private void PutTextValue(Node node, Criterion criterion)
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

    private void PutNumericOption(Node node, Criterion criterion)
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
        option.ValueChanged += (_, _) => { NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]); Graph.Invalidate(); };
        PropertyInspector.Controls.Add(option);
    }

    private void PutNumericValue(Node node, Criterion criterion)
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
            criterion.Value = 0.ToString();
        }
        option.PerformLayout();
        option.ValueChanged += (_, _) => criterion.Value = option.Value.ToString();
        PropertyInspector.Controls.Add(option);
    }

    private void PutComparison(Node node, Criterion criterion)
    {
        ComboBox equ = GetComboBox(node);
        equ.Items.AddRange(Enum.GetNames(typeof(ComparisonEquations)));
        if (int.TryParse(criterion.Value!, out int res))
        {
            if (res < equ.Items.Count && res >= 1)
            {
                equ.SelectedIndex = res - 1;
            }
            else
            {
                equ.SelectedIndex = 0;
                criterion.Value = 0.ToString();
            }
        }
        else
        {
            equ.SelectedIndex = 0;
            criterion.Value = 0.ToString();
        }
        equ.Select(equ.SelectedItem?.ToString()?.Length ?? 0, 0);
        equ.PerformLayout();
        equ.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Value = (equ.SelectedIndex).ToString());
        PropertyInspector.Controls.Add(equ);
    }

    private void PutZone(Node node, Criterion criterion)
    {
        ComboBox zone = GetComboBox(node);
        zone.Items.AddRange(Enum.GetNames(typeof(ZoneEnums)));
        zone.SelectedItem = criterion.Key;
        zone.SelectionLength = 0;
        zone.SelectionStart = 0;
        zone.PerformLayout();
        zone.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Key = (string)zone.SelectedItem!);
        PropertyInspector.Controls.Add(zone);
    }

    private void PutCompareType(Node node, Criterion criterion)
    {
        ComboBox compareType = GetComboBox(node);
        compareType.Items.AddRange(Enum.GetNames(typeof(CompareTypes)));
        compareType.SelectedItem = criterion.CompareType.ToString();
        compareType.SelectedText = string.Empty;
        compareType.SelectionLength = 0;
        compareType.SelectionStart = 0;
        compareType.PerformLayout();
        compareType.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.CompareType = Enum.Parse<CompareTypes>((string)compareType.SelectedItem!));
        compareType.SelectedIndexChanged += (_, _) => ShowProperties(node);
        PropertyInspector.Controls.Add(compareType);
    }

    private ComboBox PutCharacter1(Node node, Criterion criterion)
    {
        ComboBox compareType = GetComboBox(node);
        compareType.Items.AddRange(Enum.GetNames(typeof(Characters)));
        compareType.SelectedItem = criterion.Character;
        compareType.SelectionLength = 0;
        compareType.SelectionStart = 0;
        compareType.PerformLayout();
        compareType.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Character = (string)compareType.SelectedItem!);
        PropertyInspector.Controls.Add(compareType);
        return compareType;
    }

    private void PutCharacter2(Node node, Criterion criterion)
    {
        ComboBox compareType = GetComboBox(node);
        compareType.Items.AddRange(Enum.GetNames(typeof(Characters)));
        compareType.SelectedItem = criterion.Character2;
        compareType.SelectedText = string.Empty;
        compareType.SelectionLength = 0;
        compareType.SelectionStart = 0;
        compareType.PerformLayout();
        compareType.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Character2 = (string)compareType.SelectedItem!);
        PropertyInspector.Controls.Add(compareType);
    }

    private void PutBoolValue(Node node, Criterion criterion)
    {
        ComboBox boolValue = GetComboBox(node);
        boolValue.Items.AddRange(Enum.GetNames(typeof(BoolCritera)));
        boolValue.SelectedItem = criterion.BoolValue!.ToString();
        boolValue.SelectionLength = 0;
        boolValue.SelectionStart = 0;
        boolValue.PerformLayout();
        boolValue.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.BoolValue = Enum.Parse<BoolCritera>(boolValue.SelectedItem!.ToString()!));
        PropertyInspector.Controls.Add(boolValue);
    }

    private void PutItem(Node node, Criterion criterion)
    {
        ComboBox item = GetComboBox(node);
        item.Items.AddRange(Enum.GetNames<Items>());
        item.SelectedItem = criterion.Key!.Replace(" ", "");
        item.PerformLayout();
        item.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Key = item.SelectedItem!.ToString());
        PropertyInspector.Controls.Add(item);
    }

    private void PutEquals(Node node, Criterion criterion)
    {
        ComboBox equals = GetComboBox(node);
        equals.Items.AddRange(Enum.GetNames(typeof(EqualsValues)));
        equals.SelectedIndex = (int)criterion.EqualsValue!;
        equals.PerformLayout();
        equals.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.EqualsValue = (EqualsValues?)equals.SelectedIndex);
        PropertyInspector.Controls.Add(equals);
    }

    private ComboBox GetComboBox(Node node)
    {
        var box = new ComboBox()
        {
            Dock = DockStyle.Fill,
            AutoCompleteMode = AutoCompleteMode.SuggestAppend,
            AutoCompleteSource = AutoCompleteSource.ListItems,
            DropDownStyle = ComboBoxStyle.DropDownList,
        };

        return box;
    }

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
            ClearOverlayBitmap();
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

    //todo probably needs some more work for some events and criteria to populate defaults
    private void SpawnNodeFromSpaceSpawner(object sender, EventArgs e)
    {
        SpawnableNodeType selectedType = Enum.Parse<SpawnableNodeType>(NodeSpawnBox.SelectedItem?.ToString()!);

        if (!NodeSpawnBox.Enabled)
        {
            nodeToLinkFrom = Node.NullNode;
            oldMousePosBeforeSpawnWindow = Point.Empty;
            return;
        }
        NodeSpawnBox.Enabled = false;
        NodeSpawnBox.Visible = false;

        string character = GetProbableCharacter();
        var newNode = GetNodeFromSpawnableType(selectedType, character);
        ScreenToGraph(oldMousePosBeforeSpawnWindow.X, oldMousePosBeforeSpawnWindow.Y, out float nodeX, out float nodeY);

        SetUpNewlySpawnedNode(newNode);

        newNode.Position = new(nodeX, nodeY);

        if (nodeToLinkFrom != Node.NullNode)
        {
            Link(nodeToLinkFrom, newNode);
            ShowProperties(newNode);
            nodeToLinkFrom = Node.NullNode;
            oldMousePosBeforeSpawnWindow = Point.Empty;
        }
    }

    private string GetProbableCharacter()
    {
        string character = SelectedCharacter;

        if (clickedNode != Node.NullNode)
        {
            character = clickedNode.FileName;
        }

        return character;
    }

    private void SetUpNewlySpawnedNode(Node newNode, PointF? oldPoint = null)
    {
        if (ActiveForm is null)
        {
            Debugger.Break();
            return;
        }

        if (clickedNode == Node.NullNode)
        {
            PointF ScreenPos;
            if (oldPoint is null)
            {
                var pos = Graph.PointToClient(Cursor.Position);
                ScreenToGraph(pos.X, pos.Y, out float ScreenPosX, out float ScreenPosY);
                ScreenPosX -= NodeSizeX / 2;
                ScreenPosY -= NodeSizeY / 2;
                ScreenPos = new PointF(ScreenPosX, ScreenPosY);
            }
            else
            {
                ScreenPos = oldPoint.Value;
                ScreenPos -= new SizeF(NodeSizeX / 2, NodeSizeY / 2);
            }

            newNode.Position = ScreenPos;
        }
        else
        {
            newNode.Position = clickedNode.Position + new Size(scaleX, 0);
        }

        clickedNode = newNode;
        ShowProperties(newNode);

        Graph.Invalidate();
        Graph.Focus();
    }

    private Node GetNodeFromSpawnableType(SpawnableNodeType selectedType, string character)
    {
        Node newNode = Node.NullNode;
        switch (selectedType)
        {
            case SpawnableNodeType.Criterion:
            {
                string id = Guid.NewGuid().ToString();
                newNode = new Node(id, NodeType.Criterion, string.Empty, nodes[character].Positions)
                {
                    RawData = new Criterion() { Character = character },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    Link(clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.ItemAction:
            {
                string id = "action";
                newNode = new Node(id, NodeType.ItemAction, string.Empty, nodes[character].Positions)
                {
                    RawData = new ItemAction() { ActionName = id },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    Link(clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.Achievement:
            {
                var guid = Guid.NewGuid().ToString();
                newNode = new Node(guid, NodeType.Achievement, string.Empty, nodes[character].Positions)
                {
                    RawData = new Achievement() { Id = guid },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    Link(clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.BGC:
            {
                newNode = new Node("BGC" + (Stories[character].BackgroundChatter!.Count + 1).ToString(), NodeType.BGC, string.Empty, nodes[character].Positions)
                {
                    RawData = new BackgroundChatter() { Id = Stories[character].BackgroundChatter!.Count + 1 },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    Link(clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.BGCResponse:
            {
                newNode = new Node(Guid.NewGuid().ToString(), NodeType.BGCResponse, string.Empty, nodes[character].Positions)
                {
                    RawData = new BackgroundChatterResponse() { Label = "response:" },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    Link(clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.CriteriaGroup:
            {
                string id = Guid.NewGuid().ToString();
                newNode = new Node(id, NodeType.CriteriaGroup, string.Empty, nodes[character].Positions)
                {
                    RawData = new CriteriaGroup() { Name = id },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    Link(clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.Dialogue:
            {
                int id = (Stories[character].Dialogues!.Count + 1);
                newNode = new Node(id.ToString(), NodeType.Dialogue, string.Empty, nodes[character].Positions)
                {
                    RawData = new Dialogue() { ID = id },
                    FileName = character,
                };
                nodes[character].Add(newNode);
                Stories[character].Dialogues!.Add(newNode.Data<Dialogue>()!);

                if (clickedNode != Node.NullNode)

                    if (clickedNode != Node.NullNode)
                    {
                        Link(clickedNode, newNode);
                    }
                break;
            }
            case SpawnableNodeType.AlternateText:
            {
                string id = Guid.NewGuid().ToString();
                newNode = new Node(id, NodeType.AlternateText, string.Empty, nodes[character].Positions)
                {
                    RawData = new AlternateText() { },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    Link(clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.GameEvent:
            {
                string id = Guid.NewGuid().ToString();
                newNode = new Node(id, NodeType.GameEvent, string.Empty, nodes[character].Positions)
                {
                    RawData = new GameEvent() { Character = character },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    Link(clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.EventTrigger:
            {
                string id = Guid.NewGuid().ToString();
                newNode = new Node(id, NodeType.EventTrigger, string.Empty, nodes[character].Positions)
                {
                    RawData = new EventTrigger() { Id = id },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    Link(clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.StoryItem:
            {
                string id = "item name";
                newNode = new Node(id, NodeType.StoryItem, string.Empty, nodes[character].Positions)
                {
                    RawData = new ItemOverride() { ItemName = id },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    Link(clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.ItemInteraction:
            {
                string id = "interaction name";
                newNode = new Node(id, NodeType.ItemInteraction, string.Empty, nodes[character].Positions)
                {
                    RawData = new ItemInteraction() { ItemName = id },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    Link(clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.ItemGroupInteraction:
            {
                string id = "interaction name";
                newNode = new Node(id, NodeType.ItemInteraction, string.Empty, nodes[character].Positions)
                {
                    RawData = new ItemGroupInteraction() { Name = id },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    Link(clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.ItemGroup:
            {
                string id = Guid.NewGuid().ToString();
                newNode = new Node(id, NodeType.ItemGroup, string.Empty, nodes[character].Positions)
                {
                    RawData = new ItemGroup() { Id = id },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    Link(clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.Quest:
            {
                string id = Guid.NewGuid().ToString();
                newNode = new Node(id, NodeType.Quest, string.Empty, nodes[character].Positions)
                {
                    RawData = new Quest() { CharacterName = character, ID = id },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    Link(clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.Response:
            {
                string id = Guid.NewGuid().ToString();
                newNode = new Node(id, NodeType.Response, string.Empty, nodes[character].Positions)
                {
                    RawData = new Response() { Id = id },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    Link(clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.Value:
            {
                string id = "ValueName";
                newNode = new Node(id, NodeType.Value, string.Empty, nodes[character].Positions)
                {
                    RawData = new Value() { value = id },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    Link(clickedNode, newNode);
                }
                if (character == Player)
                {
                    Story.PlayerValues.Add(newNode.Data<Value>()!.value!);
                }
                else
                {
                    Stories[character].StoryValues.Add(newNode.Data<Value>()!.value!);
                }
                break;
            }
            case SpawnableNodeType.UseWith:
            {
                string id = "use on item:";
                newNode = new Node(id, NodeType.UseWith, string.Empty, nodes[character].Positions)
                {
                    RawData = new UseWith() { ItemName = id },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    Link(clickedNode, newNode);
                }
                break;
            }
            default:
            {
                break;
            }
        }

        return newNode;
    }
}
