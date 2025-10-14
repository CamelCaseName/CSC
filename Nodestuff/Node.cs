using CSC.Components;
using CSC.StoryItems;
using System.Diagnostics;
using System.Text.Json.Serialization;
using static CSC.StoryItems.StoryEnums;

namespace CSC.Nodestuff
{
    public readonly struct NodeID(string file, NodeType Type, string ID, string nodeOriginFilename, Type DataType)
    {
        public NodeType Type { get; } = Type;
        public string ID { get; } = ID;
        public string Filename { get; } = nodeOriginFilename;
        public string File { get; } = file;
        public Type DataType { get; } = DataType;

        public static implicit operator long(NodeID n)
        {
            unchecked // Overflow is fine, just wrap
            {
                long hash = 17;
                hash = hash * 23 + (int)n.Type;
                hash = hash * 23 + CalculateHash(n.ID);
                hash = hash * 17 + CalculateHash(n.File);
                hash = hash * 23 + CalculateHash(n.Filename);
                hash = hash * 23 + CalculateHash(n.DataType.ToString());
                return hash;
            }
        }

        static long CalculateHash(string read)
        {
            ulong hashedValue = 3074457345618258791ul;
            for (int i = 0; i < read.Length; i++)
            {
                hashedValue += read[i];
                hashedValue *= 3074457345618258799ul;
            }
            return (long)hashedValue;
        }
    }

    public enum ClickedNodeTypes
    {
        Null,
        Highlight,
        Info,
        OpenInEditor,
        Edit
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NodeType
    {
        Achievement,
        AlternateText,
        BGC,
        BGCResponse,
        CharacterGroup,
        Clothing,
        CriteriaGroup,
        Criterion,
        Cutscene,
        Dialogue,
        Door,
        EventTrigger,
        GameEvent,
        Inventory,
        ItemAction,
        ItemGroup,
        ItemGroupBehaviour,
        ItemGroupInteraction,
        ItemInteraction,
        Null,
        Personality,
        Pose,
        Property,
        Quest,
        Response,
        Social,
        State,
        StoryItem,
        UseWith,
        Value,
    }

    public enum SpawnableNodeType
    {
        Achievement,
        AlternateText,
        BGC,
        BGCResponse,
        CriteriaGroup,
        Criterion,
        Dialogue,
        EventTrigger,
        GameEvent,
        ItemAction,
        ItemGroup,
        ItemGroupInteraction,
        ItemInteraction,
        Quest,
        Response,
        StoryItem,
        UseWith,
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
        public static readonly string[] AllowedFileNames = [Main.Player, .. Enum.GetNames<Characters>().Cast<string>(), "Phone Call", "Liz Katz", "Doja Cat"];

        public static readonly Node NullNode = new();
        public string ID;
        public SizeF Size = new(Main.NodeSizeX, Main.NodeSizeY);
        public string StaticText;
        public NodeType Type;
        private readonly Dictionary<string, PointF> Positions = [];
        private readonly List<string> dupedFileNames = [];
        private object? data = null;
        private Type dataType = typeof(MissingReferenceInfo);

        private string fileName = Main.NoCharacter;
        private string origfilename = Main.NoCharacter;
        private bool setOnce = false;
        public string OrigFileName => origfilename;

        public Node(string iD, NodeType type, string text)
        {
            ID = iD;
            StaticText = text;
            Type = type;
            Size = new SizeF(Main.NodeSizeX, Main.NodeSizeY);
            Main.SetNodePos(this);
        }

        public Node(string iD, NodeType type, string text, object data)
        {
            ID = iD;
            StaticText = text;
            Type = type;
            RawData = data;
            DataType = data.GetType();
            Size = new SizeF(Main.NodeSizeX, Main.NodeSizeY);
            Main.SetNodePos(this);
        }

        public Node()
        {
            ID = string.Empty;
            StaticText = string.Empty;
            Type = NodeType.Null;
            Main.SetNodePos(this);
        }

        public Type DataType { get => dataType; private set => dataType = value; }

        public string FileName
        {
            get
            {
                return fileName;
            }

            set
            {
                if (!AllowedFileNames.Contains(value))
                {
                    //todo remove once charactergroups are in
                    value = Main.Player;
                }
                if (!setOnce)
                {
                    origfilename = value;
                    setOnce = true;
                }
                fileName = value;
            }
        }

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
                    return PointF.Empty;
                }
            }

            set
            {
                Main.ClearNodePos(this, Main.SelectedCharacter);
                Positions[Main.SelectedCharacter] = value;
                Main.SetNodePos(this, Main.SelectedCharacter);
                Main.NeedsSaving = true;
            }
        }

        public object? RawData
        {
            get
            {
                return data ?? new MissingReferenceInfo(Text);
            }
            set
            {
                data = value;
                DataType = value?.GetType() ?? typeof(object);

                if (DataType == typeof(Dialogue) || DataType == typeof(ItemInteraction) || DataType == typeof(ItemGroupInteraction))
                {
                    Size.Height *= 2;
                }
            }
        }

        public RectangleF Rectangle { get => new(Position, Size); }

        public Rectangle RectangleNonF { get => new((int)Position.X, (int)Position.Y, (int)Size.Width, (int)Size.Height); }

        public string Text
        {
            get
            {
                if (DataType == typeof(MissingReferenceInfo))
                {
                    return StaticText;
                }

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
                            var criterion = Data<Criterion>()!;
                            return $"{criterion.Character}{criterion.CompareType}{criterion.Key}{criterion.Value}";
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
                        GameEvent gevent = Data<GameEvent>()!;
                        try
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
                                    return gevent.Character + "'s  " + (gevent.Option4 == 0 ? (((ClothingType)int.Parse(gevent.Value!)).ToString()) : gevent.Value) + " in set " + (gevent.Option3 == 0 ? "any" : (gevent.Option3 - 1).ToString());
                                }
                                case GameEvents.Combat:
                                {
                                    break;
                                }
                                case GameEvents.CombineValue:
                                {
                                    return "Add " + gevent.Character + ":" + gevent.Key + " to " + gevent.Character2 + ":" + gevent.Value;
                                }
                                case GameEvents.CutScene:
                                {
                                    return ((CutsceneAction)gevent.Option).ToString() + " " + gevent.Key + " with " + gevent.Character + ", " + gevent.Value + ", " + gevent.Value2 + ", " + gevent.Character2 + " (location: " + gevent.Option2 + ")";
                                }
                                case GameEvents.Dialogue:
                                {
                                    return ((DialogueAction)gevent.Option).ToString() + " " + gevent.Character + "'s Dialogue " + gevent.Value;
                                }
                                case GameEvents.DisableNPC:
                                {
                                    return "Disable NPC " + gevent.Character;
                                }
                                case GameEvents.DisplayGameMessage:
                                {
                                    return "Display Message: " + gevent.Value;
                                }
                                case GameEvents.Door:
                                {
                                    return ((DoorAction)gevent.Option).ToString() + " " + gevent.Key!.ToString();
                                }
                                case GameEvents.Emote:
                                {
                                    break;
                                }
                                case GameEvents.EnableNPC:
                                {
                                    return "Enable NPC " + gevent.Character;
                                }
                                case GameEvents.EventTriggers:
                                {
                                    return gevent.Character + (gevent.Option == 0 ? " Perform Event " : " Set Enabled ") + (gevent.Option2 == 0 ? "(False) " : "(True) ") + gevent.Value;
                                }
                                case GameEvents.FadeIn:
                                {
                                    return "Fade in over " + gevent.Value + "s";
                                }
                                case GameEvents.FadeOut:
                                {
                                    return "Fade out over " + gevent.Value + "s";
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
                                    return gevent.Key!.ToString() + " " + ((ItemEventAction)gevent.Option).ToString() + " (" + gevent.Value + ") " + " (" + (gevent.Option2 == 1 ? "True" : "False") + ") ";
                                }
                                case GameEvents.ItemFromItemGroup:
                                {
                                    return gevent.Key!.ToString() + " " + ((ItemGroupAction)gevent.Option).ToString() + " (" + gevent.Value + ") " + " (" + (gevent.Option2 == 1 ? "True" : "False") + ") ";
                                }
                                case GameEvents.LookAt:
                                {
                                    break;
                                }
                                case GameEvents.Personality:
                                {
                                    return gevent.Character + " " + ((PersonalityTraits)gevent.Option).ToString() + " " + ((Modification)gevent.Option2).ToString() + " " + gevent.Value;
                                }
                                case GameEvents.Property:
                                {
                                    return gevent.Character + " " + EEnum.StringParse<InteractiveProperties>(gevent.Value!).ToString() + " " + (gevent.Option2 == 1 ? "True" : "False");
                                }
                                case GameEvents.MatchValue:
                                {
                                    return "set " + gevent.Character + ":" + gevent.Key + " to " + gevent.Character2 + ":" + gevent.Value;
                                }
                                case GameEvents.ModifyValue:
                                {
                                    return (gevent.Option == 0 ? "Equals " : "Add ") + gevent.Character + ":" + gevent.Key + " to " + gevent.Value;
                                }
                                case GameEvents.Player:
                                {
                                    return ((PlayerActions)gevent.Option).ToString() + (gevent.Option == 0 ? gevent.Option2 == 0 ? " Add " : " Remove " : " ") + gevent.Value + "/" + gevent.Character;
                                }
                                case GameEvents.PlaySoundboardClip:
                                {
                                    break;
                                }
                                case GameEvents.Pose:
                                {
                                    return "Set " + gevent.Character + " Pose no. " + gevent.Value + " " + (gevent.Option == 0 ? " False" : " True");
                                }
                                case GameEvents.Quest:
                                {
                                    return ((QuestActions)gevent.Option).ToString() + " the quest " + gevent.Value + " from " + gevent.Character;
                                }
                                case GameEvents.RandomizeIntValue:
                                {
                                    return "set " + gevent.Character + ":" + gevent.Key + " to a random value between " + gevent.Value + " and " + gevent.Value2;
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
                                    return gevent.Character + " " + ((SendEvents)gevent.Option).ToString();
                                }
                                case GameEvents.SetPlayerPref:
                                {
                                    break;
                                }
                                case GameEvents.Social:
                                {
                                    return gevent.Character + " " + ((SocialStatuses)gevent.Option).ToString() + " " + gevent.Character2 + (gevent.Option2 == 0 ? " Equals " : " Add ") + gevent.Value;
                                }
                                case GameEvents.State:
                                {
                                    return (gevent.Option == 0 ? "Add " : "Remove ") + gevent.Character + " State " + ((InteractiveStates)int.Parse(gevent.Value!)).ToString();
                                }
                                case GameEvents.TriggerBGC:
                                {
                                    return "trigger " + gevent.Character + "'s BGC " + gevent.Value + " as " + ((ImportanceSpecified)gevent.Option).ToString();
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
                                    return "None";
                                }
                                default:
                                {
                                    break;
                                }
                            }
                            goto case default;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            gevent.Value = "0";
                            gevent.Key = "";
                            gevent.Value2 = "";
                            return Text;
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
                        goto case default;
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
                        if (Data<Quest>() is null)
                        {
                            return StaticText;
                        }
                        else
                        {
                            return $"{Data<Quest>()?.Name ?? ID} -> {Data<Quest>()?.CharacterName}|{Data<Quest>()?.Name} on completion: [{Data<Quest>()?.CompletedDetails}] on failure: [{Data<Quest>()?.FailedDetails}] ";
                        }
                    }
                    case NodeType.Response:
                    {
                        return $"{Data<Response>()?.Id} -> {Data<Response>()?.Text} | leads to: {Data<Response>()?.Next}";
                    }
                    case NodeType.Value:
                    {
                        return $"{fileName}:{Data<string>()} \n{StaticText}";
                    }
                    case NodeType.UseWith:
                    {
                        return $"{Data<UseWith>()?.ItemName} -> {Data<UseWith>()?.CustomCantDoThatMessage}";
                    }
                    case NodeType.Pose:
                    {
                        if (int.TryParse(ID, out int res))
                        {
                            return ((Poses)res).ToString();
                        }
                        else
                        {
                            return "Pose" + ID;
                        }
                    }
                    case NodeType.Inventory:
                    case NodeType.StoryItem:
                    case NodeType.ItemInteraction:
                    case NodeType.Property:
                    case NodeType.Social:
                    case NodeType.State:
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
            }
        }

        public IEnumerable<string> DupedFileNames { get => dupedFileNames; }

        public static Node CreateCriteriaNode(Criterion criterion, string filename, NodeStore nodes)
        {
            //create all criteria nodes the same way so they can possibly be replaced by the actual text later
            var result = nodes.Nodes.Find((n) => n.Type == NodeType.Criterion && n.ID == $"{criterion.Character}{criterion.CompareType}{criterion.Key}{criterion.Value}");
            if (result is not null)
            {
                return result;
            }
            else
            {
                return new Node(
                    $"{criterion.Character}{criterion.CompareType}{criterion.Key}{criterion.Value}",
                    NodeType.Criterion,
                    $"{criterion.Character}|{criterion.CompareType}|{criterion.DialogueStatus}|{criterion.Key}|{criterion.Value}")
                { FileName = filename, RawData = criterion };
            }
        }

        public void AddCriteria(List<Criterion> criteria, NodeStore nodes)
        {
            foreach (Criterion criterion in criteria)
            {
                Node tempNode = CreateCriteriaNode(criterion, this.FileName, nodes);
                tempNode.RawData = criterion;
                nodes.AddParent(this, tempNode);
            }
        }

        public void AddEvents(List<GameEvent> events, NodeStore nodes)
        {
            foreach (GameEvent _event in events)
            {
                var nodeEvent = new Node(_event.Id ?? "none", NodeType.GameEvent, _event.Value ?? "none", this)
                {
                    RawData = _event,
                    DataType = typeof(GameEvent),
                    FileName = this.FileName
                };

                nodeEvent.AddCriteria(_event.Criteria ?? [], nodes);

                nodes.AddChild(this, nodeEvent);
            }
        }

        public T? Data<T>() where T : class
        {
            if (typeof(T) == DataType && RawData is not null)
            {
                return (T)RawData;
            }
            else
            {
                return null;
            }
        }

        public void DupeToOtherSorting(string filename)
        {
            if (!Positions.ContainsKey(filename))
            {
                Positions[filename] = new();

                if (origfilename != filename)
                {
                    dupedFileNames.Add(filename);
                }

                Main.SetNodePos(this, filename);
            }
        }

        public void RemoveFromSorting(string filename)
        {
            Positions.Remove(filename);
            Main.ClearNodePos(this, filename);
        }

        public string ToOutputFormat()
        {
            return $"{ID}|{Text}";
        }

        public override string ToString()
        {
            return $"{Type} | {ID} | {Text}";
        }
    }
}
