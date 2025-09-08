using Microsoft.VisualBasic.Devices;
using System.Drawing.Drawing2D;
using Translator.Explorer.JSONItems;

namespace CSC
{
    public partial class Main : Form
    {
        public bool MovingChild = false;
        Size OffsetFromDragClick = Size.Empty;
        private Control? movedChild;
        Point start;
        Point end;
        Pen linePen;

        public Main()
        {
            InitializeComponent();
            button1.MouseMove += Main_MouseMove;
            button2.MouseMove += Main_MouseMove;
            linePen = new Pen(Brushes.Black, 2);
            linePen.EndCap = LineCap.Triangle;
            linePen.StartCap = LineCap.Round;
        }

        private void Start_Click(object sender, EventArgs e)
        {

        }

        private void Add_Click(object sender, EventArgs e)
        {

        }

        private void Main_Click(object sender, EventArgs e)
        {
            var pos = PointToClient(Cursor.Position);
            movedChild = GetChildAtPoint(pos + new Size(4, 4));
            movedChild ??= GetChildAtPoint(pos + new Size(4, -4));
            movedChild ??= GetChildAtPoint(pos + new Size(-4, 4));
            movedChild ??= GetChildAtPoint(pos + new Size(-4, -4));
            if (movedChild is not null)
            {
                if (movedChild.GetType() == typeof(ToolStrip))
                {
                    return;
                }

                MovingChild = !MovingChild;
                if (MovingChild)
                {
                    OffsetFromDragClick = new Size(movedChild.Location.X - pos.X, movedChild.Location.Y - pos.Y);
                }
            }
        }

        private void Main_DoubleClick(object sender, EventArgs e)
        {
            Details.Visible = !Details.Visible;
        }

        private void Main_MouseMove(object? sender, MouseEventArgs e)
        {
            if (MovingChild && movedChild is not null)
            {
                movedChild.Location = e.Location + OffsetFromDragClick;
                Invalidate();

            }
        }

        //draw bezier curves from one button to other

        //to smoothly enter and exit we must set the y axis to the same value as the button, and the x value to some value in between both buttons

        private void Button1Click(object sender, MouseEventArgs e)
        {
            Details.SelectedObject = new Criterion();
        }

        private void Button2Click(object sender, EventArgs e)
        {
            Details.SelectedObject = new BackgroundChatterResponse();
        }

        private void Main_Paint(object sender, PaintEventArgs e)
        {
            //todo filter, duh
            DrawEdge(e, button1, button2);
            DrawEdge(e, button1, button3);
            DrawEdge(e, button2, button4);
            DrawEdge(e, button3, button5);
            DrawEdge(e, button5, button1);

        }

        private void DrawEdge(PaintEventArgs e, Control parent, Control child)
        {
            start = parent.Location + new Size(parent.Size.Width, parent.Size.Height / 2);
            end = child.Location + new Size(0, child.Size.Height / 2);


            Point controlStart;
            Point controlEnd;
            int controlEndY, controlStartY;

            int distanceX = Math.Abs(end.X - start.X);
            if (start.X < end.X)
            {
                controlStart = new Point((distanceX / 2) + start.X, start.Y);
                controlEnd = new Point(((end.X - start.X) / 2) + start.X, end.Y);
            }
            else
            {
                int distanceY = Math.Abs(end.Y - start.Y);
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
                controlStart = new Point((start.X + distanceX / 2), controlStartY);
                controlEnd = new Point((end.X - distanceX / 2), controlEndY);
            }

            e.Graphics.DrawBezier(linePen, start, controlStart, controlEnd, end);
            e.Graphics.DrawEllipse(Pens.Green, new Rectangle(controlStart, new Size(4, 4)));
            e.Graphics.DrawEllipse(Pens.Red, new Rectangle(controlEnd, new Size(4, 4)));
            
        }
    }
}
