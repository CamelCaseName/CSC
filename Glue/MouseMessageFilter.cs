namespace CSC.Glue
{
    class MouseMessageFilter : IMessageFilter
    {
        public static event MouseEventHandler MouseMove = delegate { };
        const int WM_MOUSEMOVE = 0x0200;

        public bool PreFilterMessage(ref Message m)
        {

            if (m.Msg == WM_MOUSEMOVE)
            {
                if(Form.ActiveForm is null)
                {
                    return false;
                }
                if(Form.ActiveForm.GetType() != typeof(Main))
                {
                    return false;
                }

                var mousePosition = new Point(0,0);
                MouseMove(null, new MouseEventArgs(MouseButtons.None, 0, mousePosition.X, mousePosition.Y, 0));
            }
            return false;
        }
    }
}
