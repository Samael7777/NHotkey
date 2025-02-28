using Avalonia.Input;

namespace NHotkey.Avalonia
{
    static class Extensions
    {
        public static bool HasFlag(this KeyModifiers  modifiers, KeyModifiers flag)
        {
            return (modifiers & flag) == flag;
        }

        public static bool HasFlag(this HotkeyFlags flags, HotkeyFlags flag)
        {
            return (flags & flag) == flag;
        }
    }
}
