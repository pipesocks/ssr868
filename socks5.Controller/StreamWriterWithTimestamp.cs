using System;
using System.IO;

namespace socks5.Controller
{
	public class StreamWriterWithTimestamp : StreamWriter
	{
		public StreamWriterWithTimestamp(Stream stream) : base(stream)
		{
		}

		private string a()
		{
			return "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ";
		}

		public override void WriteLine(string value)
		{
			base.WriteLine(this.a() + value);
		}

		public override void Write(string value)
		{
			base.Write(this.a() + value);
		}
	}
}
