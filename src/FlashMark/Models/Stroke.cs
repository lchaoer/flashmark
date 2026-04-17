namespace FlashMark.Models;

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;

public class Stroke
{
    public List<Point> Points { get; set; } = new();
    public ToolType Tool { get; set; }
    public Color Color { get; set; } = Color.Parse("#FFBB33");
    public double Width { get; set; } = 5.0;
    public double Opacity { get; set; } = 1.0;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public FadeMode FadeMode { get; set; } = FadeMode.Fading;
    public bool IsComplete { get; set; }
}
