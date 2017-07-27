using System;
using System.Net.Sockets;
using System.Text;

namespace socks5.Controller
{
	public class ProxySocketTunLocal : ProxySocketTun
	{
		public string local_sendback_protocol;

		public ProxySocketTunLocal(Socket socket) : base(socket)
		{
		}

		public ProxySocketTunLocal(AddressFamily af, SocketType type, ProtocolType protocol) : base(af, type, protocol)
		{
		}

		public override int Send(byte[] buffer, int size, SocketFlags flags)
		{
			if (this.local_sendback_protocol != null)
			{
				if (this.local_sendback_protocol == "http")
				{
					byte[] bytes = Encoding.UTF8.GetBytes("HTTP/1.1 200 Connection Established\r\n\r\n");
					this._socket.Send(bytes, bytes.Length, SocketFlags.None);
				}
				else if (this.local_sendback_protocol == "socks5")
				{
					if (this._socket.AddressFamily == AddressFamily.InterNetwork)
					{
						byte[] expr_67 = new byte[10];
						expr_67[0] = 5;
						expr_67[3] = 1;
						byte[] buffer2 = expr_67;
						this._socket.Send(buffer2);
					}
					else
					{
						byte[] expr_86 = new byte[22];
						expr_86[0] = 5;
						expr_86[3] = 4;
						byte[] buffer3 = expr_86;
						this._socket.Send(buffer3);
					}
				}
				this.local_sendback_protocol = null;
			}
			return base.SendAll(buffer, size, SocketFlags.None);
		}
	}
}
