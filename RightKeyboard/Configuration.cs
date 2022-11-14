using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace RightKeyboard
{
    public class Configuration
    {
		public Dictionary<IntPtr, Layout> LanguageMappings { get; } = new Dictionary<IntPtr, Layout>();

		public static Configuration LoadConfiguration(KeyboardDevicesCollection devices)
		{
			var configuration = new Configuration();
			var languageMappings = configuration.LanguageMappings;

			string configFilePath = GetConfigFilePath();
			if (File.Exists(configFilePath))
			{
				using (TextReader input = File.OpenText(configFilePath))
				{

					var layouts = Layout.EnumerateLayouts().ToDictionary(k => k.Identifier, v => v);

					string line;
					while ((line = input.ReadLine()) != null)
					{
						string[] parts = line.Split('=');
						Debug.Assert(parts.Length == 2);

						string deviceName = parts[0];
						var layoutId = new IntPtr(int.Parse(parts[1], NumberStyles.HexNumber));

						if (devices.TryGetByName(deviceName, out var deviceHandle)
							&& layouts.TryGetValue(layoutId, out var layout))
						{
							languageMappings.Add(deviceHandle, layout);
						}
					}
				}
			}

			return configuration;
		}

		public void Save(KeyboardDevicesCollection devices)
		{
			string configFilePath = GetConfigFilePath();
			using (TextWriter output = File.CreateText(configFilePath))
			{
				foreach (var device in devices)
				{
					if (LanguageMappings.TryGetValue(device.Handle, out var layout))
					{
						output.WriteLine("{0}={1:X8}", device.Name, layout.Identifier.ToInt32());
					}
				}
			}
		}

		private static string GetConfigFilePath()
		{
			string configFileDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RightKeyboard");
			if (!Directory.Exists(configFileDir))
			{
				Directory.CreateDirectory(configFileDir);
			}

			return Path.Combine(configFileDir, "config.txt");
		}
	}
}
