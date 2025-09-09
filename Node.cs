using CSC.StoryItems;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        public string ID;
        public string Text;
        public Type DataType = typeof(object);
        public Control control;

        public Node(Control control, string iD, NodeType type, string text)
        {
            this.control = control;
            ID = iD;
            Text = text;
            Type = type;
        }
        public Node(Control control, string iD, NodeType type, string text, object data)
        {
            this.control = control;
            ID = iD;
            Text = text;
            Type = type;
            Data = data;
            DataType = data.GetType();
        }

        public Node()
        {
            control = new();
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
