using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using RightKeyboard.Win32;

namespace RightKeyboard
{
    public class KeyboardDevicesCollection : IEnumerable<KeyboardDevice>
    {
        private Dictionary<string, IntPtr> devicesByName = new Dictionary<string, IntPtr>();

        public bool TryGetByName(string deviceName, out IntPtr deviceHandle)
        {
            return devicesByName.TryGetValue(deviceName, out deviceHandle);
        }

        public KeyboardDevicesCollection()
        {
            foreach (API.RAWINPUTDEVICELIST rawInputDevice in API.GetRawInputDeviceList())
            {
                if (rawInputDevice.dwType == API.RIM_TYPEKEYBOARD)
                {
                    IntPtr deviceHandle = rawInputDevice.hDevice;
                    string deviceName = API.GetRawInputDeviceName(deviceHandle);
                    devicesByName.Add(deviceName, deviceHandle);
                }
            }
        }

        public IEnumerator<KeyboardDevice> GetEnumerator()
        {
            return devicesByName
                .Select(kvp => new KeyboardDevice(kvp.Key, kvp.Value))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
