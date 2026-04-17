namespace FlashMark.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

public class BorderGlow : Control
{
    private bool _isGlowVisible;

    public bool IsGlowVisible
    {
        get => _isGlowVisible;
        set
        {
            _isGlowVisible = value;
            IsVisible = value;
            InvalidateVisual();
        }
    }

    public BorderGlow()
    {
        IsHitTestVisible = false;
        IsVisible = false;
    }

    public override void Render(DrawingContext context)
    {
        if (!_isGlowVisible) return;

        var bounds = new Rect(Bounds.Size);
        var pen = new Pen(
            new SolidColorBrush(Color.FromArgb(153, 255, 68, 68)), // #FF4444 at ~0.6 opacity
            4.0
        );
        context.DrawRectangle(null, pen, bounds.Deflate(2));
    }
}
