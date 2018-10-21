using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gods.Web {
	/// <summary>
	/// HttpListenner监听Post请求参数值实体
	/// </summary>
	public class HttpListenerPostValue {
		/// <summary>
		/// 0=> 参数
		/// 1=> 文件
		/// </summary>
		public int type = 0;
		public string name;
		public byte[] datas;
	}

	/// <summary>
	/// 获取Post请求中的参数和值帮助类
	/// </summary>
	public class HttpListenerPostParaHelper {
		private HttpListenerContext request;

		public HttpListenerPostParaHelper(HttpListenerContext request) {
			this.request = request;
		}

		private bool CompareBytes(byte[] source, byte[] comparison) {
			try {
				if (source.Length != comparison.Length) {
					return false;
				}
				for (int i = 0; i < source.Length; i++)
					if (source[i] != comparison[i])
						return false;
				return true;
			} catch {
			}
			return false;
		}

		private byte[] ReadLineAsBytes(Stream SourceStream) {
			var ms = new MemoryStream();
			while (true) {
				int data = SourceStream.ReadByte();
				ms.WriteByte((byte)data);
				if (data == 10)
					break;
			}
			ms.Position = 0;
			var dataBytes = new byte[ms.Length];
			ms.Read(dataBytes, 0, dataBytes.Length);
			return dataBytes;
		}

		/// <summary>
		/// 获取Post过来的参数和数据
		/// </summary>
		/// <returns></returns>
		public List<HttpListenerPostValue> GetHttpListenerPostValue() {
			var HttpListenerPostValueList = new List<HttpListenerPostValue>();
			try {
				if (request.Request.ContentType.Length > 20 && string.Compare(request.Request.ContentType.Substring(0, 20), "multipart/form-data;", true) == 0) {
					var HttpListenerPostValue = request.Request.ContentType.Split(';').Skip(1).ToArray();
					var boundary = string.Join(";", HttpListenerPostValue).Replace("boundary=", "").Trim();
					var chunkBoundary = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
					var endBoundary = Encoding.UTF8.GetBytes("--" + boundary + "--\r\n");
					var stream = request.Request.InputStream;
					var resultStream = new MemoryStream();
					var CanMoveNext = true;
					HttpListenerPostValue data = null;
					while (CanMoveNext) {
						byte[] currentChunk = ReadLineAsBytes(stream);
						if (!Encoding.UTF8.GetString(currentChunk).Equals("\r\n"))
							resultStream.Write(currentChunk, 0, currentChunk.Length);
						if (CompareBytes(chunkBoundary, currentChunk)) {
							byte[] result = new byte[resultStream.Length - chunkBoundary.Length];
							resultStream.Position = 0;
							resultStream.Read(result, 0, result.Length);
							if (result.Length > 0) {
								data.datas = result;
							}
							data = new HttpListenerPostValue();
							HttpListenerPostValueList.Add(data);
							resultStream.Dispose();
							resultStream = new MemoryStream();

						} else if (Encoding.UTF8.GetString(currentChunk).Contains("Content-Disposition")) {
							byte[] result = new byte[resultStream.Length - 2];
							resultStream.Position = 0;
							resultStream.Read(result, 0, result.Length);
							data.name = Encoding.UTF8.GetString(result).Replace("Content-Disposition: form-data; name=\"", "").Replace("\"", "").Split(';')[0];
							resultStream.Dispose();
							resultStream = new MemoryStream();
						} else if (Encoding.UTF8.GetString(currentChunk).Contains("Content-Type")) {
							data.type = 1;
							resultStream.Dispose();
							resultStream = new MemoryStream();
						} else if (CompareBytes(endBoundary, currentChunk)) {
							byte[] result = new byte[resultStream.Length - endBoundary.Length - 2];
							resultStream.Position = 0;
							resultStream.Read(result, 0, result.Length);
							data.datas = result;
							resultStream.Dispose();
							CanMoveNext = false;
						}
					}
				}
				return HttpListenerPostValueList;
			} catch {
				return null;
			}
		}
	}
}