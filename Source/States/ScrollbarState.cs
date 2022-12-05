namespace Xplorer.States;

public class ScrollbarState
{
    public bool Visible { get; }
    public int GripStartPosition { get; }
    public int GripEndPosition { get; }

    public ScrollbarState(bool visible, int gripStartPosition, int gripEndPosition)
    {
        Visible = visible;
        GripStartPosition = gripStartPosition;
        GripEndPosition = gripEndPosition;
    }
}