namespace FlashMark.Controls;

using System;
using Avalonia;
using Avalonia.Media;
using FlashMark.Models;

public class PieMenu
{
    private const double InnerRadius = 50;
    private const double OuterRadius = 100;
    private const double SectorAngle = 72; // 360 / 5

    // Outer ring: tools starting from top (270 degrees), clockwise
    private static readonly (ToolType Tool, string Label)[] Tools =
    {
        (ToolType.Pen, "Pen"),
        (ToolType.Arrow, "Arrow"),
        (ToolType.Rect, "Rect"),
        (ToolType.Ellipse, "Ellipse"),
        (ToolType.Eraser, "Eraser"),
    };

    // Inner ring: colors
    private static readonly (Color Color, string Label)[] ColorOptions =
    {
        (Color.Parse("#FF4444"), "Red"),
        (Color.Parse("#4488FF"), "Blue"),
        (Color.Parse("#44BB44"), "Green"),
        (Color.Parse("#FFBB33"), "Yellow"),
        (Colors.White, "White"),
    };

    public Point Center { get; set; }
    public bool IsOpen { get; set; }
    public Action<ToolType>? OnToolSelected { get; set; }
    public Action<Color>? OnColorSelected { get; set; }

    private Point _mousePos;

    public void UpdateMousePosition(Point pos)
    {
        _mousePos = pos;
    }

    /// <summary>
    /// Determine selection on release. Returns true if a selection was made.
    /// </summary>
    public bool ConfirmSelection()
    {
        var (ring, index) = GetHoveredSector();
        if (ring == Ring.Outer && index >= 0)
        {
            OnToolSelected?.Invoke(Tools[index].Tool);
            return true;
        }
        if (ring == Ring.Inner && index >= 0)
        {
            OnColorSelected?.Invoke(ColorOptions[index].Color);
            return true;
        }
        return false;
    }

    public void Render(DrawingContext context)
    {
        if (!IsOpen) return;

        var (hoveredRing, hoveredIndex) = GetHoveredSector();

        // Background overlay circle
        context.DrawEllipse(
            new SolidColorBrush(Color.FromArgb(180, 30, 30, 30)),
            null, Center, OuterRadius, OuterRadius);

        var strokePen = new Pen(Brushes.Gray, 1);

        // Draw outer ring sectors (tools)
        for (int i = 0; i < Tools.Length; i++)
        {
            bool hovered = hoveredRing == Ring.Outer && hoveredIndex == i;
            var fill = hovered
                ? new SolidColorBrush(Color.FromArgb(200, 80, 80, 120))
                : new SolidColorBrush(Color.FromArgb(140, 50, 50, 70));
            DrawSector(context, InnerRadius, OuterRadius, i, fill, strokePen);

            // Label
            var midAngle = GetMidAngle(i);
            var labelRadius = (InnerRadius + OuterRadius) / 2;
            var lx = Center.X + labelRadius * Math.Cos(midAngle);
            var ly = Center.Y + labelRadius * Math.Sin(midAngle);
            var ft = new FormattedText(Tools[i].Label, System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight, Typeface.Default, 12,
                hovered ? Brushes.White : new SolidColorBrush(Color.FromArgb(220, 200, 200, 200)));
            context.DrawText(ft, new Point(lx - ft.Width / 2, ly - ft.Height / 2));
        }

        // Draw inner ring sectors (colors)
        for (int i = 0; i < ColorOptions.Length; i++)
        {
            bool hovered = hoveredRing == Ring.Inner && hoveredIndex == i;
            var fill = hovered
                ? new SolidColorBrush(ColorOptions[i].Color)
                : new SolidColorBrush(Color.FromArgb(180,
                    ColorOptions[i].Color.R, ColorOptions[i].Color.G, ColorOptions[i].Color.B));
            DrawSector(context, 0, InnerRadius, i, fill, strokePen);
        }

        // Center dot
        context.DrawEllipse(new SolidColorBrush(Color.FromArgb(200, 60, 60, 60)), null, Center, 8, 8);
    }

    private void DrawSector(DrawingContext context, double rInner, double rOuter, int index,
        IBrush fill, IPen? pen)
    {
        double startDeg = -90 + index * SectorAngle; // start from top
        double endDeg = startDeg + SectorAngle;
        double startRad = startDeg * Math.PI / 180;
        double endRad = endDeg * Math.PI / 180;

        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            if (rInner < 1)
            {
                // Pie slice from center
                ctx.BeginFigure(Center, true);
                var p1 = new Point(Center.X + rOuter * Math.Cos(startRad),
                                   Center.Y + rOuter * Math.Sin(startRad));
                ctx.LineTo(p1, true);
                var p2 = new Point(Center.X + rOuter * Math.Cos(endRad),
                                   Center.Y + rOuter * Math.Sin(endRad));
                ctx.ArcTo(p2, new Size(rOuter, rOuter), 0, false, SweepDirection.Clockwise, true);
                ctx.LineTo(Center, true);
                ctx.EndFigure(true);
            }
            else
            {
                // Ring sector (annular)
                var outerStart = new Point(Center.X + rOuter * Math.Cos(startRad),
                                           Center.Y + rOuter * Math.Sin(startRad));
                var outerEnd = new Point(Center.X + rOuter * Math.Cos(endRad),
                                         Center.Y + rOuter * Math.Sin(endRad));
                var innerEnd = new Point(Center.X + rInner * Math.Cos(endRad),
                                         Center.Y + rInner * Math.Sin(endRad));
                var innerStart = new Point(Center.X + rInner * Math.Cos(startRad),
                                           Center.Y + rInner * Math.Sin(startRad));

                ctx.BeginFigure(outerStart, true);
                ctx.ArcTo(outerEnd, new Size(rOuter, rOuter), 0, false, SweepDirection.Clockwise, true);
                ctx.LineTo(innerEnd, true);
                ctx.ArcTo(innerStart, new Size(rInner, rInner), 0, false, SweepDirection.CounterClockwise, true);
                ctx.LineTo(outerStart, true);
                ctx.EndFigure(true);
            }
        }
        context.DrawGeometry(fill, pen, geometry);
    }

    private double GetMidAngle(int index)
    {
        double midDeg = -90 + index * SectorAngle + SectorAngle / 2;
        return midDeg * Math.PI / 180;
    }

    private (Ring ring, int index) GetHoveredSector()
    {
        double dx = _mousePos.X - Center.X;
        double dy = _mousePos.Y - Center.Y;
        double dist = Math.Sqrt(dx * dx + dy * dy);

        if (dist > OuterRadius) return (Ring.None, -1);

        // Angle from top, clockwise: convert atan2 to our sector system
        double angleDeg = Math.Atan2(dy, dx) * 180 / Math.PI; // -180..180
        angleDeg += 90; // rotate so top = 0
        if (angleDeg < 0) angleDeg += 360;
        int index = (int)(angleDeg / SectorAngle) % 5;

        if (dist <= InnerRadius)
            return (Ring.Inner, index);
        return (Ring.Outer, index);
    }

    private enum Ring { None, Inner, Outer }
}
