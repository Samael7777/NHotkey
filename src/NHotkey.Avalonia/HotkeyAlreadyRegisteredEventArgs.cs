namespace NHotkey.Avalonia
{
    public class HotkeyAlreadyRegisteredEventArgs : EventArgs
    {
        public HotkeyAlreadyRegisteredEventArgs(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
