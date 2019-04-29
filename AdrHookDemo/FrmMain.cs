using AdrHook;
using System;
using System.Windows.Forms;

namespace AdrHookDemo {
	/// <summary>
	/// 功能说明：键盘全局钩子示例程序
	/// 开发人员：王旭（http://www.wxzzz.com）
	/// 开发时间：2014年4月3日
	/// </summary>
	public partial class FrmMain : Form {
		public FrmMain() {
			InitializeComponent();
		}

		#region 复选框设置或删除特定的事件处理程序

		private void checkBoxOnMouseMove_CheckedChanged(object sender, EventArgs e) {
			if (checkBoxOnMouseMove.Checked) {
				HookManager.MouseMove += HookManager_MouseMove;
			} else {
				HookManager.MouseMove -= HookManager_MouseMove;
			}
		}

		private void checkBoxOnMouseClick_CheckedChanged(object sender, EventArgs e) {
			if (checkBoxOnMouseClick.Checked) {
				HookManager.MouseClick += HookManager_MouseClick;
			} else {
				HookManager.MouseClick -= HookManager_MouseClick;
			}
		}

		private void checkBoxOnMouseUp_CheckedChanged(object sender, EventArgs e) {
			if (checkBoxOnMouseUp.Checked) {
				HookManager.MouseUp += HookManager_MouseUp;
			} else {
				HookManager.MouseUp -= HookManager_MouseUp;
			}
		}

		private void checkBoxOnMouseDown_CheckedChanged(object sender, EventArgs e) {
			if (checkBoxOnMouseDown.Checked) {
				HookManager.MouseDown += HookManager_MouseDown;
			} else {
				HookManager.MouseDown -= HookManager_MouseDown;
			}
		}

		private void checkBoxMouseDoubleClick_CheckedChanged(object sender, EventArgs e) {
			if (checkBoxMouseDoubleClick.Checked) {
				HookManager.MouseDoubleClick += HookManager_MouseDoubleClick;
			} else {
				HookManager.MouseDoubleClick -= HookManager_MouseDoubleClick;
			}
		}

		private void checkBoxMouseWheel_CheckedChanged(object sender, EventArgs e) {
			if (checkBoxMouseWheel.Checked) {
				HookManager.MouseWheel += HookManager_MouseWheel;
			} else {
				HookManager.MouseWheel -= HookManager_MouseWheel;
			}
		}

		private void checkBoxKeyDown_CheckedChanged(object sender, EventArgs e) {
			if (checkBoxKeyDown.Checked) {
				HookManager.KeyDown += HookManager_KeyDown;
			} else {
				HookManager.KeyDown -= HookManager_KeyDown;
			}
		}


		private void checkBoxKeyUp_CheckedChanged(object sender, EventArgs e) {
			if (checkBoxKeyUp.Checked) {
				HookManager.KeyUp += HookManager_KeyUp;
			} else {
				HookManager.KeyUp -= HookManager_KeyUp;
			}
		}

		private void checkBoxKeyPress_CheckedChanged(object sender, EventArgs e) {
			if (checkBoxKeyPress.Checked) {
				HookManager.KeyPress += HookManager_KeyPress;
			} else {
				HookManager.KeyPress -= HookManager_KeyPress;
			}
		}

		#endregion

		#region 事件处理

		private void HookManager_KeyDown(object sender, KeyEventArgs e) {
			textBoxLog.AppendText(string.Format("KeyDown - {0}\n", e.KeyCode));
			textBoxLog.ScrollToCaret();
		}

		private void HookManager_KeyUp(object sender, KeyEventArgs e) {
			textBoxLog.AppendText(string.Format("KeyUp - {0}\n", e.KeyCode));
			textBoxLog.ScrollToCaret();
		}

		private void HookManager_KeyPress(object sender, KeyPressEventArgs e) {
			textBoxLog.AppendText(string.Format("KeyPress - {0}\n", e.KeyChar));
			textBoxLog.ScrollToCaret();
		}

		private void HookManager_MouseMove(object sender, MouseEventArgs e) {
			labelMousePosition.Text = string.Format("x={0:0000}; y={1:0000}", e.X, e.Y);
		}

		private void HookManager_MouseClick(object sender, MouseEventArgs e) {
			textBoxLog.AppendText(string.Format("MouseClick - {0}\n", e.Button));
			textBoxLog.ScrollToCaret();
		}

		private void HookManager_MouseUp(object sender, MouseEventArgs e) {
			textBoxLog.AppendText(string.Format("MouseUp - {0}\n", e.Button));
			textBoxLog.ScrollToCaret();
		}

		private void HookManager_MouseDown(object sender, MouseEventArgs e) {
			textBoxLog.AppendText(string.Format("MouseDown - {0}\n", e.Button));
			textBoxLog.ScrollToCaret();

		}

		private void HookManager_MouseDoubleClick(object sender, MouseEventArgs e) {
			textBoxLog.AppendText(string.Format("MouseDoubleClick - {0}\n", e.Button));
			textBoxLog.ScrollToCaret();
		}

		private void HookManager_MouseWheel(object sender, MouseEventArgs e) {
			labelWheel.Text = string.Format("Wheel={0:000}", e.Delta);
		}

		#endregion
	}
}
