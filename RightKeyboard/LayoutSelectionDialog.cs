using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RightKeyboard {
	public partial class LayoutSelectionDialog : Form {
		public LayoutSelectionDialog() {
			InitializeComponent();

			LoadLanguageList();
		}

		private void LoadLanguageList() {
			lbLayouts.Items.Clear();
			recentLayoutsCount = 0;

			foreach(Layout layout in Layout.EnumerateLayouts()) {
				lbLayouts.Items.Add(layout);
			}

			lbLayouts.SelectedIndex = 0;
		}

		private int recentLayoutsCount = 0;
		private Layout selectedLayout;
		private bool okPressed = false;

		public new Layout Layout {
			get {
				return selectedLayout;
			}
		}

		private void btOk_Click(object sender, EventArgs e) {
			selectedLayout = (Layout)lbLayouts.SelectedItem;
			okPressed = true;
			Close();
		}

		private void lbLayouts_SelectedIndexChanged(object sender, EventArgs e) {
			btOk.Enabled = lbLayouts.SelectedIndex != recentLayoutsCount || recentLayoutsCount == 0;
		}

		private void lbLayouts_DoubleClick(object sender, EventArgs e) {
			if(btOk.Enabled) {
				btOk_Click(this, EventArgs.Empty);
			}
		}

		protected override void OnClosing(CancelEventArgs e) {
			e.Cancel = !okPressed;
			okPressed = false;
			base.OnClosing(e);
		}
	}
}