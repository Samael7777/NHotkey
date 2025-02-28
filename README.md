NHotkey
=======

Forked from https://github.com/thomaslevesque/NHotkey

Add Avalonia framework support.

[![NuGet version](https://img.shields.io/nuget/v/PhoenixTools.NHotkey.svg?logo=nuget&label=PhoenixTools.NHotkey)](https://www.nuget.org/packages/PhoenixTools.NHotkey)
[![NuGet version](https://img.shields.io/nuget/v/PhoenixTools.NHotkey.Wpf.svg?logo=nuget&label=PhoenixTools.NHotkey.Wpf)](https://www.nuget.org/packages/PhoenixTools.NHotkey.Wpf)
[![NuGet version](https://img.shields.io/nuget/v/PhoenixTools.NHotkey.WindowsForms.svg?logo=nuget&label=PhoenixTools.NHotkey.WindowsForms)](https://www.nuget.org/packages/PhoenixTools.NHotkey.WindowsForms)
[![NuGet version](https://img.shields.io/nuget/v/PhoenixTools.NHotkey.Avalonia.svg?logo=nuget&label=PhoenixTools.NHotkey.Avalonia)](https://www.nuget.org/packages/PhoenixTools.NHotkey.Avalonia)

Easily handle shortcut keys even when your WPF or WinForms app doesn't have focus. Declare hotkeys in XAML with the familiar `KeyBinding` syntax.

Nuget packages:
- for WPF: [PhoenixTools.NHotkey.Wpf](http://www.nuget.org/packages/PhoenixTools.NHotkey.Wpf/)
- for Windows Forms: [PhoenixTools.NHotkey.WindowsForms](http://www.nuget.org/packages/PhoenixTools.NHotkey.WindowsForms/)
- for Avalonia : [PhoenixTools.NHotkey.Avalonia](http://www.nuget.org/packages/PhoenixTools.NHotkey.Avalonia/)

### Windows Forms usage

Add a reference to `NHotkey.dll` and `NHotkey.WindowsForms.dll`. In the file where you want to
handle hotkeys, import the `NHotkey.WindowsForms` namespace:

```csharp
    using NHotkey.WindowsForms;
```

During initialization, add some hotkeys:

```csharp
    HotkeyManager.Current.AddOrReplace("Increment", Keys.Control | Keys.Alt | Keys.Add, OnIncrement);
    HotkeyManager.Current.AddOrReplace("Decrement", Keys.Control | Keys.Alt | Keys.Subtract, OnDecrement);
```

- the first parameter is an application-defined name for the hotkey; it can be anything you like,
as long as it's unique;
- the second parameter is the combination of keys for which you want to register a hotkey;
- the last parameter is a delegate of type `EventHandler<HotkeyEventArgs>` that will be called
when this hotkey is pressed. For instance:

```csharp
    private void OnIncrement(object sender, HotkeyEventArgs e)
    {
        Value++;
        e.Handled = true;
    }

    private void OnDecrement(object sender, HotkeyEventArgs e)
    {
        Value--;
        e.Handled = true;
    }
```

If you want to handle several hotkeys with the same handler, you can check the `Name`
property of the `HotkeyEventArgs`:

```csharp
    private void OnIncrementOrDecrement(object sender, HotkeyEventArgs e)
    {
        switch (e.Name)
        {
            case "Increment":
                Value++;
                break;
            case "Decrement":
                Value--;
                break;
        }
        e.Handled = true;
    }
```

### WPF usage

The approach for WPF is very similar to the one for Windows Forms; the exposed API is slightly
different to account for the differences between WinForms and WPF. The WPF version also
supports `KeyBindings`.

Add a reference to `NHotkey.dll` and `NHotkey.Wpf.dll`. In the file where you want to
handle hotkeys, import the `NHotkey.Wpf` namespace:

```csharp
    using NHotkey.Wpf;
```

During initialization, add some hotkeys:

```csharp
    HotkeyManager.Current.AddOrReplace("Increment", Key.Add, ModifierKeys.Control | ModifierKeys.Alt, OnIncrement);
    HotkeyManager.Current.AddOrReplace("Decrement", Key.Subtract, ModifierKeys.Control | ModifierKeys.Alt, OnDecrement);
```

- the first parameter is an application-defined name for the hotkey; it can be anything you like,
as long as it's unique;
- the second and third parameters are the key and modifiers for which you want to register a hotkey;
- the last parameter is a delegate of type `EventHandler<HotkeyEventArgs>` that will be called
when this hotkey is pressed.

To support applications that use the MVVM pattern, you can also specify hotkeys in XAML using
`InputBindings`. Just declare `KeyBindings` as usual, and set the `HotkeyManager.RegisterGlobalHotkey`
attached property to `true`:

```xml
    ...
    <Window.InputBindings>
        <KeyBinding Gesture="Ctrl+Alt+Add" Command="{Binding IncrementCommand}"
                    HotkeyManager.RegisterGlobalHotkey="True" />
        <KeyBinding Gesture="Ctrl+Alt+Subtract" Command="{Binding DecrementCommand}"
                    HotkeyManager.RegisterGlobalHotkey="True" />
    </Window.InputBindings>
    ...
```

**Known limitations of this feature**

- the `HotkeyManager` can't detect if you remove a `KeyBinding`; it only relies on the
attached property being set to true or false. If you want to remove a KeyBinding at runtime,
make sure you set `HotkeyManager.RegisterGlobalHotkey` to false, otherwise it will
still be registered
- changing the keys or modifiers of a `KeyBinding` at runtime is currently not supported. If
you need to modify a `KeyBinding` at runtime, you need to set `HotkeyManager.RegisterGlobalHotkey`
to false, change the key, and set `HotkeyManager.RegisterGlobalHotkey` to true again.

### Avalonia usage

In-code usage and limitations are similar to WPF.

Namespace :
```csharp
    using NHotkey.Avalonia;
```

AXAML syntax:
```xml
    <Window 
    xmlns:avalonia="clr-namespace:NHotkey.Avalonia;assembly=NHotkey.Avalonia"
    ... >
    ...
    <Window.KeyBindings>
        <KeyBinding Gesture="Ctrl+Alt+N" Command="{Binding NegateCommand}"
                    avalonia:GlobalKeyBinding.RegisterGlobalHotkey="True" />
        <KeyBinding Gesture="Ctrl+Alt+T" Command="{Binding TestCommand}"
                    avalonia:GlobalKeyBinding.RegisterGlobalHotkey="True" />
    </Window.KeyBindings>
    ...
```
