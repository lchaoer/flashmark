namespace FlashMark.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Media;

public class AppState : INotifyPropertyChanged
{
    private bool _isActive;
    private ToolType _currentTool = ToolType.Pen;
    private Color _currentColor = Color.Parse("#FFBB33");
    private double _currentWidth = 5.0;
    private FadeMode _fadeMode = FadeMode.Fading;

    public bool IsActive
    {
        get => _isActive;
        set { _isActive = value; OnPropertyChanged(); }
    }

    public ToolType CurrentTool
    {
        get => _currentTool;
        set { _currentTool = value; OnPropertyChanged(); }
    }

    public Color CurrentColor
    {
        get => _currentColor;
        set { _currentColor = value; OnPropertyChanged(); }
    }

    public double CurrentWidth
    {
        get => _currentWidth;
        set { _currentWidth = value; OnPropertyChanged(); }
    }

    public FadeMode FadeMode
    {
        get => _fadeMode;
        set { _fadeMode = value; OnPropertyChanged(); }
    }

    public List<Stroke> Strokes { get; } = new();

    public void Undo()
    {
        if (Strokes.Count > 0)
            Strokes.RemoveAt(Strokes.Count - 1);
    }

    public void ClearAll() => Strokes.Clear();
    public FadeConfig FadeConfig { get; set; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
