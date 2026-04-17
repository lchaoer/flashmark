using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Threading;

namespace FlashMark.Platform;

public class WindowsHelper : IPlatformHelper, IDisposable
{
    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TRANSPARENT = 0x00000020;
    private const int WS_EX_TOOLWINDOW = 0x00000080;
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr GetModuleHandle(string? lpModuleName);

    [StructLayout(LayoutKind.Sequential)]
    private struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    private IntPtr _hookHandle = IntPtr.Zero;
    private LowLevelKeyboardProc? _hookProc;
    private Action? _hotkeyCallback;
    private uint _targetModifiers;
    private uint _targetKey;

    // Modifier constants matching Win32 MOD_ values
    public const uint MOD_CONTROL = 0x0002;
    public const uint MOD_SHIFT = 0x0004;

    // Virtual key codes
    public const uint VK_D = 0x44;

    public void RegisterHotkey(IntPtr windowHandle, int id, uint modifiers, uint key, Action callback)
    {
        _targetModifiers = modifiers;
        _targetKey = key;
        _hotkeyCallback = callback;

        _hookProc = HookCallback;
        using var process = Process.GetCurrentProcess();
        using var module = process.MainModule!;
        _hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, _hookProc, GetModuleHandle(module.ModuleName), 0);
    }

    public void UnregisterHotkey(IntPtr windowHandle, int id)
    {
        if (_hookHandle != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookHandle);
            _hookHandle = IntPtr.Zero;
        }
    }

    public void SetClickThrough(IntPtr windowHandle, bool enable)
    {
        if (windowHandle == IntPtr.Zero) return;

        var style = GetWindowLong(windowHandle, GWL_EXSTYLE);
        if (enable)
            style |= WS_EX_TRANSPARENT;
        else
            style &= ~WS_EX_TRANSPARENT;
        SetWindowLong(windowHandle, GWL_EXSTYLE, style);
    }

    public void HideFromAltTab(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero) return;
        var style = GetWindowLong(windowHandle, GWL_EXSTYLE);
        style |= WS_EX_TOOLWINDOW;
        SetWindowLong(windowHandle, GWL_EXSTYLE, style);
    }

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    private const int VK_CONTROL = 0x11;
    private const int VK_SHIFT = 0x10;

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            var hookStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

            if (hookStruct.vkCode == _targetKey)
            {
                bool ctrlDown = (_targetModifiers & MOD_CONTROL) == 0 || (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0;
                bool shiftDown = (_targetModifiers & MOD_SHIFT) == 0 || (GetAsyncKeyState(VK_SHIFT) & 0x8000) != 0;

                if (ctrlDown && shiftDown)
                {
                    Dispatcher.UIThread.Post(() => _hotkeyCallback?.Invoke());
                    return (IntPtr)1; // Swallow the key
                }
            }
        }

        return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
    }

    public void Dispose()
    {
        UnregisterHotkey(IntPtr.Zero, 0);
    }
}
