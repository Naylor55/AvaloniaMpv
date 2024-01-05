using System;
using System.Runtime.InteropServices;
using Avalonia;
using LibMpv.Client;

namespace AvaloniaApplication1.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        InitMpv();
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();


    public static void InitMpv()
    {
        var platformId = FunctionResolverFactory.GetPlatformId();
        if (platformId == LibMpvPlatformID.Win32NT)
        {
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mpv", "windows");
#if DEBUG
            path =
                @"D:\Code\AvaloniaMpv\AvaloniaApplication1.Desktop\mpv\windows\";
#endif
            Console.WriteLine("MPVPATH:" + path);
            LibMpv.Client.LibMpv.UseLibMpv(2).UseLibraryPath(path);
        }
        else if (platformId == LibMpvPlatformID.Unix)
        {
            //var path = $"/usr/lib/x86_64-linux-gnu";
            //var path = $"/usr/local/lib";
            //LibMpv.Client.LibMpv.UseLibMpv(1).UseLibraryPath(path);
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mpv", "liunx");
            Console.WriteLine("MPVPATH:" + path);
            LibMpv.Client.LibMpv.UseLibMpv(1).UseLibraryPath(path);
        }
        else if (platformId == LibMpvPlatformID.MacOSX)
        {
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mpv",
                "macos", RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? "arm64" : "amd64");
            Console.WriteLine("MPVPATH:" + path);
            LibMpv.Client.LibMpv.UseLibMpv(0).UseLibraryPath(path);
        }
    }
}
