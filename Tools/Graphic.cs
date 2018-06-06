using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Tools {
	public abstract class Gif {
		public Gif(Image image) {
			Image = image;
		}

		public Image Image { get; }

		public abstract void Read();

		public void Start() {
			new Task(() => {
				var dim = new FrameDimension(Image.FrameDimensionsList[0]);
				for (int i = 0; i < Image.GetFrameCount(dim); i++) {
					Image.SelectActiveFrame(dim, i);
					for (int j = 0; j < Image.PropertyIdList.Length; j++) {
						if ((int)Image.PropertyIdList.GetValue(j) == 0x5100) {
							PropertyItem pItem = (PropertyItem)Image.PropertyItems.GetValue(j);//获取延迟时间属性
							byte[] delayByte = new byte[4];//延迟时间，以1/100秒为单位
							delayByte[0] = pItem.Value[i * 4];
							delayByte[1] = pItem.Value[1 + i * 4];
							delayByte[2] = pItem.Value[2 + i * 4];
							delayByte[3] = pItem.Value[3 + i * 4];
							int delay = BitConverter.ToInt32(delayByte, 0) * 10; //乘以10，获取到毫秒
							Read();
							System.Threading.Thread.Sleep(delay);
							break;
						}
					}
				}
			}).Start();
		}
	}
}
