using System.Text.Json.Serialization;
using static CSC.StoryItems.StoryEnums;

namespace CSC.StoryItems
{
    public sealed class Criterion
    {
        public BoolCritera BoolValue { get; set; } = BoolCritera.True;

        public string Character { get; set; } = "#";

        public string Character2 { get; set; } = "#";
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CompareTypes CompareType { get; set; } = CompareTypes.Never;
        public DialogueStatuses DialogueStatus { get; set; } = DialogueStatuses.WasShown;
        public bool DisplayInEditor { get; set; } = true;
        public DoorOptionValues DoorOptions { get; set; } = DoorOptionValues.IsClosed;
        public EqualsValues EqualsValue { get; set; } = EqualsValues.Equals;
        public ComparisonEquations EquationValue { get; set; } = ComparisonEquations.Equals;
        public ValueSpecificFormulas ValueFormula { get; set; } = ValueSpecificFormulas.EqualsValue;
        public ItemComparisonTypes ItemComparison { get; set; } = ItemComparisonTypes.IsActive;
        public ItemFromItemGroupComparisonTypes ItemFromItemGroupComparison { get; set; } = ItemFromItemGroupComparisonTypes.IsActive;

        public string Key { get; set; } = "#";

        public string Key2 { get; set; } = "#";
        public int Order { get; set; } = 0;
        public PlayerInventoryOptions PlayerInventoryOption { get; set; } = PlayerInventoryOptions.HasItem;
        public PoseOptions PoseOption { get; set; } = PoseOptions.IsCurrentlyPosing;
        public SocialStatuses SocialStatus { get; set; } = SocialStatuses.Drunk;

        public string Value { get; set; } = "#";
        public int Option { get; set; } = 0;

        [JsonConverter(typeof(JsonNumberEnumConverter<CompareTypes>))]
        public CompareTypes GroupSubCompareType { get; set; } = CompareTypes.Never;
        public string Version { get; set; } = "1.0";
    }

    public sealed class ItemAction
    {
        public string ActionName { get; set; } = string.Empty;
        public List<Criterion> Criteria { get; set; } = [];
        public bool DisplayInEditor { get; set; } = true;
        public List<GameEvent> OnTakeActionEvents { get; set; } = [];
    }

    public sealed class UseWith
    {
        public List<Criterion> Criteria { get; set; } = [];

        public string CustomCantDoThatMessage { get; set; } = string.Empty;
        public bool DisplayInEditor { get; set; } = true;

        public string ItemName { get; set; } = string.Empty;
        public List<GameEvent> OnSuccessEvents { get; set; } = [];
    }

    public sealed class InteractiveitemBehaviour
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool DisplayInEditor { get; set; } = true;

        public string DisplayName { get; set; } = string.Empty;
        public List<ItemAction> ItemActions { get; set; } = [];

        public string ItemName { get; set; } = string.Empty;
        public List<UseWith> UseWiths { get; set; } = [];
        public bool UseDefaultRadialOptions { get; set; } = true;
    }

    public sealed class ItemGroupBehavior
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = string.Empty;

        public string GroupName { get; set; } = string.Empty;
        public bool DisplayInEditor { get; set; } = true;
        public List<ItemAction> ItemActions { get; set; } = [];
        public List<UseWith> UseWiths { get; set; } = [];
    }

    public sealed class Achievement
    {
        public string Description { get; set; } = string.Empty;

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Image { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public bool ShowInEditor { get; set; } = true;

        public string SteamName { get; set; } = string.Empty;
    }

    public sealed class CriteriaListWrapper
    {
        public List<Criterion> CriteriaList { get; set; } = [];
    }

    public sealed class CriteriaGroup
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = string.Empty;

        public string LinkedGroupName { get; set; } = string.Empty;
        public bool DisplayInEditor { get; set; } = true;
        public PassCondition PassCondition { get; set; } = PassCondition.AnySetIsTrue;
        public List<CriteriaListWrapper> CriteriaList { get; set; } = [new CriteriaListWrapper()];
    }

    public sealed class ItemGroup
    {

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = string.Empty;
        public bool DisplayInEditor { get; set; } = true;
        public List<string> ItemsInGroup { get; set; } = [];
    }

    public sealed class GameEvent
    {
        public EventSpecialHandling Handling { get; set; } = EventSpecialHandling.None;

        public string Version { get; set; } = "2.0";

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool Enabled { get; set; } = true;
        public GameEvents EventType { get; set; } = GameEvents.None;
        public GameEvents GroupSubEventType { get; set; } = GameEvents.None;

        public string Character { get; set; } = "#";

        public string Character2 { get; set; } = "#";

        public string Key { get; set; } = "#";
        public int Option { get; set; } = 0;
        public int Option2 { get; set; } = 0;
        public int Option3 { get; set; } = 0;
        public int Option4 { get; set; } = 0;
        public string Value { get; set; } = string.Empty;
        public string Value2 { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
        public double Delay { get; set; } = 0;
        public double OriginalDelay { get; set; } = 0;
        public double StartDelayTime { get; set; } = 0;
        public bool UseConditions { get; set; } = false;
        public bool DisplayInEditor { get; set; } = true;
        public List<Criterion> Criteria { get; set; } = [];
    }

    public sealed class EventTrigger
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string CharacterToReactTo { get; set; } = "#";
        public List<Criterion> Critera { get; set; } = [];
        public double CurrentIteration { get; set; } = 0;
        public bool Enabled { get; set; } = true;
        public List<GameEvent> Events { get; set; } = [];

        public string Key { get; set; } = "#";

        public string Name { get; set; } = string.Empty;
        public bool ShowInInspector { get; set; } = true;
        public EventTypes Type { get; set; } = EventTypes.Never;
        public double UpdateIteration { get; set; } = 0;

        public string Value { get; set; } = "#";

        public LocationTargetOption LocationTargetOption { get; set; } = LocationTargetOption.MoveTarget;
    }

    public sealed class CharacterGroup
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = string.Empty;
        public bool DisplayInEditor { get; set; } = true;
        public List<string> CharactersInGroup { get; set; } = [];
    }

    public sealed class MainStory
    {
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
        public bool AllowPlayerFemale { get; set; } = true;
        public bool AllowPlayerMale { get; set; } = true;
        public bool UseEekDefaultItemEnableBehavior { get; set; } = false;
        public List<Achievement> Achievements { get; set; } = [];
        public List<CharacterGroup> CharacterGroups { get; set; } = [];
        public List<CriteriaGroup> CriteriaGroups { get; set; } = [];
        public List<EventTrigger> PlayerReactions { get; set; } = [];
        public List<GameEvent> GameStartEvents { get; set; } = [];
        public List<ItemGroup> ItemGroups { get; set; } = [];
        public List<ItemGroupBehavior> ItemGroupBehaviors { get; set; } = [];
        public List<InteractiveitemBehaviour> ItemOverrides { get; set; } = [];
        public List<object> PlayerPeriodcBehaviors { get; set; } = [];
        public List<object> Interactions { get; set; } = [];
        public List<object> Opportunities { get; set; } = [];
        public List<object> PlayerReactionBehaviors { get; set; } = [];

        public List<object> OnGameStartScripts { get; set; } = [];
        public List<string> PlayerValues { get; set; } = [];

        public string HousePartyVersion { get; set; } = Main.HousePartyVersion;
    }

    public sealed class AlternateText
    {
        public List<Criterion> Critera { get; set; } = [];
        public int Order { get; set; } = 0;
        public bool Show { get; set; } = true;

        public string Text { get; set; } = "#";
    }

    public sealed class Response
    {
        public bool Selected { get; set; } = false;

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool AlwaysDisplay { get; set; } = false;
        public int Next { get; set; } = 0;
        public int Order { get; set; } = 0;
        public List<Criterion> ResponseCriteria { get; set; } = [];
        public List<GameEvent> ResponseEvents { get; set; } = [];
        public bool TestingCriteraOverride { get; set; } = false;

        public string Text { get; set; } = string.Empty;
        public ResponseReactionTypes CurrentNPCReaction { get; set; } = ResponseReactionTypes.Neutral;
        public bool Show { get; set; } = true;
        public bool ShowResponseCriteria { get; set; } = false;
        public bool ShowResponseEvents { get; set; } = false;
        public bool IsDynamic { get; set; } = false;
        public bool ShowDynamicNegativeCritera { get; set; } = false;
        public bool ShowDynamicPositiveCritera { get; set; } = false;
        public bool ShowTopics { get; set; } = false;
        public bool ShowTones { get; set; } = false;
        public bool ShowTypes { get; set; } = false;
        public ResponseTypes Type { get; set; } = ResponseTypes.Generic;
        public List<ConversationalTopics> Topics { get; set; } = [];
        public List<ResponseTones> Tones { get; set; } = [];
        public List<Criterion> DynamicPositiveCritera { get; set; } = [];
        public List<Criterion> DynamicNegativeCritera { get; set; } = [];
    }

    public sealed class Dialogue
    {
        public bool Shown { get; set; } = false;
        public List<AlternateText> AlternateTexts { get; set; } = [];
        public List<GameEvent> CloseEvents { get; set; } = [];
        public int ID { get; set; } = 0;
        public bool Important { get; set; } = false;
        public List<Response> Responses { get; set; } = [];
        public bool ShowGlobalGoodByeResponses { get; set; } = true;
        public bool AutoImmersive { get; set; } = false;
        public bool ShowGlobalResponses { get; set; } = true;
        public bool DoesNotCountAsMet { get; set; } = false;
        public bool ShowResponses { get; set; } = true;
        public string SpeakingToCharacterName { get; set; } = Main.Player;
        public string CurrentSpeaker { get; set; } = string.Empty;
        public List<GameEvent> StartEvents { get; set; } = [];

        public string Text { get; set; } = string.Empty;
    }

    public sealed class BackgroundChatter
    {
        public int Id { get; set; } = 0;

        public string Text { get; set; } = string.Empty;
        public List<Criterion> Critera { get; set; } = [];
        public bool IsConversationStarter { get; set; } = false;
        public bool ShowInInspector { get; set; } = true;
        public bool PlaySilently { get; set; } = false;

        public string Label { get; set; } = "#";

        public string SpeakingTo { get; set; } = AnybodyCharacters.Anybody.ToString();
        public bool OverrideCombatRestriction { get; set; } = false;
        public List<GameEvent> StartEvents { get; set; } = [];
        public List<BackgroundChatterResponse> Responses { get; set; } = [];

        public BGCEmotes PairedEmote { get; set; } = BGCEmotes.None;
        public Importance DefaultImportance { get; set; } = Importance.None;
        public Importance CurrentImportance { get; set; } = Importance.None;
    }

    public sealed class BackgroundChatterResponse
    {

        public string CharacterName { get; set; } = "#";

        public string Label { get; set; } = Guid.NewGuid().ToString();
        public int ChatterId { get; set; } = 0;
        public bool ShowInInspector { get; set; } = true;
    }

    public sealed class Trait
    {
        public PersonalityTraits Type { get; set; } = PersonalityTraits.Nice;
        public int Value { get; set; } = 0;
    }

    public sealed class Personality
    {
        public List<Trait> Values { get; set; } = [];
    }

    public sealed class ExtendedDetail
    {
        public int Value { get; set; } = 0;

        public string Details { get; set; } = string.Empty;
    }

    public sealed class Quest
    {
        public string CharacterName { get; set; } = "#";
        public int CompleteAt { get; set; } = 0;
        public int CurrentValue { get; set; } = 0;

        public string Details { get; set; } = string.Empty;

        public string CompletedDetails { get; set; } = string.Empty;

        public string FailedDetails { get; set; } = string.Empty;
        public List<ExtendedDetail> ExtendedDetails { get; set; } = [];

        public string ID { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = string.Empty;
        public bool ObtainOnStart { get; set; } = false;
        public bool SeenByPlayer { get; set; } = false;
        public bool ShowProgress { get; set; } = false;
        public QuestStatus Status { get; set; } = QuestStatus.NotObtained;
        public int ObtainedDateTime { get; set; } = 0;
        public int LastUpdatedDateTime { get; set; } = 0;
        public bool ShowInInspector { get; set; } = true;
    }

    public sealed class ItemInteraction
    {
        public List<Criterion> Critera { get; set; } = [];

        public string ItemName { get; set; } = string.Empty;
        public List<GameEvent> OnAcceptEvents { get; set; } = [];
        public List<GameEvent> OnRefuseEvents { get; set; } = [];
        public bool DisplayInEditor { get; set; } = true;
    }

    public sealed class ItemGroupInteraction
    {
        public List<Criterion> Criteria { get; set; } = [];

        public string Name { get; set; } = string.Empty;

        public string GroupName { get; set; } = string.Empty;

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public List<GameEvent> OnAcceptEvents { get; set; } = [];
        public List<GameEvent> OnRefuseEvents { get; set; } = [];
        public bool DisplayInEditor { get; set; } = true;
    }

    public sealed class CharacterStory
    {
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

        public int DialogueID { get; set; } = 0;
        public List<BackgroundChatter> BackgroundChatter { get; set; } = [];
        public List<Dialogue> Dialogues { get; set; } = [];
        public List<EventTrigger> Reactions { get; set; } = [];
        public List<ItemGroupInteraction> CharacterItemGroupInteractions { get; set; } = [];
        public List<Quest> Quests { get; set; } = [];
        public List<Response> GlobalGoodbyeResponses { get; set; } = [];
        public List<Response> GlobalResponses { get; set; } = [];
        public List<ItemInteraction> StoryItems { get; set; } = [];
        public List<string> StoryValues { get; set; } = [];
        public Personality Personality { get; set; } = new();
        public string CharacterName { get; set; } = "#";

        public List<object> PersonalityPrompts { get; set; } = [];
        public List<object> Dialogue { get; set; } = [];
        public List<object> PeriodicBehaviors { get; set; } = [];
        public List<object> ReactionBehaviors { get; set; } = [];

        public string HousePartyVersion { get; set; } = Main.HousePartyVersion;
    }
}