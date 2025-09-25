using CSC.StoryItems;
using System.Xml.Schema;

namespace CSC.Nodestuff
{
    public static class StoryNodeExtractor
    {
        //todo make all node creations the same for each type
        public static void GetAchievements(MainStory story, NodeStore nodes)
        {
            //list to collect all achievement Main.nodes
            //go through all of them
            foreach (Achievement achievement in story.Achievements ?? [])
            {
                //node to add the description as child to, needs reference to parent, hence can't be anonymous
                var node = new Node(achievement.Id ?? string.Empty, NodeType.Achievement, achievement.Name ?? string.Empty, nodes.Positions) { rawData = achievement };
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
                var bgcNode = new Node($"BGC{backgroundChatter.Id}", NodeType.BGC, backgroundChatter.Text ?? string.Empty, nodes.Positions) { rawData = backgroundChatter };

                //criteria
                bgcNode.AddCriteria(backgroundChatter.Critera ?? [], nodes);

                //startevents
                bgcNode.AddEvents(backgroundChatter.StartEvents ?? [], nodes);

                //responses
                foreach (BackgroundChatterResponse response in backgroundChatter.Responses ?? [])
                {
                    var nodeResponse = new Node($"{response.CharacterName}{response.ChatterId}", NodeType.BGCResponse, "see id", bgcNode, nodes.Positions) { rawData = response };

                    nodes.AddChild(bgcNode, nodeResponse);
                }
                nodes.Add(bgcNode);
            }
        }

        public static void GetCriteriaGroups(MainStory story, NodeStore nodes)
        {
            foreach (CriteriaGroup group in story.CriteriaGroups ?? [])
            {
                //add items to list
                var nodeCriteriaGroup = new Node(group.Id!, NodeType.CriteriaGroup, group.Name + " True if " + group.PassCondition, nodes.Positions) { rawData = group };

                foreach (CriteriaList1 criteriaList in group.CriteriaList ?? [])
                {
                    nodeCriteriaGroup.AddCriteria(criteriaList.CriteriaList ?? [], nodes);
                }

                nodes.Add(nodeCriteriaGroup);
            }
        }

        public static void GetDialogues(CharacterStory story, NodeStore nodes)
        {
            var responseDialogueLinks = new List<Tuple<Node, int>>();

            foreach (Dialogue dialogue in story.Dialogues ?? [])
            {
                var nodeDialogue = new Node(dialogue.ID.ToString(), NodeType.Dialogue, dialogue.Text ?? string.Empty, nodes.Positions) { rawData = dialogue, FileName = story.CharacterName! };
                int alternateTextCounter = 1;

                //add all alternate texts to teh dialogue
                foreach (AlternateText alternateText in dialogue.AlternateTexts ?? [])
                {
                    var nodeAlternateText = new Node($"{dialogue.ID}.{alternateText.Order}", NodeType.AlternateText, alternateText.Text ?? string.Empty, nodeDialogue, nodes.Positions) { rawData = alternateText, FileName = story.CharacterName! };

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
                    var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty, nodeDialogue, nodes.Positions) { rawData = response, FileName = story.CharacterName! };

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

        public static void GetGameStartEvents(MainStory story, NodeStore nodes)
        {
            var nodeEvents = new Node("GameStartEvents"!, NodeType.EventTrigger, "GameStartEvents", nodes.Positions);
            foreach (GameEvent _event in story.GameStartEvents ?? [])
            {
                var nodeEvent = new Node(_event.Id ?? "none", NodeType.GameEvent, _event.Value ?? "none", nodes.Positions) { rawData = _event, FileName = Main.Player };

                nodeEvent.AddCriteria(_event.Criteria ?? [], nodes);

                nodes.AddChild(nodeEvents, nodeEvent);
            }
            nodes.Add(nodeEvents);
        }

        public static void GetGlobalGoodByeResponses(CharacterStory story, NodeStore nodes)
        {
            //add all responses as childs to this dialogue
            foreach (Response response in story.GlobalGoodbyeResponses ?? [])
            {
                var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty, nodes.Positions) { rawData = response };

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
                var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty, nodes.Positions) { rawData = response };

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
                var nodeGroup = new Node(itemGroupBehaviour.Id ?? string.Empty, NodeType.ItemGroupBehaviour, itemGroupBehaviour.Name ?? string.Empty, nodes.Positions) { rawData = itemGroupBehaviour };
                //get actions for item
                foreach (ItemAction itemAction in itemGroupBehaviour.ItemActions ?? [])
                {
                    //node to addevents to
                    var nodeAction = new Node(itemAction.ActionName ?? string.Empty, NodeType.ItemAction, itemAction.ActionName ?? string.Empty, nodeGroup, nodes.Positions) { rawData = itemAction };

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
                var nodeGroup = new Node(itemGroup.Id ?? string.Empty, NodeType.ItemGroup, itemGroup.Name ?? string.Empty, nodes.Positions) { rawData = itemGroup };
                //get actions for item
                foreach (string item in itemGroup.ItemsInGroup ?? [])
                {
                    //node to addevents to
                    var nodeItem = new Node(item ?? string.Empty, NodeType.StoryItem, item ?? string.Empty, nodeGroup, nodes.Positions) { rawData = item };

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
                var nodeItem = new Node(itemOverride.Id ?? string.Empty, NodeType.StoryItem, itemOverride.DisplayName ?? string.Empty, nodes.Positions) { rawData = itemOverride };
                //get actions for item
                foreach (ItemAction itemAction in itemOverride.ItemActions ?? [])
                {
                    //create action node to add criteria and events to
                    var nodeAction = new Node(itemAction.ActionName ?? string.Empty, NodeType.ItemAction, itemAction.ActionName ?? string.Empty, nodeItem, nodes.Positions) { rawData = itemAction };

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
                    var useNode = new Node(use.ItemName ?? string.Empty, NodeType.UseWith, use.CustomCantDoThatMessage ?? string.Empty, nodeItem, nodes.Positions) { rawData = use };

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

        public static void GetItemInteractions(CharacterStory story, NodeStore nodes)
        {
            foreach (ItemInteraction item in story.StoryItems ?? [])
            {
                //add items to list
                var nodeItem = new Node(item.ItemName!, NodeType.ItemInteraction, item.ItemName!, nodes.Positions) { rawData = item };
                nodeItem.FileName = story.CharacterName!;
                nodeItem.AddCriteria(item.Critera ?? [], nodes);
                nodeItem.AddEvents(item.OnRefuseEvents ?? [], nodes);
                nodeItem.AddEvents(item.OnAcceptEvents ?? [], nodes);
                nodes.Add(nodeItem);
            }
        }

        public static void GetPersonality(CharacterStory story, NodeStore nodes)
        {
            var traitRoot = new Node(story.CharacterName + "'s Traits", NodeType.Personality, story.CharacterName + "'s Traits", story.Personality!, nodes.Positions) { FileName = story.CharacterName! };
            nodes.Add(traitRoot);
            foreach (Trait valuee in story.Personality?.Values ?? [])
            {
                //add items to list
                var nodeValue = new Node(valuee.Type.ToString()!, NodeType.Personality, valuee.Type + " " + valuee.Value, nodes.Positions) { rawData = valuee };
                nodes.AddChild(traitRoot, nodeValue);
            }
        }

        public static void GetPlayerReactions(MainStory story, NodeStore nodes)
        {
            foreach (EventTrigger playerReaction in story.PlayerReactions ?? [])
            {
                //add items to list
                var nodeReaction = new Node(playerReaction.Id ?? string.Empty, NodeType.EventTrigger, playerReaction.Name ?? string.Empty, nodes.Positions) { rawData = playerReaction };

                //get actions for item
                nodeReaction.AddEvents(playerReaction.Events ?? [], nodes);

                nodeReaction.AddCriteria(playerReaction.Critera ?? [], nodes);

                nodes.Add(nodeReaction);
            }
        }

        public static void GetQuests(CharacterStory story, NodeStore nodes)
        {
            var questRoot = new Node(story.CharacterName + "'s Quests", NodeType.Quest, story.CharacterName + "'s Quests", nodes.Positions) { FileName = story.CharacterName! };
            nodes.Add(questRoot);
            foreach (Quest quest in story.Quests ?? [])
            {
                var nodeQuest = new Node(quest.ID ?? string.Empty, NodeType.Quest, quest.Name ?? string.Empty, nodes.Positions) { rawData = quest, FileName = story.CharacterName! };

                nodes.AddChild(questRoot, nodeQuest);
                //Add details
                if (quest.Details?.Length > 0)
                {
                    nodes.AddChild(nodeQuest, new Node($"{quest.ID}Description", NodeType.Quest, quest.Details, nodes.Positions) { FileName = story.CharacterName! });
                }
                //Add completed details
                if (quest.CompletedDetails?.Length > 0)
                {
                    nodes.AddChild(nodeQuest, new Node($"{quest.ID}CompletedDetails", NodeType.Quest, quest.CompletedDetails, nodes.Positions) { FileName = story.CharacterName! });
                }
                //Add failed details
                if (quest.FailedDetails?.Length > 0)
                {
                    nodes.AddChild(nodeQuest, new Node($"{quest.ID}FailedDetails", NodeType.Quest, quest.FailedDetails, nodes.Positions) { FileName = story.CharacterName! });
                }

                //Add extended details

                foreach (ExtendedDetail detail in quest.ExtendedDetails ?? [])
                {
                    nodes.AddChild(nodeQuest, new Node($"{quest.ID}Description{detail.Value}", NodeType.Quest, detail.Details ?? string.Empty, nodes.Positions) { rawData = detail, FileName = story.CharacterName! });
                }

            }
        }

        public static void GetReactions(CharacterStory story, NodeStore nodes)
        {
            foreach (EventTrigger playerReaction in story.Reactions ?? [])
            {
                //add items to list
                var nodeReaction = new Node(playerReaction.Id ?? string.Empty, NodeType.EventTrigger, playerReaction.Name ?? string.Empty, nodes.Positions) { rawData = playerReaction, FileName = story.CharacterName! };
                //get actions for item
                nodeReaction.AddEvents(playerReaction.Events ?? [], nodes);

                nodeReaction.AddCriteria(playerReaction.Critera ?? [], nodes);

                nodes.Add(nodeReaction);
            }
        }
        public static void GetValues(CharacterStory story, NodeStore nodes)
        {
            var ValueStore = new Node(story.CharacterName + "'s Values", NodeType.Value, story.CharacterName + "'s Values", nodes.Positions) { FileName = story.CharacterName! };
            nodes.Add(ValueStore);
            foreach (string value in story.StoryValues ?? [])
            {
                if (value == string.Empty)
                {
                    continue;
                }
                //add items to list
                var nodeValue = new Node(value!, NodeType.Value, ", referenced values: ", nodes.Positions) { rawData = new Value() { value = value }, FileName = story.CharacterName! };
                nodes.AddChild(ValueStore, nodeValue);
            }
        }

        public static void GetValues(MainStory story, NodeStore nodes)
        {
            foreach (string value in story.PlayerValues ?? [])
            {
                //add items to list
                var nodeValue = new Node(value, NodeType.Value, ", referenced values: ", nodes.Positions) { rawData = new Value() { value = value } };
                nodes.Add(nodeValue);
            }
        }
    }
}