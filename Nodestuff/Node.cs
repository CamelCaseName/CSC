using CSC.StoryItems;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using System.Windows.Forms.VisualStyles;
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
        Value,
        UseWith
    }

    public enum SpawnableNodeType
    {
        Criterion,
        ItemAction,
        Achievement,
        BGC,
        BGCResponse,
        CriteriaGroup,
        Dialogue,
        AlternateText,
        Event,
        EventTrigger,
        Item,
        ItemGroup,
        Quest,
        UseWith,
        Response,
        Value
    }

    public sealed class MissingReferenceInfo
    {
        public MissingReferenceInfo()
        {
            Text = string.Empty;
        }
        public MissingReferenceInfo(string text)
        {
            Text = text;
        }

        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Text { get; set; }
    }

    public sealed class Node
    {
        public static readonly Node NullNode = new(new());

        public Gender Gender = Gender.None;
        public Guid Guid = Guid.NewGuid();
        public int Mass = 1;
        public NodeType Type;
        private object? data = null;
        public SizeF Size = new(Main.NodeSizeX, Main.NodeSizeY);
        public string ID;
        public string Text;
        private string fileName = Main.NoCharacter;
        private Type dataType = typeof(MissingReferenceInfo);
        public NodePositionSorting CurrentPositionSorting
        {
            get
            {
                if (SortingList.TryGetValue(Main.SelectedCharacter, out var result))
                {
                    return result;
                }
                else
                {
                    return SortingList[FileName];
                }
            }
        }

        private readonly Dictionary<string, NodePositionSorting> SortingList;
        private readonly Dictionary<string, PointF> Positions = new() { { Main.NoCharacter, new() } };

        public PointF Position
        {
            get
            {
                if (Positions.TryGetValue(Main.SelectedCharacter, out var result))
                {
                    return result;
                }
                else
                {
                    return Positions[FileName];
                }
            }

            set
            {
                CurrentPositionSorting.ClearNode(this);
                Positions[Main.SelectedCharacter] = value;
                CurrentPositionSorting.SetNode(this);
            }
        }

        public void DupeToOtherSorting(string filename, NodePositionSorting sorting)
        {
            SortingList[filename] = sorting;
            Positions[filename] = new();
            sorting.SetNode(this);
        }

        public bool Visited { get; internal set; }
        private bool firstTimeSettingFilename = true;

        public RectangleF Rectangle { get => new(Position, Size); }

        public Rectangle RectangleNonF { get => new((int)Position.X, (int)Position.Y, (int)Size.Width, (int)Size.Height); }

        public object? rawData
        {
            get
            {
                return data ?? new MissingReferenceInfo(Text);
            }
            set
            {
                data = value;
                DataType = value?.GetType() ?? typeof(object);
            }
        }

        public T? Data<T>() where T : class, new()
        {
            if (typeof(T) == DataType && rawData is not null)
            {
                return (T)rawData;
            }
            else
            {
                return null;
            }
        }

        public string FileName
        {
            get
            {
                return fileName;
            }

            set
            {
                fileName = value;
                if (firstTimeSettingFilename)
                {
                    if (value != Main.NoCharacter)
                    {
                        firstTimeSettingFilename = false;
                        SortingList.Add(fileName, SortingList[Main.NoCharacter]);
                        SortingList.Remove(Main.NoCharacter);
                        Positions.Add(fileName, Positions[Main.NoCharacter]);
                        Positions.Remove(Main.NoCharacter);
                    }
                }
            }
        }

        public Type DataType { get => dataType; private set => dataType = value; }

        public Node(string iD, NodeType type, string text, NodePositionSorting sorting)
        {
            SortingList = new()
            {
                { Main.NoCharacter, sorting }
            };
            ID = iD;
            Text = text;
            Type = type;
            Size = new SizeF(Main.NodeSizeX, Main.NodeSizeY);
        }

        public Node(string iD, NodeType type, string text, object data, NodePositionSorting sorting)
        {
            SortingList = new()
            {
                { Main.NoCharacter, sorting }
            };
            ID = iD;
            Text = text;
            Type = type;
            rawData = data;
            DataType = data.GetType();
            Size = new SizeF(Main.NodeSizeX, Main.NodeSizeY);
        }

        public Node(NodePositionSorting sorting)
        {
            SortingList = new()
            {
                { Main.NoCharacter, sorting }
            };
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
                    $"{criterion.Character}|{criterion.CompareType}|{criterion.DialogueStatus}|{criterion.Key}|{criterion.Value}", node.CurrentPositionSorting)
                { FileName = node.FileName };
            }
        }

        public void AddCriteria(List<Criterion> criteria, NodeStore nodes)
        {
            foreach (Criterion criterion in criteria)
            {
                Node tempNode = CreateCriteriaNode(criterion, this, nodes);
                tempNode.rawData = criterion;
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
                var nodeEvent = new Node(_event.Id ?? "none", NodeType.Event, _event.Value ?? "none", this, CurrentPositionSorting)
                {
                    rawData = _event,
                    DataType = typeof(GameEvent),
                    FileName = this.FileName
                };

                nodeEvent.AddCriteria(_event.Criteria ?? [], nodes);

                nodes.AddChild(this, nodeEvent);
            }
        }
    }
}
