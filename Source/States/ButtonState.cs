namespace Xplorer.States
{
    public class ButtonState
    {
        public string Label { get; }
        public bool Enabled { get; }

        public ButtonState(string label, bool enabled)
        {
            Label = label;
            Enabled = enabled;
        }
    }
}