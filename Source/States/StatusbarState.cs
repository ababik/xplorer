namespace Xplorer.States;

public class StatusbarState
{
    public string Status { get; }
    
    public StatusbarState(string status)
    {
        Status = status;
    }
}