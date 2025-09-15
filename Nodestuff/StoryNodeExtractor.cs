using CSC.StoryItems;
using System.Xml.Schema;

namespace CSC.Nodestuff
{
    public static class StoryNodeExtractor
    {
        public static void GetAchievements(MainStory story, NodeStore nodes)
        {
            //list to collect all achievement Main.nodes
            //go through all of them
            foreach (Achievement achievement in story.Achievements ?? [])
            {
                //node to add the description as child to, needs reference to parent, hence can't be anonymous
                var node = new Node(achievement.Id ?? string.Empty, NodeType.Achievement, achievement.Name ?? string.Empty, nodes.Positions) { Data = achievement, DataType = typeof(Achievement) };
                nodes.AddChild(node, new Node(achievement.Id + "Description", NodeType.Achievement, achievement.Description ?? string.Empty, node, nodes.Positions));
                nodes.AddChild(node, new Node(achievement.Id + "SteamName", NodeType.Achievement, achievement.SteamName ?? string.Empty, node, nodes.Positions));
                //add achievement with description child to list
                nodes.Add(node);
            }

            //return list of achievements
        }

        public static void GetBackGroundChatter(CharacterStory story, NodeStore nodes)
        {
            foreach (BackgroundChatter backgroundChatter in story.BackgroundChatter ?? [])
            {
                var bgcNode = new Node($"BGC{backgroundChatter.Id}", NodeType.BGC, backgroundChatter.Text ?? string.Empty, nodes.Positions) { Data = backgroundChatter, DataType = typeof(BackgroundChatter) };

                //criteria
                bgcNode.AddCriteria(backgroundChatter.Critera ?? [], nodes);

                //startevents
                bgcNode.AddEvents(backgroundChatter.StartEvents ?? [], nodes);

                //responses
                foreach (BackgroundChatterResponse response in backgroundChatter.Responses ?? [])
                {
                    var nodeResponse = new Node($"{response.CharacterName}{response.ChatterId}", NodeType.BGCResponse, "see id", bgcNode, nodes.Positions) { Data = response, DataType = typeof(Response) };

                    nodes.AddChild(bgcNode, nodeResponse);
                }
                nodes.Add(bgcNode);
            }
        }

        public static void GetDialogues(CharacterStory story, NodeStore nodes)
        {
            var responseDialogueLinks = new List<Tuple<Node, int>>();

            foreach (Dialogue dialogue in story.Dialogues ?? [])
            {
                var nodeDialogue = new Node(dialogue.ID.ToString(), NodeType.Dialogue, dialogue.Text ?? string.Empty, nodes.Positions) { Data = dialogue, DataType = typeof(Dialogue), FileName = story.CharacterName! };
                int alternateTextCounter = 1;

                //add all alternate texts to teh dialogue
                foreach (AlternateText alternateText in dialogue.AlternateTexts ?? [])
                {
                    var nodeAlternateText = new Node($"{dialogue.ID}.{alternateTextCounter}", NodeType.Dialogue, alternateText.Text ?? string.Empty, nodeDialogue, nodes.Positions) { Data = alternateText, DataType = typeof(AlternateText), FileName = story.CharacterName! };

                    //increasse counter to ensure valid id
                    alternateTextCounter++;

                    nodeAlternateText.AddCriteria(alternateText.Critera ?? [], nodes);

                    //add alternate to the default text as a child, parent already set on the child
                    nodes.AddChild(nodeDialogue, nodeAlternateText);
                }

                //some events in here may have strings that are connected to the dialogue closing
                nodeDialogue.AddEvents(dialogue.CloseEvents ?? [], nodes);

                //add all responses as childs to this dialogue
                foreach (Response response in dialogue.Responses ?? [])
                {
                    var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty, nodeDialogue, nodes.Positions) { Data = response, DataType = typeof(Response), FileName = story.CharacterName! };

                    nodeResponse.AddCriteria(response.ResponseCriteria ?? [], nodes);

                    //check all responses to this dialogue
                    nodeResponse.AddEvents(response.ResponseEvents ?? [], nodes);

                    if (response.Next != 0)
                    {
                        responseDialogueLinks.Add(new Tuple<Node, int>(nodeResponse, response.Next));
                    }

                    nodes.AddChild(nodeDialogue, nodeResponse);
                }

                //add the starting events
                nodeDialogue.AddEvents(dialogue.StartEvents ?? [], nodes);

                //finally add node
                nodes.Add(nodeDialogue);
            }

            var list = nodes.KeyNodes().ToList();

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

        public static void GetGlobalGoodByeResponses(CharacterStory story, NodeStore nodes)
        {
            //add all responses as childs to this dialogue
            foreach (Response response in story.GlobalGoodbyeResponses ?? [])
            {
                var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty, nodes.Positions) { Data = response, DataType = typeof(Response) };

                nodeResponse.AddCriteria(response.ResponseCriteria ?? [], nodes);

                //check all responses to this dialogue
                nodeResponse.AddEvents(response.ResponseEvents ?? [], nodes);

                nodes.Add(nodeResponse);
            }
        }

        public static void GetGlobalResponses(CharacterStory story, NodeStore nodes)
        {
            foreach (Response response in story.GlobalResponses ?? [])
            {
                var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty, nodes.Positions) { Data = response, DataType = typeof(Response) };

                nodeResponse.AddCriteria(response.ResponseCriteria ?? [], nodes);

                //check all responses to this dialogue
                nodeResponse.AddEvents(response.ResponseEvents ?? [], nodes);

                nodes.Add(nodeResponse);
            }
        }

        public static void GetItemGroupBehaviours(MainStory story, NodeStore nodes)
        {
            //go through all item groups to find events
            foreach (ItemGroupBehavior itemGroupBehaviour in story.ItemGroupBehaviors ?? [])
            {
                if (itemGroupBehaviour is null)
                {
                    continue;
                }
                //create item group node to add events/criteria to
                var nodeGroup = new Node(itemGroupBehaviour.Id ?? string.Empty, NodeType.ItemGroupBehaviour, itemGroupBehaviour.Name ?? string.Empty, nodes.Positions) { Data = itemGroupBehaviour, DataType = typeof(ItemGroupBehavior) };
                //get actions for item
                foreach (ItemAction itemAction in itemGroupBehaviour.ItemActions ?? [])
                {
                    //node to addevents to
                    var nodeAction = new Node(itemAction.ActionName ?? string.Empty, NodeType.ItemAction, itemAction.ActionName ?? string.Empty, nodeGroup, nodes.Positions) { Data = itemAction, DataType = typeof(ItemAction) };

                    //add text that is shown when item is taken
                    nodeAction.AddEvents(itemAction.OnTakeActionEvents ?? [], nodes);

                    //add criteria that influence this item
                    nodeAction.AddCriteria(itemAction.Criteria ?? [], nodes);

                    //add action to item
                    nodes.AddChild(nodeGroup, nodeAction);
                }

                //add item gruop with everything to collecting list
                nodes.Add(nodeGroup);
            }
        }

        public static void GetItemGroups(MainStory story, NodeStore nodes)
        {
            //go through all item groups to find events
            foreach (ItemGroup itemGroup in story.ItemGroups ?? [])
            {
                if (itemGroup is null)
                {
                    continue;
                }
                //create item group node to add events/criteria to
                var nodeGroup = new Node(itemGroup.Id ?? string.Empty, NodeType.ItemGroup, itemGroup.Name ?? string.Empty, nodes.Positions) { Data = itemGroup, DataType = typeof(ItemGroup) };
                //get actions for item
                foreach (string item in itemGroup.ItemsInGroup ?? [])
                {
                    //node to addevents to
                    var nodeItem = new Node(item ?? string.Empty, NodeType.Item, item ?? string.Empty, nodeGroup, nodes.Positions) { Data = item, DataType = typeof(string) };

                    //add item to item group
                    nodes.AddChild(nodeGroup, nodeItem);
                }

                //add item gruop with everything to collecting list
                nodes.Add(nodeGroup);
            }
        }

        public static void GetItemOverrides(MainStory story, NodeStore nodes)
        {
            //go through all Main.nodes to search them for actions
            foreach (ItemOverride itemOverride in story.ItemOverrides ?? [])
            {
                //add items to list
                var nodeItem = new Node(itemOverride.Id ?? string.Empty, NodeType.Item, itemOverride.DisplayName ?? string.Empty, nodes.Positions) { Data = itemOverride, DataType = typeof(ItemOverride) };
                //get actions for item
                foreach (ItemAction itemAction in itemOverride.ItemActions ?? [])
                {
                    //create action node to add criteria and events to
                    var nodeAction = new Node(itemAction.ActionName ?? string.Empty, NodeType.ItemAction, itemAction.ActionName ?? string.Empty, nodeItem, nodes.Positions) { Data = itemAction, DataType = typeof(ItemAction)};

                    //add text that is shown when item is taken
                    nodeAction.AddEvents(itemAction.OnTakeActionEvents ?? [], nodes);

                    //add criteria that influence this item
                    nodeAction.AddCriteria(itemAction.Criteria ?? [], nodes);

                    //add action to item
                    nodes.AddChild(nodeItem, nodeAction);

                    //add action to node list for later use
                    nodes.Add(nodeAction);
                }

                //get actions for item
                foreach (UseWith use in itemOverride.UseWiths ?? [])
                {
                    //node to add all references to
                    var useNode = new Node(use.ItemName ?? string.Empty, NodeType.Item, use.CustomCantDoThatMessage ?? string.Empty, nodeItem, nodes.Positions) { Data = use, DataType = typeof(UseWith) };

                    //add criteria that influence this item
                    useNode.AddCriteria(use.Criteria ?? [], nodes);

                    //add action to item
                    useNode.AddEvents(use.OnSuccessEvents ?? [], nodes);

                    //add note to list for later
                    nodes.Add(useNode);
                }

                //add item with all child Main.nodes to collector list
                nodes.Add(nodeItem);
            }
        }

        public static void GetPlayerReactions(MainStory story, NodeStore nodes)
        {
            foreach (EventTrigger playerReaction in story.PlayerReactions ?? [])
            {
                //add items to list
                var nodeReaction = new Node(playerReaction.Id ?? string.Empty, NodeType.EventTrigger, playerReaction.Name ?? string.Empty, nodes.Positions) { Data = playerReaction, DataType = typeof(EventTrigger) };

                //get actions for item
                nodeReaction.AddEvents(playerReaction.Events ?? [], nodes);

                nodeReaction.AddCriteria(playerReaction.Critera ?? [], nodes);

                nodes.Add(nodeReaction);
            }
        }

        public static void GetQuests(CharacterStory story, NodeStore nodes)
        {

            foreach (Quest quest in story.Quests ?? [])
            {
                var nodeQuest = new Node(quest.ID ?? string.Empty, NodeType.Quest, quest.Name ?? string.Empty, nodes.Positions) { Data = quest, DataType = typeof(Quest) };

                //Add details
                if (quest.Details?.Length > 0)
                {
                    nodes.AddChild(nodeQuest, new Node($"{quest.ID}Description", NodeType.Quest, quest.Details, nodes.Positions));
                }
                //Add completed details
                if (quest.CompletedDetails?.Length > 0)
                {
                    nodes.AddChild(nodeQuest, new Node($"{quest.ID}CompletedDetails", NodeType.Quest, quest.CompletedDetails, nodes.Positions));
                }
                //Add failed details
                if (quest.FailedDetails?.Length > 0)
                {
                    nodes.AddChild(nodeQuest, new Node($"{quest.ID}FailedDetails", NodeType.Quest, quest.FailedDetails, nodes.Positions));
                }

                //Add extended details

                foreach (ExtendedDetail detail in quest.ExtendedDetails ?? [])
                {
                    nodes.AddChild(nodeQuest, new Node($"{quest.ID}Description{detail.Value}", NodeType.Quest, detail.Details ?? string.Empty, nodes.Positions) { Data = detail, DataType = typeof(ExtendedDetail) });
                }

                nodes.Add(nodeQuest);
            }
        }

        public static void GetReactions(CharacterStory story, NodeStore nodes)
        {
            foreach (EventTrigger playerReaction in story.Reactions ?? [])
            {
                //add items to list
                var nodeReaction = new Node(playerReaction.Id ?? string.Empty, NodeType.EventTrigger, playerReaction.Name ?? string.Empty, nodes.Positions) { Data = playerReaction, DataType = typeof(EventTrigger) , FileName = story.CharacterName!};
                //get actions for item
                nodeReaction.AddEvents(playerReaction.Events ?? [], nodes);

                nodeReaction.AddCriteria(playerReaction.Critera ?? [], nodes);

                nodes.Add(nodeReaction);
            }
        }

        public static void GetCriteriaGroups(MainStory story, NodeStore nodes)
        {
            foreach (CriteriaGroup group in story.CriteriaGroups ?? [])
            {
                //add items to list
                var nodeCriteriaGroup = new Node(group.Id!, NodeType.CriteriaGroup, group.Name + " True if " + group.PassCondition, nodes.Positions) { Data = group, DataType = typeof(CriteriaGroup) };

                foreach (CriteriaList1 criteriaList in group.CriteriaList ?? [])
                {
                    nodeCriteriaGroup.AddCriteria(criteriaList.CriteriaList ?? [], nodes);
                }

                nodes.Add(nodeCriteriaGroup);
            }
        }

        public static void GetPersonality(CharacterStory story, NodeStore nodes)
        {
            foreach (Trait valuee in story.Personality?.Values ?? [])
            {
                //add items to list
                var nodeValue = new Node(valuee.Type.ToString()!, NodeType.Personality, story.CharacterName + " " + valuee.Type + " " + valuee.Value, nodes.Positions) { Data = valuee, DataType = typeof(Trait) };
                nodes.Add(nodeValue);
            }
        }

        public static void GetItems(CharacterStory story, NodeStore nodes)
        {
            foreach (StoryItem item in story.StoryItems ?? [])
            {
                //add items to list
                var nodeItem = new Node(item.ItemName!, NodeType.Item, item.ItemName!, nodes.Positions) { Data = item, DataType = typeof(StoryItem) };
                nodeItem.FileName = story.CharacterName!;
                nodeItem.AddCriteria(item.Critera ?? [], nodes);
                nodeItem.AddEvents(item.OnRefuseEvents ?? [], nodes);
                nodeItem.AddEvents(item.OnAcceptEvents ?? [], nodes);
                nodes.Add(nodeItem);
            }
        }

        public static void GetValues(CharacterStory story, NodeStore nodes)
        {
            foreach (string value in story.StoryValues ?? [])
            {
                //add items to list
                var nodeValue = new Node(value!, NodeType.Value, story.CharacterName + value + ", referenced values: ", nodes.Positions) { Data = new Value() { value = value }, DataType = typeof(Value) };
                nodes.Add(nodeValue);
            }
        }

        public static void GetValues(MainStory story, NodeStore nodes)
        {
            foreach (string value in story.PlayerValues ?? [])
            {
                //add items to list
                var nodeValue = new Node(value, NodeType.Value, "Player " + value + ", referenced values: ", nodes.Positions) { Data = new Value() { value = value }, DataType = typeof(Value) };
                nodes.Add(nodeValue);
            }
        }

        public static void GetGameStartEvents(MainStory story, NodeStore nodes)
        {
            var nodeEvents = new Node("GameStartEvents"!, NodeType.EventTrigger, "GameStartEvents", nodes.Positions);
            foreach (GameEvent _event in story.GameStartEvents ?? [])
            {
                var nodeEvent = new Node(_event.Id ?? "none", NodeType.Event, _event.Value ?? "none", nodes.Positions) { Data = _event, DataType = typeof(GameEvent) };

                nodeEvent.AddCriteria(_event.Criteria ?? [], nodes);

                nodes.AddChild(nodeEvents, nodeEvent);
            }
            nodes.Add(nodeEvents);
        }
    }
}