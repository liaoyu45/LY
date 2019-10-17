using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Math;

namespace Tools {
	public partial class MainWindow : Window {
		FrameDimension dimension = null;
		Bitmap bmp = null;
		private MemoryStream ms;
		private int itemIndex;

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
		static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
		const int SWP_SHOWWINDOW = 0x0040;
		private System.Windows.Media.Brush buttonBrush = new SolidColorBrush(Colors.Red);
		private string file;

		public MainWindow() {
			InitializeComponent();
			Loaded += delegate {
				IntPtr handle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
				//SetWindowPos(handle, HWND_TOPMOST, 0, 0, 100, 200, SWP_SHOWWINDOW);
			};
			options.Visibility = Visibility.Collapsed;
			erazor.Visibility = Visibility.Collapsed;
			Tag = Background;
			first.Click += (s, e) => SetBackground(0);
			previous.Click += (s, e) => SetBackground(CurrentFrame - 1);
			next.Click += (s, e) => SetBackground(CurrentFrame + 1);
			last.Click += (s, e) => SetBackground((int)frames.Maximum);
			loop.Tag = loop.Foreground;
			start.Tag = start.Foreground;
			start.Click += (s, e) => Toggle();
			loop.Click += (s, e) => loop.Foreground = WillLoop ? loop.Tag as System.Windows.Media.Brush : buttonBrush;
			open.Click += (s, e) => PickPicture();
			reset.Click += (s, e) => ClearAndReset();
			save.Click += (s, e) => Save();
			handle.Click += (s, e) => handle.Content = Dragging ? "🧔" : "🖐";
			KeyDown += (s, e) => WindowState = WindowState.Normal;
		}

		private int CurrentFrame => (int)frames.Value;
		private bool WillLoop => loop.Tag != loop.Foreground;
		public bool Animating {
			get {
				return start.Foreground != start.Tag;
			}
			set {
				start.Foreground = value ? buttonBrush : start.Tag as System.Windows.Media.Brush;
			}
		}
		public bool Dragging => handle.Content as string == "🖐";

		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) {
			if (bmp == null) {
				PickPicture();
				return;
			}
			options.Visibility = options.Visibility == Visibility ? Visibility.Collapsed : Visibility;
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if (Dragging) {
				DragMove();
				return;
			}
			var p = e.GetPosition(this);
			ImageHelper.FloodFill(bmp, new System.Drawing.Point((int)p.X, (int)p.Y), System.Drawing.Color.Transparent, outter.Offset * 100);
			SetBackground(CurrentFrame);
			bmp.Save("dd.gif");
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e) {
			outter.Offset += e.Delta > 0 ? .1 : -.1;
			outter.Offset = Max(0, Min(1, outter.Offset));
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			if (bmp == null) {
				return;
			}
			var p = e.GetPosition(this);
			erazor.Margin = new Thickness {
				Left = p.X - erazor.Width / 2,
				Top = p.Y - erazor.Height / 2
			};
			var x = Max(0, Min(bmp.Width - 1, (int)(p.X * bmp.Width / Width)));
			var y = Max(0, Min(bmp.Height - 1, (int)(p.Y * bmp.Height / Height)));
			var c = bmp.GetPixel(x, y);
			erazorStroke.Color = mainColor.Color = new System.Windows.Media.Color {
				A = c.A,
				R = c.R,
				G = c.G,
				B = c.B
			};
		}

		protected override void OnMouseLeave(MouseEventArgs e) {
			erazor.Visibility = Visibility.Collapsed;
		}

		protected override void OnMouseEnter(MouseEventArgs e) {
			if (bmp != null && Dragging) {
				erazor.Visibility = Visibility;
			}
		}

		private void Toggle() {
			Animating = !Animating;
			Animate();
		}

		private void Animate() {
			Dispatcher.Invoke(() => {
				if (!Animating) {
					return;
				}
				if (CurrentFrame == frames.Maximum) {
					frames.Value = 0;
					if (!WillLoop) {
						start.Foreground = start.Tag as System.Windows.Media.Brush;
						return;
					}
				}
				var v = SetBackground(CurrentFrame + 1);
				if (v == 0) {
					v = 88;
				}
				new Task(() => {
					Thread.Sleep(v);
					Animate();
				}).Start();
			});
		}

		private void ClearAndReset() {
			outter.Offset = 1;
			bmp?.Dispose();
			bmp = null;
			Background = Tag as System.Windows.Media.Brush;
			ms?.Dispose();
			ms = null;
			itemIndex = 0;
			dimension = null;
			frames.Maximum = 0;
			frames.Value = 0;
			options.Visibility = Visibility.Collapsed;
		}

		private void Save() {
			throw new NotImplementedException();
		}

		private void PickPicture() {
			var d = new OpenFileDialog {
				Filter = "图片|*.jpg;*.png;*.gif;*.bmp;"
			};
			if (d.ShowDialog() != true) {
				return;
			}
			bmp?.Dispose();
			bmp = Image.FromFile(file = d.FileName) as Bitmap;
			Width = bmp.Size.Width;
			Height = bmp.Size.Height;
			itemIndex = bmp.PropertyIdList.ToList().IndexOf(0x5100);
			dimension = new FrameDimension(bmp.FrameDimensionsList[0]);
			frames.Value = 0;
			frames.Maximum = bmp.GetFrameCount(dimension) - 1;
			SetBackground(0);
		}

		private int SetBackground(int f) {
			if (bmp == null) {
				return 0;
			}
			if (f > frames.Maximum || f < 0) {
				return 0;
			}
			frames.Value = f;
			bmp.SelectActiveFrame(dimension, f);
			ms?.Dispose();
			ms = new MemoryStream();
			bmp.Save(ms, ImageFormat.Png);
			var bi = new BitmapImage();
			bi.BeginInit();
			bi.StreamSource = ms;
			bi.EndInit();
			Background = new ImageBrush(bi);
			if (itemIndex > -1) {
				var p = bmp.PropertyItems[itemIndex];
				var bs = new byte[4];
				bs[0] = p.Value[f * 4];
				bs[1] = p.Value[1 + f * 4];
				bs[2] = p.Value[2 + f * 4];
				bs[3] = p.Value[3 + f * 4];
				return BitConverter.ToInt32(bs, 0) * 10;
			}
			return 0;
		}
	}
}
