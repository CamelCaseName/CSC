using CSC.Components;
using CSC.Direct2D;
using CSC.Glue;
using CSC.Nodestuff;
using CSC.Search;
using CSC.StoryItems;
using Silk.NET.Core.Win32Extras;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Numerics;
using System.Text.Json;
using static CSC.StoryItems.StoryEnums;
using Enum = System.Enum;
using Rectangle = System.Drawing.Rectangle;

namespace CSC;

public partial class Main : Form
{
    private bool adding;
    private bool CurrentlyInPan = false;
    private bool IsCtrlPressed;
    private bool isFirstLoad = true;
    private bool IsShiftPressed;
    private bool selecting;
    private bool subtracting;
    private const string Anybody = "Anybody";
    private int GeventPropertyCounter = 0;
    internal float AfterZoomNodeX;
    internal float AfterZoomNodeY;
    internal float BeforeZoomNodeX;
    internal float BeforeZoomNodeY;
    internal float StartPanOffsetX = 0f;
    internal float StartPanOffsetY = 0f;
    private Node highlightNode;
    private Node movedNode;
    private Node nodeToLinkFrom;
    private Point oldMousePosBeforeSpawnWindow = Point.Empty;
    private Point startSelectingMousePos = Point.Empty;
    private readonly int scaleX = (int)(NodeSizeX * 1.5f);
    private readonly int scaleY = (int)(NodeSizeY * 1.5f);
    private readonly List<int> maxYperX = [];
    public static readonly List<Node> selected = [];
    private readonly List<Node> visited = [];
    private readonly List<SizeF> SelectedNodeOffsets = [];
    private static readonly SizeF NodeCenter = new(NodeSizeX / 2, NodeSizeY / 2);
    private static readonly SizeF CircleSize = new(17, 17);
    private RectangleF adjustedMouseClipBounds;
    private SearchDialog searchWindow = null!;
    private SizeF OffsetFromDragClick = SizeF.Empty;
    private static bool needsSaving;
    private static MainStory Story = null!;
    private static readonly Dictionary<string, NodeStore> nodes = [];
    private static string _selectedCharacter = NoCharacter;
    private string StoryFolder = string.Empty;
    public bool MovingChild = false;
    public const int NodeSizeX = 200;
    public const int NodeSizeY = 50;
    public const string HousePartyVersion = "1.4.2";
    public const string NoCharacter = "None";
    public const string Player = "Player";
    public static readonly Dictionary<string, CharacterStory> Stories = [];
    private static readonly Dictionary<string, float> OffsetX = [];
    private static readonly Dictionary<string, float> OffsetY = [];
    private static readonly Dictionary<string, float> Scaling = [];
    //do all node related rendering in direct2d but keep the simple stuff in gdi+ because why change
    private readonly D2DRenderer render;
    //gdi+ resources
    private readonly SolidBrush SelectionFill;
    private readonly Pen SelectionEdge;
    private readonly Pen nodeToLinkPen;
    private CachedBitmap? oldGraph;
    private static bool positionsChanged = false;
    private static readonly List<string> Files = [Player];
    public static bool PositionsChanged => positionsChanged;

    internal static float Scalee => Scaling[SelectedCharacter];
    internal static Vector2 Offset => new(OffsetX[SelectedCharacter], OffsetY[SelectedCharacter]);

    private static Main Instance { get; set; } = null!;

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
            if (value == StoryName)
            {
                SelectedFile = 0;
            }
            else
            {
                SelectedFile = Files.IndexOf(_selectedCharacter);
            }
            if (SelectedFile == -1)
            {
                Debugger.Break();
            }
            positionsChanged = true;
        }
    }

    public static int SelectedFile;

    public int RightClickFrameCounter { get; private set; } = 0;

    public int LeftClickFrameCounter { get; private set; } = 0;

    public static bool NeedsSaving { get => needsSaving; set => needsSaving = value; }
    private Node SelectedNode = null!;

    public static Node Selected => Instance.SelectedNode;
    public static Node Highlight => Instance.highlightNode;
    public static Node LinkFrom => Instance.nodeToLinkFrom;

    //todo we need to pause updating the search during a typing streak and cumulatively update after its done
    //todo filter/hide node types

    //todo add story node cache on disk
    //todo add story search tree cache on disk
    //todo add info when trying to link incompatible notes
    //todo unify all node creation so its always the same
    //todo add grouping
    //todo add comments

    public Main()
    {
        Instance = this;
        InitializeComponent();
        StoryTree.ExpandAll();
        Application.AddMessageFilter(new MouseMessageFilter());
        MouseMessageFilter.MouseMove += HandleMouseEvents;

        nodeToLinkPen = new Pen(Brushes.LightCyan, 3)
        {
            EndCap = LineCap.Triangle,
            StartCap = LineCap.Round
        };

        SelectionFill = new SolidBrush(Color.FromArgb(80, Color.Gray));
        SelectionEdge = new Pen(Color.LightGray, 1f);

        NodeSpawnBox.Items.AddRange(Enum.GetNames<SpawnableNodeType>());

        PropertyInspector.SizeChanged += (_, _) => UpdatePropertyColoumnWidths();

        nodes.Add(NoCharacter, new(NoCharacter));
        Scaling.Add(Main.NoCharacter, 0.3f);
        OffsetX.Add(Main.NoCharacter, 0);
        OffsetY.Add(Main.NoCharacter, 0);

        nodes.Add(Player, new(Player));
        Scaling.Add(Main.Player, 0.3f);
        OffsetX.Add(Main.Player, 0);
        OffsetY.Add(Main.Player, 0);

        SelectedNode = Node.NullNode;
        highlightNode = Node.NullNode;
        movedNode = Node.NullNode;
        nodeToLinkFrom = Node.NullNode;

        render = new();
        Graph.Invalidate();
    }

    private void OnFormClosing(object? sender, FormClosingEventArgs? e)
    {
        if (Story is null || Stories.Count <= 0 || !NeedsSaving)
        {
            if (MessageBox.Show("Are you sure, you want to close the Program?", "Close the Program?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                e!.Cancel = true;
            }
            if (!e!.Cancel)
            {
                render.Release();
            }
            return;
        }

        var res = new SaveOnCloseDialog().ShowDialog();
        //yes == save all
        //ok == save files
        //continue == save positions
        //cancel == dont save any

        switch (res)
        {
            case DialogResult.Yes:
                ExportAllFiles();
                SafeSavePositions();
                break;
            case DialogResult.OK:
                ExportAllFiles();
                break;
            case DialogResult.Continue:
                SafeSavePositions();
                break;
            case DialogResult.Cancel:
                e!.Cancel = true;
                break;
                //rest do nothing#
        }

        if (!e!.Cancel)
        {
            render.Release();
        }
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

            if (SelectedNode == Node.NullNode && PropertyInspector.Controls.Count > 1)
            {
                PropertyInspector.Controls.Clear();
            }
        }
        else if (e.KeyData == Keys.Delete && Graph.Focused)
        {
            TryDeleteNode();
        }
        else if (e.KeyData == Keys.F && IsCtrlPressed)
        {
            StartSearch();
        }
    }

    public void StartSearch()
    {
        searchWindow = new SearchDialog(nodes);
        searchWindow.Show();
    }

    public static int FileIndex(string file)
    {
        return Files.IndexOf(file);
    }

    private void TryDeleteNode()
    {
        Node removedNode = Node.NullNode;
        if (highlightNode != Node.NullNode)
        {
            removedNode = highlightNode;
        }
        else if (SelectedNode != Node.NullNode)
        {
            removedNode = SelectedNode;
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
            PropertyInspector.Controls.Clear();
            if (nodeToLinkFrom == removedNode)
            {
                nodeToLinkFrom = Node.NullNode;
            }
            if (SelectedNode == removedNode)
            {
                SelectedNode = Node.NullNode;
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
            removedNode.RemoveFromSorting(SelectedFile);

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
            if (SelectedNode != Node.NullNode)
            {
                switch (SelectedNode.Type)
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
                    case NodeType.Social:
                    case NodeType.State:
                    case NodeType.Property:
                    case NodeType.Cutscene:
                    case NodeType.Door:
                    case NodeType.CriteriaGroup:
                    {
                        return [NodeType.GameEvent, NodeType.Criterion];
                    }
                    case NodeType.Quest:
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
            if (SelectedNode != Node.NullNode)
            {
                switch (SelectedNode.Type)
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
                    case NodeType.Quest:
                    {
                        return [NodeType.GameEvent, NodeType.Criterion, NodeType.Quest];
                    }
                    //event criterion
                    case NodeType.CharacterGroup:
                    case NodeType.AlternateText:
                    case NodeType.EventTrigger:
                    case NodeType.Clothing:
                    case NodeType.Personality:
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
            foreach (var dropdown in PropertyInspector.Controls)
            {
                if (dropdown is ComboBox box)
                {
                    if (box.DroppedDown && box.Focused)
                    {
                        return;
                    }
                }
            }
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

    private void SetPanOffset(Point location)
    {
        StartPanOffsetX = location.X;
        StartPanOffsetY = location.Y;
    }

    private void OnNone(PointF graphPos)
    {
        UpdateLeftRightClickStates(graphPos);
        EndPan();

        bool doHighlight = true;
        foreach (var dropdown in PropertyInspector.Controls)
        {
            if (dropdown is ComboBox box)
            {
                if (box.DroppedDown && box.Focused)
                {
                    doHighlight = false;
                    break;
                }
            }
        }
        if (doHighlight)
        {
            UpdateHighlightNode(graphPos);
        }
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
        else if (!selecting && LeftClickFrameCounter == 1)
        {
            _ = UpdateClickedNode(graphPos);
            selected.Clear();
            SelectedNodeOffsets.Clear();
            LeftClickFrameCounter = 0;
            Graph.Invalidate();
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
        if (LeftClickFrameCounter == 1)
        {
            startSelectingMousePos = new((int)screenPos.X, (int)screenPos.Y);
        }
        if (LeftClickFrameCounter > 2)
        {
            if (!selecting)
            {
                StartSelecting();
            }
        }
        //only pull focus if we are not in a dropdown

        foreach (var dropdown in PropertyInspector.Controls)
        {
            if (dropdown is ComboBox box)
            {
                if (box.DroppedDown && box.Focused)
                {
                    return;
                }
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

    private void StartSelecting()
    {
        selecting = true;
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
        positionsChanged = true;
    }

    public static void ClearNodePos(Node node, string file)
    {
        nodes[file].Positions.ClearNode(node);
        positionsChanged = true;
    }

    public static void ClearNodePos(Node node, int file)
    {
        nodes[Files[file]].Positions.ClearNode(node);
        positionsChanged = true;
    }

    public static void ClearAllNodePos(Node node)
    {
        foreach (var pos in nodes.Values)
        {
            pos.Positions.ClearNode(node);
        }
    }

    public static void SetNodePos(Node node, int file)
    {
        nodes[Files[file]].Positions.SetNode(node);
    }

    private void UpdateDoubleClickTransition(PointF ScreenPos)
    {
        var node = UpdateClickedNode(ScreenPos);
        if (node != Node.NullNode)
        {
            if (node.FileName != SelectedCharacter)
            {
                SelectFile(node.FileName);
                SelectedCharacter = node.FileName;
                CenterAndSelectNode(node);
            }
        }
    }

    public static void UpdateNode(Node node)
    {
        if (node == Instance.SelectedNode)
        {
            var focusedC = Instance.PropertyInspector.FindFocusedControl();

            if (focusedC is ComboBox)
            {
                Instance.ShowProperties(node);
            }
            else if (focusedC is NumericUpDown)
            {
                Instance.ShowProperties(node);
            }
        }
        if (SearchTrie.Initialized)
        {
            SearchTrie.AddNode(node);
        }
    }

    public static void PreUpdateNode(Node node)
    {
        if (SearchTrie.Initialized)
        {
            SearchTrie.RemoveNode(node);
        }
    }

    public static void SetSearchWindowTitle(string text)
    {
        if (Instance is not null && Instance.searchWindow is not null)
        {
            Instance.Invoke(() =>
            {
                Instance.searchWindow.Text = text;
                Instance.searchWindow.Cursor = Cursors.Default;
            });
        }
    }

    public static void SelectFile(string file)
    {
        if (Instance is null)
        {
            return;
        }
        Instance.Invoke(() =>
        {
            if (file == StoryName || file == Player || file == Anybody)
            {
                Instance.StoryTree.SelectedNode = Instance.StoryTree.Nodes[0].FirstNode;
            }
            else
            {
                foreach (TreeNode treeNode in Instance.StoryTree.Nodes[0].LastNode!.Nodes)
                {
                    if (treeNode.Text == file)
                    {
                        Instance.StoryTree.SelectedNode = treeNode;
                        break;
                    }
                }
            }
        });
    }

    public static void CenterAndSelectNode(Node node, float newScale = 1.5f)
    {
        Scaling[SelectedCharacter] = newScale;
        var clipWidth = Instance.Graph.Size.Width / Scaling[SelectedCharacter];
        var clipHeight = Instance.Graph.Size.Height / Scaling[SelectedCharacter];
        float x = node.Position.X - (clipWidth / 2) + (node.Size.Width / 2);
        float y = node.Position.Y - (clipHeight / 2) + (node.Size.Height / 2);
        OffsetX[SelectedCharacter] = x;
        OffsetY[SelectedCharacter] = y;
        Instance.SelectedNode = node;
        Instance.Graph.Invalidate();
        Instance.ShowProperties(node);
    }

    public static void ScreenToGraph(float screenX, float screenY, out float graphX, out float graphY)
    {
        graphX = screenX / Scaling[SelectedCharacter] + OffsetX[SelectedCharacter];
        graphY = screenY / Scaling[SelectedCharacter] + OffsetY[SelectedCharacter];
    }

    public static void GraphToScreen(float graphX, float graphY, out float screenX, out float screenY)
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

        res = NewFileDialog.ShowDropdownBox(ref newChar, [.. list], "Character to add:", "Select Character");

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
            var res = NewFileDialog.ShowTextBox(ref newStoryName, "Name for new Story:", "Story Title");

            if (res != DialogResult.OK)
            {
                return;
            }

            Files.Add(Player);
            AddStory(newStoryName);
            SetupStartPositions();
        }
    }

    private void AddCharacterStory(string newCharacterName)
    {
        Files.Add(newCharacterName);
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
        Files.Clear();
        Stories.Clear();
        NodeLinker.ClearLinkCache();
        SearchTrie.Reset();
        StoryName = NoCharacter;
        SelectedCharacter = NoCharacter;

        nodes.Clear();
        nodes.Add(NoCharacter, new(NoCharacter));
        nodes.Add(Player, new(Player));

        StoryTreeReset();
        Graph.Invalidate();
        PropertyInspector.Controls.Clear();
        isFirstLoad = true;
        NeedsSaving = false;
    }

    private void StoryTreeReset()
    {
        StoryTree.Nodes.Clear();
        TreeNode treeNode1 = new("Characters");
        TreeNode treeNode2 = new("Story Root", [treeNode1]);
        StoryTree.Nodes.AddRange([treeNode2]);
    }

    private static void DrawLinkTo(Graphics g, Node parent, Node child, Pen pen, PointF start = default, PointF end = default)
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
    }

    public static PointF GetStartHeightFromThird(Node parent, PointF start, int third)
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

    public static int GetEdgeStartHeight(Node parent, Node child, int third)
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

    public static void GetLinkCircleRects(Node node, out RectangleF leftRect, out RectangleF rightRect)
    {
        leftRect = new RectangleF(node.Position + new SizeF(-CircleSize.Width / 2, (node.Size.Height / 2) - CircleSize.Width / 2), CircleSize);
        rightRect = new RectangleF(node.Position + new SizeF(node.Size.Width - CircleSize.Width / 2, (node.Size.Height / 2) - CircleSize.Width / 2), CircleSize);
        if (node.FileName != SelectedCharacter)
        {
            if (node == Instance.SelectedNode)
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
        else if (node == Instance.SelectedNode)
        {
            leftRect.Location -= new SizeF(15 / 2, 0);
            rightRect.Location += new SizeF(15 / 2, 0);
        }
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

    private void Graph_Paint(object sender, PaintEventArgs e)
    {
        var g = e.Graphics;

        if (oldGraph is null || (nodeToLinkFrom == Node.NullNode && !selecting))
        {
            g.ToLowQuality();
            //update canvas transforms
            var offset = -Main.Offset * Main.Scalee;
            g.TranslateTransform(offset.X, offset.Y);
            g.ScaleTransform(Main.Scalee, Main.Scalee);
            if (Main.Scalee < 0)
            {
                Debugger.Break();
            }
            adjustedMouseClipBounds = new(OffsetX[SelectedCharacter],
                                          OffsetY[SelectedCharacter],
                                          g.VisibleClipBounds.Width,
                                          g.VisibleClipBounds.Height);
            render.Paint(g, nodes[SelectedCharacter], Graph.ClientRectangle);
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
        //teh renderer has already made new objects by now, so we no longer need to update
        positionsChanged = false;
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

        DrawLinkTo(g, nodeToLinkFrom, Node.NullNode, nodeToLinkPen, start: screenPos, end: pos);
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
        List<string> fileList = [.. Directory.GetFiles(StoryFolder)];
        fileList.Sort();

        Files.Add(Player);
        foreach (var file in fileList)
        {
            if (Path.GetExtension(file) == ".story")
            {
                continue;
            }

            //story is supposed to be 0 hence its already in
            Files.Add(Path.GetFileNameWithoutExtension(file));
        }

        foreach (var file in fileList)
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

        isFirstLoad = false;

        StoryTree.SelectedNode = StoryTree.Nodes[0].Nodes[1].Nodes[Stories.Count - 1];
        NeedsSaving = false;
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
                if (isFirstLoad)
                {
                    SetupStartPositions();
                }
                SavePositions();
            }
            Cursor = Cursors.Default;
        }

        NeedsSaving = false;
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
            //no idea why necessary but easier than looking for the weird ass cause
            nodes[fileStore].Positions.Clear();
            foreach (var node in nodes[fileStore].Nodes)
            {
                if (positions[fileStore].TryGetValue(new NodeID(fileStore, node.Type, node.ID, node.Text), out var point))
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
                    Debug.WriteLine(fileStore + "|" + node.FileName + "|" + node.Type + ":" + node.ID);
                }
                SetStartPositionsForNodesInList(10, 0, nodes[fileStore], notSet, false);
                hadNodesNewlySet = true;
            }
            CenterAndSelectNode(nodes[fileStore].Nodes.First(), 0.8f);
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
                positions[nodeStore].Add(new NodeID(nodeStore, node.Type, node.ID, node.Text), node.Position);
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
                UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Skip,
                IncludeFields = false
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
        NodeStore tempStore = new(FileName);
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
            StoryTree.Nodes[0].LastNode!.Nodes.Add(new TreeNode(FileName));
            StoryTree.SelectedNode = StoryTree.Nodes[0].LastNode!.LastNode;
        }
        StoryTree.ExpandAll();
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
            nodeStore.Positions.Clear();

            //no idea why we have to clear it or where the wrong ones come from....
            SetStartPositionsForNodesInList(100, 1, nodeStore, [.. nodeStore.Nodes]);

            CenterAndSelectNode(nodes[store].Nodes.First(), 0.8f);
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

        nodeList.Sort(new NodeComparers(nodeStore));
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

            //selectedcharacter is set correctly here
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
            parents.Sort(new NodeComparers(nodeStore));

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

        NeedsSaving = false;
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
            else if (SelectedNode != node)
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

        if (SelectedNode != Node.NullNode && node == Node.NullNode && PropertyInspector.Focused)
        {
            return SelectedNode;
        }

        SelectedNode = node;
        return node;
    }

    private void SpawnContextMenu(PointF graphPos)
    {
        GraphToScreen(graphPos.X, graphPos.Y, out float screenPosX, out float screenPosY);
        Point ScreenPos = new((int)screenPosX, (int)screenPosY);

        NodeContext.Items.Clear();

        var list = GetSpawnableNodeTypes();

        if (SelectedNode != Node.NullNode)
        {
            NodeContext.Items.Add(SortConnectedMenu);
            NodeContext.Items.Add(PullChildsMenu);
            NodeContext.Items.Add(PullParentsMenu);
            NodeContext.Items.Add(Seperator1);

            if (SelectedNode.DupedFileNames.Any())
            {
                foreach (var file in SelectedNode.DupedFileNames)
                {
                    if (file == SelectedCharacter)
                    {
                        continue;
                    }
                    var button = new ToolStripMenuItem("See reference in " + file, null, onClick: (_, _) =>
                    {
                        SelectFile(file);
                        CenterAndSelectNode(SelectedNode);
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

                if (SelectedNode.FileName != SelectedCharacter)
                {
                    var button = new ToolStripMenuItem("See reference in " + SelectedNode.FileName, null, onClick: (_, _) =>
                    {
                        SelectFile(SelectedNode.FileName);
                        CenterAndSelectNode(SelectedNode);
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

                NodeContext.Items.Add(Seperator2);
            }
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
                PropertyInspector.ColumnCount = 9;
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
                        //todo (charactergroup) this is quite involved it seems so i dont want to do it for now
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
                            if (!Stories.ContainsKey(criterion.Character))
                            {
                                criterion.Character = Stories.First().Key;
                            }
                            valueChar1.Items.AddRange([.. Stories[criterion.Character!].StoryValues!]);
                        }
                        valueChar1.SelectedItem = criterion.Key!;
                        valueChar1.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Key = valueChar1.SelectedItem!.ToString()!);
                        PropertyInspector.Controls.Add(valueChar1);

                        ComboBox formula = GetComboBox();
                        formula.Items.AddRange(Enum.GetNames<ValueSpecificFormulas>());
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
                            if (!Stories.ContainsKey(criterion.Character2))
                            {
                                criterion.Character2 = Stories.First().Key;
                            }
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
                        if (!Stories.ContainsKey(criterion.Character))
                        {
                            criterion.Character = Stories.First().Key;
                        }
                        for (int i = 0; i < Stories[criterion.Character!].Dialogues!.Count; i++)
                        {
                            dialogue.Items.Add(Stories[criterion.Character!].Dialogues![i].ID.ToString());
                        }
                        dialogue.SelectedItem = criterion.Value!;
                        dialogue.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Value = dialogue.SelectedItem!.ToString()!);
                        PropertyInspector.Controls.Add(dialogue);

                        ComboBox formula = GetComboBox();
                        formula.Items.AddRange(Enum.GetNames<DialogueStatuses>());
                        formula.SelectedItem = criterion.DialogueStatus.ToString();
                        formula.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.DialogueStatus = Enum.Parse<DialogueStatuses>(formula.SelectedItem!.ToString()!)!);
                        PropertyInspector.Controls.Add(formula);
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
                        obj1.TextChanged += (_, _) => node.ID = $"{criterion.Character}{criterion.CompareType}{criterion.Key}{criterion.Value}";
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

                        PutEquation(node, criterion);
                        PutNumericOption(node, criterion);

                        break;
                    }
                    case CompareTypes.Door:
                    {
                        PutCompareType(node, criterion);
                        PutEnumKey<Doors>(node, criterion);

                        ComboBox formula = GetComboBox();
                        formula.Items.AddRange(Enum.GetNames<DoorOptionValues>());
                        formula.SelectedItem = criterion.DoorOptions.ToString();
                        formula.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.DoorOptions = Enum.Parse<DoorOptionValues>(formula.SelectedItem!.ToString()!)!);
                        PropertyInspector.Controls.Add(formula);
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
                    case CompareTypes.Vision:
                    case CompareTypes.SameZoneAs:
                    case CompareTypes.IsInFrontOf:
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
                        state.Items.AddRange(Enum.GetNames<ItemComparisonTypes>());
                        state.SelectedItem = criterion.ItemComparison.ToString();
                        state.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.ItemComparison = Enum.Parse<ItemComparisonTypes>(state.SelectedItem!.ToString()!));
                        PropertyInspector.Controls.Add(state);

                        break;
                    }
                    case CompareTypes.IsBeingSpokenTo:
                    case CompareTypes.IsInHouse:
                    case CompareTypes.IsCharacterEnabled:
                    case CompareTypes.MetByPlayer:
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
                    case CompareTypes.IsGameUncensored:
                    case CompareTypes.IsNewGame:
                    case CompareTypes.ScreenFadeVisible:
                    case CompareTypes.UseLegacyIntimacy:
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
                        box.Items.AddRange(Enum.GetNames<ItemFromItemGroupComparisonTypes>());
                        box.SelectedItem = criterion.ItemFromItemGroupComparison.ToString();
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
                                //this is broken in the original CSC??
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
                    case CompareTypes.Personality:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);
                        PutEnumKey<PersonalityTraits>(node, criterion);
                        PutEquation(node, criterion);
                        PutNumericValue(node, criterion);
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
                        state.Items.AddRange(Enum.GetNames<PlayerInventoryOptions>());
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
                        PutEquation(node, criterion);
                        PutTextValue(node, criterion);

                        break;
                    }
                    case CompareTypes.Posing:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);

                        ComboBox option = GetComboBox();
                        option.Items.AddRange(Enum.GetNames<PoseOptions>());
                        option.SelectedItem = criterion.PoseOption.ToString();
                        option.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.PoseOption = Enum.Parse<PoseOptions>(option.SelectedItem!.ToString()!));
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
                        List<Tuple<string, Quest>> Quests = [];
                        foreach (var item in Stories.Values)
                        {
                            foreach (var questData in item.Quests)
                            {
                                Quests.Add(new(item.CharacterName, questData));
                                quest.Items.Add(item.CharacterName + "|" + questData.Name);
                            }
                        }
                        quest.SelectedItem = (Quests.Find(n => n.Item2.Name == criterion.Key2)?.Item1 ?? node.FileName) + "|" + criterion.Key2;
                        quest.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) =>
                        {
                            criterion.Key2 = quest.SelectedItem.ToString()!.Split("|")[1];
                            criterion.Character = Quests.Find(n => n.Item2.Name == criterion.Key2)?.Item1 ?? node.FileName;
                            criterion.Key = Quests.Find(n => n.Item2.Name == criterion.Key2)?.Item2.ID ?? "#";
                        });
                        PropertyInspector.Controls.Add(quest);
                        PropertyInspector.SetColumnSpan(quest, 3);

                        PutEquals(node, criterion);
                        PutEnumValue<QuestStatus>(node, criterion);
                        break;
                    }
                    case CompareTypes.Social:
                    {
                        PutCompareType(node, criterion);
                        PutCharacter1(node, criterion);

                        ComboBox social = GetComboBox();
                        social.Items.AddRange(Enum.GetNames<SocialStatuses>());
                        social.SelectedItem = criterion.SocialStatus!.ToString();
                        social.AddComboBoxHandler(node, nodes[SelectedCharacter], (sender, values) => criterion.SocialStatus = Enum.Parse<SocialStatuses>(social.SelectedItem!.ToString()!));
                        PropertyInspector.Controls.Add(social);

                        PutEquation(node, criterion);
                        PutNumericValue(node, criterion);
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
                            if (!Stories.ContainsKey(criterion.Character))
                            {
                                criterion.Character = Stories.First().Key;
                            }
                            for (int i = 0; i < Stories[criterion.Character!].StoryValues!.Count; i++)
                            {
                                value.Items.Add(Stories[criterion.Character!].StoryValues![i]);
                            }
                        }
                        value.SelectedItem = criterion.Key!;
                        value.AddComboBoxHandler(node, nodes[SelectedCharacter], (sender, values) => criterion.Key = value.SelectedItem!.ToString()!);
                        PropertyInspector.Controls.Add(value);

                        PutEquation(node, criterion);

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
                talkingTo.Items.AddRange(Enum.GetNames<AnybodyCharacters>());
                talkingTo.SelectedItem = dialogue.SpeakingTo;
                talkingTo.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => dialogue.SpeakingTo = talkingTo.SelectedItem!.ToString()!);
                PropertyInspector.Controls.Add(talkingTo);

                label = GetLabel("Importance:");
                PropertyInspector.Controls.Add(label);

                ComboBox importance = GetComboBox();
                importance.Items.AddRange(Enum.GetNames<Importance>());
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
                pairedEmote.Items.AddRange(Enum.GetNames<BGCEmotes>());
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
                talkingTo.Items.AddRange(Enum.GetNames<Characters>());
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

                if (alternate is null)
                {
                    Label label2 = GetLabel("No data on this node", ContentAlignment.TopCenter);
                    PropertyInspector.Controls.Add(label2);
                    break;
                }

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
                type.Items.AddRange(Enum.GetNames<GameEvents>());
                type.SelectedItem = gevent.EventType.ToString();
                type.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) =>
                    {
                        gevent.EventType = Enum.Parse<GameEvents>(type.SelectedItem.ToString()!);
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
                        //todo (charactergroup) seems very involved, not gonna implement now
                        break;
                    }
                    case GameEvents.CharacterFunction:
                    {
                        //todo (characterfunction) have to find out how to do functions
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
                                        PutEnumOption<OnOff>(node, gevent);

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
                                        PutEnumOption<OnOff>(node, gevent);

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
                                PutEnumOption<OnOff>(node, gevent);

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
                            if (!Stories.ContainsKey(gevent.Character))
                            {
                                gevent.Character = Stories.First().Key;
                            }
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
                        PropertyInspector.SetColumnSpan(value, 3);

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
                        PutEnumOption<OnOff>(node, gevent);

                        if (gevent.Option == 0)
                        {
                            var box = GetComboBox();
                            box.Items.AddRange(Enum.GetNames<Characters>());
                            box.Items.AddRange(Enum.GetNames<Items>());
                            box.SelectedItem = gevent.Key.Enumize();
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
                                PutEnumValueText<Chairs>(node, gevent);
                                break;
                            }
                            case PlayerActions.LayDown:
                            {
                                PutEnumValueText<Beds>(node, gevent);
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
                        //todo (charactergroup) add charactergroups but i dont care for now
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
                        List<Tuple<string, Quest>> Quests = [];
                        foreach (var item in Stories.Values)
                        {
                            foreach (var questData in item.Quests)
                            {
                                Quests.Add(new(item.CharacterName, questData));
                                box.Items.Add(item.CharacterName + "|" + questData.Name);
                            }
                        }
                        box.SelectedItem = Quests.Find(n => n.Item2.Name == gevent.Value)?.Item1 ?? node.FileName + "|" + gevent.Value;
                        box.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) =>
                        {
                            gevent.Value = box.SelectedItem.ToString()!.Split("|")[1];
                            gevent.Character = Quests.Find(n => n.Item2.Name == gevent.Value)?.Item1 ?? node.FileName;
                            gevent.Key = Quests.Find(n => n.Item2.Name == gevent.Value)?.Item2.ID ?? "#";
                        });
                        PropertyInspector.Controls.Add(box, GeventPropertyCounter++, 1);
                        PropertyInspector.SetColumnSpan(box, 2);
                        GeventPropertyCounter++;

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

                        if (gevent.Character == Player)
                        {
                            gevent.Character = "Amy";
                        }

                        if (!Stories.ContainsKey(gevent.Character))
                        {
                            gevent.Character = Stories.First().Key;
                        }
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
                                PutEnumValueText<MoveTargets>(node, gevent);
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
                        zone.Items.AddRange(Enum.GetNames<Zones>());
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
                        targetType.Items.AddRange(Enum.GetNames<LocationTargetOption>());
                        targetType.SelectedItem = eventTrigger.LocationTargetOption.ToString();
                        targetType.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.LocationTargetOption = Enum.Parse<LocationTargetOption>(targetType.SelectedItem!.ToString()!));
                        PropertyInspector.Controls.Add(targetType);

                        switch (eventTrigger.LocationTargetOption)
                        {
                            case LocationTargetOption.MoveTarget:
                            {
                                ComboBox target = GetComboBox();
                                target.Items.AddRange(Enum.GetNames<MoveTargets>());
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
                        zone.Items.AddRange(Enum.GetNames<BodyRegion>());
                        zone.SelectedItem = ((BodyRegion)int.Parse(eventTrigger.Value!)).ToString();
                        zone.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.Value = ((int)Enum.Parse<BodyRegion>(zone.SelectedItem!.ToString()!)).ToString());
                        PropertyInspector.Controls.Add(zone);

                        break;
                    }
                    case EventTypes.StartedIntimacyAct:
                    {
                        PutEnumKey<SexualActs>(node, eventTrigger);
                        if (eventTrigger.Key == SexualActs.Masturbating.ToString())
                        {
                            PutCharacterValue(node, eventTrigger);
                        }
                        else
                        {
                            eventTrigger.Value = eventTrigger.CharacterToReactTo;
                        }
                        break;
                    }
                    case EventTypes.PlayerReleasesItem:
                    {
                        eventTrigger.Value = string.Empty;
                        PutItemValue(node, eventTrigger);
                        PutStartCondition(node, eventTrigger);
                        break;
                    }
                    case EventTypes.PlayerGrabsItem:
                    {
                        PutItemValue(node, eventTrigger);
                        PutStartCondition(node, eventTrigger);
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
                        //todo (itemfunctions) EventTypes.OnItemFunction:
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
                        PutStartCondition(node, eventTrigger);
                        PutItemValue(node, eventTrigger);
                        break;
                    }
                    case EventTypes.PokedByVibrator:
                    case EventTypes.OnFriendshipIncreaseWith:
                    case EventTypes.OnFriendshipDecreaseWith:
                    case EventTypes.OnRomanceDecreaseWith:
                    case EventTypes.OnRomanceIncreaseWith:
                    {
                        PutStartCondition(node, eventTrigger);
                        PutCharacterValue(node, eventTrigger);
                        break;
                    }
                    case EventTypes.PlayerInteractsWithCharacter:
                    {
                        PutStartCondition(node, eventTrigger);
                        PutCharacter(node, eventTrigger);
                        break;
                    }
                    case EventTypes.PlayerInteractsWithItem:
                    {
                        PutStartCondition(node, eventTrigger);
                        PutCharacter(node, eventTrigger);
                        break;
                    }
                    case EventTypes.OnAfterCutSceneEnds:
                    {
                        PutStartCondition(node, eventTrigger);

                        ComboBox cutscene = GetComboBox();
                        cutscene.Items.AddRange(Enum.GetNames<Cutscenes>());
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

                if (node.FileName == Player)
                {
                    Debugger.Break();
                    return;
                }

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
            case NodeType.Quest:
            {
                PropertyInspector.RowCount = 2;
                PropertyInspector.RowStyles[0].SizeType = SizeType.Absolute;
                PropertyInspector.RowStyles[0].Height = 35;
                PropertyInspector.RowStyles[1].SizeType = SizeType.AutoSize;
                PropertyInspector.ColumnCount = 9;

                Label label = GetLabel(node.FileName + "'s quest:");
                PropertyInspector.Controls.Add(label, 0, 0);

                var quest = node.Data<Quest>();
                if (quest is not null)
                {
                    TextBox name = new()
                    {
                        Dock = DockStyle.Fill,
                        TextAlign = HorizontalAlignment.Center,
                        ForeColor = Color.LightGray,
                        Text = quest!.Name!,
                        Width = 300
                    };
                    name.TextChanged += (_, args) => quest.Name = name.Text;
                    name.TextChanged += (_, _) =>
                    {
                        NodeLinker.UpdateLinks(node, node.FileName, nodes[SelectedCharacter]);
                        Graph.Invalidate();
                    };
                    PropertyInspector.Controls.Add(name, 1, 0);
                    PropertyInspector.SetColumnSpan(name, 3);

                    CheckBox checkBox = GetCheckbox("Obtain on start:", quest.ObtainOnStart);
                    checkBox.CheckedChanged += (_, args) => quest.ObtainOnStart = checkBox.Checked;
                    PropertyInspector.Controls.Add(checkBox, 4, 0);

                    checkBox = GetCheckbox("Display progress:", quest.ShowProgress);
                    checkBox.CheckedChanged += (_, args) => quest.ShowProgress = checkBox.Checked;
                    PropertyInspector.Controls.Add(checkBox, 5, 0);

                    label = GetLabel("complete at");
                    PropertyInspector.Controls.Add(label, 6, 0);

                    NumericUpDown completionValue = GetNumericUpDown(quest.CompleteAt);
                    completionValue.DecimalPlaces = 0;
                    completionValue.Minimum = 0;
                    completionValue.ValueChanged += (_, _) => quest.CompleteAt = (int)completionValue.Value;
                    PropertyInspector.Controls.Add(completionValue, 7, 0);

                    TextBox details = new()
                    {
                        Dock = DockStyle.Fill,
                        ForeColor = Color.LightGray,
                        Text = quest!.Details!,
                        Multiline = true,
                        WordWrap = true,
                        ScrollBars = ScrollBars.Both
                    };
                    details.TextChanged += (_, args) => quest.Details = details.Text;
                    PropertyInspector.Controls.Add(details, 0, 1);

                    TextBox completeddetails = new()
                    {
                        Dock = DockStyle.Fill,
                        ForeColor = Color.LightGray,
                        Text = quest!.CompletedDetails!,
                        Multiline = true,
                        WordWrap = true,
                        ScrollBars = ScrollBars.Both
                    };
                    completeddetails.TextChanged += (_, args) => quest.CompletedDetails = completeddetails.Text;
                    PropertyInspector.Controls.Add(completeddetails, 3, 1);

                    TextBox faileddetails = new()
                    {
                        Dock = DockStyle.Fill,
                        ForeColor = Color.LightGray,
                        Text = quest!.FailedDetails!,
                        Multiline = true,
                        WordWrap = true,
                        ScrollBars = ScrollBars.Both
                    };
                    faileddetails.TextChanged += (_, args) => quest.FailedDetails = faileddetails.Text;
                    PropertyInspector.Controls.Add(faileddetails, 6, 1);

                    PropertyInspector.SetColumnSpan(details, 3);
                    PropertyInspector.SetColumnSpan(faileddetails, 3);
                    PropertyInspector.SetColumnSpan(completeddetails, 3);

                }
                else
                {
                    Label label2 = GetLabel("No data on this node!", ContentAlignment.TopCenter);
                    PropertyInspector.Controls.Add(label2);
                }
                break;
            }
            //todo do the following propetyinspectors
            case NodeType.StoryItem:
            case NodeType.ItemAction:
            case NodeType.ItemGroup:
            case NodeType.ItemGroupBehaviour:
            case NodeType.ItemGroupInteraction:
            case NodeType.BGCResponse:
            case NodeType.CharacterGroup:
            //dont need these:
            case NodeType.State:
            case NodeType.Property:
            case NodeType.Social:
            case NodeType.Achievement:
            case NodeType.Clothing:
            case NodeType.CriteriaGroup:
            case NodeType.Cutscene:
            case NodeType.Door:
            case NodeType.Inventory:
            case NodeType.Null:
            case NodeType.Pose:
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
        NeedsSaving = false;

        PropertyInspector.ResumeLayout();
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
        options.SelectedItem = gevent.Value.Enumize();
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Value = options.SelectedItem!.ToString()!);
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
        options.SelectedItem = gevent.Value2.Enumize();
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Value2 = options.SelectedItem!.ToString()!);
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
        options.SelectedItem = gevent.Key.Enumize();
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Key = options.SelectedItem!.ToString()!);
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumValue<E>(Node node, Criterion criterion) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!int.TryParse(criterion.Value, out int x))
        {
            criterion.Value = "0";
        }
        options.SelectedItem = (Enum.GetName((E)(object)int.Parse(criterion.Value)));
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Value = ((int)(object)Enum.Parse<E>(options.SelectedItem!.ToString()!)).ToString());
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumKey<E>(Node node, Criterion criterion) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!int.TryParse(criterion.Key, out int x))
        {
            criterion.Key = "0";
        }
        options.SelectedItem = (Enum.GetName((E)(object)int.Parse(criterion.Key)));
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Key = ((int)(object)Enum.Parse<E>(options.SelectedItem!.ToString()!)).ToString());
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumKey<E>(Node node, EventTrigger et) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!Enum.GetNames<E>().Contains(et.Key))
        {
            et.Key = options.Items[0]!.ToString()!;
        }
        options.SelectedItem = et.Key.Enumize();
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => et.Key = options.SelectedItem!.ToString()!);
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumKey2<E>(Node node, Criterion criterion) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!int.TryParse(criterion.Key2, out int x))
        {
            criterion.Key2 = "0";
        }
        options.SelectedItem = (Enum.GetName((E)(object)int.Parse(criterion.Key2)));
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Key2 = ((int)(object)Enum.Parse<E>(options.SelectedItem!.ToString()!)).ToString());
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumValueText<E>(Node node, Criterion criterion) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!Enum.GetNames<E>().Contains(criterion.Value))
        {
            criterion.Value = options.Items[0]!.ToString()!;
        }
        options.SelectedItem = criterion.Value.Enumize();
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Value = options.SelectedItem!.ToString()!);
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
        PropertyInspector.Controls.Add(options, GeventPropertyCounter++, 1);
    }

    private void PutEnumOption<E>(Node node, Criterion criterion) where E : struct, Enum
    {
        var options = GetComboBox();
        options.Items.AddRange(Enum.GetNames<E>());
        if (!Enum.GetValues<E>().Contains((E)(object)criterion.Option))
        {
            criterion.Option = 0;
        }
        options.SelectedItem = Enum.GetName(typeof(E), criterion.Option)!;
        options.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Option = (int)(object)Enum.Parse<E>(options.SelectedItem.ToString()!));
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
        character2.Items.AddRange(Enum.GetNames<NoneCharacters>());
        character2.SelectedItem = gevent.Character2!;
        character2.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Character2 = character2.SelectedItem.ToString()!);
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
        character.Items.AddRange(Enum.GetNames<Characters>());
        character.SelectedItem = gevent.Character!;
        character.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => gevent.Character = character.SelectedItem.ToString()!);
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
            Minimum = -100,
            Maximum = 100,
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
            AutoSize = true
        };
    }

    private void PutItemValue(Node node, EventTrigger eventTrigger)
    {
        ComboBox item = GetComboBox();
        item.Items.AddRange(Enum.GetNames<Items>());
        item.SelectedItem = eventTrigger.Value.Enumize();
        item.PerformLayout();
        item.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.Value = item.SelectedItem!.ToString()!);
        PropertyInspector.Controls.Add(item);
    }

    private void PutItemkey(Node node, EventTrigger eventTrigger)
    {
        ComboBox item = GetComboBox();
        item.Items.AddRange(Enum.GetNames<Items>());
        item.SelectedItem = eventTrigger.Key.Enumize();
        item.PerformLayout();
        item.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.Key = item.SelectedItem!.ToString()!);
        PropertyInspector.Controls.Add(item);
    }

    private void PutCharacterValue(Node node, EventTrigger eventTrigger)
    {
        ComboBox character = GetComboBox();
        character.Items.AddRange(Enum.GetNames<AnybodyCharacters>());
        character.SelectedItem = eventTrigger.Value;
        character.PerformLayout();
        character.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => eventTrigger.Value = character.SelectedItem!.ToString()!);
        PropertyInspector.Controls.Add(character);
    }

    private ComboBox PutCharacter(Node node, EventTrigger eventTrigger)
    {
        ComboBox character = GetComboBox();
        character.Items.AddRange(Enum.GetNames<AnybodyCharacters>());
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

    private void PutTextValue(Node node, Criterion criterion)
    {
        TextBox obj2 = new()
        {
            Dock = DockStyle.Fill,
            TextAlign = HorizontalAlignment.Center,
            Text = criterion.Value
        };
        obj2.TextChanged += (_, args) => criterion.Value = obj2.Text;
        obj2.TextChanged += (_, _) => node.ID = $"{criterion.Character}{criterion.CompareType}{criterion.Key}{criterion.Value}";
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
        option.ValueChanged += (_, _) => node.ID = $"{criterion.Character}{criterion.CompareType}{criterion.Key}{criterion.Value}";
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
        option.ValueChanged += (_, _) => node.ID = $"{criterion.Character}{criterion.CompareType}{criterion.Key}{criterion.Value}";
        PropertyInspector.Controls.Add(option);
    }

    private void PutEquation(Node node, Criterion criterion)
    {
        ComboBox equ = GetComboBox();
        equ.Items.AddRange(Enum.GetNames<ComparisonEquations>());
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
        zone.Items.AddRange(Enum.GetNames<Zones>());
        zone.SelectedItem = criterion.Key;
        zone.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Key = (string)zone.SelectedItem!);
        PropertyInspector.Controls.Add(zone);
    }

    private void PutCompareType(Node node, Criterion criterion)
    {
        ComboBox compareType = GetComboBox();
        compareType.Items.AddRange(Enum.GetNames<CompareTypes>());
        compareType.SelectedItem = criterion.CompareType.ToString();
        compareType.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.CompareType = Enum.Parse<CompareTypes>((string)compareType.SelectedItem!));
        PropertyInspector.Controls.Add(compareType);
    }

    private ComboBox PutCharacter1(Node node, Criterion criterion)
    {
        ComboBox compareType = GetComboBox();
        compareType.Items.AddRange(Enum.GetNames<Characters>());
        compareType.SelectedItem = criterion.Character;
        compareType.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Character = (string)compareType.SelectedItem!);
        PropertyInspector.Controls.Add(compareType);
        return compareType;
    }

    private void PutCharacter2(Node node, Criterion criterion)
    {
        ComboBox compareType = GetComboBox();
        compareType.Items.AddRange(Enum.GetNames<Characters>());
        compareType.SelectedItem = criterion.Character2;
        compareType.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Character2 = (string)compareType.SelectedItem!);
        PropertyInspector.Controls.Add(compareType);
    }

    private void PutBoolCriteria(Node node, Criterion criterion)
    {
        ComboBox boolValue = GetComboBox();
        boolValue.Items.AddRange(Enum.GetNames<BoolCritera>());
        boolValue.SelectedItem = criterion.BoolValue!.ToString();
        boolValue.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.BoolValue = Enum.Parse<BoolCritera>(boolValue.SelectedItem!.ToString()!));
        PropertyInspector.Controls.Add(boolValue);
    }

    private void PutItem(Node node, Criterion criterion)
    {
        ComboBox item = GetComboBox();
        item.Items.AddRange(Enum.GetNames<Items>());
        item.SelectedItem = criterion.Key!.Enumize();
        item.AddComboBoxHandler(node, nodes[SelectedCharacter], (_, _) => criterion.Key = item.SelectedItem!.ToString()!);
        PropertyInspector.Controls.Add(item);
    }

    private void PutEquals(Node node, Criterion criterion)
    {
        ComboBox equals = GetComboBox();
        equals.Items.AddRange(Enum.GetNames<EqualsValues>());
        equals.SelectedIndex = (int)criterion.EqualsValue!;
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
            if (Scaling[SelectedCharacter] < 5)
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

        if (SelectedNode != Node.NullNode)
        {
            character = SelectedNode.FileName;
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

        if (SelectedNode == Node.NullNode)
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
            ShoveNodesToRight(newNode, SelectedNode.Position + new Size(scaleX, 0));
            newNode.Position = SelectedNode.Position + new Size(scaleX, 0);
        }

        SelectedNode = newNode;
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
                newNode = Node.CreateCriteriaNode(new() { Character = character }, character, nodes[SelectedCharacter]);
                nodes[character].Add(newNode);

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
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

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
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

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
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

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
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

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
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

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
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

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
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

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.GameEvent:
            {
                string id = Guid.NewGuid().ToString();
                newNode = new Node(id, NodeType.GameEvent, string.Empty, nodes[character].Positions)
                {
                    RawData = new GameEvent() { Character = character, Id = id },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
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

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
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

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
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

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.ItemGroupInteraction:
            {
                string id = "interaction name";
                newNode = new Node(id, NodeType.ItemGroupInteraction, string.Empty, nodes[character].Positions)
                {
                    RawData = new ItemGroupInteraction() { Name = id },
                    FileName = character,
                };
                nodes[character].Add(newNode);

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
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

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
                }
                break;
            }
            case SpawnableNodeType.Quest:
            {
                string id = Guid.NewGuid().ToString();

                if (SelectedNode != Node.NullNode)
                {
                    if (SelectedNode.Type == NodeType.Quest && SelectedNode.DataType == typeof(MissingReferenceInfo))
                    {
                        newNode = new Node(id, NodeType.Quest, string.Empty, nodes[character].Positions)
                        {
                            RawData = new Quest() { CharacterName = character, ID = id },
                            FileName = character,
                        };
                    }
                    else if (SelectedNode.Type == NodeType.Quest && SelectedNode.DataType == typeof(Quest))
                    {
                        newNode = new Node(id, NodeType.Quest, string.Empty, nodes[character].Positions)
                        {
                            RawData = new ExtendedDetail() { Value = SelectedNode.Data<Quest>()!.ExtendedDetails!.Count },
                            FileName = character,
                        };

                        SelectedNode.Data<Quest>()!.ExtendedDetails.Add(newNode.Data<ExtendedDetail>()!);
                    }
                }
                else
                {
                    newNode = new Node(id, NodeType.Quest, string.Empty, nodes[character].Positions)
                    {
                        RawData = new Quest() { CharacterName = character, ID = id },
                        FileName = character,
                    };
                }

                nodes[character].Add(newNode);

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
                }

                if (newNode.Data<Quest>() is not null)
                {
                    Stories[character].Quests.Add(newNode.Data<Quest>()!);
                    var questStore = nodes[character].Nodes.FirstOrDefault(n => n.ID == character + "'s Quests");
                    if (questStore is null)
                    {
                        Debugger.Break();
                        break;
                    }
                    nodes[character].AddChild(questStore, newNode);
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

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
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

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
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

                if (SelectedNode != Node.NullNode)
                {
                    NodeLinker.Link(nodes[SelectedCharacter], SelectedNode, newNode);
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
        var childs = nodes[SelectedCharacter].Childs(SelectedNode);

        for (int i = 0; i < childs.Count; i++)
        {
            var newPos = SelectedNode.Position + new SizeF(scaleX, (i - (childs.Count / 2)) * scaleY);
            ShoveNodesToRight(childs[i], newPos);

            childs[i].Position = newPos;
        }

        CenterAndSelectNode(SelectedNode, 0.3f);
        Graph.Invalidate();
    }

    private void PullParentsClose(object sender, EventArgs e)
    {
        //clickednode is set when this is called
        var parents = nodes[SelectedCharacter].Parents(SelectedNode);

        for (int i = 0; i < parents.Count; i++)
        {
            var newPos = SelectedNode.Position - new SizeF(scaleX, (i - (parents.Count / 2)) * scaleY);
            ShoveNodesToLeft(parents[i], newPos);
            parents[i].Position = newPos;
        }

        CenterAndSelectNode(SelectedNode, 0.3f);
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
        if (SelectedNode == Node.NullNode)
        {
            return;
        }
        //clickednode is set when this is called
        visited.Clear();
        var intX = (int)(SelectedNode.Position.X / scaleX);
        var intY = (int)(SelectedNode.Position.Y / scaleY);
        maxYperX.ExtendToIndex(intX, intY);

        for (int i = 0; i < maxYperX.Count; i++)
        {
            maxYperX[i] = intY;
        }
        //selectedcharacter is set correctly here
        SetStartPosForConnected(intX, nodes[SelectedCharacter], SelectedNode);

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

    private void SearchButton_Click(object sender, EventArgs e)
    {
        StartSearch();
    }
}
