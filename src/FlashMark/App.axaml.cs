using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using FlashMark.Views;
using System;
using System.IO;
using SkiaSharp;

namespace FlashMark;

public partial class App : Application
{
    private OverlayWindow? _overlay;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            _overlay = new OverlayWindow();
            desktop.MainWindow = _overlay;

            var trayIcon = new TrayIcon
            {
                ToolTipText = "FlashMark",
                Icon = CreateTrayIcon(),
                Menu = new NativeMenu
                {
                    new NativeMenuItem("Toggle (Ctrl+Shift+D)")
                    {
                        Command = new RelayCommand(() => _overlay.ToggleActive())
                    },
                    new NativeMenuItemSeparator(),
                    new NativeMenuItem("Exit")
                    {
                        Command = new RelayCommand(() =>
                        {
                            _overlay.Close();
                            desktop.Shutdown();
                        })
                    }
                }
            };

            var icons = TrayIcon.GetIcons(this) ?? new TrayIcons();
            icons.Add(trayIcon);
            TrayIcon.SetIcons(this, icons);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static WindowIcon CreateTrayIcon()
    {
        const int size = 32;
        using var surface = SKSurface.Create(new SKImageInfo(size, size, SKColorType.Rgba8888, SKAlphaType.Premul));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        using var paint = new SKPaint { IsAntialias = true };

        // Blue circle background
        paint.Color = new SKColor(68, 136, 255, 220);
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawCircle(size / 2f, size / 2f, 13, paint);

        // White pen stroke icon
        paint.Color = SKColors.White;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 2.5f;
        paint.StrokeCap = SKStrokeCap.Round;
        using var path = new SKPath();
        path.MoveTo(10, 22);
        path.QuadTo(16, 10, 22, 14);
        canvas.DrawPath(path, paint);

        // Pen tip dot
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawCircle(10, 22, 1.5f, paint);

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        var ms = new MemoryStream(data.ToArray());
        return new WindowIcon(ms);
    }
}

internal class RelayCommand : System.Windows.Input.ICommand
{
    private readonly Action _action;
    public RelayCommand(Action action) => _action = action;
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => _action();
}
