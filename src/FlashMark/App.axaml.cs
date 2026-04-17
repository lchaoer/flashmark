using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace FlashMark;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // TODO: 后续替换为 OverlayWindow
            desktop.MainWindow = new Avalonia.Controls.Window { Title = "FlashMark" };
        }

        base.OnFrameworkInitializationCompleted();
    }
}