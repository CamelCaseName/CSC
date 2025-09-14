namespace CSC.Components
{
    public class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint
                     | ControlStyles.ResizeRedraw
                     | ControlStyles.ContainerControl
                          , true);
        }
    }
}
