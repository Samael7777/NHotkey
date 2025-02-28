using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace NHotkey.Avalonia;

public class GlobalKeyBinding : AvaloniaObject
{
    static GlobalKeyBinding()
    {
        RegisterGlobalHotkeyProperty.Changed.AddClassHandler<KeyBinding>(RegisterGlobalHotkeyPropertyChanged);
    }
    
    public static readonly AttachedProperty<bool> RegisterGlobalHotkeyProperty =
        AvaloniaProperty.RegisterAttached<GlobalKeyBinding, KeyBinding, bool>
        (
            name: "RegisterGlobalHotkey"
        );

    public static void SetRegisterGlobalHotkey(KeyBinding binding, bool value)
    {
        binding.SetValue(RegisterGlobalHotkeyProperty, value);
    }

    public static bool GetRegisterGlobalHotkey(KeyBinding binding)
    {
        return binding.GetValue(RegisterGlobalHotkeyProperty);
    }

    private static void RegisterGlobalHotkeyPropertyChanged(KeyBinding binding, AvaloniaPropertyChangedEventArgs arg)
    {

        var oldValue = arg.GetOldValue<bool>();
        var newValue = arg.GetNewValue<bool>();

        if (Design.IsDesignMode) return;
        
        if (oldValue && !newValue)
        {
            HotkeyManager.Current.RemoveKeyBinding(binding);
        }
        else if (newValue && !oldValue)
        {
            HotkeyManager.Current.AddKeyBinding(binding);
        } 
    }
}