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

                var mousePosition = Main.ActiveForm!.PointToClient(Cursor.Position);
                MouseMove(null, new MouseEventArgs(Control.MouseButtons, 0, mousePosition.X, mousePosition.Y, 0));
            }
            return false;
        }
    }
}
