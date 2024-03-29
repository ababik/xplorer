using System.Runtime.InteropServices;
using Xplorer.Components;
using Xplorer.FileSystems;
using Xplorer.Models;
using Xplorer.OperationSystems;
using Xplorer.States;
using Xplorer.Themes;

namespace Xplorer;

public class Context
{
    public IFileSystem PrimaryFileSystem { get; }
    public IFileSystem SecondaryFileSystem { get; }
    public IOperationSystem OperationSystem { get; }
    public ITheme Theme { get; }
    private OSPlatform Platform { get; }
    public Model Model { get; set; }
    public MasterState State { get; set; }
    public MasterComponent Component { get; set; }

    public Context()
    {
        Platform = DetectPlatform();
        PrimaryFileSystem = CreateFileSystem();
        SecondaryFileSystem = CreateFileSystem();
        OperationSystem = CreateOperationSystem();
        Theme = CreateTheme();
    }

    private static OSPlatform DetectPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return OSPlatform.Windows;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return OSPlatform.OSX;
        }

        return OSPlatform.Linux;
    }

    private IFileSystem CreateFileSystem()
    {
        return new FileSystem();
    }

    private IOperationSystem CreateOperationSystem()
    {
        if (Platform == OSPlatform.Windows)
        {
            return new WindowsOperationSystem();
        }

        if (Platform == OSPlatform.OSX)
        {
            return new OsxOperationSystem();
        }

        return new LinuxOperationSystem();
    }

    private ITheme CreateTheme()
    {
        if (RevertableTheme.IsSupported)
        {
            return new RevertableTheme();
        }

        return new TerminalTheme();
    }
}