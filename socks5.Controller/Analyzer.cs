using System;
using System.Collections.Generic;

namespace socks5.Controller
{
	public class Analyzer
	{
		private static Analyzer a = new Analyzer();

		private Dictionary<ProxySocketTun, h> b = new Dictionary<ProxySocketTun, h>();

		private Dictionary<string, g> c = new Dictionary<string, g>();

		public static Analyzer instance()
		{
			return Analyzer.a;
		}

		public bool Add(string remote_host, int remote_port, ProxySocketTun socket, byte[] buffer, int size, bool remote)
		{
			if (socket == null)
			{
				return false;
			}
			h h = null;
			Dictionary<ProxySocketTun, h> obj = this.b;
			lock (obj)
			{
				if (this.b.ContainsKey(socket))
				{
					h = this.b[socket];
				}
				else
				{
					h = new h();
					this.b[socket] = h;
				}
			}
			if (remote)
			{
				return h.f(buffer, size);
			}
			return h.a(buffer, size);
		}

		public void Close(string remote_host, int remote_port, ProxySocketTun socket)
		{
			if (socket != null)
			{
				h h = null;
				Dictionary<ProxySocketTun, h> obj = this.b;
				lock (obj)
				{
					if (this.b.ContainsKey(socket))
					{
						h = this.b[socket];
						this.b.Remove(socket);
					}
				}
				if (h != null)
				{
					g g = null;
					Dictionary<string, g> obj2 = this.c;
					lock (obj2)
					{
						string text = remote_host + ":" + remote_port.ToString();
						if (this.c.ContainsKey(text))
						{
							g = this.c[text];
						}
						else
						{
							g = new g(text);
							this.c[text] = g;
						}
					}
					g.a(h);
				}
			}
		}
	}
}
