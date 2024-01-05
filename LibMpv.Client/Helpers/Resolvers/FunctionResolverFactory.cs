using System.Runtime.InteropServices;

namespace LibMpv.Client;

public enum LibMpvPlatformID 
{
    Win32NT = 1,
    Unix = 2,
    MacOSX = 3,
    Android = 4,
    Other = 10
}

public static class FunctionResolverFactory
{
    public static LibMpvPlatformID GetPlatformId()
    {
        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Win32NT:
                return LibMpvPlatformID.Win32NT;
            case PlatformID.Unix: //.netCore中mac会被替换为Unix
                {
                    var isAndroid = RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID"));
                    var isMacos = RuntimeInformation.IsOSPlatform(OSPlatform.Create("MACOS"));
                    return isAndroid  ? LibMpvPlatformID.Android :  (isMacos ? LibMpvPlatformID.MacOSX : LibMpvPlatformID.Unix);;
                }
            case PlatformID.MacOSX:
                return LibMpvPlatformID.MacOSX;
            default:
                return LibMpvPlatformID.Other;
        }
    }

    public static IFunctionResolver Create()
    {
        var os = System.Environment.OSVersion;
        switch (GetPlatformId())
        {
            case LibMpvPlatformID.MacOSX:
                return new MacFunctionResolver();
            case LibMpvPlatformID.Unix:
                return new LinuxFunctionResolver();
            case LibMpvPlatformID.Android:
                return new AndroidFunctionResolver();
            case LibMpvPlatformID.Win32NT:
                return new WindowsFunctionResolver();
            default:
                throw new PlatformNotSupportedException();
        }
    }
}
