using System;

namespace RightKeyboard
{
    public struct KeyboardDevice
    {
        public string Name { get; }

        public IntPtr Handle { get; }

        public KeyboardDevice(string name, IntPtr handle)
        {
            Name = name;
            Handle = handle;
        }
    }
}
