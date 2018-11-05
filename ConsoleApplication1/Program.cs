using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// HttpListenner监听Post请求参数值实体
/// </summary>
public class HttpListenerPostValue {
	/// <summary>
	/// 0=> 参数
	/// 1=> 文件
	/// </summary>
	public int Type { get; set; }
	public string Name { get; set; }
	public byte[] Data { get; set; }
	public string DataAsString { get; set; }
}

/// <summary>
/// 获取Post请求中的参数和值帮助类
/// </summary>
public class HttpListenerPostParaHelper {
	private HttpListenerContext context;

	public HttpListenerPostParaHelper(HttpListenerContext context) {
		this.context = context;
	}

	private bool CompareBytes(byte[] source, byte[] comparison) {
		try {
			int count = source.Length;
			if (source.Length != comparison.Length)
				return false;
			for (int i = 0; i < count; i++)
				if (source[i] != comparison[i])
					return false;
			return true;
		} catch {
			return false;
		}
	}

	private byte[] ReadLineAsBytes(Stream SourceStream) {
		var resultStream = new MemoryStream();
		while (true) {
			int data = SourceStream.ReadByte();
			resultStream.WriteByte((byte)data);
			if (data == 10)
				break;
		}
		resultStream.Position = 0;
		var dataBytes = new byte[resultStream.Length];
		resultStream.Read(dataBytes, 0, dataBytes.Length);
		return dataBytes;
	}

	private static string[] form = { "application/x-www-form-urlencoded", "multipart/form-data", "text/plain" };

	/// <summary>
	/// 获取Post过来的参数和数据
	/// </summary>
	/// <returns></returns>
	public List<HttpListenerPostValue> GetHttpListenerPostValue() {
		if (context.Request.ContentType.StartsWith("multipart/form-data")) {
			var nc = context.Request.ContentEncoding;
			var r = new List<HttpListenerPostValue>();
			var HttpListenerPostValue = context.Request.ContentType.Split(';').Skip(1).ToArray();
			var boundary = context.Request.ContentType.Replace("boundary=", "]").Split(']')[1].Trim();
			var chunkBoundary = nc.GetBytes("--" + boundary + "\r\n");
			var EndBoundary = nc.GetBytes("--" + boundary + "--\r\n");
			var resultStream = new MemoryStream();
			var ongoing = true;
			HttpListenerPostValue data = null;
			while (ongoing) {
				var currentChunk = ReadLineAsBytes(context.Request.InputStream);
				var currentChunkString = nc.GetString(currentChunk);
				if (currentChunkString != "\r\n") {
					resultStream.Write(currentChunk, 0, currentChunk.Length);
				}
				if (CompareBytes(chunkBoundary, currentChunk)) {
					var result = new byte[resultStream.Length - chunkBoundary.Length];
					resultStream.Position = 0;
					resultStream.Read(result, 0, result.Length);
					if (result.Length > 0) {
						data.Data = result;
					}
					data = new HttpListenerPostValue();
					r.Add(data);
					resultStream.Dispose();
					resultStream = new MemoryStream();
				} else if (currentChunkString.Contains("Content-Disposition")) {
					var result = new byte[resultStream.Length - 2];
					resultStream.Position = 0;
					resultStream.Read(result, 0, result.Length);
					data.Name = nc.GetString(result).Replace("Content-Disposition: form-data; name=\"", "").Replace("\"", "").Split(';')[0];
					resultStream.Dispose();
					resultStream = new MemoryStream();
				} else if (currentChunkString.Contains("Content-Type")) {
					data.Type = 1;
					resultStream.Dispose();
					resultStream = new MemoryStream();
				} else if (CompareBytes(EndBoundary, currentChunk)) {
					var result = new byte[resultStream.Length - EndBoundary.Length - 2];
					resultStream.Position = 0;
					resultStream.Read(result, 0, result.Length);
					data.Data = result;
					resultStream.Dispose();
					ongoing = false;
				}
			}
			return r;
		}
		using (var reader = new StreamReader(context.Request.InputStream)) {
			return reader.ReadToEnd().Split('&').Select(e => e.Split('=')).Select(e => new HttpListenerPostValue() { Name = e[0], DataAsString = e[1].Trim() }).ToList();
		}
	}
}

class Program {
	private static HttpListener httpPostRequest = new HttpListener();

	static void Main(string[] args) {
		while (true) {
			var ss = Console.ReadLine();
			var s = byte.Parse(ss);
			Console.WriteLine(Encoding.UTF8.GetString(new byte[] { 240, 159, 149, s }));
		}
	}

	private static void TestHttpListener() {
		httpPostRequest.Prefixes.Add("http://127.0.0.1:30000/posttype/");
		httpPostRequest.Start();

		Thread ThrednHttpPostRequest = new Thread(new ThreadStart(httpPostRequestHandle));
		ThrednHttpPostRequest.Start();
	}

	private static void httpPostRequestHandle() {
		while (true) {
			HttpListenerContext requestContext = httpPostRequest.GetContext();
			Thread threadsub = new Thread(new ParameterizedThreadStart((requestcontext) => {
				HttpListenerContext request = (HttpListenerContext)requestcontext;

				//获取Post请求中的参数和值帮助类
				HttpListenerPostParaHelper httppost = new HttpListenerPostParaHelper(request);
				//获取Post过来的参数和数据
				List<HttpListenerPostValue> lst = httppost.GetHttpListenerPostValue();

				string userName = "";
				string password = "";
				string suffix = "";
				string adType = "";

				//使用方法
				foreach (var key in lst) {
					if (key.Type == 0) {
						string value = Encoding.UTF8.GetString(key.Data).Replace("\r\n", "");
						if (key.Name == "username") {
							userName = value;
							Console.WriteLine(value);
						}
						if (key.Name == "password") {
							password = value;
							Console.WriteLine(value);
						}
						if (key.Name == "suffix") {
							suffix = value;
							Console.WriteLine(value);
						}
						if (key.Name == "adtype") {
							adType = value;
							Console.WriteLine(value);
						}
					}
					if (key.Type == 1) {
						string fileName = request.Request.QueryString["FileName"];
						if (!string.IsNullOrEmpty(fileName)) {
							string filePath = AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("yyMMdd_HHmmss_ffff") + Path.GetExtension(fileName).ToLower();
							if (key.Name == "File") {
								FileStream fs = new FileStream(filePath, FileMode.Create);
								fs.Write(key.Data, 0, key.Data.Length);
								fs.Close();
								fs.Dispose();
							}
						}
					}
				}

				//Response
				request.Response.StatusCode = 200;
				request.Response.Headers.Add("Access-Control-Allow-Origin", "*");
				request.Response.ContentType = "application/json";
				requestContext.Response.ContentEncoding = Encoding.UTF8;
			}));
			threadsub.Start(requestContext);
		}
	}
}