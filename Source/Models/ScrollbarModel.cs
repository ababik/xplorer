namespace Xplorer.Models
{
    public class ScrollbarModel
    {
        public bool Visible { get; }
        public int GripStartPosition { get; }
        public int GripEndPosition { get; }

        public ScrollbarModel(bool visible, int gripStartPosition, int gripEndPosition)
        {
            Visible = visible;
            GripStartPosition = gripStartPosition;
            GripEndPosition = gripEndPosition;
        }
    }
}