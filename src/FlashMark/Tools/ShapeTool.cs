using System;
using Avalonia;
using Avalonia.Media;
using FlashMark.Models;

namespace FlashMark.Tools;

public class ShapeTool : ITool
{
    public ToolType Type => ToolType.Rect;

    public void OnPointerPressed(Point position, Stroke stroke)
    {
        stroke.Points.Add(position);
    }

    public void OnPointerMoved(Point position, Stroke stroke)
    {
        if (stroke.Points.Count > 1)
            stroke.Points[1] = position;
        else
            stroke.Points.Add(position);
    }

    public void OnPointerReleased(Point position, Stroke stroke)
    {
    }

    public void Render(DrawingContext context, Stroke stroke)
    {
        if (stroke.Points.Count < 2) return;

        var p0 = stroke.Points[0];
        var p1 = stroke.Points[1];

        var brush = new SolidColorBrush(stroke.Color, stroke.Opacity);
        var pen = new Pen(brush, stroke.Width,
            dashStyle: null,
            lineCap: PenLineCap.Round,
            lineJoin: PenLineJoin.Round);

        var rect = new Rect(
            Math.Min(p0.X, p1.X), Math.Min(p0.Y, p1.Y),
            Math.Abs(p1.X - p0.X), Math.Abs(p1.Y - p0.Y));

        if (stroke.Tool == ToolType.Ellipse)
            context.DrawEllipse(null, pen, rect.Center, rect.Width / 2, rect.Height / 2);
        else
            context.DrawRectangle(null, pen, rect);
    }
}
