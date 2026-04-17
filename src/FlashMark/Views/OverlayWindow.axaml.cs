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
            _platformHelper.RegisterHotkey(
                _windowHandle, 1,
                WindowsHelper.MOD_CONTROL | WindowsHelper.MOD_SHIFT,
                WindowsHelper.VK_D,
                ToggleActive
            );
        }

        // Start in active mode
        SetActive(true);

        // Listen for state changes to keep UI in sync
        Canvas.State.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName is nameof(Canvas.State.CurrentTool) or nameof(Canvas.State.FadeMode))
                StatusBar.Update(Canvas.State.CurrentTool, Canvas.State.FadeMode);
        };
    }

    private void ToggleActive()
    {
        SetActive(!Canvas.State.IsActive);
    }

    public void SetActive(bool active)
    {
        Canvas.State.IsActive = active;
        BorderGlow.IsGlowVisible = active;
        StatusBar.IsBarVisible = active;
        StatusBar.Update(Canvas.State.CurrentTool, Canvas.State.FadeMode);

        _platformHelper?.SetClickThrough(_windowHandle, !active);

        if (active)
            Canvas.Focus();
    }

    protected override void OnClosed(EventArgs e)
    {
        _platformHelper?.Dispose();
        base.OnClosed(e);
    }
}
