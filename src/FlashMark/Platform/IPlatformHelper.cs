using System;

namespace FlashMark.Platform;

public interface IPlatformHelper
{
    void RegisterHotkey(IntPtr windowHandle, int id, uint modifiers, uint key, Action callback);
    void UnregisterHotkey(IntPtr windowHandle, int id);
    void SetClickThrough(IntPtr windowHandle, bool enable);
}
