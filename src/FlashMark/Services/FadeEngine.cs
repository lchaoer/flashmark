using Avalonia.Threading;
using FlashMark.Models;
using System;

namespace FlashMark.Services;

public class FadeEngine
{
    private readonly AppState _state;
    private readonly Action _invalidate;
    private DispatcherTimer? _timer;

    public FadeEngine(AppState state, Action invalidate)
    {
        _state = state;
        _invalidate = invalidate;
    }

    public void Start()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        _timer.Tick += OnTick;
        _timer.Start();
    }

    public void Stop() => _timer?.Stop();

    private void OnTick(object? sender, EventArgs e)
    {
        var now = DateTime.Now;
        bool needsRedraw = false;
        var config = _state.FadeConfig;

        for (int i = _state.Strokes.Count - 1; i >= 0; i--)
        {
            var stroke = _state.Strokes[i];
            if (!stroke.IsComplete || stroke.FadeMode == FadeMode.Permanent) continue;

            var elapsed = (now - stroke.CreatedAt).TotalMilliseconds;
            if (elapsed < config.DelayMs) continue;

            var fadeElapsed = elapsed - config.DelayMs;
            if (fadeElapsed >= config.DurationMs)
            {
                _state.Strokes.RemoveAt(i);
                needsRedraw = true;
            }
            else
            {
                var newOpacity = 1.0 - (fadeElapsed / config.DurationMs);
                if (Math.Abs(stroke.Opacity - newOpacity) > 0.01)
                {
                    stroke.Opacity = newOpacity;
                    needsRedraw = true;
                }
            }
        }

        if (needsRedraw) _invalidate();
    }
}
