namespace Xplorer.States;

public class ToolbarState
{
    public ButtonState HelpButton { get; }
    public ButtonState CopyButton { get; }
    public ButtonState MoveButton { get; }
    public ButtonState RenameButton { get; }
    public ButtonState RemoveButton { get; }
    public DialogState Dialog { get; }

    public ToolbarState(ButtonState helpButton, ButtonState copyButton, ButtonState moveButton, ButtonState renameButton, ButtonState removeButton, DialogState dialog)
    {
        HelpButton = helpButton;
        CopyButton = copyButton;
        MoveButton = moveButton;
        RenameButton = renameButton;
        RemoveButton = removeButton;
        Dialog = dialog;
    }
}