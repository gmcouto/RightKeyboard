using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RightKeyboard.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Globalization;
using System.Linq;

namespace RightKeyboard {
	public partial class MainForm : Form {
		private bool selectingLayout = false;
		private LayoutSelectionDialog layoutSelectionDialog = new LayoutSelectionDialog();

		private Dictionary<IntPtr, Layout> languageMappings = new Dictionary<IntPtr, Layout>();

		private Dictionary<string, IntPtr> devicesByName = new Dictionary<string, IntPtr>();

		public MainForm() {
			InitializeComponent();

			RAWINPUTDEVICE rawInputDevice = new RAWINPUTDEVICE(1, 6, API.RIDEV_INPUTSINK, this);
			bool ok = API.RegisterRawInputDevices(rawInputDevice);
			if(!ok) {
				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
			Debug.Assert(ok);

			WindowState = FormWindowState.Minimized;

			LoadDeviceList();
			LoadConfiguration();
		}

		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			SaveConfiguration();
		}

		private void SaveConfiguration() {
			try {
				string configFilePath = GetConfigFilePath();
				using(TextWriter output = File.CreateText(configFilePath)) {
					foreach(KeyValuePair<string, IntPtr> entry in devicesByName) {
						if(languageMappings.TryGetValue(entry.Value, out var layout)) {
							output.WriteLine("{0}={1:X8}", entry.Key, layout.Identifier.ToInt32());
						}
					}
				}
			}
			catch(Exception err) {
				MessageBox.Show("Could not save the configuration. Reason: " + err.Message);
			}
		}

		private void LoadConfiguration() {
			try {
				string configFilePath = GetConfigFilePath();
				if(File.Exists(configFilePath)) {
					using(TextReader input = File.OpenText(configFilePath)) {

						var layouts = RightKeyboard.Layout.EnumerateLayouts().ToDictionary(k => k.Identifier, v => v);

						string line;
						while((line = input.ReadLine()) != null) {
							string[] parts = line.Split('=');
							Debug.Assert(parts.Length == 2);

							string deviceName = parts[0];
							var layoutId = new IntPtr(int.Parse(parts[1], NumberStyles.HexNumber));

							if(devicesByName.TryGetValue(deviceName, out var deviceHandle)
								&& layouts.TryGetValue(layoutId, out var layout)) {
								languageMappings.Add(deviceHandle, layout);
							}
						}
					}
				}
			}
			catch(Exception err) {
				MessageBox.Show("Could not load the configuration. Reason: " + err.Message);
			}
		}

		private static string GetConfigFilePath() {
			string configFileDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RightKeyboard");
			if(!Directory.Exists(configFileDir)) {
				Directory.CreateDirectory(configFileDir);
			}

			return Path.Combine(configFileDir, "config.txt");
		}

		private void LoadDeviceList() {
			foreach(API.RAWINPUTDEVICELIST rawInputDevice in API.GetRawInputDeviceList()) {
				if(rawInputDevice.dwType == API.RIM_TYPEKEYBOARD) {
					IntPtr deviceHandle = rawInputDevice.hDevice;
					string deviceName = API.GetRawInputDeviceName(deviceHandle);
					devicesByName.Add(deviceName, deviceHandle);
				}
			}
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			Hide();
		}

		protected override void WndProc(ref Message message) {
			switch(message.Msg) {
				case API.WM_INPUT:
					if(!selectingLayout) {
						ProcessInputMessage(message);
					}
					break;

				case API.WM_POWERBROADCAST:
					ProcessPowerMessage(message);
					break;

				default:
					base.WndProc(ref message);
					break;
			}
		}

		private void ProcessPowerMessage(Message message) {
			switch(message.WParam.ToInt32()) {
				case API.PBT_APMQUERYSUSPEND:
					Debug.WriteLine("PBT_APMQUERYSUSPEND");
					break;

				case API.PBT_APMQUERYSTANDBY:
					Debug.WriteLine("PBT_APMQUERYSTANDBY");
					break;

				case API.PBT_APMQUERYSUSPENDFAILED:
					Debug.WriteLine("PBT_APMQUERYSUSPENDFAILED");
					break;

				case API.PBT_APMQUERYSTANDBYFAILED:
					Debug.WriteLine("PBT_APMQUERYSTANDBYFAILED");
					break;

				case API.PBT_APMSUSPEND:
					Debug.WriteLine("PBT_APMSUSPEND");
					break;

				case API.PBT_APMSTANDBY:
					Debug.WriteLine("PBT_APMSTANDBY");
					break;

				case API.PBT_APMRESUMECRITICAL:
					Debug.WriteLine("PBT_APMRESUMECRITICAL");
					break;

				case API.PBT_APMRESUMESUSPEND:
					Debug.WriteLine("PBT_APMRESUMESUSPEND");
					break;

				case API.PBT_APMRESUMESTANDBY:
					Debug.WriteLine("PBT_APMRESUMESTANDBY");
					break;

				case API.PBT_APMBATTERYLOW:
					Debug.WriteLine("PBT_APMBATTERYLOW");
					break;

				case API.PBT_APMPOWERSTATUSCHANGE:
					Debug.WriteLine("PBT_APMPOWERSTATUSCHANGE");
					break;

				case API.PBT_APMOEMEVENT:
					Debug.WriteLine("PBT_APMOEMEVENT");
					break;

				case API.PBT_APMRESUMEAUTOMATIC:
					Debug.WriteLine("PBT_APMRESUMEAUTOMATIC");
					break;

				default:
					break;
			}
		}

		private void ProcessInputMessage(Message message) {
			RAWINPUTHEADER header;
			uint result = API.GetRawInputData(message.LParam, API.RID_HEADER, out header);
			Debug.Assert(result != uint.MaxValue);
			if(result == uint.MaxValue) {
				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
			ValidateCurrentDevice(header.hDevice);
		}

		private void ValidateCurrentDevice(IntPtr hCurrentDevice) {
			if (!languageMappings.TryGetValue(hCurrentDevice, out var layout)) {
				selectingLayout = true;
				layoutSelectionDialog.ShowDialog();
				selectingLayout = false;
				layout = layoutSelectionDialog.Layout;
				languageMappings.Add(hCurrentDevice, layout);
			}

			var currentSysLayout = API.GetKeyboardLayout();

			if (currentSysLayout != layout.Identifier)
			{
				SetCurrentLayout(layout.Identifier);
				SetDefaultLayout(layout.Identifier);
			}
		}

		private void SetCurrentLayout(IntPtr layoutId) {
				uint recipients = API.BSM_APPLICATIONS;
				API.BroadcastSystemMessage(API.BSF_POSTMESSAGE, ref recipients, API.WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, layoutId);
		}

		private void SetDefaultLayout(IntPtr layoutId) {
			API.SystemParametersInfo(API.SPI_SETDEFAULTINPUTLANG, 0, new IntPtr[] { layoutId }, API.SPIF_SENDCHANGE);
		}

		private void clearToolStripMenuItem_Click(object sender, EventArgs e)
		{
			languageMappings.Clear();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
			Close();
		}
	}
}