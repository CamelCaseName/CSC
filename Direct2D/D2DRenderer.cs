using CSC.Glue;
using CSC.Nodestuff;
using Silk.NET.Core.Native;
using Silk.NET.Direct2D;
using Silk.NET.DXGI;
using Silk.NET.Maths;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using AlphaMode = Silk.NET.Direct2D.AlphaMode;
using DashStyle = Silk.NET.Direct2D.DashStyle;
using LineJoin = Silk.NET.Direct2D.LineJoin;
using IDWriteFactory = Silk.NET.DirectWrite.IDWriteFactory;
using IDWriteTextFormat = Silk.NET.DirectWrite.IDWriteTextFormat;
using DWFactoryType = Silk.NET.DirectWrite.FactoryType;
using Rectangle = System.Drawing.Rectangle;
using DWrite = Silk.NET.DirectWrite.DWrite;
using DWExtensions = Silk.NET.DirectWrite.DWriteFactoryVtblExtensions;

namespace CSC.Direct2D
{
    internal unsafe class D2DRenderer
    {
        //misc
        private const TextFormatFlags TextFlags = TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.LeftAndRightPadding;
        private readonly SizeF CircleSize = new(15, 15);
        private ComPtr<ID2D1SolidColorBrush> achievementNodeBrush;
        private RectangleF adjustedVisibleClipBounds = new();
        private ComPtr<ID2D1SolidColorBrush> alternateTextNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> bgcNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> bgcResponseNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> characterGroupNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> circlePen;
        private ComPtr<ID2D1SolidColorBrush> clickedLinePen;
        private ComPtr<ID2D1SolidColorBrush> ClickedNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> clothingNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> criteriaGroupNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> criterionNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> cutsceneNodeBrush;

        //direct2d stuffs
        private D2D d2d = null!;
        private ComPtr<ID2D1SolidColorBrush> darkachievementNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkalternateTextNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkbgcNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkbgcResponseNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkcharacterGroupNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkclothingNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkcriteriaGroupNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkcriterionNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkcutsceneNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkdefaultNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkdialogueNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkdoorNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkeventNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkeventTriggerNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkinventoryNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkitemActionNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkitemGroupBehaviourNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkitemGroupInteractionNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkitemGroupNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkitemNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkpersonalityNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkposeNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkpropertyNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkquestNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkresponseNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darksocialNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkstateNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> darkvalueNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> defaultNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> dialogueNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> doorNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> eventNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> eventTriggerNodeBrush;
        private ComPtr<ID2D1Factory> factory;
        private ComPtr<IDWriteFactory> dwfactory;
        private ComPtr<ID2D1SolidColorBrush> HighlightNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> highlightPen;
        private ComPtr<ID2D1GradientStopCollection> interlinkedGradientstops;
        private ComPtr<ID2D1LinearGradientBrush> InterlinkedNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> inventoryNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> itemActionNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> itemGroupBehaviourNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> itemGroupInteractionNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> itemGroupNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> itemNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> linePen;
        private ComPtr<ID2D1SolidColorBrush> NodeToLinkNextBrush;
        private ComPtr<ID2D1SolidColorBrush> personalityNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> poseNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> propertyNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> questNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> responseNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> LightTextBrush;
        private ComPtr<ID2D1SolidColorBrush> DarkTextBrush;
        private ComPtr<ID2D1SolidColorBrush> BlackTextBrush;
        private static readonly float fontSize = 40f;
        private readonly char[] fontFamilyName = [.. "Consolas"];
        private readonly char[] locale = [.. "en-us"];
        private ComPtr<ID2D1SolidColorBrush> SelectionEdge;
        private ComPtr<ID2D1SolidColorBrush> socialNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> stateNodeBrush;
        private ComPtr<ID2D1DCRenderTarget> target = default;
        private ComPtr<ID2D1SolidColorBrush> valueNodeBrush;
        private ComPtr<ID2D1StrokeStyle> defaultStyle;
        private ComPtr<ID2D1StrokeStyle> interlinkedStyle;
        private ComPtr<IDWriteTextFormat> defaultFormat;
        private const float linePenWidth = 0.2f;
        private const float circlePenWidth = 0.8f;
        private const float clickedLinePenWidth = 3f;
        private const float highlightPenWidth = 3f;
        private const float selectionEdgeWidth = 1f;
        private float oldScale = 0.3f;

        public D2DRenderer()
        {
            GetD2DResources();
        }

        ~D2DRenderer()
        {
            ReleaseD2DResources();
        }

        public void CreateBrush(Color color, ref ComPtr<ID2D1SolidColorBrush> brush)
        {
            BrushProperties brushProperties = new(opacity: 100);
            D3Dcolorvalue colo = color.ToD3D();
            int res = target.CreateSolidColorBrush(&colo, &brushProperties, ref brush);
            if (res != 0)
            {
                Debugger.Break();
            }
        }

        public void Paint(Graphics g, NodeStore nodes, RectangleF screenclip)
        {
            unsafe
            {
                adjustedVisibleClipBounds = new(Main.Offset.X - Main.NodeSizeX,
                                                Main.Offset.Y - Main.NodeSizeY,
                                                g.VisibleClipBounds.Width + Main.NodeSizeX,
                                                g.VisibleClipBounds.Height + Main.NodeSizeY);

                //cannot access transform after weve connectd the deviceContexts...so we save it here before connecting
                Matrix3X2<float> position = g.Transform.MatrixElements.ToMatrix3X2();

                //Bind each frame...
                Box2D<int> dcRect = screenclip.ToBox();
                nint hdc = g.GetHdc();
                int res = target.BindDC(hdc, ref dcRect);
                if (res != 0)
                {
                    Debugger.Break();
                }

                target.BeginDraw();
                target.SetTransform(ref position);
                D3Dcolorvalue bc = new(40 / 255f, 40 / 255f, 40 / 255f, 1f);
                target.Clear(ref bc);

                DrawAllNodes(g, nodes);

                ulong tag1 = 0, tag2 = 0;
                target.EndDraw(ref tag1, ref tag2);
                g.ReleaseHdc(hdc);
            }
        }

        public void Release()
        {
            ReleaseD2DResources();
        }

        private void DrawAllNodes(Graphics g, NodeStore nodes)
        {
            foreach (var node in nodes.Nodes)
            {
                var list = nodes.Childs(node);
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        DrawEdge(g, node, item, linePen.AsBrush());
                    }
                }
            }

            if (Main.Selected != Node.NullNode)
            {
                var family = nodes[Main.Selected];
                if (family.Childs.Count > 0)
                {
                    foreach (var item in family.Childs)
                    {
                        DrawEdge(g, Main.Selected, item, clickedLinePen.AsBrush());
                    }
                }
                if (family.Parents.Count > 0)
                {
                    foreach (var item in family.Parents)
                    {
                        DrawEdge(g, item, Main.Selected, clickedLinePen.AsBrush());
                    }
                }

                foreach (var node in nodes.Positions[adjustedVisibleClipBounds])
                {
                    //c++;
                    bool light = family.Childs.Contains(node) || family.Parents.Contains(node) || Main.Selected == node;
                    DrawNode(g, node, GetNodeColor(node.Type, light), light);
                }
            }
            else
            {
                foreach (var node in nodes.Positions[adjustedVisibleClipBounds])
                {
                    //c++;
                    DrawNode(g, node, GetNodeColor(node.Type, false));
                }
            }

            if (Main.Highlight != Node.NullNode)
            {
                var family = nodes[Main.Highlight];
                if (family.Childs.Count > 0)
                {
                    foreach (var item in family.Childs)
                    {
                        DrawEdge(g, Main.Highlight, item, highlightPen.AsBrush());
                    }
                }
                if (family.Parents.Count > 0)
                {
                    foreach (var item in family.Parents)
                    {
                        DrawEdge(g, item, Main.Highlight, highlightPen.AsBrush());
                    }
                }
                DrawNode(g, Main.Highlight, HighlightNodeBrush.AsBrush(), true);
            }

            if (Main.LinkFrom != Node.NullNode)
            {
                DrawNode(g, Main.LinkFrom, NodeToLinkNextBrush.AsBrush(), true);
            }
        }

        private static void DrawEdge(Graphics g, Node parent, Node child, ID2D1Brush* pen, PointF start = default, PointF end = default)
        {
            int third = 0;

            third = Main.GetEdgeStartHeight(parent, child, third);
            if (third == 0)
            {
                third = Main.GetEdgeStartHeight(child, parent, third);
            }
            if (start == default)
            {
                start = Main.GetStartHeightFromThird(parent, parent.Position, third);
            }
            if (end == default)
            {
                end = child.Position + new SizeF(0, child.Size.Height / 2);
            }

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

            //todo convert to direct2d
            //g.DrawBezier(pen, start, controlStart, controlEnd, end);
            //e.Graphics.DrawEllipse(Pens.Green, new Rectangle(controlStart, new Size(4, 4)));
            //e.Graphics.DrawEllipse(Pens.Red, new Rectangle(controlEnd, new Size(4, 4)));
        }

        private void DrawNode(Graphics g, Node node, ID2D1Brush* brush, bool lighttext = false)
        {
            if (node == Node.NullNode)
            {
                return;
            }
            if (Main.Scalee > 0.28f)
            {
                Main.GetLinkCircleRects(node, out RectangleF leftRect, out RectangleF rightRect);

                Ellipse left = leftRect.ToEllipse();
                Ellipse right = rightRect.ToEllipse();

                target.DrawEllipse(&left, circlePen.AsBrush(), circlePenWidth, defaultStyle);
                target.DrawEllipse(&right, circlePen.AsBrush(), circlePenWidth, defaultStyle);
            }
            if (node == Main.Selected)
            {
                lighttext = true;
                if (node.FileName != Main.SelectedCharacter)
                {
                    lighttext = true;
                    var interRect = ScaleRect(node.Rectangle, 25).ToRoundedRect(18f);
                    target.FillRoundedRectangle(ref interRect, ClickedNodeBrush);
                }
                var clickRect = ScaleRect(node.Rectangle, 15).ToRoundedRect(15f);
                target.FillRoundedRectangle(ref clickRect, ClickedNodeBrush);
            }
            else if (node.FileName != Main.SelectedCharacter)
            {
                var interRect = ScaleRect(node.Rectangle, 15).ToRoundedRect(15f);
                target.FillRoundedRectangle(ref interRect, InterlinkedNodeBrush);
            }

            var rect = node.Rectangle.ToRoundedRect(10f);
            target.FillRoundedRectangle(ref rect, brush);

            if (Main.selected.Contains(node))
            {
                var selectRect = node.Rectangle.ToBox<float>();
                target.DrawRectangle(ref selectRect, SelectionEdge.AsBrush(), selectionEdgeWidth, defaultStyle);
            }

            if (Main.Scalee > 0.28f)
            {
                var scaledRect = node.Rectangle;
                scaledRect.Location += new Size(3, 3);
                scaledRect.Size -= new Size(6, 6);

                bool useBlack = false;
                var brushColor = (*(ID2D1SolidColorBrush*)brush).GetColor();
                if ((brushColor.R * 0.299 + brushColor.G * 0.587 + brushColor.B * 0.114) > 0.6f)
                {
                    useBlack = true;
                }

                var text = node.Text.AsSpan()[..Math.Min(node.Text.Length, 100)].ToArray();
                if (text.Length == 0)
                {
                    text = [' '];
                }
                var textRect = scaledRect.ToBox<float>();
                fixed (char* t = text)
                {
                    if (useBlack)
                    {
                        D2D1DCRenderTargetVtblExtensions.DrawTextA(target, t, (uint)text.Length, (Silk.NET.Direct2D.IDWriteTextFormat*)defaultFormat.Handle, &textRect, BlackTextBrush.AsBrush(), DrawTextOptions.Clip, DwriteMeasuringMode.Natural);
                    }
                    else if (lighttext)
                    {
                        D2D1DCRenderTargetVtblExtensions.DrawTextA(target, t, (uint)text.Length, (Silk.NET.Direct2D.IDWriteTextFormat*)defaultFormat.Handle, &textRect, LightTextBrush.AsBrush(), DrawTextOptions.Clip, DwriteMeasuringMode.Natural);
                    }
                    else
                    {
                        D2D1DCRenderTargetVtblExtensions.DrawTextA(target, t, (uint)text.Length, (Silk.NET.Direct2D.IDWriteTextFormat*)defaultFormat.Handle, &textRect, DarkTextBrush.AsBrush(), DrawTextOptions.Clip, DwriteMeasuringMode.Natural);
                    }
                }
            }
        }

        private static RectangleF ScaleRect(RectangleF rect, float increase) => new(rect.X - (increase / 2), rect.Y - (increase / 2), rect.Width + increase, rect.Height + increase);

        private ID2D1Brush* GetNodeColor(NodeType type, bool light)
        {
            return type switch
            {
                NodeType.Null => light ? defaultNodeBrush.AsBrush() : darkdefaultNodeBrush.AsBrush(),
                NodeType.CharacterGroup => light ? characterGroupNodeBrush.AsBrush() : darkcharacterGroupNodeBrush.AsBrush(),
                NodeType.Criterion => light ? criterionNodeBrush.AsBrush() : darkcriterionNodeBrush.AsBrush(),
                NodeType.ItemAction => light ? itemActionNodeBrush.AsBrush() : darkitemActionNodeBrush.AsBrush(),
                NodeType.ItemGroupBehaviour => light ? itemGroupBehaviourNodeBrush.AsBrush() : darkitemGroupBehaviourNodeBrush.AsBrush(),
                NodeType.ItemGroupInteraction => light ? itemGroupInteractionNodeBrush.AsBrush() : darkitemGroupInteractionNodeBrush.AsBrush(),
                NodeType.Pose => light ? poseNodeBrush.AsBrush() : darkposeNodeBrush.AsBrush(),
                NodeType.Achievement => light ? achievementNodeBrush.AsBrush() : darkachievementNodeBrush.AsBrush(),
                NodeType.BGC => light ? bgcNodeBrush.AsBrush() : darkbgcNodeBrush.AsBrush(),
                NodeType.BGCResponse => light ? bgcResponseNodeBrush.AsBrush() : darkbgcResponseNodeBrush.AsBrush(),
                NodeType.Clothing => light ? clothingNodeBrush.AsBrush() : darkclothingNodeBrush.AsBrush(),
                NodeType.CriteriaGroup => light ? criteriaGroupNodeBrush.AsBrush() : darkcriteriaGroupNodeBrush.AsBrush(),
                NodeType.Cutscene => light ? cutsceneNodeBrush.AsBrush() : darkcutsceneNodeBrush.AsBrush(),
                NodeType.Dialogue => light ? dialogueNodeBrush.AsBrush() : darkdialogueNodeBrush.AsBrush(),
                NodeType.AlternateText => light ? alternateTextNodeBrush.AsBrush() : darkalternateTextNodeBrush.AsBrush(),
                NodeType.Door => light ? doorNodeBrush.AsBrush() : darkdoorNodeBrush.AsBrush(),
                NodeType.GameEvent => light ? eventNodeBrush.AsBrush() : darkeventNodeBrush.AsBrush(),
                NodeType.EventTrigger => light ? eventTriggerNodeBrush.AsBrush() : darkeventTriggerNodeBrush.AsBrush(),
                NodeType.Inventory => light ? inventoryNodeBrush.AsBrush() : darkinventoryNodeBrush.AsBrush(),
                NodeType.StoryItem => light ? itemNodeBrush.AsBrush() : darkitemNodeBrush.AsBrush(),
                NodeType.ItemGroup => light ? itemGroupNodeBrush.AsBrush() : darkitemGroupNodeBrush.AsBrush(),
                NodeType.Personality => light ? personalityNodeBrush.AsBrush() : darkpersonalityNodeBrush.AsBrush(),
                NodeType.Property => light ? propertyNodeBrush.AsBrush() : darkpropertyNodeBrush.AsBrush(),
                NodeType.Quest => light ? questNodeBrush.AsBrush() : darkquestNodeBrush.AsBrush(),
                NodeType.Response => light ? responseNodeBrush.AsBrush() : darkresponseNodeBrush.AsBrush(),
                NodeType.Social => light ? socialNodeBrush.AsBrush() : darksocialNodeBrush.AsBrush(),
                NodeType.State => light ? stateNodeBrush.AsBrush() : darkstateNodeBrush.AsBrush(),
                NodeType.Value => light ? valueNodeBrush.AsBrush() : darkvalueNodeBrush.AsBrush(),
                _ => defaultNodeBrush.AsBrush(),
            };
        }

        private void GetD2DResources()
        {
            unsafe
            {
                d2d = D2D.GetApi();

                Guid factoryguid = ID2D1Factory.Guid;
                FactoryOptions options = new(DebugLevel.Information);
                void* pfactory = (void*)0;
                HResult res = d2d.D2D1CreateFactory(FactoryType.SingleThreaded, ref factoryguid, ref options, ref pfactory);
                factory = new((ID2D1Factory*)pfactory);
                if (res != 0)
                {
                    Debugger.Break();
                }

                RenderTargetProperties renderTargetProperties = new()
                {
                    Type = RenderTargetType.Default,
                    DpiY = 0,
                    DpiX = 0,
                    PixelFormat = new()
                    {
                        AlphaMode = AlphaMode.Premultiplied,
                        Format = Format.FormatB8G8R8A8Unorm
                    },
                    Usage = RenderTargetUsage.GdiCompatible,
                    MinLevel = FeatureLevel.LevelDefault,
                };
                res = factory.CreateDCRenderTarget(&renderTargetProperties, ref target);
                if (res != 0)
                {
                    Debugger.Break();
                }

                CreateBrush(Color.FromArgb(75, 75, 75), ref linePen);
                CreateBrush(Color.FromArgb(75, 75, 75), ref circlePen);
                CreateBrush(Color.LightGray, ref clickedLinePen);
                CreateBrush(Color.DeepPink, ref highlightPen);
                CreateBrush(Color.White, ref SelectionEdge);
                CreateBrush(Color.FromArgb(100, 100, 100), ref defaultNodeBrush);
                CreateBrush(Color.FromArgb(200, 10, 200), ref achievementNodeBrush);
                CreateBrush(Color.FromArgb(110, 120, 190), ref alternateTextNodeBrush);
                CreateBrush(Color.FromArgb(40, 190, 255), ref bgcNodeBrush);
                CreateBrush(Color.FromArgb(150, 225, 255), ref bgcResponseNodeBrush);
                CreateBrush(Color.FromArgb(190, 180, 130), ref characterGroupNodeBrush);
                CreateBrush(Color.FromArgb(95, 235, 60), ref clothingNodeBrush);
                CreateBrush(Color.FromArgb(150, 50, 50), ref criteriaGroupNodeBrush);
                CreateBrush(Color.FromArgb(180, 20, 40), ref criterionNodeBrush);
                CreateBrush(Color.FromArgb(235, 30, 160), ref cutsceneNodeBrush);
                CreateBrush(Color.FromArgb(45, 60, 185), ref dialogueNodeBrush);
                CreateBrush(Color.FromArgb(200, 225, 65), ref doorNodeBrush);
                CreateBrush(Color.FromArgb(50, 150, 50), ref eventNodeBrush);
                CreateBrush(Color.FromArgb(60, 100, 70), ref eventTriggerNodeBrush);
                CreateBrush(Color.FromArgb(65, 225, 185), ref inventoryNodeBrush);
                CreateBrush(Color.FromArgb(85, 195, 195), ref itemActionNodeBrush);
                CreateBrush(Color.FromArgb(160, 200, 195), ref itemGroupBehaviourNodeBrush);
                CreateBrush(Color.FromArgb(95, 120, 115), ref itemGroupInteractionNodeBrush);
                CreateBrush(Color.FromArgb(45, 190, 165), ref itemGroupNodeBrush);
                CreateBrush(Color.FromArgb(45, 255, 255), ref itemNodeBrush);
                CreateBrush(Color.FromArgb(255, 255, 90), ref personalityNodeBrush);
                CreateBrush(Color.FromArgb(255, 210, 90), ref poseNodeBrush);
                CreateBrush(Color.FromArgb(255, 90, 150), ref propertyNodeBrush);
                CreateBrush(Color.FromArgb(255, 103, 0), ref questNodeBrush);
                CreateBrush(Color.FromArgb(55, 155, 225), ref responseNodeBrush);
                CreateBrush(Color.FromArgb(255, 160, 90), ref socialNodeBrush);
                CreateBrush(Color.FromArgb(40, 190, 50), ref stateNodeBrush);
                CreateBrush(Color.FromArgb(120, 0, 150), ref valueNodeBrush);

                //darker color variants
                float darkening = 0.18f;
                CreateBrush(Color.FromArgb(100, 100, 100).Times(darkening), ref darkdefaultNodeBrush);
                CreateBrush(Color.FromArgb(200, 10, 200).Times(darkening), ref darkachievementNodeBrush);
                CreateBrush(Color.FromArgb(110, 120, 190).Times(darkening), ref darkalternateTextNodeBrush);
                CreateBrush(Color.FromArgb(40, 190, 255).Times(darkening), ref darkbgcNodeBrush);
                CreateBrush(Color.FromArgb(150, 225, 255).Times(darkening), ref darkbgcResponseNodeBrush);
                CreateBrush(Color.FromArgb(190, 180, 130).Times(darkening), ref darkcharacterGroupNodeBrush);
                CreateBrush(Color.FromArgb(95, 235, 60).Times(darkening), ref darkclothingNodeBrush);
                CreateBrush(Color.FromArgb(150, 50, 50).Times(darkening), ref darkcriteriaGroupNodeBrush);
                CreateBrush(Color.FromArgb(180, 20, 40).Times(darkening), ref darkcriterionNodeBrush);
                CreateBrush(Color.FromArgb(235, 30, 160).Times(darkening), ref darkcutsceneNodeBrush);
                CreateBrush(Color.FromArgb(45, 60, 185).Times(darkening), ref darkdialogueNodeBrush);
                CreateBrush(Color.FromArgb(200, 225, 65).Times(darkening), ref darkdoorNodeBrush);
                CreateBrush(Color.FromArgb(50, 150, 50).Times(darkening), ref darkeventNodeBrush);
                CreateBrush(Color.FromArgb(60, 100, 70).Times(darkening), ref darkeventTriggerNodeBrush);
                CreateBrush(Color.FromArgb(65, 225, 185).Times(darkening), ref darkinventoryNodeBrush);
                CreateBrush(Color.FromArgb(85, 195, 195).Times(darkening), ref darkitemActionNodeBrush);
                CreateBrush(Color.FromArgb(160, 200, 195).Times(darkening), ref darkitemGroupBehaviourNodeBrush);
                CreateBrush(Color.FromArgb(95, 120, 115).Times(darkening), ref darkitemGroupInteractionNodeBrush);
                CreateBrush(Color.FromArgb(45, 190, 165).Times(darkening), ref darkitemGroupNodeBrush);
                CreateBrush(Color.FromArgb(45, 255, 255).Times(darkening), ref darkitemNodeBrush);
                CreateBrush(Color.FromArgb(255, 255, 90).Times(darkening), ref darkpersonalityNodeBrush);
                CreateBrush(Color.FromArgb(255, 210, 90).Times(darkening), ref darkposeNodeBrush);
                CreateBrush(Color.FromArgb(255, 90, 150).Times(darkening), ref darkpropertyNodeBrush);
                CreateBrush(Color.FromArgb(255, 103, 0).Times(darkening), ref darkquestNodeBrush);
                CreateBrush(Color.FromArgb(55, 155, 225).Times(darkening), ref darkresponseNodeBrush);
                CreateBrush(Color.FromArgb(255, 160, 90).Times(darkening), ref darksocialNodeBrush);
                CreateBrush(Color.FromArgb(40, 190, 50).Times(darkening), ref darkstateNodeBrush);
                CreateBrush(Color.FromArgb(120, 0, 150).Times(darkening), ref darkvalueNodeBrush);

                CreateBrush(Color.DarkCyan, ref HighlightNodeBrush);
                CreateBrush(Color.BlueViolet, ref ClickedNodeBrush);
                CreateBrush(Color.LightGray, ref NodeToLinkNextBrush);
                CreateBrush(Color.White, ref LightTextBrush);
                CreateBrush(Color.DarkGray, ref DarkTextBrush);
                CreateBrush(Color.Black, ref BlackTextBrush);

                BrushProperties brushProperties = new(opacity: 100);
                LinearGradientBrushProperties linearProperties = new()
                {
                    StartPoint = new(0, 0),
                    EndPoint = new(150, 150)
                };
                GradientStop[] gradientStops = [
                    new(){
                        Color = Color.DeepPink.ToD3D(),
                        Position = 0
                    },
                    new(){
                        Color = Color.DeepSkyBlue.ToD3D(),
                        Position = 1
                    }
                    ];

                res = target.CreateGradientStopCollection(gradientStops, 2, Gamma.Gamma22, ExtendMode.Clamp, interlinkedGradientstops.GetAddressOf());
                if (res != 0)
                {
                    Debugger.Break();
                }
                res = target.CreateLinearGradientBrush(&linearProperties, &brushProperties, interlinkedGradientstops, InterlinkedNodeBrush.GetAddressOf());
                if (res != 0)
                {
                    Debugger.Break();
                }

                StrokeStyleProperties defaultProps = new()
                {
                    DashCap = CapStyle.Flat,
                    DashOffset = 0,
                    DashStyle = DashStyle.Solid,
                    EndCap = CapStyle.Flat,
                    LineJoin = LineJoin.Miter,
                    MiterLimit = 1,
                    StartCap = CapStyle.Flat
                };
                float[] dashes = [];
                res = factory.CreateStrokeStyle(&defaultProps, dashes, 0, defaultStyle.GetAddressOf());
                if (res != 0)
                {
                    Debugger.Break();
                }

                defaultProps.DashStyle = DashStyle.Dash;
                res = factory.CreateStrokeStyle(&defaultProps, dashes, 0, interlinkedStyle.GetAddressOf());
                if (res != 0)
                {
                    Debugger.Break();
                }

                var dw = DWrite.GetApi();
                res = dw.DWriteCreateFactory(DWFactoryType.Shared, out dwfactory);
                if (res != 0)
                {
                    Debugger.Break();
                }

                res = DWExtensions.CreateTextFormat(dwfactory,
                                                    fontFamilyName,
                                                    (Silk.NET.DirectWrite.IDWriteFontCollection*)0,
                                                    Silk.NET.DirectWrite.FontWeight.Normal,
                                                    Silk.NET.DirectWrite.FontStyle.Normal,
                                                    Silk.NET.DirectWrite.FontStretch.Normal,
                                                    fontSize * Main.Scalee,
                                                    locale,
                                                    defaultFormat.GetAddressOf());
                if (res != 0)
                {
                    Debugger.Break();
                }
            }
        }

        private void ReleaseD2DResources()
        {
            //release factory twice because the boxing somehow counts as one reference
            factory.Release();
            factory.Release();

            clickedLinePen.Release();
            SelectionEdge.Release();
            highlightPen.Release();
            linePen.Release();
            circlePen.Release();
            ClickedNodeBrush.Release();
            HighlightNodeBrush.Release();
            NodeToLinkNextBrush.Release();
            achievementNodeBrush.Release();
            alternateTextNodeBrush.Release();
            bgcNodeBrush.Release();
            bgcResponseNodeBrush.Release();
            characterGroupNodeBrush.Release();
            clothingNodeBrush.Release();
            criteriaGroupNodeBrush.Release();
            criterionNodeBrush.Release();
            cutsceneNodeBrush.Release();
            defaultNodeBrush.Release();
            dialogueNodeBrush.Release();
            doorNodeBrush.Release();
            eventNodeBrush.Release();
            eventTriggerNodeBrush.Release();
            inventoryNodeBrush.Release();
            itemActionNodeBrush.Release();
            itemGroupBehaviourNodeBrush.Release();
            itemGroupInteractionNodeBrush.Release();
            itemGroupNodeBrush.Release();
            itemNodeBrush.Release();
            personalityNodeBrush.Release();
            poseNodeBrush.Release();
            propertyNodeBrush.Release();
            questNodeBrush.Release();
            responseNodeBrush.Release();
            socialNodeBrush.Release();
            stateNodeBrush.Release();
            valueNodeBrush.Release();
            darkachievementNodeBrush.Release();
            darkalternateTextNodeBrush.Release();
            darkbgcNodeBrush.Release();
            darkbgcResponseNodeBrush.Release();
            darkcharacterGroupNodeBrush.Release();
            darkclothingNodeBrush.Release();
            darkcriteriaGroupNodeBrush.Release();
            darkcriterionNodeBrush.Release();
            darkcutsceneNodeBrush.Release();
            darkdefaultNodeBrush.Release();
            darkdialogueNodeBrush.Release();
            darkdoorNodeBrush.Release();
            darkeventNodeBrush.Release();
            darkeventTriggerNodeBrush.Release();
            darkinventoryNodeBrush.Release();
            darkitemActionNodeBrush.Release();
            darkitemGroupBehaviourNodeBrush.Release();
            darkitemGroupInteractionNodeBrush.Release();
            darkitemGroupNodeBrush.Release();
            darkitemNodeBrush.Release();
            darkpersonalityNodeBrush.Release();
            darkposeNodeBrush.Release();
            darkpropertyNodeBrush.Release();
            darkquestNodeBrush.Release();
            darkresponseNodeBrush.Release();
            darksocialNodeBrush.Release();
            darkstateNodeBrush.Release();
            darkvalueNodeBrush.Release();
            InterlinkedNodeBrush.Release();
            interlinkedGradientstops.Release();

            defaultStyle.Release();
            interlinkedStyle.Release();
            defaultFormat.Release();
            dwfactory.Release();
            LightTextBrush.Release();
            DarkTextBrush.Release();
            BlackTextBrush.Release();

            target.Release();
            d2d.Dispose();
        }
    }
}

