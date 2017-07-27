using socks5.Controller;
using socks5.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Timers;

internal class d
{
	public interface b
	{
		bool a(byte[] A_0, int A_1, Socket A_2);
	}

	[CompilerGenerated]
	private sealed class a
	{
		public Socket a;

		public d b;

		internal void d(object A_0, ElapsedEventArgs A_1)
		{
			this.b.a(A_0, A_1, this.a);
		}

		internal void c(object A_0, ElapsedEventArgs A_1)
		{
			this.b.a(A_0, A_1, this.a);
		}
	}

	private Configuration a;

	private bool b;

	private string c;

	private Socket d;

	private Socket e;

	private bool f;

	private IList<d.b> g;

	protected Timer h;

	protected object i = new object();

	public d(IList<d.b> A_0)
	{
		this.g = A_0;
		this.f = false;
	}

	public IList<d.b> b()
	{
		return this.g;
	}

	private bool a(int A_0)
	{
		try
		{
			IPEndPoint[] activeTcpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
			for (int i = 0; i < activeTcpListeners.Length; i++)
			{
				if (activeTcpListeners[i].Port == A_0)
				{
					return true;
				}
			}
		}
		catch
		{
		}
		return false;
	}

	public bool a(Configuration A_0)
	{
		try
		{
			if (this.b != A_0.shareOverLan || this.d == null || ((IPEndPoint)this.d.LocalEndPoint).Port != A_0.localPort)
			{
				return true;
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	public void a(Configuration A_0, int A_1)
	{
		this.a = A_0;
		this.b = A_0.shareOverLan;
		this.c = "";
		this.f = false;
		int num = (A_1 == 0) ? this.a.localPort : A_1;
		if (this.a(num))
		{
			throw new Exception("Port already in use");
		}
		try
		{
			bool arg_6D_0 = true;
			this.d = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.d.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			if (arg_6D_0)
			{
				try
				{
					this.e = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
					this.e.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
				}
				catch
				{
					this.e = null;
				}
			}
			IPEndPoint localEP = new IPEndPoint(IPAddress.Any, num);
			IPEndPoint localEP2 = new IPEndPoint(IPAddress.IPv6Any, num);
			if (this.e != null)
			{
				this.e.Bind(localEP2);
				this.e.Listen(1024);
			}
			this.d.Bind(localEP);
			this.d.Listen(1024);
			Console.WriteLine("socks5 started on port " + num.ToString());
			this.d.BeginAccept(new AsyncCallback(this.b), this.d);
			if (this.e != null)
			{
				this.e.BeginAccept(new AsyncCallback(this.b), this.e);
			}
		}
		catch (SocketException arg_154_0)
		{
			Logging.LogUsefulException(arg_154_0);
			if (this.d != null)
			{
				this.d.Close();
				this.d = null;
			}
			if (this.e != null)
			{
				this.e.Close();
				this.e = null;
			}
			throw;
		}
	}

	public void a()
	{
		this.a(0.0, null);
		this.f = true;
		if (this.d != null)
		{
			this.d.Close();
			this.d = null;
		}
		if (this.e != null)
		{
			this.e.Close();
			this.e = null;
		}
	}

	private void a(double A_0, Socket A_1)
	{
		d.a a = new d.a();
		a.b = this;
		a.a = A_1;
		if (A_0 <= 0.0 && this.h == null)
		{
			return;
		}
		object obj = this.i;
		lock (obj)
		{
			if (A_0 <= 0.0)
			{
				if (this.h != null)
				{
					this.h.Enabled = false;
					this.h.Elapsed -= new ElapsedEventHandler(a.d);
					this.h.Dispose();
					this.h = null;
				}
			}
			else if (this.h == null)
			{
				this.h = new Timer(A_0 * 1000.0);
				this.h.Elapsed += new ElapsedEventHandler(a.c);
				this.h.Start();
			}
			else
			{
				this.h.Interval = A_0 * 1000.0;
				this.h.Stop();
				this.h.Start();
			}
		}
	}

	private void a(object A_0, ElapsedEventArgs A_1, Socket A_2)
	{
		if (this.h == null)
		{
			return;
		}
		try
		{
			A_2.BeginAccept(new AsyncCallback(this.b), A_2);
			this.a(0.0, A_2);
		}
		catch (ObjectDisposedException)
		{
		}
		catch (Exception arg_34_0)
		{
			Logging.LogUsefulException(arg_34_0);
			this.a(5.0, A_2);
		}
	}

	public void b(IAsyncResult A_0)
	{
		if (this.f)
		{
			return;
		}
		Socket socket = (Socket)A_0.AsyncState;
		try
		{
			Socket socket2 = socket.EndAccept(A_0);
			if (!this.b && !global::a.b(socket2))
			{
				socket2.Shutdown(SocketShutdown.Both);
				socket2.Close();
			}
			if ((this.c ?? "").Length == 0 && !global::a.a(socket2))
			{
				socket2.Shutdown(SocketShutdown.Both);
				socket2.Close();
			}
			else
			{
				byte[] array = new byte[4096];
				object[] state = new object[]
				{
					socket2,
					array
				};
				socket2.BeginReceive(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.a), state);
			}
		}
		catch (ObjectDisposedException)
		{
		}
		catch (Exception arg_A0_0)
		{
			Console.WriteLine(arg_A0_0);
		}
		finally
		{
			try
			{
				socket.BeginAccept(new AsyncCallback(this.b), socket);
			}
			catch (ObjectDisposedException)
			{
			}
			catch (Exception arg_C1_0)
			{
				Logging.LogUsefulException(arg_C1_0);
				this.a(5.0, socket);
			}
		}
	}

	private void a(IAsyncResult A_0)
	{
		object[] expr_0B = (object[])A_0.AsyncState;
		Socket socket = (Socket)expr_0B[0];
		byte[] a_ = (byte[])expr_0B[1];
		try
		{
			int a_2 = socket.EndReceive(A_0);
			using (IEnumerator<d.b> enumerator = this.g.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.a(a_, a_2, socket))
					{
						return;
					}
				}
			}
			socket.Shutdown(SocketShutdown.Both);
			socket.Close();
		}
		catch (Exception arg_67_0)
		{
			Console.WriteLine(arg_67_0);
			socket.Shutdown(SocketShutdown.Both);
			socket.Close();
		}
	}
}
