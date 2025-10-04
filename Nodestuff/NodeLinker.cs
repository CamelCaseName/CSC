using CSC.StoryItems;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Xml.Linq;
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

        public static string FileName { get; private set; } = Main.NoCharacter;

        //todo needs some more linking for criteria or events with other node types so we auto populate. should reuse the code from the node spawning#
        public static void Link(NodeStore nodes, Node addFrom, Node addToThis)
        {
            if (addToThis.DataType == typeof(MissingReferenceInfo))
            {
                return;
            }

            bool linked = false;

            if (addFrom.DataType == typeof(Criterion))
            {
                if (addToThis.DataType == typeof(ItemAction))
                {
                    addToThis.Data<ItemAction>()!.Criteria!.Add(addFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (addToThis.DataType == typeof(UseWith))
                {
                    addToThis.Data<UseWith>()!.Criteria!.Add(addFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (addToThis.DataType == typeof(CriteriaListWrapper))
                {
                    addToThis.Data<CriteriaListWrapper>()!.CriteriaList!.Add(addFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (addToThis.DataType == typeof(GameEvent))
                {
                    addToThis.Data<GameEvent>()!.Criteria!.Add(addFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (addToThis.DataType == typeof(EventTrigger))
                {
                    addToThis.Data<EventTrigger>()!.Critera!.Add(addFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (addToThis.DataType == typeof(AlternateText))
                {
                    addToThis.Data<AlternateText>()!.Critera!.Add(addFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (addToThis.DataType == typeof(Response))
                {
                    addToThis.Data<Response>()!.ResponseCriteria!.Add(addFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (addToThis.DataType == typeof(BackgroundChatter))
                {
                    addToThis.Data<BackgroundChatter>()!.Critera!.Add(addFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (addToThis.DataType == typeof(ItemInteraction))
                {
                    addToThis.Data<ItemInteraction>()!.Critera!.Add(addFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (addToThis.DataType == typeof(ItemGroupInteraction))
                {
                    addToThis.Data<ItemGroupInteraction>()!.Criteria!.Add(addFrom.Data<Criterion>()!);
                    linked = true;
                }

                if (linked)
                {
                    nodes.AddParent(addToThis, addFrom);
                }
            }
            else if (addFrom.DataType == typeof(GameEvent))
            {
                if (addToThis.DataType == typeof(ItemAction))
                {
                    addToThis.Data<ItemAction>()!.OnTakeActionEvents!.Add(addFrom.Data<GameEvent>()!);
                    linked = true;
                }
                else if (addToThis.DataType == typeof(UseWith))
                {
                    addToThis.Data<UseWith>()!.OnSuccessEvents!.Add(addFrom.Data<GameEvent>()!);
                    linked = true;
                }
                else if (addToThis.DataType == typeof(EventTrigger))
                {
                    addToThis.Data<EventTrigger>()!.Events!.Add(addFrom.Data<GameEvent>()!);
                    linked = true;
                }
                else if (addToThis.DataType == typeof(MainStory))
                {
                    addToThis.Data<MainStory>()!.GameStartEvents!.Add(addFrom.Data<GameEvent>()!);
                    linked = true;
                }
                else if (addToThis.DataType == typeof(Response))
                {
                    addToThis.Data<Response>()!.ResponseEvents!.Add(addFrom.Data<GameEvent>()!);
                    linked = true;
                }
                else if (addToThis.DataType == typeof(Dialogue))
                {
                    var result = MessageBox.Show("Add as StartEvent? Hit yes for StartEvent, no for CloseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        addToThis.Data<Dialogue>()!.StartEvents!.Add(addFrom.Data<GameEvent>()!);
                        linked = true;
                    }
                    else if (result == DialogResult.No)
                    {
                        addToThis.Data<Dialogue>()!.CloseEvents!.Add(addFrom.Data<GameEvent>()!);
                        linked = true;
                    }
                }
                else if (addToThis.DataType == typeof(BackgroundChatter))
                {
                    addToThis.Data<BackgroundChatter>()!.StartEvents!.Add(addFrom.Data<GameEvent>()!);
                    linked = true;
                }
                else if (addToThis.DataType == typeof(ItemInteraction))
                {
                    var result = MessageBox.Show("Add as OnAcceptEvent? Hit yes for OnAcceptEvent, no for OnRefuseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        addToThis.Data<ItemInteraction>()!.OnAcceptEvents!.Add(addFrom.Data<GameEvent>()!);
                        linked = true;
                    }
                    else if (result == DialogResult.No)
                    {
                        addToThis.Data<ItemInteraction>()!.OnRefuseEvents!.Add(addFrom.Data<GameEvent>()!);
                        linked = true;
                    }
                }
                else if (addToThis.DataType == typeof(ItemGroupInteraction))
                {
                    var result = MessageBox.Show("Add as OnAcceptEvent? Hit yes for OnAcceptEvent, no for OnRefuseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        addToThis.Data<ItemGroupInteraction>()!.OnAcceptEvents!.Add(addFrom.Data<GameEvent>()!);
                        linked = true;
                    }
                    else if (result == DialogResult.No)
                    {
                        addToThis.Data<ItemGroupInteraction>()!.OnRefuseEvents!.Add(addFrom.Data<GameEvent>()!);
                        linked = true;
                    }
                }
                else if (addToThis.DataType == typeof(Criterion))
                {
                    addFrom.Data<GameEvent>()!.Criteria.Add(addToThis.Data<Criterion>()!);
                    nodes.AddParent(addToThis, addFrom);
                }

                if (linked)
                {
                    nodes.AddChild(addToThis, addFrom);
                }
            }
            else if (addFrom.DataType == typeof(ItemAction))
            {
                if (addToThis.DataType == typeof(InteractiveitemBehaviour))
                {
                    addToThis.Data<InteractiveitemBehaviour>()!.ItemActions.Add(addFrom.Data<ItemAction>()!);
                    nodes.AddChild(addToThis, addFrom);
                }
                else if (addToThis.DataType == typeof(ItemGroupBehavior))
                {
                    addToThis.Data<ItemGroupBehavior>()!.ItemActions.Add(addFrom.Data<ItemAction>()!);
                    nodes.AddChild(addToThis, addFrom);
                }
                else if (addToThis.DataType == typeof(Criterion))
                {
                    addFrom.Data<ItemAction>()!.Criteria.Add(addToThis.Data<Criterion>()!);
                    nodes.AddChild(addToThis, addFrom);
                }
                else if (addToThis.DataType == typeof(GameEvent))
                {
                    addFrom.Data<ItemAction>()!.OnTakeActionEvents.Add(addToThis.Data<GameEvent>()!);
                    nodes.AddParent(addToThis, addFrom);
                }
            }
            else if (addFrom.DataType == typeof(UseWith))
            {
                if (addToThis.DataType == typeof(InteractiveitemBehaviour))
                {
                    addToThis.Data<InteractiveitemBehaviour>()!.UseWiths.Add(addFrom.Data<UseWith>()!);
                    nodes.AddChild(addToThis, addFrom);
                }
                else if (addToThis.DataType == typeof(ItemGroupBehavior))
                {
                    addToThis.Data<ItemGroupBehavior>()!.UseWiths.Add(addFrom.Data<UseWith>()!);
                    nodes.AddChild(addToThis, addFrom);
                }
                else if (addToThis.DataType == typeof(Criterion))
                {
                    addFrom.Data<UseWith>()!.Criteria.Add(addToThis.Data<Criterion>()!);
                    nodes.AddChild(addToThis, addFrom);
                }
                else if (addToThis.DataType == typeof(GameEvent))
                {
                    addFrom.Data<UseWith>()!.OnSuccessEvents.Add(addToThis.Data<GameEvent>()!);
                    nodes.AddParent(addToThis, addFrom);
                }
            }
            else if (addFrom.DataType == typeof(InteractiveitemBehaviour))
            {
                if (addToThis.DataType == typeof(ItemAction))
                {
                    addFrom.Data<InteractiveitemBehaviour>()!.ItemActions.Add(addToThis.Data<ItemAction>()!);
                    nodes.AddParent(addToThis, addFrom);
                }
                else if (addToThis.DataType == typeof(UseWith))
                {
                    addFrom.Data<InteractiveitemBehaviour>()!.UseWiths.Add(addToThis.Data<UseWith>()!);
                    nodes.AddParent(addToThis, addFrom);
                }
            }
            else if (addFrom.DataType == typeof(ItemGroupBehavior))
            {
                if (addToThis.DataType == typeof(ItemAction))
                {
                    addFrom.Data<ItemGroupBehavior>()!.ItemActions.Add(addToThis.Data<ItemAction>()!);
                    nodes.AddParent(addToThis, addFrom);
                }
                else if (addToThis.DataType == typeof(UseWith))
                {
                    addFrom.Data<ItemGroupBehavior>()!.UseWiths.Add(addToThis.Data<UseWith>()!);
                    nodes.AddParent(addToThis, addFrom);
                }
            }
            else if (addFrom.DataType == typeof(Achievement))
            {
                //todo
            }
            else if (addFrom.DataType == typeof(CriteriaListWrapper))
            {
                //todo
            }
            else if (addFrom.DataType == typeof(CriteriaGroup))
            {
                //todo
            }
            else if (addFrom.DataType == typeof(ItemGroup))
            {
                //todo
            }
            else if (addFrom.DataType == typeof(EventTrigger))
            {
                if (addToThis.DataType == typeof(Criterion))
                {
                    addFrom.Data<EventTrigger>()!.Critera.Add(addToThis.Data<Criterion>()!);
                    nodes.AddChild(addToThis, addFrom);
                }
                else if (addToThis.DataType == typeof(GameEvent))
                {
                    addFrom.Data<EventTrigger>()!.Events.Add(addToThis.Data<GameEvent>()!);
                    nodes.AddParent(addToThis, addFrom);
                }
            }
            else if (addFrom.DataType == typeof(CharacterGroup))
            {
                //todo
            }
            else if (addFrom.DataType == typeof(AlternateText))
            {
                if (addToThis.DataType == typeof(Criterion))
                {
                    addFrom.Data<AlternateText>()!.Critera.Add(addToThis.Data<Criterion>()!);
                    nodes.AddChild(addToThis, addFrom);
                }
                else if (addToThis.DataType == typeof(Dialogue))
                {
                    addToThis.Data<Dialogue>()!.AlternateTexts.Add(addFrom.Data<AlternateText>()!);
                    nodes.AddChild(addToThis, addFrom);
                }
            }
            else if (addFrom.DataType == typeof(Response))
            {
                if (addToThis.DataType == typeof(Criterion))
                {
                    addFrom.Data<Response>()!.ResponseCriteria.Add(addToThis.Data<Criterion>()!);
                    nodes.AddChild(addToThis, addFrom);
                }
                else if (addToThis.DataType == typeof(GameEvent))
                {
                    addFrom.Data<Response>()!.ResponseEvents.Add(addToThis.Data<GameEvent>()!);
                    nodes.AddParent(addToThis, addFrom);
                }
                else if (addToThis.DataType == typeof(Dialogue))
                {
                    var result = MessageBox.Show("Lead to this dialogue from the response? Hit yes for that, no to add the response as a normal response to this dialogue", "Select Response place", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        addFrom.Data<Response>()!.Next = addToThis.Data<Dialogue>()!.ID;
                        nodes.AddParent(addToThis, addFrom);
                    }
                    else if (result == DialogResult.No)
                    {
                        addToThis.Data<Dialogue>()!.Responses.Add(addFrom.Data<Response>()!);
                        nodes.AddChild(addToThis, addFrom);
                    }
                }
            }
            else if (addFrom.DataType == typeof(Dialogue))
            {
                if (addToThis.DataType == typeof(GameEvent))
                {
                    var result = MessageBox.Show("Add as StartEvent? Hit yes for StartEvent, no for CloseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        addFrom.Data<Dialogue>()!.StartEvents!.Add(addToThis.Data<GameEvent>()!);
                        nodes.AddParent(addToThis, addFrom);
                    }
                    else if (result == DialogResult.No)
                    {
                        addFrom.Data<Dialogue>()!.CloseEvents!.Add(addToThis.Data<GameEvent>()!);
                        nodes.AddParent(addToThis, addFrom);
                    }
                }
                else if (addToThis.DataType == typeof(Response))
                {
                    var result = MessageBox.Show("Add as a response? Hit yes for Response, no for the response leading to this dialogue", "Select Response place", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        addFrom.Data<Dialogue>()!.Responses.Add(addToThis.Data<Response>()!);
                        nodes.AddParent(addToThis, addFrom);
                    }
                    else if (result == DialogResult.No)
                    {
                        addToThis.Data<Response>()!.Next = addFrom.Data<Dialogue>()!.ID;
                        nodes.AddChild(addToThis, addFrom);
                    }
                }
                else if (addToThis.DataType == typeof(AlternateText))
                {
                    addFrom.Data<Dialogue>()!.AlternateTexts.Add(addToThis.Data<AlternateText>()!);
                    nodes.AddParent(addToThis, addFrom);
                }
            }
            else if (addFrom.DataType == typeof(BackgroundChatter))
            {
                if (addToThis.DataType == typeof(Criterion))
                {
                    addFrom.Data<BackgroundChatter>()!.Critera.Add(addToThis.Data<Criterion>()!);
                    nodes.AddChild(addToThis, addFrom);
                }
                else if (addToThis.DataType == typeof(GameEvent))
                {
                    addFrom.Data<BackgroundChatter>()!.StartEvents.Add(addToThis.Data<GameEvent>()!);
                    nodes.AddParent(addToThis, addFrom);
                }
                else if (addToThis.DataType == typeof(BackgroundChatterResponse))
                {
                    addFrom.Data<BackgroundChatter>()!.Responses.Add(addToThis.Data<BackgroundChatterResponse>()!);
                    nodes.AddParent(addToThis, addFrom);
                }
            }
            else if (addFrom.DataType == typeof(BackgroundChatterResponse))
            {
                if (addToThis.DataType == typeof(BackgroundChatter))
                {
                    addToThis.Data<BackgroundChatter>()!.Responses.Add(addFrom.Data<BackgroundChatterResponse>()!);
                    nodes.AddChild(addToThis, addFrom);
                }
            }
            else if (addFrom.DataType == typeof(Trait))
            {
                //todo
            }
            else if (addFrom.DataType == typeof(ExtendedDetail))
            {
                //todo
            }
            else if (addFrom.DataType == typeof(Quest))
            {
                //todo
            }
            else if (addFrom.DataType == typeof(ItemInteraction))
            {
                //todo
            }
            else if (addFrom.DataType == typeof(ItemGroupInteraction))
            {
                //todo
            }
            else if (addFrom.DataType == typeof(string))
            {
                //todo
            }

            NodeLinker.UpdateLinks(addToThis, Main.SelectedCharacter, nodes);

            addFrom = Node.NullNode;
        }

        public static void Unlink(NodeStore nodes, Node removeFrom, Node removeThis, bool removeAll = false)
        {
            if (removeThis.DataType == typeof(MissingReferenceInfo))
            {
                return;
            }

            bool linked = false;

            if (removeFrom.DataType == typeof(Criterion))
            {
                if (removeThis.DataType == typeof(ItemAction))
                {
                    removeThis.Data<ItemAction>()!.Criteria!.Remove(removeFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (removeThis.DataType == typeof(UseWith))
                {
                    removeThis.Data<UseWith>()!.Criteria!.Remove(removeFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (removeThis.DataType == typeof(CriteriaListWrapper))
                {
                    removeThis.Data<CriteriaListWrapper>()!.CriteriaList!.Remove(removeFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (removeThis.DataType == typeof(GameEvent))
                {
                    removeThis.Data<GameEvent>()!.Criteria!.Remove(removeFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (removeThis.DataType == typeof(EventTrigger))
                {
                    removeThis.Data<EventTrigger>()!.Critera!.Remove(removeFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (removeThis.DataType == typeof(AlternateText))
                {
                    removeThis.Data<AlternateText>()!.Critera!.Remove(removeFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (removeThis.DataType == typeof(Response))
                {
                    removeThis.Data<Response>()!.ResponseCriteria!.Remove(removeFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (removeThis.DataType == typeof(BackgroundChatter))
                {
                    removeThis.Data<BackgroundChatter>()!.Critera!.Remove(removeFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (removeThis.DataType == typeof(ItemInteraction))
                {
                    removeThis.Data<ItemInteraction>()!.Critera!.Remove(removeFrom.Data<Criterion>()!);
                    linked = true;
                }
                else if (removeThis.DataType == typeof(ItemGroupInteraction))
                {
                    removeThis.Data<ItemGroupInteraction>()!.Criteria!.Remove(removeFrom.Data<Criterion>()!);
                    linked = true;
                }

                if (linked)
                {
                    nodes.RemoveParent(removeThis, removeFrom);
                }
            }
            else if (removeFrom.DataType == typeof(GameEvent))
            {
                if (removeThis.DataType == typeof(ItemAction))
                {
                    removeThis.Data<ItemAction>()!.OnTakeActionEvents!.Remove(removeFrom.Data<GameEvent>()!);
                    linked = true;
                }
                else if (removeThis.DataType == typeof(UseWith))
                {
                    removeThis.Data<UseWith>()!.OnSuccessEvents!.Remove(removeFrom.Data<GameEvent>()!);
                    linked = true;
                }
                else if (removeThis.DataType == typeof(EventTrigger))
                {
                    removeThis.Data<EventTrigger>()!.Events!.Remove(removeFrom.Data<GameEvent>()!);
                    linked = true;
                }
                else if (removeThis.DataType == typeof(MainStory))
                {
                    removeThis.Data<MainStory>()!.GameStartEvents!.Remove(removeFrom.Data<GameEvent>()!);
                    linked = true;
                }
                else if (removeThis.DataType == typeof(Response))
                {
                    removeThis.Data<Response>()!.ResponseEvents!.Remove(removeFrom.Data<GameEvent>()!);
                    linked = true;
                }
                else if (removeThis.DataType == typeof(Dialogue))
                {
                    DialogResult result = DialogResult.None;
                    if (!removeAll)
                    {
                        result = MessageBox.Show("Remove as StartEvent? Hit yes for StartEvent, no for CloseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    }
                    if (result == DialogResult.Yes || removeAll)
                    {
                        removeThis.Data<Dialogue>()!.StartEvents!.Remove(removeFrom.Data<GameEvent>()!);
                        linked = true;
                    }
                    if (result == DialogResult.No || removeAll)
                    {
                        removeThis.Data<Dialogue>()!.CloseEvents!.Remove(removeFrom.Data<GameEvent>()!);
                        linked = true;
                    }
                }
                else if (removeThis.DataType == typeof(BackgroundChatter))
                {
                    removeThis.Data<BackgroundChatter>()!.StartEvents!.Remove(removeFrom.Data<GameEvent>()!);
                    linked = true;
                }
                else if (removeThis.DataType == typeof(ItemInteraction))
                {
                    DialogResult result = DialogResult.None;
                    if (!removeAll)
                    {
                        result = MessageBox.Show("Remove as OnAcceptEvent? Hit yes for OnAcceptEvent, no for OnRefuseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    }
                    if (result == DialogResult.Yes || removeAll)
                    {
                        removeThis.Data<ItemInteraction>()!.OnAcceptEvents!.Remove(removeFrom.Data<GameEvent>()!);
                        linked = true;
                    }
                    if (result == DialogResult.No || removeAll)
                    {
                        removeThis.Data<ItemInteraction>()!.OnRefuseEvents!.Remove(removeFrom.Data<GameEvent>()!);
                        linked = true;
                    }
                }
                else if (removeThis.DataType == typeof(ItemGroupInteraction))
                {
                    DialogResult result = DialogResult.None;
                    if (!removeAll)
                    {
                        result = MessageBox.Show("Remove as OnAcceptEvent? Hit yes for OnAcceptEvent, no for OnRefuseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    }
                    if (result == DialogResult.Yes || removeAll)
                    {
                        removeThis.Data<ItemGroupInteraction>()!.OnAcceptEvents!.Remove(removeFrom.Data<GameEvent>()!);
                        linked = true;
                    }
                    if (result == DialogResult.No || removeAll)
                    {
                        removeThis.Data<ItemGroupInteraction>()!.OnRefuseEvents!.Remove(removeFrom.Data<GameEvent>()!);
                        linked = true;
                    }
                }
                else if (removeThis.DataType == typeof(Criterion))
                {
                    removeFrom.Data<GameEvent>()!.Criteria.Remove(removeThis.Data<Criterion>()!);
                    nodes.RemoveParent(removeThis, removeFrom);
                }

                if (linked)
                {
                    nodes.RemoveChild(removeThis, removeFrom);
                }
            }
            else if (removeFrom.DataType == typeof(ItemAction))
            {
                if (removeThis.DataType == typeof(InteractiveitemBehaviour))
                {
                    removeThis.Data<InteractiveitemBehaviour>()!.ItemActions.Remove(removeFrom.Data<ItemAction>()!);
                    nodes.RemoveChild(removeThis, removeFrom);
                }
                else if (removeThis.DataType == typeof(ItemGroupBehavior))
                {
                    removeThis.Data<ItemGroupBehavior>()!.ItemActions.Remove(removeFrom.Data<ItemAction>()!);
                    nodes.RemoveChild(removeThis, removeFrom);
                }
                else if (removeThis.DataType == typeof(Criterion))
                {
                    removeFrom.Data<ItemAction>()!.Criteria.Remove(removeThis.Data<Criterion>()!);
                    nodes.RemoveChild(removeThis, removeFrom);
                }
                else if (removeThis.DataType == typeof(GameEvent))
                {
                    removeFrom.Data<ItemAction>()!.OnTakeActionEvents.Remove(removeThis.Data<GameEvent>()!);
                    nodes.RemoveParent(removeThis, removeFrom);
                }
            }
            else if (removeFrom.DataType == typeof(UseWith))
            {
                if (removeThis.DataType == typeof(InteractiveitemBehaviour))
                {
                    removeThis.Data<InteractiveitemBehaviour>()!.UseWiths.Remove(removeFrom.Data<UseWith>()!);
                    nodes.RemoveChild(removeThis, removeFrom);
                }
                else if (removeThis.DataType == typeof(ItemGroupBehavior))
                {
                    removeThis.Data<ItemGroupBehavior>()!.UseWiths.Remove(removeFrom.Data<UseWith>()!);
                    nodes.RemoveChild(removeThis, removeFrom);
                }
                else if (removeThis.DataType == typeof(Criterion))
                {
                    removeFrom.Data<UseWith>()!.Criteria.Remove(removeThis.Data<Criterion>()!);
                    nodes.RemoveChild(removeThis, removeFrom);
                }
                else if (removeThis.DataType == typeof(GameEvent))
                {
                    removeFrom.Data<UseWith>()!.OnSuccessEvents.Remove(removeThis.Data<GameEvent>()!);
                    nodes.RemoveParent(removeThis, removeFrom);
                }
            }
            else if (removeFrom.DataType == typeof(InteractiveitemBehaviour))
            {
                if (removeThis.DataType == typeof(ItemAction))
                {
                    removeFrom.Data<InteractiveitemBehaviour>()!.ItemActions.Remove(removeThis.Data<ItemAction>()!);
                    nodes.RemoveParent(removeThis, removeFrom);
                }
                else if (removeThis.DataType == typeof(UseWith))
                {
                    removeFrom.Data<InteractiveitemBehaviour>()!.UseWiths.Remove(removeThis.Data<UseWith>()!);
                    nodes.RemoveParent(removeThis, removeFrom);
                }
            }
            else if (removeFrom.DataType == typeof(ItemGroupBehavior))
            {
                if (removeThis.DataType == typeof(ItemAction))
                {
                    removeFrom.Data<ItemGroupBehavior>()!.ItemActions.Remove(removeThis.Data<ItemAction>()!);
                    nodes.RemoveParent(removeThis, removeFrom);
                }
                else if (removeThis.DataType == typeof(UseWith))
                {
                    removeFrom.Data<ItemGroupBehavior>()!.UseWiths.Remove(removeThis.Data<UseWith>()!);
                    nodes.RemoveParent(removeThis, removeFrom);
                }
            }
            else if (removeFrom.DataType == typeof(Achievement))
            {
                //todo
            }
            else if (removeFrom.DataType == typeof(CriteriaListWrapper))
            {
                //todo
            }
            else if (removeFrom.DataType == typeof(CriteriaGroup))
            {
                //todo
            }
            else if (removeFrom.DataType == typeof(ItemGroup))
            {
                //todo
            }
            else if (removeFrom.DataType == typeof(EventTrigger))
            {
                if (removeThis.DataType == typeof(Criterion))
                {
                    removeFrom.Data<EventTrigger>()!.Critera.Remove(removeThis.Data<Criterion>()!);
                    nodes.RemoveChild(removeThis, removeFrom);
                }
                else if (removeThis.DataType == typeof(GameEvent))
                {
                    removeFrom.Data<EventTrigger>()!.Events.Remove(removeThis.Data<GameEvent>()!);
                    nodes.RemoveParent(removeThis, removeFrom);
                }
            }
            else if (removeFrom.DataType == typeof(CharacterGroup))
            {
                //todo
            }
            else if (removeFrom.DataType == typeof(AlternateText))
            {
                if (removeThis.DataType == typeof(Criterion))
                {
                    removeFrom.Data<AlternateText>()!.Critera.Remove(removeThis.Data<Criterion>()!);
                    nodes.RemoveChild(removeThis, removeFrom);
                }
                else if (removeThis.DataType == typeof(Dialogue))
                {
                    removeThis.Data<Dialogue>()!.AlternateTexts.Remove(removeFrom.Data<AlternateText>()!);
                    nodes.RemoveChild(removeThis, removeFrom);
                }
            }
            else if (removeFrom.DataType == typeof(Response))
            {
                if (removeThis.DataType == typeof(Criterion))
                {
                    removeFrom.Data<Response>()!.ResponseCriteria.Remove(removeThis.Data<Criterion>()!);
                    nodes.RemoveChild(removeThis, removeFrom);
                }
                else if (removeThis.DataType == typeof(GameEvent))
                {
                    removeFrom.Data<Response>()!.ResponseEvents.Remove(removeThis.Data<GameEvent>()!);
                    nodes.RemoveParent(removeThis, removeFrom);
                }
                else if (removeThis.DataType == typeof(Dialogue))
                {
                    DialogResult result = DialogResult.None;
                    if (!removeAll)
                    {
                        result = MessageBox.Show("Lead to this dialogue from the response? Hit yes for that, no to Remove the response as a normal response to this dialogue", "Select Response place", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    }
                    if (result == DialogResult.Yes || removeAll)
                    {
                        removeFrom.Data<Response>()!.Next = 0;
                        nodes.RemoveParent(removeThis, removeFrom);
                    }
                    if (result == DialogResult.No || removeAll)
                    {
                        removeThis.Data<Dialogue>()!.Responses.Remove(removeFrom.Data<Response>()!);
                        nodes.RemoveChild(removeThis, removeFrom);
                    }
                }
            }
            else if (removeFrom.DataType == typeof(Dialogue))
            {
                if (removeThis.DataType == typeof(GameEvent))
                {
                    DialogResult result = DialogResult.None;
                    if (!removeAll)
                    {
                        result = MessageBox.Show("Remove as StartEvent? Hit yes for StartEvent, no for CloseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    }
                    if (result == DialogResult.Yes || removeAll)
                    {
                        removeFrom.Data<Dialogue>()!.StartEvents!.Remove(removeThis.Data<GameEvent>()!);
                        nodes.RemoveParent(removeThis, removeFrom);
                    }
                    if (result == DialogResult.No || removeAll)
                    {
                        removeFrom.Data<Dialogue>()!.CloseEvents!.Remove(removeThis.Data<GameEvent>()!);
                        nodes.RemoveParent(removeThis, removeFrom);
                    }
                }
                else if (removeThis.DataType == typeof(Response))
                {
                    DialogResult result = DialogResult.None;
                    if (!removeAll)
                    {
                        result = MessageBox.Show("Remove as a response? Hit yes for Response, no for the response leading to this dialogue", "Select Response place", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    }
                    if (result == DialogResult.Yes || removeAll)
                    {
                        removeFrom.Data<Dialogue>()!.Responses.Remove(removeThis.Data<Response>()!);
                        nodes.RemoveParent(removeThis, removeFrom);
                    }
                    if (result == DialogResult.No || removeAll)
                    {
                        removeThis.Data<Response>()!.Next = 0;
                        nodes.RemoveChild(removeThis, removeFrom);
                    }
                }
                else if (removeThis.DataType == typeof(AlternateText))
                {
                    removeFrom.Data<Dialogue>()!.AlternateTexts.Remove(removeThis.Data<AlternateText>()!);
                    nodes.RemoveParent(removeThis, removeFrom);
                }
            }
            else if (removeFrom.DataType == typeof(BackgroundChatter))
            {
                if (removeThis.DataType == typeof(Criterion))
                {
                    removeFrom.Data<BackgroundChatter>()!.Critera.Remove(removeThis.Data<Criterion>()!);
                    nodes.RemoveChild(removeThis, removeFrom);
                }
                else if (removeThis.DataType == typeof(GameEvent))
                {
                    removeFrom.Data<BackgroundChatter>()!.StartEvents.Remove(removeThis.Data<GameEvent>()!);
                    nodes.RemoveParent(removeThis, removeFrom);
                }
                else if (removeThis.DataType == typeof(BackgroundChatterResponse))
                {
                    removeFrom.Data<BackgroundChatter>()!.Responses.Remove(removeThis.Data<BackgroundChatterResponse>()!);
                    nodes.RemoveParent(removeThis, removeFrom);
                }
            }
            else if (removeFrom.DataType == typeof(BackgroundChatterResponse))
            {
                if (removeThis.DataType == typeof(BackgroundChatter))
                {
                    removeThis.Data<BackgroundChatter>()!.Responses.Remove(removeFrom.Data<BackgroundChatterResponse>()!);
                    nodes.RemoveChild(removeThis, removeFrom);
                }
            }
            else if (removeFrom.DataType == typeof(Trait))
            {
                //todo
            }
            else if (removeFrom.DataType == typeof(ExtendedDetail))
            {
                //todo
            }
            else if (removeFrom.DataType == typeof(Quest))
            {
                //todo
            }
            else if (removeFrom.DataType == typeof(ItemInteraction))
            {
                //todo
            }
            else if (removeFrom.DataType == typeof(ItemGroupInteraction))
            {
                //todo
            }
            else if (removeFrom.DataType == typeof(string))
            {
                //todo
            }

            NodeLinker.UpdateLinks(removeThis, Main.SelectedCharacter, nodes);
        }

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

            AnalyzeAndConnectNode(store, node, nodes, false);
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
            InteractiveitemBehaviour itemOverride;
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
                            else if (dupeTo)
                            {
                                //create and add value node, hasnt been referenced yet
                                var clothing = new Node(criterion.Option + criterion.Value, NodeType.Clothing, criterion.Character + "'s  " + ((ClothingType)int.Parse(criterion.Value!)).ToString() + " in set " + (criterion.Option == 0 ? "any" : (criterion.Option - 1).ToString())) { FileName = criterion.Character! };
                                Clothing.Add(clothing);
                                nodes.AddParent(node, clothing);
                            }
                            break;
                        }
                        case CompareTypes.CompareValues:
                        {
                            result = Values.Find((n) => n.Type == NodeType.Value && n.ID == criterion.Key && n.FileName == criterion.Character);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                            }
                            else if (dupeTo)
                            {
                                //create and add value node, hasnt been referenced yet
                                var value = new Node(criterion.Key!, NodeType.Value, criterion.Character + " value " + criterion.Key + ", referenced values: " + GetSymbolsFromValueFormula(criterion.ValueFormula) + criterion.Value + ", ") { FileName = criterion.Character ?? string.Empty };
                                Values.Add(value);
                                nodes.AddParent(node, value);
                            }
                            result = Values.Find((n) => n.Type == NodeType.Value && n.ID == criterion.Key2 && n.FileName == criterion.Character2);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                            }
                            else if (dupeTo)
                            {
                                //create and add value node, hasnt been referenced yet
                                var value = new Node(criterion.Key2!, NodeType.Value, criterion.Character + " value " + criterion.Key2 + ", referenced values: " + GetSymbolsFromValueFormula(criterion.ValueFormula) + criterion.Value + ", ") { FileName = criterion.Character ?? string.Empty };
                                Values.Add(value);
                                nodes.AddParent(node, value);
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
                            result = searchIn.Find((n) => n.Type == NodeType.Cutscene && n.ID == criterion.Key);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddParent(node, result);
                            }
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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

                                if (!result.StaticText.Contains(GetSymbolsFromValueFormula(criterion.ValueFormula) + criterion.Value))
                                {
                                    result.StaticText += GetSymbolsFromValueFormula(criterion.ValueFormula) + criterion.Value + ", ";
                                }

                                nodes.AddParent(node, result);
                                break;
                            }
                            else if (dupeTo)
                            {
                                //create and add value node, hasnt been referenced yet
                                var value = new Node(criterion.Key!, NodeType.Value, criterion.Character + " value " + criterion.Key + ", referenced values: " + GetSymbolsFromValueFormula(criterion.ValueFormula) + criterion.Value + ", ") { FileName = criterion.Character ?? string.Empty };
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
                            else if (dupeTo)
                            {
                                //create and add value node, hasnt been referenced yet
                                var clothing = new Node(gameEvent.Option + gameEvent.Value, NodeType.Clothing, gameEvent.Character + "'s  " + ((ClothingType)int.Parse(gameEvent.Value!)).ToString() + " in set " + (gameEvent.Option == 0 ? "any" : (gameEvent.Option - 1).ToString())) { FileName = gameEvent.Character! };
                                Clothing.Add(clothing);
                                nodes.AddChild(node, clothing);
                                clothing.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.MatchValue:
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            result = searchIn.Find((n) => n.Type == NodeType.Cutscene && n.ID == gameEvent.Key);
                            if (result is not null)
                            {
                                if (dupeTo)
                                {
                                    result.DupeToOtherSorting(node.FileName);
                                }

                                nodes.AddChild(node, result);
                            }
                            else if (dupeTo)
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
                            else if (dupeTo)
                            {
                                if (gameEvent.Option != (int)DialogueAction.TriggerStartDialogue)
                                {
                                    //create and add new dialogue, should be from someone else
                                    var item = new Node(gameEvent.Value!, NodeType.Dialogue, ((DialogueAction)gameEvent.Option).ToString() + " " + gameEvent.Character + "'s Dialogue " + gameEvent.Value) { FileName = gameEvent.Character! };
                                    searchIn.Add(item);
                                    nodes.AddChild(node, item);
                                    item.DupeToOtherSorting(node.FileName);
                                }
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
                            {
                                //create and add property node, hasnt been referenced yet
                                var property = new Node(gameEvent.Character + "Property" + gameEvent.Value, NodeType.Property, gameEvent.Character + EEnum.StringParse<InteractiveProperties>(gameEvent.Value!)) { FileName = gameEvent.Character! };
                                Properties.Add(property);
                                nodes.AddChild(node, property);
                                property.DupeToOtherSorting(node.FileName);
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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
                    else if (dupeTo)
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
                            else if (dupeTo)
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
                            else if (dupeTo)
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

                    foreach (Criterion _criterion in itemGroupInteraction.Criteria!)
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
                        else if (dupeTo)
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
                case NodeType.StoryItem when (itemOverride = node.Data<InteractiveitemBehaviour>()!) is not null:
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
                        else if (dupeTo)
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
                    else if (dupeTo)
                    {
                        //create and add event, hasnt been referenced yet, we can not know its id if it doesnt already exist
                        var newNode = new Node(useWith.ItemName ?? string.Empty, NodeType.StoryItem, useWith.ItemName ?? string.Empty) { FileName = NodeLinker.FileName };
                        searchIn.Add(newNode);
                        nodes.AddChild(node, newNode);
                    }
                    break;
                }
            }

            if (nodes.Childs(node).Count == 0
                && nodes.Parents(node).Count == 0
                && node.FileName != FileName)
            {
                nodes.Remove(node);
                node.RemoveFromSorting(FileName);
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
            else if (dupeTo)
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
            else if (dupeTo)
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
            else if (dupeTo)
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
            else if (dupeTo)
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
                                    Main.clearAllNodePos(node);
                                }
                            }
                        }
                    }
                }
            }
            //check some comparevaluenodes.again because the referenced values havent been added yet
            RecheckValues(stores, true);
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

        private static void RecheckValues(Dictionary<string, NodeStore> stores, bool dupeTo = false)
        {
            List<Node> values = [.. Values];
            foreach (Node node in values)
            {
                //we have the real nodehere, replace all false ones
                if (node.DataType == typeof(string))
                {
                    foreach (var result in values.FindAll(n => n.Type == NodeType.Value && n.DataType == typeof(MissingReferenceInfo) && n.ID == node.ID && n.FileName == node.FileName))
                    {
                        foreach (var store in stores.Values)
                        {
                            if (store.Contains(result))
                            {
                                store.Replace(result, node);
                            }
                        }
                        Values.Remove(result);
                        Main.clearAllNodePos(result);
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
                StoryNodeExtractor.GetValues(story, nodes);
                foreach (var node in nodes.Nodes)
                {
                    if (node.DataType == typeof(string))
                    {
                        Values.Add(node);
                    }
                }
                StoryNodeExtractor.GetItemInteractions(story, nodes);
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
                StoryNodeExtractor.GetValues(story, nodes);
                foreach (var node in nodes.Nodes)
                {
                    if (node.DataType == typeof(string))
                    {
                        Values.Add(node);
                    }
                }
                StoryNodeExtractor.GetItemOverrides(story, nodes);
                StoryNodeExtractor.GetItemGroups(story, nodes);
                StoryNodeExtractor.GetAchievements(story, nodes);
                StoryNodeExtractor.GetPlayerReactions(story, nodes);
                StoryNodeExtractor.GetCriteriaGroups(story, nodes);
                StoryNodeExtractor.GetGameStartEvents(story, nodes);
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
