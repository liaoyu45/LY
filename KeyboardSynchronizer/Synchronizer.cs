using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeyboardSynchronizer {
    public partial class Synchronizer : Form {

        IntPtr last;

        public Synchronizer() {
            InitializeComponent();
            MaximizedBounds = ClientRectangle;
            Worker.Collector = getAllWindows;
            Worker.Stopped += Worker_Stopped;
            Worker.Started += Worker_Started;
            Worker.FailToStart += Worker_FailToStart;
            Worker.WindowLost += Worker_WindowLost;
            Worker.WorkingNotice += Worker_WorkingNotice;
            Worker.SendKeyUpChanged += resetTips;
            Worker.AddSpecifyKey(Keys.Oemtilde);
            var yeller = new Yeller();
            yeller.Yelling += yellFree;
            Worker.Yellers.Add(yeller);
            Worker.Prepare();
        }

        private void Worker_WindowLost(IntPtr obj) {
            var row = getAllRows().FirstOrDefault(r => (IntPtr)r.Tag == obj);
            if (row != null) {
                allWindows.Rows.Remove(row);
            }
        }

        private void Worker_WorkingNotice(IntPtr obj) {
            Invoke(new Action(() => {
                var row = getAllRows().FirstOrDefault(r => (IntPtr)r.Tag == obj);
                if (row != null) {
                    row.Selected = true;
                }
            }));
        }

        void Synchronizer_Load(object sender, EventArgs e) {
            setTips("hello");
        }

        void Worker_Started() {
            Invoke(new Action(() => {
                run.Text = "pause";
                main.ReadOnly = true;
                remove.ReadOnly = true;
                pick.Enabled = false;
                allWindows.Enabled = false;
            }));
            var fore = last == default(IntPtr) ? Worker.MainWindows.First() : last;
            fore.SwitchToThisWindow();
        }

        void Worker_Stopped() {
            var current = Win32ApiWrapper.GetForegroundWindow();
            if (Worker.MainWindows.Any(w => w.SameOrBelongTo(current))) {
                last = current;
            }
            Invoke(new Action(() => {
                run.Text = "run";
                main.ReadOnly = false;
                remove.ReadOnly = false;
                pick.Enabled = true;
                allWindows.Enabled = true;
                run.Focus();
            }));
        }

        void Worker_FailToStart() {
            setTips("mainWindow");
        }

        void yellFree(Yeller y, YellEventArgs e) {
            var i = e.Index;
            switch (i) {
                case Yeller.TERMINALED:
                    resetTips();
                    break;
                case Yeller.FINISHED:
                    setTips("yeller");
                    Handle.SwitchToThisWindow();
                    break;
                default:
                    tips.Text = $@"Type ""{(y.Text.Substring(0, i + 1) + y.Text.Substring(i + 1).ToLower())}"" to set this at top.";
                    return;
            }
        }

        void resetTips() {
            setTips("title");
            Invoke(new Action(() => tips.Text += $"\ncurrent mode: {(Worker.SendKeyUp ? "GAME" : "TEXT")}"));
        }

        void allWindows_CellClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex < 0) {
                return;
            }
            var row = allWindows.Rows[e.RowIndex];
            var c = allWindows.Columns[e.ColumnIndex];
            var hWnd = (IntPtr)row.Tag;
            if (c == main) {
                return;
            }
            if (c == winName) {
                hWnd.SwitchToThisWindow();
                return;
            }
            if (c == remove) {
                allWindows.Rows.Remove(row);
                return;
            }
            if (c == up) {
                if (e.RowIndex == 0) {
                    return;
                }
                var i = e.RowIndex - 1;
                allWindows.Rows.Remove(row);
                allWindows.Rows.Insert(i, row);
                allWindows.CurrentCell = allWindows.Rows[i].Cells[e.ColumnIndex];
            }
        }

        void moveWindow(object sender, MouseEventArgs e) {
            Win32ApiWrapper.ReleaseCapture();
            Win32ApiWrapper.SendMessage(Handle, WinApiConsts.Msg.WM_SYSCOMMAND, WinApiConsts.SystemCommands.SC_DRAGMOVE, 0);
        }

        void run_Click(object sender, EventArgs e) {
            if (Worker.Working) {
                Worker.Stop();
                return;
            }
            Worker.Start();
        }

        Dictionary<IntPtr, bool> getAllWindows() {
            return getAllRows().ToDictionary(r => (IntPtr)r.Tag, r => (bool)r.Cells[0].Value);
        }

        private IEnumerable<DataGridViewRow> getAllRows() {
            return allWindows.Rows.Cast<DataGridViewRow>();
        }

        #region PICK
        private void pick_Leave(object sender, EventArgs e) {
            pick.Tag = null;
            resetTips();
        }

        void pick_Click(object sender, EventArgs e) {
            if (pick.Tag == null) {
                pick.Tag = new object();
                setTips("pick");
                return;
            }
            var i = Win32ApiWrapper.WindowFromPoint(Cursor.Position);
            if (i == Handle || Win32.SameOrBelongTo(i, Handle)) {
                setTips("retry");
                return;
            }
            if (getAllWindows().ContainsKey(i)) {
                setTips("already");
                shilver(getAllRows().First(r => (IntPtr)r.Tag == i));
                return;
            }
            var row = allWindows.Rows[allWindows.Rows.Add()];
            row.Tag = i;
            var cells = row.Cells;
            cells[0].Value = true;
            var sb = new StringBuilder(256);
            Win32ApiWrapper.GetWindowText(i, sb, sb.Capacity);
            if (sb.ToString().Length == 0) {
                Win32ApiWrapper.GetClassName(i, sb, sb.Capacity);
            }
            cells[1].Value = sb.ToString();
            cells[2].Value = '↑';
            cells[3].Value = 'X';
            Worker.Collect();
            shilver(row);
        }

        private void shilver(DataGridViewRow row) {
            row.DefaultCellStyle.SelectionBackColor = row.DefaultCellStyle.BackColor = Color.Red;
            var t = new Timer { Interval = 111, Tag = 0 };
            t.Tick += (ss, ee) => {
                var p = (int)t.Tag;
                if (p > 555) {
                    t.Dispose();
                    row.DefaultCellStyle.BackColor = allWindows.DefaultCellStyle.BackColor;
                    row.DefaultCellStyle.SelectionBackColor = allWindows.DefaultCellStyle.SelectionBackColor;
                } else {
                    t.Tag = p + 111;
                }
            };
            t.Start();
        }
        #endregion

        void setTips(string key) {
            Invoke(new Action(() => tips.Text = Tips.All.Keys.Contains(key) ? Tips.All[key] : key));
        }

        void Synchronizer_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.F1) {
                var tip = Tips.Enumerator;
                tip.Step = e.Shift ? -1 : 1;
                tip.MoveNext();
                tips.Text = tip.Index.ToString("X") + ": " + tip.Current.Value;
                return;
            }
            if (e.KeyCode == Keys.Delete) {
                deleteSelectedRows();
                return;
            }
            if (e.Alt) {
                switch (e.KeyCode) {
                    case Keys.P:
                        pick.PerformClick();
                        break;
                    case Keys.R:
                        run.PerformClick();
                        break;
                    case Keys.W:
                        allWindows.Focus();
                        break;
                    case Keys.Q:
                        quit.PerformClick();
                        break;
                }
                return;
            }
            if (e.Control) {
                switch (e.KeyCode) {
                    case Keys.A:
                        allWindows.SelectAll();
                        break;
                    case Keys.D:
                        deleteSelectedRows();
                        break;
                    default:
                        break;
                }
                return;
            }
        }

        void deleteSelectedRows() {
            var rows = allWindows.SelectedRows;
            for (var i = rows.Count - 1; i >= 0; i--) {
                var r = rows[i];
                allWindows.Rows.Remove(r);
            }
        }

        void quit_Click(object sender, EventArgs e) {
            if (quit.Tag == null) {
                var t = new Timer { Interval = 111, Tag = 0 };
                t.Tick += (ss, ee) => {
                    var i = (int)t.Tag;
                    if (i > 555) {
                        quit.Tag = null;
                        t.Dispose();
                        return;
                    }
                    t.Tag = i + 111;
                };
                t.Start();
                quit.Tag = t;
                setTips("quit");
                return;
            }
            Worker.Terminal();
            Application.Exit();
        }

        private void excludedKeys_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                Worker.ExcludedKeys.Clear();
                excludedKeys.ResetText();
                return;
            }
            if (Worker.ExcludedKeys.Contains(e.KeyCode)) {
                return;
            }
            Worker.ExcludedKeys.Add(e.KeyCode);
            excludedKeys.Text = string.Join(",", Worker.ExcludedKeys);
        }
    }
}