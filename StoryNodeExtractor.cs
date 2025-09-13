using CSC.StoryItems;

namespace CSC
{
    public static class StoryNodeExtractor
    {
        public static void GetAchievements(MainStory story)
        {
            //list to collect all achievement Main.nodes
            //go through all of them
            foreach (Achievement achievement in story.Achievements ?? [])
            {
                //node to add the description as child to, needs reference to parent, hence can't be anonymous
                var node = new Node(achievement.Id ?? string.Empty, NodeType.Achievement, achievement.Name ?? string.Empty) { Data = achievement, DataType = typeof(Achievement) };
                Main.nodes.AddChild(node, new Node(achievement.Id + "Description", NodeType.Achievement, achievement.Description ?? string.Empty, node));
                Main.nodes.AddChild(node, new Node(achievement.Id + "SteamName", NodeType.Achievement, achievement.SteamName ?? string.Empty, node));
                //add achievement with description child to list
                Main.nodes.Add(node);
            }

            //return list of achievements
        }

        public static void GetBackGroundChatter(CharacterStory story)
        {
            foreach (BackgroundChatter backgroundChatter in story.BackgroundChatter ?? [])
            {
                var bgcNode = new Node($"BGC{backgroundChatter.Id}", NodeType.BGC, backgroundChatter.Text ?? string.Empty) { Data = backgroundChatter, DataType = typeof(BackgroundChatter) };

                //criteria
                bgcNode.AddCriteria(backgroundChatter.Critera ?? []);

                //startevents
                bgcNode.AddEvents(backgroundChatter.StartEvents ?? []);

                //responses
                foreach (BackgroundChatterResponse response in backgroundChatter.Responses ?? [])
                {
                    var nodeResponse = new Node($"{response.CharacterName}{response.ChatterId}", NodeType.BGCResponse, "see id", bgcNode) { Data = response, DataType = typeof(Response) };

                    Main.nodes.AddChild(bgcNode, nodeResponse);
                }
                Main.nodes.Add(bgcNode);
            }
        }

        public static void GetDialogues(CharacterStory story)
        {
            var responseDialogueLinks = new List<Tuple<Node, int>>();

            foreach (Dialogue dialogue in story.Dialogues ?? [])
            {
                var nodeDialogue = new Node(dialogue.ID.ToString(), NodeType.Dialogue, dialogue.Text ?? string.Empty) { Data = dialogue, DataType = typeof(Dialogue) };
                int alternateTextCounter = 1;

                //add all alternate texts to teh dialogue
                foreach (AlternateText alternateText in dialogue.AlternateTexts ?? [])
                {
                    var nodeAlternateText = new Node($"{dialogue.ID}.{alternateTextCounter}", NodeType.Dialogue, alternateText.Text ?? string.Empty, nodeDialogue) { Data = alternateText, DataType = typeof(AlternateText) };

                    //increasse counter to ensure valid id
                    alternateTextCounter++;

                    nodeAlternateText.AddCriteria(alternateText.Critera ?? []);

                    //add alternate to the default text as a child, parent already set on the child
                    Main.nodes.AddChild(nodeDialogue, nodeAlternateText);
                }

                //some events in here may have strings that are connected to the dialogue closing
                nodeDialogue.AddEvents(dialogue.CloseEvents ?? []);

                //add all responses as childs to this dialogue
                foreach (Response response in dialogue.Responses ?? [])
                {
                    var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty, nodeDialogue) { Data = response, DataType = typeof(Response) };

                    nodeResponse.AddCriteria(response.ResponseCriteria ?? []);

                    //check all responses to this dialogue
                    nodeResponse.AddEvents(response.ResponseEvents ?? []);

                    if (response.Next != 0)
                    {
                        responseDialogueLinks.Add(new Tuple<Node, int>(nodeResponse, response.Next));
                    }

                    Main.nodes.AddChild(nodeDialogue, nodeResponse);
                }

                //add the starting events
                nodeDialogue.AddEvents(dialogue.StartEvents ?? []);

                //finally add node
                Main.nodes.Add(nodeDialogue);
            }

            var list = Main.nodes.KeyNodes().ToList();

            //link up the dialogues and responses/next dialogues
            foreach (Tuple<Node, int> next in responseDialogueLinks)
            {
                Node node = list.Find(n => n.ID == next.Item2.ToString()) ?? Node.NullNode;
                if (node == Node.NullNode)
                {
                    continue;
                }
            }

            responseDialogueLinks.Clear();
        }

        public static void GetGlobalGoodByeResponses(CharacterStory story)
        {
            //add all responses as childs to this dialogue
            foreach (Response response in story.GlobalGoodbyeResponses ?? [])
            {
                var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty) { Data = response, DataType = typeof(Response) };

                nodeResponse.AddCriteria(response.ResponseCriteria ?? []);

                //check all responses to this dialogue
                nodeResponse.AddEvents(response.ResponseEvents ?? []);

                Main.nodes.Add(nodeResponse);
            }
        }

        public static void GetGlobalResponses(CharacterStory story)
        {
            foreach (Response response in story.GlobalResponses ?? [])
            {
                var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty) { Data = response, DataType = typeof(Response) };

                nodeResponse.AddCriteria(response.ResponseCriteria ?? []);

                //check all responses to this dialogue
                nodeResponse.AddEvents(response.ResponseEvents ?? []);

                Main.nodes.Add(nodeResponse);
            }
        }

        public static void GetItemGroupBehaviours(MainStory story)
        {
            //go through all item groups to find events
            foreach (ItemGroupBehavior itemGroupBehaviour in story.ItemGroupBehaviors ?? [])
            {
                if (itemGroupBehaviour is null)
                {
                    continue;
                }
                //create item group node to add events/criteria to
                var nodeGroup = new Node(itemGroupBehaviour.Id ?? string.Empty, NodeType.ItemGroupBehaviour, itemGroupBehaviour.Name ?? string.Empty) { Data = itemGroupBehaviour, DataType = typeof(ItemGroupBehavior) };
                //get actions for item
                foreach (ItemAction itemAction in itemGroupBehaviour.ItemActions ?? [])
                {
                    //node to addevents to
                    var nodeAction = new Node(itemAction.ActionName ?? string.Empty, NodeType.ItemAction, itemAction.ActionName ?? string.Empty, nodeGroup) { Data = itemAction, DataType = typeof(ItemAction) };

                    //add text that is shown when item is taken
                    nodeAction.AddEvents(itemAction.OnTakeActionEvents ?? []);

                    //add criteria that influence this item
                    nodeAction.AddCriteria(itemAction.Criteria ?? []);

                    //add action to item
                    Main.nodes.AddChild(nodeGroup, nodeAction);
                }

                //add item gruop with everything to collecting list
                Main.nodes.Add(nodeGroup);
            }
        }

        public static void GetItemGroups(MainStory story)
        {
            //go through all item groups to find events
            foreach (ItemGroup itemGroup in story.ItemGroups ?? [])
            {
                if (itemGroup is null)
                {
                    continue;
                }
                //create item group node to add events/criteria to
                var nodeGroup = new Node(itemGroup.Id ?? string.Empty, NodeType.ItemGroup, itemGroup.Name ?? string.Empty) { Data = itemGroup, DataType = typeof(ItemGroup) };
                //get actions for item
                foreach (string item in itemGroup.ItemsInGroup ?? [])
                {
                    //node to addevents to
                    var nodeItem = new Node(item ?? string.Empty, NodeType.Item, item ?? string.Empty, nodeGroup) { Data = item, DataType = typeof(string) };

                    //add item to item group
                    Main.nodes.AddChild(nodeGroup, nodeItem);
                }

                //add item gruop with everything to collecting list
                Main.nodes.Add(nodeGroup);
            }
        }

        public static void GetItemOverrides(MainStory story)
        {
            //go through all Main.nodes to search them for actions
            foreach (ItemOverride itemOverride in story.ItemOverrides ?? [])
            {
                //add items to list
                var nodeItem = new Node(itemOverride.Id ?? string.Empty, NodeType.Item, itemOverride.DisplayName ?? string.Empty) { Data = itemOverride, DataType = typeof(ItemOverride) };
                //get actions for item
                foreach (ItemAction itemAction in itemOverride.ItemActions ?? [])
                {
                    //create action node to add criteria and events to
                    var nodeAction = new Node(itemAction.ActionName ?? string.Empty, NodeType.ItemAction, itemAction.ActionName ?? string.Empty, nodeItem) { Data = itemAction, DataType = typeof(ItemAction) };

                    //add text that is shown when item is taken
                    nodeAction.AddEvents(itemAction.OnTakeActionEvents ?? []);

                    //add criteria that influence this item
                    nodeAction.AddCriteria(itemAction.Criteria ?? []);

                    //add action to item
                    Main.nodes.AddChild(nodeItem, nodeAction);

                    //add action to node list for later use
                    Main.nodes.Add(nodeAction);
                }

                //get actions for item
                foreach (UseWith use in itemOverride.UseWiths ?? [])
                {
                    //node to add all references to
                    var useNode = new Node(use.ItemName ?? string.Empty, NodeType.Item, use.CustomCantDoThatMessage ?? string.Empty, nodeItem) { Data = use, DataType = typeof(UseWith) };

                    //add criteria that influence this item
                    useNode.AddCriteria(use.Criteria ?? []);

                    //add action to item
                    useNode.AddEvents(use.OnSuccessEvents ?? []);

                    //add note to list for later
                    Main.nodes.Add(useNode);
                }

                //add item with all child Main.nodes to collector list
                Main.nodes.Add(nodeItem);
            }
        }

        public static void GetPlayerReactions(MainStory story)
        {
            foreach (EventTrigger playerReaction in story.PlayerReactions ?? [])
            {
                //add items to list
                var nodeReaction = new Node(playerReaction.Id ?? string.Empty, NodeType.EventTrigger, playerReaction.Name ?? string.Empty) { Data = playerReaction, DataType = typeof(EventTrigger) };

                //get actions for item
                nodeReaction.AddEvents(playerReaction.Events ?? []);

                nodeReaction.AddCriteria(playerReaction.Critera ?? []);

                Main.nodes.Add(nodeReaction);
            }
        }

        public static void GetQuests(CharacterStory story)
        {

            foreach (Quest quest in story.Quests ?? [])
            {
                var nodeQuest = new Node(quest.ID ?? string.Empty, NodeType.Quest, quest.Name ?? string.Empty) { Data = quest, DataType = typeof(Quest) };

                //Add details
                if (quest.Details?.Length > 0)
                {
                    Main.nodes.AddChild(nodeQuest, new Node($"{quest.ID}Description", NodeType.Quest, quest.Details));
                }
                //Add completed details
                if (quest.CompletedDetails?.Length > 0)
                {
                    Main.nodes.AddChild(nodeQuest, new Node($"{quest.ID}CompletedDetails", NodeType.Quest, quest.CompletedDetails));
                }
                //Add failed details
                if (quest.FailedDetails?.Length > 0)
                {
                    Main.nodes.AddChild(nodeQuest, new Node($"{quest.ID}FailedDetails", NodeType.Quest, quest.FailedDetails));
                }

                //Add extended details

                foreach (ExtendedDetail detail in quest.ExtendedDetails ?? [])
                {
                    Main.nodes.AddChild(nodeQuest, new Node($"{quest.ID}Description{detail.Value}", NodeType.Quest, detail.Details ?? string.Empty) { Data = detail, DataType = typeof(ExtendedDetail) });
                }

                Main.nodes.Add(nodeQuest);
            }
        }

        public static void GetReactions(CharacterStory story)
        {
            foreach (EventTrigger playerReaction in story.Reactions ?? [])
            {
                //add items to list
                var nodeReaction = new Node(playerReaction.Id ?? string.Empty, NodeType.EventTrigger, playerReaction.Name ?? string.Empty) { Data = playerReaction, DataType = typeof(EventTrigger) };
                //get actions for item
                nodeReaction.AddEvents(playerReaction.Events ?? []);

                nodeReaction.AddCriteria(playerReaction.Critera ?? []);

                Main.nodes.Add(nodeReaction);
            }
        }

        public static void GetCriteriaGroups(MainStory story)
        {
            foreach (CriteriaGroup group in story.CriteriaGroups ?? [])
            {
                //add items to list
                var nodeCriteriaGroup = new Node(group.Id!, NodeType.CriteriaGroup, group.Name + " True if " + group.PassCondition) { Data = group, DataType = typeof(CriteriaGroup) };

                foreach (CriteriaList1 criteriaList in group.CriteriaList ?? [])
                {
                    nodeCriteriaGroup.AddCriteria(criteriaList.CriteriaList ?? []);
                }

                Main.nodes.Add(nodeCriteriaGroup);
            }
        }

        public static void GetPersonality(CharacterStory story)
        {
            foreach (Trait valuee in story.Personality?.Values ?? [])
            {
                //add items to list
                var nodeValue = new Node(valuee.Type.ToString()!, NodeType.Personality, story.CharacterName + " " + valuee.Type + " " + valuee.Value) { Data = valuee, DataType = typeof(Trait) };
                Main.nodes.Add(nodeValue);
            }
        }

        public static void GetItems(CharacterStory story)
        {
            foreach (StoryItem item in story.StoryItems ?? [])
            {
                //add items to list
                var nodeItem = new Node(item.ItemName!, NodeType.Item, item.ItemName!) { Data = item, DataType = typeof(StoryItem) };
                nodeItem.AddCriteria(item.Critera ?? []);
                nodeItem.AddEvents(item.OnRefuseEvents ?? []);
                nodeItem.AddEvents(item.OnAcceptEvents ?? []);
                Main.nodes.Add(nodeItem);
            }
        }

        public static void GetValues(CharacterStory story)
        {
            foreach (string value in story.StoryValues ?? [])
            {
                //add items to list
                var nodeValue = new Node(value!, NodeType.Value, story.CharacterName + value + ", referenced values: ") { Data = new Value() { value = value }, DataType = typeof(Value) };
                Main.nodes.Add(nodeValue);
            }
        }

        public static void GetValues(MainStory story)
        {
            foreach (string value in story.PlayerValues ?? [])
            {
                //add items to list
                var nodeValue = new Node(value, NodeType.Value, "Player " + value + ", referenced values: ") { Data = new Value() { value = value }, DataType = typeof(Value) };
                Main.nodes.Add(nodeValue);
            }
        }

        public static void GetGameStartEvents(MainStory story)
        {
            var nodeEvents = new Node("GameStartEvents"!, NodeType.EventTrigger, "GameStartEvents");
            foreach (GameEvent _event in story.GameStartEvents ?? [])
            {
                var nodeEvent = new Node(_event.Id ?? "none", NodeType.Event, _event.Value ?? "none") { Data = _event, DataType = typeof(GameEvent) };

                nodeEvent.AddCriteria(_event.Criteria ?? []);

                Main.nodes.AddChild(nodeEvents, nodeEvent);
            }
            Main.nodes.Add(nodeEvents);
        }
    }
}