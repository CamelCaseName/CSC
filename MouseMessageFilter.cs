namespace CSC
{
    class MouseMessageFilter : IMessageFilter
    {
        public static event MouseEventHandler MouseMove = delegate { };
        const int WM_MOUSEMOVE = 0x0200;

        public bool PreFilterMessage(ref Message m)
        {

            if (m.Msg == WM_MOUSEMOVE)
            {
                if(Main.ActiveForm is null)
                {
                    return false;
                }

                var mousePosition = new Point(0,0);
                MouseMove(null, new MouseEventArgs(Control.MouseButtons, 0, mousePosition.X, mousePosition.Y, 0));
            }
            return false;
        }
    }
}
