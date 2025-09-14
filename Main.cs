using CSC.StoryItems;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using System.Xml.Linq;
using static CSC.StoryItems.StoryEnums;

namespace CSC
{
    public partial class Main : Form
    {
        public const int NodeSizeX = 200;
        public const int NodeSizeY = 50;
        public static readonly NodeStore nodes = new();
        public bool MovingChild = false;
        private const int NodeCount = 100;
        private readonly SolidBrush achievementNodeBrush;
        private readonly SolidBrush alternateTextNodeBrush;
        private readonly SolidBrush bgcNodeBrush;
        private readonly SolidBrush bgcResponseNodeBrush;
        private readonly SolidBrush characterGroupNodeBrush;
        private readonly SolidBrush clothingNodeBrush;
        private readonly SolidBrush criteriaGroupNodeBrush;
        private readonly SolidBrush criterionNodeBrush;
        private readonly SolidBrush cutsceneNodeBrush;
        private readonly Brush DarkFontBrush;
        private readonly SolidBrush defaultNodeBrush;
        private readonly SolidBrush dialogueNodeBrush;
        private readonly SolidBrush doorNodeBrush;
        private readonly List<Node> Doors = [];
        private readonly SolidBrush eventNodeBrush;
        private readonly SolidBrush eventTriggerNodeBrush;
        private readonly SolidBrush HighlightNodeBrush;
        private readonly Pen highlightPen;
        private readonly SolidBrush inventoryNodeBrush;
        private readonly SolidBrush itemActionNodeBrush;
        private readonly SolidBrush itemGroupBehaviourNodeBrush;
        private readonly SolidBrush itemGroupInteractionNodeBrush;
        private readonly SolidBrush itemGroupNodeBrush;
        private readonly SolidBrush itemNodeBrush;
        private readonly List<int> layerYperX = [];
        private readonly Brush LightFontBrush;
        private readonly Pen linePen;
        private readonly SolidBrush personalityNodeBrush;
        private readonly SolidBrush poseNodeBrush;
        private readonly SolidBrush propertyNodeBrush;
        private readonly SolidBrush questNodeBrush;
        private readonly SolidBrush responseNodeBrush;
        private readonly int scaleX = NodeSizeX + 40;
        private readonly int scaleY = NodeSizeY + 20;
        private readonly SolidBrush socialNodeBrush;
        private readonly SolidBrush stateNodeBrush;
        private readonly SolidBrush valueNodeBrush;
        private readonly List<Node> Values = [];
        private readonly List<Node> visited = [];
        private float AfterZoomMouseX;
        private float AfterZoomMouseY;
        private float BeforeZoomMouseX;
        private float BeforeZoomMouseY;
        private int counter = 0;
        private bool CurrentlyInPan = false;
        private PointF end;
        private Node highlightNode = Node.NullNode;
        private bool IsCtrlPressed;
        private bool IsShiftPressed;
        private Node lastNode = Node.NullNode;
        private Node movedNode;
        private Size OffsetFromDragClick = Size.Empty;
        private float OffsetX;
        private float OffsetY;
        private Cursor priorCursor = Cursors.Default;
        private int runningTotal = 0;
        private float Scaling = 0.3f;
        private PointF start;
        private float StartPanOffsetX = 0f;
        private float StartPanOffsetY = 0f;
        private Font scaledFont = DefaultFont;
        public Main()
        {
            InitializeComponent();
            Application.AddMessageFilter(new MouseMessageFilter());
            MouseMessageFilter.MouseMove += HandleMouseEvents;
            linePen = new Pen(Brushes.LightGray, 2)
            {
                EndCap = LineCap.Triangle,
                StartCap = LineCap.Round
            };
            highlightPen = new Pen(Brushes.Green, 3)
            {
                EndCap = LineCap.Triangle,
                StartCap = LineCap.Round
            };
            defaultNodeBrush = new SolidBrush(Color.FromArgb(255, 100, 100, 100));
            achievementNodeBrush = new SolidBrush(Color.FromArgb(255, 200, 10, 200));
            alternateTextNodeBrush = new SolidBrush(Color.FromArgb(255, 110, 120, 190));
            bgcNodeBrush = new SolidBrush(Color.FromArgb(255, 40, 190, 255));
            bgcResponseNodeBrush = new SolidBrush(Color.FromArgb(255, 150, 225, 255));
            characterGroupNodeBrush = new SolidBrush(Color.FromArgb(255, 190, 180, 130));
            clothingNodeBrush = new SolidBrush(Color.FromArgb(255, 115, 235, 30));
            criteriaGroupNodeBrush = new SolidBrush(Color.FromArgb(255, 150, 50, 50));
            criterionNodeBrush = new SolidBrush(Color.FromArgb(255, 220, 20, 20));
            cutsceneNodeBrush = new SolidBrush(Color.FromArgb(255, 235, 30, 160));
            dialogueNodeBrush = new SolidBrush(Color.FromArgb(255, 45, 60, 185));
            doorNodeBrush = new SolidBrush(Color.FromArgb(255, 200, 225, 65));
            eventNodeBrush = new SolidBrush(Color.FromArgb(255, 50, 150, 50));
            eventTriggerNodeBrush = new SolidBrush(Color.FromArgb(255, 20, 220, 20));
            inventoryNodeBrush = new SolidBrush(Color.FromArgb(255, 65, 225, 185));
            itemActionNodeBrush = new SolidBrush(Color.FromArgb(255, 85, 195, 195));
            itemGroupBehaviourNodeBrush = new SolidBrush(Color.FromArgb(255, 160, 200, 195));
            itemGroupInteractionNodeBrush = new SolidBrush(Color.FromArgb(255, 95, 120, 115));
            itemGroupNodeBrush = new SolidBrush(Color.FromArgb(255, 45, 190, 165));
            itemNodeBrush = new SolidBrush(Color.FromArgb(255, 45, 255, 255));
            personalityNodeBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 90));
            poseNodeBrush = new SolidBrush(Color.FromArgb(255, 255, 210, 90));
            propertyNodeBrush = new SolidBrush(Color.FromArgb(255, 255, 90, 150));
            questNodeBrush = new SolidBrush(Color.FromArgb(255, 150, 210, 155));
            responseNodeBrush = new SolidBrush(Color.FromArgb(255, 55, 155, 225));
            socialNodeBrush = new SolidBrush(Color.FromArgb(255, 255, 160, 90));
            stateNodeBrush = new SolidBrush(Color.FromArgb(255, 40, 190, 50));
            valueNodeBrush = new SolidBrush(Color.FromArgb(255, 40, 0, 190));
            HighlightNodeBrush = new SolidBrush(Color.DarkCyan);
            LightFontBrush = Brushes.White;
            DarkFontBrush = Brushes.Black;

        }

        public string? FileName { get; private set; }
        public void HandleKeyBoard(object? sender, KeyEventArgs e)
        {
            //get the shift key state so we can determine later if we want to redraw the tree on node selection or not
            IsShiftPressed = e.KeyData == (Keys.ShiftKey | Keys.Shift);
            IsCtrlPressed = e.KeyData == (Keys.Control | Keys.ControlKey);
            if (!IsCtrlPressed)
            {
                movedNode = Node.NullNode;
                MovingChild = false;
            }
        }

        public void HandleMouseEvents(object? sender, MouseEventArgs e)
        {
            if (Main.ActiveForm is null)
            {
                return;
            }
            var pos = Main.ActiveForm!.PointToClient(Cursor.Position);
            ScreenToGraph(pos.X, pos.Y, out float ScreenPosX, out float ScreenPosY);
            var ScreenPos = new Point((int)ScreenPosX, (int)ScreenPosY);
            //set old position for next frame/call
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (e.Clicks > 1)
                    {
                        Details.Visible = !Details.Visible;
                    }
                    else
                    {
                        UpdateClickedNode(ScreenPosX, ScreenPosY, ScreenPos);
                    }
                    break;
                case MouseButtons.None:
                {
                    EndPan();
                    UpdateHighlightNode(ScreenPos);
                }
                break;
                case MouseButtons.Right:
                    break;
                case MouseButtons.Middle:
                    UpdatePan(pos);
                    break;
                case MouseButtons.XButton1:
                    break;
                case MouseButtons.XButton2:
                    break;
                default:
                {
                    EndPan();
                    UpdateHighlightNode(ScreenPos);
                }
                break;
            }

            if (MovingChild && movedNode != Node.NullNode)
            {
                movedNode.Position = ScreenPos + OffsetFromDragClick;
                Invalidate();
            }


            //everything else, scrolling for example
            if (e.Delta != 0)
            {
                //todo we need a limit here so we dont scroll out too far and make the texture hugeeee
                UpdateScaling(e);
                //redraw
                Invalidate();
            }
        }

        public void ScreenToGraph(float screenX, float screenY, out float graphX, out float graphY)
        {
            graphX = screenX / Scaling + OffsetX;
            graphY = screenY / Scaling + OffsetY;
        }

        private static void DissectStory(MainStory story)
        {
            if (story is not null)
            {
                //add all items in the story
                StoryNodeExtractor.GetItemOverrides(story);
                //add all item groups with their actions
                StoryNodeExtractor.GetItemGroups(story);
                //add all items in the story
                StoryNodeExtractor.GetAchievements(story);
                //add all reactions the player will say
                StoryNodeExtractor.GetPlayerReactions(story);
                //add all criteriagroups
                StoryNodeExtractor.GetCriteriaGroups(story);
                //gets the playervalues
                StoryNodeExtractor.GetValues(story);
                //the events which fire at game start
                StoryNodeExtractor.GetGameStartEvents(story);
                //add all item groups actions
                StoryNodeExtractor.GetItemGroupBehaviours(story);
            }
        }

        private static Node GetNodeAtPoint(Point mouseGraphLocation)
        {
            foreach (var key in NodePositionSorting.Singleton[mouseGraphLocation])
            {
                if (new RectangleF(key.Position, key.Size).Contains(mouseGraphLocation))
                {
                    return key;
                }
            }
            return Node.NullNode;
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

        private void Add_Click(object sender, EventArgs e)
        {
            Node node = CreateNode(typeof(Criterion));

            Main.nodes.Add(node);

            lastNode = node;

            Invalidate();
        }

        private void AddChild_Click(object sender, EventArgs e)
        {
            Node node = CreateNode(typeof(Response));

            Main.nodes.AddChild(lastNode, node);

            lastNode = node;

            Invalidate();
        }

        private void AddParent_Click(object sender, EventArgs e)
        {
            Node node = CreateNode(typeof(Dialogue));

            Main.nodes.AddParent(lastNode, node);

            lastNode = node;

            Invalidate();
        }

        private Node CreateNode(Type type)
        {
            object item;
            if (type == typeof(Dialogue))
            {
                item = new Dialogue
                {
                    ID = counter
                };
            }
            else if (type == typeof(Response))
            {
                item = new Response
                {
                    Order = counter
                };
            }
            else
            {
                item = new Criterion
                {
                    Order = counter
                };
            }

            GetStartingPos(out int x, out int y);

            //Debug.WriteLine($"spawned number {counter} at {x}|{y}");

            var node = new Node(counter.ToString(), NodeType.Null, "blabla", item)
            {
                Position = new PointF(x, y),
                Size = new SizeF(NodeSizeX, NodeSizeY),
                ID = item.GetType().Name + counter
            };

            counter++;
            return node;
        }

        private void DissectCharacter(CharacterStory story)
        {
            if (story is not null)
            {
                //get all relevant items from the json
                StoryNodeExtractor.GetItems(story);
                StoryNodeExtractor.GetValues(story);
                StoryNodeExtractor.GetPersonality(story);
                StoryNodeExtractor.GetDialogues(story);
                StoryNodeExtractor.GetGlobalGoodByeResponses(story);
                StoryNodeExtractor.GetGlobalResponses(story);
                StoryNodeExtractor.GetBackGroundChatter(story);
                StoryNodeExtractor.GetQuests(story);
                StoryNodeExtractor.GetReactions(story);

                //clear criteria to free memory, we dont need them anyways
                //cant be called recusrively so we cant add it, it would break the combination

                var newList = Main.nodes.KeyNodes().ToList();
                FileName = story.CharacterName;
                for (int i = 0; i < newList.Count; i++)
                {
                    newList[i].FileName = story.CharacterName ?? string.Empty;
                }
            }
        }

        private void DoAllChilds(int zeroRow, int layerX, List<Node> childs)
        {
            foreach (var currentChild in childs)
            {
                if (visited.Contains(currentChild))
                {
                    continue;
                }

                currentChild.Position = new PointF((layerX + 1) * scaleX, layerYperX[layerX] * scaleY);
                //Debug.WriteLine("on node " + currentChild.control.Text + " we arrived at y level" + layerYperX[layerX]);
                visited.Add(currentChild);

                var newChilds = Main.nodes.Childs(currentChild);
                if (newChilds.Count == 0)
                {
                    layerYperX[layerX]++;
                    continue;
                }
                else
                {
                    if (layerYperX.Count <= layerX + 1)
                    {
                        layerYperX.Add(layerYperX[layerX]);
                    }
                    else
                    {
                        layerYperX[layerX + 1] = Math.Max(layerYperX[layerX], layerYperX[layerX + 1]);
                    }
                    DoAllChilds(zeroRow, layerX + 1, newChilds);
                    layerYperX[layerX]++;
                    layerYperX[zeroRow] = Math.Max(layerYperX[zeroRow], layerYperX[layerX]);
                }
            }
        }

        private void DrawEdge(PaintEventArgs e, Node parent, Node child, Pen pen)
        {
            start = parent.Position + new SizeF(parent.Size.Width, parent.Size.Height / 2);
            end = child.Position + new SizeF(0, child.Size.Height / 2);

            PointF controlStart;
            PointF controlEnd;
            float controlEndY, controlStartY;

            float distanceX = MathF.Abs(end.X - start.X);
            if (start.X < end.X)
            {
                controlStart = new PointF((distanceX / 2) + start.X, start.Y);
                controlEnd = new PointF(((end.X - start.X) / 2) + start.X, end.Y);
            }
            else
            {
                float distanceY = MathF.Abs(end.Y - start.Y);
                if (start.Y > end.Y)
                {
                    controlStartY = start.Y - distanceY / 2;
                    controlEndY = end.Y + distanceY / 2;
                }
                else
                {
                    controlStartY = start.Y + distanceY / 2;
                    controlEndY = end.Y - distanceY / 2;
                }
                controlStart = new PointF((start.X + distanceX / 2), controlStartY);
                controlEnd = new PointF((end.X - distanceX / 2), controlEndY);
            }

            e.Graphics.DrawBezier(pen, start, controlStart, controlEnd, end);
            //e.Graphics.DrawEllipse(Pens.Green, new Rectangle(controlStart, new Size(4, 4)));
            //e.Graphics.DrawEllipse(Pens.Red, new Rectangle(controlEnd, new Size(4, 4)));
        }

        private void DrawNode(PaintEventArgs e, Node node, SolidBrush brush)
        {
            e.Graphics.FillEllipse(brush, new RectangleF(node.Position, node.Size));
            if (Scaling > 0.2f)
            {
                int length = 32;
                var scaledRect = GetScaledRect(e.Graphics, node.RectangleNonF, Scaling);
                while (TextRenderer.MeasureText(node.Text[..Math.Min(node.Text.Length, length)], scaledFont).Width > scaledRect.Size.Width)
                {
                    length--;
                }

                Color textColor = Color.White;
                if ((brush.Color.R * 0.299 + brush.Color.G * 0.587 + brush.Color.B * 0.114) > 186)
                {
                    textColor = Color.Black;
                }

                TextRenderer.DrawText(e.Graphics,
                                      node.Text[..Math.Min(node.Text.Length, length)],
                                      scaledFont,
                                      scaledRect,
                                      textColor,
                                      TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        private void EndPan()
        {
            //end of pan
            if (CurrentlyInPan)
            {
                CurrentlyInPan = false;
                Cursor = priorCursor;
            }
        }

        private SolidBrush GetNodeColor(NodeType type)
        {
            switch (type)
            {
                case NodeType.Null:
                    return defaultNodeBrush;
                case NodeType.CharacterGroup:
                    return characterGroupNodeBrush;
                case NodeType.Criterion:
                    return criterionNodeBrush;
                case NodeType.ItemAction:
                    return itemActionNodeBrush;
                case NodeType.ItemGroupBehaviour:
                    return itemGroupBehaviourNodeBrush;
                case NodeType.ItemGroupInteraction:
                    return itemGroupInteractionNodeBrush;
                case NodeType.Pose:
                    return poseNodeBrush;
                case NodeType.Achievement:
                    return achievementNodeBrush;
                case NodeType.BGC:
                    return bgcNodeBrush;
                case NodeType.BGCResponse:
                    return bgcResponseNodeBrush;
                case NodeType.Clothing:
                    return clothingNodeBrush;
                case NodeType.CriteriaGroup:
                    return criteriaGroupNodeBrush;
                case NodeType.Cutscene:
                    return cutsceneNodeBrush;
                case NodeType.Dialogue:
                    return dialogueNodeBrush;
                case NodeType.AlternateText:
                    return alternateTextNodeBrush;
                case NodeType.Door:
                    return doorNodeBrush;
                case NodeType.Event:
                    return eventNodeBrush;
                case NodeType.EventTrigger:
                    return eventTriggerNodeBrush;
                case NodeType.Inventory:
                    return inventoryNodeBrush;
                case NodeType.Item:
                    return itemNodeBrush;
                case NodeType.ItemGroup:
                    return itemGroupNodeBrush;
                case NodeType.Personality:
                    return personalityNodeBrush;
                case NodeType.Property:
                    return propertyNodeBrush;
                case NodeType.Quest:
                    return questNodeBrush;
                case NodeType.Response:
                    return responseNodeBrush;
                case NodeType.Social:
                    return socialNodeBrush;
                case NodeType.State:
                    return stateNodeBrush;
                case NodeType.Value:
                    return valueNodeBrush;
                default:
                    return defaultNodeBrush;
            }
        }

        private void GetStartingPos(out int x, out int y)
        {
            int xstep = 100;
            int ystep = 50;
            //~sidelength of the most square layout we can achieve witrh the number of Main.nodes we have
            int sideLength = (int)(Math.Sqrt(NodeCount) + 0.5);
            //modulo of running total with sidelength gives x coords, repeating after sidelength
            //offset by halfe sidelength to center x
            x = runningTotal % sideLength - sideLength / 2;
            x += 17;
            x *= xstep;
            //running total divided by sidelength gives y coords,
            //increments after runningtotal increments sidelength times
            //offset by halfe sidelength to center y
            y = runningTotal / sideLength - sideLength / 2;
            y += 17;
            y *= ystep;
            //set position
            //increase running total
            runningTotal++;
        }

        private void InterlinkNodes()
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
            try
            {
                int count = Main.nodes.Count;
                var newList = Main.nodes.KeyNodes().ToList();
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
                                    Main.nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var clothing = new Node(criterion.Option + criterion.Value, NodeType.Clothing, criterion.Character + "'s  " + ((Clothes)int.Parse(criterion.Value!)).ToString() + " in set " + (criterion.Option == 0 ? "any" : (criterion.Option - 1).ToString())) { FileName = criterion.Character! };
                                    Clothing.Add(clothing);
                                    Main.nodes.AddParent(newList[i], clothing);
                                }
                                break;
                            }
                            case CompareTypes.CompareValues:
                            {
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    Main.nodes.AddParent(newList[i], result);
                                }
                                else
                                {
                                    CompareValuesToCheckAgain.Add(newList[i]);
                                }
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == criterion.Key2);
                                if (result is not null)
                                {
                                    Main.nodes.AddParent(newList[i], result);
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
                                    Main.nodes.AddParent(newList[i], result);
                                    break;
                                }
                                break;
                            }
                            case CompareTypes.CutScene:
                            {
                                result = Values.Find((n) => n.Type == NodeType.Cutscene && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    Main.nodes.AddParent(newList[i], result);
                                }
                                else
                                {
                                    //add cutscene
                                    var item = new Node(criterion.Key!, NodeType.Cutscene, criterion.Key!);
                                    newList.Add(item);
                                    Main.nodes.AddParent(newList[i], item);
                                }
                                break;
                            }
                            case CompareTypes.Dialogue:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Dialogue && n.FileName == criterion.Character && n.ID == criterion.Value);
                                if (result is not null)
                                {
                                    //dialogue influences this criteria
                                    Main.nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add new personality, should be from someone else
                                    var item = new Node(criterion.Value!, NodeType.Dialogue, criterion.Character + " dialoge " + criterion.Value) { FileName = criterion.Character! };
                                    newList.Add(item);
                                    Main.nodes.AddParent(newList[i], item);
                                }
                                break;
                            }
                            case CompareTypes.Door:
                            {
                                result = Doors.Find((n) => n.Type == NodeType.Door && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    Main.nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var door = new Node(criterion.Key!, NodeType.Door, criterion.Key!);
                                    Doors.Add(door);
                                    Main.nodes.AddParent(newList[i], door);
                                }
                                break;
                            }
                            case CompareTypes.Item:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Item && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    Main.nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var item = new Node(criterion.Key!, NodeType.Item, criterion.Key!);
                                    newList.Add(item);
                                    Main.nodes.AddParent(newList[i], item);
                                }
                                break;
                            }
                            case CompareTypes.IsCurrentlyBeingUsed:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Item && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    Main.nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var item = new Node(criterion.Key!, NodeType.Item, criterion.Key!);
                                    newList.Add(item);
                                    Main.nodes.AddParent(newList[i], item);
                                }
                                break;
                            }
                            case CompareTypes.IsCurrentlyUsing:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Item && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    Main.nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var item = new Node(criterion.Key!, NodeType.Item, criterion.Key!);
                                    newList.Add(item);
                                    Main.nodes.AddParent(newList[i], item);
                                }
                                break;
                            }
                            case CompareTypes.ItemFromItemGroup:
                            {
                                result = newList.Find((n) => n.Type == NodeType.ItemGroup && n.Text == criterion.Key);
                                if (result is not null)
                                {
                                    Main.nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var item = new Node(criterion.Key!, NodeType.Item, criterion.Key!);
                                    newList.Add(item);
                                    Main.nodes.AddParent(newList[i], item);
                                }
                                break;
                            }
                            case CompareTypes.Personality:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Personality && n.FileName == criterion.Character && n.ID == ((PersonalityTraits)int.Parse(criterion.Key!)).ToString());
                                if (result is not null)
                                {
                                    Main.nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add new personality, should be from someone else
                                    var item = new Node(((PersonalityTraits)int.Parse(criterion.Key!)).ToString(), NodeType.Personality, criterion.Character + "'s Personality " + ((PersonalityTraits)int.Parse(criterion.Key!)).ToString()) { FileName = criterion.Character! };
                                    newList.Add(item);
                                    Main.nodes.AddParent(newList[i], item);
                                }
                                break;
                            }
                            case CompareTypes.PlayerInventory:
                            {
                                //find/add inventory item
                                result = InventoryItems.Find((n) => n.Type == NodeType.Inventory && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    Main.nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var item = new Node(criterion.Key!, NodeType.Inventory, "Items: " + criterion.Key);
                                    InventoryItems.Add(item);
                                    Main.nodes.AddParent(newList[i], item);
                                }
                                //find normal item if it exists
                                result = newList.Find((n) => n.Type == NodeType.Item && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    Main.nodes.AddChild(newList[i], result);
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
                                    Main.nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add pose node, hasnt been referenced yet
                                    var pose = new Node(criterion.Value!, NodeType.Pose, "Pose number " + criterion.Value);
                                    Poses.Add(pose);
                                    Main.nodes.AddParent(newList[i], pose);
                                }
                                break;
                            }
                            case CompareTypes.Property:
                            {
                                result = Properties.Find((n) => n.Type == NodeType.Property && n.ID == criterion.Character + "Property" + criterion.Value);
                                if (result is not null)
                                {
                                    Main.nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add property node, hasnt been referenced yet
                                    var property = new Node(criterion.Character + "Property" + criterion.Value, NodeType.Property, criterion.Character + ((InteractiveProperties)int.Parse(criterion.Value!)).ToString()) { FileName = criterion.Character! };
                                    Properties.Add(property);
                                    Main.nodes.AddParent(newList[i], property);
                                }
                                break;
                            }
                            case CompareTypes.Quest:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Quest && n.ID == criterion.Key);
                                if (result is not null)
                                {
                                    Main.nodes.AddParent(newList[i], result);
                                    break;
                                }
                                break;
                            }
                            case CompareTypes.Social:
                            {
                                result = Socials.Find((n) => n.Type == NodeType.Social && n.ID == criterion.Character + criterion.SocialStatus + criterion.Character2);
                                if (result is not null)
                                {
                                    Main.nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add property node, hasnt been referenced yet
                                    var social = new Node(criterion.Character + criterion.SocialStatus + criterion.Character2, NodeType.Social, criterion.Character + " " + criterion.SocialStatus + " " + criterion.Character2) { FileName = criterion.Character! };
                                    Socials.Add(social);
                                    Main.nodes.AddParent(newList[i], social);
                                }
                                break;
                            }
                            case CompareTypes.State:
                            {
                                result = States.Find((n) => n.Type == NodeType.State && n.FileName == criterion.Character && n.Text.AsSpan()[..2].Contains(criterion.Value!.AsSpan(), StringComparison.InvariantCulture));
                                if (result is not null)
                                {
                                    Main.nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add state node, hasnt been referenced yet
                                    var state = new Node(criterion.Character + "State" + criterion.Value, NodeType.State, criterion.Value + "|" + ((InteractiveStates)int.Parse(criterion.Value!)).ToString()) { FileName = criterion.Character! };
                                    States.Add(state);
                                    Main.nodes.AddParent(newList[i], state);
                                }
                                break;
                            }
                            case CompareTypes.Value:
                            {
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == criterion.Key && FileName == criterion.Character);
                                if (result is not null)
                                {
                                    if (!result.Text.Contains(GetSymbolsFromValueFormula(criterion.ValueFormula ?? ValueSpecificFormulas.EqualsValue) + criterion.Value))
                                    {
                                        result.Text += GetSymbolsFromValueFormula(criterion.ValueFormula ?? ValueSpecificFormulas.EqualsValue) + criterion.Value + ", ";
                                    }

                                    Main.nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var value = new Node(criterion.Key!, NodeType.Value, criterion.Character + " value " + criterion.Key + ", referenced values: " + GetSymbolsFromValueFormula(criterion.ValueFormula ?? ValueSpecificFormulas.EqualsValue) + criterion.Value + ", ") { FileName = criterion.Character ?? string.Empty };
                                    Values.Add(value);
                                    Main.nodes.AddParent(newList[i], value);
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
                                    Main.nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var clothing = new Node(gameEvent.Option + gameEvent.Value, NodeType.Clothing, gameEvent.Character + "'s  " + ((Clothes)int.Parse(gameEvent.Value!)).ToString() + " in set " + (gameEvent.Option == 0 ? "any" : (gameEvent.Option - 1).ToString())) { FileName = gameEvent.Character! };
                                    Clothing.Add(clothing);
                                    Main.nodes.AddChild(newList[i], clothing);
                                }
                                newList[i].Text = gameEvent.Character + " " + ((Clothes)int.Parse(gameEvent.Value!)).ToString() + " in set " + (gameEvent.Option == 0 ? "any" : (gameEvent.Option - 1).ToString()) + " " + (gameEvent.Option2 == 0 ? "Change" : "Assign default set") + " " + (gameEvent.Option3 == 0 ? "On" : "Off");
                                break;
                            }
                            case GameEvents.CombineValue:
                            {
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Key && FileName == gameEvent.Character);
                                if (result is not null)
                                {
                                    Main.nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var value = new Node(gameEvent.Key!, NodeType.Value, gameEvent.Character + " value " + gameEvent.Key) { FileName = gameEvent.Character ?? string.Empty };
                                    Values.Add(value);
                                    Main.nodes.AddChild(newList[i], value);
                                }
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Value && FileName == gameEvent.Character2);
                                if (result is not null)
                                {
                                    Main.nodes.AddParent(newList[i], result);
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var value = new Node(gameEvent.Value!, NodeType.Value, gameEvent.Character2 + " value " + gameEvent.Value) { FileName = gameEvent.Character2 ?? string.Empty };
                                    Values.Add(value);
                                    Main.nodes.AddParent(newList[i], value);
                                }
                                newList[i].Text = "Add " + gameEvent.Character + ":" + gameEvent.Key + " to " + gameEvent.Character2 + ":" + gameEvent.Value;
                                break;
                            }
                            case GameEvents.CutScene:
                            {
                                result = Values.Find((n) => n.Type == NodeType.Cutscene && n.ID == gameEvent.Key);
                                if (result is not null)
                                {
                                    Main.nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //add cutscene
                                    var item = new Node(gameEvent.Key!, NodeType.Cutscene, gameEvent.Key!);
                                    newList.Add(item);
                                    Main.nodes.AddChild(newList[i], item);
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
                                    Main.nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add new personality, should be from someone else
                                    var item = new Node(gameEvent.Value!, NodeType.Dialogue, gameEvent.Character + " dialoge " + gameEvent.Value) { FileName = gameEvent.Character! };
                                    newList.Add(item);
                                    Main.nodes.AddChild(newList[i], item);
                                }
                                newList[i].Text = ((DialogueAction)gameEvent.Option).ToString() + " " + gameEvent.Character + "'s Dialogue " + gameEvent.Value;
                                break;
                            }
                            case GameEvents.Door:
                            {
                                result = Doors.Find((n) => n.Type == NodeType.Door && n.ID == gameEvent.Key);
                                if (result is not null)
                                {
                                    Main.nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var door = new Node(gameEvent.Key!, NodeType.Door, gameEvent.Key!);
                                    Doors.Add(door);
                                    Main.nodes.AddChild(newList[i], door);
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
                                        Main.nodes.AddChild(newList[i], result);
                                    }
                                }
                                else
                                {
                                    //create and add event, hasnt been referenced yet, we can not know its id if it doesnt already exist
                                    var _event = new Node("NA-" + gameEvent.Value, NodeType.Event, gameEvent.Value!);
                                    newList.Add(_event);
                                    Main.nodes.AddChild(newList[i], _event);
                                }
                                newList[i].Text = gameEvent.Character + (gameEvent.Option == 0 ? " Perform Event " : " Set Enabled ") + (gameEvent.Option2 == 0 ? "(False) " : "(True) ") + gameEvent.Value;
                                break;
                            }
                            case GameEvents.Item:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Item && n.ID == gameEvent.Key);
                                if (result is not null)
                                {
                                    Main.nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var item = new Node(gameEvent.Key!, NodeType.Item, gameEvent.Key!);
                                    newList.Add(item);
                                    Main.nodes.AddChild(newList[i], item);
                                }
                                newList[i].Text = gameEvent.Key!.ToString() + " " + ((ItemEventAction)gameEvent.Option).ToString() + " (" + gameEvent.Value + ") " + " (" + (gameEvent.Option2 == 1 ? "True" : "False") + ") ";
                                break;
                            }
                            case GameEvents.ItemFromItemGroup:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Item && n.ID == gameEvent.Key);
                                if (result is not null)
                                {
                                    Main.nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var item = new Node(gameEvent.Key!, NodeType.Item, gameEvent.Key!);
                                    newList.Add(item);
                                    Main.nodes.AddChild(newList[i], item);
                                }
                                newList[i].Text = gameEvent.Key!.ToString() + " " + ((ItemGroupAction)gameEvent.Option).ToString() + " (" + gameEvent.Value + ") " + " (" + (gameEvent.Option2 == 1 ? "True" : "False") + ") ";
                                break;
                            }
                            case GameEvents.Personality:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Personality && n.FileName == gameEvent.Character && n.ID == ((PersonalityTraits)gameEvent.Option).ToString());
                                if (result is not null)
                                {
                                    Main.nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add new personality, should be from someone else
                                    var item = new Node(((PersonalityTraits)gameEvent.Option).ToString(), NodeType.Personality, gameEvent.Character + "'s Personality " + ((PersonalityTraits)gameEvent.Option).ToString()) { FileName = gameEvent.Character! };
                                    newList.Add(item);
                                    Main.nodes.AddChild(newList[i], item);
                                }
                                newList[i].Text = gameEvent.Character + " " + ((PersonalityTraits)gameEvent.Option).ToString() + " " + ((PersonalityAction)gameEvent.Option2).ToString() + " " + gameEvent.Value;
                                break;
                            }
                            case GameEvents.Property:
                            {
                                result = Properties.Find((n) => n.Type == NodeType.Property && n.ID == gameEvent.Character + "Property" + gameEvent.Value);
                                if (result is not null)
                                {
                                    Main.nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add property node, hasnt been referenced yet
                                    var property = new Node(gameEvent.Character + "Property" + gameEvent.Value, NodeType.Property, gameEvent.Character + Enum.Parse<InteractiveProperties>(gameEvent.Value!).ToString()) { FileName = gameEvent.Character! };
                                    Properties.Add(property);
                                    Main.nodes.AddChild(newList[i], property);
                                }
                                newList[i].Text = gameEvent.Character + " " + Enum.Parse<InteractiveProperties>(gameEvent.Value!).ToString() + " " + (gameEvent.Option2 == 1 ? "True" : "False");
                                break;
                            }
                            case GameEvents.MatchValue:
                            {
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Key && FileName == gameEvent.Character);
                                if (result is not null)
                                {
                                    Main.nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var value = new Node(gameEvent.Key!, NodeType.Value, gameEvent.Character + " value " + gameEvent.Key) { FileName = gameEvent.Character ?? string.Empty };
                                    Values.Add(value);
                                    Main.nodes.AddChild(newList[i], value);
                                }
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Value && FileName == gameEvent.Character2);
                                if (result is not null)
                                {
                                    Main.nodes.AddParent(newList[i], result);
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var value = new Node(gameEvent.Value!, NodeType.Value, gameEvent.Character2 + " value " + gameEvent.Value) { FileName = gameEvent.Character2 ?? string.Empty };
                                    Values.Add(value);
                                    Main.nodes.AddParent(newList[i], value);
                                }
                                newList[i].Text = "set " + gameEvent.Character + ":" + gameEvent.Key + " to " + gameEvent.Character2 + ":" + gameEvent.Value;
                                break;
                            }
                            case GameEvents.ModifyValue:
                            {
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Key && FileName == gameEvent.Character);
                                if (result is not null)
                                {
                                    Main.nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var value = new Node(gameEvent.Key!, NodeType.Value, gameEvent.Character + " value " + gameEvent.Key) { FileName = gameEvent.Character ?? string.Empty };
                                    Values.Add(value);
                                    Main.nodes.AddChild(newList[i], value);
                                }
                                newList[i].Text = (gameEvent.Option == 0 ? "Equals" : "Add") + gameEvent.Character + ":" + gameEvent.Key + " to " + gameEvent.Value;
                                break;
                            }
                            case GameEvents.Player:
                            {                                //find/add inventory item
                                result = InventoryItems.Find((n) => n.Type == NodeType.Inventory && n.ID == gameEvent.Value);
                                if (result is not null)
                                {
                                    Main.nodes.AddParent(newList[i], result);
                                    break;
                                }
                                else
                                {
                                    //create and add item node, hasnt been referenced yet
                                    var item = new Node(gameEvent.Value!, NodeType.Inventory, "Items: " + gameEvent.Value);
                                    InventoryItems.Add(item);
                                    Main.nodes.AddParent(newList[i], item);
                                }
                                result = newList.Find((n) => n.Type == NodeType.Item && n.ID == gameEvent.Value);
                                if (result is not null)
                                {
                                    Main.nodes.AddChild(newList[i], result);
                                }

                                newList[i].Text = ((PlayerActions)gameEvent.Option).ToString() + (gameEvent.Option == 0 ? gameEvent.Option2 == 0 ? " Add " : " Remove " : " ") + gameEvent.Value + "/" + gameEvent.Character;
                                break;
                            }
                            case GameEvents.Pose:
                            {
                                result = Poses.Find((n) => n.Type == NodeType.Pose && n.ID == gameEvent.Value);
                                if (result is not null)
                                {
                                    Main.nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add pose node, hasnt been referenced yet
                                    var pose = new Node(gameEvent.Value!, NodeType.Pose, "Pose number " + gameEvent.Value);
                                    Poses.Add(pose);
                                    Main.nodes.AddChild(newList[i], pose);
                                }
                                newList[i].Text = "Set " + gameEvent.Character + " Pose no. " + gameEvent.Value + " " + (gameEvent.Option == 0 ? " False" : " True");
                                break;
                            }
                            case GameEvents.Quest:
                            {
                                result = newList.Find((n) => n.Type == NodeType.Quest && n.ID == gameEvent.Key);
                                if (result is not null)
                                {
                                    Main.nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add property node, hasnt been referenced yet
                                    var quest = new Node(gameEvent.Value!, NodeType.Social, gameEvent.Character + "'s quest " + gameEvent.Value + ", not found in loaded story files") { FileName = gameEvent.Character! };
                                    newList.Add(quest);
                                    Main.nodes.AddChild(newList[i], quest);
                                }
                                newList[i].Text = ((QuestActions)gameEvent.Option).ToString() + " the quest " + gameEvent.Value + " from " + gameEvent.Character;
                                break;
                            }
                            case GameEvents.RandomizeIntValue:
                            {
                                result = Values.Find((n) => n.Type == NodeType.Value && n.ID == gameEvent.Key && FileName == gameEvent.Character);
                                if (result is not null)
                                {
                                    Main.nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add value node, hasnt been referenced yet
                                    var value = new Node(gameEvent.Key!, NodeType.Value, gameEvent.Character + " value " + gameEvent.Key) { FileName = gameEvent.Character ?? string.Empty };
                                    Values.Add(value);
                                    Main.nodes.AddChild(newList[i], value);
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
                                    Main.nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add property node, hasnt been referenced yet
                                    var social = new Node(gameEvent.Character + ((SocialStatuses)gameEvent.Option).ToString() + gameEvent.Character2, NodeType.Social, gameEvent.Character + " " + ((SocialStatuses)gameEvent.Option).ToString() + " " + gameEvent.Character2) { FileName = gameEvent.Character! };
                                    Socials.Add(social);
                                    Main.nodes.AddChild(newList[i], social);
                                }
                                newList[i].Text = gameEvent.Character + " " + ((SocialStatuses)gameEvent.Option).ToString() + " " + gameEvent.Character2 + (gameEvent.Option2 == 0 ? " Equals " : " Add ") + gameEvent.Value;
                                break;
                            }
                            case GameEvents.State:
                            {
                                result = States.Find((n) => n.Type == NodeType.State && n.FileName == gameEvent.Character && n.Text.AsSpan()[..2].Contains(gameEvent.Value!.AsSpan(), StringComparison.InvariantCulture));
                                if (result is not null)
                                {
                                    Main.nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add state node, hasnt been referenced yet
                                    var state = new Node(gameEvent.Character + "State" + gameEvent.Value, NodeType.State, gameEvent.Value + "|" + ((InteractiveStates)int.Parse(gameEvent.Value!)).ToString()) { FileName = gameEvent.Character! };
                                    States.Add(state);
                                    Main.nodes.AddChild(newList[i], state);
                                }
                                newList[i].Text = (gameEvent.Option == 0 ? "Add " : "Remove ") + gameEvent.Character + " State " + ((InteractiveStates)int.Parse(gameEvent.Value!)).ToString();
                                break;
                            }
                            case GameEvents.TriggerBGC:
                            {
                                result = newList.Find((n) => n.Type == NodeType.BGC && n.ID == "BGC" + gameEvent.Value);
                                if (result is not null)
                                {
                                    Main.nodes.AddChild(newList[i], result);
                                }
                                else
                                {
                                    //create and add property node, hasnt been referenced yet
                                    var bgc = new Node("BGC" + gameEvent.Value, NodeType.BGC, gameEvent.Character + "'s BGC " + gameEvent.Value + ", not found in loaded story files") { FileName = gameEvent.Character! };
                                    newList.Add(bgc);
                                    Main.nodes.AddChild(newList[i], bgc);
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
                                Main.nodes.AddChild(newList[i], result);
                            }
                            else
                            {
                                //create and add event, hasnt been referenced yet, we can not know its id if it doesnt already exist
                                var eventNode = new Node(_event.Id ?? "none", NodeType.Event, _event.Value ?? "none");
                                newList.Add(eventNode);
                                Main.nodes.AddChild(newList[i], eventNode);
                            }
                        }
                        //link against criteria
                        foreach (Criterion _criterion in trigger.Critera!)
                        {
                            result = newList.Find((n) => n.Type == NodeType.Criterion && n.ID == $"{_criterion.Character}{_criterion.CompareType}{_criterion.Value}");
                            if (result is not null)
                            {
                                Main.nodes.AddParent(newList[i], result);
                            }
                            else
                            {
                                newList.Add(Node.CreateCriteriaNode(_criterion, newList[i]));
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
                            Main.nodes.AddChild(newList[i], result);
                        }
                        else
                        {
                            //create and add event, hasnt been referenced yet, we can not know its id if it doesnt already exist
                            var dialogue = new Node(response.Next.ToString(), NodeType.Dialogue, $"dialogue number {response.Next} for {newList[i].FileName}");
                            newList.Add(dialogue);
                            Main.nodes.AddChild(newList[i], dialogue);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            //check some comparevalue Main.nodes again because the referenced values havent been added yet
            RecheckCompareValues(CompareValuesToCheckAgain);

            //merge doors with items if applicable
            MergeDoors();
        }

        private void Main_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.ToLowQuality();

            //update canvas transforms
            g.TranslateTransform(-OffsetX * Scaling, -OffsetY * Scaling);
            g.ScaleTransform(Scaling, Scaling);
            RectangleF adjustedVisibleClipBounds = new(OffsetX - NodeSizeX, OffsetY - NodeSizeY, g.VisibleClipBounds.Width + NodeSizeX, g.VisibleClipBounds.Height + NodeSizeY);

            if (Scaling < 0)
            {
                Debugger.Break();
            }
            scaledFont = GetScaledFont(g, DefaultFont, Scaling);
            

            //int c = 0;
            foreach (var node in NodePositionSorting.Singleton[adjustedVisibleClipBounds])
            {
                //c++;
                DrawNode(e, node, GetNodeColor(node.Type));
            }
            //todo we need to cull here as well somehow
            foreach (var node in Main.nodes.KeyNodes())
            {
                var list = Main.nodes.Childs(node);
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        DrawEdge(e, node, item, linePen);
                    }
                }
            }

            if (highlightNode != Node.NullNode)
            {
                var family = Main.nodes[highlightNode];
                if (family.Childs.Count > 0)
                {
                    foreach (var item in family.Childs)
                    {
                        DrawEdge(e, highlightNode, item, highlightPen);
                    }
                }
                if (family.Parents.Count > 0)
                {
                    foreach (var item in family.Parents)
                    {
                        DrawEdge(e, item, highlightNode, highlightPen);
                    }
                }
                DrawNode(e, highlightNode, HighlightNodeBrush);
            }
        }

        private void MergeDoors()
        {
            Node? result;
            var newlist = Main.nodes.KeyNodes().ToList();
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

        private void OpenButton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();

            if (result != DialogResult.OK)
            {
                return;
            }

            string FilePath = dialog.FileName;

            if (File.Exists(FilePath))
            {
                //read in Main.nodes and set positions later, is faster than the old system
                string fileString = File.ReadAllText(FilePath);
                fileString = fileString.Replace('', ' ');
                //else create new
                try
                {
                    if (Path.GetExtension(FilePath) == ".story")
                    {
                        DissectStory(JsonConvert.DeserializeObject<MainStory>(fileString) ?? new MainStory());
                    }
                    else
                    {
                        DissectCharacter(JsonConvert.DeserializeObject<CharacterStory>(fileString) ?? new CharacterStory());
                    }
                }
                catch (JsonReaderException ex)
                {
                    Debug.WriteLine(ex.Message);
                    return;
                }
                catch (AggregateException ex)
                {
                    Debug.WriteLine(ex.Message);
                    return;
                }

                //even link for single file, should be able to link most suff so it stays readable
                InterlinkNodes();

                SetupStartPositions();

                Invalidate();
            }
        }

        private void RecheckCompareValues(List<Node> CompareValuesToCheckAgain)
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

        private void ResetButton_Click(object sender, EventArgs e)
        {
            nodes.Clear();
            visited.Clear();
            Invalidate();
            counter = 0;
        }

        private void SetPanOffset(Point location)
        {
            StartPanOffsetX = location.X;
            StartPanOffsetY = location.Y;
        }

        private void SetupStartPositions()
        {
            layerYperX.Clear();
            layerYperX.Add(0);
            visited.Clear();

            int sideLengthY = (int)(Math.Sqrt(Main.nodes.KeyNodes().Count) + 0.5);
            int zeroColoumn = 0;

            foreach (var key in Main.nodes.KeyNodes())
            {
                Family family = Main.nodes[key];
                if (family.Parents.Count == 0)
                {
                    //we have a root start node with no parents, start child search and layout from here
                    key.Position = new PointF((zeroColoumn + 1) * scaleX, layerYperX[zeroColoumn] * scaleY);
                    visited.Add(key);

                    if (family.Childs.Count == 0)
                    {
                        layerYperX[zeroColoumn]++;
                        if (layerYperX[zeroColoumn] > sideLengthY)
                        {
                            zeroColoumn = layerYperX.Count;
                            layerYperX.Add(0);
                            layerYperX.Add(0);
                        }
                        continue;
                    }
                    if (layerYperX.Count <= 1)
                    {
                        layerYperX.Add(layerYperX[zeroColoumn]);
                    }
                    else
                    {
                        layerYperX[zeroColoumn + 1] = Math.Max(layerYperX[zeroColoumn], layerYperX[zeroColoumn + 1]);
                    }
                    //Debug.WriteLine("on node " + key.control.Text + " we went to its children at y level " + layerYperX[0]);
                    DoAllChilds(zeroColoumn, zeroColoumn + 1, family.Childs);
                    layerYperX[zeroColoumn]++;

                }
            }
        }

        private void Start_Click(object sender, EventArgs e)
        {
            Node node = CreateNode(typeof(Criterion));

            Main.nodes.Add(node);

            lastNode = node;
            for (int i = 0; i < NodeCount; i++)
            {
                switch (Random.Shared.Next(20))
                {
                    case 9:
                    case 8:
                    case 7:
                    case 6:
                    case 5:
                    case 4:
                    case 19:
                    case 18:
                    case 17:
                    case 16:
                    case 15:
                    case 14:
                    case 13:
                    case 12:
                    case 11:
                    {
                        node = CreateNode(typeof(Response));
                        Main.nodes.AddChild(lastNode, node);
                        break;
                    }
                    case 3:
                    {
                        node = CreateNode(typeof(Criterion));
                        Main.nodes.Add(node);
                        break;
                    }
                    case 10:
                    case 2:
                    case 1:
                    case 0:
                    {
                        node = CreateNode(typeof(Dialogue));
                        Main.nodes.AddParent(lastNode, node);
                        break;
                    }
                    default:
                        break;
                }

                if (Random.Shared.Next(5) < 3)
                {
                    lastNode = node;
                }
            }

            //position according to the child system
            SetupStartPositions();

            Invalidate();
        }

        private void UpdateClickedNode(float ScreenPosX, float ScreenPosY, Point ScreenPos)
        {
            Node clickedNode = GetNodeAtPoint(ScreenPos);

            if (clickedNode == Node.NullNode)
            {
                return;
            }

            if (IsCtrlPressed)
            {
                movedNode = clickedNode;
                MovingChild = !MovingChild;
                if (MovingChild)
                {
                    OffsetFromDragClick = new Size((int)(movedNode.Position.X - ScreenPosX), (int)(movedNode.Position.Y - ScreenPosY));
                }
            }
            else
            {
                Details.SelectedObject = clickedNode.Data;
                lastNode = clickedNode;
            }
        }

        private void UpdateHighlightNode(Point ScreenPos)
        {
            var oldHighlight = highlightNode;
            highlightNode = GetNodeAtPoint(ScreenPos);
            if (highlightNode != oldHighlight)
            {
                Invalidate();
            }
        }

        private void UpdatePan(Point mouseLocation)
        {
            //start of pan
            if (!CurrentlyInPan)
            {
                CurrentlyInPan = true;
                //get current position in screen coordinates when we start to pan
                SetPanOffset(mouseLocation);
                priorCursor = Cursor;
                Cursor = Cursors.Cross;
            }
            //in pan
            else if (CurrentlyInPan)
            {
                UpdatePanOffset(mouseLocation);
                //redraw
                Invalidate();
            }
        }

        private void UpdatePanOffset(Point location)
        {
            //better scaling thanks to the one lone coder
            //https://www.youtube.com/watch?v=ZQ8qtAizis4
            //update opffset by the difference in screen coordinates we travelled so far
            OffsetX -= (location.X - StartPanOffsetX) / Scaling;
            OffsetY -= (location.Y - StartPanOffsetY) / Scaling;

            //replace old start by new start coordinates so we only look at one interval,
            //and do not accumulate the change in position
            StartPanOffsetX = location.X;
            StartPanOffsetY = location.Y;
        }

        private void UpdateScaling(MouseEventArgs e)
        {
            //get last mouse position in world space before the zoom so we can
            //offset back by the distance in world space we got shifted by zooming
            ScreenToGraph(e.X, e.Y, out BeforeZoomMouseX, out BeforeZoomMouseY);

            //WHEEL_DELTA = 120, as per windows documentation
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.mouseeventargs.delta?view=windowsdesktop-6.0
            if (e.Delta > 0)
            {
                Scaling *= 1.2f;
            }
            else if (e.Delta < 0)
            {
                Scaling *= 0.8f;
            }

            //capture mouse coordinates in world space again so we can calculate the offset cause by zooming and compensate
            ScreenToGraph(e.X, e.Y, out AfterZoomMouseX, out AfterZoomMouseY);

            //update pan offset by the distance caused by zooming
            OffsetX += BeforeZoomMouseX - AfterZoomMouseX;
            OffsetY += BeforeZoomMouseY - AfterZoomMouseY;
        }

        //see https://stackoverflow.com/questions/8850528/how-to-apply-graphics-scale-and-translate-to-the-textrenderer
        private static Font GetScaledFont(Graphics g, Font f, float scale)
        {
            if(f.SizeInPoints * scale < 0)
            {
                Debugger.Break();
            }
            if (f.SizeInPoints * scale > 2000)
            {
                Debugger.Break();
            }

            return new Font(f.FontFamily,
                            f.SizeInPoints * scale,
                            f.Style,
                            GraphicsUnit.Point,
                            f.GdiCharSet,
                            f.GdiVerticalFont);
        }

        private static Rectangle GetScaledRect(Graphics g, Rectangle r, float scale)
        {
            return new Rectangle((int)Math.Ceiling(r.X * scale),
                                (int)Math.Ceiling(r.Y * scale),
                                (int)Math.Ceiling(r.Width * scale),
                                (int)Math.Ceiling(r.Height * scale));
        }
    }
}
