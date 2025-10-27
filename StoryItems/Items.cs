using System.Text.Json.Serialization;
using static CSC.StoryItems.StoryEnums;

namespace CSC.StoryItems
{
    public interface IItem
    {
        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public const string hash = "#";
    }

    public sealed class Criterion : IItem
    {
        private BoolCritera boolValue = BoolCritera.True;
        private string character = IItem.hash;
        private string character2 = IItem.hash;
        private CompareTypes compareType = CompareTypes.Never;
        private DialogueStatuses dialogueStatus = DialogueStatuses.WasShown;
        private bool displayInEditor = true;
        private DoorOptionValues doorOptions = DoorOptionValues.IsClosed;
        private EqualsValues equalsValue = EqualsValues.Equals;
        private ComparisonEquations equationValue = ComparisonEquations.Equals;
        private ValueSpecificFormulas valueFormula = ValueSpecificFormulas.EqualsValue;
        private ItemComparisonTypes itemComparison = ItemComparisonTypes.IsActive;
        private ItemFromItemGroupComparisonTypes itemFromItemGroupComparison = ItemFromItemGroupComparisonTypes.IsActive;
        private string key = IItem.hash;
        private string key2 = IItem.hash;
        private int order = 0;
        private PlayerInventoryOptions playerInventoryOption = PlayerInventoryOptions.HasItem;
        private PoseOptions poseOption = PoseOptions.IsCurrentlyPosing;
        private SocialStatuses socialStatus = SocialStatuses.Drunk;
        private string _value = IItem.hash;
        private int option = 0;
        private CompareTypes groupSubCompareType = CompareTypes.Never;
        private string version = "1.0";
        private bool TextNeedsUpdate = true;
        private string _text = string.Empty;

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange = (o) =>
        {
            if (o is Criterion c)
            {
                c.TextNeedsUpdate = true;
            }
        };

        public BoolCritera BoolValue { get => boolValue; set { OnBeforeChange?.Invoke(this); boolValue = value; OnAfterChange?.Invoke(this); } }
        public string Character { get => character; set { OnBeforeChange?.Invoke(this); if (string.IsNullOrWhiteSpace(value)) { character = IItem.hash; } else { character = value; } OnAfterChange?.Invoke(this); } }
        public string Character2 { get => character2; set { OnBeforeChange?.Invoke(this); if (string.IsNullOrWhiteSpace(value)) { character2 = IItem.hash; } else { character2 = value; } OnAfterChange?.Invoke(this); } }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CompareTypes CompareType { get => compareType; set { OnBeforeChange?.Invoke(this); compareType = value; OnAfterChange?.Invoke(this); } }
        public DialogueStatuses DialogueStatus { get => dialogueStatus; set { OnBeforeChange?.Invoke(this); dialogueStatus = value; OnAfterChange?.Invoke(this); } }
        public bool DisplayInEditor { get => displayInEditor; set { OnBeforeChange?.Invoke(this); displayInEditor = value; OnAfterChange?.Invoke(this); } }
        public DoorOptionValues DoorOptions { get => doorOptions; set { OnBeforeChange?.Invoke(this); doorOptions = value; OnAfterChange?.Invoke(this); } }
        public EqualsValues EqualsValue { get => equalsValue; set { OnBeforeChange?.Invoke(this); equalsValue = value; OnAfterChange?.Invoke(this); } }
        public ComparisonEquations EquationValue { get => equationValue; set { OnBeforeChange?.Invoke(this); equationValue = value; OnAfterChange?.Invoke(this); } }
        public ValueSpecificFormulas ValueFormula { get => valueFormula; set { OnBeforeChange?.Invoke(this); valueFormula = value; OnAfterChange?.Invoke(this); } }
        public ItemComparisonTypes ItemComparison { get => itemComparison; set { OnBeforeChange?.Invoke(this); itemComparison = value; OnAfterChange?.Invoke(this); } }
        public ItemFromItemGroupComparisonTypes ItemFromItemGroupComparison { get => itemFromItemGroupComparison; set { OnBeforeChange?.Invoke(this); itemFromItemGroupComparison = value; OnAfterChange?.Invoke(this); } }
        public string Key { get => key; set { OnBeforeChange?.Invoke(this); if (string.IsNullOrWhiteSpace(value)) { key = IItem.hash; } else { key = value; } OnAfterChange?.Invoke(this); } }
        public string Key2 { get => key2; set { OnBeforeChange?.Invoke(this); if (string.IsNullOrWhiteSpace(value)) { key2 = IItem.hash; } else { key2 = value; } OnAfterChange?.Invoke(this); } }
        public int Order { get => order; set { OnBeforeChange?.Invoke(this); order = value; OnAfterChange?.Invoke(this); } }
        public PlayerInventoryOptions PlayerInventoryOption { get => playerInventoryOption; set { OnBeforeChange?.Invoke(this); playerInventoryOption = value; OnAfterChange?.Invoke(this); } }
        public PoseOptions PoseOption { get => poseOption; set { OnBeforeChange?.Invoke(this); poseOption = value; OnAfterChange?.Invoke(this); } }
        public SocialStatuses SocialStatus { get => socialStatus; set { OnBeforeChange?.Invoke(this); socialStatus = value; OnAfterChange?.Invoke(this); } }
        public string Value { get => _value; set { OnBeforeChange?.Invoke(this); if (string.IsNullOrWhiteSpace(value)) { _value = IItem.hash; } else { _value = value; } OnAfterChange?.Invoke(this); } }
        public int Option { get => option; set { OnBeforeChange?.Invoke(this); option = value; OnAfterChange?.Invoke(this); } }
        [JsonConverter(typeof(JsonNumberEnumConverter<CompareTypes>))]
        public CompareTypes GroupSubCompareType { get => groupSubCompareType; set { OnBeforeChange?.Invoke(this); groupSubCompareType = value; OnAfterChange?.Invoke(this); } }
        public string Version { get => version; set { OnBeforeChange?.Invoke(this); version = value; OnAfterChange?.Invoke(this); } }
        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != typeof(Criterion))
            {
                return false;
            }

            bool result = false;

            if (obj is Criterion other)
            {
                result = true;
                result &= BoolValue.Equals(other.BoolValue);
                result &= Character.Equals(other.Character);
                result &= Character2.Equals(other.Character2);
                result &= CompareType.Equals(other.CompareType);
                result &= DialogueStatus.Equals(other.DialogueStatus);
                result &= DisplayInEditor.Equals(other.DisplayInEditor);
                result &= DoorOptions.Equals(other.DoorOptions);
                result &= EqualsValue.Equals(other.EqualsValue);
                result &= EquationValue.Equals(other.EquationValue);
                result &= ValueFormula.Equals(other.ValueFormula);
                result &= ItemComparison.Equals(other.ItemComparison);
                result &= ItemFromItemGroupComparison.Equals(other.ItemFromItemGroupComparison);
                result &= Key.Equals(other.Key);
                result &= Key2.Equals(other.Key2);
                result &= Order.Equals(other.Order);
                result &= PlayerInventoryOption.Equals(other.PlayerInventoryOption);
                result &= PoseOption.Equals(other.PoseOption);
                result &= SocialStatus.Equals(other.SocialStatus);
                result &= Value.Equals(other.Value);
                result &= Option.Equals(other.Option);
                result &= GroupSubCompareType.Equals(other.GroupSubCompareType);
                result &= Version.Equals(other.Version);
            }

            return result;
        }

        public static bool operator !=(Criterion c1, Criterion c2)
        {
            return !(c1 == c2);
        }

        public static bool operator ==(Criterion c1, Criterion c2)
        {
            return c1.Equals(c2);
        }

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString()
        {
            if (TextNeedsUpdate)
            {
                _text = ToS();
                TextNeedsUpdate = false;
            }

            return _text;

            string ToS()
            {
                switch (CompareType)
                {
                    case CompareTypes.Never:
                    {
                        return "Never";
                    }
                    case CompareTypes.CharacterFromCharacterGroup:
                    {
                        //todo (charactergroup) once character groups are in
                        return "None";
                    }
                    case CompareTypes.Clothing:
                    {
                        return $"{CompareType} {Character} {Value} {(ClothingSet)Option} {BoolValue}";
                    }
                    case CompareTypes.CoinFlip:
                    {
                        return "Coinflip";
                    }
                    case CompareTypes.CompareValues:
                    {
                        return $"{CompareType} {Character} {Key} {ValueFormula} {Character2} {Key2}";
                    }
                    case CompareTypes.CriteriaGroup:
                    {
                        return $"{CompareType} {Value} {Key2} {BoolValue}";
                    }
                    case CompareTypes.CutScene:
                    {
                        return $"{CompareType}  {Value} {BoolValue}";
                    }
                    case CompareTypes.Dialogue:
                    {
                        return $"{CompareType} {Character} {Value} {DialogueStatus}";
                    }
                    case CompareTypes.Distance:
                    {
                        return $"{CompareType} {Key} {Key2} {EquationValue} {Option}";
                    }
                    case CompareTypes.Door:
                    {
                        return $"{CompareType} {Key} {DoorOptions}";
                    }
                    case CompareTypes.Gender:
                    {
                        return $"{CompareType} {Character} {(Gender)Option}";
                    }
                    case CompareTypes.IntimacyPartner:
                    {
                        return $"{CompareType} {Character} {EqualsValue} {Value}";
                    }
                    case CompareTypes.IntimacyState:
                    {
                        return $"{CompareType} {Character} {EqualsValue} {Value}";
                    }
                    case CompareTypes.InZone:
                    {
                        return $"{CompareType} {Character} {Key} {BoolValue}";
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
                        return $"{Character} {CompareType} {Character2} {BoolValue}";
                    }
                    case CompareTypes.Item:
                    {
                        return $"{CompareType} {Key} {ItemComparison}";
                    }
                    case CompareTypes.IsBeingSpokenTo:
                    case CompareTypes.IsCharacterEnabled:
                    case CompareTypes.IsInHouse:
                    case CompareTypes.MetByPlayer:
                    {
                        return $"{CompareType} {Character} {BoolValue}";
                    }
                    case CompareTypes.IsCurrentlyBeingUsed:
                    {
                        return $"{CompareType} {Key} {BoolValue}";
                    }
                    case CompareTypes.IsCurrentlyUsing:
                    {
                        return $"{CompareType} {Character} {Key} {BoolValue}";
                    }
                    case CompareTypes.IsExplicitGameVersion:
                    case CompareTypes.IsGameUncensored:
                    case CompareTypes.IsNewGame:
                    case CompareTypes.ScreenFadeVisible:
                    case CompareTypes.UseLegacyIntimacy:
                    {
                        return $"{CompareType} {BoolValue}";
                    }
                    case CompareTypes.IsPackageInstalled:
                    {
                        return $"{CompareType} {Value}";
                    }
                    case CompareTypes.IsZoneEmpty:
                    {
                        return $"{CompareType} {Key} {BoolValue}";
                    }
                    case CompareTypes.ItemFromItemGroup:
                    {
                        return $"{CompareType} {Key} {ItemFromItemGroupComparison} {(ItemFromItemGroupComparison == ItemFromItemGroupComparisonTypes.IsVisibleTo ? Character : "")} {BoolValue}";
                    }
                    case CompareTypes.Personality:
                    {
                        return $"{CompareType} {Character} {Key} {EquationValue} {Value}";
                    }
                    case CompareTypes.PlayerGender:
                    {
                        return $"{CompareType} {Value}";
                    }
                    case CompareTypes.PlayerInventory:
                    {
                        return $"{CompareType} {PlayerInventoryOption} {(PlayerInventoryOption == PlayerInventoryOptions.HasItem ? Key : "")} {BoolValue}";
                    }
                    case CompareTypes.PlayerPrefs:
                    {
                        return $"{CompareType} {Key} {EquationValue} {Value}";
                    }
                    case CompareTypes.Posing:
                    {
                        return $"{CompareType} {Character} {PoseOption} {(PoseOption == PoseOptions.CurrentPose ? (EqualsValue.ToString() + Value) : BoolValue)}";
                    }
                    case CompareTypes.Property:
                    {
                        return $"{CompareType} {Character} {Value} {BoolValue}";
                    }
                    case CompareTypes.Quest:
                    {
                        return $"{CompareType} {Key2} {EqualsValue} {Value}";
                    }
                    case CompareTypes.Social:
                    {
                        return $"{CompareType} {Character} {SocialStatus} {EquationValue} {Value}";
                    }
                    case CompareTypes.State:
                    {
                        return $"{CompareType} {Character} {Value} {BoolValue}";
                    }
                    case CompareTypes.Value:
                    {
                        return $"{CompareType} {Key} {EquationValue} {Value}";
                    }
                    case CompareTypes.None:
                    default:
                    {
                        return "None";
                    }
                }
            }
        }
    }

    public sealed class ItemAction : IItem
    {
        private string actionName = string.Empty;
        private List<Criterion> criteria = [];
        private bool displayInEditor = true;
        private List<GameEvent> onTakeActionEvents = [];

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public string ActionName { get => actionName; set { OnBeforeChange?.Invoke(this); actionName = value; OnAfterChange?.Invoke(this); } }
        public List<Criterion> Criteria { get => criteria; set { OnBeforeChange?.Invoke(this); criteria = value; OnAfterChange?.Invoke(this); } }
        public bool DisplayInEditor { get => displayInEditor; set { OnBeforeChange?.Invoke(this); displayInEditor = value; OnAfterChange?.Invoke(this); } }
        public List<GameEvent> OnTakeActionEvents { get => onTakeActionEvents; set { OnBeforeChange?.Invoke(this); onTakeActionEvents = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class UseWith : IItem
    {
        private List<Criterion> criteria = [];
        private string customCantDoThatMessage = string.Empty;
        private bool displayInEditor = true;
        private string itemName = string.Empty;
        private List<GameEvent> onSuccessEvents = [];

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public List<Criterion> Criteria { get => criteria; set { OnBeforeChange?.Invoke(this); criteria = value; OnAfterChange?.Invoke(this); } }
        public string CustomCantDoThatMessage { get => customCantDoThatMessage; set { OnBeforeChange?.Invoke(this); customCantDoThatMessage = value; OnAfterChange?.Invoke(this); } }
        public bool DisplayInEditor { get => displayInEditor; set { OnBeforeChange?.Invoke(this); displayInEditor = value; OnAfterChange?.Invoke(this); } }
        public string ItemName { get => itemName; set { OnBeforeChange?.Invoke(this); itemName = value; OnAfterChange?.Invoke(this); } }
        public List<GameEvent> OnSuccessEvents { get => onSuccessEvents; set { OnBeforeChange?.Invoke(this); onSuccessEvents = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class InteractiveitemBehaviour : IItem
    {
        private string id = Guid.NewGuid().ToString();
        private bool displayInEditor = true;
        private string displayName = string.Empty;
        private List<ItemAction> itemActions = [];
        private string itemName = string.Empty;
        private List<UseWith> useWiths = [];
        private bool useDefaultRadialOptions = true;

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public string Id { get => id; set { OnBeforeChange?.Invoke(this); id = value; OnAfterChange?.Invoke(this); } }
        public bool DisplayInEditor { get => displayInEditor; set { OnBeforeChange?.Invoke(this); displayInEditor = value; OnAfterChange?.Invoke(this); } }
        public string DisplayName { get => displayName; set { OnBeforeChange?.Invoke(this); displayName = value; OnAfterChange?.Invoke(this); } }
        public List<ItemAction> ItemActions { get => itemActions; set { OnBeforeChange?.Invoke(this); itemActions = value; OnAfterChange?.Invoke(this); } }
        public string ItemName { get => itemName; set { OnBeforeChange?.Invoke(this); itemName = value; OnAfterChange?.Invoke(this); } }
        public List<UseWith> UseWiths { get => useWiths; set { OnBeforeChange?.Invoke(this); useWiths = value; OnAfterChange?.Invoke(this); } }
        public bool UseDefaultRadialOptions { get => useDefaultRadialOptions; set { OnBeforeChange?.Invoke(this); useDefaultRadialOptions = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class ItemGroupBehavior : IItem
    {
        private string id = Guid.NewGuid().ToString();
        private string name = string.Empty;
        private string groupName = string.Empty;
        private bool displayInEditor = true;
        private List<ItemAction> itemActions = [];
        private List<UseWith> useWiths = [];

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public string Id { get => id; set { OnBeforeChange?.Invoke(this); id = value; OnAfterChange?.Invoke(this); } }
        public string Name { get => name; set { OnBeforeChange?.Invoke(this); name = value; OnAfterChange?.Invoke(this); } }
        public string GroupName { get => groupName; set { OnBeforeChange?.Invoke(this); groupName = value; OnAfterChange?.Invoke(this); } }
        public bool DisplayInEditor { get => displayInEditor; set { OnBeforeChange?.Invoke(this); displayInEditor = value; OnAfterChange?.Invoke(this); } }
        public List<ItemAction> ItemActions { get => itemActions; set { OnBeforeChange?.Invoke(this); itemActions = value; OnAfterChange?.Invoke(this); } }
        public List<UseWith> UseWiths { get => useWiths; set { OnBeforeChange?.Invoke(this); useWiths = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class Achievement : IItem
    {
        private string description = string.Empty;
        private string id = Guid.NewGuid().ToString();
        private string image = string.Empty;
        private string name = string.Empty;
        private bool showInEditor = true;
        private string steamName = string.Empty;

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public string Description { get => description; set { OnBeforeChange?.Invoke(this); description = value; OnAfterChange?.Invoke(this); } }
        public string Id { get => id; set { OnBeforeChange?.Invoke(this); id = value; OnAfterChange?.Invoke(this); } }
        public string Image { get => image; set { OnBeforeChange?.Invoke(this); image = value; OnAfterChange?.Invoke(this); } }
        public string Name { get => name; set { OnBeforeChange?.Invoke(this); name = value; OnAfterChange?.Invoke(this); } }
        public bool ShowInEditor { get => showInEditor; set { OnBeforeChange?.Invoke(this); showInEditor = value; OnAfterChange?.Invoke(this); } }
        public string SteamName { get => steamName; set { OnBeforeChange?.Invoke(this); steamName = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class CriteriaListWrapper : IItem
    {
        private List<Criterion> criteriaList = [];

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public List<Criterion> CriteriaList { get => criteriaList; set { OnBeforeChange?.Invoke(this); criteriaList = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class CriteriaGroup : IItem
    {
        private string id = Guid.NewGuid().ToString();
        private string name = string.Empty;
        private string linkedGroupName = string.Empty;
        private bool displayInEditor = true;
        private PassCondition passCondition = PassCondition.AnySetIsTrue;
        private List<CriteriaListWrapper> criteriaList = [new CriteriaListWrapper()];

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public string Id { get => id; set { OnBeforeChange?.Invoke(this); id = value; OnAfterChange?.Invoke(this); } }
        public string Name { get => name; set { OnBeforeChange?.Invoke(this); name = value; OnAfterChange?.Invoke(this); } }
        public string LinkedGroupName { get => linkedGroupName; set { OnBeforeChange?.Invoke(this); linkedGroupName = value; OnAfterChange?.Invoke(this); } }
        public bool DisplayInEditor { get => displayInEditor; set { OnBeforeChange?.Invoke(this); displayInEditor = value; OnAfterChange?.Invoke(this); } }
        public PassCondition PassCondition { get => passCondition; set { OnBeforeChange?.Invoke(this); passCondition = value; OnAfterChange?.Invoke(this); } }
        public List<CriteriaListWrapper> CriteriaList { get => criteriaList; set { OnBeforeChange?.Invoke(this); criteriaList = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class ItemGroup : IItem
    {
        private string id = Guid.NewGuid().ToString();
        private string name = string.Empty;
        private bool displayInEditor = true;
        private List<string> itemsInGroup = [];

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;

        public string Id { get => id; set { OnBeforeChange?.Invoke(this); id = value; OnAfterChange?.Invoke(this); } }
        public string Name { get => name; set { OnBeforeChange?.Invoke(this); name = value; OnAfterChange?.Invoke(this); } }
        public bool DisplayInEditor { get => displayInEditor; set { OnBeforeChange?.Invoke(this); displayInEditor = value; OnAfterChange?.Invoke(this); } }
        public List<string> ItemsInGroup { get => itemsInGroup; set { OnBeforeChange?.Invoke(this); itemsInGroup = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class GameEvent : IItem
    {
        private EventSpecialHandling handling = EventSpecialHandling.None;
        private string version = "2.0";
        private string id = Guid.NewGuid().ToString();
        private bool enabled = true;
        private GameEvents eventType = GameEvents.None;
        private GameEvents groupSubEventType = GameEvents.None;
        private string character = IItem.hash;
        private string character2 = IItem.hash;
        private string key = IItem.hash;
        private int option = 0;
        private int option2 = 0;
        private int option3 = 0;
        private int option4 = 0;
        private string _value = string.Empty;
        private string value2 = string.Empty;
        private int sortOrder = 0;
        private double delay = 0;
        private double originalDelay = 0;
        private double startDelayTime = 0;
        private bool useConditions = false;
        private bool displayInEditor = true;
        private List<Criterion> criteria = [];

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public EventSpecialHandling Handling { get => handling; set { OnBeforeChange?.Invoke(this); handling = value; OnAfterChange?.Invoke(this); } }
        public string Version { get => version; set { OnBeforeChange?.Invoke(this); version = value; OnAfterChange?.Invoke(this); } }
        public string Id { get => id; set { OnBeforeChange?.Invoke(this); id = value; OnAfterChange?.Invoke(this); } }
        public bool Enabled { get => enabled; set { OnBeforeChange?.Invoke(this); enabled = value; OnAfterChange?.Invoke(this); } }
        public GameEvents EventType { get => eventType; set { OnBeforeChange?.Invoke(this); eventType = value; OnAfterChange?.Invoke(this); } }
        public GameEvents GroupSubEventType { get => groupSubEventType; set { OnBeforeChange?.Invoke(this); groupSubEventType = value; OnAfterChange?.Invoke(this); } }
        public string Character { get => character; set { OnBeforeChange?.Invoke(this); if (string.IsNullOrWhiteSpace(value)) { character = IItem.hash; } else { character = value; } OnAfterChange?.Invoke(this); } }
        public string Character2 { get => character2; set { OnBeforeChange?.Invoke(this); if (string.IsNullOrWhiteSpace(value)) { character2 = IItem.hash; } else { character2 = value; } OnAfterChange?.Invoke(this); } }
        public string Key { get => key; set { OnBeforeChange?.Invoke(this); if (string.IsNullOrWhiteSpace(value)) { key = IItem.hash; } else { key = value; } OnAfterChange?.Invoke(this); } }
        public int Option { get => option; set { OnBeforeChange?.Invoke(this); option = value; OnAfterChange?.Invoke(this); } }
        public int Option2 { get => option2; set { OnBeforeChange?.Invoke(this); option2 = value; OnAfterChange?.Invoke(this); } }
        public int Option3 { get => option3; set { OnBeforeChange?.Invoke(this); option3 = value; OnAfterChange?.Invoke(this); } }
        public int Option4 { get => option4; set { OnBeforeChange?.Invoke(this); option4 = value; OnAfterChange?.Invoke(this); } }
        public string Value { get => _value; set { OnBeforeChange?.Invoke(this); _value = value; OnAfterChange?.Invoke(this); } }
        public string Value2 { get => value2; set { OnBeforeChange?.Invoke(this); value2 = value; OnAfterChange?.Invoke(this); } }
        public int SortOrder { get => sortOrder; set { OnBeforeChange?.Invoke(this); sortOrder = value; OnAfterChange?.Invoke(this); } }
        public double Delay { get => delay; set { OnBeforeChange?.Invoke(this); delay = value; OnAfterChange?.Invoke(this); } }
        public double OriginalDelay { get => originalDelay; set { OnBeforeChange?.Invoke(this); originalDelay = value; OnAfterChange?.Invoke(this); } }
        public double StartDelayTime { get => startDelayTime; set { OnBeforeChange?.Invoke(this); startDelayTime = value; OnAfterChange?.Invoke(this); } }
        public bool UseConditions { get => useConditions; set { OnBeforeChange?.Invoke(this); useConditions = value; OnAfterChange?.Invoke(this); } }
        public bool DisplayInEditor { get => displayInEditor; set { OnBeforeChange?.Invoke(this); displayInEditor = value; OnAfterChange?.Invoke(this); } }
        public List<Criterion> Criteria { get => criteria; set { OnBeforeChange?.Invoke(this); criteria = value; OnAfterChange?.Invoke(this); } }
        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != typeof(GameEvent))
            {
                return false;
            }

            bool result = false;

            if (obj is GameEvent other)
            {
                result = true;
                result &= Handling.Equals(other.Handling);
                result &= Version.Equals(other.Version);
                result &= Id.Equals(other.Id);
                result &= Enabled.Equals(other.Enabled);
                result &= EventType.Equals(other.EventType);
                result &= GroupSubEventType.Equals(other.GroupSubEventType);
                result &= Character.Equals(other.Character);
                result &= Character2.Equals(other.Character2);
                result &= Key.Equals(other.Key);
                result &= Option.Equals(other.Option);
                result &= Option2.Equals(other.Option2);
                result &= Option3.Equals(other.Option3);
                result &= Option4.Equals(other.Option4);
                result &= Value.Equals(other.Value);
                result &= Value2.Equals(other.Value2);
                result &= SortOrder.Equals(other.SortOrder);
                result &= Delay.Equals(other.Delay);
                result &= OriginalDelay.Equals(other.OriginalDelay);
                result &= StartDelayTime.Equals(other.StartDelayTime);
                result &= UseConditions.Equals(other.UseConditions);
                result &= Criteria.Count == other.Criteria.Count;
                if (result)
                {
                    for (int i = 0; i < Criteria.Count; i++)
                    {
                        Criterion? crit = Criteria[i];
                        Criterion? crit2 = other.Criteria[i];

                        result &= crit.Equals(crit2);
                    }
                }
            }

            return result;
        }

        public static bool operator !=(GameEvent g1, GameEvent g2)
        {
            return !(g1 == g2);
        }

        public static bool operator ==(GameEvent g1, GameEvent g2)
        {
            return g1.Equals(g2);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public sealed class EventTrigger : IItem
    {
        private string id = Guid.NewGuid().ToString();
        private string characterToReactTo = IItem.hash;
        private List<Criterion> critera = [];
        private double currentIteration = 0;
        private bool enabled = true;
        private List<GameEvent> events = [];
        private string key = IItem.hash;
        private string name = string.Empty;
        private bool showInInspector = true;
        private EventTypes type = EventTypes.Never;
        private double updateIteration = 0;
        private string _value = IItem.hash;
        private LocationTargetOption locationTargetOption = LocationTargetOption.MoveTarget;
        private bool TextNeedsUpdate;
        private string _text = string.Empty;

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange = (o) =>
        {
            if (o is EventTrigger e)
            {
                e.TextNeedsUpdate = true;
            }
        };
        public string Id { get => id; set { OnBeforeChange?.Invoke(this); id = value; OnAfterChange?.Invoke(this); } }
        public string CharacterToReactTo { get => characterToReactTo; set { OnBeforeChange?.Invoke(this); if (string.IsNullOrWhiteSpace(value)) { characterToReactTo = IItem.hash; } else { characterToReactTo = value; } OnAfterChange?.Invoke(this); } }
        public List<Criterion> Critera { get => critera; set { OnBeforeChange?.Invoke(this); critera = value; OnAfterChange?.Invoke(this); } }
        public double CurrentIteration { get => currentIteration; set { OnBeforeChange?.Invoke(this); currentIteration = value; OnAfterChange?.Invoke(this); } }
        public bool Enabled { get => enabled; set { OnBeforeChange?.Invoke(this); enabled = value; OnAfterChange?.Invoke(this); } }
        public List<GameEvent> Events { get => events; set { OnBeforeChange?.Invoke(this); events = value; OnAfterChange?.Invoke(this); } }
        public string Key { get => key; set { OnBeforeChange?.Invoke(this); if (string.IsNullOrWhiteSpace(value)) { key = IItem.hash; } else { key = value; } OnAfterChange?.Invoke(this); } }
        public string Name { get => name; set { OnBeforeChange?.Invoke(this); name = value; OnAfterChange?.Invoke(this); } }
        public bool ShowInInspector { get => showInInspector; set { OnBeforeChange?.Invoke(this); showInInspector = value; OnAfterChange?.Invoke(this); } }
        public EventTypes Type { get => type; set { OnBeforeChange?.Invoke(this); type = value; OnAfterChange?.Invoke(this); } }
        public double UpdateIteration { get => updateIteration; set { OnBeforeChange?.Invoke(this); updateIteration = value; OnAfterChange?.Invoke(this); } }
        public string Value { get => _value; set { OnBeforeChange?.Invoke(this); if (string.IsNullOrWhiteSpace(value)) { _value = IItem.hash; } else { _value = value; } OnAfterChange?.Invoke(this); } }
        public LocationTargetOption LocationTargetOption { get => locationTargetOption; set { OnBeforeChange?.Invoke(this); locationTargetOption = value; OnAfterChange?.Invoke(this); } }

        public override string ToString()
        {
            if (TextNeedsUpdate)
            {
                _text = ToS();
                TextNeedsUpdate = false;
            }

            return _text;

            string ToS()
            {
                switch (Type)
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
                        return $"{Name}: {CharacterToReactTo} {Type}";
                    }
                    case EventTypes.IsBlockedByLockedDoor:
                    case EventTypes.EntersZone:
                    case EventTypes.IsAttacked:
                    case EventTypes.PeesOnItem:
                    case EventTypes.PlayerThrowsItem:
                    {
                        return $"{Name}: {CharacterToReactTo} {Type} {Value}";
                    }
                    case EventTypes.ReachesTarget:
                    {
                        return $"{Name}: {CharacterToReactTo} {Type} {LocationTargetOption} {Value}";
                    }
                    case EventTypes.GetsHitWithProjectile:
                    {
                        return $"{Name}: {CharacterToReactTo} {Type} {Key} {Value}";
                    }
                    case EventTypes.StartedIntimacyAct:
                    {
                        if (Key != SexualActs.Masturbating.ToString())
                        {
                            return $"{Name}: {Type} {Key} {CharacterToReactTo}";
                        }
                        else
                        {
                            return $"{Name}: {Type} {Key}";
                        }
                    }
                    case EventTypes.PlayerReleasesItem:
                    case EventTypes.PlayerGrabsItem:
                    {
                        return $"{Name}: {Value} {Type}";
                    }
                    case EventTypes.Periodically:
                    {
                        return $"{Name}: {Type} {UpdateIteration}";
                    }
                    case EventTypes.OnItemFunction:
                    {
                        //key is item name and value function name
                        return $"{Name}: {Type} {Key} {Value}";
                    }
                    case EventTypes.StartedUsingActionItem:
                    case EventTypes.StoppedUsingActionItem:
                    case EventTypes.PokedByVibrator:
                    case EventTypes.OnFriendshipIncreaseWith:
                    case EventTypes.OnFriendshipDecreaseWith:
                    case EventTypes.OnRomanceDecreaseWith:
                    case EventTypes.OnRomanceIncreaseWith:
                    case EventTypes.PlayerInteractsWithItem:
                    case EventTypes.OnAfterCutSceneEnds:
                    {
                        return $"{Name}: {Type} {Value}";
                    }
                    case EventTypes.PlayerInteractsWithCharacter:
                    {
                        return $"{Name}: {Type} {CharacterToReactTo}";
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
                        return $"{Name}: {Type}";
                    }
                }
            }
        }
    }

    public sealed class CharacterGroup : IItem
    {
        private string id = Guid.NewGuid().ToString();
        private string name = string.Empty;
        private bool displayInEditor = true;
        private List<string> charactersInGroup = [];

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public string Id { get => id; set { OnBeforeChange?.Invoke(this); id = value; OnAfterChange?.Invoke(this); } }
        public string Name { get => name; set { OnBeforeChange?.Invoke(this); name = value; OnAfterChange?.Invoke(this); } }
        public bool DisplayInEditor { get => displayInEditor; set { OnBeforeChange?.Invoke(this); displayInEditor = value; OnAfterChange?.Invoke(this); } }
        public List<string> CharactersInGroup { get => charactersInGroup; set { OnBeforeChange?.Invoke(this); charactersInGroup = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class MainStory : IItem
    {
        private bool allowPlayerFemale = true;
        private bool allowPlayerMale = true;
        private bool useEekDefaultItemEnableBehavior = false;
        private List<Achievement> achievements = [];
        private List<CharacterGroup> characterGroups = [];
        private List<CriteriaGroup> criteriaGroups = [];
        private List<EventTrigger> playerReactions = [];
        private List<GameEvent> gameStartEvents = [];
        private List<ItemGroup> itemGroups = [];
        private List<ItemGroupBehavior> itemGroupBehaviors = [];
        private List<InteractiveitemBehaviour> itemOverrides = [];
        private List<object> playerPeriodcBehaviors = [];
        private List<object> interactions = [];
        private List<object> opportunities = [];
        private List<object> playerReactionBehaviors = [];
        private List<object> onGameStartScripts = [];
        private List<string> playerValues = [];
        private string housePartyVersion = Main.HousePartyVersion;

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public MainStory(string name)
        {
            PlayerReactions.Add(new()
            {
                Name = "Gamestart-Gamestarts",
                Type = EventTypes.GameStarts,
                CharacterToReactTo = "Player",
                Critera = [new() { CompareType = CompareTypes.IsNewGame, BoolValue = BoolCritera.True }],
                Events = [
                    new(){
                        SortOrder = 2,
                        Character = AnybodyCharacters.Anybody.ToString(),
                        EventType = GameEvents.DisplayGameMessage,
                        Option = (int)GameMessageType.Narration,
                        Value = "Welcome to "+name+"! Have fun here :)"
                    },
                    new(){
                        SortOrder = 3,
                        EventType = GameEvents.EventTriggers,
                        Character = "Player",
                        Option = (int)TriggerOptions.PerformEvent,
                        Value = "Gamestart-EnableNPCs",
                    }
                    ]
            });
            PlayerReactions.Add(new()
            {
                Name = "Gamestart-EnableNPCs",
                Type = EventTypes.Never,
                Events = [
                    new(){
                        EventType = GameEvents.EnableNPC,
                        Character = StoryCharacters.Amy.ToString()
                    },new(){
                        EventType = GameEvents.EnableNPC,
                        Character = StoryCharacters.Ashley.ToString()
                    },new(){
                        EventType = GameEvents.EnableNPC,
                        Character = StoryCharacters.Brittney.ToString()
                    },new(){
                        EventType = GameEvents.EnableNPC,
                        Character = StoryCharacters.Derek.ToString()
                    },new(){
                        EventType = GameEvents.EnableNPC,
                        Character = StoryCharacters.Frank.ToString()
                    },new(){
                        EventType = GameEvents.EnableNPC,
                        Character = StoryCharacters.Katherine.ToString()
                    },new(){
                        EventType = GameEvents.EnableNPC,
                        Character = StoryCharacters.Leah.ToString()
                    },new(){
                        EventType = GameEvents.EnableNPC,
                        Character = StoryCharacters.Madison.ToString()
                    },new(){
                        EventType = GameEvents.EnableNPC,
                        Character = StoryCharacters.Patrick.ToString()
                    },new(){
                        EventType = GameEvents.EnableNPC,
                        Character = StoryCharacters.Rachael.ToString()
                    },new(){
                        EventType = GameEvents.EnableNPC,
                        Character = StoryCharacters.Stephanie.ToString()
                    },new(){
                        EventType = GameEvents.EnableNPC,
                        Character = StoryCharacters.Vickie.ToString()
                    },
                    ]
            });

        }
        public MainStory() { }
        public bool AllowPlayerFemale { get => allowPlayerFemale; set { OnBeforeChange?.Invoke(this); allowPlayerFemale = value; OnAfterChange?.Invoke(this); } }
        public bool AllowPlayerMale { get => allowPlayerMale; set { OnBeforeChange?.Invoke(this); allowPlayerMale = value; OnAfterChange?.Invoke(this); } }
        public bool UseEekDefaultItemEnableBehavior { get => useEekDefaultItemEnableBehavior; set { OnBeforeChange?.Invoke(this); useEekDefaultItemEnableBehavior = value; OnAfterChange?.Invoke(this); } }
        public List<Achievement> Achievements { get => achievements; set { OnBeforeChange?.Invoke(this); achievements = value; OnAfterChange?.Invoke(this); } }
        public List<CharacterGroup> CharacterGroups { get => characterGroups; set { OnBeforeChange?.Invoke(this); characterGroups = value; OnAfterChange?.Invoke(this); } }
        public List<CriteriaGroup> CriteriaGroups { get => criteriaGroups; set { OnBeforeChange?.Invoke(this); criteriaGroups = value; OnAfterChange?.Invoke(this); } }
        public List<EventTrigger> PlayerReactions { get => playerReactions; set { OnBeforeChange?.Invoke(this); playerReactions = value; OnAfterChange?.Invoke(this); } }
        public List<GameEvent> GameStartEvents { get => gameStartEvents; set { OnBeforeChange?.Invoke(this); gameStartEvents = value; OnAfterChange?.Invoke(this); } }
        public List<ItemGroup> ItemGroups { get => itemGroups; set { OnBeforeChange?.Invoke(this); itemGroups = value; OnAfterChange?.Invoke(this); } }
        public List<ItemGroupBehavior> ItemGroupBehaviors { get => itemGroupBehaviors; set { OnBeforeChange?.Invoke(this); itemGroupBehaviors = value; OnAfterChange?.Invoke(this); } }
        public List<InteractiveitemBehaviour> ItemOverrides { get => itemOverrides; set { OnBeforeChange?.Invoke(this); itemOverrides = value; OnAfterChange?.Invoke(this); } }
        public List<object> PlayerPeriodcBehaviors { get => playerPeriodcBehaviors; set { OnBeforeChange?.Invoke(this); playerPeriodcBehaviors = value; OnAfterChange?.Invoke(this); } }
        public List<object> Interactions { get => interactions; set { OnBeforeChange?.Invoke(this); interactions = value; OnAfterChange?.Invoke(this); } }
        public List<object> Opportunities { get => opportunities; set { OnBeforeChange?.Invoke(this); opportunities = value; OnAfterChange?.Invoke(this); } }
        public List<object> PlayerReactionBehaviors { get => playerReactionBehaviors; set { OnBeforeChange?.Invoke(this); playerReactionBehaviors = value; OnAfterChange?.Invoke(this); } }
        public List<object> OnGameStartScripts { get => onGameStartScripts; set { OnBeforeChange?.Invoke(this); onGameStartScripts = value; OnAfterChange?.Invoke(this); } }
        public List<string> PlayerValues { get => playerValues; set { OnBeforeChange?.Invoke(this); playerValues = value; OnAfterChange?.Invoke(this); } }
        public string HousePartyVersion { get => housePartyVersion; set { OnBeforeChange?.Invoke(this); housePartyVersion = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class AlternateText : IItem
    {
        private int order = 0;
        private List<Criterion> critera = [];
        private bool show = true;
        private string text = IItem.hash;

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public List<Criterion> Critera { get => critera; set { OnBeforeChange?.Invoke(this); critera = value; OnAfterChange?.Invoke(this); } }
        public int Order { get => order; set { OnBeforeChange?.Invoke(this); order = value; OnAfterChange?.Invoke(this); } }
        public bool Show { get => show; set { OnBeforeChange?.Invoke(this); show = value; OnAfterChange?.Invoke(this); } }
        public string Text { get => text; set { OnBeforeChange?.Invoke(this); text = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class Response : IItem
    {
        private bool selected = false;
        private string id = Guid.NewGuid().ToString();
        private bool alwaysDisplay = false;
        private int next = 0;
        private int order = 0;
        private List<Criterion> responseCriteria = [];
        private List<GameEvent> responseEvents = [];
        private bool testingCriteraOverride = false;
        private string text = string.Empty;
        private ResponseReactionTypes currentNPCReaction = ResponseReactionTypes.Neutral;
        private bool show = true;
        private bool showResponseCriteria = false;
        private bool showResponseEvents = false;
        private bool isDynamic = false;
        private bool showDynamicNegativeCritera = false;
        private bool showDynamicPositiveCritera = false;
        private bool showTopics = false;
        private bool showTones = false;
        private bool showTypes = false;
        private ResponseTypes type = ResponseTypes.Generic;
        private List<ConversationalTopics> topics = [];
        private List<ResponseTones> tones = [];
        private List<Criterion> dynamicPositiveCritera = [];
        private List<Criterion> dynamicNegativeCritera = [];

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public bool Selected { get => selected; set { OnBeforeChange?.Invoke(this); selected = value; OnAfterChange?.Invoke(this); } }
        public string Id { get => id; set { OnBeforeChange?.Invoke(this); id = value; OnAfterChange?.Invoke(this); } }
        public bool AlwaysDisplay { get => alwaysDisplay; set { OnBeforeChange?.Invoke(this); alwaysDisplay = value; OnAfterChange?.Invoke(this); } }
        public int Next { get => next; set { OnBeforeChange?.Invoke(this); next = value; OnAfterChange?.Invoke(this); } }
        public int Order { get => order; set { OnBeforeChange?.Invoke(this); order = value; OnAfterChange?.Invoke(this); } }
        public List<Criterion> ResponseCriteria { get => responseCriteria; set { OnBeforeChange?.Invoke(this); responseCriteria = value; OnAfterChange?.Invoke(this); } }
        public List<GameEvent> ResponseEvents { get => responseEvents; set { OnBeforeChange?.Invoke(this); responseEvents = value; OnAfterChange?.Invoke(this); } }
        public bool TestingCriteraOverride { get => testingCriteraOverride; set { OnBeforeChange?.Invoke(this); testingCriteraOverride = value; OnAfterChange?.Invoke(this); } }
        public string Text { get => text; set { OnBeforeChange?.Invoke(this); text = value; OnAfterChange?.Invoke(this); } }
        public ResponseReactionTypes CurrentNPCReaction { get => currentNPCReaction; set { OnBeforeChange?.Invoke(this); currentNPCReaction = value; OnAfterChange?.Invoke(this); } }
        public bool Show { get => show; set { OnBeforeChange?.Invoke(this); show = value; OnAfterChange?.Invoke(this); } }
        public bool ShowResponseCriteria { get => showResponseCriteria; set { OnBeforeChange?.Invoke(this); showResponseCriteria = value; OnAfterChange?.Invoke(this); } }
        public bool ShowResponseEvents { get => showResponseEvents; set { OnBeforeChange?.Invoke(this); showResponseEvents = value; OnAfterChange?.Invoke(this); } }
        public bool IsDynamic { get => isDynamic; set { OnBeforeChange?.Invoke(this); isDynamic = value; OnAfterChange?.Invoke(this); } }
        public bool ShowDynamicNegativeCritera { get => showDynamicNegativeCritera; set { OnBeforeChange?.Invoke(this); showDynamicNegativeCritera = value; OnAfterChange?.Invoke(this); } }
        public bool ShowDynamicPositiveCritera { get => showDynamicPositiveCritera; set { OnBeforeChange?.Invoke(this); showDynamicPositiveCritera = value; OnAfterChange?.Invoke(this); } }
        public bool ShowTopics { get => showTopics; set { OnBeforeChange?.Invoke(this); showTopics = value; OnAfterChange?.Invoke(this); } }
        public bool ShowTones { get => showTones; set { OnBeforeChange?.Invoke(this); showTones = value; OnAfterChange?.Invoke(this); } }
        public bool ShowTypes { get => showTypes; set { OnBeforeChange?.Invoke(this); showTypes = value; OnAfterChange?.Invoke(this); } }
        public ResponseTypes Type { get => type; set { OnBeforeChange?.Invoke(this); type = value; OnAfterChange?.Invoke(this); } }
        public List<ConversationalTopics> Topics { get => topics; set { OnBeforeChange?.Invoke(this); topics = value; OnAfterChange?.Invoke(this); } }
        public List<ResponseTones> Tones { get => tones; set { OnBeforeChange?.Invoke(this); tones = value; OnAfterChange?.Invoke(this); } }
        public List<Criterion> DynamicPositiveCritera { get => dynamicPositiveCritera; set { OnBeforeChange?.Invoke(this); dynamicPositiveCritera = value; OnAfterChange?.Invoke(this); } }
        public List<Criterion> DynamicNegativeCritera { get => dynamicNegativeCritera; set { OnBeforeChange?.Invoke(this); dynamicNegativeCritera = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class Dialogue : IItem
    {
        private bool shown = false;
        private List<AlternateText> alternateTexts = [];
        private List<GameEvent> closeEvents = [];
        private int iD = 0;
        private bool important = false;
        private List<Response> responses = [];
        private bool showGlobalGoodByeResponses = true;
        private bool autoImmersive = false;
        private bool showGlobalResponses = true;
        private bool doesNotCountAsMet = false;
        private bool showResponses = true;
        private string speakingToCharacterName = Main.Player;
        private string currentSpeaker = string.Empty;
        private List<GameEvent> startEvents = [];
        private string text = string.Empty;

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public bool Shown { get => shown; set { OnBeforeChange?.Invoke(this); shown = value; OnAfterChange?.Invoke(this); } }
        public List<AlternateText> AlternateTexts { get => alternateTexts; set { OnBeforeChange?.Invoke(this); alternateTexts = value; OnAfterChange?.Invoke(this); } }
        public List<GameEvent> CloseEvents { get => closeEvents; set { OnBeforeChange?.Invoke(this); closeEvents = value; OnAfterChange?.Invoke(this); } }
        public int ID { get => iD; set { OnBeforeChange?.Invoke(this); iD = value; OnAfterChange?.Invoke(this); } }
        public bool Important { get => important; set { OnBeforeChange?.Invoke(this); important = value; OnAfterChange?.Invoke(this); } }
        public List<Response> Responses { get => responses; set { OnBeforeChange?.Invoke(this); responses = value; OnAfterChange?.Invoke(this); } }
        public bool ShowGlobalGoodByeResponses { get => showGlobalGoodByeResponses; set { OnBeforeChange?.Invoke(this); showGlobalGoodByeResponses = value; OnAfterChange?.Invoke(this); } }
        public bool AutoImmersive { get => autoImmersive; set { OnBeforeChange?.Invoke(this); autoImmersive = value; OnAfterChange?.Invoke(this); } }
        public bool ShowGlobalResponses { get => showGlobalResponses; set { OnBeforeChange?.Invoke(this); showGlobalResponses = value; OnAfterChange?.Invoke(this); } }
        public bool DoesNotCountAsMet { get => doesNotCountAsMet; set { OnBeforeChange?.Invoke(this); doesNotCountAsMet = value; OnAfterChange?.Invoke(this); } }
        public bool ShowResponses { get => showResponses; set { OnBeforeChange?.Invoke(this); showResponses = value; OnAfterChange?.Invoke(this); } }
        public string SpeakingToCharacterName { get => speakingToCharacterName; set { OnBeforeChange?.Invoke(this); speakingToCharacterName = value; OnAfterChange?.Invoke(this); } }
        public string CurrentSpeaker { get => currentSpeaker; set { OnBeforeChange?.Invoke(this); currentSpeaker = value; OnAfterChange?.Invoke(this); } }
        public List<GameEvent> StartEvents { get => startEvents; set { OnBeforeChange?.Invoke(this); startEvents = value; OnAfterChange?.Invoke(this); } }
        public string Text { get => text; set { OnBeforeChange?.Invoke(this); text = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class BackgroundChatter : IItem
    {
        private int id = 0;
        private string text = string.Empty;
        private List<Criterion> critera = [];
        private bool isConversationStarter = false;
        private bool showInInspector = true;
        private bool playSilently = false;
        private string label = IItem.hash;
        private string speakingTo = AnybodyCharacters.Anybody.ToString();
        private bool overrideCombatRestriction = false;
        private List<GameEvent> startEvents = [];
        private List<BackgroundChatterResponse> responses = [];
        private BGCEmotes pairedEmote = BGCEmotes.None;
        private Importance defaultImportance = Importance.None;
        private Importance currentImportance = Importance.None;

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public int Id { get => id; set { OnBeforeChange?.Invoke(this); id = value; OnAfterChange?.Invoke(this); } }
        public string Text { get => text; set { OnBeforeChange?.Invoke(this); text = value; OnAfterChange?.Invoke(this); } }
        public List<Criterion> Critera { get => critera; set { OnBeforeChange?.Invoke(this); critera = value; OnAfterChange?.Invoke(this); } }
        public bool IsConversationStarter { get => isConversationStarter; set { OnBeforeChange?.Invoke(this); isConversationStarter = value; OnAfterChange?.Invoke(this); } }
        public bool ShowInInspector { get => showInInspector; set { OnBeforeChange?.Invoke(this); showInInspector = value; OnAfterChange?.Invoke(this); } }
        public bool PlaySilently { get => playSilently; set { OnBeforeChange?.Invoke(this); playSilently = value; OnAfterChange?.Invoke(this); } }
        public string Label { get => label; set { OnBeforeChange?.Invoke(this); label = value; OnAfterChange?.Invoke(this); } }
        public string SpeakingTo { get => speakingTo; set { OnBeforeChange?.Invoke(this); speakingTo = value; OnAfterChange?.Invoke(this); } }
        public bool OverrideCombatRestriction { get => overrideCombatRestriction; set { OnBeforeChange?.Invoke(this); overrideCombatRestriction = value; OnAfterChange?.Invoke(this); } }
        public List<GameEvent> StartEvents { get => startEvents; set { OnBeforeChange?.Invoke(this); startEvents = value; OnAfterChange?.Invoke(this); } }
        public List<BackgroundChatterResponse> Responses { get => responses; set { OnBeforeChange?.Invoke(this); responses = value; OnAfterChange?.Invoke(this); } }
        public BGCEmotes PairedEmote { get => pairedEmote; set { OnBeforeChange?.Invoke(this); pairedEmote = value; OnAfterChange?.Invoke(this); } }
        public Importance DefaultImportance { get => defaultImportance; set { OnBeforeChange?.Invoke(this); defaultImportance = value; OnAfterChange?.Invoke(this); } }
        public Importance CurrentImportance { get => currentImportance; set { OnBeforeChange?.Invoke(this); currentImportance = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class BackgroundChatterResponse : IItem
    {
        private string characterName = IItem.hash;
        private int chatterId = 0;
        private string label = Guid.NewGuid().ToString();
        private bool showInInspector = true;

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;

        public string CharacterName { get => characterName; set { OnBeforeChange?.Invoke(this); characterName = value; OnAfterChange?.Invoke(this); } }
        public string Label { get => label; set { OnBeforeChange?.Invoke(this); label = value; OnAfterChange?.Invoke(this); } }
        public int ChatterId { get => chatterId; set { OnBeforeChange?.Invoke(this); chatterId = value; OnAfterChange?.Invoke(this); } }
        public bool ShowInInspector { get => showInInspector; set { OnBeforeChange?.Invoke(this); showInInspector = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class Trait : IItem
    {
        private PersonalityTraits type = PersonalityTraits.Nice;
        private int _value = 0;

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public PersonalityTraits Type { get => type; set { OnBeforeChange?.Invoke(this); type = value; OnAfterChange?.Invoke(this); } }
        public int Value { get => _value; set { OnBeforeChange?.Invoke(this); _value = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class Personality : IItem
    {
        private List<Trait> values = [];

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public List<Trait> Values { get => values; set { OnBeforeChange?.Invoke(this); values = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class ExtendedDetail : IItem
    {
        private int _value = 0;
        private string details = string.Empty;

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public int Value { get => _value; set { OnBeforeChange?.Invoke(this); _value = value; OnAfterChange?.Invoke(this); } }
        public string Details { get => details; set { OnBeforeChange?.Invoke(this); details = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class Quest : IItem
    {
        private string characterName = IItem.hash;
        private int completeAt = 0;
        private int currentValue = 0;
        private string details = string.Empty;
        private string completedDetails = string.Empty;
        private string failedDetails = string.Empty;
        private List<ExtendedDetail> extendedDetails = [];
        private string iD = Guid.NewGuid().ToString();
        private string name = string.Empty;
        private bool obtainOnStart = false;
        private bool seenByPlayer = false;
        private bool showProgress = false;
        private QuestStatus status = QuestStatus.NotObtained;
        private int obtainedDateTime = 0;
        private int lastUpdatedDateTime = 0;
        private bool showInInspector = true;

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public string CharacterName { get => characterName; set { OnBeforeChange?.Invoke(this); characterName = value; OnAfterChange?.Invoke(this); } }
        public int CompleteAt { get => completeAt; set { OnBeforeChange?.Invoke(this); completeAt = value; OnAfterChange?.Invoke(this); } }
        public int CurrentValue { get => currentValue; set { OnBeforeChange?.Invoke(this); currentValue = value; OnAfterChange?.Invoke(this); } }
        public string Details { get => details; set { OnBeforeChange?.Invoke(this); details = value; OnAfterChange?.Invoke(this); } }
        public string CompletedDetails { get => completedDetails; set { OnBeforeChange?.Invoke(this); completedDetails = value; OnAfterChange?.Invoke(this); } }
        public string FailedDetails { get => failedDetails; set { OnBeforeChange?.Invoke(this); failedDetails = value; OnAfterChange?.Invoke(this); } }
        public List<ExtendedDetail> ExtendedDetails { get => extendedDetails; set { OnBeforeChange?.Invoke(this); extendedDetails = value; OnAfterChange?.Invoke(this); } }
        public string ID { get => iD; set { OnBeforeChange?.Invoke(this); iD = value; OnAfterChange?.Invoke(this); } }
        public string Name { get => name; set { OnBeforeChange?.Invoke(this); name = value; OnAfterChange?.Invoke(this); } }
        public bool ObtainOnStart { get => obtainOnStart; set { OnBeforeChange?.Invoke(this); obtainOnStart = value; OnAfterChange?.Invoke(this); } }
        public bool SeenByPlayer { get => seenByPlayer; set { OnBeforeChange?.Invoke(this); seenByPlayer = value; OnAfterChange?.Invoke(this); } }
        public bool ShowProgress { get => showProgress; set { OnBeforeChange?.Invoke(this); showProgress = value; OnAfterChange?.Invoke(this); } }
        public QuestStatus Status { get => status; set { OnBeforeChange?.Invoke(this); status = value; OnAfterChange?.Invoke(this); } }
        public int ObtainedDateTime { get => obtainedDateTime; set { OnBeforeChange?.Invoke(this); obtainedDateTime = value; OnAfterChange?.Invoke(this); } }
        public int LastUpdatedDateTime { get => lastUpdatedDateTime; set { OnBeforeChange?.Invoke(this); lastUpdatedDateTime = value; OnAfterChange?.Invoke(this); } }
        public bool ShowInInspector { get => showInInspector; set { OnBeforeChange?.Invoke(this); showInInspector = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class ItemInteraction : IItem
    {
        private List<Criterion> critera = [];
        private string itemName = string.Empty;
        private List<GameEvent> onAcceptEvents = [];
        private List<GameEvent> onRefuseEvents = [];
        private bool displayInEditor = true;

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public List<Criterion> Critera { get => critera; set { OnBeforeChange?.Invoke(this); critera = value; OnAfterChange?.Invoke(this); } }
        public string ItemName { get => itemName; set { OnBeforeChange?.Invoke(this); itemName = value; OnAfterChange?.Invoke(this); } }
        public List<GameEvent> OnAcceptEvents { get => onAcceptEvents; set { OnBeforeChange?.Invoke(this); onAcceptEvents = value; OnAfterChange?.Invoke(this); } }
        public List<GameEvent> OnRefuseEvents { get => onRefuseEvents; set { OnBeforeChange?.Invoke(this); onRefuseEvents = value; OnAfterChange?.Invoke(this); } }
        public bool DisplayInEditor { get => displayInEditor; set { OnBeforeChange?.Invoke(this); displayInEditor = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class ItemGroupInteraction : IItem
    {
        private List<Criterion> criteria = [];
        private string name = string.Empty;
        private string groupName = string.Empty;
        private string id = Guid.NewGuid().ToString();
        private List<GameEvent> onAcceptEvents = [];
        private List<GameEvent> onRefuseEvents = [];
        private bool displayInEditor = true;

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public List<Criterion> Criteria { get => criteria; set { OnBeforeChange?.Invoke(this); criteria = value; OnAfterChange?.Invoke(this); } }
        public string Name { get => name; set { OnBeforeChange?.Invoke(this); name = value; OnAfterChange?.Invoke(this); } }
        public string GroupName { get => groupName; set { OnBeforeChange?.Invoke(this); groupName = value; OnAfterChange?.Invoke(this); } }
        public string Id { get => id; set { OnBeforeChange?.Invoke(this); id = value; OnAfterChange?.Invoke(this); } }
        public List<GameEvent> OnAcceptEvents { get => onAcceptEvents; set { OnBeforeChange?.Invoke(this); onAcceptEvents = value; OnAfterChange?.Invoke(this); } }
        public List<GameEvent> OnRefuseEvents { get => onRefuseEvents; set { OnBeforeChange?.Invoke(this); onRefuseEvents = value; OnAfterChange?.Invoke(this); } }
        public bool DisplayInEditor { get => displayInEditor; set { OnBeforeChange?.Invoke(this); displayInEditor = value; OnAfterChange?.Invoke(this); } }
    }

    public sealed class CharacterStory : IItem
    {
        private int dialogueID = 0;
        private List<BackgroundChatter> backgroundChatter = [];
        private List<Dialogue> dialogues = [];
        private List<EventTrigger> reactions = [];
        private List<ItemGroupInteraction> characterItemGroupInteractions = [];
        private List<Quest> quests = [];
        private List<Response> globalGoodbyeResponses = [];
        private List<Response> globalResponses = [];
        private List<ItemInteraction> storyItems = [];
        private List<string> storyValues = [];
        private Personality personality = new();
        private string characterName = IItem.hash;
        private List<object> personalityPrompts = [];
        private List<object> dialogue = [];
        private List<object> periodicBehaviors = [];
        private List<object> reactionBehaviors = [];
        private string housePartyVersion = Main.HousePartyVersion;

        public event Action<object>? OnBeforeChange;
        public event Action<object>? OnAfterChange;
        public CharacterStory(string name)
        {
            CharacterName = name;
            foreach (var trait in Enum.GetValues<PersonalityTraits>())
            {
                Personality.Values.Add(new Trait() { Type = trait, Value = 0 });
            }

            var resp = new Response() { Text = "Goodbye " + name + "!" };

            Dialogues.Add(new Dialogue()
            {
                Text = "Hello, i'm " + name + " :D I was made in the CCSC",
                ShowGlobalResponses = true,
                ShowGlobalGoodByeResponses = false,
                SpeakingToCharacterName = "Player"
            });
            GlobalResponses.Add(resp);

            Reactions.Add(new EventTrigger()
            {
                CharacterToReactTo = name,
                Name = name + "-GameStart",
                Type = EventTypes.GameStarts,
                Events = [new GameEvent() {
                    Character = name,
                    EventType = GameEvents.WarpTo,
                    Option = (int)WarpToOption.MoveTarget,
                    Value = MoveTargets.Kitchen.ToString(),
                    }],
                Critera = [new Criterion() {
                    CompareType = CompareTypes.IsNewGame,
                    BoolValue = BoolCritera.True,
                    ItemComparison = ItemComparisonTypes.IsActive,
                }]
            });
        }
        public CharacterStory()
        {
        }

        public int DialogueID { get => dialogueID; set { OnBeforeChange?.Invoke(this); dialogueID = value; OnAfterChange?.Invoke(this); } }
        public List<BackgroundChatter> BackgroundChatter { get => backgroundChatter; set { OnBeforeChange?.Invoke(this); backgroundChatter = value; OnAfterChange?.Invoke(this); } }
        public List<Dialogue> Dialogues { get => dialogues; set { OnBeforeChange?.Invoke(this); dialogues = value; OnAfterChange?.Invoke(this); } }
        public List<EventTrigger> Reactions { get => reactions; set { OnBeforeChange?.Invoke(this); reactions = value; OnAfterChange?.Invoke(this); } }
        public List<ItemGroupInteraction> CharacterItemGroupInteractions { get => characterItemGroupInteractions; set { OnBeforeChange?.Invoke(this); characterItemGroupInteractions = value; OnAfterChange?.Invoke(this); } }
        public List<Quest> Quests { get => quests; set { OnBeforeChange?.Invoke(this); quests = value; OnAfterChange?.Invoke(this); } }
        public List<Response> GlobalGoodbyeResponses { get => globalGoodbyeResponses; set { OnBeforeChange?.Invoke(this); globalGoodbyeResponses = value; OnAfterChange?.Invoke(this); } }
        public List<Response> GlobalResponses { get => globalResponses; set { OnBeforeChange?.Invoke(this); globalResponses = value; OnAfterChange?.Invoke(this); } }
        public List<ItemInteraction> StoryItems { get => storyItems; set { OnBeforeChange?.Invoke(this); storyItems = value; OnAfterChange?.Invoke(this); } }
        public List<string> StoryValues { get => storyValues; set { OnBeforeChange?.Invoke(this); storyValues = value; OnAfterChange?.Invoke(this); } }
        public Personality Personality { get => personality; set { OnBeforeChange?.Invoke(this); personality = value; OnAfterChange?.Invoke(this); } }
        public string CharacterName { get => characterName; set { OnBeforeChange?.Invoke(this); characterName = value; OnAfterChange?.Invoke(this); } }
        public List<object> PersonalityPrompts { get => personalityPrompts; set { OnBeforeChange?.Invoke(this); personalityPrompts = value; OnAfterChange?.Invoke(this); } }
        public List<object> Dialogue { get => dialogue; set { OnBeforeChange?.Invoke(this); dialogue = value; OnAfterChange?.Invoke(this); } }
        public List<object> PeriodicBehaviors { get => periodicBehaviors; set { OnBeforeChange?.Invoke(this); periodicBehaviors = value; OnAfterChange?.Invoke(this); } }
        public List<object> ReactionBehaviors { get => reactionBehaviors; set { OnBeforeChange?.Invoke(this); reactionBehaviors = value; OnAfterChange?.Invoke(this); } }
        public string HousePartyVersion { get => housePartyVersion; set { OnBeforeChange?.Invoke(this); housePartyVersion = value; OnAfterChange?.Invoke(this); } }
    }
}