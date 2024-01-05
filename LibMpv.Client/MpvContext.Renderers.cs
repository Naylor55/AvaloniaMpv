﻿using System.Runtime.InteropServices;

namespace LibMpv.Client;

public unsafe partial class MpvContext
{
    public void ConfigureRenderer(IMpvRendererConfiguration rendererConfiguration)
    {
        if (disposed || configuration != null || rendererConfiguration == null)
            return;

        if (rendererConfiguration is NativeRendererConfiguration nativeRendererConfiguration)
        {
            ConfigureNativeRenderer(nativeRendererConfiguration);
            return;
        }
        else if (rendererConfiguration is OpenGlRendererConfiguration glRendererConfiguartion)
        {
            Console.WriteLine($"ConfigureOpenGlRenderer:{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffffff}");
            ConfigureOpenGlRenderer(glRendererConfiguartion);
            return;
        }
        else if (rendererConfiguration is SoftwareRendererConfiguartion softwareRendererConfiguartion)
        {
            ConfigureSoftwareRenderer(softwareRendererConfiguartion);
            return;
        }

        throw new MpvException($"Not supported renderer {rendererConfiguration.MpvRenderer}");
    }

    public void RenderOpenGl(int width, int height, int fb = 0, int flipY = 0)
    {
        if (disposed || renderContext == null) return;

        var fboArray = new MpvOpenglFbo[1]
        {
            new()
            {
                W = width,
                H = height,
                Fbo = fb
            }
        };

        var flipYArray = new int[1] { flipY };

        int result = 0;
        fixed (MpvOpenglFbo* fboArrayPtr = fboArray)
        {
            fixed (int* flipYArrayPtr = flipYArray)
            {
                var parameters = new MpvRenderParam[]
                {
                    new()
                    {
                        Type = MpvRenderParamType.MpvRenderParamOpenglFbo,
                        Data = (byte*)fboArrayPtr
                    },
                    new()
                    {
                        Type = MpvRenderParamType.MpvRenderParamFlipY,
                        Data = (byte*)flipYArrayPtr
                    },
                    new()
                    {
                        Type = MpvRenderParamType.MpvRenderParamInvalid
                    },
                };
                fixed (MpvRenderParam* parametersPtr = parameters)
                {
                    result = LibMpv.MpvRenderContextRender(renderContext, parametersPtr);
                }
            }
        }

        if (result < 0) this.ThrowMpvError(result);
    }

    public void RenderBitmap(int width, int height, nint surfaceAddress, string format)
    {
        if (disposed || renderContext == null) return;

        using MarshalHelper marshalHelper = new MarshalHelper();

        var size = new int[2] { width, height };
        var stride = new uint[1] { (uint)width * 4 };

        int result = 0;
        fixed (int* sizePtr = size)
        {
            fixed (uint* stridePtr = stride)
            {
                var parameters = new MpvRenderParam[]
                {
                    new()
                    {
                        Type = MpvRenderParamType.MpvRenderParamSwSize,
                        Data = sizePtr
                    },
                    new()
                    {
                        Type = MpvRenderParamType.MpvRenderParamSwFormat,
                        Data = (void*)marshalHelper.CStringFromManagedUTF8String(format)
                    },
                    new()
                    {
                        Type = MpvRenderParamType.MpvRenderParamSwStride,
                        Data = stridePtr
                    },
                    new()
                    {
                        Type = MpvRenderParamType.MpvRenderParamSwPointer,
                        Data = (void*)surfaceAddress
                    },
                    new()
                    {
                        Type = MpvRenderParamType.MpvRenderParamInvalid,
                        Data = null
                    }
                };

                fixed (MpvRenderParam* parametersPtr = parameters)
                {
                    result = LibMpv.MpvRenderContextRender(renderContext, parametersPtr);
                }
            }
        }

        if (result < 0) this.ThrowMpvError(result);
    }


    private void ConfigureOpenGlRenderer(OpenGlRendererConfiguration glRendererConfiguartion)
    {
        if (disposed || glRendererConfiguartion.OpnGlGetProcAddress == null ||
            glRendererConfiguartion.UpdateCallback == null) return;
        this.StopRendering();
        Console.WriteLine("ConfigureOpenGlRenderer 1");
        this.glGetProcAddress = (_, name) => (void*)glRendererConfiguartion.OpnGlGetProcAddress(name);
        this.updateCallback =
            new MpvRenderContextSetUpdateCallbackCallback((_) => glRendererConfiguartion.UpdateCallback());
        Console.WriteLine("ConfigureOpenGlRenderer 2");

        using MarshalHelper marshalHelper = new MarshalHelper();
        Console.WriteLine("ConfigureOpenGlRenderer 3");
        var parameters = new MpvRenderParam[]
        {
            new()
            {
                Type = MpvRenderParamType.MpvRenderParamApiType,
                Data = (void*)marshalHelper.StringToHGlobalAnsi(LibMpv.MpvRenderApiTypeOpengl)
            },
            new()
            {
                Type = MpvRenderParamType.MpvRenderParamOpenglInitParams,
                Data = (void*)marshalHelper.AllocHGlobal<MpvOpenglInitParams>(new MpvOpenglInitParams()
                {
                    GetProcAddress = this.glGetProcAddress,
                    GetProcAddressCtx = null
                })
            },
            new()
            {
                Type = MpvRenderParamType.MpvRenderParamAdvancedControl,
                Data = (void*)marshalHelper.AllocHGlobalValue(0)
            },
            new()
            {
                Type = MpvRenderParamType.MpvRenderParamInvalid,
                Data = null
            }
        };
        Console.WriteLine("ConfigureOpenGlRenderer 4");

        int result = 0;
        Console.WriteLine("ConfigureOpenGlRenderer 41");

        Console.WriteLine("ConfigureOpenGlRenderer 42");
        MpvRenderContext* contextPtr = null;
        fixed (MpvRenderParam* parametersPtr = parameters)
        {
            Console.WriteLine($"ConfigureOpenGlRenderer 43 {(int)handle} {(int)&contextPtr} {(int)parametersPtr} {DateTime.Now:yyyy-MM-dd HH:mm:ss.ffffff}");
            result = LibMpv.MpvRenderContextCreate(&contextPtr, handle, parametersPtr);

            Console.WriteLine("ConfigureOpenGlRenderer 44");
        }


        Console.WriteLine("ConfigureOpenGlRenderer 5");

        if (result < 0) this.ThrowMpvError(result);
        this.renderContext = contextPtr;
        Console.WriteLine("ConfigureOpenGlRenderer 6");

        LibMpv.MpvRenderContextSetUpdateCallback(this.renderContext, this.updateCallback, null);
        Console.WriteLine("ConfigureOpenGlRenderer 7");
    }

    private void ConfigureSoftwareRenderer(SoftwareRendererConfiguartion softwareRendererConfiguartion)
    {
        if (softwareRendererConfiguartion.UpdateCallback == null) return;

        this.updateCallback =
            new MpvRenderContextSetUpdateCallbackCallback((_) => softwareRendererConfiguartion.UpdateCallback());

        using MarshalHelper marshalHelper = new MarshalHelper();

        var parameters = new MpvRenderParam[]
        {
            new()
            {
                Type = MpvRenderParamType.MpvRenderParamApiType,
                Data = (void*)marshalHelper.StringToHGlobalAnsi(LibMpv.MpvRenderApiTypeSw)
            },
            new()
            {
                Type = MpvRenderParamType.MpvRenderParamAdvancedControl,
                Data = (void*)marshalHelper.AllocHGlobalValue(0)
            },
            new()
            {
                Type = MpvRenderParamType.MpvRenderParamInvalid,
                Data = null
            }
        };

        int result = 0;
        MpvRenderContext* contextPtr = null;
        fixed (MpvRenderParam* parametersPtr = parameters)
        {
            result = LibMpv.MpvRenderContextCreate((MpvRenderContext**)&contextPtr, handle, parametersPtr);
        }

        if (result < 0) this.ThrowMpvError(result);
        this.renderContext = contextPtr;
        LibMpv.MpvRenderContextSetUpdateCallback(this.renderContext, this.updateCallback, null);
    }

    private void ConfigureNativeRenderer(NativeRendererConfiguration nativeRendererConfiguration)
    {
        if (nativeRendererConfiguration.WindowHandle == IntPtr.Zero)
            throw new MpvException("Invalid window handle");

        Console.WriteLine("wid");
        SetPropertyLong("wid", (long)nativeRendererConfiguration.WindowHandle);
        Console.WriteLine("wid1");

        this.configuration = nativeRendererConfiguration;
    }

    public void StopRendering()
    {
        if (renderContext != null)
        {
            Console.WriteLine("StopRendering");
            LibMpv.MpvRenderContextFree(renderContext);
            renderContext = null;
        }
        else
        {
            Console.WriteLine("wid111-0");
            this.SetPropertyLong("wid", 0);
            Console.WriteLine("wid111-1");
        }
    }

    private MpvRenderContextSetUpdateCallbackCallback updateCallback;
    private MpvOpenglInitParamsGetProcAddress glGetProcAddress;
}

public delegate nint OpenGlGetProcAddressDelegate(string name);

public delegate void UpdateCallbackDelegate();

public enum MpvRenderer
{
    Native,
    OpenGl,
    Software
}

public interface IMpvRendererConfiguration
{
    public MpvRenderer MpvRenderer { get; }
}

public class NativeRendererConfiguration : IMpvRendererConfiguration
{
    public IntPtr WindowHandle { get; set; } = IntPtr.Zero;
    public MpvRenderer MpvRenderer => MpvRenderer.Native;
}

public class SoftwareRendererConfiguartion : IMpvRendererConfiguration
{
    public UpdateCallbackDelegate UpdateCallback { get; set; } = null;

    public MpvRenderer MpvRenderer => MpvRenderer.Software;
}

public class OpenGlRendererConfiguration : IMpvRendererConfiguration
{
    public UpdateCallbackDelegate UpdateCallback { get; set; } = null;

    public OpenGlGetProcAddressDelegate OpnGlGetProcAddress { get; set; } = null;

    public MpvRenderer MpvRenderer => MpvRenderer.OpenGl;
}