using CSC.StoryItems;
using System.Diagnostics;
using static CSC.StoryItems.StoryEnums;

namespace CSC.Nodestuff
{
    internal static class NodeLinker
    {
        private static readonly List<Node> Doors = [];
        private static readonly List<Node> Values = [];
        public static string? FileName { get; private set; }

        public static void Interlinknodes(NodeStore nodes)
        {
            DateTime start = DateTime.UtcNow;
            //lists to save new stuff in
            List<Node> Socials = [];
            List<Node> States = [];
            List<Node> Clothing = [];
            List<Node> Poses = [];
            List<Node> InventoryItems = [];
            List<Node> Properties = [];
            List<Node> CompareValuesToCheckAgain = [];

            Node? result;
            Criterion criterion;
            GameEvent gameEvent;
            EventTrigger trigger;
            Values.Clear();
            Doors.Clear();
            try
            {
                int count = nodes.Count;
                var newList = nodes.KeyNodes().ToList();
                //link up different stories and dialogues
                //doesnt matter that we add some in here, we only care about the ones added so far
                for (int i = 0; i < count; i++)
                {
                    //link all useful criteria and add influencing values as parents
                    if (newList[i].Type == NodeType.Criterion && newList[i].Data is not null)
                    {
                        //node is dialogue so data should contain the criteria itself!
                        criterion = (Criterion)newList[i].Data!;
                        switch (criterion.CompareType)
                        {
                            case CompareTypes.Clothing:
                            {
                                result = Clothing.Find((n) => n.Type == NodeType.Clothing && n.FileName == criterion.Character && n.ID == criterion.Option + criterion.Value);
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var clothing = new Node(criterion.Option + criterion.Value, NodeType.Clothing, criterion.Character + "'s  " + ((Clothes)int.Parse(criterion.Value!)).ToString() + " in set " + (criterion.Option == 0 ? "any" : (criterion.Option - 1).ToString()),nodes.Positions) { FileName = criterion.Character! };
                                    Clothing.Add(clothing);
                                    nodes.AddParent(newList[i], clothing);
                                }
                                break;
                            }
                            case CompareTypes.CompareValues:
                            {
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                }
                                else
                                {
                                    CompareValuesToCheckAgain.Add(newList[i]);
                                }
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == criterion.Key2);
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                }
                                else
                                {
                                    CompareValuesToCheckAgain.Add(newList[i]);
                                }
                                break;
                            }
                            case CompareTypes.CriteriaGroup:
                            {
                                result = newList.Find((n) => n.Type == NodeType.CriteriaGroup && n.ID == criterion.Value);
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                    break;
                                }
                                break;
                            }
                            case CompareTypes.CutScene:
                            {
                                result = Values.Find((n) => n.Type == NodeType.Cutscene && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                }
                                else
                                {
                                    //add cutscene
                                    var item = new Node(criterion.Key!, NodeType.Cutscene, criterion.Key!, nodes.Positions);
                                    item.FileName = FileName!;
                                    newList.Add(item);
                                    nodes.AddParent(newList[i], item);
                                }
                                break;
                            }
                            case CompareTypes.Dialogue:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Dialogue && n.FileName == criterion.Character && n.ID == criterion.Value);
                                if (result is not null)
                                {
                                    //dialogue influences this criteria
                                    nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add new personality, should be from someone else
                                    var item = new Node(criterion.Value!, NodeType.Dialogue, criterion.Character + " dialogue " + criterion.Value, nodes.Positions) { FileName = criterion.Character! };
                                    newList.Add(item);
                                    nodes.AddParent(newList[i], item);
                                }
                                break;
                            }
                            case CompareTypes.Door:
                            {
                                result = Doors.Find((n) => n.Type == NodeType.Door && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var door = new Node(criterion.Key!, NodeType.Door, criterion.Key!, nodes.Positions);
                                    door.FileName = FileName!;
                                    Doors.Add(door);
                                    nodes.AddParent(newList[i], door);
                                }
                                break;
                            }
                            case CompareTypes.Item:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Item && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var item = new Node(criterion.Key!, NodeType.Item, criterion.Key!, nodes.Positions);
                                    item.FileName = FileName!;
                                    newList.Add(item);
                                    nodes.AddParent(newList[i], item);
                                }
                                break;
                            }
                            case CompareTypes.IsCurrentlyBeingUsed:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Item && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var item = new Node(criterion.Key!, NodeType.Item, criterion.Key!, nodes.Positions);
                                    item.FileName = FileName!;
                                    newList.Add(item);
                                    nodes.AddParent(newList[i], item);
                                }
                                break;
                            }
                            case CompareTypes.IsCurrentlyUsing:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Item && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var item = new Node(criterion.Key!, NodeType.Item, criterion.Key!, nodes.Positions);
                                    item.FileName = FileName!;
                                    newList.Add(item);
                                    nodes.AddParent(newList[i], item);
                                }
                                break;
                            }
                            case CompareTypes.ItemFromItemGroup:
                            {
                                result = newList.Find((n) => n.Type == NodeType.ItemGroup && n.Text == criterion.Key);
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var item = new Node(criterion.Key!, NodeType.Item, criterion.Key!, nodes.Positions);
                                    item.FileName = FileName!;
                                    newList.Add(item);
                                    nodes.AddParent(newList[i], item);
                                }
                                break;
                            }
                            case CompareTypes.Personality:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Personality && n.FileName == criterion.Character && n.ID == ((PersonalityTraits)int.Parse(criterion.Key!)).ToString());
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add new personality, should be from someone else
                                    var item = new Node(((PersonalityTraits)int.Parse(criterion.Key!)).ToString(), NodeType.Personality, criterion.Character + "'s Personality " + ((PersonalityTraits)int.Parse(criterion.Key!)).ToString(), nodes.Positions) { FileName = criterion.Character! };
                                    newList.Add(item);
                                    nodes.AddParent(newList[i], item);
                                }
                                break;
                            }
                            case CompareTypes.PlayerInventory:
                            {
                                //find/add inventory item
                                result = InventoryItems.Find((n) => n.Type == NodeType.Inventory && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var item = new Node(criterion.Key!, NodeType.Inventory, "Items: " + criterion.Key, nodes.Positions);
                                    item.FileName = FileName!;
                                    InventoryItems.Add(item);
                                    nodes.AddParent(newList[i], item);
                                }
                                //find normal item if it exists
                                result = newList.Find((n) => n.Type == NodeType.Item && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
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
                                    nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add pose node, hasnt been referenced yet
                                    var pose = new Node(criterion.Value!, NodeType.Pose, "Pose number " + criterion.Value, nodes.Positions);
                                    pose.FileName = FileName!;
                                    Poses.Add(pose);
                                    nodes.AddParent(newList[i], pose);
                                }
                                break;
                            }
                            case CompareTypes.Property:
                            {
                                result = Properties.Find((n) => n.Type == NodeType.Property && n.ID == criterion.Character + "Property" + criterion.Value);
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add property node, hasnt been referenced yet
                                    var property = new Node(criterion.Character + "Property" + criterion.Value, NodeType.Property, criterion.Character + ((InteractiveProperties)int.Parse(criterion.Value!)).ToString(), nodes.Positions) { FileName = criterion.Character! };
                                    Properties.Add(property);
                                    nodes.AddParent(newList[i], property);
                                }
                                break;
                            }
                            case CompareTypes.Quest:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Quest && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                    break;
                                }
                                break;
                            }
                            case CompareTypes.Social:
                            {
                                result = Socials.Find((n) => n.Type == NodeType.Social && n.ID == criterion.Character + criterion.SocialStatus + criterion.Character2);
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add property node, hasnt been referenced yet
                                    var social = new Node(criterion.Character + criterion.SocialStatus + criterion.Character2, NodeType.Social, criterion.Character + " " + criterion.SocialStatus + " " + criterion.Character2, nodes.Positions) { FileName = criterion.Character! };
                                    Socials.Add(social);
                                    nodes.AddParent(newList[i], social);
                                }
                                break;
                            }
                            case CompareTypes.State:
                            {
                                result = States.Find((n) => n.Type == NodeType.State && n.FileName == criterion.Character && n.Text.AsSpan()[..2].Contains(criterion.Value!.AsSpan(), StringComparison.InvariantCulture));
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add state node, hasnt been referenced yet
                                    var state = new Node(criterion.Character + "State" + criterion.Value, NodeType.State, criterion.Value + "|" + ((InteractiveStates)int.Parse(criterion.Value!)).ToString(), nodes.Positions) { FileName = criterion.Character! };
                                    States.Add(state);
                                    nodes.AddParent(newList[i], state);
                                }
                                break;
                            }
                            case CompareTypes.Value:
                            {
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == criterion.Key && n.FileName == criterion.Character);
                                if (result is not null)
                                {
                                    if (!result.Text.Contains(GetSymbolsFromValueFormula(criterion.ValueFormula ?? ValueSpecificFormulas.EqualsValue) + criterion.Value))
                                    {
                                        result.Text += GetSymbolsFromValueFormula(criterion.ValueFormula ?? ValueSpecificFormulas.EqualsValue) + criterion.Value + ", ";
                                    }

                                    nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var value = new Node(criterion.Key!, NodeType.Value, criterion.Character + " value " + criterion.Key + ", referenced values: " + GetSymbolsFromValueFormula(criterion.ValueFormula ?? ValueSpecificFormulas.EqualsValue) + criterion.Value + ", ", nodes.Positions) { FileName = criterion.Character ?? string.Empty };
                                    Values.Add(value);
                                    nodes.AddParent(newList[i], value);
                                }
                                break;
                            }
                            default:
                                break;
                        }
                    }
                    else if (newList[i].Type == NodeType.Event && newList[i].Data is not null)
                    {
                        gameEvent = (GameEvent)newList[i].Data!;
                        switch (gameEvent.EventType)
                        {
                            case GameEvents.Clothing:
                            {
                                result = Clothing.Find((n) => n.Type == NodeType.Clothing && n.FileName == gameEvent.Character && n.ID == gameEvent.Option + gameEvent.Value);
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var clothing = new Node(gameEvent.Option + gameEvent.Value, NodeType.Clothing, gameEvent.Character + "'s  " + ((Clothes)int.Parse(gameEvent.Value!)).ToString() + " in set " + (gameEvent.Option == 0 ? "any" : (gameEvent.Option - 1).ToString()), nodes.Positions) { FileName = gameEvent.Character! };
                                    Clothing.Add(clothing);
                                    nodes.AddChild(newList[i], clothing);
                                }
                                newList[i].Text = gameEvent.Character + " " + ((Clothes)int.Parse(gameEvent.Value!)).ToString() + " in set " + (gameEvent.Option == 0 ? "any" : (gameEvent.Option - 1).ToString()) + " " + (gameEvent.Option2 == 0 ? "Change" : "Assign default set") + " " + (gameEvent.Option3 == 0 ? "On" : "Off");
                                break;
                            }
                            case GameEvents.CombineValue:
                            {
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Key && FileName == gameEvent.Character);
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var value = new Node(gameEvent.Key!, NodeType.Value, gameEvent.Character + " value " + gameEvent.Key, nodes.Positions) { FileName = gameEvent.Character ?? string.Empty };
                                    Values.Add(value);
                                    nodes.AddChild(newList[i], value);
                                }
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Value && FileName == gameEvent.Character2);
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var value = new Node(gameEvent.Value!, NodeType.Value, gameEvent.Character2 + " value " + gameEvent.Value, nodes.Positions) { FileName = gameEvent.Character2 ?? string.Empty };
                                    Values.Add(value);
                                    nodes.AddParent(newList[i], value);
                                }
                                newList[i].Text = "Add " + gameEvent.Character + ":" + gameEvent.Key + " to " + gameEvent.Character2 + ":" + gameEvent.Value;
                                break;
                            }
                            case GameEvents.CutScene:
                            {
                                result = Values.Find((n) => n.Type == NodeType.Cutscene && n.ID == gameEvent.Key);
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //add cutscene
                                    var item = new Node(gameEvent.Key!, NodeType.Cutscene, gameEvent.Key!, nodes.Positions);
                                    item.FileName = FileName!;
                                    newList.Add(item);
                                    nodes.AddChild(newList[i], item);
                                }
                                newList[i].Text = ((CutsceneAction)gameEvent.Option).ToString() + " " + gameEvent.Key + " with " + gameEvent.Character + ", " + gameEvent.Value + ", " + gameEvent.Value2 + ", " + gameEvent.Character2 + " (location: " + gameEvent.Option2 + ")";
                                break;
                            }
                            case GameEvents.Dialogue:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Dialogue && n.FileName == gameEvent.Character && n.ID == gameEvent.Value);
                                if (result is not null)
                                {
                                    //dialogue influences this criteria
                                    nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add new dialogue, should be from someone else
                                    var item = new Node(gameEvent.Value!, NodeType.Dialogue, gameEvent.Character + " dialoge " + gameEvent.Value, nodes.Positions) { FileName = gameEvent.Character! };
                                    newList.Add(item);
                                    nodes.AddChild(newList[i], item);
                                }
                                newList[i].Text = ((DialogueAction)gameEvent.Option).ToString() + " " + gameEvent.Character + "'s Dialogue " + gameEvent.Value;
                                break;
                            }
                            case GameEvents.Door:
                            {
                                result = Doors.Find((n) => n.Type == NodeType.Door && n.ID == gameEvent.Key);
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var door = new Node(gameEvent.Key!, NodeType.Door, gameEvent.Key!, nodes.Positions);
                                    door.FileName = FileName!;
                                    Doors.Add(door);
                                    nodes.AddChild(newList[i], door);
                                }
                                newList[i].Text = ((DoorAction)gameEvent.Option).ToString() + " " + gameEvent.Key!.ToString();
                                break;
                            }
                            case GameEvents.EventTriggers:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Event && n.Text == gameEvent.Value);
                                if (result is not null)
                                {
                                    //stop 0 step cyclic self reference as it is not allowed
                                    if (newList[i] != result)
                                    {
                                        nodes.AddChild(newList[i], result);
                                    }
                                }
                                else
                                {
                                    //create and add event, hasnt been referenced yet, we can not know its id if it doesnt already exist
                                    var _event = new Node("NA-" + gameEvent.Value, NodeType.Event, gameEvent.Value!, nodes.Positions);
                                    newList.Add(_event);
                                    nodes.AddChild(newList[i], _event);
                                }
                                newList[i].Text = gameEvent.Character + (gameEvent.Option == 0 ? " Perform Event " : " Set Enabled ") + (gameEvent.Option2 == 0 ? "(False) " : "(True) ") + gameEvent.Value;
                                break;
                            }
                            case GameEvents.Item:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Item && n.ID == gameEvent.Key);
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var item = new Node(gameEvent.Key!, NodeType.Item, gameEvent.Key!, nodes.Positions);
                                    item.FileName = FileName!;
                                    newList.Add(item);
                                    nodes.AddChild(newList[i], item);
                                }
                                newList[i].Text = gameEvent.Key!.ToString() + " " + ((ItemEventAction)gameEvent.Option).ToString() + " (" + gameEvent.Value + ") " + " (" + (gameEvent.Option2 == 1 ? "True" : "False") + ") ";
                                break;
                            }
                            case GameEvents.ItemFromItemGroup:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Item && n.ID == gameEvent.Key);
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var item = new Node(gameEvent.Key!, NodeType.Item, gameEvent.Key!, nodes.Positions);
                                    item.FileName = FileName!;
                                    newList.Add(item);
                                    nodes.AddChild(newList[i], item);
                                }
                                newList[i].Text = gameEvent.Key!.ToString() + " " + ((ItemGroupAction)gameEvent.Option).ToString() + " (" + gameEvent.Value + ") " + " (" + (gameEvent.Option2 == 1 ? "True" : "False") + ") ";
                                break;
                            }
                            case GameEvents.Personality:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Personality && n.FileName == gameEvent.Character && n.ID == ((PersonalityTraits)gameEvent.Option).ToString());
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add new personality, should be from someone else
                                    var item = new Node(((PersonalityTraits)gameEvent.Option).ToString(), NodeType.Personality, gameEvent.Character + "'s Personality " + ((PersonalityTraits)gameEvent.Option).ToString(), nodes.Positions) { FileName = gameEvent.Character! };
                                    newList.Add(item);
                                    nodes.AddChild(newList[i], item);
                                }
                                newList[i].Text = gameEvent.Character + " " + ((PersonalityTraits)gameEvent.Option).ToString() + " " + ((PersonalityAction)gameEvent.Option2).ToString() + " " + gameEvent.Value;
                                break;
                            }
                            case GameEvents.Property:
                            {
                                result = Properties.Find((n) => n.Type == NodeType.Property && n.ID == gameEvent.Character + "Property" + gameEvent.Value);
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add property node, hasnt been referenced yet
                                    var property = new Node(gameEvent.Character + "Property" + gameEvent.Value, NodeType.Property, gameEvent.Character + Enum.Parse<InteractiveProperties>(gameEvent.Value!).ToString(), nodes.Positions) { FileName = gameEvent.Character! };
                                    Properties.Add(property);
                                    nodes.AddChild(newList[i], property);
                                }
                                newList[i].Text = gameEvent.Character + " " + Enum.Parse<InteractiveProperties>(gameEvent.Value!).ToString() + " " + (gameEvent.Option2 == 1 ? "True" : "False");
                                break;
                            }
                            case GameEvents.MatchValue:
                            {
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Key && FileName == gameEvent.Character);
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var value = new Node(gameEvent.Key!, NodeType.Value, gameEvent.Character + " value " + gameEvent.Key, nodes.Positions) { FileName = gameEvent.Character ?? string.Empty };
                                    Values.Add(value);
                                    nodes.AddChild(newList[i], value);
                                }
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Value && FileName == gameEvent.Character2);
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var value = new Node(gameEvent.Value!, NodeType.Value, gameEvent.Character2 + " value " + gameEvent.Value, nodes.Positions) { FileName = gameEvent.Character2 ?? string.Empty };
                                    Values.Add(value);
                                    nodes.AddParent(newList[i], value);
                                }
                                newList[i].Text = "set " + gameEvent.Character + ":" + gameEvent.Key + " to " + gameEvent.Character2 + ":" + gameEvent.Value;
                                break;
                            }
                            case GameEvents.ModifyValue:
                            {
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Key && FileName == gameEvent.Character);
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var value = new Node(gameEvent.Key!, NodeType.Value, gameEvent.Character + " value " + gameEvent.Key, nodes.Positions) { FileName = gameEvent.Character ?? string.Empty };
                                    Values.Add(value);
                                    nodes.AddChild(newList[i], value);
                                }
                                newList[i].Text = (gameEvent.Option == 0 ? "Equals" : "Add") + gameEvent.Character + ":" + gameEvent.Key + " to " + gameEvent.Value;
                                break;
                            }
                            case GameEvents.Player:
                            {                                //find/add inventory item
                                result = InventoryItems.Find((n) => n.Type == NodeType.Inventory && n.ID == gameEvent.Value);
                                if (result is not null)
                                {
                                    nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var item = new Node(gameEvent.Value!, NodeType.Inventory, "Items: " + gameEvent.Value, nodes.Positions);
                                    item.FileName = FileName!;
                                    InventoryItems.Add(item);
                                    nodes.AddParent(newList[i], item);
                                }
                                result = newList.Find((n) => n.Type == NodeType.Item && n.ID == gameEvent.Value);
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
                                }

                                newList[i].Text = ((PlayerActions)gameEvent.Option).ToString() + (gameEvent.Option == 0 ? gameEvent.Option2 == 0 ? " Add " : " Remove " : " ") + gameEvent.Value + "/" + gameEvent.Character;
                                break;
                            }
                            case GameEvents.Pose:
                            {
                                result = Poses.Find((n) => n.Type == NodeType.Pose && n.ID == gameEvent.Value);
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add pose node, hasnt been referenced yet
                                    var pose = new Node(gameEvent.Value!, NodeType.Pose, "Pose number " + gameEvent.Value, nodes.Positions);
                                    pose.FileName = FileName!;
                                    Poses.Add(pose);
                                    nodes.AddChild(newList[i], pose);
                                }
                                newList[i].Text = "Set " + gameEvent.Character + " Pose no. " + gameEvent.Value + " " + (gameEvent.Option == 0 ? " False" : " True");
                                break;
                            }
                            case GameEvents.Quest:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Quest && n.ID == gameEvent.Key);
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add property node, hasnt been referenced yet
                                    var quest = new Node(gameEvent.Value!, NodeType.Social, gameEvent.Character + "'s quest " + gameEvent.Value + ", not found in loaded story files", nodes.Positions) { FileName = gameEvent.Character! };
                                    newList.Add(quest);
                                    nodes.AddChild(newList[i], quest);
                                }
                                newList[i].Text = ((QuestActions)gameEvent.Option).ToString() + " the quest " + gameEvent.Value + " from " + gameEvent.Character;
                                break;
                            }
                            case GameEvents.RandomizeIntValue:
                            {
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Key && FileName == gameEvent.Character);
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var value = new Node(gameEvent.Key!, NodeType.Value, gameEvent.Character + " value " + gameEvent.Key, nodes.Positions) { FileName = gameEvent.Character ?? string.Empty };
                                    Values.Add(value);
                                    nodes.AddChild(newList[i], value);
                                }
                                newList[i].Text = "set " + gameEvent.Character + ":" + gameEvent.Key + " to a random value between " + gameEvent.Value + " and " + gameEvent.Value2;
                                break;
                            }
                            case GameEvents.SendEvent:
                            {
                                newList[i].Text = gameEvent.Character + " " + ((SendEvents)gameEvent.Option).ToString();
                                break;
                            }
                            case GameEvents.Social:
                            {
                                result = Socials.Find((n) => n.Type == NodeType.Social && n.ID == gameEvent.Character + ((SocialStatuses)gameEvent.Option).ToString() + gameEvent.Character2);
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add property node, hasnt been referenced yet
                                    var social = new Node(gameEvent.Character + ((SocialStatuses)gameEvent.Option).ToString() + gameEvent.Character2, NodeType.Social, gameEvent.Character + " " + ((SocialStatuses)gameEvent.Option).ToString() + " " + gameEvent.Character2, nodes.Positions) { FileName = gameEvent.Character! };
                                    Socials.Add(social);
                                    nodes.AddChild(newList[i], social);
                                }
                                newList[i].Text = gameEvent.Character + " " + ((SocialStatuses)gameEvent.Option).ToString() + " " + gameEvent.Character2 + (gameEvent.Option2 == 0 ? " Equals " : " Add ") + gameEvent.Value;
                                break;
                            }
                            case GameEvents.State:
                            {
                                result = States.Find((n) => n.Type == NodeType.State && n.FileName == gameEvent.Character && n.Text.AsSpan()[..2].Contains(gameEvent.Value!.AsSpan(), StringComparison.InvariantCulture));
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add state node, hasnt been referenced yet
                                    var state = new Node(gameEvent.Character + "State" + gameEvent.Value, NodeType.State, gameEvent.Value + "|" + ((InteractiveStates)int.Parse(gameEvent.Value!)).ToString(), nodes.Positions) { FileName = gameEvent.Character! };
                                    States.Add(state);
                                    nodes.AddChild(newList[i], state);
                                }
                                newList[i].Text = (gameEvent.Option == 0 ? "Add " : "Remove ") + gameEvent.Character + " State " + ((InteractiveStates)int.Parse(gameEvent.Value!)).ToString();
                                break;
                            }
                            case GameEvents.TriggerBGC:
                            {
                                result = newList.Find((n) => n.Type == NodeType.BGC && n.ID == "BGC" + gameEvent.Value);
                                if (result is not null)
                                {
                                    nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add property node, hasnt been referenced yet
                                    var bgc = new Node("BGC" + gameEvent.Value, NodeType.BGC, gameEvent.Character + "'s BGC " + gameEvent.Value + ", not found in loaded story files", nodes.Positions) { FileName = gameEvent.Character! };
                                    newList.Add(bgc);
                                    nodes.AddChild(newList[i], bgc);
                                }
                                newList[i].Text = "trigger " + gameEvent.Character + "'s BGC " + gameEvent.Value + " as " + ((BGCOption)gameEvent.Option).ToString();
                                break;
                            }
                            default:
                                break;
                        }
                    }
                    else if (newList[i].Type == NodeType.EventTrigger && newList[i].Data is not null)
                    {
                        trigger = (EventTrigger)newList[i].Data!;
                        //link against events
                        foreach (GameEvent _event in trigger.Events!)
                        {
                            result = newList.Find((n) => n.Type == NodeType.Event && n.ID == _event.Id);
                            if (result is not null)
                            {
                                nodes.AddChild(newList[i], result);
                            }
                            else
                            {
                                //create and add event, hasnt been referenced yet, we can not know its id if it doesnt already exist
                                var eventNode = new Node(_event.Id ?? "none", NodeType.Event, _event.Value ?? "none", nodes.Positions);
                                newList.Add(eventNode);
                                nodes.AddChild(newList[i], eventNode);
                            }
                        }
                        //link against criteria
                        foreach (Criterion _criterion in trigger.Critera!)
                        {
                            result = newList.Find((n) => n.Type == NodeType.Criterion && n.ID == $"{_criterion.Character}{_criterion.CompareType}{_criterion.Value}");
                            if (result is not null)
                            {
                                nodes.AddParent(newList[i], result);
                            }
                            else
                            {
                                newList.Add(Node.CreateCriteriaNode(_criterion, newList[i],nodes));
                            }
                        }
                        newList[i].Text = trigger.Critera.Count == 0
                            ? trigger.Name + " " + trigger.Type
                            : trigger.CharacterToReactTo + " " + trigger.Type + " " + trigger.UpdateIteration + " " + trigger.Name;
                    }
                    else if (newList[i].Type == NodeType.Response && newList[i].Data is not null)
                    {
                        var response = (Response)newList[i].Data!;
                        if (response.Next == 0)
                        {
                            continue;
                        }

                        result = newList.Find((n) => n.Type == NodeType.Dialogue && n.ID == response.Next.ToString());

                        if (result is not null)
                        {
                            nodes.AddChild(newList[i], result);
                        }
                        else
                        {
                            //create and add event, hasnt been referenced yet, we can not know its id if it doesnt already exist
                            var dialogue = new Node(response.Next.ToString(), NodeType.Dialogue, $"dialogue number {response.Next} for {newList[i].FileName}", nodes.Positions);
                            newList.Add(dialogue);
                            nodes.AddChild(newList[i], dialogue);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            //check some comparevaluenodes.again because the referenced values havent been added yet
            RecheckCompareValues(CompareValuesToCheckAgain, nodes);

            //merge doors with items if applicable
            MergeDoors(nodes);
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
                            //todo need to link here more
                            switch (node.Type)
                            {
                                case NodeType.Null:
                                    break;
                                case NodeType.CharacterGroup:
                                    break;
                                case NodeType.Criterion:
                                    break;
                                case NodeType.ItemAction:
                                    break;
                                case NodeType.ItemGroupBehaviour:
                                    break;
                                case NodeType.ItemGroupInteraction:
                                    break;
                                case NodeType.Pose:
                                    break;
                                case NodeType.Achievement:
                                    break;
                                case NodeType.BGC:
                                    break;
                                case NodeType.BGCResponse:
                                    break;
                                case NodeType.Clothing:
                                    break;
                                case NodeType.CriteriaGroup:
                                    break;
                                case NodeType.Cutscene:
                                    break;
                                case NodeType.Dialogue:
                                    var result = templist2.Find(newRefNode => newRefNode.Type == node.Type && newRefNode.ID == node.ID && newRefNode.FileName == node.FileName);
                                    if (result is not null)
                                    {
                                        stores[store].Replace(node, result);
                                        result.DupeToOtherSorting(store, node.CurrentPositionSorting);
                                    }
                                    break;
                                case NodeType.AlternateText:
                                    break;
                                case NodeType.Door:
                                    break;
                                case NodeType.Event:
                                    break;
                                case NodeType.EventTrigger:
                                    break;
                                case NodeType.Inventory:
                                    break;
                                case NodeType.Item:
                                    break;
                                case NodeType.ItemGroup:
                                    break;
                                case NodeType.Personality:
                                    break;
                                case NodeType.Property:
                                    break;
                                case NodeType.Quest:
                                    break;
                                case NodeType.Response:
                                    break;
                                case NodeType.Social:
                                    break;
                                case NodeType.State:
                                    break;
                                case NodeType.Value:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }
        private static void MergeDoors(NodeStore nodes)
        {
            Node? result;
            var newlist = nodes.KeyNodes().ToList();
            foreach (Node door in Doors.ToArray())
            {
                result = newlist.Find((n) => n.ID == door.ID);
                if (result is not null)
                {
                    foreach (Node parentNode in nodes.Parents(door).ToArray())
                    {
                        nodes.AddChild(parentNode, result);
                        nodes.RemoveChild(parentNode, door);
                    }
                    foreach (Node childNode in nodes.Childs(door).ToArray())
                    {
                        nodes.AddParent(childNode, result);
                        nodes.RemoveParent(childNode, door);
                    }
                    Doors.Remove(door);
                }
            }
        }

        private static void RecheckCompareValues(List<Node> CompareValuesToCheckAgain, NodeStore nodes)
        {
            Node? result;
            foreach (Node node in CompareValuesToCheckAgain)
            {
                if (node.DataType == typeof(Criterion))
                {
                    var criterion = (Criterion)node.Data!;
                    result = Values.Find((n) => n.Type == NodeType.Value && n.ID == criterion.Key);
                    if (result is not null)
                    {
                        nodes.AddParent(node, result);
                    }

                    result = Values.Find((n) => n.Type == NodeType.Value && n.ID == criterion.Key2);
                    if (result is not null)
                    {
                        nodes.AddParent(node, result);
                    }
                }
            }
        }

        public static void DissectCharacter(CharacterStory story, NodeStore nodes)
        {
            if (story is not null && nodes is not null)
            {
                FileName = story.CharacterName;
                //get all relevant items from the json
                StoryNodeExtractor.GetItems(story, nodes);
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
                        nodes.Nodes[i].FileName = StoryName ?? string.Empty;
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
