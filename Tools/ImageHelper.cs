using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Tools {
	class ImageHelper {
		public static double Distance(Color c0, Color c1) {
			return Math.Sqrt(Math.Pow((double)c0.R - c1.R, 2) + Math.Pow((double)c0.G - c0.G, 2) + Math.Pow((double)c0.B - c1.B, 2));
		}

		public static Bitmap FloodFill(Bitmap bmp, Point p, Color fill, double tolerance, bool keep = true) {
			var re = new Rectangle(Point.Empty, bmp.Size);
			var result = keep ? bmp : bmp.Clone(re, bmp.PixelFormat);
			var r = new Stack<Point>();
			var center = result.GetPixel(p.X, p.Y);
			var records = new bool[result.Width, result.Height];
			r.Push(p);
			var siblings = new int[][] { new[] { 0, 1 }, new[] { 0, -1 }, new[] { -1, 0 }, new[] { 1, 0 } };
			while (r.Count > 0) {
				var current = r.Pop();
				if (records[current.X, current.Y]) {
					continue;
				}
				records[current.X, current.Y] = true;
				if (Distance(result.GetPixel(current.X, current.Y), center) > tolerance) {
					continue;
				}
				result.SetPixel(current.X, current.Y, fill);
				siblings.Select(e => new Point(current.X + e[0], current.Y + e[1])).Where(e => re.Contains(e)).ToList().ForEach(r.Push);
			}
			return result;
		}










		/// <summary> 

		/// 设置GIF大小 
		/// </summary> 
		/// <param name="path">图片路径</param> 
		/// <param name="width">宽</param> 
		/// <param name="height">高</param> 
		private void setGifSize(string path, int width, int height) {
			var gif = new Bitmap(width, height);
			var frame = new Bitmap(width, height);
			var res = Image.FromFile(path);
			var g = Graphics.FromImage(gif);
			var rg = new Rectangle(0, 0, width, height);
			var gFrame = Graphics.FromImage(frame);
			foreach (var gd in res.FrameDimensionsList) {
				var fd = new FrameDimension(gd);
				//因为是缩小GIF文件所以这里要设置为Time，如果是TIFF这里要设置为PAGE，因为GIF以时间分割，TIFF为页分割 
				var count = res.GetFrameCount(fd);
				var codecInfo = GetEncoder(ImageFormat.Gif);
				System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.SaveFlag;
				EncoderParameters eps = null;
				for (int i = 0; i < count; i++) {
					res.SelectActiveFrame(FrameDimension.Time, i);
					if (0 == i) {
						g.DrawImage(res, rg);
						eps = new EncoderParameters(1);
						//第一帧需要设置为MultiFrame 
						eps.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.MultiFrame);
						bindProperty(res, gif);
						gif.Save(@"C:\tmp\test\aaa.gif", codecInfo, eps);
					} else {
						gFrame.DrawImage(res, rg);
						eps = new EncoderParameters(1);
						//如果是GIF这里设置为FrameDimensionTime，如果为TIFF则设置为FrameDimensionPage 
						eps.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.FrameDimensionTime);
						bindProperty(res, frame);
						gif.SaveAdd(frame, eps);
					}
				}
				eps = new EncoderParameters(1);
				eps.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.Flush);
				gif.SaveAdd(eps);
			}
		}

		/// <summary> 
		/// 将源图片文件里每一帧的属性设置到新的图片对象里 
		/// </summary> 
		/// <param name="a">源图片帧</param> 
		/// <param name="b">新的图片帧</param> 
		private void bindProperty(Image a, Image b) {

			//这个东西就是每一帧所拥有的属性，可以用GetPropertyItem方法取得这里用为完全复制原有属性所以直接赋值了 

			//顺便说一下这个属性里包含每帧间隔的秒数和透明背景调色板等设置，这里具体那个值对应那个属性大家自己在msdn搜索GetPropertyItem方法说明就有了 

			for (int i = 0; i < a.PropertyItems.Length; i++) {
				b.SetPropertyItem(a.PropertyItems[i]);
			}
		}

		private ImageCodecInfo GetEncoder(ImageFormat format) {
			var codecs = ImageCodecInfo.GetImageDecoders();
			foreach (ImageCodecInfo codec in codecs) {
				if (codec.FormatID == format.Guid) {
					return codec;
				}
			}
			return null;
		}
	}
}
