namespace FlashMark.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using FlashMark.Models;

public class DrawCanvas : Control
{
    private Stroke? _currentStroke;

    public AppState State { get; }

    public DrawCanvas()
    {
        State = new AppState { IsActive = true };
        ClipToBounds = true;
        Focusable = true;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        var point = e.GetCurrentPoint(this);
        if (!point.Properties.IsLeftButtonPressed) return;

        _currentStroke = new Stroke
        {
            Tool = State.CurrentTool,
            Color = State.CurrentColor,
            Width = State.CurrentWidth,
            FadeMode = State.FadeMode,
        };
        _currentStroke.Points.Add(point.Position);
        State.Strokes.Add(_currentStroke);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_currentStroke == null) return;

        _currentStroke.Points.Add(e.GetCurrentPoint(this).Position);
        InvalidateVisual();
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_currentStroke == null) return;

        _currentStroke.IsComplete = true;
        _currentStroke = null;
        e.Handled = true;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        foreach (var stroke in State.Strokes)
        {
            if (stroke.Points.Count < 2) continue;

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
}
