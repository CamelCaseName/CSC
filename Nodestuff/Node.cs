using CSC.StoryItems;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using System.Runtime.InteropServices;
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
        GameEvent,
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
        public string StaticText;
        public string Text
        {
            get
            {
                switch (Type)
                {
                    case NodeType.Null:
                        return "Null node!";
                    case NodeType.CharacterGroup:
                    {
                        return $"{Data<CharacterGroup>()?.Id}|{Data<CharacterGroup>()?.Name} -> {Data<CharacterGroup>()?.CharactersInGroup?.ListRepresentation()}";
                    }
                    case NodeType.Criterion:
                    {
                        //todo
                        switch (Data<Criterion>()?.CompareType)
                        {
                            case CompareTypes.Never:
                            {
                                break;
                            }
                            case CompareTypes.CharacterFromCharacterGroup:
                            {
                                break;
                            }
                            case CompareTypes.Clothing:
                            {
                                break;
                            }
                            case CompareTypes.CoinFlip:
                            {
                                break;
                            }
                            case CompareTypes.CompareValues:
                            {
                                break;
                            }
                            case CompareTypes.CriteriaGroup:
                            {
                                break;
                            }
                            case CompareTypes.CutScene:
                            {
                                break;
                            }
                            case CompareTypes.Dialogue:
                            {
                                break;
                            }
                            case CompareTypes.Distance:
                            {
                                break;
                            }
                            case CompareTypes.Door:
                            {
                                break;
                            }
                            case CompareTypes.Gender:
                            {
                                break;
                            }
                            case CompareTypes.IntimacyPartner:
                            {
                                break;
                            }
                            case CompareTypes.IntimacyState:
                            {
                                break;
                            }
                            case CompareTypes.InZone:
                            {
                                break;
                            }
                            case CompareTypes.InVicinity:
                            {
                                break;
                            }
                            case CompareTypes.InVicinityAndVision:
                            {
                                break;
                            }
                            case CompareTypes.Item:
                            {
                                break;
                            }
                            case CompareTypes.IsBeingSpokenTo:
                            {
                                break;
                            }
                            case CompareTypes.IsAloneWithPlayer:
                            {
                                break;
                            }
                            case CompareTypes.IsDLCActive:
                            {
                                break;
                            }
                            case CompareTypes.IsOnlyInVicinityOf:
                            {
                                break;
                            }
                            case CompareTypes.IsOnlyInVisionOf:
                            {
                                break;
                            }
                            case CompareTypes.IsOnlyInVicinityAndVisionOf:
                            {
                                break;
                            }
                            case CompareTypes.IsCharacterEnabled:
                            {
                                break;
                            }
                            case CompareTypes.IsCurrentlyBeingUsed:
                            {
                                break;
                            }
                            case CompareTypes.IsCurrentlyUsing:
                            {
                                break;
                            }
                            case CompareTypes.IsExplicitGameVersion:
                            {
                                break;
                            }
                            case CompareTypes.IsGameUncensored:
                            {
                                break;
                            }
                            case CompareTypes.IsPackageInstalled:
                            {
                                break;
                            }
                            case CompareTypes.IsInFrontOf:
                            {
                                break;
                            }
                            case CompareTypes.IsInHouse:
                            {
                                break;
                            }
                            case CompareTypes.IsNewGame:
                            {
                                break;
                            }
                            case CompareTypes.IsZoneEmpty:
                            {
                                break;
                            }
                            case CompareTypes.ItemFromItemGroup:
                            {
                                break;
                            }
                            case CompareTypes.MetByPlayer:
                            {
                                break;
                            }
                            case CompareTypes.Personality:
                            {
                                break;
                            }
                            case CompareTypes.PlayerGender:
                            {
                                break;
                            }
                            case CompareTypes.PlayerInventory:
                            {
                                break;
                            }
                            case CompareTypes.PlayerPrefs:
                            {
                                break;
                            }
                            case CompareTypes.Posing:
                            {
                                break;
                            }
                            case CompareTypes.Property:
                            {
                                break;
                            }
                            case CompareTypes.Quest:
                            {
                                break;
                            }
                            case CompareTypes.SameZoneAs:
                            {
                                break;
                            }
                            case CompareTypes.ScreenFadeVisible:
                            {
                                break;
                            }
                            case CompareTypes.Social:
                            {
                                break;
                            }
                            case CompareTypes.State:
                            {
                                break;
                            }
                            case CompareTypes.Value:
                            {
                                break;
                            }
                            case CompareTypes.Vision:
                            {
                                break;
                            }
                            case CompareTypes.UseLegacyIntimacy:
                            {
                                break;
                            }
                            case CompareTypes.None:
                            {
                                break;
                            }
                            default:
                            {
                                break;
                            }
                        }
                        {
                            return StaticText;
                        }
                    }
                    case NodeType.ItemAction:
                    {
                        return $"{Data<ItemAction>()?.ActionName}";
                    }
                    case NodeType.ItemGroupBehaviour:
                    {
                        return $"{Data<ItemGroupBehavior>()?.Name} -> {Data<ItemGroupBehavior>()?.Name}";
                    }
                    case NodeType.ItemGroupInteraction:
                    {
                        return $"{Data<ItemGroupInteraction>()?.Name} on {Data<ItemGroupBehavior>()?.GroupName} | {Data<ItemGroupBehavior>()?.Id}";
                    }
                    case NodeType.Achievement:
                    {
                        return $"{Data<Achievement>()?.Name} | {Data<Achievement>()?.Id}";
                    }
                    case NodeType.BGC:
                    {
                        return $"{Data<BackgroundChatter>()?.Text} | {Data<BackgroundChatter>()?.Id}";
                    }
                    case NodeType.BGCResponse:
                    {
                        return $"{Data<BackgroundChatterResponse>()?.Label} -> respondant: {Data<BackgroundChatterResponse>()?.CharacterName}|{Data<BackgroundChatterResponse>()?.ChatterId}";
                    }
                    case NodeType.CriteriaGroup:
                    {
                        return $"{Data<CriteriaGroup>()?.Name} on group: {Data<CriteriaGroup>()?.LinkedGroupName} | {Data<CriteriaGroup>()?.Id}";
                    }
                    case NodeType.Dialogue:
                    {
                        return $"{Data<Dialogue>()?.ID} | {Data<Dialogue>()?.Text}";
                    }
                    case NodeType.AlternateText:
                    {
                        return $"{Data<AlternateText>()?.Text}";
                    }
                    case NodeType.GameEvent:
                    {
                        //todo
                        switch (Data<GameEvent>()?.EventType)
                        {
                            case GameEvents.AddForce:
                            {
                                break;
                            }
                            case GameEvents.AllowPlayerSave:
                            {
                                break;
                            }
                            case GameEvents.ChangeBodyScale:
                            {
                                break;
                            }
                            case GameEvents.CharacterFromCharacterGroup:
                            {
                                break;
                            }
                            case GameEvents.CharacterFunction:
                            {
                                break;
                            }
                            case GameEvents.Clothing:
                            {
                                break;
                            }
                            case GameEvents.Combat:
                            {
                                break;
                            }
                            case GameEvents.CombineValue:
                            {
                                break;
                            }
                            case GameEvents.CutScene:
                            {
                                break;
                            }
                            case GameEvents.Dialogue:
                            {
                                break;
                            }
                            case GameEvents.DisableNPC:
                            {
                                break;
                            }
                            case GameEvents.DisplayGameMessage:
                            {
                                break;
                            }
                            case GameEvents.Door:
                            {
                                break;
                            }
                            case GameEvents.Emote:
                            {
                                break;
                            }
                            case GameEvents.EnableNPC:
                            {
                                break;
                            }
                            case GameEvents.EventTriggers:
                            {
                                break;
                            }
                            case GameEvents.FadeIn:
                            {
                                break;
                            }
                            case GameEvents.FadeOut:
                            {
                                break;
                            }
                            case GameEvents.IKReach:
                            {
                                break;
                            }
                            case GameEvents.Intimacy:
                            {
                                break;
                            }
                            case GameEvents.Item:
                            {
                                break;
                            }
                            case GameEvents.ItemFromItemGroup:
                            {
                                break;
                            }
                            case GameEvents.LookAt:
                            {
                                break;
                            }
                            case GameEvents.Personality:
                            {
                                break;
                            }
                            case GameEvents.Property:
                            {
                                break;
                            }
                            case GameEvents.MatchValue:
                            {
                                break;
                            }
                            case GameEvents.ModifyValue:
                            {
                                break;
                            }
                            case GameEvents.Player:
                            {
                                break;
                            }
                            case GameEvents.PlaySoundboardClip:
                            {
                                break;
                            }
                            case GameEvents.Pose:
                            {
                                break;
                            }
                            case GameEvents.Quest:
                            {
                                break;
                            }
                            case GameEvents.RandomizeIntValue:
                            {
                                break;
                            }
                            case GameEvents.ResetReactionCooldown:
                            {
                                break;
                            }
                            case GameEvents.Roaming:
                            {
                                break;
                            }
                            case GameEvents.SendEvent:
                            {
                                break;
                            }
                            case GameEvents.SetPlayerPref:
                            {
                                break;
                            }
                            case GameEvents.Social:
                            {
                                break;
                            }
                            case GameEvents.State:
                            {
                                break;
                            }
                            case GameEvents.TriggerBGC:
                            {
                                break;
                            }
                            case GameEvents.Turn:
                            {
                                break;
                            }
                            case GameEvents.TurnInstantly:
                            {
                                break;
                            }
                            case GameEvents.UnlockAchievement:
                            {
                                break;
                            }
                            case GameEvents.WalkTo:
                            {
                                break;
                            }
                            case GameEvents.WarpOverTime:
                            {
                                break;
                            }
                            case GameEvents.WarpTo:
                            {
                                break;
                            }
                            case GameEvents.None:
                            {
                                break;
                            }
                            default:
                            {
                                break;
                            }
                        }
                        {
                            return StaticText;
                        }
                    }
                    case NodeType.EventTrigger:
                    {
                        //todo
                        switch (Data<EventTrigger>()?.Type)
                        {
                            case EventTypes.Never:
                            {
                                break;
                            }
                            case EventTypes.GameStarts:
                            {
                                break;
                            }
                            case EventTypes.EntersVision:
                            {
                                break;
                            }
                            case EventTypes.ExitsVision:
                            {
                                break;
                            }
                            case EventTypes.EntersVicinity:
                            {
                                break;
                            }
                            case EventTypes.EntersZone:
                            {
                                break;
                            }
                            case EventTypes.ReachesTarget:
                            {
                                break;
                            }
                            case EventTypes.IsBlockedByLockedDoor:
                            {
                                break;
                            }
                            case EventTypes.IsAttacked:
                            {
                                break;
                            }
                            case EventTypes.GetsKnockedOut:
                            {
                                break;
                            }
                            case EventTypes.Dies:
                            {
                                break;
                            }
                            case EventTypes.GetsHitWithProjectile:
                            {
                                break;
                            }
                            case EventTypes.FallsOver:
                            {
                                break;
                            }
                            case EventTypes.IsNaked:
                            {
                                break;
                            }
                            case EventTypes.IsBottomless:
                            {
                                break;
                            }
                            case EventTypes.IsTopless:
                            {
                                break;
                            }
                            case EventTypes.ExposesGenitals:
                            {
                                break;
                            }
                            case EventTypes.CaughtMasturbating:
                            {
                                break;
                            }
                            case EventTypes.CaughtHavingSex:
                            {
                                break;
                            }
                            case EventTypes.ExposesChest:
                            {
                                break;
                            }
                            case EventTypes.StartedIntimacyAct:
                            {
                                break;
                            }
                            case EventTypes.Orgasms:
                            {
                                break;
                            }
                            case EventTypes.EjaculatesOnMe:
                            {
                                break;
                            }
                            case EventTypes.GropesMyBreast:
                            {
                                break;
                            }
                            case EventTypes.GropesMyAss:
                            {
                                break;
                            }
                            case EventTypes.PlayerGrabsItem:
                            {
                                break;
                            }
                            case EventTypes.PlayerReleasesItem:
                            {
                                break;
                            }
                            case EventTypes.VapesOnMe:
                            {
                                break;
                            }
                            case EventTypes.PopperedMe:
                            {
                                break;
                            }
                            case EventTypes.PhoneBlindedMe:
                            {
                                break;
                            }
                            case EventTypes.Periodically:
                            {
                                break;
                            }
                            case EventTypes.OnItemFunction:
                            {
                                break;
                            }
                            case EventTypes.OnAnyItemAcceptFallback:
                            {
                                break;
                            }
                            case EventTypes.OnAnyItemRefuseFallback:
                            {
                                break;
                            }
                            case EventTypes.CombatModeToggled:
                            {
                                break;
                            }
                            case EventTypes.PokedByVibrator:
                            {
                                break;
                            }
                            case EventTypes.ImpactsGround:
                            {
                                break;
                            }
                            case EventTypes.ImpactsWall:
                            {
                                break;
                            }
                            case EventTypes.ScoredBeerPongPoint:
                            {
                                break;
                            }
                            case EventTypes.PeesOnMe:
                            {
                                break;
                            }
                            case EventTypes.PeesOnItem:
                            {
                                break;
                            }
                            case EventTypes.StartedPeeing:
                            {
                                break;
                            }
                            case EventTypes.StoppedPeeing:
                            {
                                break;
                            }
                            case EventTypes.PlayerThrowsItem:
                            {
                                break;
                            }
                            case EventTypes.StartedUsingActionItem:
                            {
                                break;
                            }
                            case EventTypes.StoppedUsingActionItem:
                            {
                                break;
                            }
                            case EventTypes.OnFriendshipIncreaseWith:
                            {
                                break;
                            }
                            case EventTypes.OnRomanceIncreaseWith:
                            {
                                break;
                            }
                            case EventTypes.OnFriendshipDecreaseWith:
                            {
                                break;
                            }
                            case EventTypes.OnRomanceDecreaseWith:
                            {
                                break;
                            }
                            case EventTypes.IsDancing:
                            {
                                break;
                            }
                            case EventTypes.StartedLapDance:
                            {
                                break;
                            }
                            case EventTypes.PlayerInventoryOpened:
                            {
                                break;
                            }
                            case EventTypes.PlayerInventoryClosed:
                            {
                                break;
                            }
                            case EventTypes.PlayerOpportunityWindowOpened:
                            {
                                break;
                            }
                            case EventTypes.PlayerInteractsWithCharacter:
                            {
                                break;
                            }
                            case EventTypes.PlayerInteractsWithItem:
                            {
                                break;
                            }
                            case EventTypes.OnScreenFadeInComplete:
                            {
                                break;
                            }
                            case EventTypes.OnScreenFadeOutComplete:
                            {
                                break;
                            }
                            case EventTypes.FinishedPopulatingMainDialogueText:
                            {
                                break;
                            }
                            case EventTypes.PlayerTookCameraPhoto:
                            {
                                break;
                            }
                            case EventTypes.OnAfterCutSceneEnds:
                            {
                                break;
                            }
                            case EventTypes.Ejaculates:
                            {
                                break;
                            }
                            case EventTypes.None:
                            {
                                break;
                            }
                            default:
                            {
                                break;
                            }
                        }
                        {
                            return StaticText;
                        }
                    }
                    case NodeType.ItemGroup:
                    {
                        return $"{Data<ItemGroup>()?.Name} | {Data<ItemGroup>()?.Id} |  -> {Data<ItemGroup>()?.ItemsInGroup.ListRepresentation()}";
                    }
                    case NodeType.Personality:
                    {
                        if (dataType == typeof(Trait))
                        {
                            return $"{Data<Trait>()?.Type} : {Data<Trait>()?.Value}";
                        }
                        else
                        {
                            return StaticText;
                        }
                    }
                    case NodeType.Quest:
                    {
                        return $"{Data<Quest>()?.ID} -> {Data<Quest>()?.CharacterName}|{Data<Quest>()?.Name} on success: [{Data<Quest>()?.CompletedDetails}] on success: [{Data<Quest>()?.FailedDetails}] ";
                    }
                    case NodeType.Response:
                    {
                        return $"{Data<Response>()?.Id} -> {Data<Response>()?.Text} | leads to: {Data<Response>()?.Next}";
                    }
                    case NodeType.Value:
                    {
                        return $"{fileName}:{Data<Value>()?.value} \n{StaticText}";
                    }
                    case NodeType.UseWith:
                    {
                        return $"{Data<UseWith>()?.ItemName} -> {Data<UseWith>()?.CustomCantDoThatMessage}";
                    }
                    case NodeType.Inventory:
                    case NodeType.Item:
                    case NodeType.Property:
                    case NodeType.Social:
                    case NodeType.State:
                    case NodeType.Pose:
                    case NodeType.Door:
                    case NodeType.Clothing:
                    case NodeType.Cutscene:
                    default:
                    {
                        if (string.IsNullOrEmpty(StaticText))
                        {
                            return $"{fileName} | {Type} | {ID}";
                        }
                        else
                        {
                            return StaticText;
                        }
                    }
                }

                return $"{fileName} | {Type} | {ID}";
            }
        }
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
            StaticText = text;
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
            StaticText = text;
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
            StaticText = string.Empty;
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
                var nodeEvent = new Node(_event.Id ?? "none", NodeType.GameEvent, _event.Value ?? "none", this, CurrentPositionSorting)
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
