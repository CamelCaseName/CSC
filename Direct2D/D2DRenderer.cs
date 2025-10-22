using CSC.Glue;
using CSC.Nodestuff;
using Silk.NET.Core.Native;
using Silk.NET.Direct2D;
using Silk.NET.DXGI;
using Silk.NET.Maths;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using AlphaMode = Silk.NET.Direct2D.AlphaMode;
using Rectangle = System.Drawing.Rectangle;

namespace CSC.Direct2D
{
    internal unsafe class D2DRenderer
    {
        //direct2d stuffs
        D2D d2d = null!;
        ComPtr<ID2D1Factory> factory = default;
        //ID2D1HwndRenderTarget target;
        ComPtr<ID2D1DCRenderTarget> target = default;

        private ComPtr<ID2D1SolidColorBrush> clickedLinePen;
        private ComPtr<ID2D1SolidColorBrush> SelectionEdge;
        private ComPtr<ID2D1SolidColorBrush> highlightPen;
        private ComPtr<ID2D1SolidColorBrush> linePen;
        private ComPtr<ID2D1SolidColorBrush> circlePen;
        private ComPtr<ID2D1SolidColorBrush> ClickedNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> HighlightNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> NodeToLinkNextBrush;
        private ComPtr<ID2D1SolidColorBrush> achievementNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> alternateTextNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> bgcNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> bgcResponseNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> characterGroupNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> clothingNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> criteriaGroupNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> criterionNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> cutsceneNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> defaultNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> dialogueNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> doorNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> eventNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> eventTriggerNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> inventoryNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> itemActionNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> itemGroupBehaviourNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> itemGroupInteractionNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> itemGroupNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> itemNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> personalityNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> poseNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> propertyNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> questNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> responseNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> socialNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> stateNodeBrush;
        private ComPtr<ID2D1SolidColorBrush> valueNodeBrush;
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
        private ComPtr<ID2D1LinearGradientBrush> InterlinkedNodeBrush;
        private ComPtr<ID2D1GradientStopCollection> interlinkedGradientstops;

        //misc
        private const TextFormatFlags TextFlags = TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.LeftAndRightPadding;
        private Font scaledFont = new(FontFamily.GenericSansSerif, 8f);
        private readonly SizeF CircleSize = new(15, 15);
        private RectangleF adjustedVisibleClipBounds = new();

        public D2DRenderer()
        {
            GetD2DResources();
        }

        ~D2DRenderer()
        {
            ReleaseD2DResources();
        }

        public void Release()
        {
            ReleaseD2DResources();
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

                //linePen = new Pen(Color.FromArgb(75, 75, 75), 0.2f)
                //{
                //    EndCap = LineCap.Triangle,
                //    StartCap = LineCap.Round
                //};
                //circlePen = new Pen(Color.FromArgb(75, 75, 75), 0.5f)
                //{
                //    EndCap = LineCap.Triangle,
                //    StartCap = LineCap.Round
                //};
                //clickedLinePen = new Pen(Brushes.LightGray, 3)
                //{
                //    EndCap = LineCap.Triangle,
                //    StartCap = LineCap.Round
                //};
                //highlightPen = new Pen(Brushes.DeepPink, 3)
                //{
                //    EndCap = LineCap.Triangle,
                //    StartCap = LineCap.Round
                //};
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

                //todo we have to release this for sure later...
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

            target.Release();
            d2d.Dispose();
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

        public void Paint(Graphics g, NodeStore nodes)
        {
            unsafe
            {
                //g.ToLowQuality();
                //Bind each frame...
                //todo investigate what rect we need and if its enough to only bind on resize
                Box2D<int> dcRect = g.ClipBounds.ToBox<int>();
                return;
                int res = target.BindDC(g.GetHdc(), ref dcRect);
                if (res != 0)
                {
                    Debugger.Break();
                }

                target.BeginDraw();
                Matrix3X2<float> identity = Matrix3X2<float>.Identity;
                target.SetTransform(ref identity);
                D3Dcolorvalue bc = Color.White.ToD3D();
                target.Clear(ref bc);

                ////update canvas transforms
                //g.TranslateTransform(-OffsetX[SelectedCharacter] * Scaling[SelectedCharacter], -OffsetY[SelectedCharacter] * Scaling[SelectedCharacter]);
                //g.ScaleTransform(Scaling[SelectedCharacter], Scaling[SelectedCharacter]);
                //adjustedVisibleClipBounds = new(OffsetX[SelectedCharacter] - NodeSizeX,
                //                                OffsetY[SelectedCharacter] - NodeSizeY,
                //                                g.VisibleClipBounds.Width + NodeSizeX,
                //                                g.VisibleClipBounds.Height + NodeSizeY);
                //adjustedMouseClipBounds = new(OffsetX[SelectedCharacter],
                //                                OffsetY[SelectedCharacter],
                //                                g.VisibleClipBounds.Width,
                //                                g.VisibleClipBounds.Height);

                //if (Scaling[SelectedCharacter] < 0)
                //{
                //    Debugger.Break();
                //}
                //scaledFont = GetScaledFont(new(DefaultFont.FontFamily, 8f), Scaling[SelectedCharacter]);

                //DrawAllNodes(g, nodes);

                ulong tag1 = 0, tag2 = 0;
                target.EndDraw(ref tag1, ref tag2);
            }
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

        private static RectangleF ScaleRect(RectangleF rect, float increase) => new(rect.X - (increase / 2), rect.Y - (increase / 2), rect.Width + increase, rect.Height + increase);

        private static GraphicsPath RoundedRect(RectangleF bounds, float radius)
        {//see https://stackoverflow.com/questions/33853434/how-to-draw-a-rounded-rectangle-in-c-sharp

            float diameter = radius * 2;
            SizeF size = new(diameter, diameter);
            RectangleF arc = new(bounds.Location, size);
            GraphicsPath path = new();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private static Font GetScaledFont(Font f, float scale)
        {
            //see https://stackoverflow.com/questions/8850528/how-to-apply-graphics-scale-and-translate-to-the-textrenderer

            if (f.SizeInPoints * scale < 0)
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

        private static Rectangle GetScaledRect(Rectangle r, float scale)
        {
            return new Rectangle((int)Math.Ceiling(r.X * scale),
                                (int)Math.Ceiling(r.Y * scale),
                                (int)Math.Ceiling(r.Width * scale),
                                (int)Math.Ceiling(r.Height * scale));
        }

        private void DrawNode(Graphics g, Node node, ID2D1Brush* brush, bool lightText = false)
        {
            if (node == Node.NullNode)
            {
                return;
            }
            if (Main.Scalee > 0.28f)
            {
                Main.GetLinkCircleRects(node, out RectangleF leftRect, out RectangleF rightRect);

                //g.DrawEllipse(circlePen, leftRect);
                //g.DrawEllipse(circlePen, rightRect);
            }
            if (node == Main.Selected)
            {
                lightText = true;
                if (node.FileName != Main.SelectedCharacter)
                {
                    lightText = true;
                    //g.FillPath(InterlinkedNodeBrush, RoundedRect(ScaleRect(node.Rectangle, 25), 18f));
                }
                //g.FillPath(ClickedNodeBrush, RoundedRect(ScaleRect(node.Rectangle, 15), 15f));
            }
            else if (node.FileName != Main.SelectedCharacter)
            {
                //g.FillPath(InterlinkedNodeBrush, RoundedRect(ScaleRect(node.Rectangle, 15), 15f));
            }

            //g.FillPath(brush, RoundedRect(node.Rectangle, 10f));
            if (Main.selected.Contains(node))
            {
                //g.DrawRectangle(SelectionEdge, node.Rectangle);
            }

            if (Main.Scalee > 0.28f)
            {
                var scaledRect = GetScaledRect(node.RectangleNonF, Main.Scalee);
                scaledRect.Location += new Size(3, 3);
                scaledRect.Size -= new Size(6, 6);

                Color textColor = lightText ? Color.White : Color.DarkGray;
                //todo figure out how to do this...
                //if (((*brush).R * 0.299 + brush.Color.G * 0.587 + brush.Color.B * 0.114) > 150)
                //{
                //    textColor = Color.Black;
                //}

                TextRenderer.DrawText(g,
                                      node.Text[..Math.Min(node.Text.Length, 100)],
                                      scaledFont,
                                      scaledRect,
                                      textColor,
                                      TextFlags);
            }
        }
    }
}

