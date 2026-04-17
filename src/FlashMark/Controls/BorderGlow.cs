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
        double radius = 16;

        // Soft outer glow — 4 layers simulate blur
        for (int i = 5; i >= 1; i--)
        {
            double t = i / 5.0;
            byte alpha = (byte)(35 * t * t);
            double width = 1.0 + i * 1.5;
            double deflate = 2 + i * 1.8;

            var glowBrush = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromArgb(alpha, 80, 120, 255), 0),
                    new GradientStop(Color.FromArgb(alpha, 60, 200, 250), 0.25),
                    new GradientStop(Color.FromArgb(alpha, 120, 80, 255), 0.5),
                    new GradientStop(Color.FromArgb(alpha, 60, 200, 250), 0.75),
                    new GradientStop(Color.FromArgb(alpha, 80, 120, 255), 1.0),
                },
                SpreadMethod = GradientSpreadMethod.Reflect,
            };
            var glowPen = new Pen(glowBrush, width);
            context.DrawRectangle(null, glowPen, bounds.Deflate(deflate), radius, radius);
        }

        // Core bright border
        var coreBrush = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.FromArgb(200, 80, 140, 255), 0),
                new GradientStop(Color.FromArgb(220, 60, 210, 250), 0.25),
                new GradientStop(Color.FromArgb(200, 130, 90, 255), 0.5),
                new GradientStop(Color.FromArgb(220, 60, 210, 250), 0.75),
                new GradientStop(Color.FromArgb(200, 80, 140, 255), 1.0),
            },
            SpreadMethod = GradientSpreadMethod.Reflect,
        };
        var corePen = new Pen(coreBrush, 1.5);
        context.DrawRectangle(null, corePen, bounds.Deflate(3), radius, radius);
    }
}
