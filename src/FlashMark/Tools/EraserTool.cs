using Avalonia;
using Avalonia.Media;
using FlashMark.Models;

namespace FlashMark.Tools;

public class EraserTool : ITool
{
    public ToolType Type => ToolType.Eraser;

    private const double HitThreshold = 15.0;
    private AppState? _state;

    public void SetState(AppState state) => _state = state;

    public void OnPointerPressed(Point position, Stroke stroke) => Erase(position);
    public void OnPointerMoved(Point position, Stroke stroke) => Erase(position);
    public void OnPointerReleased(Point position, Stroke stroke) { }
    public void Render(DrawingContext context, Stroke stroke) { }

    private void Erase(Point position)
    {
        if (_state == null) return;
        for (int i = _state.Strokes.Count - 1; i >= 0; i--)
        {
            var stroke = _state.Strokes[i];
            foreach (var pt in stroke.Points)
            {
                var dx = pt.X - position.X;
                var dy = pt.Y - position.Y;
                if (dx * dx + dy * dy < HitThreshold * HitThreshold)
                {
                    _state.Strokes.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
