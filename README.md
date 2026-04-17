# FlashMark

**Draw. Fade. Focus.**

A lightweight screen annotation tool with auto-fading strokes. Built for remote meetings, teaching, and screen sharing.

## How It Works

```mermaid
stateDiagram-v2
    [*] --> Idle: Launch
    Idle --> Annotating: Ctrl+Shift+D
    Annotating --> Idle: Ctrl+Shift+D

    state Idle {
        [*] --> Background
        Background: Hidden in system tray, zero interference
    }

    state Annotating {
        [*] --> Draw
        Draw: Fullscreen overlay with blue glow border
        Draw: Left click to draw, strokes auto-fade
        Draw: Right click for pie menu
    }
```

## Features

| Feature | Description |
|---------|-------------|
| **Auto-fading** | Strokes fade away automatically (2s delay + 1s fade) |
| **5 Tools** | Pen, Arrow, Rectangle, Ellipse, Eraser |
| **5 Colors** | Red, Blue, Green, Yellow (default), White |
| **Pie Menu** | Right-click radial menu for quick tool/color switching |
| **Global Hotkey** | `Ctrl+Shift+D` toggles annotation mode from anywhere |
| **Visual Feedback** | Blue gradient glow border + status bar when active |
| **System Tray** | Runs silently in tray, right-click to toggle or exit |
| **Undo / Clear** | `Ctrl+Z` undo, `Ctrl+Shift+Z` clear all |
| **Stroke Width** | Scroll wheel to adjust (1px ~ 20px, default 5px) |
| **Stealth Mode** | Hidden from Alt+Tab, invisible when inactive |

## Keyboard Shortcuts

> Only work in annotation mode (after `Ctrl+Shift+D`)

| Shortcut | Action |
|----------|--------|
| `Ctrl+Shift+D` | Toggle annotation mode (**global**) |
| `Right Click` | Open Pie Menu |
| `Scroll Wheel` | Adjust stroke width |
| `Ctrl+Z` / `Ctrl+Shift+Z` | Undo / Clear all |
| `1` `2` `3` `4` `5` | Pen / Arrow / Rect / Ellipse / Eraser |
| `Q` `W` `E` `R` `T` | Red / Blue / Green / Yellow / White |

## Tech Stack

- C# / .NET 10
- Avalonia UI 12
- Skia-based 2D rendering
- Win32 P/Invoke (global keyboard hook, window styles)

## Build & Run

```bash
dotnet build
dotnet run --project src/FlashMark
```

## Roadmap

- [x] P0 — Windows annotation with auto-fade
- [ ] P1 — Number markers ①②③, magnifier, fade/permanent toggle
- [ ] P2 — macOS support, text annotations, spotlight, custom hotkeys

## License

MIT
