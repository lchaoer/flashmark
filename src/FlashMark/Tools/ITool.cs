using Avalonia;
using Avalonia.Media;
using FlashMark.Models;

namespace FlashMark.Tools;

public interface ITool
{
    ToolType Type { get; }
    void OnPointerPressed(Point position, Stroke stroke);
    void OnPointerMoved(Point position, Stroke stroke);
    void OnPointerReleased(Point position, Stroke stroke);
    void Render(DrawingContext context, Stroke stroke);
}
