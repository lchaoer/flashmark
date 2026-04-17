namespace FlashMark.Controls;

using System;
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
    private readonly PieMenu _pieMenu;

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
        var eraserTool = new EraserTool();
        eraserTool.SetState(State);

        _tools = new Dictionary<ToolType, ITool>
        {
            [ToolType.Pen] = penTool,
            [ToolType.Arrow] = arrowTool,
            [ToolType.Line] = arrowTool,
            [ToolType.Rect] = shapeTool,
            [ToolType.Ellipse] = shapeTool,
            [ToolType.Eraser] = eraserTool,
        };

        _pieMenu = new PieMenu
        {
            OnToolSelected = tool => State.CurrentTool = tool,
            OnColorSelected = color => State.CurrentColor = color,
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

        if (point.Properties.IsRightButtonPressed)
        {
            _pieMenu.Center = point.Position;
            _pieMenu.IsOpen = true;
            _pieMenu.UpdateMousePosition(point.Position);
            InvalidateVisual();
            e.Handled = true;
            return;
        }

        if (!point.Properties.IsLeftButtonPressed || _pieMenu.IsOpen) return;

        _currentStroke = new Stroke
        {
            Tool = State.CurrentTool,
            Color = State.CurrentColor,
            Width = State.CurrentWidth,
            FadeMode = State.FadeMode,
        };
        _tools[_currentStroke.Tool].OnPointerPressed(point.Position, _currentStroke);
        if (State.CurrentTool != ToolType.Eraser)
            State.Strokes.Add(_currentStroke);
        InvalidateVisual();
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (_pieMenu.IsOpen)
        {
            _pieMenu.UpdateMousePosition(e.GetCurrentPoint(this).Position);
            InvalidateVisual();
            e.Handled = true;
            return;
        }

        if (_currentStroke == null) return;

        _tools[_currentStroke.Tool].OnPointerMoved(e.GetCurrentPoint(this).Position, _currentStroke);
        InvalidateVisual();
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (_pieMenu.IsOpen && e.InitialPressMouseButton == MouseButton.Right)
        {
            _pieMenu.UpdateMousePosition(e.GetCurrentPoint(this).Position);
            _pieMenu.ConfirmSelection();
            _pieMenu.IsOpen = false;
            InvalidateVisual();
            e.Handled = true;
            return;
        }

        if (_currentStroke == null) return;

        _tools[_currentStroke.Tool].OnPointerReleased(e.GetCurrentPoint(this).Position, _currentStroke);
        if (_currentStroke.Tool != ToolType.Eraser)
            _currentStroke.IsComplete = true;
        _currentStroke = null;
        e.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Z && e.KeyModifiers.HasFlag(KeyModifiers.Control) && e.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            State.ClearAll();
            InvalidateVisual();
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Z && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            State.Undo();
            InvalidateVisual();
            e.Handled = true;
            return;
        }

        switch (e.Key)
        {
            case Key.D1: State.CurrentTool = ToolType.Pen; e.Handled = true; break;
            case Key.D2: State.CurrentTool = ToolType.Arrow; e.Handled = true; break;
            case Key.D3: State.CurrentTool = ToolType.Rect; e.Handled = true; break;
            case Key.D4: State.CurrentTool = ToolType.Ellipse; e.Handled = true; break;
            case Key.D5: State.CurrentTool = ToolType.Eraser; e.Handled = true; break;
        }

        var colors = new Dictionary<Key, Color>
        {
            [Key.Q] = Color.Parse("#FF4444"),
            [Key.W] = Color.Parse("#4488FF"),
            [Key.E] = Color.Parse("#44BB44"),
            [Key.R] = Color.Parse("#FFBB33"),
            [Key.T] = Colors.White,
        };
        if (colors.TryGetValue(e.Key, out var color))
        {
            State.CurrentColor = color;
            e.Handled = true;
        }
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        var delta = e.Delta.Y > 0 ? 1.0 : -1.0;
        State.CurrentWidth = Math.Clamp(State.CurrentWidth + delta, 1.0, 20.0);
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

        _pieMenu.Render(context);
    }
}
