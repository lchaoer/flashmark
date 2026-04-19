# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

FlashMark — a lightweight screen annotation tool with auto-fading strokes. C# / .NET 10 / Avalonia UI 12. Windows-only (uses Win32 P/Invoke for global hotkey, Alt+Tab hiding, click-through).

## Build & Run

```bash
dotnet build
dotnet run --project src/FlashMark

# Portable exe (no .NET required to run)
dotnet publish src/FlashMark -c Release -r win-x64 --self-contained true \
  -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=full -o publish
```

Kill before rebuild: `taskkill /f /im FlashMark.exe`

No tests exist yet.

## Architecture

```
OverlayWindow (transparent fullscreen, move-offscreen when inactive)
  ├── DrawCanvas — core drawing surface, owns AppState, routes input to Tools
  │     ├── Tool system: ITool → PenTool, ArrowTool, ShapeTool, EraserTool
  │     └── PieMenu — right-click radial menu for tool/color switching
  ├── BorderGlow — multi-layer gradient glow border (active indicator)
  └── StatusBar — current tool/mode display

App.axaml.cs — TrayIcon setup (SkiaSharp-generated icon), ShutdownMode.OnExplicitShutdown
AppState — central state (INotifyPropertyChanged), owns stroke list
FadeEngine — DispatcherTimer at 60fps, handles opacity decay and stroke removal
WindowsHelper — Win32 P/Invoke: global keyboard hook, WS_EX_TOOLWINDOW, WS_EX_TRANSPARENT
```

## Key Design Decisions

- **Hit-test on transparent window**: Panel must have `Background="#01000000"` (alpha=1) — fully transparent surfaces don't receive pointer events in Avalonia.
- **Window hiding**: Move to (-10000,-10000) + resize 1x1. Hide()/Minimize causes Avalonia shutdown or Alt+Tab visibility issues. `WS_EX_TOOLWINDOW` must be reapplied after every WindowState change.
- **Global hotkey**: Uses low-level keyboard hook (`WH_KEYBOARD_LL`) instead of `RegisterHotKey` for reliability. Callback dispatches to UI thread via `Dispatcher.UIThread.Post`.
- **Platform abstraction**: `IPlatformHelper` interface exists but only `WindowsHelper` is implemented. macOS support requires AppKit/Cocoa interop (P2 roadmap).

## Code Conventions

- All code comments, strings, TODOs, and identifiers must be in English (no Chinese in code files).
- SkiaSharp is used only for tray icon generation, not for main drawing (which uses Avalonia's DrawingContext).
