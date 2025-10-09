using CSC.Components;
using CSC.Nodestuff;
using CSC.StoryItems;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.Json;
using static CSC.StoryItems.StoryEnums;

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
    private readonly SizeF NodeCenter = new(NodeSizeX / 2, NodeSizeY / 2);
    private readonly List<int> maxYperX = [];
    private readonly List<Node> visited = [];
    private readonly List<Node> selected = [];
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
    private readonly SolidBrush SelectionFill;
    private readonly Pen SelectionEdge;
    private RectangleF adjustedMouseClipBounds;
    private SizeF OffsetFromDragClick = SizeF.Empty;
    private readonly List<SizeF> SelectedNodeOffsets = [];
    private SizeF CircleSize = new(15, 15);
    private CachedBitmap? oldGraph;
    private static MainStory Story = null!;
    public static readonly Dictionary<string, CharacterStory> Stories = [];
    private static readonly Dictionary<string, NodeStore> nodes = [];
    private static string _selectedCharacter = NoCharacter;
    public bool MovingChild = false;
    private int GeventPropertyCounter = 0;
    public const int NodeSizeX = 200;
    public const int NodeSizeY = 50;
    public const string NoCharacter = "None";
    public const string Player = "Player";
    private const string Anybody = "Anybody";
    private RectangleF adjustedVisibleClipBounds = new();
    private Point oldMousePosBeforeSpawnWindow = Point.Empty;
    private bool selecting;
    private bool subtracting;
    private bool adding;
    private Point startSelectingMousePos = Point.Empty;
    private string StoryFolder = string.Empty;
    public const string HousePartyVersion = "1.4.2";

    public static string StoryName { get; private set; } = NoCharacter;

    public static string SelectedCharacter
    {
        get
        {
            if (_selectedCharacter == StoryName && StoryName != NoCharacter)
            {
                return Player;
            }
            else
            {
                return _selectedCharacter;
            }
        }

        set
        {

            if (value == string.Empty || value is null)
            {
                _selectedCharacter = Player;
            }
            else
            {
                _selectedCharacter = value;
            }
        }
    }

    public int RightClickFrameCounter { get; private set; } = 0;

    public int LeftClickFrameCounter { get; private set; } = 0;

    //todo when adding an event to another somethign and the event is already part of another node we need to clone the event, sadly we cannot use the exact same event object in multiple places :(
    //todo add option to view all referenced values in other files, like reverse strike brush

    //todo add info when trying to link incompatible notes
    //todo add search
    //todo unify all node creation so its always the same
    //todo add grouping

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
        SelectionEdge = new Pen(Color.LightGray, 1f);
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
        clothingNodeBrush = new SolidBrush(Color.FromArgb(95, 235, 60));
        criteriaGroupNodeBrush = new SolidBrush(Color.FromArgb(150, 50, 50));
        criterionNodeBrush = new SolidBrush(Color.FromArgb(180, 20, 40));
        cutsceneNodeBrush = new SolidBrush(Color.FromArgb(235, 30, 160));
        dialogueNodeBrush = new SolidBrush(Color.FromArgb(45, 60, 185));
        doorNodeBrush = new SolidBrush(Color.FromArgb(200, 225, 65));
        eventNodeBrush = new SolidBrush(Color.FromArgb(50, 150, 50));
        eventTriggerNodeBrush = new SolidBrush(Color.FromArgb(60, 100, 70));
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
        SelectionFill = new SolidBrush(Color.FromArgb(80, Color.Gray));

        NodeSpawnBox.Items.AddRange(Enum.GetNames<SpawnableNodeType>());

        PropertyInspector.SizeChanged += (_, _) => UpdatePropertyColoumnWidths();

        nodes.Add(NoCharacter, new());
        Scaling.Add(NoCharacter, 0.3f);
        OffsetX.Add(NoCharacter, 0);
        OffsetY.Add(NoCharacter, 0);

        nodes.Add(Player, new());
        Scaling.Add(Player, 0.3f);
        OffsetX.Add(Player, 0);
        OffsetY.Add(Player, 0);

        clickedNode = Node.NullNode;
        highlightNode = Node.NullNode;
        movedNode = Node.NullNode;
        nodeToLinkFrom = Node.NullNode;

        Graph.Invalidate();
    }

    public void HandleKeyBoard(object? sender, KeyEventArgs e)
    {
        //get the shift key state so we can determine later if we want to redraw the tree on node selection or not
        IsShiftPressed = e.KeyData == (Keys.ShiftKey | Keys.Shift);
        IsCtrlPressed = e.KeyData == (Keys.Control | Keys.ControlKey);

        if (selected.Count > 0)
        {
            subtracting = IsCtrlPressed;
        }
        else
        {
            subtracting = false;
        }

        if (selected.Count > 0)
        {
            adding = IsShiftPressed;
        }
        else
        {
            adding = false;
        }

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

            if (clickedNode == Node.NullNode && PropertyInspector.Controls.Count > 1)
            {
                PropertyInspector.Controls.Clear();
            }
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
            removedNode = highlightNode;
        }
        else if (clickedNode != Node.NullNode)
        {
            removedNode = clickedNode;
        }

        if (removedNode == Node.NullNode)
        {
            return;
        }

        if (removedNode.Type is NodeType.Social or NodeType.Personality or NodeType.Clothing or NodeType.Cutscene or NodeType.Door or NodeType.Inventory or NodeType.Pose or NodeType.State)
        {
            //cant delete these types of nodes reasonably
            MessageBox.Show("Can't delete Node of type " + removedNode.Type.ToString(), "Can't Delete!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        //SystemSounds.Question.Play();
        if (MessageBox.Show("Delete \"" + removedNode.Text[..Math.Min(removedNode.Text.Length, 20)] + "[...]\"?", "Delete for real?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
        {
            if (nodeToLinkFrom == removedNode)
            {
                nodeToLinkFrom = Node.NullNode;
            }
            if (clickedNode == removedNode)
            {
                clickedNode = Node.NullNode;
            }
            if (highlightNode == removedNode)
            {
                highlightNode = Node.NullNode;
            }
            if (movedNode == removedNode)
            {
                movedNode = Node.NullNode;
            }

            var family = nodes[SelectedCharacter][removedNode];
            List<Node> childs = [.. family.Childs];
            List<Node> parents = [.. family.Parents];

            foreach (var child in childs)
            {
                NodeLinker.Unlink(nodes[SelectedCharacter], removedNode, child);
            }
            foreach (var parent in parents)
            {
                NodeLinker.Unlink(nodes[SelectedCharacter], parent, removedNode);
            }
            nodes[SelectedCharacter].Remove(removedNode);
            removedNode.RemoveFromSorting(SelectedCharacter);

            foreach (var child in childs)
            {
                NodeLinker.UpdateLinks(child, SelectedCharacter, nodes[SelectedCharacter]);
            }
            foreach (var parent in parents)
            {
                NodeLinker.UpdateLinks(parent, SelectedCharacter, nodes[SelectedCharacter]);
            }
            ShowProperties(Node.NullNode);
            Graph.Invalidate();
        }
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
        if (SelectedCharacter == Player)
        {
            if (clickedNode != Node.NullNode)
            {
                switch (clickedNode.Type)
                {
                    //itemaction uswith criterialist event eventtrigger alternatetext
                    //response dialogue bgc item itemgroup
                    case NodeType.Criterion:
                    {
                        return [NodeType.ItemAction, NodeType.UseWith, NodeType.CriteriaGroup, NodeType.GameEvent, NodeType.EventTrigger, NodeType.StoryItem, NodeType.ItemGroup, NodeType.Value];
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
                    case NodeType.Value:
                    {
                        return [NodeType.GameEvent, NodeType.Criterion, NodeType.Value];
                    }
                    //event criterion
                    case NodeType.CharacterGroup:
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
                        return [NodeType.Criterion, NodeType.ItemAction, NodeType.Achievement, NodeType.CriteriaGroup, NodeType.GameEvent, NodeType.EventTrigger, NodeType.StoryItem, NodeType.ItemGroup, NodeType.Quest, NodeType.UseWith, NodeType.Value];
                    }
                }
            }
            else
            {
                return [NodeType.Criterion, NodeType.ItemAction, NodeType.Achievement, NodeType.CriteriaGroup, NodeType.GameEvent, NodeType.EventTrigger, NodeType.StoryItem, NodeType.ItemGroup, NodeType.Quest, NodeType.UseWith, NodeType.Value];
            }
        }
        else
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

        if (adjustedMouseClipBounds.Contains(graphPos))
        {
            cursorPos.Text = $"Cursor position: ({graphPos.X:0000.00}|{graphPos.Y:0000.00})";
            cursorPos.Invalidate();
        }

        if (MovingChild)
        {
            if (selected.Count > 0 && !selecting)
            {
                for (int i = 0; i < selected.Count; i++)
                {
                    selected[i].Position = graphPos + SelectedNodeOffsets[i];
                }
                Cursor = Cursors.SizeAll;
                Graph.Invalidate();
            }
            else if (movedNode != Node.NullNode)
            {
                movedNode.Position = graphPos + OffsetFromDragClick;
                Cursor = Cursors.SizeAll;
                Graph.Invalidate();
            }
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
        else if (e.Button == MouseButtons.None && MouseButtons == MouseButtons.Left)
        {
            UpdateLeftClick(pos);
            Graph.Invalidate();
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
                if (e.Clicks == 2)
                {
                    //double click
                    UpdateDoubleClickTransition(graphPos);
                    Graph.Focus();
                }
                else
                {
                    UpdateLeftClick(pos);
                }
                break;
            }
            case MouseButtons.None:
            {
                OnNone(graphPos);
            }
            break;
            case MouseButtons.Right:
            {
                Graph.Focus();
                LeftClickFrameCounter = 0;
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
                OnNone(graphPos);
            }
            break;
        }
    }

    private void OnNone(PointF graphPos)
    {
        UpdateLeftRightClickStates(graphPos);
        EndPan();
        UpdateHighlightNode(graphPos);
        TryCreateOverlayBitmap();
    }

    private void UpdateLeftRightClickStates(PointF graphPos)
    {
        if (!MovingChild && RightClickFrameCounter > 0)
        {
            _ = UpdateClickedNode(graphPos);
            //right click only no move, spawn context
            SpawnContextMenu(graphPos);
        }
        else if (!selecting && LeftClickFrameCounter > 0)
        {
            if (LeftClickFrameCounter == 1)
            {
                _ = UpdateClickedNode(graphPos);
                selected.Clear();
                SelectedNodeOffsets.Clear();
                LeftClickFrameCounter = 0;
                Graph.Invalidate();
            }
        }
        if (MouseButtons != MouseButtons.Right)
        {
            RightClickFrameCounter = 0;
            MovingChild = false;
        }
        if (MouseButtons != MouseButtons.Left && LeftClickFrameCounter >= 2)
        {
            LeftClickFrameCounter = 0;
            UpdateLeftClickSelection(graphPos);
            selecting = false;
        }
    }

    private void UpdateLeftClickSelection(PointF graphPos)
    {
        if (!selecting)
        {
            return;
        }

        if (!subtracting && !adding)
        {
            selected.Clear();
        }

        var Pos1 = graphPos;
        ScreenToGraph(startSelectingMousePos.X, startSelectingMousePos.Y, out float GraphPosX, out float GraphPosY);
        var Pos2 = new PointF(GraphPosX, GraphPosY);
        int MinX = (int)MathF.Min(Pos1.X, Pos2.X);
        int MaxX = (int)MathF.Max(Pos1.X, Pos2.X);
        int MinY = (int)MathF.Min(Pos1.Y, Pos2.Y);
        int MaxY = (int)MathF.Max(Pos1.Y, Pos2.Y);

        for (int x = MinX; x < MaxX; x += (NodeSizeX / 2))
        {
            for (int y = MinY; y < MaxY; y += (NodeSizeY / 2))
            {
                var maybeNode = GetNodesAtPoint(new PointF(x, y));
                if (maybeNode.Count > 0)
                {
                    incorporateNode(maybeNode);
                }
            }
        }
        //do rightmost edge
        for (int y = MinY; y < MaxY; y += (NodeSizeY / 2))
        {
            var maybeNode = GetNodesAtPoint(new PointF(MaxX, y));
            if (maybeNode.Count > 0)
            {
                incorporateNode(maybeNode);
            }
        }
        //do bottom edge
        for (int x = MinX; x < MaxX; x += (NodeSizeY / 2))
        {
            var maybeNode = GetNodesAtPoint(new PointF(x, MaxY));
            if (maybeNode.Count > 0)
            {
                incorporateNode(maybeNode);
            }
        }
        selecting = false;

        void incorporateNode(List<Node> maybeNode)
        {
            foreach (var item in maybeNode)
            {
                if (subtracting)
                {
                    selected.Remove(item);
                }
                else if (!selected.Contains(item))
                {
                    selected.Add(item);
                }
            }
        }
    }

    private void UpdateLeftClick(PointF screenPos)
    {
        LeftClickFrameCounter++;
        if (LeftClickFrameCounter > 2)
        {
            if (!selecting)
            {
                StartSelecting(screenPos);
            }
        }
        Graph.Focus();
    }

    private void UpdateRightClick(PointF graphPos)
    {
        RightClickFrameCounter++;
        if (!MovingChild && RightClickFrameCounter > 1)
        {
            if (selected.Count > 0 && !selecting)
            {
                SelectedNodeOffsets.Clear();
                foreach (var node in selected)
                {
                    SelectedNodeOffsets.Add(new SizeF((node.Position.X - graphPos.X), (node.Position.Y - graphPos.Y)));
                }
            }
            else
            {
                Node node = UpdateClickedNode(graphPos);
                movedNode = node;
                OffsetFromDragClick = new SizeF((movedNode.Position.X - graphPos.X), (movedNode.Position.Y - graphPos.Y));
            }
            MovingChild = true;
        }
        Graph.Focus();
    }

    private void StartSelecting(PointF screenPos)
    {
        selecting = true;
        startSelectingMousePos = new((int)screenPos.X, (int)screenPos.Y);
        if (!subtracting && !adding)
        {
            selected.Clear();
        }
        SelectedNodeOffsets.Clear();
        TryCreateOverlayBitmap();
    }

    private void TryCreateOverlayBitmap()
    {
        if (nodeToLinkFrom != Node.NullNode || selecting)
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

    public static void ClearAllNodePos(Node node)
    {
        foreach (var pos in nodes.Values)
        {
            pos.Positions.ClearNode(node);
        }
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

    private List<Node> GetNodesAtPoint(PointF mouseGraphLocation)
    {
        if (adjustedMouseClipBounds.Contains(mouseGraphLocation))
        {
            List<Node> results = [];
            foreach (var key in nodes[SelectedCharacter].Positions[mouseGraphLocation])
            {
                if (key.Rectangle.Contains(mouseGraphLocation))
                {
                    results.Add(key);
                }
            }
            return results;
        }
        return [];
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
        if (string.IsNullOrEmpty(StoryName) || StoryName == NoCharacter || Story is null)
        {
            AddStory();
        }
        else if (Story is not null)
        {
            AddCharacterStory();
        }
    }

    private void AddCharacterStory()
    {
        List<object> list = [.. Enum.GetNames<StoryCharacters>()];
        foreach (var key in Stories.Keys)
        {
            list.Remove(key);
        }
        if (list.Count == 0)
        {
            MessageBox.Show("Cannot add any more characters", "No more!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        //add character story if it is not alredy and only characters supported in CSC
        DialogResult res = MessageBox.Show("Do you want to add a new character?", "New Character?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (res != DialogResult.Yes)
        {
            return;
        }

        var newChar = string.Empty;

        res = Dialogs.ShowDropdownBox(ref newChar, [.. list], "Character to add:", "Select Character");

        if (res != DialogResult.OK)
        {
            return;
        }

        AddCharacterStory(newChar);
        SetupStartPositions();
        return;
    }

    private void AddStory()
    {
        if (Story is null)
        {
            var newStoryName = string.Empty;
            var res = Dialogs.ShowTextBox(ref newStoryName, "Name for new Story:", "Story Title");

            if (res != DialogResult.OK)
            {
                return;
            }

            AddStory(newStoryName);
            SetupStartPositions();
        }
    }

    private void AddCharacterStory(string newCharacterName)
    {
        CharacterStory story = new(newCharacterName);
        Stories[newCharacterName] = story;
        SelectedCharacter = newCharacterName;
        AddCharacterAsItemToStory(newCharacterName);

        ExtractAndAddStories(newCharacterName);
    }

    private void AddStory(string newStoryName)
    {
        Story = new(newStoryName);
        StoryName = newStoryName;
        SelectedCharacter = Player;

        ExtractAndAddStories(Player, newStoryName);
    }

    private void NewStory_Click(object sender, EventArgs e)
    {
        if (Story is not null)
        {
            DialogResult res = MessageBox.Show("This will overwrite your current story and NOT SAVE", "Overwrite?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (res != DialogResult.OK)
            {
                return;
            }
        }
        Reset();

        AddStory();
        if (Story is not null)
        {
            foreach (var character in Enum.GetNames<StoryCharacters>())
            {
                AddCharacterStory(character);
                AddCharacterAsItemToStory(character);
            }
            SetupStartPositions();
        }
    }

    private static void AddCharacterAsItemToStory(string character) => Story.ItemOverrides.Add(new()
    {
        ItemName = character,
        DisplayInEditor = true,
        DisplayName = character,
        UseDefaultRadialOptions = true,
        ItemActions = [
                            new(){
                            ActionName = "Inspect",
                            OnTakeActionEvents = [
                                new(){
                                    EventType = GameEvents.DisplayGameMessage,
                                    Option = (int)GameMessageType.CenterScreenText,
                                    Value = "It's " + character,
                                    Character = AnybodyCharacters.Anybody.ToString(),
                                }
                                ]
                        },
                        new(){
                            ActionName = "Talk",
                            OnTakeActionEvents = [
                                new(){
                                    EventType = GameEvents.Dialogue,
                                    Option = (int)DialogueAction.TriggerStartDialogue,
                                    Character = character,
                                }
                                ]
                        },
                        new(){
                            ActionName = "Give",
                            Criteria = [
                                new(){
                                    BoolValue = BoolCritera.True,
                                    CompareType = CompareTypes.PlayerInventory,
                                    PlayerInventoryOption = PlayerInventoryOptions.HasAtLeastOneItem,
                                    EqualsValue = EqualsValues.Equals,
                                    EquationValue = ComparisonEquations.Equals,
                                    ValueFormula = ValueSpecificFormulas.EqualsValue,
                                    ItemComparison = ItemComparisonTypes.IsActive,
                                }
                                ],
                            OnTakeActionEvents = [
                                new(){
                                    EventType = GameEvents.Player,
                                    Option = (int)PlayerActions.TriggerGiveTo,
                                    Character = character,
                                }
                                ]
                        }
                            ]
    });

    private void Reset()
    {
        Story = null!;
        Stories.Clear();
        StoryName = NoCharacter;
        SelectedCharacter = NoCharacter;
        var keys = nodes.Keys;
        foreach (var item in keys)
        {
            if (item == NoCharacter)
            {
                continue;
            }
            nodes.Remove(item);
        }

        nodes.Add(Player, new());

        StoryTreeReset();
        Graph.Invalidate();
        PropertyInspector.Controls.Clear();
    }

    private void StoryTreeReset()
    {
        StoryTree.Nodes.Clear();
        TreeNode treeNode1 = new("Characters");
        TreeNode treeNode2 = new("Story Root", [treeNode1]);
        StoryTree.Nodes.AddRange([treeNode2]);
    }

    private static void DrawEdge(Graphics g, Node parent, Node child, Pen pen, PointF start = default, PointF end = default)
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
            start += new SizeF(parent.Size.Width, parent.Size.Height / 2);
        }
        else if (third == 1)
        {
            start += new SizeF(parent.Size.Width, parent.Size.Height / 5);
        }
        else if (third == 2)
        {
            start += new SizeF(parent.Size.Width, parent.Size.Height / 5 * 4);
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
        if (node == Node.NullNode)
        {
            return;
        }
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
        if (selected.Contains(node))
        {
            g.DrawRectangle(SelectionEdge, node.Rectangle);
        }

        if (Scaling[SelectedCharacter] > 0.28f)
        {
            var scaledRect = GetScaledRect(node.RectangleNonF, Scaling[SelectedCharacter]);
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

    private static GraphicsPath RoundedRect(RectangleF bounds, float radius)
    {//see https://stackoverflow.com/questions/33853434/how-to-draw-a-rounded-rectangle-in-c-sharp

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
        var g = e.Graphics;

        if (oldGraph is null || (nodeToLinkFrom == Node.NullNode && !selecting))
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
            scaledFont = GetScaledFont(new(DefaultFont.FontFamily, 8f), Scaling[SelectedCharacter]);

            DrawAllNodes(g);
        }
        else
        {
            g.DrawCachedBitmap(oldGraph, 0, 0);
            if (selecting)
            {
                DrawSelectionSquare(g);
            }
            else
            {
                DrawLinkToNextEdge(g);
            }
        }
    }

    private void DrawSelectionSquare(Graphics g)
    {
        Point pos;
        if (startSelectingMousePos != Point.Empty)
        {
            pos = startSelectingMousePos;
        }
        else
        {
            pos = Graph.PointToClient(Cursor.Position);
        }
        Point cursorNow = Graph.PointToClient(Cursor.Position);
        Rectangle rect;

        if (pos.X < cursorNow.X)
        {
            if (pos.Y < cursorNow.Y)
            {
                rect = new(pos, new Size(cursorNow.X - pos.X, cursorNow.Y - pos.Y));
            }
            else
            {
                rect = new(new(pos.X, cursorNow.Y), new Size(cursorNow.X - pos.X, pos.Y - cursorNow.Y));
            }
        }
        else
        {
            if (pos.Y < cursorNow.Y)
            {
                rect = new(new(cursorNow.X, pos.Y), new Size(pos.X - cursorNow.X, cursorNow.Y - pos.Y));
            }
            else
            {
                rect = new(cursorNow, new Size(pos.X - cursorNow.X, pos.Y - cursorNow.Y));
            }
        }

        g.FillRectangle(SelectionFill, rect);
        g.DrawRectangle(SelectionEdge, rect);
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
        if (Story is not null)
        {
            DialogResult res = MessageBox.Show("This will overwrite your current story and NOT SAVE", "Overwrite?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (res != DialogResult.OK)
            {
                return;
            }
        }
        Reset();

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
        Cursor = Cursors.WaitCursor;
        StoryFolder = Path.GetDirectoryName(FilePath)!;
        foreach (var file in Directory.GetFiles(StoryFolder))
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

        if (!TryLoadOldPositions())
        {
            SetupStartPositions();
            SafeSavePositions();
        }

        Graph.Invalidate();
        Cursor = Cursors.Default;

        StoryTree.SelectedNode = StoryTree.Nodes[0].Nodes[1].Nodes[Stories.Count - 1];
    }

    private void SafeSavePositions()
    {
        try
        {
            SavePositions();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            MessageBox.Show("It seems there are duplicate GUIDs in the story files. " +
                "The CCSC will now try and resolve that, but it will take a while and then save the new GUIDs back");
            Cursor = Cursors.WaitCursor;
            NodeLinker.InterlinkBetweenFiles(nodes, true);
            ExportAllFiles();
            if (!TryLoadOldPositions())
            {
                SetupStartPositions();
                SavePositions();
            }
            Cursor = Cursors.Default;
        }
    }

    private bool TryLoadOldPositions()
    {
        string positionPath = GetAppDataPathForCurrentStory();

        if (!File.Exists(positionPath))
        {
            return false;
        }

        var fileContent = File.ReadAllText(positionPath, System.Text.Encoding.Unicode);
        var positions = JsonSerializer.Deserialize<Dictionary<string, Dictionary<long, PointF>>>(fileContent)!;

        var lastCharacter = SelectedCharacter;

        var hadNodesNewlySet = false;

        foreach (var fileStore in positions.Keys)
        {
            if (fileStore == NoCharacter)
            {
                continue;
            }
            List<Node> notSet = [];
            SelectedCharacter = fileStore;
            foreach (var node in nodes[fileStore].Nodes)
            {
                if (positions[fileStore].TryGetValue(new NodeID(fileStore, node.Type, node.ID, node.OrigFileName, node.DataType), out var point))
                {
                    node.Position = point;
                }
                else
                {
                    notSet.Add(node);
                }
            }
            if (notSet.Count > 0)
            {
                foreach (var node in notSet)
                {
                    Debug.WriteLine(fileStore + "|" + node.FileName + ":" + node.ID);
                }
                SetStartPositionsForNodesInList(10, 0, nodes[fileStore], notSet, false);
                hadNodesNewlySet = true;
            }
            CenterOnNode(nodes[fileStore].Nodes.First(), 0.8f);
        }

        SelectedCharacter = lastCharacter;

        if (hadNodesNewlySet)
        {
            MessageBox.Show("Some nodes were new this time around, you'll find them in each file around the 0/0 mark.");
        }

        return true;
    }

    private static void SavePositions()
    {
        string positionPath = GetAppDataPathForCurrentStory();

        var lastCharacter = SelectedCharacter;
        Dictionary<string, Dictionary<long, PointF>> positions = [];

        foreach (var nodeStore in nodes.Keys)
        {
            if (nodeStore == NoCharacter)
            {
                continue;
            }
            SelectedCharacter = nodeStore;
            positions.Add(nodeStore, []);
            foreach (var node in nodes[nodeStore].Nodes)
            {
                positions[nodeStore].Add(new NodeID(nodeStore, node.Type, node.ID, node.OrigFileName, node.DataType), node.Position);
            }
        }

        var fileContent = JsonSerializer.Serialize(positions)!;
        if (!File.Exists(positionPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(positionPath)!);
        }
        File.WriteAllText(positionPath, fileContent, System.Text.Encoding.Unicode);

        SelectedCharacter = lastCharacter;
    }

    private static string GetAppDataPathForCurrentStory() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CCSC", "Positions", StoryName, "Positions.json");

    private void LoadFileIntoStore(string FilePath)
    {
        string fileString = File.ReadAllText(FilePath);
        fileString = fileString.Replace('', ' ');

        string FileName;
        string? storyName = null;
        //else create new
        try
        {
            var settings = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Skip
            };
            if (Path.GetExtension(FilePath) == ".story")
            {
                Story = JsonSerializer.Deserialize<MainStory>(fileString, settings) ?? new MainStory();
                FileName = Player;
                storyName = Path.GetFileNameWithoutExtension(FilePath);
            }
            else
            {
                CharacterStory story = JsonSerializer.Deserialize<CharacterStory>(fileString, settings) ?? new CharacterStory();
                FileName = story.CharacterName!;
                Stories.Add(FileName, story);

            }
        }
        catch (JsonException ex)
        {
            Debug.WriteLine(ex.Message);
            return;
        }
        catch (AggregateException ex)
        {
            Debug.WriteLine(ex.Message);
            return;
        }

        ExtractAndAddStories(FileName, storyName);

        Graph.Invalidate();
        return;
    }

    private void ExtractAndAddStories(string FileName, string? storyName = null)
    {
        NodeStore tempStore = new();
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
            NodeLinker.DissectStory(Story, tempStore);
        }
        else
        {
            NodeLinker.DissectCharacter(Stories[FileName], tempStore);
        }
        //even link for single file, should be able to link most suff so it stays readable
        NodeLinker.Interlinknodes(tempStore, FileName);

        if (SelectedCharacter == Player)
        {
            AddItemToStoryTree(true, storyName!);
        }
        else
        {
            AddItemToStoryTree(false, FileName);
        }
    }

    private void AddItemToStoryTree(bool isStory, string FileName)
    {
        if (isStory)
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

            NodeStore nodeStore = nodes[store];
            var nodeList = nodeStore.Nodes;
            SetStartPositionsForNodesInList(100, 1, nodeStore, nodeList);

            CenterOnNode(nodes[store].Nodes.First(), 0.8f);
        }
    }

    private void SetStartPositionsForNodesInList(int intX, int intY, NodeStore nodeStore, List<Node> nodeList, bool inListOnly = false)
    {
        int ParentEdgeMaxStartValue = 0;
        maxYperX.Clear();
        maxYperX.ExtendToIndex(intX, intY);

        for (int i = 0; i < maxYperX.Count; i++)
        {
            maxYperX[i] = intY;
        }

        nodeList.Sort(new NodeParentComparer(nodeStore));
        bool restarted = false;
        int skipCount = 0;
    Restart:
        visited.Clear();
        foreach (var key in nodeList)
        {
            Family family = nodeStore[key];
            if (!restarted && (family.Parents.Count > ParentEdgeMaxStartValue || visited.Contains(key)))
            {
                skipCount++;
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
            maxYperX.ExtendToIndex(intX, intY);

            intX = SetStartPosForConnected(intX, nodeStore, key, inListOnly);
        }
        if (!restarted && skipCount == nodeList.Count)
        {
            restarted = true;
            goto Restart;
        }

    }

    private int SetStartPosForConnected(int intX, NodeStore nodeStore, Node start, bool inListOnly = false)
    {
        maxYperX.ExtendToIndex(intX, (int)(start.Position.Y / scaleY));

        Queue<Node> toExplore = [];
        Queue<int> layerX = [];
        toExplore.Enqueue(start);
        layerX.Enqueue(intX);
        int lastIntX = 0;

        //Debug.WriteLine($"starting on {key.ID} at {intX}|{1}");

        while (toExplore.Count > 0)
        {
            var node = toExplore.Dequeue();
            intX = layerX.Dequeue();

            if (visited.Contains(node) || (inListOnly && !selected.Contains(node)))
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
            rest = rest <= 0 ? 1 : rest;

            var newPos = new PointF(intX * scaleX, maxYperX[intX] * scaleY);
            //if we are more right along the screen this is a child
            if (lastIntX < intX)
            {
                ShoveNodesToRight(node, newPos);
            }
            else
            {
                ShoveNodesToLeft(node, newPos);
            }

            node.Position = newPos;

            maxYperX[intX] += rest;

            if (parents.Count > 0)
            {
                if (maxYperX[newParentsX] < maxYperX[intX] - rest)
                {
                    maxYperX[newParentsX] = maxYperX[intX] - rest;
                }

                foreach (var item in parents)
                {
                    if (visited.Contains(item) || (inListOnly && !selected.Contains(item)))
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
                    if (visited.Contains(item) || (inListOnly && !selected.Contains(item)))
                    {
                        continue;
                    }

                    layerX.Enqueue(newChildX);
                    toExplore.Enqueue(item);
                }
            }
            lastIntX = intX;
        }

        return intX;
    }

    private void Save_Click(object sender, EventArgs e)
    {
        if (Story is null)
        {
            return;
        }

        if (!ExportAllFiles())
        {
            return;
        }
        SafeSavePositions();

        MessageBox.Show("Saved files!");
    }

    private bool ExportAllFiles()
    {
        if (string.IsNullOrEmpty(StoryFolder) || !Directory.Exists(StoryFolder))
        {
            var folder = new FolderBrowserDialog()
            {
                Description = "select story save location",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = true,
                OkRequiresInteraction = true,
                AddToRecent = true,
                ShowPinnedPlaces = true,
                RootFolder = Environment.SpecialFolder.Recent
            };
            var res = folder.ShowDialog();

            if (res == DialogResult.OK)
            {
                StoryFolder = folder.SelectedPath;
            }
            else
            {
                return false;
            }
        }

        ExportFile(StoryName);
        foreach (var character in Stories.Keys)
        {
            ExportFile(character);
        }

        return true;
    }

    private void ExportFile(string FileName)
    {
        if (Path.GetFileNameWithoutExtension(StoryFolder) != StoryName)
        {
            var path = Directory.CreateDirectory(Path.Combine(StoryFolder, StoryName));
            StoryFolder = path.FullName;
        }

        try
        {
            var settings = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Skip,
                WriteIndented = true,
            };
            if (StoryName == FileName)
            {
                var data = JsonSerializer.Serialize(Story, settings);
                File.WriteAllText(Path.Combine(StoryFolder, FileName + ".story"), data, System.Text.Encoding.Unicode);
            }
            else
            {
                var data = JsonSerializer.Serialize(Stories[FileName], settings);
                File.WriteAllText(Path.Combine(StoryFolder, FileName + ".character"), data, System.Text.Encoding.Unicode);
            }
        }
        catch (JsonException ex)
        {
            Debug.WriteLine(ex.Message);
            return;
        }
        catch (AggregateException ex)
        {
            Debug.WriteLine(ex.Message);
            return;
        }

        return;
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
                    NodeLinker.Unlink(nodes[SelectedCharacter], nodeToLinkFrom, node);
                }
                else
                {
                    NodeLinker.Link(nodes[SelectedCharacter], nodeToLinkFrom, node);
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
                    NodeLinker.Link(nodes[SelectedCharacter], nodeToLinkFrom, circleNode);

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

        var list = GetSpawnableNodeTypes();

        if (clickedNode != Node.NullNode)
        {
            NodeContext.Items.Add(SortConnectedMenu);
            NodeContext.Items.Add(PullChildsMenu);
            NodeContext.Items.Add(PullParentsMenu);
            NodeContext.Items.Add(Seperator1);
        }

        else if (selected.Count > 0)
        {
            NodeContext.Items.Add(SortSelectedConnectedMenu);
            NodeContext.Items.Add(SortSelectedMenu);
            NodeContext.Items.Add(Seperator1);
        }

        foreach (var item in list)
        {
            var button = new ToolStripMenuItem("Add " + item.ToString(), null, onClick: (_, _) =>
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
                        //todo this is quite involved it seems so i dont want to do it for now
                        PutCompareType(node, criterion);
                        break;
                    }
                    case CompareTypes.Clothing:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);
                        PutEnumValue<ClothingType>(node, criterion);
                        PutEnumOption<ClothingSet>(node, criterion);
                        PutBoolCriteria(node, criterion);
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

                        ComboBox valueChar1 = GetComboBox();
                        if (criterion.Key == Player)
                        {
                            valueChar1.Items.AddRange([.. Story.PlayerValues!]);
                        }
                        else
                        {
                            valueChar1.Items.AddRange([.. Stories[criterion.Character!].StoryValues!]);
                        }
                        valueChar1.SelectedItem = criterion.Key!;
                        valueChar1.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Key = valueChar1.SelectedItem!.ToString()!);
                        PropertyInspector.Controls.Add(valueChar1);

                        ComboBox formula = GetComboBox();
                        formula.Items.AddRange(Enum.GetNames(typeof(ValueSpecificFormulas)));
                        formula.SelectedIndex = (int)criterion.ValueFormula!;
                        formula.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.ValueFormula = (ValueSpecificFormulas)formula.SelectedIndex!);
                        PropertyInspector.Controls.Add(formula);

                        PutCharacter2(node, criterion);

                        ComboBox valueChar2 = GetComboBox();
                        if (criterion.Key2 == Player)
                        {
                            valueChar2.Items.AddRange([.. Story.PlayerValues!]);
                        }
                        else
                        {
                            valueChar2.Items.AddRange([.. Stories[criterion.Character2!].StoryValues!]);
                        }
                        valueChar2.SelectedItem = criterion.Key2!;
                        valueChar2.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Key2 = valueChar2.SelectedItem!.ToString()!);
                        PropertyInspector.Controls.Add(valueChar2);

                        break;
                    }
                    case CompareTypes.CriteriaGroup:
                    {
                        PutCompareType(node, criterion);

                        ComboBox group = GetComboBox();
                        Dictionary<string, CriteriaGroup> groups = [];
                        for (int i = 0; i < Story.CriteriaGroups!.Count; i++)
                        {
                            group.Items.Add(Story.CriteriaGroups[i].Name!);
                            groups.Add(Story.CriteriaGroups[i].Name!, Story.CriteriaGroups[i]);
                        }
                        group.SelectedItem = criterion.Value!;
                        group.SelectedIndex = group.SelectedIndex;
                        group.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) =>
                        {
                            if (groups.TryGetValue(group.SelectedItem!.ToString()!, out var val))
                            {
                                criterion.Value = val.Name;
                                criterion.Key = val.Id;
                                criterion.Key2 = val.PassCondition.ToString();
                            }
                        });
                        PropertyInspector.Controls.Add(group);

                        PutEnumOption<BoolCritera>(node, criterion);
                        break;
                    }
                    case CompareTypes.CutScene:
                    {
                        PutCompareType(node, criterion);
                        PutEnumValue<CutscenePlaying>(node, criterion);
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.Dialogue:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);

                        ComboBox dialogue = GetComboBox();
                        for (int i = 0; i < Stories[criterion.Character!].Dialogues!.Count; i++)
                        {
                            dialogue.Items.Add(Stories[criterion.Character!].Dialogues![i].ID.ToString());
                        }
                        dialogue.SelectedItem = criterion.Value!;
                        dialogue.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Value = dialogue.SelectedItem!.ToString()!);
                        PropertyInspector.Controls.Add(dialogue);

                        PutEnumValue<DialogueStatuses>(node, criterion);
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
                        PutEnumKey<Doors>(node, criterion);
                        PutEnumOption<DoorOptionValues>(node, criterion);
                        break;
                    }
                    case CompareTypes.Gender:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);
                        PutEnumOption<Gender>(node, criterion);
                        break;
                    }
                    case CompareTypes.IntimacyPartner:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);
                        PutEquals(node, criterion);
                        PutEnumValueText<IntimateCharacters>(node, criterion);
                        break;
                    }
                    case CompareTypes.IntimacyState:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);
                        PutEquals(node, criterion);
                        PutEnumValueText<SexualActs>(node, criterion);
                        break;
                    }
                    case CompareTypes.InZone:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);
                        PutZone(node, criterion);
                        PutBoolCriteria(node, criterion);
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
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.Item:
                    {
                        PutCompareType(node, criterion);
                        PutItem(node, criterion);

                        ComboBox state = GetComboBox();
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
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.IsCharacterEnabled:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.IsCurrentlyBeingUsed:
                    {
                        PutCompareType(node, criterion);
                        PutItem(node, criterion);
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.IsCurrentlyUsing:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);
                        PutItem(node, criterion);
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.IsExplicitGameVersion:
                    {
                        PutCompareType(node, criterion);
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.IsGameUncensored:
                    {
                        PutCompareType(node, criterion);
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.IsPackageInstalled:
                    {
                        PutCompareType(node, criterion);
                        PutEnumValueText<Packages>(node, criterion);
                        break;
                    }
                    case CompareTypes.IsInFrontOf:
                    {
                        PutCharacter1(node, criterion);
                        PutCompareType(node, criterion);
                        PutCharacter2(node, criterion);
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.IsInHouse:
                    {
                        PutCharacter1(node, criterion);
                        PutCompareType(node, criterion);
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.IsNewGame:
                    {
                        PutCompareType(node, criterion);
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.IsZoneEmpty:
                    {
                        PutCompareType(node, criterion);
                        PutZone(node, criterion);
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.ItemFromItemGroup:
                    {
                        PutCompareType(node, criterion);

                        ComboBox box = GetComboBox();
                        foreach (var group in Story.ItemGroups)
                        {
                            box.Items.Add(group);
                        }
                        box.SelectedItem = criterion.Key;
                        box.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Key = box.SelectedItem!.ToString()!);
                        PropertyInspector.Controls.Add(box);

                        box = GetComboBox();
                        box.Items.AddRange(Enum.GetNames(typeof(ItemFromItemGroupComparisonTypes)));
                        box.SelectedItem = criterion.ItemComparison.ToString();
                        box.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.ItemFromItemGroupComparison = Enum.Parse<ItemFromItemGroupComparisonTypes>(box.SelectedItem!.ToString()!));
                        PropertyInspector.Controls.Add(box);

                        switch (criterion.ItemFromItemGroupComparison)
                        {
                            case ItemFromItemGroupComparisonTypes.IsVisibleTo:
                            {
                                PutCharacter1(node, criterion);
                                break;
                            }
                            case ItemFromItemGroupComparisonTypes.IsMountedTo:
                            {
                                //todo this is broken in the original CSC??
                                break;
                            }
                            default:
                            case ItemFromItemGroupComparisonTypes.IsActive:
                            case ItemFromItemGroupComparisonTypes.IsMounted:
                            case ItemFromItemGroupComparisonTypes.IsHeldByPlayer:
                            case ItemFromItemGroupComparisonTypes.IsInPlayerInventory:
                            case ItemFromItemGroupComparisonTypes.IsInventoriedOrHeldByPlayer:
                            {
                                break;
                            }
                        }
                        PutBoolCriteria(node, criterion);

                        break;
                    }
                    case CompareTypes.MetByPlayer:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.Personality:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);
                        PutEnumKey<PersonalityTraits>(node, criterion);
                        PutComparison(node, criterion);
                        PutNumericValue(criterion);
                        break;
                    }
                    case CompareTypes.PlayerGender:
                    {
                        PutCompareType(node, criterion);
                        PutEnumValueText<Gender>(node, criterion);
                        break;
                    }
                    case CompareTypes.PlayerInventory:
                    {
                        PutCompareType(node, criterion);

                        ComboBox state = GetComboBox();
                        state.Items.AddRange(Enum.GetNames(typeof(PlayerInventoryOptions)));
                        state.SelectedItem = criterion.PlayerInventoryOption.ToString();
                        state.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.PlayerInventoryOption = Enum.Parse<PlayerInventoryOptions>(state.SelectedItem!.ToString()!));
                        PropertyInspector.Controls.Add(state);
                        if (criterion.PlayerInventoryOption == PlayerInventoryOptions.HasItem)
                        {
                            PutItem(node, criterion);
                        }
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.PlayerPrefs:
                    {
                        PutCompareType(node, criterion);
                        PutEnumKey<PlayerPrefs>(node, criterion);
                        PutComparison(node, criterion);
                        PutTextValue(criterion);

                        break;
                    }
                    case CompareTypes.Posing:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);

                        ComboBox option = GetComboBox();
                        option.Items.AddRange(Enum.GetNames(typeof(PoseOptions)));
                        option.SelectedItem = criterion.PoseOption.ToString();
                        option.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.PoseOption = Enum.Parse<PoseOptions>(option.SelectedItem!.ToString()!));
                        option.SelectedIndexChanged += (_, _) => ShowProperties(node);
                        PropertyInspector.Controls.Add(option);

                        if (criterion.PoseOption == PoseOptions.CurrentPose)
                        {
                            PutEquals(node, criterion);
                            PutEnumValue<Poses>(node, criterion);
                        }
                        else
                        {
                            PutBoolCriteria(node, criterion);
                        }
                        break;
                    }
                    case CompareTypes.Property:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);
                        PutEnumValue<InteractiveProperties>(node, criterion);
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.Quest:
                    {
                        PutCompareType(node, criterion);

                        ComboBox quest = GetComboBox();
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
                            criterion.Key2 = quest.SelectedItem!.ToString()!;
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
                        PutEnumValue<QuestStatus>(node, criterion);
                        break;
                    }
                    case CompareTypes.SameZoneAs:
                    {
                        PutCharacter1(node, criterion);
                        PutCompareType(node, criterion);
                        PutCharacter2(node, criterion);
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.ScreenFadeVisible:
                    {
                        PutCompareType(node, criterion);
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.Social:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);

                        ComboBox social = GetComboBox();
                        social.Items.AddRange(Enum.GetNames(typeof(SocialStatuses)));
                        social.SelectedItem = criterion.SocialStatus!.ToString();
                        social.AddComboBoxHandler(node, nodes[SelectedCharacter], (sender, values) => criterion.SocialStatus = Enum.Parse<SocialStatuses>(social.SelectedItem!.ToString()!));
                        PropertyInspector.Controls.Add(social);

                        PutComparison(node, criterion);
                        PutNumericValue(criterion);
                        break;
                    }
                    case CompareTypes.State:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);
                        PutEnumValue<InteractiveStates>(node, criterion);
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.Value:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion).SelectedIndexChanged += (_, _) => ShowProperties(node);

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
                            for (int i = 0; i < Stories[criterion.Character!].StoryValues!.Count; i++)
                            {
                                value.Items.Add(Stories[criterion.Character!].StoryValues![i]);
                            }
                        }
                        value.SelectedItem = criterion.Key!;
                        value.AddComboBoxHandler(node, nodes[SelectedCharacter], (sender, values) => criterion.Key = value.SelectedItem!.ToString()!);
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
                        PutBoolCriteria(node, criterion);
                        break;
                    }
                    case CompareTypes.UseLegacyIntimacy:
                    {
                        PutCompareType(node, criterion);
                        PutBoolCriteria(node, criterion);
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

                ComboBox talkingTo = GetComboBox();
                talkingTo.Items.AddRange(Enum.GetNames(typeof(AnybodyCharacters)));
                talkingTo.SelectedItem = dialogue.SpeakingTo;
                talkingTo.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => dialogue.SpeakingTo = talkingTo.SelectedItem!.ToString()!);
                PropertyInspector.Controls.Add(talkingTo);

                label = GetLabel("Importance:");
                PropertyInspector.Controls.Add(label);

                ComboBox importance = GetComboBox();
                importance.Items.AddRange(Enum.GetNames(typeof(Importance)));
                importance.SelectedItem = dialogue.SpeakingTo;
                importance.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => dialogue.SpeakingTo = importance.SelectedItem!.ToString()!);
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

                ComboBox pairedEmote = GetComboBox();
                pairedEmote.Items.AddRange(Enum.GetNames(typeof(BGCEmotes)));
                pairedEmote.SelectedItem = dialogue.PairedEmote;
                pairedEmote.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => dialogue.PairedEmote = Enum.Parse<BGCEmotes>(pairedEmote.SelectedItem.ToString()!));
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
                PropertyInspector.SetColumnSpan(text, 10);
                PropertyInspector.AutoSize = false;
                break;
            }
            case NodeType.Dialogue:
            {
                PropertyInspector.RowCount = 2;
                Label label = GetLabel(node.FileName + "'s Dialogue " + node.ID + "\n Talking to:");
                PropertyInspector.Controls.Add(label);
                if (node.Data<Dialogue>() is null)
                {
                    Label label2 = GetLabel("No data on this node", ContentAlignment.TopCenter);
                    PropertyInspector.Controls.Add(label2);
                    break;
                }
                Dialogue dialogue = node.Data<Dialogue>()!;

                ComboBox talkingTo = GetComboBox();
                talkingTo.Items.AddRange(Enum.GetNames(typeof(Characters)));
                talkingTo.SelectedItem = dialogue.SpeakingToCharacterName;
                talkingTo.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => dialogue.SpeakingToCharacterName = talkingTo.SelectedItem!.ToString()!);
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

                checkBox = GetCheckbox("Auto Immersive:", dialogue.AutoImmersive);
                checkBox.CheckedChanged += (_, args) => dialogue.AutoImmersive = checkBox.Checked;
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
                PropertyInspector.SetColumnSpan(text, 10);
                PropertyInspector.AutoSize = false;

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
                PropertyInspector.SetColumnSpan(text, 10);
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
                PropertyInspector.RowStyles[0].SizeType = SizeType.Absolute;
                PropertyInspector.RowStyles[0].Height = 35;
                PropertyInspector.ColumnCount = 7;
                GeventPropertyCounter = 0;

                GameEvent gevent = node.Data<GameEvent>()!;

                if (gevent.Character == "#" || string.IsNullOrEmpty(gevent.Character))
                {
                    gevent.Character = AnybodyCharacters.Anybody.ToString();
                }

                Label label = GetLabel("Order:");
                PropertyInspector.Controls.Add(label, 0, 0);

                NumericUpDown sortOrder = GetNumericUpDown(gevent.SortOrder);
                sortOrder.DecimalPlaces = 0;
                sortOrder.Minimum = 0;
                sortOrder.ValueChanged += (_, _) => gevent.SortOrder = (int)sortOrder.Value;
                PropertyInspector.Controls.Add(sortOrder, 1, 0);

                ComboBox type = GetComboBox();
                type.Items.AddRange(Enum.GetNames(typeof(GameEvents)));
                type.SelectedItem = gevent.EventType.ToString();
                type.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) =>
                    {
                        gevent.EventType = Enum.Parse<GameEvents>(type.SelectedItem.ToString()!);
                        ShowProperties(node);
                    });
                PropertyInspector.Controls.Add(type, 2, 0);

                Label label3 = GetLabel("Delay:");
                PropertyInspector.Controls.Add(label3, 3, 0);

                NumericUpDown delay = GetNumericUpDown((float)gevent.Delay);
                delay.ValueChanged += (_, _) => gevent.Delay = (double)delay.Value;
                PropertyInspector.Controls.Add(delay, 4, 0);

                PropertyInspector.Controls.Add(new Panel(), 5, 0);

                switch (gevent.EventType)
                {
                    case GameEvents.AddForce:
                    {
                        PutCharacter1(node, gevent);
                        float floatX = 0;
                        float floatY = 0;
                        float floatZ = 0;
                        var x = GetNumericUpDown(0);
                        x.DecimalPlaces = 3;
                        x.ValueChanged += (_, _) => floatX = (float)x.Value;
                        x.ValueChanged += (_, _) => gevent.Value = $"{floatX}:{floatY}:{floatZ}";
                        PropertyInspector.Controls.Add(x, GeventPropertyCounter++, 1);
                        var y = GetNumericUpDown(0);
                        y.DecimalPlaces = 3;
                        y.ValueChanged += (_, _) => floatX = (float)y.Value;
                        y.ValueChanged += (_, _) => gevent.Value = $"{floatX}:{floatY}:{floatZ}";
                        PropertyInspector.Controls.Add(y, GeventPropertyCounter++, 1);
                        var z = GetNumericUpDown(0);
                        z.DecimalPlaces = 3;
                        z.ValueChanged += (_, _) => floatX = (float)z.Value;
                        z.ValueChanged += (_, _) => gevent.Value = $"{floatX}:{floatY}:{floatZ}";
                        PropertyInspector.Controls.Add(z, GeventPropertyCounter++, 1);

                        break;
                    }
                    case GameEvents.AllowPlayerSave:
                    {
                        PutEnumOption<BoolCritera>(node, gevent);
                        break;
                    }
                    case GameEvents.ChangeBodyScale:
                    {
                        PutCharacter1(node, gevent);
                        PutNumberValue(gevent);
                        PutNumberValue2(gevent);
                        break;
                    }
                    case GameEvents.CharacterFromCharacterGroup:
                    {
                        //todo seems very involved, not gonna implement now
                        break;
                    }
                    case GameEvents.CharacterFunction:
                    {
                        //todo have to find out how to do functions
                        break;
                    }
                    case GameEvents.Clothing:
                    {
                        PutCharacter1(node, gevent);
                        PutEnumOption2<ClothingOptions>(node, gevent);

                        switch ((ClothingOptions)gevent.Option2)
                        {
                            case ClothingOptions.Change:
                            {
                                PutEnumOption4<ClothingChangeOptions>(node, gevent);

                                switch ((ClothingChangeOptions)gevent.Option4)
                                {
                                    case ClothingChangeOptions.ClothingType:
                                    {
                                        PutEnumValue<ClothingType>(node, gevent);
                                        PutEnumOption<ClothingOnOff>(node, gevent);

                                        if (gevent.Option == 0)
                                        {
                                            PutEnumOption3<SetEnum>(node, gevent);
                                        }

                                        break;
                                    }
                                    case ClothingChangeOptions.AllOn:
                                    case ClothingChangeOptions.AllOff:
                                    {
                                        break;
                                    }
                                    case ClothingChangeOptions.ChangeItem:
                                    {
                                        //todo items warable by - in value
                                        PutEnumOption<ClothingOnOff>(node, gevent);

                                        break;
                                    }
                                    case ClothingChangeOptions.ChangeToOutfit:
                                    {
                                        //todo outfits wearable by - in value
                                        break;
                                    }
                                    case ClothingChangeOptions.RemoveFromOutfit:
                                    {
                                        PutEnumOption<ClothingTypeOrName>(node, gevent);

                                        if (gevent.Option == 0)
                                        {
                                            //todo items warable by - in value
                                        }
                                        else
                                        {
                                            PutEnumOption3<ClothingType>(node, gevent);
                                        }

                                        break;
                                    }
                                    default:
                                        break;
                                }

                                break;
                            }
                            case ClothingOptions.ToggleBloodyEffect:
                            case ClothingOptions.ToggleWetEffect:
                            {
                                PutEnumValue<ClothingType>(node, gevent);
                                PutEnumOption<ClothingOnOff>(node, gevent);

                                break;
                            }
                            default:
                                break;
                        }

                        break;
                    }
                    case GameEvents.Combat:
                    {
                        PutCharacter1(node, gevent);
                        PutEnumOption<CombatOptions>(node, gevent);
                        if (gevent.Option == (int)(CombatOptions.Fight))
                        {
                            PutCharacter2(node, gevent);
                        }
                        break;
                    }
                    case GameEvents.CutScene:
                    {
                        PutEnumOption<CutsceneAction>(node, gevent);
                        switch ((CutsceneAction)gevent.Option)
                        {
                            case CutsceneAction.PlayScene:
                            case CutsceneAction.EndScene:
                            {
                                PutEnumKeyText<Cutscenes>(node, gevent);
                                break;
                            }
                            case CutsceneAction.PlayRandomSceneFromLocation:
                            {
                                PutEnumOption2<Zones>(node, gevent);
                                break;
                            }
                            case CutsceneAction.PlayRandomSceneFromCurrentLocation:
                            {
                                gevent.Key = string.Empty;
                                gevent.Option2 = 0;
                                break;
                            }
                        }

                        if (gevent.Option == (int)CutsceneAction.EndAnySceneWithPlayer || gevent.Option == (int)CutsceneAction.EndScene)
                        {
                            gevent.Character = string.Empty;
                            gevent.Character2 = string.Empty;
                            gevent.Value = string.Empty;
                            gevent.Value2 = string.Empty;
                        }
                        else
                        {
                            PutCharacter1(node, gevent);
                            PutEnumValueText<NoneCharacters>(node, gevent);
                            PutEnumValue2Text<NoneCharacters>(node, gevent);
                            PutCharacter2(node, gevent);
                        }
                        break;
                    }
                    case GameEvents.Dialogue:
                    {
                        PutCharacter1(node, gevent);
                        PutEnumOption<DialogueAction>(node, gevent);
                        if (gevent.Option == 1)
                        {
                            PutCharacter2(node, gevent);
                        }
                        if (gevent.Option == 0 || gevent.Option == 5)
                        {
                            var box = GetComboBox();
                            foreach (var item in Stories[gevent.Character].Dialogues)
                            {
                                box.Items.Add(item.ID);
                            }
                            box.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Value = box.SelectedItem!.ToString()!);
                        }

                        break;
                    }
                    case GameEvents.DisableNPC:
                    {
                        PutCharacter1(node, gevent);
                        break;
                    }
                    case GameEvents.DisplayGameMessage:
                    {
                        gevent.Key = "";

                        var options = GetComboBox();
                        options.Items.AddRange(Enum.GetNames<GameMessageType>());
                        if (gevent.Option >= 21)
                        {
                            gevent.Option = 0;
                        }
                        options.SelectedIndex = gevent.Option / 10;
                        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Option = options.SelectedIndex);
                        options.SelectedIndexChanged += (_, _) => ShowProperties(node);
                        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);

                        PutTextValue(gevent);

                        break;
                    }
                    case GameEvents.Door:
                    {
                        PutEnumKeyText<Doors>(node, gevent);
                        PutEnumOption<DoorAction>(node, gevent);
                        break;
                    }
                    case GameEvents.Emote:
                    {
                        PutCharacter1(node, gevent);
                        PutEnumValue<Emotions>(node, gevent);
                        PutNumberValue2(gevent);

                        break;
                    }
                    case GameEvents.EnableNPC:
                    {
                        PutCharacter1(node, gevent);
                        break;
                    }
                    case GameEvents.EventTriggers:
                    {
                        PutCharacter1(node, gevent);
                        gevent.Character2 = "#";
                        gevent.Key = "";

                        PutEnumOption<TriggerOptions>(node, gevent);

                        if (gevent.Option == 1)
                        {
                            PutEnumOption2<BoolCritera>(node, gevent);
                        }

                        var value = GetComboBox();
                        string character = gevent.Character;
                        if (character == Player || character == "#")
                        {
                            for (int i = 0; i < Story.PlayerReactions!.Count; i++)
                            {
                                value.Items.Add(Story.PlayerReactions![i].Name);
                            }
                        }
                        else
                        {
                            if (Stories.TryGetValue(character, out CharacterStory? _story))
                            {
                                for (int i = 0; i < _story.Reactions!.Count; i++)
                                {
                                    value.Items.Add(_story.Reactions![i].Name);
                                }
                            }
                        }
                        value.SelectedItem = gevent.Value;
                        value.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Value = value.SelectedItem.ToString()!);
                        PropertyInspector.Controls.Add(value, GeventPropertyCounter++, 1);
                        PropertyInspector.SetColumnSpan(value, 2);

                        break;
                    }
                    case GameEvents.FadeIn:
                    case GameEvents.FadeOut:
                    {
                        PutNumberValue(gevent);
                        break;
                    }
                    case GameEvents.IKReach:
                    {
                        PutCharacter1(node, gevent);
                        PutEnumOption2<IKTargets>(node, gevent);
                        PutEnumOption<ClothingOnOff>(node, gevent);

                        if (gevent.Option == 0)
                        {
                            var box = GetComboBox();
                            box.Items.AddRange(Enum.GetNames<Characters>());
                            box.Items.AddRange(Enum.GetNames<Items>());
                            box.SelectedItem = gevent.Key;
                            box.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Key = box.SelectedItem!.ToString()!);
                            PropertyInspector.Controls.Add(box, GeventPropertyCounter++, 1);
                        }
                        break;
                    }
                    case GameEvents.Intimacy:
                    {
                        PutCharacter1(node, gevent);
                        PutEnumOption<IntimacyOptions>(node, gevent);
                        if (gevent.Option == 0)
                        {
                            PutEnumValue<IntimacyEvents>(node, gevent);
                            if (!(Enum.Parse<IntimacyEvents>(gevent.Value) is IntimacyEvents.End or IntimacyEvents.StartMasturbation))
                            {
                                PutCharacter2(node, gevent);
                            }
                        }

                        break;
                    }
                    case GameEvents.Item:
                    {
                        //todo seems pretty involved
                        break;
                    }
                    case GameEvents.ItemFromItemGroup:
                    {
                        //todo (itemgroups) just as involved as the item gameevent type
                        break;
                    }
                    case GameEvents.LookAt:
                    {
                        PutCharacter1(node, gevent);
                        PutEnumOption<LookAtTargets>(node, gevent);

                        switch ((LookAtTargets)gevent.Option)
                        {
                            case LookAtTargets.Character:
                            {
                                PutCharacter2(node, gevent);
                                gevent.Value = string.Empty;
                                break;
                            }
                            case LookAtTargets.InteractiveItem:
                            {
                                PutEnumValueText<Items>(node, gevent);
                                gevent.Character2 = string.Empty;
                                break;
                            }
                            case LookAtTargets.GameObject:
                            {
                                PutTextValue(gevent);
                                gevent.Character2 = string.Empty;
                                break;
                            }
                            case LookAtTargets.PlayerPenis:
                            {
                                gevent.Character2 = string.Empty;
                                gevent.Value = string.Empty;
                                break;
                            }
                        }
                        break;
                    }
                    case GameEvents.Personality:
                    {
                        PutCharacter1(node, gevent);
                        PutEnumOption<PersonalityTraits>(node, gevent);
                        PutEnumOption2<Modification>(node, gevent);
                        PutNumberValue(gevent);
                        break;
                    }
                    case GameEvents.Property:
                    {
                        PutCharacter1(node, gevent);
                        PutEnumValue<InteractiveProperties>(node, gevent);
                        PutEnumOption<BoolCritera>(node, gevent);
                        break;
                    }
                    case GameEvents.MatchValue:
                    case GameEvents.CombineValue:
                    {
                        PutCharacter1(node, gevent);
                        PutValueKey(node, gevent.Character, gevent);

                        Label typeLabel = GetLabel(gevent.EventType.ToString());
                        PropertyInspector.Controls.Add(typeLabel, GeventPropertyCounter++, 1);

                        PutCharacter2(node, gevent);
                        PutValueValue(node, gevent.Character2, gevent);
                        break;
                    }
                    case GameEvents.ModifyValue:
                    {
                        gevent.Character2 = "#";
                        PutCharacter1(node, gevent);
                        PutValueKey(node, gevent.Character, gevent);
                        PutEnumOption<Modification>(node, gevent);
                        PutNumberValue(gevent);
                        break;
                    }
                    case GameEvents.Player:
                    {
                        PutEnumOption<PlayerActions>(node, gevent);
                        switch ((PlayerActions)gevent.Option)
                        {
                            case PlayerActions.Inventory:
                            {
                                PutEnumOption2<AddRemoveActions>(node, gevent);
                                PutEnumValueText<Items>(node, gevent);
                                break;
                            }
                            case PlayerActions.TriggerGiveTo:
                            {
                                PutCharacter1(node, gevent);
                                break;
                            }
                            case PlayerActions.Sit:
                            {
                                //todo implement chairs
                                break;
                            }
                            case PlayerActions.LayDown:
                            {
                                //todo implement beds
                                break;
                            }
                            case PlayerActions.ToggleRadialFor:
                            {
                                PutEnumOption2<RadialTriggerOptions>(node, gevent);
                                if (gevent.Option2 == (int)RadialTriggerOptions.Character)
                                {
                                    PutEnumValueText<StoryCharacters>(node, gevent);
                                }
                                else
                                {
                                    PutEnumValueText<Items>(node, gevent);
                                }
                                break;
                            }
                            case PlayerActions.GrabFromInventory:
                            {
                                PutEnumValueText<Items>(node, gevent);
                                break;
                            }
                        }
                        break;
                    }
                    case GameEvents.PlaySoundboardClip:
                    {
                        PutEnumOption<SupportedClipNames>(node, gevent);
                        break;
                    }
                    case GameEvents.Pose:
                    {
                        PutCharacter1(node, gevent);
                        //todo add charactergroups but i dont care for now
                        PutEnumOption<BoolCritera>(node, gevent);
                        if (Enum.GetNames<Females>().Contains(gevent.Character))
                        {
                            gevent.Option2 = (int)Gender.Female;
                        }
                        else
                        {
                            gevent.Option2 = (int)Gender.Male;
                        }

                        if (gevent.Option == (int)BoolCritera.True)
                        {
                            if (gevent.Option2 == (int)Gender.Female)
                            {
                                PutEnumValue<FemalePoses>(node, gevent);
                            }
                            else
                            {
                                PutEnumValue<MalePoses>(node, gevent);
                            }
                        }
                        break;
                    }
                    case GameEvents.Quest:
                    {
                        var box = GetComboBox();
                        List<Quest> Quests = [];
                        foreach (var item in Stories.Values)
                        {
                            foreach (var quest in item.Quests)
                            {
                                Quests.Add(quest);
                                box.Items.Add(quest.ID);
                            }
                        }
                        box.SelectedItem = gevent.Key;
                        box.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) =>
                        {
                            gevent.Key = box.SelectedItem.ToString()!;
                            gevent.Value = Quests.Find(n => n.ID == gevent.Key)?.Name ?? "#";
                        });
                        box.SelectedIndexChanged += (_, _) => ShowProperties(node);
                        PropertyInspector.Controls.Add(box, GeventPropertyCounter++, 1);

                        PutEnumOption<QuestActions>(node, gevent);
                        gevent.Character = string.Empty;
                        gevent.Character2 = string.Empty;
                        break;
                    }
                    case GameEvents.RandomizeIntValue:
                    {
                        PutCharacter1(node, gevent);
                        PutValueKey(node, gevent.Character, gevent);
                        PutNumberValue(gevent);

                        Label typeLabel = GetLabel("lower bound - upper bound");
                        PropertyInspector.Controls.Add(typeLabel, GeventPropertyCounter++, 1);

                        PutNumberValue2(gevent);
                        break;
                    }
                    case GameEvents.ResetReactionCooldown:
                    {
                        PutCharacter1(node, gevent);
                        PutCharacter2(node, gevent);
                        PutEnumOption<EventTypes>(node, gevent);
                        break;
                    }
                    case GameEvents.Roaming:
                    {
                        if (gevent.Option != (int)RoamingOptions.StopAllCurrentRoamingMotionTo)
                        {
                            PutCharacter1(node, gevent);
                        }
                        else
                        {
                            gevent.Character = string.Empty;
                        }
                        PutEnumOption<RoamingOptions>(node, gevent);
                        if (gevent.Option == (int)RoamingOptions.Allow)
                        {
                            PutEnumOption2<BoolCritera>(node, gevent);
                        }
                        else
                        {
                            if (gevent.Option == (int)RoamingOptions.StopAllCurrentRoamingMotionTo
                                || gevent.Option == (int)RoamingOptions.AllowLocation
                                || gevent.Option == (int)RoamingOptions.ChangeLocation
                                || gevent.Option == (int)RoamingOptions.ProhibitLocation)
                            {
                                PutEnumOption3<LocationTargetOption>(node, gevent);
                                switch ((LocationTargetOption)gevent.Option3)
                                {
                                    case LocationTargetOption.MoveTarget:
                                    {
                                        PutEnumValueText<MoveTargets>(node, gevent);
                                        break;
                                    }
                                    case LocationTargetOption.Character:
                                    {
                                        PutEnumValueText<Characters>(node, gevent);
                                        break;
                                    }
                                    case LocationTargetOption.Item:
                                    {
                                        PutEnumValueText<Items>(node, gevent);
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    }
                    case GameEvents.SendEvent:
                    {
                        PutEnumOption<SendEventOptions>(node, gevent);
                        if (gevent.Option != (int)SendEventOptions.GameOver)
                        {
                            PutCharacter1(node, gevent);
                            if (gevent.Option == (int)SendEventOptions.PickupLeftHand || gevent.Option == (int)SendEventOptions.PickupRightHand)
                            {
                                PutEnumKeyText<Items>(node, gevent);
                            }
                            else if (gevent.Option == (int)SendEventOptions.StartStripTease || gevent.Option == (int)SendEventOptions.StopStripTease)
                            {
                                PutEnumValue<Characters>(node, gevent);
                            }
                        }
                        break;
                    }
                    case GameEvents.SetPlayerPref:
                    {
                        PutEnumKeyText<PlayerPrefs>(node, gevent);
                        PutTextValue(gevent);
                        break;
                    }
                    case GameEvents.Social:
                    {
                        PutCharacter1(node, gevent);
                        PutEnumOption<SocialStatuses>(node, gevent);

                        switch ((SocialStatuses)gevent.Option)
                        {
                            case SocialStatuses.Mood:
                            case SocialStatuses.Drunk:
                            {
                                PutEnumOption2<Modification>(node, gevent);
                                PutNumberValue(gevent);
                                break;
                            }
                            case SocialStatuses.Loves:
                            case SocialStatuses.Likes:
                            case SocialStatuses.Scared:
                            {
                                PutCharacter2(node, gevent);
                                PutEnumOption2<Modification>(node, gevent);
                                PutNumberValue(gevent);
                                break;
                            }
                            case SocialStatuses.SendText:
                            case SocialStatuses.Offended:
                            {
                                break;
                            }
                            case SocialStatuses.TalkTo:
                            {
                                PutCharacter2(node, gevent);
                                break;
                            }
                            default:
                            {
                                gevent.Option = 0;
                                ShowProperties(node);
                                break;
                            }
                        }

                        break;
                    }
                    case GameEvents.State:
                    {
                        PutCharacter1(node, gevent);
                        PutEnumValue<InteractiveStates>(node, gevent);
                        PutEnumOption<AddRemoveActions>(node, gevent);
                        break;
                    }
                    case GameEvents.TriggerBGC:
                    {
                        PutCharacter1(node, gevent);
                        PutEnumOption<ImportanceSpecified>(node, gevent);

                        if (Stories[gevent.Character].BackgroundChatter.Count > 0)
                        {
                            if (!int.TryParse(gevent.Value, out int _))
                            {
                                gevent.Value = "0";
                            }

                            var box = GetComboBox();
                            foreach (var item in Stories[gevent.Character].BackgroundChatter)
                            {
                                box.Items.Add(item.Id);
                            }

                            box.SelectedItem = gevent.Value;
                            box.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Value = box.SelectedItem.ToString()!);
                            box.SelectedIndexChanged += (_, _) => ShowProperties(node);

                            var chatter = GetLabel(Stories[gevent.Character].BackgroundChatter[int.Parse(box.SelectedItem?.ToString() ?? "0")].Text);
                            PropertyInspector.Controls.Add(chatter, GeventPropertyCounter++, 1);
                        }
                        else
                        {
                            var noChatter = GetLabel("no background chatter");
                            PropertyInspector.Controls.Add(noChatter, GeventPropertyCounter++, 1);
                        }

                        break;
                    }
                    case GameEvents.Turn:
                    case GameEvents.TurnInstantly:
                    {
                        PutCharacter1(node, gevent);
                        PutEnumOption<TurnOptions>(node, gevent);

                        if (gevent.Option == 3)
                        {
                            PutEnumOption2<LocationTargetOption>(node, gevent);
                            switch ((LocationTargetOption)gevent.Option2)
                            {
                                case LocationTargetOption.MoveTarget:
                                {
                                    PutEnumValue<MoveTargets>(node, gevent);
                                    break;
                                }
                                case LocationTargetOption.Character:
                                {
                                    PutEnumValue<StoryCharacters>(node, gevent);
                                    break;
                                }
                                case LocationTargetOption.Item:
                                {
                                    PutEnumValue<Items>(node, gevent);
                                    break;
                                }
                                default:
                                    break;
                            }
                        }

                        break;
                    }
                    case GameEvents.UnlockAchievement:
                    {
                        //todo (achievements) but not really important anyways
                        break;
                    }
                    case GameEvents.WalkTo:
                    {
                        PutCharacter1(node, gevent);
                        PutEnumOption<WalkToTargetOptions>(node, gevent);
                        switch ((WalkToTargetOptions)gevent.Option)
                        {
                            case WalkToTargetOptions.MoveTarget:
                            {
                                PutEnumValueText<WalkToTargetOptions>(node, gevent);
                                break;
                            }
                            case WalkToTargetOptions.Character:
                            {
                                PutEnumValueText<Characters>(node, gevent);
                                break;
                            }
                            case WalkToTargetOptions.Item:
                            {
                                PutEnumValueText<Items>(node, gevent);
                                break;
                            }
                            case WalkToTargetOptions.Cancel:
                            {
                                var box = GetComboBox();
                                box.Items.AddRange(Enum.GetNames<WalkToTargetOptions>());
                                box.Items.AddRange(Enum.GetNames<Characters>());
                                box.Items.AddRange(Enum.GetNames<Items>());
                                box.SelectedItem = gevent.Value;
                                box.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Value = box.SelectedItem!.ToString()!);
                                PropertyInspector.Controls.Add(box, GeventPropertyCounter++, 1);
                                break;
                            }
                        }
                        break;
                    }
                    case GameEvents.WarpOverTime:
                    {
                        PutCharacter1(node, gevent);
                        PutEnumOption<LocationTargetOption>(node, gevent);
                        switch ((LocationTargetOption)gevent.Option)
                        {
                            case LocationTargetOption.MoveTarget:
                            {
                                PutEnumValueText<MoveTargets>(node, gevent);
                                break;
                            }
                            case LocationTargetOption.Character:
                            {
                                PutEnumValueText<Characters>(node, gevent);
                                break;
                            }
                            case LocationTargetOption.Item:
                            {
                                PutEnumValueText<Items>(node, gevent);
                                break;
                            }
                        }
                        PutNumberValue2(gevent);
                        break;
                    }
                    case GameEvents.WarpTo:
                    {
                        PutCharacter1(node, gevent);
                        PutEnumOption<LocationTargetOption>(node, gevent);
                        switch ((LocationTargetOption)gevent.Option)
                        {
                            case LocationTargetOption.MoveTarget:
                            {
                                PutEnumValueText<MoveTargets>(node, gevent);
                                break;
                            }
                            case LocationTargetOption.Character:
                            {
                                PutEnumValueText<Characters>(node, gevent);
                                break;
                            }
                            case LocationTargetOption.Item:
                            {
                                PutEnumValueText<Items>(node, gevent);
                                break;
                            }
                        }
                        break;
                    }
                    case GameEvents.None:
                    default:
                    {
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
                PropertyInspector.SetColumnSpan(customName, 10);

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

                        ComboBox zone = GetComboBox();
                        zone.Items.AddRange(Enum.GetNames(typeof(Zones)));
                        zone.SelectedItem = eventTrigger.Value;
                        zone.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.Value = (string)zone.SelectedItem!);
                        PropertyInspector.Controls.Add(zone);

                        break;
                    }
                    case EventTypes.ReachesTarget:
                    {
                        PutCharacter(node, eventTrigger);
                        PutStartCondition(node, eventTrigger);

                        ComboBox targetType = GetComboBox();
                        targetType.Items.AddRange(Enum.GetNames(typeof(LocationTargetOption)));
                        targetType.SelectedItem = eventTrigger.LocationTargetOption.ToString();
                        targetType.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.LocationTargetOption = Enum.Parse<LocationTargetOption>(targetType.SelectedItem!.ToString()!));
                        targetType.SelectedIndexChanged += (_, _) => ShowProperties(node);
                        PropertyInspector.Controls.Add(targetType);

                        switch (eventTrigger.LocationTargetOption)
                        {
                            case LocationTargetOption.MoveTarget:
                            {
                                ComboBox target = GetComboBox();
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

                        ComboBox zone = GetComboBox();
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

                        ComboBox cutscene = GetComboBox();
                        cutscene.Items.AddRange(Enum.GetNames(typeof(Cutscenes)));
                        cutscene.SelectedItem = eventTrigger.Value;
                        cutscene.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.Value = cutscene.SelectedItem!.ToString()!);
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

                ComboBox dialogue = GetComboBox();
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
                label3.MaximumSize = new Size(PropertyInspector.Width - 300, 50);
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

                checkBox = GetCheckbox("Global Response:", Stories[SelectedCharacter].GlobalResponses.Contains(response));
                checkBox.CheckedChanged += (_, args) =>
                {
                    if (checkBox.Checked && !Stories[SelectedCharacter].GlobalResponses.Contains(response))
                    {
                        Stories[SelectedCharacter].GlobalResponses.Add(response);
                    }
                    else
                    {
                        Stories[SelectedCharacter].GlobalResponses.Remove(response);
                    }
                };
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
                PropertyInspector.SetColumnSpan(text, 10);
                break;
            }
            case NodeType.Value:
            {
                Label label = GetLabel(node.FileName + "'s value:");
                PropertyInspector.Controls.Add(label);

                if (node.Data<string>() is not null)
                {
                    TextBox obj2 = new()
                    {
                        Dock = DockStyle.Fill,
                        TextAlign = HorizontalAlignment.Center,
                        ForeColor = Color.LightGray,
                        Text = node.Data<string>()!,
                        AutoSize = false,
                        Width = 200
                    };
                    obj2.TextChanged += (_, args) => node.RawData = obj2.Text;
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

        UpdatePropertyColoumnWidths();

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
        PropertyInspector.ColumnCount += 1;
        PropertyInspector.Controls.Add(new Panel() { Dock = DockStyle.Fill });
    }

    private void PutEnumValue<E>(Node node, GameEvent gevent) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!int.TryParse(gevent.Value, out int x))
        {
            gevent.Value = "0";
        }
        options.SelectedItem = (Enum.GetName((E)(object)int.Parse(gevent.Value)));
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Value = ((int)(object)Enum.Parse<E>(options.SelectedItem!.ToString()!)).ToString());
        options.SelectedIndexChanged += (_, _) => ShowProperties(node);
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumValueText<E>(Node node, GameEvent gevent) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!Enum.GetNames<E>().Contains(gevent.Value))
        {
            gevent.Value = options.Items[0]!.ToString()!;
        }
        options.SelectedItem = gevent.Value;
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Value = options.SelectedItem!.ToString()!);
        options.SelectedIndexChanged += (_, _) => ShowProperties(node);
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumValue2Text<E>(Node node, GameEvent gevent) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!Enum.GetNames<E>().Contains(gevent.Value2))
        {
            gevent.Value2 = options.Items[0]!.ToString()!;
        }
        options.SelectedItem = gevent.Value2;
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Value2 = options.SelectedItem!.ToString()!);
        options.SelectedIndexChanged += (_, _) => ShowProperties(node);
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumKeyText<E>(Node node, GameEvent gevent) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!Enum.GetNames<E>().Contains(gevent.Key))
        {
            gevent.Key = options.Items[0]!.ToString()!;
        }
        options.SelectedItem = gevent.Key;
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Key = options.SelectedItem!.ToString()!);
        options.SelectedIndexChanged += (_, _) => ShowProperties(node);
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumValue<E>(Node node, Criterion crit) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!int.TryParse(crit.Value, out int x))
        {
            crit.Value = "0";
        }
        options.SelectedItem = (Enum.GetName((E)(object)int.Parse(crit.Value)));
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => crit.Value = ((int)(object)Enum.Parse<E>(options.SelectedItem!.ToString()!)).ToString());
        options.SelectedIndexChanged += (_, _) => ShowProperties(node);
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumKey<E>(Node node, Criterion crit) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!int.TryParse(crit.Key, out int x))
        {
            crit.Key = "0";
        }
        options.SelectedItem = (Enum.GetName((E)(object)int.Parse(crit.Key)));
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => crit.Key = ((int)(object)Enum.Parse<E>(options.SelectedItem!.ToString()!)).ToString());
        options.SelectedIndexChanged += (_, _) => ShowProperties(node);
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumKey2<E>(Node node, Criterion crit) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!int.TryParse(crit.Key2, out int x))
        {
            crit.Key2 = "0";
        }
        options.SelectedItem = (Enum.GetName((E)(object)int.Parse(crit.Key2)));
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => crit.Key2 = ((int)(object)Enum.Parse<E>(options.SelectedItem!.ToString()!)).ToString());
        options.SelectedIndexChanged += (_, _) => ShowProperties(node);
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumValueText<E>(Node node, Criterion crit) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!Enum.GetNames<E>().Contains(crit.Value))
        {
            crit.Value = options.Items[0]!.ToString()!;
        }
        options.SelectedItem = crit.Value;
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => crit.Value = options.SelectedItem!.ToString()!);
        options.SelectedIndexChanged += (_, _) => ShowProperties(node);
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumOption<E>(Node node, GameEvent gevent) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!Enum.GetValues<E>().Contains((E)(object)gevent.Option))
        {
            gevent.Option = 0;
        }
        options.SelectedItem = Enum.GetName(typeof(E), gevent.Option)!;
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Option = (int)(object)Enum.Parse<E>(options.SelectedItem.ToString()!));
        options.SelectedIndexChanged += (_, _) => ShowProperties(node);
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumOption<E>(Node node, Criterion crit) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!Enum.GetValues<E>().Contains((E)(object)crit.Option))
        {
            crit.Option = 0;
        }
        options.SelectedItem = Enum.GetName(typeof(E), crit.Option)!;
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => crit.Option = (int)(object)Enum.Parse<E>(options.SelectedItem.ToString()!));
        options.SelectedIndexChanged += (_, _) => ShowProperties(node);
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumOption2<E>(Node node, GameEvent gevent) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!Enum.GetValues<E>().Contains((E)(object)gevent.Option2))
        {
            gevent.Option2 = 0;
        }
        options.SelectedItem = Enum.GetName(typeof(E), gevent.Option2)!;
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Option2 = (int)(object)Enum.Parse<E>(options.SelectedItem.ToString()!));
        options.SelectedIndexChanged += (_, _) => ShowProperties(node);
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumOption3<E>(Node node, GameEvent gevent) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!Enum.GetValues<E>().Contains((E)(object)gevent.Option3))
        {
            gevent.Option3 = 0;
        }
        options.SelectedItem = Enum.GetName(typeof(E), gevent.Option3)!;
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Option3 = (int)(object)Enum.Parse<E>(options.SelectedItem.ToString()!));
        options.SelectedIndexChanged += (_, _) => ShowProperties(node);
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumOption4<E>(Node node, GameEvent gevent) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!Enum.GetValues<E>().Contains((E)(object)gevent.Option4))
        {
            gevent.Option4 = 0;
        }
        options.SelectedItem = Enum.GetName(typeof(E), gevent.Option4)!;
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Option4 = (int)(object)Enum.Parse<E>(options.SelectedItem.ToString()!));
        options.SelectedIndexChanged += (_, _) => ShowProperties(node);
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutTextValue(GameEvent gevent)
    {
        TextBox textBox = new()
        {
            Dock = DockStyle.Fill,
            TextAlign = HorizontalAlignment.Left,
            Text = gevent.Value,
            Multiline = true,
        };
        textBox.TextChanged += (_, args) => gevent.Value = textBox.Text;
        PropertyInspector.Controls.Add(textBox, GeventPropertyCounter++, 1);
        GeventPropertyCounter += 2;
        PropertyInspector.SetColumnSpan(textBox, 3);
    }

    private void PutNumberValue2(GameEvent gevent)
    {
        _ = float.TryParse(gevent.Value2, out float result);
        var number = GetNumericUpDown(result);
        number.ValueChanged += (_, _) => gevent.Value2 = number.Value.ToString();
        PropertyInspector.Controls.Add(number, GeventPropertyCounter++, 1);
    }

    private void PutNumberValue(GameEvent gevent)
    {
        _ = float.TryParse(gevent.Value, out float result);
        var number = GetNumericUpDown(result);
        number.ValueChanged += (_, _) => gevent.Value = number.Value.ToString();
        PropertyInspector.Controls.Add(number, GeventPropertyCounter++, 1);
    }

    private void PutCharacter2(Node node, GameEvent gevent)
    {
        ComboBox character2 = GetComboBox();
        character2.Items.AddRange(Enum.GetNames(typeof(NoneCharacters)));
        character2.SelectedItem = gevent.Character2!;
        character2.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Character2 = character2.SelectedItem.ToString()!);
        character2.SelectedIndexChanged += (_, _) => ShowProperties(node);
        PropertyInspector.Controls.Add(character2, GeventPropertyCounter++, 1);
    }

    private void PutValueValue(Node node, string character, GameEvent gevent)
    {
        ComboBox value2 = GetComboBox();
        if (character == Player)
        {
            for (int i = 0; i < Story.PlayerValues!.Count; i++)
            {
                value2.Items.Add(Story.PlayerValues![i]);
            }
        }
        else
        {
            if (Stories.TryGetValue(character, out CharacterStory? _story))
            {
                for (int i = 0; i < _story.StoryValues!.Count; i++)
                {
                    value2.Items.Add(_story.StoryValues![i]);
                }
            }
        }
        value2.SelectedItem = gevent.Value!;
        value2.AddComboBoxHandler(node, nodes[SelectedCharacter], (sender, values) => gevent.Value = value2.SelectedItem.ToString()!);
        PropertyInspector.Controls.Add(value2, GeventPropertyCounter++, 1);
    }

    private void PutValueKey(Node node, string character, GameEvent gevent)
    {
        ComboBox value = GetComboBox();
        if (character == Player)
        {
            for (int i = 0; i < Story.PlayerValues!.Count; i++)
            {
                value.Items.Add(Story.PlayerValues![i]);
            }
        }
        else
        {
            if (Stories.TryGetValue(character, out CharacterStory? _story))
            {
                for (int i = 0; i < _story.StoryValues!.Count; i++)
                {
                    value.Items.Add(_story.StoryValues![i]);
                }
            }
        }
        value.SelectedItem = gevent.Key!;
        value.AddComboBoxHandler(node, nodes[SelectedCharacter], (sender, values) => gevent.Key = value.SelectedItem.ToString()!);
        PropertyInspector.Controls.Add(value, GeventPropertyCounter++, 1);
    }

    private void PutCharacter1(Node node, GameEvent gevent)
    {
        ComboBox character = GetComboBox();
        character.Items.AddRange(Enum.GetNames(typeof(Characters)));
        character.SelectedItem = gevent.Character!;
        character.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Character = character.SelectedItem.ToString()!);
        character.SelectedIndexChanged += (_, _) => ShowProperties(node);
        PropertyInspector.Controls.Add(character, GeventPropertyCounter++, 1);
    }

    private void UpdatePropertyColoumnWidths()
    {
        foreach (ColumnStyle coloumn in PropertyInspector.ColumnStyles)
        {
            coloumn.SizeType = SizeType.Absolute;
            coloumn.Width = (PropertyInspector.Width / (PropertyInspector.ColumnCount - 1));
        }
    }

    private static NumericUpDown GetNumericUpDown(float start)
    {
        NumericUpDown sortOrder = new()
        {
            //Dock = DockStyle.Fill,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
            Value = (decimal)start,
            Dock = DockStyle.Fill,
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
            Dock = DockStyle.Fill,
            ForeColor = Color.LightGray,
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
        };
    }

    private void PutItemValue(Node node, EventTrigger eventTrigger)
    {
        ComboBox item = GetComboBox();
        item.Items.AddRange(Enum.GetNames(typeof(Items)));
        item.SelectedItem = eventTrigger.Value;
        item.PerformLayout();
        item.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.Value = item.SelectedItem!.ToString()!);
        PropertyInspector.Controls.Add(item);
    }

    private void PutItemkey(Node node, EventTrigger eventTrigger)
    {
        ComboBox item = GetComboBox();
        item.Items.AddRange(Enum.GetNames(typeof(Items)));
        item.SelectedItem = eventTrigger.Key;
        item.PerformLayout();
        item.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.Key = item.SelectedItem!.ToString()!);
        PropertyInspector.Controls.Add(item);
    }

    private void PutCharacterValue(Node node, EventTrigger eventTrigger)
    {
        ComboBox character = GetComboBox();
        character.Items.AddRange(Enum.GetNames(typeof(AnybodyCharacters)));
        character.SelectedItem = eventTrigger.Value;
        character.PerformLayout();
        character.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.Value = character.SelectedItem!.ToString()!);
        PropertyInspector.Controls.Add(character);
    }

    private ComboBox PutCharacter(Node node, EventTrigger eventTrigger)
    {
        ComboBox character = GetComboBox();
        character.Items.AddRange(Enum.GetNames(typeof(AnybodyCharacters)));
        character.SelectedItem = eventTrigger.CharacterToReactTo;
        character.PerformLayout();
        character.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.CharacterToReactTo = character.SelectedItem!.ToString()!);
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
            criterion.Value = 0.ToString();
        }
        option.PerformLayout();
        option.ValueChanged += (_, _) => criterion.Value = option.Value.ToString();
        PropertyInspector.Controls.Add(option);
    }

    private void PutComparison(Node node, Criterion criterion)
    {
        ComboBox equ = GetComboBox();
        equ.Items.AddRange(Enum.GetNames(typeof(ComparisonEquations)));
        if (!Enum.TryParse<ComparisonEquations>(criterion.EquationValue!.ToString()!, out var res))
        {
            equ.SelectedIndex = 0;
            criterion.EquationValue = 0;
        }
        else
        {
            equ.SelectedItem = criterion.EquationValue!.ToString()!;
        }
        equ.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.EquationValue = Enum.Parse<ComparisonEquations>(equ.SelectedItem!.ToString()!));
        PropertyInspector.Controls.Add(equ);
    }

    private void PutZone(Node node, Criterion criterion)
    {
        ComboBox zone = GetComboBox();
        zone.Items.AddRange(Enum.GetNames(typeof(Zones)));
        zone.SelectedItem = criterion.Key;
        zone.SelectionLength = 0;
        zone.SelectionStart = 0;
        zone.PerformLayout();
        zone.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Key = (string)zone.SelectedItem!);
        PropertyInspector.Controls.Add(zone);
    }

    private void PutCompareType(Node node, Criterion criterion)
    {
        ComboBox compareType = GetComboBox();
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
        ComboBox compareType = GetComboBox();
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
        ComboBox compareType = GetComboBox();
        compareType.Items.AddRange(Enum.GetNames(typeof(Characters)));
        compareType.SelectedItem = criterion.Character2;
        compareType.SelectedText = string.Empty;
        compareType.SelectionLength = 0;
        compareType.SelectionStart = 0;
        compareType.PerformLayout();
        compareType.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Character2 = (string)compareType.SelectedItem!);
        PropertyInspector.Controls.Add(compareType);
    }

    private void PutBoolCriteria(Node node, Criterion criterion)
    {
        ComboBox boolValue = GetComboBox();
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
        ComboBox item = GetComboBox();
        item.Items.AddRange(Enum.GetNames<Items>());
        item.SelectedItem = criterion.Key!.Replace(" ", "");
        item.PerformLayout();
        item.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Key = item.SelectedItem!.ToString()!);
        PropertyInspector.Controls.Add(item);
    }

    private void PutEquals(Node node, Criterion criterion)
    {
        ComboBox equals = GetComboBox();
        equals.Items.AddRange(Enum.GetNames(typeof(EqualsValues)));
        equals.SelectedIndex = (int)criterion.EqualsValue!;
        equals.PerformLayout();
        equals.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.EqualsValue = (EqualsValues)equals.SelectedIndex);
        PropertyInspector.Controls.Add(equals);
    }

    private static ComboBox GetComboBox()
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

    private static Font GetScaledFont(Font f, float scale)
    {//see https://stackoverflow.com/questions/8850528/how-to-apply-graphics-scale-and-translate-to-the-textrenderer

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

    private static Rectangle GetScaledRect(Rectangle r, float scale)
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
            NodeLinker.Link(nodes[SelectedCharacter], nodeToLinkFrom, newNode);
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

            ShoveNodesToRight(newNode, ScreenPos);

            newNode.Position = ScreenPos;
        }
        else
        {
            ShoveNodesToRight(newNode, clickedNode.Position + new Size(scaleX, 0));
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
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
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
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
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
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
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
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
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
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
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
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.Dialogue:
            {
                int id = (Stories[character].Dialogues!.Count);
                newNode = new Node(id.ToString(), NodeType.Dialogue, string.Empty, nodes[character].Positions)
                {
                    RawData = new Dialogue() { ID = id },
                    FileName = character,
                };
                nodes[character].Add(newNode);
                Stories[character].Dialogues!.Add(newNode.Data<Dialogue>()!);

                if (clickedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
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
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
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
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
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
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.StoryItem:
            {
                string id = "item name";
                newNode = new Node(id, NodeType.StoryItem, string.Empty, nodes[character].Positions)
                {
                    RawData = new InteractiveitemBehaviour() { ItemName = id },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
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
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
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
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
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
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
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
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
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
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.Value:
            {
                string id = "ValueName";
                newNode = new Node(id, NodeType.Value, string.Empty, nodes[character].Positions)
                {
                    RawData = id,
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
                }
                if (character == Player)
                {
                    Story.PlayerValues.Add(newNode.Data<string>()!);
                }
                else
                {
                    Stories[character].StoryValues.Add(newNode.Data<string>()!);
                }
                break;
            }
            case SpawnableNodeType.UseWith:
            {
                string id = "use on item:";
                //todo add auto linking to item name in ID
                newNode = new Node(id, NodeType.UseWith, string.Empty, nodes[character].Positions)
                {
                    RawData = new UseWith() { ItemName = id },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (clickedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], clickedNode, newNode);
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

    private void PullChildsClose(object sender, EventArgs e)
    {
        //clickednode is set when this is called
        var childs = nodes[SelectedCharacter].Childs(clickedNode);

        for (int i = 0; i < childs.Count; i++)
        {
            var newPos = clickedNode.Position + new SizeF(scaleX, (i - (childs.Count / 2)) * scaleY);
            ShoveNodesToRight(childs[i], newPos);

            childs[i].Position = newPos;
        }

        CenterOnNode(clickedNode, 0.3f);
        Graph.Invalidate();
    }

    private void PullParentsClose(object sender, EventArgs e)
    {
        //clickednode is set when this is called
        var parents = nodes[SelectedCharacter].Parents(clickedNode);

        for (int i = 0; i < parents.Count; i++)
        {
            var newPos = clickedNode.Position - new SizeF(scaleX, (i - (parents.Count / 2)) * scaleY);
            ShoveNodesToLeft(parents[i], newPos);
            parents[i].Position = newPos;
        }

        CenterOnNode(clickedNode, 0.3f);
        Graph.Invalidate();
    }

    private void ShoveNodesToLeft(Node nodeToPlace, PointF newPos)
    {
        var maybeNodePos = newPos;
        Node maybeThere;
        Node maybeNewSpot;

        while ((maybeNewSpot = GetNodeAtPoint(maybeNodePos + NodeCenter)) != Node.NullNode && maybeNewSpot != nodeToPlace)
        {
            maybeNodePos = newPos;
            while ((maybeThere = GetNodeAtPoint(maybeNodePos + NodeCenter)) != Node.NullNode && maybeThere != nodeToPlace)
            {
                maybeNodePos -= new SizeF(scaleX, 0);
                if (GetNodeAtPoint(maybeNodePos + NodeCenter) == Node.NullNode)
                {
                    maybeThere.Position = maybeNodePos;
                    break;
                }
            }
        }
    }

    private void ShoveNodesToRight(Node nodeToPlace, PointF newPos)
    {
        var maybeNodePos = newPos;
        Node maybeThere;
        Node maybeNewSpot;

        while ((maybeNewSpot = GetNodeAtPoint(maybeNodePos + NodeCenter)) != Node.NullNode && maybeNewSpot != nodeToPlace)
        {
            maybeNodePos = newPos;
            while ((maybeThere = GetNodeAtPoint(maybeNodePos + NodeCenter)) != Node.NullNode && maybeThere != nodeToPlace)
            {
                maybeNodePos += new SizeF(scaleX, 0);
                if (GetNodeAtPoint(maybeNodePos + NodeCenter) == Node.NullNode)
                {
                    maybeThere.Position = maybeNodePos;
                    break;
                }
            }
        }
    }

    private void SortConnected(object sender, EventArgs e)
    {
        if (clickedNode == Node.NullNode)
        {
            return;
        }
        //clickednode is set when this is called
        visited.Clear();
        var intX = (int)(clickedNode.Position.X / scaleX);
        var intY = (int)(clickedNode.Position.Y / scaleY);
        maxYperX.ExtendToIndex(intX, intY);

        for (int i = 0; i < maxYperX.Count; i++)
        {
            maxYperX[i] = intY;
        }
        SetStartPosForConnected(intX, nodes[SelectedCharacter], clickedNode);

        Graph.Invalidate();
    }

    private void SortSelected(object sender, EventArgs e)
    {
        if (selected.Count == 0)
        {
            return;
        }
        SetStartPositionsForNodesInList((int)(selected[0].Position.X / scaleX), (int)(selected[0].Position.Y / scaleY), nodes[SelectedCharacter], selected, true);

        Graph.Invalidate();
    }

    private void SortSelectedConnected(object sender, EventArgs e)
    {
        if (selected.Count == 0)
        {
            return;
        }
        SetStartPositionsForNodesInList((int)(selected[0].Position.X / scaleX), (int)(selected[0].Position.Y / scaleY), nodes[SelectedCharacter], selected);

        Graph.Invalidate();
    }
}
