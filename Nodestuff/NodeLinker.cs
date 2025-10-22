using CSC.Glue;
using CSC.StoryItems;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static CSC.StoryItems.StoryEnums;

namespace CSC.Nodestuff
{
    internal static partial class NodeLinker
    {
        private static readonly List<Node> Doors = [];
        private static readonly Dictionary<string, Node> Values = [];
        private static readonly List<Node> Socials = [];
        private static readonly List<Node> States = [];
        private static readonly List<Node> Clothing = [];
        private static readonly List<Node> Poses = [];
        private static readonly List<Node> InventoryItems = [];
        private static readonly List<Node> StoryItems = [];
        private static readonly List<Node> Properties = [];
        private static readonly string[] StateNames = Enum.GetNames<InteractiveStates>();

        private static void AllLink(Node source, Node destination, bool link)
        {
            string Add = link ? "Add " : "Remove ";
            if (source.DataType == typeof(Criterion))
            {
                if (destination.DataType == typeof(ItemAction))
                {
                    if (link)
                    {
                        destination.Data<ItemAction>()!.Criteria!.Add(source.Data<Criterion>()!);
                    }
                    else
                    {
                        destination.Data<ItemAction>()!.Criteria!.Remove(source.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(UseWith))
                {
                    if (link)
                    {
                        destination.Data<UseWith>()!.Criteria!.Add(source.Data<Criterion>()!);
                    }
                    else
                    {
                        destination.Data<UseWith>()!.Criteria!.Remove(source.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(CriteriaListWrapper))
                {
                    if (link)
                    {
                        destination.Data<CriteriaListWrapper>()!.CriteriaList!.Add(source.Data<Criterion>()!);
                    }
                    else
                    {
                        destination.Data<CriteriaListWrapper>()!.CriteriaList!.Remove(source.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(GameEvent))
                {
                    if (link)
                    {
                        destination.Data<GameEvent>()!.Criteria!.Add(source.Data<Criterion>()!);
                    }
                    else
                    {
                        destination.Data<GameEvent>()!.Criteria!.Remove(source.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(EventTrigger))
                {
                    if (link)
                    {
                        destination.Data<EventTrigger>()!.Critera!.Add(source.Data<Criterion>()!);
                    }
                    else
                    {
                        destination.Data<EventTrigger>()!.Critera!.Remove(source.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(AlternateText))
                {
                    if (link)
                    {
                        destination.Data<AlternateText>()!.Critera!.Add(source.Data<Criterion>()!);
                    }
                    else
                    {
                        destination.Data<AlternateText>()!.Critera!.Remove(source.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(Response))
                {
                    if (link)
                    {
                        destination.Data<Response>()!.ResponseCriteria!.Add(source.Data<Criterion>()!);
                    }
                    else
                    {
                        destination.Data<Response>()!.ResponseCriteria!.Remove(source.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(BackgroundChatter))
                {
                    if (link)
                    {
                        destination.Data<BackgroundChatter>()!.Critera!.Add(source.Data<Criterion>()!);
                    }
                    else
                    {
                        destination.Data<BackgroundChatter>()!.Critera!.Remove(source.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(ItemInteraction))
                {
                    if (link)
                    {
                        destination.Data<ItemInteraction>()!.Critera!.Add(source.Data<Criterion>()!);
                    }
                    else
                    {
                        destination.Data<ItemInteraction>()!.Critera!.Remove(source.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(ItemGroupInteraction))
                {
                    if (link)
                    {
                        destination.Data<ItemGroupInteraction>()!.Criteria!.Add(source.Data<Criterion>()!);
                    }
                    else
                    {
                        destination.Data<ItemGroupInteraction>()!.Criteria!.Remove(source.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(ItemGroup))
                {
                    if (link)
                    {
                        var crit = source.Data<Criterion>()!;
                        crit.CompareType = CompareTypes.ItemFromItemGroup;
                        crit.Key = destination.Data<CriteriaGroup>()!.Name;
                        crit.ItemFromItemGroupComparison = ItemFromItemGroupComparisonTypes.IsActive;
                    }
                    else
                    {
                        source.Data<Criterion>()!.CompareType = CompareTypes.None;
                    }
                }
                else if (destination.DataType == typeof(Quest))
                {
                    if (link)
                    {
                        var crit = source.Data<Criterion>()!;
                        crit.CompareType = CompareTypes.Quest;
                        crit.Key = destination.Data<Quest>()!.ID;
                        crit.Key2 = destination.Data<Quest>()!.Name;
                        crit.Value = QuestStatus.NotObtained.ToString();
                    }
                    else
                    {
                        source.Data<Criterion>()!.CompareType = CompareTypes.None;
                    }
                }
                else if (destination.DataType == typeof(Trait))
                {
                    if (link)
                    {
                        var c = source.Data<Criterion>()!;
                        c.CompareType = CompareTypes.Personality;
                        c.Character = Main.SelectedCharacter;
                        c.Key = ((int)destination.Data<Trait>()!.Type).ToString();
                        c.EquationValue = ComparisonEquations.Equals;
                    }
                    else
                    {
                        source.Data<Criterion>()!.CompareType = CompareTypes.None;
                    }
                }
            }
            else if (source.DataType == typeof(GameEvent))
            {
                if (destination.DataType == typeof(ItemAction))
                {
                    if (link)
                    {
                        destination.Data<ItemAction>()!.OnTakeActionEvents!.Add(source.Data<GameEvent>()!);
                    }
                    else
                    {
                        destination.Data<ItemAction>()!.OnTakeActionEvents!.Remove(source.Data<GameEvent>()!);
                    }
                }
                else if (destination.DataType == typeof(UseWith))
                {
                    if (link)
                    {
                        destination.Data<UseWith>()!.OnSuccessEvents!.Add(source.Data<GameEvent>()!);
                    }
                    else
                    {
                        destination.Data<UseWith>()!.OnSuccessEvents!.Remove(source.Data<GameEvent>()!);
                    }
                }
                else if (destination.DataType == typeof(EventTrigger))
                {
                    if (link)
                    {
                        destination.Data<EventTrigger>()!.Events!.Add(source.Data<GameEvent>()!);
                    }
                    else
                    {
                        destination.Data<EventTrigger>()!.Events!.Remove(source.Data<GameEvent>()!);
                    }
                }
                else if (destination.DataType == typeof(MainStory))
                {
                    if (link)
                    {
                        destination.Data<MainStory>()!.GameStartEvents!.Add(source.Data<GameEvent>()!);
                    }
                    else
                    {
                        destination.Data<MainStory>()!.GameStartEvents!.Remove(source.Data<GameEvent>()!);
                    }
                }
                else if (destination.DataType == typeof(Response))
                {
                    if (link)
                    {
                        destination.Data<Response>()!.ResponseEvents!.Add(source.Data<GameEvent>()!);
                    }
                    else
                    {
                        destination.Data<Response>()!.ResponseEvents!.Remove(source.Data<GameEvent>()!);
                    }
                }
                else if (destination.DataType == typeof(Dialogue))
                {
                    var result = MessageBox.Show("Add as StartEvent? Hit yes for StartEvent, no for CloseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        if (link)
                        {
                            destination.Data<Dialogue>()!.StartEvents!.Add(source.Data<GameEvent>()!);
                        }
                        else
                        {
                            destination.Data<Dialogue>()!.StartEvents!.Remove(source.Data<GameEvent>()!);
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        if (link)
                        {
                            destination.Data<Dialogue>()!.CloseEvents!.Add(source.Data<GameEvent>()!);
                        }
                        else
                        {
                            destination.Data<Dialogue>()!.CloseEvents!.Remove(source.Data<GameEvent>()!);
                        }
                    }
                }
                else if (destination.DataType == typeof(BackgroundChatter))
                {
                    if (link)
                    {
                        destination.Data<BackgroundChatter>()!.StartEvents!.Add(source.Data<GameEvent>()!);
                    }
                    else
                    {
                        destination.Data<BackgroundChatter>()!.StartEvents!.Remove(source.Data<GameEvent>()!);
                    }
                }
                else if (destination.DataType == typeof(ItemInteraction))
                {
                    var result = MessageBox.Show(Add + "as OnAcceptEvent? Hit yes for OnAcceptEvent, no for OnRefuseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        if (link)
                        {
                            destination.Data<ItemInteraction>()!.OnAcceptEvents!.Add(source.Data<GameEvent>()!);
                        }
                        else
                        {
                            destination.Data<ItemInteraction>()!.OnAcceptEvents!.Remove(source.Data<GameEvent>()!);
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        if (link)
                        {
                            destination.Data<ItemInteraction>()!.OnRefuseEvents!.Add(source.Data<GameEvent>()!);
                        }
                        else
                        {
                            destination.Data<ItemInteraction>()!.OnRefuseEvents!.Remove(source.Data<GameEvent>()!);
                        }
                    }
                }
                else if (destination.DataType == typeof(ItemGroupInteraction))
                {
                    var result = MessageBox.Show(Add + "as OnAcceptEvent? Hit yes for OnAcceptEvent, no for OnRefuseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        if (link)
                        {
                            destination.Data<ItemGroupInteraction>()!.OnAcceptEvents!.Add(source.Data<GameEvent>()!);
                        }
                        else
                        {
                            destination.Data<ItemGroupInteraction>()!.OnAcceptEvents!.Remove(source.Data<GameEvent>()!);
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        if (link)
                        {
                            destination.Data<ItemGroupInteraction>()!.OnRefuseEvents!.Add(source.Data<GameEvent>()!);
                        }
                        else
                        {
                            destination.Data<ItemGroupInteraction>()!.OnRefuseEvents!.Remove(source.Data<GameEvent>()!);
                        }
                    }
                }
                else if (destination.DataType == typeof(Criterion))
                {
                    if (link)
                    {
                        source.Data<GameEvent>()!.Criteria.Add(destination.Data<Criterion>()!);
                    }
                    else
                    {
                        source.Data<GameEvent>()!.Criteria.Remove(destination.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(Quest))
                {
                    if (link)
                    {
                        var e = source.Data<GameEvent>()!;
                        e.Key = destination.Data<Quest>()!.ID;
                        e.Value = destination.Data<Quest>()!.Name;
                        e.Option = (int)QuestActions.Start;
                        e.Character = string.Empty;
                        e.Character2 = string.Empty;
                    }
                    else
                    {
                        source.Data<GameEvent>()!.EventType = GameEvents.None;
                    }
                }
                else if (destination.DataType == typeof(Trait))
                {
                    if (link)
                    {
                        var e = source.Data<GameEvent>()!;
                        e.EventType = GameEvents.Personality;
                        e.Character = Main.SelectedCharacter;
                        e.Option = (int)destination.Data<Trait>()!.Type;
                        e.Option2 = (int)Modification.Equals;
                        e.Value = destination.Data<Trait>()!.Value.ToString();
                    }
                    else
                    {
                        source.Data<GameEvent>()!.EventType = GameEvents.None;
                    }
                }
            }
            else if (source.DataType == typeof(ItemAction))
            {
                if (destination.DataType == typeof(InteractiveitemBehaviour))
                {
                    if (link)
                    {
                        destination.Data<InteractiveitemBehaviour>()!.ItemActions.Add(source.Data<ItemAction>()!);
                    }
                    else
                    {
                        destination.Data<InteractiveitemBehaviour>()!.ItemActions.Remove(source.Data<ItemAction>()!);
                    }
                }
                else if (destination.DataType == typeof(ItemGroupBehavior))
                {
                    if (link)
                    {
                        destination.Data<ItemGroupBehavior>()!.ItemActions.Add(source.Data<ItemAction>()!);
                    }
                    else
                    {
                        destination.Data<ItemGroupBehavior>()!.ItemActions.Remove(source.Data<ItemAction>()!);
                    }
                }
                else if (destination.DataType == typeof(Criterion))
                {
                    if (link)
                    {
                        source.Data<ItemAction>()!.Criteria.Add(destination.Data<Criterion>()!);
                    }
                    else
                    {
                        source.Data<ItemAction>()!.Criteria.Remove(destination.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(GameEvent))
                {
                    if (link)
                    {
                        source.Data<ItemAction>()!.OnTakeActionEvents.Add(destination.Data<GameEvent>()!);
                    }
                    else
                    {
                        source.Data<ItemAction>()!.OnTakeActionEvents.Remove(destination.Data<GameEvent>()!);
                    }
                }
            }
            else if (source.DataType == typeof(UseWith))
            {
                if (destination.DataType == typeof(InteractiveitemBehaviour))
                {
                    if (link)
                    {
                        destination.Data<InteractiveitemBehaviour>()!.UseWiths.Add(source.Data<UseWith>()!);
                    }
                    else
                    {
                        destination.Data<InteractiveitemBehaviour>()!.UseWiths.Remove(source.Data<UseWith>()!);
                    }
                }
                else if (destination.DataType == typeof(ItemGroupBehavior))
                {
                    if (link)
                    {
                        destination.Data<ItemGroupBehavior>()!.UseWiths.Add(source.Data<UseWith>()!);
                    }
                    else
                    {
                        destination.Data<ItemGroupBehavior>()!.UseWiths.Remove(source.Data<UseWith>()!);
                    }
                }
                else if (destination.DataType == typeof(Criterion))
                {
                    if (link)
                    {
                        source.Data<UseWith>()!.Criteria.Add(destination.Data<Criterion>()!);
                    }
                    else
                    {
                        source.Data<UseWith>()!.Criteria.Remove(destination.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(GameEvent))
                {
                    if (link)
                    {
                        source.Data<UseWith>()!.OnSuccessEvents.Add(destination.Data<GameEvent>()!);
                    }
                    else
                    {
                        source.Data<UseWith>()!.OnSuccessEvents.Remove(destination.Data<GameEvent>()!);
                    }
                }
            }
            else if (source.DataType == typeof(InteractiveitemBehaviour))
            {
                if (destination.DataType == typeof(ItemAction))
                {
                    if (link)
                    {
                        source.Data<InteractiveitemBehaviour>()!.ItemActions.Add(destination.Data<ItemAction>()!);
                    }
                    else
                    {
                        source.Data<InteractiveitemBehaviour>()!.ItemActions.Remove(destination.Data<ItemAction>()!);
                    }
                }
                else if (destination.DataType == typeof(UseWith))
                {
                    if (link)
                    {
                        source.Data<InteractiveitemBehaviour>()!.UseWiths.Add(destination.Data<UseWith>()!);
                    }
                    else
                    {
                        source.Data<InteractiveitemBehaviour>()!.UseWiths.Remove(destination.Data<UseWith>()!);
                    }
                }
                else if (destination.DataType == typeof(InteractiveitemBehaviour))
                {
                    if (link)
                    {
                        destination.Data<ItemGroup>()!.ItemsInGroup.Add(source.Data<InteractiveitemBehaviour>()!.ItemName);
                    }
                    else
                    {
                        destination.Data<ItemGroup>()!.ItemsInGroup.Remove(source.Data<InteractiveitemBehaviour>()!.ItemName);
                    }
                }
            }
            else if (source.DataType == typeof(ItemGroupBehavior))
            {
                if (destination.DataType == typeof(ItemAction))
                {
                    if (link)
                    {
                        source.Data<ItemGroupBehavior>()!.ItemActions.Add(destination.Data<ItemAction>()!);
                    }
                    else
                    {
                        source.Data<ItemGroupBehavior>()!.ItemActions.Remove(destination.Data<ItemAction>()!);
                    }
                }
                else if (destination.DataType == typeof(UseWith))
                {
                    if (link)
                    {
                        source.Data<ItemGroupBehavior>()!.UseWiths.Add(destination.Data<UseWith>()!);
                    }
                    else
                    {
                        source.Data<ItemGroupBehavior>()!.UseWiths.Remove(destination.Data<UseWith>()!);
                    }
                }
                else if (destination.DataType == typeof(InteractiveitemBehaviour))
                {
                    if (link)
                    {
                        destination.Data<ItemGroup>()!.ItemsInGroup.Add(source.Data<InteractiveitemBehaviour>()!.ItemName);
                    }
                    else
                    {
                        destination.Data<ItemGroup>()!.ItemsInGroup.Remove(source.Data<InteractiveitemBehaviour>()!.ItemName);
                    }
                }
            }
            else if (source.DataType == typeof(Achievement))
            {
                if (destination.DataType == typeof(GameEvent))
                {
                    //todo (achievements) when i decide to implement achievements
                }
            }
            else if (source.DataType == typeof(CriteriaListWrapper))
            {
                if (destination.DataType == typeof(Criterion))
                {
                    if (link)
                    {
                        source.Data<CriteriaListWrapper>()!.CriteriaList.Add(destination.Data<Criterion>()!);
                    }
                    else
                    {
                        source.Data<CriteriaListWrapper>()!.CriteriaList.Remove(destination.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(CriteriaGroup))
                {
                    if (link)
                    {
                        destination.Data<CriteriaGroup>()!.CriteriaList.Add(source.Data<CriteriaListWrapper>()!);
                    }
                    else
                    {
                        destination.Data<CriteriaGroup>()!.CriteriaList.Remove(source.Data<CriteriaListWrapper>()!);
                    }
                }
            }
            else if (source.DataType == typeof(CriteriaGroup))
            {
                if (destination.DataType == typeof(CriteriaListWrapper))
                {
                    if (link)
                    {
                        source.Data<CriteriaGroup>()!.CriteriaList.Add(destination.Data<CriteriaListWrapper>()!);
                    }
                    else
                    {
                        source.Data<CriteriaGroup>()!.CriteriaList.Remove(destination.Data<CriteriaListWrapper>()!);
                    }
                }
                else if (destination.DataType == typeof(Criterion))
                {
                    if (link)
                    {
                        var crit = destination.Data<Criterion>()!;
                        crit.CompareType = CompareTypes.CriteriaGroup;
                        crit.Value = source.Data<CriteriaGroup>()!.Name;
                    }
                    else
                    {
                        destination.Data<Criterion>()!.CompareType = CompareTypes.Never;
                    }
                }
            }
            else if (source.DataType == typeof(ItemGroup))
            {
                if (destination.DataType == typeof(ItemInteraction))
                {
                    if (link)
                    {
                        source.Data<ItemGroup>()!.ItemsInGroup.Add(destination.Data<ItemInteraction>()!.ItemName);
                    }
                    else
                    {
                        source.Data<ItemGroup>()!.ItemsInGroup.Remove(destination.Data<ItemInteraction>()!.ItemName);
                    }
                }
                else if (destination.DataType == typeof(InteractiveitemBehaviour))
                {
                    if (link)
                    {
                        source.Data<ItemGroup>()!.ItemsInGroup.Add(destination.Data<InteractiveitemBehaviour>()!.ItemName);
                    }
                    else
                    {
                        source.Data<ItemGroup>()!.ItemsInGroup.Remove(destination.Data<InteractiveitemBehaviour>()!.ItemName);
                    }
                }
                else if (destination.DataType == typeof(Criterion))
                {
                    if (link)
                    {
                        var crit = destination.Data<Criterion>()!;
                        crit.CompareType = CompareTypes.ItemFromItemGroup;
                        crit.Key = source.Data<CriteriaGroup>()!.Name;
                        crit.ItemFromItemGroupComparison = ItemFromItemGroupComparisonTypes.IsActive;
                    }
                    else
                    {
                        destination.Data<Criterion>()!.CompareType = CompareTypes.Never;
                    }
                }
                else if (destination.DataType == typeof(GameEvent))
                {
                    //todo (itemgroups) seems mega involved
                }
            }
            else if (source.DataType == typeof(EventTrigger))
            {
                if (destination.DataType == typeof(Criterion))
                {
                    if (link)
                    {
                        source.Data<EventTrigger>()!.Critera.Add(destination.Data<Criterion>()!);
                    }
                    else
                    {
                        source.Data<EventTrigger>()!.Critera.Remove(destination.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(GameEvent))
                {
                    if (link)
                    {
                        source.Data<EventTrigger>()!.Events.Add(destination.Data<GameEvent>()!);
                    }
                    else
                    {
                        source.Data<EventTrigger>()!.Events.Remove(destination.Data<GameEvent>()!);
                    }
                }
            }
            else if (source.DataType == typeof(CharacterGroup))
            {
                if (destination.DataType == typeof(GameEvent))
                {
                    //todo (charactergroups) too involved to include for now
                }
                else if (destination.DataType == typeof(Criterion))
                {
                    //todo (charactergroups) too involved to include for now
                }
            }
            else if (source.DataType == typeof(AlternateText))
            {
                if (destination.DataType == typeof(Criterion))
                {
                    if (link)
                    {
                        source.Data<AlternateText>()!.Critera.Add(destination.Data<Criterion>()!);
                    }
                    else
                    {
                        source.Data<AlternateText>()!.Critera.Remove(destination.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(Dialogue))
                {
                    if (link)
                    {
                        destination.Data<Dialogue>()!.AlternateTexts.Add(source.Data<AlternateText>()!);
                    }
                    else
                    {
                        destination.Data<Dialogue>()!.AlternateTexts.Remove(source.Data<AlternateText>()!);
                    }
                }
            }
            else if (source.DataType == typeof(Response))
            {
                if (destination.DataType == typeof(Criterion))
                {
                    if (link)
                    {
                        source.Data<Response>()!.ResponseCriteria.Add(destination.Data<Criterion>()!);
                    }
                    else
                    {
                        source.Data<Response>()!.ResponseCriteria.Remove(destination.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(GameEvent))
                {
                    if (link)
                    {
                        source.Data<Response>()!.ResponseEvents.Add(destination.Data<GameEvent>()!);
                    }
                    else
                    {
                        source.Data<Response>()!.ResponseEvents.Remove(destination.Data<GameEvent>()!);
                    }
                }
                else if (destination.DataType == typeof(Dialogue))
                {
                    var result = MessageBox.Show("Lead to this dialogue from the response? Hit yes for that, no to add the response as a normal response to this dialogue", "Select Response place", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        if (link)
                        {
                            source.Data<Response>()!.Next = destination.Data<Dialogue>()!.ID;
                        }
                        else
                        {
                            source.Data<Response>()!.Next = 0;
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        if (link)
                        {
                            destination.Data<Dialogue>()!.Responses.Add(source.Data<Response>()!);
                        }
                        else
                        {
                            destination.Data<Dialogue>()!.Responses.Remove(source.Data<Response>()!);
                        }
                    }
                }
            }
            else if (source.DataType == typeof(Dialogue))
            {
                if (destination.DataType == typeof(GameEvent))
                {
                    var result = MessageBox.Show(Add + "as StartEvent? Hit yes for StartEvent, no for CloseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        if (link)
                        {
                            source.Data<Dialogue>()!.StartEvents!.Add(destination.Data<GameEvent>()!);
                        }
                        else
                        {
                            source.Data<Dialogue>()!.StartEvents!.Remove(destination.Data<GameEvent>()!);
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        if (link)
                        {
                            source.Data<Dialogue>()!.CloseEvents!.Add(destination.Data<GameEvent>()!);
                        }
                        else
                        {
                            source.Data<Dialogue>()!.CloseEvents!.Remove(destination.Data<GameEvent>()!);
                        }
                    }
                }
                else if (destination.DataType == typeof(Response))
                {
                    var result = MessageBox.Show(Add + "as a response? Hit yes for Response, no for the response leading to this dialogue", "Select Response place", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        if (link)
                        {
                            source.Data<Dialogue>()!.Responses.Add(destination.Data<Response>()!);
                        }
                        else
                        {
                            source.Data<Dialogue>()!.Responses.Remove(destination.Data<Response>()!);
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        if (link)
                        {
                            destination.Data<Response>()!.Next = source.Data<Dialogue>()!.ID;
                        }
                        else
                        {
                            destination.Data<Response>()!.Next = 0;
                        }
                    }
                }
                else if (destination.DataType == typeof(AlternateText))
                {
                    if (link)
                    {
                        source.Data<Dialogue>()!.AlternateTexts.Add(destination.Data<AlternateText>()!);
                    }
                    else
                    {
                        source.Data<Dialogue>()!.AlternateTexts.Remove(destination.Data<AlternateText>()!);
                    }
                }
            }
            else if (source.DataType == typeof(BackgroundChatter))
            {
                if (destination.DataType == typeof(Criterion))
                {
                    if (link)
                    {
                        source.Data<BackgroundChatter>()!.Critera.Add(destination.Data<Criterion>()!);
                    }
                    else
                    {
                        source.Data<BackgroundChatter>()!.Critera.Remove(destination.Data<Criterion>()!);
                    }
                }
                else if (destination.DataType == typeof(GameEvent))
                {
                    if (link)
                    {
                        source.Data<BackgroundChatter>()!.StartEvents.Add(destination.Data<GameEvent>()!);
                    }
                    else
                    {
                        source.Data<BackgroundChatter>()!.StartEvents.Remove(destination.Data<GameEvent>()!);
                    }
                }
                else if (destination.DataType == typeof(BackgroundChatterResponse))
                {
                    if (link)
                    {
                        source.Data<BackgroundChatter>()!.Responses.Add(destination.Data<BackgroundChatterResponse>()!);
                    }
                    else
                    {
                        source.Data<BackgroundChatter>()!.Responses.Remove(destination.Data<BackgroundChatterResponse>()!);
                    }
                }
            }
            else if (source.DataType == typeof(BackgroundChatterResponse))
            {
                if (destination.DataType == typeof(BackgroundChatter))
                {
                    if (link)
                    {
                        destination.Data<BackgroundChatter>()!.Responses.Add(source.Data<BackgroundChatterResponse>()!);
                    }
                    else
                    {
                        destination.Data<BackgroundChatter>()!.Responses.Remove(source.Data<BackgroundChatterResponse>()!);
                    }
                }
            }
            else if (source.DataType == typeof(Trait))
            {
                if (destination.DataType == typeof(GameEvent))
                {
                    if (link)
                    {
                        var e = destination.Data<GameEvent>()!;
                        e.EventType = GameEvents.Personality;
                        e.Character = Main.SelectedCharacter;
                        e.Option = (int)source.Data<Trait>()!.Type;
                        e.Option2 = (int)Modification.Equals;
                        e.Value = source.Data<Trait>()!.Value.ToString();
                    }
                    else
                    {
                        destination.Data<GameEvent>()!.EventType = GameEvents.None;
                    }
                }
                else if (destination.DataType == typeof(Criterion))
                {
                    if (link)
                    {
                        var c = destination.Data<Criterion>()!;
                        c.CompareType = CompareTypes.Personality;
                        c.Character = Main.SelectedCharacter;
                        c.Key = ((int)source.Data<Trait>()!.Type).ToString();
                        c.EquationValue = ComparisonEquations.Equals;
                    }
                    else
                    {
                        destination.Data<Criterion>()!.CompareType = CompareTypes.Never;
                    }
                }
            }
            else if (source.DataType == typeof(ExtendedDetail))
            {
                if (destination.DataType == typeof(Quest))
                {
                    if (link)
                    {
                        destination.Data<Quest>()!.ExtendedDetails.Add(source.Data<ExtendedDetail>()!);
                    }
                    else
                    {
                        destination.Data<Quest>()!.ExtendedDetails.Remove(source.Data<ExtendedDetail>()!);
                    }
                }
            }
            else if (source.DataType == typeof(Quest))
            {
                if (destination.DataType == typeof(ExtendedDetail))
                {
                    if (link)
                    {
                        source.Data<Quest>()!.ExtendedDetails.Add(destination.Data<ExtendedDetail>()!);
                    }
                    else
                    {
                        source.Data<Quest>()!.ExtendedDetails.Remove(destination.Data<ExtendedDetail>()!);
                    }
                }
                else if (destination.DataType == typeof(Criterion))
                {
                    if (link)
                    {
                        var crit = destination.Data<Criterion>()!;
                        crit.CompareType = CompareTypes.Quest;
                        crit.Key = source.Data<Quest>()!.ID;
                        crit.Key2 = source.Data<Quest>()!.Name;
                        crit.Value = QuestStatus.NotObtained.ToString();
                    }
                    else
                    {
                        destination.Data<Criterion>()!.CompareType = CompareTypes.Never;
                    }
                }
                else if (destination.DataType == typeof(GameEvent))
                {
                    if (link)
                    {
                        var e = destination.Data<GameEvent>()!;
                        e.EventType = GameEvents.Quest;
                        e.Key = source.Data<Quest>()!.ID;
                        e.Value = source.Data<Quest>()!.Name;
                        e.Option = (int)QuestActions.Start;
                        e.Character = string.Empty;
                        e.Character2 = string.Empty;
                    }
                    else
                    {
                        destination.Data<GameEvent>()!.EventType = GameEvents.None;
                    }
                }
            }
            else if (source.DataType == typeof(ItemInteraction))
            {
                if (destination.DataType == typeof(ItemGroup))
                {
                    if (link)
                    {
                        destination.Data<ItemGroup>()!.ItemsInGroup.Add(source.Data<ItemInteraction>()!.ItemName);
                    }
                    else
                    {
                        destination.Data<ItemGroup>()!.ItemsInGroup.Remove(source.Data<ItemInteraction>()!.ItemName);
                    }
                }
                else if (destination.DataType == typeof(GameEvent))
                {
                    var result = MessageBox.Show(Add + "as OnAcceptEvent? Hit yes for OnAcceptEvent, no for OnRefuseEvent", "Select Event Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        if (link)
                        {
                            source.Data<ItemInteraction>()!.OnAcceptEvents!.Add(source.Data<GameEvent>()!);
                        }
                        else
                        {
                            source.Data<ItemInteraction>()!.OnAcceptEvents!.Remove(source.Data<GameEvent>()!);
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        if (link)
                        {
                            source.Data<ItemInteraction>()!.OnRefuseEvents!.Add(source.Data<GameEvent>()!);
                        }
                        else
                        {
                            source.Data<ItemInteraction>()!.OnRefuseEvents!.Remove(source.Data<GameEvent>()!);
                        }
                    }
                }
            }
            else if (source.DataType == typeof(ItemGroupInteraction))
            {
                //todo (itemgroup) dont how how yet, dont care :D
            }
            else if (source.DataType == typeof(string))
            {
                //either value or state
                if (StateNames.Contains(source.Data<string>()))
                {
                    //state
                    if (destination.DataType == typeof(Criterion))
                    {
                        if (link)
                        {
                            var crit = destination.Data<Criterion>()!;
                            crit.CompareType = CompareTypes.State;
                            crit.Character = source.FileName;
                            crit.Value = source.Data<string>() ?? InteractiveStates.Alive.ToString();
                            crit.BoolValue = BoolCritera.True;
                        }
                        else
                        {
                            destination.Data<Criterion>()!.CompareType = CompareTypes.Never;
                        }
                    }
                    else if (destination.DataType == typeof(GameEvent))
                    {
                        if (link)
                        {
                            var e = destination.Data<GameEvent>()!;
                            e.EventType = GameEvents.State;
                            e.Value = source.Data<string>()!;
                            e.Option = (int)AddRemoveActions.Add;
                            e.Character = source.FileName;
                            e.Character2 = string.Empty;
                        }
                        else
                        {
                            destination.Data<GameEvent>()!.EventType = GameEvents.None;
                        }
                    }
                }
                else
                {
                    //value
                    if (destination.DataType == typeof(Criterion))
                    {
                        if (link)
                        {
                            var crit = destination.Data<Criterion>()!;
                            crit.CompareType = CompareTypes.Value;
                            crit.Key = source.Data<string>()!;
                            crit.Character = source.FileName;
                            crit.Value = "";
                        }
                        else
                        {
                            destination.Data<Criterion>()!.CompareType = CompareTypes.Never;
                        }
                    }
                    else if (destination.DataType == typeof(GameEvent))
                    {
                        if (link)
                        {
                            var e = destination.Data<GameEvent>()!;
                            e.EventType = GameEvents.ModifyValue;
                            e.Key = source.Data<string>()!;
                            e.Value = "0";
                            e.Option = (int)Modification.Add;
                            e.Character = source.FileName;
                            e.Character2 = string.Empty;
                        }
                        else
                        {
                            destination.Data<GameEvent>()!.EventType = GameEvents.None;
                        }
                    }
                }
            }
        }

        public static void Link(NodeStore nodes, Node addFrom, Node addToThis)
        {
            if (addToThis.DataType == typeof(MissingReferenceInfo))
            {
                return;
            }
            if (addFrom.DataType == typeof(MissingReferenceInfo))
            {
                return;
            }

            AllLink(addFrom, addToThis, true);

            NodeLinker.UpdateLinks(addFrom, Main.SelectedCharacter, nodes);
            NodeLinker.UpdateLinks(addToThis, Main.SelectedCharacter, nodes);
        }

        public static void Unlink(NodeStore nodes, Node removeFrom, Node removeThis)
        {
            if (removeThis.DataType == typeof(MissingReferenceInfo))
            {
                return;
            }
            if (removeFrom.DataType == typeof(MissingReferenceInfo))
            {
                return;
            }

            AllLink(removeFrom, removeThis, false);

            NodeLinker.UpdateLinks(removeFrom, Main.SelectedCharacter, nodes);
            NodeLinker.UpdateLinks(removeThis, Main.SelectedCharacter, nodes);
        }

        public static void Interlinknodes(NodeStore store, string filename)
        {
            var lastSelected = Main.SelectedCharacter;
            Main.SelectedCharacter = filename;
            DateTime start = DateTime.UtcNow;
            //lists to save new stuff
            try
            {
                int count = store.Count;
                var nodes = store.KeyNodes().ToList();
                List<string> links = [];
                //link up different stories and dialogues
                //doesnt matter that we add some in here, we only care about the ones added so far
                for (int i = 0; i < count; i++)
                {
                    if (nodes[i].Type == NodeType.GameEvent && !links.Contains(nodes[i].ID) && (nodes[i].Data<GameEvent>() is not null))
                    {
                        var dupeEvents = nodes.FindAll((n) => n.ID == nodes[i].ID && (n.Data<GameEvent>()?.Equals(nodes[i].Data<GameEvent>()) ?? false));

                        if (dupeEvents.Count > 1)
                        {
                            links.Add(nodes[i].ID);
                            for (int j = 1; j < dupeEvents.Count; j++)
                            {
                                nodes[i].DupeToOtherSorting(dupeEvents[j].FileName);
                                store.Replace(dupeEvents[j], nodes[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            try
            {
                int count = store.Count;
                var nodes = store.KeyNodes().ToList();
                //link up different stories and dialogues
                //doesnt matter that we add some in here, we only care about the ones added so far
                for (int i = 0; i < count; i++)
                {
                    //link all useful criteria and add influencing values as parents
                    AnalyzeAndConnectNode(store, nodes[i], nodes, true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            //merge doors with items if applicable
            MergeDoors(store, true);

            Debug.WriteLine($"completed for {Main.SelectedCharacter}/{store.Count} nodes in {(DateTime.UtcNow - start).TotalMilliseconds}ms");

            Main.SelectedCharacter = lastSelected;
        }

        public static void UpdateLinks(Node node, string fileName, NodeStore store)
        {
            var lastSelected = Main.SelectedCharacter;
            Main.SelectedCharacter = fileName;
            var family = store[node];

            List<Node> childs = [.. family.Childs];
            foreach (var child in childs)
            {
                store.RemoveChild(node, child);
            }
            List<Node> parents = [.. family.Parents];
            foreach (var parent in parents)
            {
                store.RemoveParent(node, parent);
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
            Main.SelectedCharacter = lastSelected;
            Main.NeedsSaving = true;
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
            string StoryItem;
            string State;

            switch (node.Type)
            {
                case NodeType.Criterion when (criterion = node.Data<Criterion>()!) is not null:
                {
                    if (!Node.AllowedFileNames.Contains(criterion.Character) && !string.IsNullOrWhiteSpace(criterion.Character))
                    {
                        //todo (charactergroup) once charactergroups are implemented correctly
                        criterion.Character = Main.Player;
                    }
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
                            Values.TryGetValue(criterion.Character + criterion.Key, out result);
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
                                Values.Add(value.FileName + value.ID, value);
                                nodes.AddParent(node, value);
                            }
                            Values.TryGetValue(criterion.Character2 + criterion.Key2, out result);
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
                                Values.Add(value.FileName + value.ID, value);
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
                                    FileName = Main.SelectedCharacter!
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
                                    FileName = Main.SelectedCharacter!
                                };
                                Doors.Add(door);
                                nodes.AddParent(node, door);
                            }
                            break;
                        }
                        case CompareTypes.Item:
                        {
                            result = StoryItems.Find((n) => n.Type == NodeType.StoryItem && n.ID == criterion.Key);
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
                                    RawData = criterion.Key!,
                                    FileName = Main.SelectedCharacter!
                                };
                                StoryItems.Add(item);
                                nodes.AddParent(node, item);
                            }
                            break;
                        }
                        case CompareTypes.IsCurrentlyBeingUsed:
                        {
                            result = StoryItems.Find((n) => n.Type == NodeType.StoryItem && n.ID == criterion.Key);
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
                                    RawData = criterion.Key!,
                                    FileName = Main.SelectedCharacter!
                                };
                                StoryItems.Add(item);
                                nodes.AddParent(node, item);
                            }
                            break;
                        }
                        case CompareTypes.IsCurrentlyUsing:
                        {
                            result = StoryItems.Find((n) => n.Type == NodeType.StoryItem && n.ID == criterion.Key);
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
                                    RawData = criterion.Key!,
                                    FileName = Main.SelectedCharacter!
                                };
                                StoryItems.Add(item);
                                nodes.AddParent(node, item);
                            }
                            break;
                        }
                        case CompareTypes.ItemFromItemGroup:
                        {
                            result = StoryItems.Find((n) => n.Type == NodeType.ItemGroup && n.StaticText == criterion.Key);
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
                                var item = new Node(criterion.Key!, NodeType.ItemGroup, criterion.Key!)
                                {
                                    RawData = criterion.Key!,
                                    FileName = Main.SelectedCharacter!
                                };
                                StoryItems.Add(item);
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
                                    FileName = Main.SelectedCharacter!
                                };
                                InventoryItems.Add(item);
                                nodes.AddParent(node, item);
                            }
                            //find normal item if it exists
                            result = StoryItems.Find((n) => n.Type == NodeType.StoryItem && n.ID == criterion.Key);
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
                                    FileName = Main.SelectedCharacter!
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
                            result = Socials.Find((n) => n.ID == criterion.Character + criterion.SocialStatus + criterion.Character2);
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
                            result = States.Find((n) => n.FileName == criterion.Character && n.StaticText.AsSpan()[..3].Contains(criterion.Value!.AsSpan(), StringComparison.InvariantCulture));
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
                            Values.TryGetValue(criterion.Character + criterion.Key, out result);
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
                                Values.Add(value.FileName + value.ID, value);
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
                    if (!Node.AllowedFileNames.Contains(gameEvent.Character) && !string.IsNullOrWhiteSpace(gameEvent.Character))
                    {
                        //todo (charactergroup) once charactergroups are implemented correctly
                        gameEvent.Character = Main.Player;
                    }

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
                                var clothing = new Node(gameEvent.Option + gameEvent.Value, NodeType.Clothing, gameEvent.Character + "'s  " + (gameEvent.Option4 == 0 ? (((ClothingType)int.Parse(gameEvent.Value!)).ToString()) : gameEvent.Value) + " in set " + (gameEvent.Option == 0 ? "any" : (gameEvent.Option - 1).ToString())) { FileName = gameEvent.Character! };
                                Clothing.Add(clothing);
                                nodes.AddChild(node, clothing);
                                clothing.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.MatchValue:
                        case GameEvents.CombineValue:
                        {
                            Values.TryGetValue(gameEvent.Character + gameEvent.Key, out result);
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
                                Values.Add(value.FileName + value.ID, value);
                                nodes.AddChild(node, value);
                                value.DupeToOtherSorting(node.FileName);
                            }
                            Values.TryGetValue(gameEvent.Character2 + gameEvent.Value, out result);
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
                                Values.Add(value.FileName + value.ID, value);
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
                                    FileName = Main.SelectedCharacter!
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
                                    FileName = Main.SelectedCharacter!
                                };
                                Doors.Add(door);
                                nodes.AddChild(node, door);
                                door.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.EventTriggers:
                        {
                            result = searchIn.Find((n) => n.Type == NodeType.EventTrigger && n.Text == gameEvent.Value);
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
                                var _event = new Node(gameEvent.Value, NodeType.EventTrigger, gameEvent.Value)
                                {
                                    FileName = gameEvent.Character!
                                };
                                searchIn.Add(_event);
                                nodes.AddChild(node, _event);
                                _event.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.Item:
                        {
                            result = StoryItems.Find((n) => n.Type == NodeType.StoryItem && n.ID == gameEvent.Key);
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
                                    RawData = gameEvent.Key!,
                                    FileName = Main.SelectedCharacter!
                                };
                                StoryItems.Add(item);
                                nodes.AddChild(node, item);
                                item.DupeToOtherSorting(node.FileName);
                            }
                            break;
                        }
                        case GameEvents.ItemFromItemGroup:
                        {
                            result = StoryItems.Find((n) => n.Type == NodeType.ItemGroup && n.ID == gameEvent.Key);
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
                                var item = new Node(gameEvent.Key!, NodeType.ItemGroup, gameEvent.Key!)
                                {
                                    RawData = gameEvent.Key!,
                                    FileName = Main.SelectedCharacter!
                                };
                                StoryItems.Add(item);
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
                            Values.TryGetValue(gameEvent.Character + gameEvent.Key, out result);
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
                                Values.Add(value.FileName + value.ID, value);
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
                                    FileName = Main.SelectedCharacter!
                                };
                                InventoryItems.Add(item);
                                nodes.AddParent(node, item);
                                item.DupeToOtherSorting(node.FileName);
                            }
                            result = StoryItems.Find((n) => n.Type == NodeType.StoryItem && n.ID == gameEvent.Value);
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
                                    FileName = Main.SelectedCharacter!
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
                            Values.TryGetValue(gameEvent.Character + gameEvent.Key, out result);
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
                                Values.Add(value.FileName + value.ID, value);
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
                            result = States.Find((n) => n.FileName == gameEvent.Character && n.StaticText.AsSpan()[..3].Contains(gameEvent.Value!.AsSpan(), StringComparison.InvariantCulture));
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
                        var dialogueNode = new Node(response.Next.ToString(), NodeType.Dialogue, $"dialogue number {response.Next} for {node.FileName}") { FileName = Main.SelectedCharacter };
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
                                var respNode = new Node(resp.Id!, NodeType.Response, $"response to {dialogue.ID} for {node.FileName}") { FileName = Main.SelectedCharacter };
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
                                var _node = new Node($"{dialogue.ID}.{alternate.Order!}", NodeType.AlternateText, $"alternate to {dialogue.ID} for {node.FileName}") { FileName = Main.SelectedCharacter };
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
                            var newNode = new Node($"{_response.CharacterName}{_response.ChatterId}", NodeType.BGCResponse, $"{_response.CharacterName}{_response.ChatterId}") { FileName = Main.SelectedCharacter };
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
                        result = StoryItems.Find((n) => n.Type == NodeType.StoryItem && n.ID == item);

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
                            var newNode = new Node(item, NodeType.StoryItem, item, item) { FileName = Main.SelectedCharacter };
                            StoryItems.Add(newNode);
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

                    result = StoryItems.Find((n) => n.Type == NodeType.StoryItem && n.ID == useWith.ItemName!);
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
                        var newNode = new Node(useWith.ItemName ?? string.Empty, NodeType.StoryItem, useWith.ItemName ?? string.Empty, useWith.ItemName!) { FileName = Main.SelectedCharacter };
                        StoryItems.Add(newNode);
                        nodes.AddChild(node, newNode);
                    }
                    break;
                }
                case NodeType.StoryItem when (StoryItem = node.Data<string>()!) is not null:
                {
                    result = StoryItems.Find(n => n.ID == StoryItem);
                    if (result is not null)
                    {
                        if (dupeTo)
                        {
                            result.DupeToOtherSorting(node.FileName);
                        }

                        nodes.Replace(node, result);
                    }
                    else
                    {
                        StoryItems.Add(node);
                    }
                    break;
                }
                case NodeType.State when (State = node.Data<string>()!) is not null:
                {
                    result = States.Find(n => n.ID == State);
                    if (result is not null)
                    {
                        if (dupeTo)
                        {
                            result.DupeToOtherSorting(node.FileName);
                        }

                        nodes.Replace(node, result);
                    }
                    else
                    {
                        States.Add(node);
                    }
                    break;
                }
            }

            if (nodes.Childs(node).Count == 0
                && nodes.Parents(node).Count == 0
                && node.FileName != Main.SelectedCharacter)
            {
                nodes.Remove(node);
                node.RemoveFromSorting(Main.SelectedCharacter);
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
                var newNode = new Node(_usewith.ItemName ?? string.Empty, NodeType.UseWith, _usewith.CustomCantDoThatMessage ?? string.Empty) { FileName = Main.SelectedCharacter };
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
                if (node.DataType == typeof(ItemGroupBehavior))
                {
                    var newNode = new Node(node.Data<ItemGroupBehavior>()!.Name + " " + _action.ActionName ?? string.Empty, NodeType.ItemAction, _action.ActionName ?? string.Empty) { FileName = Main.SelectedCharacter };
                    newList.Add(newNode);
                    nodes.AddChild(node, newNode);
                }
                else if (node.DataType == typeof(InteractiveitemBehaviour))
                {
                    var newNode = new Node(node.Data<InteractiveitemBehaviour>()!.ItemName + " " + _action.ActionName ?? string.Empty, NodeType.ItemAction, _action.ActionName ?? string.Empty) { FileName = Main.SelectedCharacter };
                    newList.Add(newNode);
                    nodes.AddChild(node, newNode);
                }
            }
        }

        private static void HandleCriterion(NodeStore nodes, Node node, List<Node> newList, Criterion _criterion, bool dupeTo = false)
        {
            Node? result = newList.Find((n) => n.Type == NodeType.Criterion && n.ID == $"{_criterion.Character}{_criterion.CompareType}{_criterion.Key}{_criterion.Value}");
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
                newList.Add(Node.CreateCriteriaNode(_criterion, node.FileName, nodes));
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
                var eventNode = new Node(_event.Id ?? "none", NodeType.GameEvent, _event.Value ?? "none") { FileName = Main.SelectedCharacter };
                newList.Add(eventNode);
                nodes.AddChild(node, eventNode);
            }
        }

        public static void InterlinkBetweenFiles(Dictionary<string, NodeStore> stores, bool removeDuplicateGUIDs = false)
        {
            var lastSelected = Main.SelectedCharacter;

            //check some comparevaluenodes.again because the referenced values havent been added yet
            RecheckValues(stores);

            foreach (var store in stores.Keys)
            {
                if (store == Main.NoCharacter)
                {
                    continue;
                }

                Main.SelectedCharacter = store;

                var tempList = stores[store].Nodes;

                for (int i = 0; i < tempList.Count; i++)
                {
                    Node? node = tempList[i];
                    if (node.FileName == Main.NoCharacter)
                    {
                        //currently we dont have any here <3
                        Debugger.Break();
                    }

                    if (node.DataType != typeof(MissingReferenceInfo) || !stores.TryGetValue(node.FileName, out var nodeStore))
                    {
                        continue;
                    }

                    var templist2 = nodeStore.Nodes;

                    if (node.Type == NodeType.Dialogue && node.ID == string.Empty)
                    {
                        node.ID = 0.ToString();
                    }

                    List<Node> result = [];
                    if (node.Type == NodeType.EventTrigger && !GUIDRegex().IsMatch(node.ID))
                    {
                        //special handling for referenced eventtriggers as the missingrefeence has the name as id and not a guid
                        result = templist2.FindAll(n => n.Type == node.Type && n.Text == node.ID && n.FileName == node.FileName && n.DataType != typeof(MissingReferenceInfo));
                    }
                    else if (node.Type == NodeType.Criterion)
                    {
                        //never happens :D
                        Debugger.Break();
                    }
                    else
                    {
                        //try and find similar nodes that arent missing reference
                        result = templist2.FindAll(n => n.Type == node.Type && n.ID == node.ID && n.FileName == node.FileName && n.DataType != typeof(MissingReferenceInfo));
                    }
                    foreach (var foundNode in result)
                    {
                        if (foundNode is not null)
                        {
                            foundNode.DupeToOtherSorting(store);
                            stores[store].Add(foundNode);
                            stores[store].Replace(node, foundNode);
                            Main.ClearAllNodePos(node);
                        }
                    }
                }

                if (removeDuplicateGUIDs)
                {
                    RemoveDuplicateGUIDs(stores[store]);
                }
            }

            Main.SelectedCharacter = lastSelected;
        }

        private static void RemoveDuplicateGUIDs(NodeStore store)
        {
            var tempList = store.Nodes;

            for (int i = 0; i < tempList.Count; i++)
            {
                Node? node = tempList[i];

                for (int j = i + 1; j < tempList.Count; j++)
                {
                    Node? duplicateNode = tempList[j];

                    //eventtriggers have duplicate names but the data isnt the same
                    if (node.ID == duplicateNode.ID && node.OrigFileName == duplicateNode.OrigFileName)
                    {
                        if (!GUIDRegex().IsMatch(duplicateNode.ID))
                        {
                            if (node.Type == duplicateNode.Type)
                            {
                                if (node.Type == NodeType.EventTrigger && node.DataType == typeof(EventTrigger) && duplicateNode.DataType == typeof(EventTrigger))
                                {
                                    duplicateNode.Data<EventTrigger>()!.Id = Guid.NewGuid().ToString();
                                }
                                else
                                {
                                    Debugger.Break();
                                }
                            }
                            continue;
                        }

                        duplicateNode.ID = Guid.NewGuid().ToString();
                        if (duplicateNode.RawData is GameEvent g)
                        {
                            g.Id = Guid.NewGuid().ToString();
                        }
                        else if (duplicateNode.RawData is EventTrigger e)
                        {
                            e.Id = Guid.NewGuid().ToString();
                        }
                        else if (duplicateNode.RawData is ItemGroup h)
                        {
                            h.Id = Guid.NewGuid().ToString();
                        }
                        else if (duplicateNode.RawData is ItemGroupBehavior f)
                        {
                            f.Id = Guid.NewGuid().ToString();
                        }
                        else if (duplicateNode.RawData is ItemGroupInteraction d)
                        {
                            d.Id = Guid.NewGuid().ToString();
                        }
                        else if (duplicateNode.RawData is Response r)
                        {
                            r.Id = Guid.NewGuid().ToString();
                        }
                        else if (duplicateNode.RawData is Achievement a)
                        {
                            a.Id = Guid.NewGuid().ToString();
                        }
                        else if (duplicateNode.RawData is Quest q)
                        {
                            q.ID = Guid.NewGuid().ToString();
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
            StoryItems.Clear();
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

        private static void RecheckValues(Dictionary<string, NodeStore> stores)
        {
            List<Node> values = [.. Values.Values];
            foreach (Node node in values)
            {
                //we have the real nodehere, replace all false ones
                if (node.DataType == typeof(string))
                {
                    foreach (var result in values.FindAll(n => n.Type == NodeType.Value && n.DataType == typeof(MissingReferenceInfo) && n.ID == node.ID && n.FileName == node.FileName))
                    {
                        foreach (var store in stores)
                        {
                            var last = Main.SelectedCharacter;
                            Main.SelectedCharacter = store.Key;

                            if (store.Value.Contains(result))
                            {
                                store.Value.Add(node);
                                store.Value.Replace(result, node);
                            }
                            Main.SelectedCharacter = last;
                        }
                        Values.Remove(result.FileName + result.ID);
                        Main.ClearAllNodePos(result);
                    }
                }
            }
        }

        public static void DissectCharacter(CharacterStory story, NodeStore nodes)
        {
            if (story is not null && nodes is not null)
            {
                var last = Main.SelectedCharacter;
                Main.SelectedCharacter = story.CharacterName!;
                //get all relevant items from the json
                StoryNodeExtractor.GetValues(story, nodes);
                foreach (var node in nodes.Nodes)
                {
                    if (node.DataType == typeof(string))
                    {
                        Values[node.FileName + node.ID] = node;
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
                Main.SelectedCharacter = last;
            }
        }

        public static void DissectStory(MainStory story, NodeStore nodes)
        {
            if (story is not null && nodes is not null)
            {
                var last = Main.SelectedCharacter;
                Main.SelectedCharacter = Main.Player;
                StoryNodeExtractor.GetValues(story, nodes);
                foreach (var node in nodes.Nodes)
                {
                    if (node.DataType == typeof(string))
                    {
                        Values[node.FileName + node.ID] = node;
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
                Main.SelectedCharacter = last;
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

        [GeneratedRegex("[0-9a-fA-F\\-]{32,36}")]
        private static partial Regex GUIDRegex();
    }
}
