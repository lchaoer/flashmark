namespace FlashMark.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using FlashMark.Models;

public class StatusBar : Control
{
    private string _toolName = "Pen";
    private string _modeName = "Fading";
    private bool _isBarVisible;

    public bool IsBarVisible
    {
        get => _isBarVisible;
        set
        {
            _isBarVisible = value;
            IsVisible = value;
            InvalidateVisual();
        }
    }

    public StatusBar()
    {
        IsHitTestVisible = false;
        IsVisible = false;
    }

    public void Update(ToolType tool, FadeMode mode)
    {
        _toolName = tool.ToString();
        _modeName = mode == FadeMode.Fading ? "Fade 3s" : "Permanent";
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        if (!_isBarVisible) return;

        var text = new FormattedText(
            $"{_toolName} | {_modeName}",
            System.Globalization.CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface("Inter", FontStyle.Normal, FontWeight.Medium),
            14,
            Brushes.White
        );

        var padding = 10.0;
        var bgWidth = text.Width + padding * 2;
        var bgHeight = text.Height + padding * 2;
        var x = Bounds.Width - bgWidth;
        var y = Bounds.Height - bgHeight;

        var bgRect = new Rect(x, y, bgWidth, bgHeight);
        context.DrawRectangle(
            new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)),
            null,
            bgRect,
            8, 8
        );
        context.DrawText(text, new Point(x + padding, y + padding));
    }
}
