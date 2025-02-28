using System.Diagnostics.CodeAnalysis;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Win32.Input;
using PhoenixTools.Window;

namespace NHotkey.Avalonia;

[SuppressMessage(
    "Microsoft.Design",
    "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
    Justification = "This is a singleton; disposing it would break it")]
public class HotkeyManager : HotkeyManagerBase
{
    #region Singleton implementation

    public static HotkeyManager Current => LazyInitializer.Instance;

    private static class LazyInitializer
    {
        static LazyInitializer() { }
        public static readonly HotkeyManager Instance = new ();
    }

    #endregion
        
    #region HotkeyAlreadyRegistered event

    public static event EventHandler<HotkeyAlreadyRegisteredEventArgs>? HotkeyAlreadyRegistered;

    private static void OnHotkeyAlreadyRegistered(string name)
    {
        var handler = HotkeyAlreadyRegistered;
        handler?.Invoke(null, new HotkeyAlreadyRegisteredEventArgs(name));
    }

    #endregion

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly SimpleWindow _window;
    private readonly WeakReferenceCollection<KeyBinding> _keyBindings;

    private HotkeyManager()
    {
        _keyBindings = new WeakReferenceCollection<KeyBinding>();
        _window = new SimpleWindow();
        _window.MessageReceived += HandleMessage;
        SetHwnd(_window.Handle);
    }

    public void AddOrReplace(string name, KeyGesture gesture, EventHandler<HotkeyEventArgs> handler)
    {
        AddOrReplace(name, gesture, false, handler);
    }

    public void AddOrReplace(string name, KeyGesture gesture, bool noRepeat, EventHandler<HotkeyEventArgs> handler)
    {
        AddOrReplace(name, gesture.Key, gesture.KeyModifiers, noRepeat, handler);
    }

    public void AddOrReplace(string name, Key key, KeyModifiers modifiers, EventHandler<HotkeyEventArgs>? handler)
    {
        AddOrReplace(name, key, modifiers, false, handler);
    }

    public void AddOrReplace(string name, Key key, KeyModifiers modifiers, bool noRepeat, EventHandler<HotkeyEventArgs>? handler)
    {
        var flags = GetFlags(modifiers, noRepeat);
        var vk = (uint)KeyInterop.VirtualKeyFromKey(key);
        _window.Invoke(() =>
        {
            AddOrReplace(name, vk, flags, handler);
        });
    }

    public new void Remove(string name)
    {
        _window.Invoke(() =>
        {
            base.Remove(name);
        });
    }

    private static HotkeyFlags GetFlags(KeyModifiers modifiers, bool noRepeat)
    {
        var flags = HotkeyFlags.None;
        if (modifiers.HasFlag(KeyModifiers.Shift))
            flags |= HotkeyFlags.Shift;
        if (modifiers.HasFlag(KeyModifiers.Control))
            flags |= HotkeyFlags.Control;
        if (modifiers.HasFlag(KeyModifiers.Alt))
            flags |= HotkeyFlags.Alt;
        if (modifiers.HasFlag(KeyModifiers.Meta))
            flags |= HotkeyFlags.Windows;
        if (noRepeat)
            flags |= HotkeyFlags.NoRepeat;
        return flags;
    }

    private static KeyModifiers GetModifiers(HotkeyFlags flags)
    {
        var modifiers = KeyModifiers.None;
        if (flags.HasFlag(HotkeyFlags.Shift))
            modifiers |= KeyModifiers.Shift;
        if (flags.HasFlag(HotkeyFlags.Control))
            modifiers |= KeyModifiers.Control;
        if (flags.HasFlag(HotkeyFlags.Alt))
            modifiers |= KeyModifiers.Alt;
        if (flags.HasFlag(HotkeyFlags.Windows))
            modifiers |= KeyModifiers.Meta;
        return modifiers;
    }

    internal void AddKeyBinding(KeyBinding keyBinding)
    {
        var gesture = keyBinding.Gesture;
        //var name = GetNameForKeyBinding(gesture); //Todo
        var name = gesture.ToString();
        try
        {
            AddOrReplace(name, gesture.Key, gesture.KeyModifiers, null);
            _keyBindings.Add(keyBinding);
        }
        catch (HotkeyAlreadyRegisteredException)
        {
            OnHotkeyAlreadyRegistered(name);
        }
    }

    internal void RemoveKeyBinding(KeyBinding keyBinding)
    {
        var gesture = keyBinding.Gesture;
        //var name = GetNameForKeyBinding(gesture); //todo
        var name = gesture.ToString();
        _window.Invoke(() =>
        {
            Remove(name);
        });

        _keyBindings.Remove(keyBinding);
    }

    //Todo Conversion to string?
    //private readonly KeyGestureConverter _gestureConverter = new KeyGestureConverter();
    //private string GetNameForKeyBinding(KeyGesture gesture)
    //{
    //    var name = gesture.ToString();
    //    if (string.IsNullOrEmpty(name))
    //        name = _gestureConverter.ConvertToString(gesture);
    //    return name;
    //}

    private void HandleMessage(object? sender, WindowsMessageEventArgs args)
    {
        var isHandled = args.IsHandled;
        _ = HandleHotkeyMessage(
            args.WindowHandle, 
            (int)args.Message, 
            (nint)args.WParam, 
            args.LParam, 
            ref isHandled, out var hotkey);

        if (hotkey != null)
        {
            Dispatcher.UIThread.Post(() =>
            {
                isHandled = ExecuteBoundCommand(hotkey);
            });
        }

        args.IsHandled = isHandled;
    }

    private bool ExecuteBoundCommand(Hotkey hotkey)
    {
        //var key = KeyInterop.KeyFromVirtualKey((int)hotkey.VirtualKey);
        var key = VirtualKeyToKey(hotkey.VirtualKey);
        var modifiers = GetModifiers(hotkey.Flags);
        var handled = false;
        foreach (var binding in _keyBindings)
        {
            var gesture = binding.Gesture;
            if (gesture.Key == key && gesture.KeyModifiers == modifiers)
            {
                handled |= ExecuteCommand(binding);
            }
        }
        return handled;
    }
    
    private static bool ExecuteCommand(KeyBinding binding)
    {
        var command = binding.Command;
        var parameter = binding.CommandParameter;
            
        if (command == null || !command.CanExecute(parameter))
            return false;

        command.Execute(parameter);

        return true;
    }

    private static Key VirtualKeyToKey(uint vk)
    {
        var scanCode = WinApi.MapVirtualKey(vk, MAP_VIRTUAL_KEY_TYPE.MAPVK_VK_TO_VSC);
        var key = KeyInterop.KeyFromVirtualKey((int)vk, (int)scanCode);

        return key;
    }
}