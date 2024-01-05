using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";

    [ObservableProperty] private VideoViewModel v1 = new();
    [ObservableProperty] private VideoViewModel v2 = new();
    [ObservableProperty] private VideoViewModel v3 = new();
    [ObservableProperty] private VideoViewModel v4 = new();
    [ObservableProperty] private VideoViewModel v5 = new();
    [ObservableProperty] private VideoViewModel v6 = new();
    [ObservableProperty] private VideoViewModel v7 = new();
    [ObservableProperty] private VideoViewModel v8 = new();
    [ObservableProperty] private VideoViewModel v9 = new();
    [ObservableProperty] private VideoViewModel v10 = new();
    [ObservableProperty] private VideoViewModel v11 = new();
    [ObservableProperty] private VideoViewModel v12 = new();
    [ObservableProperty] private VideoViewModel v13 = new();
    [ObservableProperty] private VideoViewModel v14 = new();
    [ObservableProperty] private VideoViewModel v15 = new();
    [ObservableProperty] private VideoViewModel v16 = new();

    public MainViewModel()
    {
        V1.Play("rtsp://admin:Admin123@192.168.1.120:554/ch1/main/av_stream");
        V2.Play("rtsp://admin:Admin123@192.168.1.120:554/ch1/main/av_stream");
        V3.Play("rtsp://admin:Admin123@192.168.1.120:554/ch1/main/av_stream");
        V4.Play("rtsp://admin:Admin123@192.168.1.120:554/ch1/main/av_stream");
        V5.Play("rtsp://admin:Admin123@192.168.1.120:554/ch1/main/av_stream");
        V6.Play("rtsp://admin:Admin123@192.168.1.120:554/ch1/main/av_stream");
        V7.Play("rtsp://admin:Admin123@192.168.1.120:554/ch1/main/av_stream");
        V8.Play("rtsp://admin:Admin123@192.168.1.120:554/ch1/main/av_stream");
        V9.Play("rtsp://admin:Admin123@192.168.1.120:554/ch1/main/av_stream");
        V10.Play("rtsp://admin:Admin123@192.168.1.120:554/ch1/main/av_stream");
        V11.Play("rtsp://admin:Admin123@192.168.1.120:554/ch1/main/av_stream");
        V12.Play("rtsp://admin:Admin123@192.168.1.120:554/ch1/main/av_stream");
        V13.Play("rtsp://admin:Admin123@192.168.1.120:554/ch1/main/av_stream");
        V14.Play("rtsp://admin:Admin123@192.168.1.120:554/ch1/main/av_stream");
        V15.Play("rtsp://admin:Admin123@192.168.1.120:554/ch1/main/av_stream");
        V16.Play("rtsp://admin:Admin123@192.168.1.120:554/ch1/main/av_stream");
    }
}
