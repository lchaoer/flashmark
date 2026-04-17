using System;
using Avalonia;
using Avalonia.Media;
using FlashMark.Models;

namespace FlashMark.Tools;

public class ArrowTool : ITool
{
    public ToolType Type => ToolType.Arrow;

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

        var start = stroke.Points[0];
        var end = stroke.Points[1];

        var brush = new SolidColorBrush(stroke.Color, stroke.Opacity);
        var pen = new Pen(brush, stroke.Width,
            dashStyle: null,
            lineCap: PenLineCap.Round,
            lineJoin: PenLineJoin.Round);

        // Draw the line
        context.DrawLine(pen, start, end);

        // Draw arrowhead
        double headSize = stroke.Width * 4;
        double angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
        double a1 = angle + Math.PI * 5 / 6;
        double a2 = angle - Math.PI * 5 / 6;

        var p1 = new Point(end.X + headSize * Math.Cos(a1), end.Y + headSize * Math.Sin(a1));
        var p2 = new Point(end.X + headSize * Math.Cos(a2), end.Y + headSize * Math.Sin(a2));

        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            ctx.BeginFigure(end, true);
            ctx.LineTo(p1);
            ctx.LineTo(p2);
            ctx.EndFigure(true);
        }

        context.DrawGeometry(brush, null, geometry);
    }
}
