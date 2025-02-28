using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;

namespace NHotkey.Avalonia.Demo;

public partial class MainVm : ObservableObject
{
    private static readonly KeyGesture _incrementGesture = new(Key.Up, KeyModifiers.Control | KeyModifiers.Alt);
    private static readonly KeyGesture _decrementGesture = new(Key.Down, KeyModifiers.Control | KeyModifiers.Alt);

    [ObservableProperty] private int _value;

    public string IncrementHotkey => _incrementGesture.ToString();
    public string DecrementHotkey => _decrementGesture.ToString();

    public bool IsHotkeyManagerEnabled
    {
        get => HotkeyManager.Current.IsEnabled;
        set => HotkeyManager.Current.IsEnabled = value;
    }

    public MainVm()
    {
        HotkeyManager.HotkeyAlreadyRegistered += HotkeyManager_HotkeyAlreadyRegistered;

        HotkeyManager.Current.AddOrReplace("Increment", _incrementGesture, OnIncrement);
        HotkeyManager.Current.AddOrReplace("Decrement", _decrementGesture, OnDecrement);
    }

    private static void HotkeyManager_HotkeyAlreadyRegistered(object? sender, HotkeyAlreadyRegisteredEventArgs e)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            "Error",
            $"The hotkey {e.Name} is already registered by another application",
            ButtonEnum.Ok,
            Icon.Error);

        box.ShowAsync();
    }
    
    [RelayCommand]
    private void Negate()
    {
        Value = -Value;
    }

    [RelayCommand]
    private void Test()
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            "Test",
            $"Test",
            ButtonEnum.Ok,
            Icon.Info);

        box.ShowAsync();
    }
    
    private void OnIncrement(object? sender, HotkeyEventArgs e)
    {
        Value++;
        e.Handled = true;
    }

    private void OnDecrement(object? sender, HotkeyEventArgs e)
    {
        Value--;
        e.Handled = true;
    }
}