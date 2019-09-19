using System;
using System.IO;
using System.Linq;
using Xplorer.FileSystems;
using Xplorer.OperationSystems;

namespace Xplorer
{
    public class Application
    {
        public static void Main(string[] args)
        {
            var application = new Application();
            application.Run();
        }

        internal static string NavUpControlName { get; } = "..";

        private IFileSystem FileSystem { get; }
        private IOperationSystem OperationSystem { get; }
        private ITheme Theme { get; }
        private Renderer Renderer { get; }
        private Context Context { get; }

        private Application()
        {
            Context = new Context()
            {
                Filter = string.Empty
            };

            FileSystem = new FileSystem();
            OperationSystem = new WindowsOperationSystem(Context);
            Renderer = new Renderer(Context);
        }

        public void Run()
        {
            Console.CancelKeyPress += (sender, args) =>
            {
                Console.Clear();
                Console.CursorVisible = true;
                Console.SetCursorPosition(0, 0);
                Environment.Exit(0);
            };

            Console.Clear();
            Console.CursorVisible = false;
            
            FileSystem.Navigate(null);
            Context.Location = FileSystem.Location;
            Context.Entries = FileSystem.Entries.ToList();

            InsertNavUpControl();

            Renderer.Render();

            while (true)
            {
                var input = Console.ReadKey(true);

                if (input.Key == ConsoleKey.Enter && input.Modifiers == ConsoleModifiers.Control)
                {
                    OperationSystem.Reveal();
                    continue;
                }

                switch (input.Key)
                {
                    case ConsoleKey.DownArrow: MoveDown(); break;
                    case ConsoleKey.UpArrow: MoveUp(); break;
                    case ConsoleKey.LeftArrow: MoveLeft(); break;
                    case ConsoleKey.RightArrow: MoveRight(); break;
                    case ConsoleKey.Enter: Navigate(); break;
                    case ConsoleKey.Escape: Escape(); break;
                    case ConsoleKey.Backspace: Back(); break;
                }

                if (char.IsLetterOrDigit(input.KeyChar))
                {
                    ApplyFilter(input.KeyChar);
                }
            }
        }

        private void Navigate()
        {
            var entry = Context.Entries[Context.ActiveIndex];
            var location = Context.Location;
            
            try
            {
                FileSystem.Navigate(entry.Type == NavigationEntryType.NavUpControl ? null : entry.Name);
            }
            catch (Exception ex)
            {
                Renderer.RenderStatus(ex.Message);
                return;
            }

            if (entry.IsExecutable())
            {
                return;
            }
            
            Context.Location = FileSystem.Location;
            Context.Entries = FileSystem.Entries.ToList();

            InsertNavUpControl();

            Context.ActiveIndex = 0;

            if (entry.Type == NavigationEntryType.NavUpControl)
            {
                var name = Path.GetFileName(location);
                entry = Context.Entries.SingleOrDefault(x => x.Name == name);
                if (entry != null)
                {
                    Context.ActiveIndex = Context.Entries.IndexOf(entry);
                }
            }

            Context.Filter = string.Empty;

            Renderer.Render();
        }

        private void NavigateUp()
        {
            if (Context.Entries.Count > 0 && Context.Entries[0].Type == NavigationEntryType.NavUpControl)
            {
                Context.ActiveIndex = 0;
                Navigate();
            }
        }

        private void Escape()
        {
            if (Context.Filter == string.Empty)
            {
                NavigateUp();
            }
            else
            {
                ClearFilter();
            }
        }

        private void Back()
        {
            if (Context.Filter == string.Empty)
            {
                NavigateUp();
            }
            else
            {
                ReduceFilter();
            }
        }

        private void MoveDown()
        {
            if (Context.ActiveIndex == Context.Entries.Count - 1)
            {
                return;
            }

            Context.ActiveIndex++;

            Renderer.Render();
        }

        private void MoveUp()
        {
            if (Context.ActiveIndex == 0)
            {
                return;
            }
            
            Context.ActiveIndex--;

            Renderer.Render();
        }

        private void MoveLeft()
        {
            var index = Context.ActiveIndex;
            Context.ActiveIndex = 0;

            Renderer.Render();
        }

        private void MoveRight()
        {
            var index = Context.ActiveIndex;
            Context.ActiveIndex = Context.Entries.Count == 0 ? 0 : Context.Entries.Count - 1;

            Renderer.Render();
        }

        private void InsertNavUpControl()
        {
            if (Context.Location != null)
            {
                Context.Entries.Insert(0, new NavigationEntry(NavUpControlName, NavigationEntryType.NavUpControl));
            }
        }

        private void ApplyFilter(Char value)
        {
            Context.Filter += value;
            Context.Entries = Context.Entries.Where(x => x.Name.Contains(Context.Filter, StringComparison.OrdinalIgnoreCase)).ToList();
            Context.Entries.Insert(0, new NavigationEntry(NavUpControlName, NavigationEntryType.NavUpControl));
            Context.ActiveIndex = 0;

            Renderer.Render();
        }

        private void ReduceFilter()
        {
            if (Context.Filter == string.Empty)
            {
                return;
            }

            Context.Filter = Context.Filter.Substring(0, Context.Filter.Length - 1);
            Context.Entries = Context.Entries.Where(x => x.Name.Contains(Context.Filter, StringComparison.OrdinalIgnoreCase)).ToList();
            Context.Entries.Insert(0, new NavigationEntry(NavUpControlName, NavigationEntryType.NavUpControl));
            Context.ActiveIndex = 0;

            Renderer.Render();
        }

        private void ClearFilter()
        {
            if (Context.Filter == string.Empty)
            {
                return;
            }

            Context.Filter = string.Empty;
            Context.Entries = Context.Entries.ToList();
            Context.Entries.Insert(0, new NavigationEntry(NavUpControlName, NavigationEntryType.NavUpControl));
            Context.ActiveIndex = 0;

            Renderer.Render();
        }
    }
}