namespace Xplorer.Models
{
    public class ScrollbarModel
    {
        public int GripStartPosition { get; }
        public int GripEndPosition { get; }

        public ScrollbarModel(int gripStartPosition, int gripEndPosition)
        {
            GripStartPosition = gripStartPosition;
            GripEndPosition = gripEndPosition;
        }
    }
}