namespace FlashMark.Controls;

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using FlashMark.Models;
using FlashMark.Services;
using FlashMark.Tools;

public class DrawCanvas : Control
{
    private Stroke? _currentStroke;
    private readonly FadeEngine _fadeEngine;
    private readonly Dictionary<ToolType, ITool> _tools;

    public AppState State { get; }

    public DrawCanvas()
    {
        State = new AppState { IsActive = true };
        _fadeEngine = new FadeEngine(State, InvalidateVisual);
        ClipToBounds = true;
        Focusable = true;

        var penTool = new PenTool();
        var arrowTool = new ArrowTool();
        var shapeTool = new ShapeTool();

        _tools = new Dictionary<ToolType, ITool>
        {
            [ToolType.Pen] = penTool,
            [ToolType.Arrow] = arrowTool,
            [ToolType.Line] = arrowTool,
            [ToolType.Rect] = shapeTool,
            [ToolType.Ellipse] = shapeTool,
        };
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _fadeEngine.Start();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _fadeEngine.Stop();
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
        _tools[_currentStroke.Tool].OnPointerPressed(point.Position, _currentStroke);
        State.Strokes.Add(_currentStroke);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_currentStroke == null) return;

        _tools[_currentStroke.Tool].OnPointerMoved(e.GetCurrentPoint(this).Position, _currentStroke);
        InvalidateVisual();
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_currentStroke == null) return;

        _tools[_currentStroke.Tool].OnPointerReleased(e.GetCurrentPoint(this).Position, _currentStroke);
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
            _tools[stroke.Tool].Render(context, stroke);
        }
    }
}
