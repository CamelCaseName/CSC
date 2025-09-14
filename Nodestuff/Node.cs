using CSC.StoryItems;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using static CSC.StoryItems.StoryEnums;

namespace CSC.Nodestuff
{
    public enum ClickedNodeTypes
    {
        Null,
        Highlight,
        Info,
        OpenInEditor,
        Edit
    }

    public enum NodeType
    {
        Null,
        CharacterGroup,
        Criterion,
        ItemAction,
        ItemGroupBehaviour,
        ItemGroupInteraction,
        Pose,
        Achievement,
        BGC,
        BGCResponse,
        Clothing,
        CriteriaGroup,
        Cutscene,
        Dialogue,
        AlternateText,
        Door,
        Event,
        EventTrigger,
        Inventory,
        Item,
        ItemGroup,
        Personality,
        Property,
        Quest,
        Response,
        Social,
        State,
        Value
    }

    public sealed class MissingreferenceInfo(string text)
    {
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Text { get; set; } = text;
    }

    public sealed class Node
    {
        public static readonly Node NullNode = new(new());

        public Gender Gender = Gender.None;
        public Guid Guid = Guid.NewGuid();
        public int Mass = 1;
        public NodeType Type;
        private object? data = null;
        private PointF position = PointF.Empty;
        public SizeF Size = SizeF.Empty;
        public string ID;
        public string Text;
        public string FileName = Main.NoCharacter;
        public Type DataType = typeof(object);
        private readonly NodePositionSorting positionSorting;

        public PointF Position
        {
            get
            {
                return position;
            }

            set
            {
                positionSorting.ClearNode(this);
                position = value;
                positionSorting.SetNode(this);
            }
        }

        public bool Visited { get; internal set; }

        public RectangleF Rectangle { get => new(position, Size); }

        public Rectangle RectangleNonF { get => new((int)position.X, (int)position.Y, (int)Size.Width, (int)Size.Height); }
       
        public object? Data
        {
            get
            {
                return data ?? new MissingreferenceInfo(Text);
            }
            set
            {
                data = value;
                DataType = value?.GetType() ?? typeof(object);
            }
        }

        public Node(string iD, NodeType type, string text, NodePositionSorting sorting)
        {
            positionSorting = sorting;
            ID = iD;
            Text = text;
            Type = type;
            Size = new SizeF(Main.NodeSizeX, Main.NodeSizeY);
        }

        public Node(string iD, NodeType type, string text, object data, NodePositionSorting sorting)
        {
            positionSorting = sorting;
            ID = iD;
            Text = text;
            Type = type;
            Data = data;
            DataType = data.GetType();
            Size = new SizeF(Main.NodeSizeX, Main.NodeSizeY);
        }

        public Node(NodePositionSorting sorting)
        {
            positionSorting = sorting;
            ID = string.Empty;
            Text = string.Empty;
            Type = NodeType.Null;
        }

        public override string ToString()
        {
            return $"{Type} | {ID} | {Text}";
        }

        public string ToOutputFormat()
        {
            return $"{ID}|{Text}";
        }

        public static Node CreateCriteriaNode(Criterion criterion, Node node, NodeStore nodes)
        {
            //create all criteria nodes the same way so they can possibly be replaced by the actual text later
            var result = nodes.Nodes.Find((n) => n.Type == NodeType.Criterion && n.ID == $"{criterion.Character}{criterion.CompareType}{criterion.Value}");
            if (result is not null)
            {
                return result;
            }
            else
            {
                return new Node(
                    $"{criterion.Character}{criterion.CompareType}{criterion.Value}",
                    NodeType.Criterion,
                    $"{criterion.Character}|{criterion.CompareType}|{criterion.DialogueStatus}|{criterion.Key}|{criterion.Value}", node.positionSorting)
                { FileName = node.FileName };
            }
        }

        public void AddCriteria(List<Criterion> criteria, NodeStore nodes)
        {
            foreach (Criterion criterion in criteria)
            {
                Node tempNode = CreateCriteriaNode(criterion, this, nodes);
                tempNode.Data = criterion;
                tempNode.DataType = typeof(Criterion);
                if (criterion.CompareType == CompareTypes.PlayerGender)
                {
                    tempNode.Gender = criterion.Value == "Female" ? Gender.Female : criterion.Value == "Male" ? Gender.Male : Gender.None;
                }

                nodes.AddParent(this, tempNode);
            }
        }

        public void AddEvents(List<GameEvent> events, NodeStore nodes)
        {
            foreach (GameEvent _event in events)
            {
                var nodeEvent = new Node(_event.Id ?? "none", NodeType.Event, _event.Value ?? "none", this, positionSorting) { FileName = FileName, Data = _event, DataType = typeof(GameEvent) };

                nodeEvent.AddCriteria(_event.Criteria ?? new List<Criterion>(), nodes);

                nodes.AddParent(this, nodeEvent);
            }
        }
    }
}
