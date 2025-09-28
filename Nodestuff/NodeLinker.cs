using CSC.StoryItems;
using System.Diagnostics;
using static CSC.StoryItems.StoryEnums;

namespace CSC.Nodestuff
{
    internal static class NodeLinker
    {
        private static readonly List<Node> Doors = [];
        private static readonly List<Node> Values = [];
        private static readonly List<Node> Socials = [];
        private static readonly List<Node> States = [];
        private static readonly List<Node> Clothing = [];
        private static readonly List<Node> Poses = [];
        private static readonly List<Node> InventoryItems = [];
        private static readonly List<Node> Properties = [];
        private static readonly List<Node> CompareValuesToCheckAgain = [];

        public static string FileName { get; private set; } = Main.NoCharacter;

        public static void Interlinknodes(NodeStore store, string filename)
        {
            FileName = filename;
            DateTime start = DateTime.UtcNow;
            //lists to save new stuff

            try
            {
                int count = store.Count;
                var nodes = store.KeyNodes().ToList();
                //link up different stories and dialogues
                //doesnt matter that we add some in here, we only care about the ones added so far
                for (int x = 0; x < 2; x++)
                {
                    for (int i = 0; i < count; i++)
                    {
                        //link all useful criteria and add influencing values as parents
                        AnalyzeAndConnectNode(store, nodes[i], nodes, true);
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            //check some comparevaluenodes.again because the referenced values havent been added yet
            RecheckCompareValues(CompareValuesToCheckAgain, store, true);

            //merge doors with items if applicable
            MergeDoors(store, true);

            Debug.WriteLine($"completed for {FileName}/{store.Count} nodes in {(DateTime.UtcNow - start).Milliseconds}ms");
        }

        public static void UpdateLinks(Node node, string fileName, NodeStore store)
        {
            FileName = fileName;
            var family = store[node];

            List<Node> childs = [.. family.Childs];
            foreach (var child in childs)
            {
                store.RemoveChild(node, child);
            }
            List<Node> parents = [.. family.Parents];
            foreach (var parent in parents)
            {
                store.RemoveChild(node, parent);
            }
            var nodes = store.KeyNodes().ToList();

            AnalyzeAndConnectNode(store, node, nodes, true);
            foreach (var child in childs)
            {
                AnalyzeAndConnectNode(store, child, nodes, false);
            }
            foreach (var parent in parents)
            {
                AnalyzeAndConnectNode(store, parent, nodes, false);
            }
        }

        private static void AnalyzeAndConnectNode(NodeStore nodes, Node node, List<Node> searchIn, bool dupeTo = false)
        {
            AlternateText alternateText;
            BackgroundChatter chatter;
            CriteriaGroup criteriaGroup;
            Criterion criterion;
            Dialogue dialogue;
            EventTrigger trigger;
            GameEvent gameEvent;
            ItemAction itemAction;
            ItemGroup itemGroup;
            ItemGroupBehavior itemGroupBehavior;
            ItemGroupInteraction itemGroupInteraction;
            ItemOverride itemOverride;
            Node? result;
            Quest quest1;
            Response response;
            UseWith useWith;

            switch (node.Type)
            {
                case NodeType.Criterion when (criterion = node.Data<Criterion>()!) is not null:
                {
                    //node is dialogue so data should contain the criteria itself!
                    switch (criterion.CompareType)
                    {
                        case CompareTypes.Clothing:
                        {
                            result = Clothing.Find((n) => n.Type == NodeType.Clothing && n.FileName == criterion.Character && n.ID == criterion.Option + criterion.Value);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                                break;
                            }
                            else
                            {
                                //create and add value node, hasnt been referenced yet
                                var clothing = new Node(criterion.Option + criterion.Value, NodeType.Clothing, criterion.Character + "'s  " + ((Clothes)int.Parse(criterion.Value!)).ToString() + " in set " + (criterion.Option == 0 ? "any" : (criterion.Option - 1).ToString())) { FileName = criterion.Character! };
                                Clothing.Add(clothing);
                                nodes.AddParent(node, clothing);
                            }
                            break;
                        }
                        case CompareTypes.CompareValues:
                        {
                            result = Values.Find((n) => n.Type == NodeType.Value && n.ID == criterion.Key);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                            }
                            else
                            {
                                CompareValuesToCheckAgain.Add(node);
                            }
                            result = Values.Find((n) => n.Type == NodeType.Value && n.ID == criterion.Key2);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                            }
                            else
                            {
                                CompareValuesToCheckAgain.Add(node);
                            }
                            break;
                        }
                        case CompareTypes.CriteriaGroup:
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.CriteriaGroup && n.ID == criterion.Value);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                                break;
                            }
                            break;
                        }
                        case CompareTypes.CutScene:
                        {
                            result = Values.Find((n) => n.Type == NodeType.Cutscene && n.ID == criterion.Key);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                            }
                            else
                            {
                                //add cutscene
                                var item = new Node(criterion.Key!, NodeType.Cutscene, criterion.Key!)
                                {
                                    FileName = FileName!
                                };
                                searchIn.Add(item);
                                nodes.AddParent(node, item);
                            }
                            break;
                        }
                        case CompareTypes.Dialogue:
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.Dialogue && n.FileName == criterion.Character && n.ID == criterion.Value);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }


                                //dialogue influences this criteria
                                nodes.AddParent(node, result);
                                break;
                            }
                            else
                            {
                                //create and add new personality, should be from someone else
                                var item = new Node(criterion.Value!, NodeType.Dialogue, criterion.Character + " dialogue " + criterion.Value) { FileName = criterion.Character! };
                                searchIn.Add(item);
                                nodes.AddParent(node, item);
                            }
                            break;
                        }
                        case CompareTypes.Door:
                        {
                            result = Doors.Find((n) => n.Type == NodeType.Door && n.ID == criterion.Key);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                                break;
                            }
                            else
                            {
                                //create and add item node, hasnt been referenced yet
                                var door = new Node(criterion.Key!, NodeType.Door, criterion.Key!)
                                {
                                    FileName = FileName!
                                };
                                Doors.Add(door);
                                nodes.AddParent(node, door);
                            }
                            break;
                        }
                        case CompareTypes.Item:
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.StoryItem && n.ID == criterion.Key);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                                break;
                            }
                            else
                            {
                                //create and add item node, hasnt been referenced yet
                                var item = new Node(criterion.Key!, NodeType.StoryItem, criterion.Key!)
                                {
                                    FileName = FileName!
                                };
                                searchIn.Add(item);
                                nodes.AddParent(node, item);
                            }
                            break;
                        }
                        case CompareTypes.IsCurrentlyBeingUsed:
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.StoryItem && n.ID == criterion.Key);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                                break;
                            }
                            else
                            {
                                //create and add item node, hasnt been referenced yet
                                var item = new Node(criterion.Key!, NodeType.StoryItem, criterion.Key!)
                                {
                                    FileName = FileName!
                                };
                                searchIn.Add(item);
                                nodes.AddParent(node, item);
                            }
                            break;
                        }
                        case CompareTypes.IsCurrentlyUsing:
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.StoryItem && n.ID == criterion.Key);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                                break;
                            }
                            else
                            {
                                //create and add item node, hasnt been referenced yet
                                var item = new Node(criterion.Key!, NodeType.StoryItem, criterion.Key!)
                                {
                                    FileName = FileName!
                                };
                                searchIn.Add(item);
                                nodes.AddParent(node, item);
                            }
                            break;
                        }
                        case CompareTypes.ItemFromItemGroup:
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.ItemGroup && n.StaticText == criterion.Key);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                                break;
                            }
                            else
                            {
                                //create and add item node, hasnt been referenced yet
                                var item = new Node(criterion.Key!, NodeType.StoryItem, criterion.Key!)
                                {
                                    FileName = FileName!
                                };
                                searchIn.Add(item);
                                nodes.AddParent(node, item);
                            }
                            break;
                        }
                        case CompareTypes.Personality:
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.Personality && n.FileName == criterion.Character && n.ID == ((PersonalityTraits)int.Parse(criterion.Key!)).ToString());
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                                break;
                            }
                            else
                            {
                                //create and add new personality, should be from someone else
                                var item = new Node(((PersonalityTraits)int.Parse(criterion.Key!)).ToString(), NodeType.Personality, criterion.Character + "'s Personality " + ((PersonalityTraits)int.Parse(criterion.Key!)).ToString()) { FileName = criterion.Character! };
                                searchIn.Add(item);
                                nodes.AddParent(node, item);
                            }
                            break;
                        }
                        case CompareTypes.PlayerInventory:
                        {
                            //find/add inventory item
                            result = InventoryItems.Find((n) => n.Type == NodeType.Inventory && n.ID == criterion.Key);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                                break;
                            }
                            else
                            {
                                //create and add item node, hasnt been referenced yet
                                var item = new Node(criterion.Key!, NodeType.Inventory, "Items: " + criterion.Key)
                                {
                                    FileName = FileName!
                                };
                                InventoryItems.Add(item);
                                nodes.AddParent(node, item);
                            }
                            //find normal item if it exists
                            result = searchIn.Find((n) => n.Type == NodeType.StoryItem && n.ID == criterion.Key);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }

                            break;
                        }
                        case CompareTypes.Posing:
                        {
                            if (criterion.PoseOption != PoseOptions.CurrentPose)
                            {
                                break;
                            }

                            result = Poses.Find((n) => n.Type == NodeType.Pose && n.ID == criterion.Value);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                                break;
                            }
                            else
                            {
                                //create and add pose node, hasnt been referenced yet
                                var pose = new Node(criterion.Value!, NodeType.Pose, "Pose number " + criterion.Value)
                                {
                                    FileName = FileName!
                                };
                                Poses.Add(pose);
                                nodes.AddParent(node, pose);
                            }
                            break;
                        }
                        case CompareTypes.Property:
                        {
                            result = Properties.Find((n) => n.Type == NodeType.Property && n.ID == criterion.Character + "Property" + criterion.Value);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                                break;
                            }
                            else
                            {
                                //create and add property node, hasnt been referenced yet
                                var property = new Node(criterion.Character + "Property" + criterion.Value, NodeType.Property, criterion.Character + ((InteractiveProperties)int.Parse(criterion.Value!)).ToString()) { FileName = criterion.Character! };
                                Properties.Add(property);
                                nodes.AddParent(node, property);
                            }
                            break;
                        }
                        case CompareTypes.Quest:
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.Quest && n.ID == criterion.Key);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                                break;
                            }
                            break;
                        }
                        case CompareTypes.Social:
                        {
                            result = Socials.Find((n) => n.Type == NodeType.Social && n.ID == criterion.Character + criterion.SocialStatus + criterion.Character2);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                                break;
                            }
                            else
                            {
                                //create and add property node, hasnt been referenced yet
                                var social = new Node(criterion.Character + criterion.SocialStatus + criterion.Character2, NodeType.Social, criterion.Character + " " + criterion.SocialStatus + " " + criterion.Character2) { FileName = criterion.Character! };
                                Socials.Add(social);
                                nodes.AddParent(node, social);
                            }
                            break;
                        }
                        case CompareTypes.State:
                        {
                            result = States.Find((n) => n.Type == NodeType.State && n.FileName == criterion.Character && n.StaticText.AsSpan()[..2].Contains(criterion.Value!.AsSpan(), StringComparison.InvariantCulture));
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                                break;
                            }
                            else
                            {
                                //create and add state node, hasnt been referenced yet
                                var state = new Node(criterion.Character + "State" + criterion.Value, NodeType.State, criterion.Value + "|" + ((InteractiveStates)int.Parse(criterion.Value!)).ToString()) { FileName = criterion.Character! };
                                States.Add(state);
                                nodes.AddParent(node, state);
                            }
                            break;
                        }
                        case CompareTypes.Value:
                        {
                            result = Values.Find((n) => n.Type == NodeType.Value && n.ID == criterion.Key && n.FileName == criterion.Character);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                if (!result.StaticText.Contains(GetSymbolsFromValueFormula(criterion.ValueFormula ?? ValueSpecificFormulas.EqualsValue) + criterion.Value))
                                {
                                    result.StaticText += GetSymbolsFromValueFormula(criterion.ValueFormula ?? ValueSpecificFormulas.EqualsValue) + criterion.Value + ", ";
                                }

                                nodes.AddParent(node, result);
                                break;
                            }
                            else
                            {
                                //create and add value node, hasnt been referenced yet
                                var value = new Node(criterion.Key!, NodeType.Value, criterion.Character + " value " + criterion.Key + ", referenced values: " + GetSymbolsFromValueFormula(criterion.ValueFormula ?? ValueSpecificFormulas.EqualsValue) + criterion.Value + ", ") { FileName = criterion.Character ?? string.Empty };
                                Values.Add(value);
                                nodes.AddParent(node, value);
                            }
                            break;
                        }
                        default:
                            break;
                    }

                    break;
                }
                case NodeType.GameEvent when (gameEvent = node.Data<GameEvent>()!) is not null:
                {
                    switch (gameEvent.EventType)
                    {
                        case GameEvents.Clothing:
                        {
                            result = Clothing.Find((n) => n.Type == NodeType.Clothing && n.FileName == gameEvent.Character && n.ID == gameEvent.Option + gameEvent.Value);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add value node, hasnt been referenced yet
                                var clothing = new Node(gameEvent.Option + gameEvent.Value, NodeType.Clothing, gameEvent.Character + "'s  " + ((Clothes)int.Parse(gameEvent.Value!)).ToString() + " in set " + (gameEvent.Option == 0 ? "any" : (gameEvent.Option - 1).ToString())) { FileName = gameEvent.Character! };
                                Clothing.Add(clothing);
                                nodes.AddChild(node, clothing);
                                clothing.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.CombineValue:
                        {
                            result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Key && FileName == gameEvent.Character);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add value node, hasnt been referenced yet
                                var value = new Node(gameEvent.Key!, NodeType.Value, gameEvent.Character + " value " + gameEvent.Key) { FileName = gameEvent.Character ?? string.Empty };
                                Values.Add(value);
                                nodes.AddChild(node, value);
                                value.DupeToOtherSorting(node.FileName);
                            }
                            result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Value && FileName == gameEvent.Character2);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                            }
                            else
                            {
                                //create and add value node, hasnt been referenced yet
                                var value = new Node(gameEvent.Value!, NodeType.Value, gameEvent.Character2 + " value " + gameEvent.Value) { FileName = gameEvent.Character2 ?? string.Empty };
                                Values.Add(value);
                                nodes.AddParent(node, value);
                                value.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.CutScene:
                        {
                            result = Values.Find((n) => n.Type == NodeType.Cutscene && n.ID == gameEvent.Key);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //add cutscene
                                var item = new Node(gameEvent.Key!, NodeType.Cutscene, gameEvent.Key!)
                                {
                                    FileName = FileName!
                                };
                                searchIn.Add(item);
                                nodes.AddChild(node, item);
                                item.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.Dialogue:
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.Dialogue && n.FileName == gameEvent.Character && n.ID == gameEvent.Value);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }


                                //dialogue influences this criteria
                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add new dialogue, should be from someone else
                                var item = new Node(gameEvent.Value!, NodeType.Dialogue, ((DialogueAction)gameEvent.Option).ToString() + " " + gameEvent.Character + "'s Dialogue " + gameEvent.Value) { FileName = gameEvent.Character! };
                                searchIn.Add(item);
                                nodes.AddChild(node, item);
                                item.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.Door:
                        {
                            result = Doors.Find((n) => n.Type == NodeType.Door && n.ID == gameEvent.Key);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add item node, hasnt been referenced yet
                                var door = new Node(gameEvent.Key!, NodeType.Door, gameEvent.Key!)
                                {
                                    FileName = FileName!
                                };
                                Doors.Add(door);
                                nodes.AddChild(node, door);
                                door.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.EventTriggers:
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.GameEvent && n.StaticText == gameEvent.Value);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }


                                //stop 0 step cyclic self reference as it is not allowed
                                if (node != result)
                                {
                                    nodes.AddChild(node, result);
                                }
                            }
                            else
                            {
                                //create and add event, hasnt been referenced yet, we can not know its id if it doesnt already exist
                                var _event = new Node("NA-" + gameEvent.Value, NodeType.GameEvent, gameEvent.Value!)
                                {
                                    FileName = FileName!
                                };
                                searchIn.Add(_event);
                                nodes.AddChild(node, _event);
                                _event.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.Item:
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.StoryItem && n.ID == gameEvent.Key);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add item node, hasnt been referenced yet
                                var item = new Node(gameEvent.Key!, NodeType.StoryItem, gameEvent.Key!)
                                {
                                    FileName = FileName!
                                };
                                searchIn.Add(item);
                                nodes.AddChild(node, item);
                                item.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.ItemFromItemGroup:
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.StoryItem && n.ID == gameEvent.Key);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add item node, hasnt been referenced yet
                                var item = new Node(gameEvent.Key!, NodeType.StoryItem, gameEvent.Key!)
                                {
                                    FileName = FileName!
                                };
                                searchIn.Add(item);
                                nodes.AddChild(node, item);
                                item.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.Personality:
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.Personality && n.FileName == gameEvent.Character && n.ID == ((PersonalityTraits)gameEvent.Option).ToString());
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add new personality, should be from someone else
                                var item = new Node(((PersonalityTraits)gameEvent.Option).ToString(), NodeType.Personality, gameEvent.Character + "'s Personality " + ((PersonalityTraits)gameEvent.Option).ToString()) { FileName = gameEvent.Character! };
                                searchIn.Add(item);
                                nodes.AddChild(node, item);
                                item.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.Property:
                        {
                            result = Properties.Find((n) => n.Type == NodeType.Property && n.ID == gameEvent.Character + "Property" + gameEvent.Value);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add property node, hasnt been referenced yet
                                var property = new Node(gameEvent.Character + "Property" + gameEvent.Value, NodeType.Property, gameEvent.Character + EEnum.StringParse<InteractiveProperties>(gameEvent.Value!)) { FileName = gameEvent.Character! };
                                Properties.Add(property);
                                nodes.AddChild(node, property);
                                property.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.MatchValue:
                        {
                            result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Key && FileName == gameEvent.Character);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add value node, hasnt been referenced yet
                                var value = new Node(gameEvent.Key!, NodeType.Value, gameEvent.Character + " value " + gameEvent.Key) { FileName = gameEvent.Character ?? string.Empty };
                                Values.Add(value);
                                nodes.AddChild(node, value);
                                value.DupeToOtherSorting(node.FileName);
                            }
                            result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Value && FileName == gameEvent.Character2);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                            }
                            else
                            {
                                //create and add value node, hasnt been referenced yet
                                var value = new Node(gameEvent.Value!, NodeType.Value, gameEvent.Character2 + " value " + gameEvent.Value) { FileName = gameEvent.Character2 ?? string.Empty };
                                Values.Add(value);
                                nodes.AddParent(node, value);
                                value.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.ModifyValue:
                        {
                            result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Key && FileName == gameEvent.Character);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add value node, hasnt been referenced yet
                                var value = new Node(gameEvent.Key!, NodeType.Value, gameEvent.Character + " value " + gameEvent.Key) { FileName = gameEvent.Character ?? string.Empty };
                                Values.Add(value);
                                nodes.AddChild(node, value);
                                value.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.Player:
                        {                                //find/add inventory item
                            result = InventoryItems.Find((n) => n.Type == NodeType.Inventory && n.ID == gameEvent.Value);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                                break;
                            }
                            else
                            {
                                //create and add item node, hasnt been referenced yet
                                var item = new Node(gameEvent.Value!, NodeType.Inventory, "Items: " + gameEvent.Value)
                                {
                                    FileName = FileName!
                                };
                                InventoryItems.Add(item);
                                nodes.AddParent(node, item);
                                item.DupeToOtherSorting(node.FileName);
                            }
                            result = searchIn.Find((n) => n.Type == NodeType.StoryItem && n.ID == gameEvent.Value);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }

                            break;
                        }
                        case GameEvents.Pose:
                        {
                            result = Poses.Find((n) => n.Type == NodeType.Pose && n.ID == gameEvent.Value);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add pose node, hasnt been referenced yet
                                var pose = new Node(gameEvent.Value!, NodeType.Pose, "Pose number " + gameEvent.Value)
                                {
                                    FileName = FileName!
                                };
                                Poses.Add(pose);
                                nodes.AddChild(node, pose);
                                pose.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.Quest:
                        {
                            if (string.IsNullOrEmpty(gameEvent.Character))
                            {
                                foreach (string key in Main.Stories.Keys)
                                {
                                    for (int i = 0; i < Main.Stories[key].Quests!.Count; i++)
                                    {
                                        if (Main.Stories[key].Quests![i].ID == gameEvent.Key)
                                        {
                                            gameEvent.Character = key;
                                            gameEvent.Value = Main.Stories[key].Quests![i].Name;
                                        }
                                    }
                                }
                            }
                            result = searchIn.Find((n) => n.Type == NodeType.Quest && n.ID == gameEvent.Key);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add property node, hasnt been referenced yet
                                var quest = new Node(gameEvent.Key!, NodeType.Quest, gameEvent.Character + "'s quest " + gameEvent.Value + ", not found in loaded story files") { FileName = gameEvent.Character! };
                                searchIn.Add(quest);
                                nodes.AddChild(node, quest);
                                quest.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.RandomizeIntValue:
                        {
                            result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Key && FileName == gameEvent.Character);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add value node, hasnt been referenced yet
                                var value = new Node(gameEvent.Key!, NodeType.Value, gameEvent.Character + " value " + gameEvent.Key) { FileName = gameEvent.Character ?? string.Empty };
                                Values.Add(value);
                                nodes.AddChild(node, value);
                                value.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.Social:
                        {
                            result = Socials.Find((n) => n.Type == NodeType.Social && n.ID == gameEvent.Character + ((SocialStatuses)gameEvent.Option).ToString() + gameEvent.Character2);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add property node, hasnt been referenced yet
                                var social = new Node(gameEvent.Character + ((SocialStatuses)gameEvent.Option).ToString() + gameEvent.Character2, NodeType.Social, gameEvent.Character + " " + ((SocialStatuses)gameEvent.Option).ToString() + " " + gameEvent.Character2) { FileName = gameEvent.Character! };
                                Socials.Add(social);
                                nodes.AddChild(node, social);
                                //social.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.State:
                        {
                            result = States.Find((n) => n.Type == NodeType.State && n.FileName == gameEvent.Character && n.StaticText.AsSpan()[..2].Contains(gameEvent.Value!.AsSpan(), StringComparison.InvariantCulture));
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add state node, hasnt been referenced yet
                                var state = new Node(gameEvent.Character + "State" + gameEvent.Value, NodeType.State, gameEvent.Value + "|" + ((InteractiveStates)int.Parse(gameEvent.Value!)).ToString()) { FileName = gameEvent.Character! };
                                States.Add(state);
                                nodes.AddChild(node, state);
                                state.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.TriggerBGC:
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.BGC && n.ID == "BGC" + gameEvent.Value);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add property node, hasnt been referenced yet
                                var bgc = new Node("BGC" + gameEvent.Value, NodeType.BGC, gameEvent.Character + "'s BGC " + gameEvent.Value + ", not found in loaded story files") { FileName = gameEvent.Character! };
                                searchIn.Add(bgc);
                                nodes.AddChild(node, bgc);
                                bgc.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        default:
                            break;
                    }

                    foreach (var _criterion in gameEvent.Criteria)
                    {
                        HandleCriterion(nodes, node, searchIn, _criterion, dupeTo);
                    }

                    break;
                }
                case NodeType.EventTrigger when (trigger = node.Data<EventTrigger>()!) is not null:
                {
                    //link against events
                    foreach (GameEvent _event in trigger.Events!)
                    {
                        HandleEvent(nodes, node, searchIn, _event, dupeTo);
                    }
                    //link against criteria
                    foreach (Criterion _criterion in trigger.Critera!)
                    {
                        HandleCriterion(nodes, node, searchIn, _criterion, dupeTo);
                    }
                    node.StaticText = trigger.Critera.Count == 0
                        ? trigger.Name + " " + trigger.Type
                        : trigger.CharacterToReactTo + " " + trigger.Type + " " + trigger.UpdateIteration + " " + trigger.Name;
                    break;
                }
                case NodeType.Response when (response = node.Data<Response>()!) is not null:
                {
                    foreach (GameEvent _event in response.ResponseEvents!)
                    {
                        HandleEvent(nodes, node, searchIn, _event, dupeTo);
                    }

                    foreach (Criterion _criterion in response.ResponseCriteria!)
                    {
                        HandleCriterion(nodes, node, searchIn, _criterion, dupeTo);
                    }

                    if (response.Next == 0)
                    {
                        return;
                    }

                    result = searchIn.Find((n) => n.Type == NodeType.Dialogue && n.ID == response.Next.ToString());

                    if (result is not null)
                    {
                        if (dupeTo)
                        {
                            result.DupeToOtherSorting(node.FileName);
                        }

                        nodes.AddChild(node, result);
                    }
                    else
                    {
                        //create and add event, hasnt been referenced yet, we can not know its id if it doesnt already exist
                        var dialogueNode = new Node(response.Next.ToString(), NodeType.Dialogue, $"dialogue number {response.Next} for {node.FileName}") { FileName = NodeLinker.FileName };
                        searchIn.Add(dialogueNode);
                        nodes.AddChild(node, dialogueNode);
                    }

                    break;
                }
                case NodeType.Dialogue when (dialogue = node.Data<Dialogue>()!) is not null:
                {
                    if (dialogue.Responses.Count > 0)
                    {
                        foreach (var resp in dialogue.Responses)
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.Response && n.ID == resp.Id!);

                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add event, hasnt been referenced yet, we can not know its id if it doesnt already exist
                                var respNode = new Node(resp.Id!, NodeType.Response, $"response to {dialogue.ID} for {node.FileName}") { FileName = NodeLinker.FileName };
                                searchIn.Add(respNode);
                                nodes.AddChild(node, respNode);
                            }
                        }
                    }

                    if (dialogue.AlternateTexts.Count > 0)
                    {
                        foreach (var alternate in dialogue.AlternateTexts)
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.AlternateText && n.ID == $"{dialogue.ID}.{alternate.Order!}");

                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else
                            {
                                //create and add event, hasnt been referenced yet, we can not know its id if it doesnt already exist
                                var _node = new Node($"{dialogue.ID}.{alternate.Order!}", NodeType.AlternateText, $"alternate to {dialogue.ID} for {node.FileName}") { FileName = NodeLinker.FileName };
                                searchIn.Add(_node);
                                nodes.AddChild(node, _node);
                            }
                        }
                    }

                    if (dialogue.StartEvents.Count > 0)
                    {
                        foreach (var _event in dialogue.StartEvents)
                        {
                            HandleEvent(nodes, node, searchIn, _event, dupeTo);
                        }
                    }

                    if (dialogue.CloseEvents.Count > 0)
                    {
                        foreach (var _event in dialogue.CloseEvents)
                        {
                            HandleEvent(nodes, node, searchIn, _event, dupeTo);
                        }
                    }

                    break;
                }
                case NodeType.ItemAction when (itemAction = node.Data<ItemAction>()!) is not null:
                {
                    foreach (GameEvent _event in itemAction.OnTakeActionEvents!)
                    {
                        HandleEvent(nodes, node, searchIn, _event, dupeTo);
                    }

                    foreach (Criterion _criterion in itemAction.Criteria!)
                    {
                        HandleCriterion(nodes, node, searchIn, _criterion, dupeTo);
                    }

                    break;
                }
                case NodeType.ItemGroupBehaviour when (itemGroupBehavior = node.Data<ItemGroupBehavior>()!) is not null:
                {
                    foreach (var _action in itemGroupBehavior.ItemActions)
                    {
                        HandleItemAction(nodes, node, searchIn, _action, dupeTo);
                    }

                    foreach (var _usewith in itemGroupBehavior.UseWiths)
                    {
                        HandleUseWith(nodes, node, searchIn, _usewith, dupeTo);
                    }
                    break;
                }
                case NodeType.ItemGroupInteraction when (itemGroupInteraction = node.Data<ItemGroupInteraction>()!) is not null:
                {
                    foreach (GameEvent _event in itemGroupInteraction.OnAcceptEvents!)
                    {
                        HandleEvent(nodes, node, searchIn, _event, dupeTo);
                    }

                    foreach (GameEvent _event in itemGroupInteraction.OnRefuseEvents!)
                    {
                        HandleEvent(nodes, node, searchIn, _event, dupeTo);
                    }

                    foreach (Criterion _criterion in itemGroupInteraction.Critera!)
                    {
                        HandleCriterion(nodes, node, searchIn, _criterion, dupeTo);
                    }

                    break;
                }
                case NodeType.BGC when (chatter = node.Data<BackgroundChatter>()!) is not null:
                {
                    foreach (var _response in chatter.Responses)
                    {
                        result = searchIn.Find((n) => n.Type == NodeType.BGCResponse && n.ID == $"{_response.CharacterName}{_response.ChatterId}");

                        if (result is not null)
                        {
                            if (dupeTo)
                            {
                                result.DupeToOtherSorting(node.FileName);
                            }

                            nodes.AddChild(node, result);
                        }
                        else
                        {
                            //create and add event, hasnt been referenced yet, we can not know its id if it doesnt already exist
                            var newNode = new Node($"{_response.CharacterName}{_response.ChatterId}", NodeType.BGCResponse, $"{_response.CharacterName}{_response.ChatterId}") { FileName = NodeLinker.FileName };
                            searchIn.Add(newNode);
                            nodes.AddChild(node, newNode);
                        }
                    }

                    foreach (var _event in chatter.StartEvents)
                    {
                        HandleEvent(nodes, node, searchIn, _event, dupeTo);
                    }

                    foreach (var _criterion in chatter.Critera)
                    {
                        HandleCriterion(nodes, node, searchIn, _criterion, dupeTo);
                    }

                    break;
                }
                case NodeType.CriteriaGroup when (criteriaGroup = node.Data<CriteriaGroup>()!) is not null:
                {
                    //todo criterialists arent handled correctly currently with their multiple lists
                    foreach (var list in criteriaGroup.CriteriaList)
                    {
                        foreach (var _criterion in list.CriteriaList)
                        {
                            HandleCriterion(nodes, node, searchIn, _criterion, dupeTo);
                        }
                    }
                    break;
                }
                case NodeType.AlternateText when (alternateText = node.Data<AlternateText>()!) is not null:
                {
                    foreach (var _criterion in alternateText.Critera)
                    {
                        HandleCriterion(nodes, node, searchIn, _criterion, dupeTo);
                    }
                    break;
                }
                case NodeType.StoryItem when (itemOverride = node.Data<ItemOverride>()!) is not null:
                {
                    foreach (var _action in itemOverride.ItemActions)
                    {
                        HandleItemAction(nodes, node, searchIn, _action, dupeTo);
                    }

                    foreach (var _usewith in itemOverride.UseWiths)
                    {
                        HandleUseWith(nodes, node, searchIn, _usewith, dupeTo);
                    }
                    break;
                }
                case NodeType.ItemGroup when (itemGroup = node.Data<ItemGroup>()!) is not null:
                {
                    foreach (var item in itemGroup.ItemsInGroup)
                    {
                        result = searchIn.Find((n) => n.Type == NodeType.StoryItem && n.ID == item);

                        if (result is not null)
                        {
                            if (dupeTo)
                            {
                                result.DupeToOtherSorting(node.FileName);
                            }

                            nodes.AddChild(node, result);
                        }
                        else
                        {
                            //create and add event, hasnt been referenced yet, we can not know its id if it doesnt already exist
                            var newNode = new Node(item, NodeType.StoryItem, item) { FileName = NodeLinker.FileName };
                            searchIn.Add(newNode);
                            nodes.AddChild(node, newNode);
                        }
                    }
                    break;
                }
                case NodeType.Quest when (quest1 = node.Data<Quest>()!) is not null:
                {
                    //Add details
                    if (quest1.Details?.Length > 0)
                    {
                        result = searchIn.Find((n) => n.Type == NodeType.Quest && n.ID == $"{quest1.ID}Description" && n.FileName == quest1.CharacterName);

                        if (result is not null)
                        {
                            if (dupeTo)
                            {
                                result.DupeToOtherSorting(node.FileName);
                            }

                            nodes.AddChild(node, result);
                        }
                    }
                    //Add completed details
                    if (quest1.CompletedDetails?.Length > 0)
                    {
                        result = searchIn.Find((n) => n.Type == NodeType.Quest && n.ID == $"{quest1.ID}CompletedDetails" && n.FileName == quest1.CharacterName);

                        if (result is not null)
                        {
                            if (dupeTo)
                            {
                                result.DupeToOtherSorting(node.FileName);
                            }

                            nodes.AddChild(node, result);
                        }
                    }
                    //Add failed details
                    if (quest1.FailedDetails?.Length > 0)
                    {
                        result = searchIn.Find((n) => n.Type == NodeType.Quest && n.ID == $"{quest1.ID}FailedDetails" && n.FileName == quest1.CharacterName);

                        if (result is not null)
                        {
                            if (dupeTo)
                            {
                                result.DupeToOtherSorting(node.FileName);
                            }

                            nodes.AddChild(node, result);
                        }

                        //Add extended details
                        foreach (ExtendedDetail detail in quest1.ExtendedDetails ?? [])
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.Quest && n.ID == $"{quest1.ID}Description{detail.Value}" && n.FileName == quest1.CharacterName);

                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                        }
                    }
                    break;
                }
                case NodeType.UseWith when (useWith = node.Data<UseWith>()!) is not null:
                {
                    foreach (var _event in useWith.OnSuccessEvents)
                    {
                        HandleEvent(nodes, node, searchIn, _event, dupeTo);
                    }

                    foreach (var _criterion in useWith.Criteria)
                    {
                        HandleCriterion(nodes, node, searchIn, _criterion, dupeTo);
                    }

                    result = searchIn.Find((n) => n.Type == NodeType.StoryItem && n.ID == useWith.ItemName!);
                    if (result is not null)
                    {
                        if (dupeTo)
                        {
                            result.DupeToOtherSorting(node.FileName);
                        }

                        nodes.AddChild(node, result);
                    }
                    else
                    {
                        //create and add event, hasnt been referenced yet, we can not know its id if it doesnt already exist
                        var newNode = new Node(useWith.ItemName ?? string.Empty, NodeType.StoryItem, useWith.ItemName ?? string.Empty) { FileName = NodeLinker.FileName };
                        searchIn.Add(newNode);
                        nodes.AddChild(node, newNode);
                    }
                    break;
                }
            }
        }

        private static void HandleUseWith(NodeStore nodes, Node node, List<Node> newList, UseWith _usewith, bool dupeTo = false)
        {
            Node? result = newList.Find((n) => n.Type == NodeType.UseWith && n.ID == _usewith.ItemName!);
            if (result is not null)
            {
                if (dupeTo)
                {
                    result.DupeToOtherSorting(node.FileName);
                }

                nodes.AddChild(node, result);
            }
            else
            {
                //create and add event, hasnt been referenced yet, we can not know its id if it doesnt already exist
                var newNode = new Node(_usewith.ItemName ?? string.Empty, NodeType.UseWith, _usewith.CustomCantDoThatMessage ?? string.Empty) { FileName = NodeLinker.FileName };
                newList.Add(newNode);
                nodes.AddChild(node, newNode);
            }
        }

        private static void HandleItemAction(NodeStore nodes, Node node, List<Node> newList, ItemAction _action, bool dupeTo = false)
        {
            Node? result = newList.Find((n) => n.Type == NodeType.ItemAction && n.ID == _action.ActionName!);
            if (result is not null)
            {
                if (dupeTo)
                {
                    result.DupeToOtherSorting(node.FileName);
                }

                nodes.AddChild(node, result);
            }
            else
            {
                //create and add event, hasnt been referenced yet, we can not know its id if it doesnt already exist
                var newNode = new Node(_action.ActionName ?? string.Empty, NodeType.ItemAction, _action.ActionName ?? string.Empty) { FileName = NodeLinker.FileName };
                newList.Add(newNode);
                nodes.AddChild(node, newNode);
            }
        }

        private static void HandleCriterion(NodeStore nodes, Node node, List<Node> newList, Criterion _criterion, bool dupeTo = false)
        {
            Node? result = newList.Find((n) => n.Type == NodeType.Criterion && n.ID == $"{_criterion.Character}{_criterion.CompareType}{_criterion.Value}");
            if (result is not null)
            {
                if (dupeTo)
                {
                    result.DupeToOtherSorting(node.FileName);
                }

                nodes.AddParent(node, result);
            }
            else
            {
                newList.Add(Node.CreateCriteriaNode(_criterion, node, nodes));
            }
        }

        private static void HandleEvent(NodeStore nodes, Node node, List<Node> newList, GameEvent _event, bool dupeTo = false)
        {
            Node? result = newList.Find((n) => n.Type == NodeType.GameEvent && n.ID == _event.Id);
            if (result is not null)
            {
                if (dupeTo)
                {
                    result.DupeToOtherSorting(node.FileName);
                }

                nodes.AddChild(node, result);
            }
            else
            {
                //create and add event, hasnt been referenced yet, we can not know its id if it doesnt already exist
                var eventNode = new Node(_event.Id ?? "none", NodeType.GameEvent, _event.Value ?? "none") { FileName = NodeLinker.FileName };
                newList.Add(eventNode);
                nodes.AddChild(node, eventNode);
            }
        }

        public static void InterlinkBetweenFiles(Dictionary<string, NodeStore> stores)
        {
            foreach (var store in stores.Keys)
            {
                if (store == Main.NoCharacter)
                {
                    continue;
                }

                var tempList = stores[store].Nodes;
                foreach (var node in tempList)
                {
                    if (node.FileName == Main.NoCharacter)
                    {
                        //currently we dont have any here <3
                        Debugger.Break();
                    }

                    if (node.FileName != store)
                    {
                        if (stores.TryGetValue(node.FileName, out var nodeStore))
                        {
                            var templist2 = nodeStore.Nodes;

                            if (node.Type == NodeType.Dialogue && node.FileName == "Patrick")
                            {
                                if (node.ID == string.Empty)
                                {
                                    node.ID = 0.ToString();
                                }
                            }

                            var result = templist2.FindAll(n => n.Type == node.Type && n.ID == node.ID && n.FileName == node.FileName && n.DataType != typeof(MissingReferenceInfo));
                            foreach (var foundNode in result)
                            {
                                if (foundNode is not null)
                                {
                                    foundNode.DupeToOtherSorting(store);
                                    stores[store].Replace(node, foundNode);
                                    //foreach (var removeStore in stores.Keys)
                                    //{
                                    //    stores[removeStore].Remove(node);
                                    //}
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void ClearLinkCache()
        {
            Doors.Clear();
            Values.Clear();
            Socials.Clear();
            States.Clear();
            Clothing.Clear();
            Poses.Clear();
            InventoryItems.Clear();
            Properties.Clear();
            CompareValuesToCheckAgain.Clear();
        }

        private static void MergeDoors(NodeStore nodes, bool dupeTo = false)
        {
            Node? result;
            var newlist = nodes.KeyNodes().ToList();
            foreach (Node node in Doors.ToArray())
            {
                result = newlist.Find((n) => n.ID == node.ID);
                if (result is not null)
                {
                    if (dupeTo)
                    {
                        result.DupeToOtherSorting(node.FileName);
                    }

                    foreach (Node parentNode in nodes.Parents(node).ToArray())
                    {
                        nodes.AddChild(parentNode, result);
                        nodes.RemoveChild(parentNode, node);
                    }
                    foreach (Node childNode in nodes.Childs(node).ToArray())
                    {
                        nodes.AddParent(childNode, result);
                        nodes.RemoveParent(childNode, node);
                    }
                    Doors.Remove(node);
                }
            }
        }

        private static void RecheckCompareValues(List<Node> CompareValuesToCheckAgain, NodeStore nodes, bool dupeTo = false)
        {
            Node? result;
            foreach (Node node in CompareValuesToCheckAgain)
            {
                Criterion criterion;
                if ((criterion = node.Data<Criterion>()!) is not null)
                {
                    result = Values.Find((n) => n.Type == NodeType.Value && n.ID == criterion.Key);
                    if (result is not null)
                    {
                        if (dupeTo)
                        {
                            result.DupeToOtherSorting(node.FileName);
                        }

                        nodes.AddParent(node, result);
                    }

                    result = Values.Find((n) => n.Type == NodeType.Value && n.ID == criterion.Key2);
                    if (result is not null)
                    {
                        if (dupeTo)
                        {
                            result.DupeToOtherSorting(node.FileName);
                        }

                        nodes.AddParent(node, result);
                    }
                }
            }
        }

        public static void DissectCharacter(CharacterStory story, NodeStore nodes)
        {
            if (story is not null && nodes is not null)
            {
                FileName = story.CharacterName!;
                //get all relevant items from the json
                StoryNodeExtractor.GetItemInteractions(story, nodes);
                StoryNodeExtractor.GetValues(story, nodes);
                StoryNodeExtractor.GetPersonality(story, nodes);
                StoryNodeExtractor.GetDialogues(story, nodes);
                StoryNodeExtractor.GetGlobalGoodByeResponses(story, nodes);
                StoryNodeExtractor.GetGlobalResponses(story, nodes);
                StoryNodeExtractor.GetBackGroundChatter(story, nodes);
                StoryNodeExtractor.GetQuests(story, nodes);
                StoryNodeExtractor.GetReactions(story, nodes);

                //clear criteria to free memory, we dont need them anyways
                //cant be called recusrively so we cant add it, it would break the combination

                for (int i = 0; i < nodes.Nodes.Count; i++)
                {
                    if (nodes.Nodes[i].FileName is ("" or Main.NoCharacter))
                    {
                        nodes.Nodes[i].FileName = story.CharacterName ?? string.Empty;
                    }
                }
            }
        }

        public static void DissectStory(MainStory story, NodeStore nodes, string StoryName)
        {
            if (story is not null && nodes is not null)
            {
                //add all items in the story
                StoryNodeExtractor.GetItemOverrides(story, nodes);
                //add all item groups with their actions
                StoryNodeExtractor.GetItemGroups(story, nodes);
                //add all items in the story
                StoryNodeExtractor.GetAchievements(story, nodes);
                //add all reactions the player will say
                StoryNodeExtractor.GetPlayerReactions(story, nodes);
                //add all criteriagroups
                StoryNodeExtractor.GetCriteriaGroups(story, nodes);
                //gets the playervalues
                StoryNodeExtractor.GetValues(story, nodes);
                //the events which fire at game start
                StoryNodeExtractor.GetGameStartEvents(story, nodes);
                //add all item groups actions
                StoryNodeExtractor.GetItemGroupBehaviours(story, nodes);

                for (int i = 0; i < nodes.Nodes.Count; i++)
                {
                    if (nodes.Nodes[i].FileName is ("" or Main.NoCharacter))
                    {
                        nodes.Nodes[i].FileName = Main.Player;
                    }
                }
            }
        }

        private static string GetSymbolsFromValueFormula(ValueSpecificFormulas formula)
        {
            return formula switch
            {
                ValueSpecificFormulas.EqualsValue => "==",
                ValueSpecificFormulas.DoesNotEqualValue => "!=",
                ValueSpecificFormulas.GreaterThanValue => ">",
                ValueSpecificFormulas.LessThanValue => "<",
                _ => string.Empty,
            };
        }
    }
}
