using static CSC.StoryItems.StoryEnums;

namespace CSC
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

    public sealed class Node
    {
        public static readonly Node NullNode = new();

        public Gender Gender = Gender.None;
        public Guid Guid = Guid.NewGuid();
        public int Mass = 1;
        public NodeType Type;
        public object? Data = null;
        public PointF Position = PointF.Empty;
        public SizeF Size = SizeF.Empty;
        public string ID;
        public string Text;
        public Type DataType = typeof(object);

        public Node(string iD, NodeType type, string text)
        {
            ID = iD;
            Text = text;
            Type = type;
        }
        public Node(string iD, NodeType type, string text, object data)
        {
            ID = iD;
            Text = text;
            Type = type;
            Data = data;
            DataType = data.GetType();
        }

        public Node()
        {
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
    }
}
