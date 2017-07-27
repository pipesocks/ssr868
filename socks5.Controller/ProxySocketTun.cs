using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace socks5.Controller
{
	public class ProxySocketTun
	{
		protected Socket _socket;

		protected EndPoint _socketEndPoint;

		protected IPEndPoint _remoteUDPEndPoint;

		protected bool _proxy;

		protected string _proxy_server;

		protected int _proxy_udp_port;

		protected const int RecvSize = 2920;

		private byte[] a = new byte[2920];

		private byte[] b = new byte[5840];

		protected bool _close;

		public bool IsClose
		{
			get
			{
				return this._close;
			}
		}

		public bool GoS5Proxy
		{
			get
			{
				return this._proxy;
			}
			set
			{
				this._proxy = value;
			}
		}

		public AddressFamily AddressFamily
		{
			get
			{
				return this._socket.AddressFamily;
			}
		}

		public int Available
		{
			get
			{
				return this._socket.Available;
			}
		}

		public ProxySocketTun(Socket socket)
		{
			this._socket = socket;
		}

		public ProxySocketTun(AddressFamily af, SocketType type, ProtocolType protocol)
		{
			this._socket = new Socket(af, type, protocol);
		}

		public Socket GetSocket()
		{
			return this._socket;
		}

		public void Shutdown(SocketShutdown how)
		{
			this._socket.Shutdown(how);
		}

		public void Close()
		{
			this._socket.Close();
			this._socket = null;
			this.a = null;
			this.b = null;
		}

		public IAsyncResult BeginConnect(EndPoint ep, AsyncCallback callback, object state)
		{
			this._close = false;
			this._socketEndPoint = ep;
			return this._socket.BeginConnect(ep, callback, state);
		}

		public void EndConnect(IAsyncResult ar)
		{
			this._socket.EndConnect(ar);
		}

		public int Receive(byte[] buffer, int size, SocketFlags flags)
		{
			return this._socket.Receive(buffer, size, SocketFlags.None);
		}

		public IAsyncResult BeginReceive(byte[] buffer, int size, SocketFlags flags, AsyncCallback callback, object state)
		{
			CallbackState callbackState = new CallbackState();
			callbackState.buffer = buffer;
			callbackState.size = size;
			callbackState.state = state;
			return this._socket.BeginReceive(buffer, 0, size, flags, callback, callbackState);
		}

		public int EndReceive(IAsyncResult ar)
		{
			int num = this._socket.EndReceive(ar);
			if (num > 0)
			{
				((CallbackState)ar.AsyncState).size = num;
				return num;
			}
			this._close = true;
			return num;
		}

		public int SendAll(byte[] buffer, int size, SocketFlags flags)
		{
			int num;
			for (int i = this._socket.Send(buffer, size, SocketFlags.None); i < size; i += num)
			{
				num = this._socket.Send(buffer, i, size - i, SocketFlags.None);
			}
			return size;
		}

		public virtual int Send(byte[] buffer, int size, SocketFlags flags)
		{
			return this.SendAll(buffer, size, SocketFlags.None);
		}

		public int BeginSend(byte[] buffer, int size, SocketFlags flags, AsyncCallback callback, object state)
		{
			CallbackState callbackState = new CallbackState();
			callbackState.size = size;
			callbackState.state = state;
			this._socket.BeginSend(buffer, 0, size, SocketFlags.None, callback, callbackState);
			return size;
		}

		public int EndSend(IAsyncResult ar)
		{
			return this._socket.EndSend(ar);
		}

		public IAsyncResult BeginReceiveFrom(byte[] buffer, int size, SocketFlags flags, ref EndPoint ep, AsyncCallback callback, object state)
		{
			CallbackState callbackState = new CallbackState();
			callbackState.buffer = buffer;
			callbackState.size = size;
			callbackState.state = state;
			return this._socket.BeginReceiveFrom(buffer, 0, size, flags, ref ep, callback, callbackState);
		}

		public int GetAsyncResultSize(IAsyncResult ar)
		{
			return ((CallbackState)ar.AsyncState).size;
		}

		public byte[] GetAsyncResultBuffer(IAsyncResult ar)
		{
			return ((CallbackState)ar.AsyncState).buffer;
		}

		public bool ConnectSocks5ProxyServer(string strRemoteHost, int iRemotePort, bool udp, string socks5RemoteUsername, string socks5RemotePassword)
		{
			int errorCode = 10054;
			this._proxy = true;
			byte[] array = new byte[10];
			array[0] = 5;
			array[1] = ((socks5RemoteUsername.Length == 0) ? 1 : 2);
			array[2] = 0;
			array[3] = 2;
			this._socket.Send(array, (int)(array[1] + 2), SocketFlags.None);
			byte[] array2 = new byte[32];
			if (this._socket.Receive(array2, array2.Length, SocketFlags.None) < 2)
			{
				throw new SocketException(errorCode);
			}
			if (array2[0] != 5 || (array2[1] != 0 && array2[1] != 2))
			{
				throw new SocketException(errorCode);
			}
			if (array2[1] != 0)
			{
				if (array2[1] != 2)
				{
					return false;
				}
				if (socks5RemoteUsername.Length == 0)
				{
					throw new SocketException(errorCode);
				}
				array = new byte[socks5RemoteUsername.Length + socks5RemotePassword.Length + 3];
				array[0] = 1;
				array[1] = (byte)socks5RemoteUsername.Length;
				for (int i = 0; i < socks5RemoteUsername.Length; i++)
				{
					array[2 + i] = (byte)socks5RemoteUsername[i];
				}
				array[socks5RemoteUsername.Length + 2] = (byte)socks5RemotePassword.Length;
				for (int j = 0; j < socks5RemotePassword.Length; j++)
				{
					array[socks5RemoteUsername.Length + 3 + j] = (byte)socks5RemotePassword[j];
				}
				this._socket.Send(array, array.Length, SocketFlags.None);
				this._socket.Receive(array2, array2.Length, SocketFlags.None);
				if (array2[0] != 1 || array2[1] != 0)
				{
					throw new SocketException(10061);
				}
			}
			if (!udp)
			{
				List<byte> list = new List<byte>();
				list.Add(5);
				list.Add(1);
				list.Add(0);
				IPAddress iPAddress;
				IPAddress.TryParse(strRemoteHost, out iPAddress);
				if (iPAddress == null)
				{
					list.Add(3);
					list.Add((byte)strRemoteHost.Length);
					for (int k = 0; k < strRemoteHost.Length; k++)
					{
						list.Add((byte)strRemoteHost[k]);
					}
				}
				else
				{
					byte[] addressBytes = iPAddress.GetAddressBytes();
					if (addressBytes.GetLength(0) > 4)
					{
						list.Add(4);
						for (int l = 0; l < 16; l++)
						{
							list.Add(addressBytes[l]);
						}
					}
					else
					{
						list.Add(1);
						for (int m = 0; m < 4; m++)
						{
							list.Add(addressBytes[m]);
						}
					}
				}
				list.Add((byte)(iRemotePort / 256));
				list.Add((byte)(iRemotePort % 256));
				this._socket.Send(list.ToArray(), list.Count, SocketFlags.None);
				if (this._socket.Receive(array2, array2.Length, SocketFlags.None) < 2 || array2[0] != 5 || array2[1] != 0)
				{
					throw new SocketException(errorCode);
				}
				return true;
			}
			else
			{
				List<byte> list2 = new List<byte>();
				list2.Add(5);
				list2.Add(3);
				list2.Add(0);
				IPAddress iPAddress2 = ((IPEndPoint)this._socketEndPoint).Address;
				byte[] addressBytes2 = iPAddress2.GetAddressBytes();
				if (addressBytes2.GetLength(0) > 4)
				{
					list2.Add(4);
					for (int n = 0; n < 16; n++)
					{
						list2.Add(addressBytes2[n]);
					}
				}
				else
				{
					list2.Add(1);
					for (int num = 0; num < 4; num++)
					{
						list2.Add(addressBytes2[num]);
					}
				}
				list2.Add(0);
				list2.Add(0);
				this._socket.Send(list2.ToArray(), list2.Count, SocketFlags.None);
				this._socket.Receive(array2, array2.Length, SocketFlags.None);
				if (array2[0] != 5 || array2[1] != 0)
				{
					throw new SocketException(errorCode);
				}
				byte[] array3;
				int port;
				if (array2[0] != 4)
				{
					array3 = new byte[4];
					Array.Copy(array2, 4, array3, 0, 4);
					port = (int)array2[8] * 256 + (int)array2[9];
				}
				else
				{
					array3 = new byte[16];
					Array.Copy(array2, 4, array3, 0, 16);
					port = (int)array2[20] * 256 + (int)array2[21];
				}
				iPAddress2 = new IPAddress(array3);
				this._remoteUDPEndPoint = new IPEndPoint(iPAddress2, port);
				return true;
			}
		}

		public void SetTcpServer(string server, int port)
		{
			this._proxy_server = server;
			this._proxy_udp_port = port;
		}

		public void SetUdpServer(string server, int port)
		{
			this._proxy_server = server;
			this._proxy_udp_port = port;
		}

		public void SetUdpEndPoint(IPEndPoint ep)
		{
			this._remoteUDPEndPoint = ep;
		}

		public bool ConnectHttpProxyServer(string strRemoteHost, int iRemotePort, string socks5RemoteUsername, string socks5RemotePassword, string proxyUserAgent)
		{
			this._proxy = true;
			IPAddress iPAddress;
			IPAddress.TryParse(strRemoteHost, out iPAddress);
			if (iPAddress != null)
			{
				strRemoteHost = iPAddress.ToString();
			}
			string text = ((strRemoteHost.IndexOf(':') >= 0) ? ("[" + strRemoteHost + "]") : strRemoteHost) + ":" + iRemotePort.ToString();
			string str = Convert.ToBase64String(Encoding.UTF8.GetBytes(socks5RemoteUsername + ":" + socks5RemotePassword));
			string text2 = string.Concat(new string[]
			{
				"CONNECT ",
				text,
				" HTTP/1.0\r\nHost: ",
				text,
				"\r\n"
			});
			if (!string.IsNullOrEmpty(proxyUserAgent))
			{
				text2 = text2 + "User-Agent: " + proxyUserAgent + "\r\n";
			}
			text2 += "Proxy-Connection: Keep-Alive\r\n";
			if (socks5RemoteUsername.Length > 0)
			{
				text2 = text2 + "Proxy-Authorization: Basic " + str + "\r\n";
			}
			text2 += "\r\n";
			byte[] bytes = Encoding.UTF8.GetBytes(text2);
			this._socket.Send(bytes, bytes.Length, SocketFlags.None);
			byte[] array = new byte[1024];
			int num = this._socket.Receive(array, array.Length, SocketFlags.None);
			if (num > 13)
			{
				string[] array2 = Encoding.UTF8.GetString(array, 0, num).Split(new char[]
				{
					' '
				});
				if (array2.Length > 1 && array2[1] == "200")
				{
					return true;
				}
			}
			return false;
		}
	}
}
