using System.Linq;
using System.Threading.Tasks;
using Remutable.Extensions;
using Xplorer.Actions;
using Xplorer.States;

namespace Xplorer.Models;

public class CopyTransaction : Transaction
{
    private CopyTransactionState TransactionState { get; set; }
    private IFileSystem FileSystem { get; }
    private Navigation Source { get; }
    private Navigation Destination { get; }

    public CopyTransaction(Context context) : base(context)
    {
        FileSystem = NavigationActions.GetActiveFileSystem(Context);
        Source = NavigationActions.GetActiveNavigation(Context);
        Destination = context.Model.PrimaryNavigation == Source
            ? Context.Model.SecondaryNavigation
            : Context.Model.PrimaryNavigation;
    }

    public override void Start()
    {
        base.Start();
        
        Context.State = Context.State.Remute(x => x.Toolbar.Dialog, new CopyCheckDialogState());
        Render();
        
        Check();
    }

    private void Check()
    {
        TransactionState = CopyTransactionState.Check;

        var entries = NavigationActions.GetNavigationEntriesForTransaction(Source).ToArray();
        var collision = FileSystem.CheckCopy(entries, Destination.Location, out var size);

        if (collision)
        {
            Confirm();
        }
        else
        {
            Process(entries, false);
        }
    }

    private void Confirm()
    {
        TransactionState = CopyTransactionState.Confirm;

        Context.State = Context.State.Remute(x => x.Toolbar.Dialog, new CopyConfirmDialogState());
        Render();
    }

    private void Process(NavigationEntry[] entries, bool overwrite)
    {
        TransactionState = CopyTransactionState.Process;

        Context.State = Context.State.Remute(x => x.Toolbar.Dialog, new CopyProcessDialogState());
        Render();

        FileSystem.Copy(entries, Destination.Location, overwrite);

        Context.State = NavigationActions.RefreshNavigations(Context, Context.State);
        Context.State = Context.State.Remute(x => x.Toolbar.Dialog, null);
        Render();

        Complete();
    }
}