using System;
using Avalonia.Controls;
using FlashMark.Platform;

namespace FlashMark.Views;

public partial class OverlayWindow : Window
{
    private WindowsHelper? _platformHelper;
    private IntPtr _windowHandle;

    public OverlayWindow()
    {
        InitializeComponent();
        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        var handle = this.TryGetPlatformHandle();
        if (handle != null)
        {
            _windowHandle = handle.Handle;
            _platformHelper = new WindowsHelper();
            _platformHelper.HideFromAltTab(_windowHandle);
            _platformHelper.RegisterHotkey(
                _windowHandle, 1,
                WindowsHelper.MOD_CONTROL | WindowsHelper.MOD_SHIFT,
                WindowsHelper.VK_D,
                ToggleActive
            );
        }

        SetActive(false);

        // Listen for state changes to keep UI in sync
        Canvas.State.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName is nameof(Canvas.State.CurrentTool) or nameof(Canvas.State.FadeMode))
                StatusBar.Update(Canvas.State.CurrentTool, Canvas.State.FadeMode);
        };
    }

    public void ToggleActive()
    {
        try
        {
            SetActive(!Canvas.State.IsActive);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ToggleActive error: {ex}");
        }
    }

    public void SetActive(bool active)
    {
        Canvas.State.IsActive = active;
        BorderGlow.IsGlowVisible = active;
        StatusBar.IsBarVisible = active;
        StatusBar.Update(Canvas.State.CurrentTool, Canvas.State.FadeMode);

        if (active)
        {
            WindowState = Avalonia.Controls.WindowState.Maximized;
            Topmost = true;
            _platformHelper?.HideFromAltTab(_windowHandle);
            Activate();
            Canvas.Focus();
        }
        else
        {
            Topmost = false;
            WindowState = Avalonia.Controls.WindowState.Normal;
            Position = new Avalonia.PixelPoint(-10000, -10000);
            Width = 1;
            Height = 1;
            _platformHelper?.HideFromAltTab(_windowHandle);
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        _platformHelper?.Dispose();
        base.OnClosed(e);
    }
}
