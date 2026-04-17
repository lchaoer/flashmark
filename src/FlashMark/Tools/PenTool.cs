using Avalonia;
using Avalonia.Media;
using FlashMark.Models;

namespace FlashMark.Tools;

public class PenTool : ITool
{
    public ToolType Type => ToolType.Pen;

    public void OnPointerPressed(Point position, Stroke stroke)
    {
        stroke.Points.Add(position);
    }

    public void OnPointerMoved(Point position, Stroke stroke)
    {
        stroke.Points.Add(position);
    }

    public void OnPointerReleased(Point position, Stroke stroke)
    {
    }

    public void Render(DrawingContext context, Stroke stroke)
    {
        if (stroke.Points.Count < 2) return;

        var brush = new SolidColorBrush(stroke.Color, stroke.Opacity);
        var pen = new Pen(brush, stroke.Width,
            dashStyle: null,
            lineCap: PenLineCap.Round,
            lineJoin: PenLineJoin.Round);

        for (int i = 1; i < stroke.Points.Count; i++)
        {
            context.DrawLine(pen, stroke.Points[i - 1], stroke.Points[i]);
        }
    }
}
