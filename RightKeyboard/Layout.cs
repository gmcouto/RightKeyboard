using System;
using RightKeyboard.Win32;
using System.Linq;
using System.Collections.Generic;

namespace RightKeyboard {
	/// <summary>
	/// Represents a keyboard layout
	/// </summary>
	public class Layout {

		/// <summary>
		/// Gets the layout's identifier
		/// </summary>
		public IntPtr Identifier { get; }

		/// <summary>
		/// Gets the layout's name
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Initializes a new instance of Layout
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="name"></param>
		public Layout(IntPtr identifier, string name) {
			this.Identifier = identifier;
			this.Name = name;
		}

		public override string ToString() {
			return Name;
		}

		/// <summary>
		/// Gets the installed keyboard layouts
		/// </summary>
		public static IEnumerable<Layout> EnumerateLayouts() {
			return API.GetKeyboardLayoutList()
				.Select(p => new Layout(p, API.GetKeyboardLayoutName(p)));
		}
	}
}