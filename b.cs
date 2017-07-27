using socks5.Controller;
using socks5.Model;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Timers;

internal class b
{
	private class a : IHandler
	{
		private Configuration a;

		private byte[] b;

		private int c;

		private ProxySocketTunLocal d;

		private ProxySocketTun e;

		private bool f;

		private bool g;

		private string h;

		private int i;

		public const int j = 11680;

		private byte[] k = new byte[11680];

		private byte[] l = new byte[11680];

		private int m;

		protected int n = 600;

		protected System.Timers.Timer o;

		protected object p = new object();

		protected DateTime q;

		public void a(Configuration A_0, byte[] A_1, int A_2, Socket A_3, string A_4, bool A_5)
		{
			this.b = A_1;
			this.c = A_2;
			this.d = new ProxySocketTunLocal(A_3);
			this.d.local_sendback_protocol = A_4;
			this.a = A_0;
			this.g = A_5;
			this.d();
		}

		private void d()
		{
			try
			{
				IPAddress iPAddress = null;
				int port = 0;
				if (this.b[0] == 1)
				{
					byte[] array = new byte[4];
					Array.Copy(this.b, 1, array, 0, array.Length);
					iPAddress = new IPAddress(array);
					port = ((int)this.b[5] << 8 | (int)this.b[6]);
					this.h = iPAddress.ToString();
				}
				else if (this.b[0] == 4)
				{
					byte[] array2 = new byte[16];
					Array.Copy(this.b, 1, array2, 0, array2.Length);
					iPAddress = new IPAddress(array2);
					port = ((int)this.b[17] << 8 | (int)this.b[18]);
					this.h = iPAddress.ToString();
				}
				else if (this.b[0] == 3)
				{
					int num = (int)this.b[1];
					byte[] array3 = new byte[num];
					Array.Copy(this.b, 2, array3, 0, array3.Length);
					this.h = Encoding.UTF8.GetString(this.b, 2, num);
					port = ((int)this.b[num + 2] << 8 | (int)this.b[num + 3]);
					if (!IPAddress.TryParse(this.h, out iPAddress) && iPAddress == null)
					{
						iPAddress = global::a.d().Get(this.h);
					}
					if (iPAddress == null)
					{
						iPAddress = global::a.a(this.h, null, false);
					}
					if (iPAddress != null)
					{
						global::a.d().Set(this.h, new IPAddress(iPAddress.GetAddressBytes()));
						global::a.d().Sweep();
					}
					else if (!this.g)
					{
						throw new SocketException(11001);
					}
				}
				this.i = port;
				IPEndPoint ep = new IPEndPoint(iPAddress, port);
				this.e = new ProxySocketTun(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				this.e.GetSocket().NoDelay = true;
				this.e.BeginConnect(ep, new AsyncCallback(this.c), null);
			}
			catch (Exception arg_1D1_0)
			{
				Logging.LogUsefulException(arg_1D1_0);
				this.e();
			}
		}

		private bool a(string A_0, int A_1)
		{
			return true;
		}

		private void c(IAsyncResult A_0)
		{
			if (this.f)
			{
				return;
			}
			try
			{
				this.e.EndConnect(A_0);
				if (this.g && !this.a(this.h, this.i))
				{
					throw new SocketException(10054);
				}
				this.c();
			}
			catch (Exception arg_45_0)
			{
				Logging.LogUsefulException(arg_45_0);
				this.e();
			}
		}

		private void a(double A_0)
		{
			if (A_0 <= 0.0 && this.o == null)
			{
				return;
			}
			if (A_0 <= 0.0)
			{
				if (this.o == null)
				{
					return;
				}
				object obj = this.p;
				lock (obj)
				{
					if (this.o != null)
					{
						this.o.Enabled = false;
						this.o.Elapsed -= new ElapsedEventHandler(this.a);
						this.o.Dispose();
						this.o = null;
					}
					return;
				}
			}
			DateTime arg_8F_0 = this.q;
			if ((DateTime.Now - this.q).TotalMilliseconds > 500.0)
			{
				object obj = this.p;
				lock (obj)
				{
					if (this.o == null)
					{
						this.o = new System.Timers.Timer(A_0 * 1000.0);
						this.o.Elapsed += new ElapsedEventHandler(this.a);
					}
					else
					{
						this.o.Interval = A_0 * 1000.0;
						this.o.Stop();
					}
					this.o.Start();
					this.q = DateTime.Now;
				}
			}
		}

		private void a(object A_0, ElapsedEventArgs A_1)
		{
			if (this.f)
			{
				return;
			}
			this.e();
		}

		private void c()
		{
			if (this.f)
			{
				return;
			}
			try
			{
				this.e.BeginReceive(this.k, 11680, SocketFlags.None, new AsyncCallback(this.b), null);
				this.d.BeginReceive(this.l, 11680, SocketFlags.None, new AsyncCallback(this.a), null);
				this.d.Send(this.l, 0, SocketFlags.None);
				this.a((double)this.n);
			}
			catch (Exception arg_77_0)
			{
				Logging.LogUsefulException(arg_77_0);
				this.e();
			}
		}

		private void b(IAsyncResult A_0)
		{
			if (this.f)
			{
				return;
			}
			try
			{
				int num = this.e.EndReceive(A_0);
				if (num > 0)
				{
					this.a((double)this.n);
					this.d.Send(this.k, num, SocketFlags.None);
					if (Analyzer.instance().Add(this.h, this.i, this.e, this.k, num, true))
					{
						Analyzer.instance().Close(this.h, this.i, this.e);
					}
					this.m += num;
					if (this.m <= 2097152)
					{
						this.e.BeginReceive(this.k, 11680, SocketFlags.None, new AsyncCallback(this.b), null);
					}
					else
					{
						this.b();
					}
				}
				else
				{
					this.e();
				}
			}
			catch (Exception arg_D3_0)
			{
				Logging.LogUsefulException(arg_D3_0);
				this.e();
			}
		}

		private void b()
		{
			bool flag = false;
			byte[] buffer = new byte[11680];
			DateTime d = DateTime.Now;
			while (!this.f)
			{
				try
				{
					int num = this.e.Receive(buffer, 11680, SocketFlags.None);
					DateTime now = DateTime.Now;
					if (this.e != null && this.e.IsClose)
					{
						flag = true;
						break;
					}
					if (this.f)
					{
						break;
					}
					this.a((double)this.n);
					if (num > 0)
					{
						this.d.Send(buffer, num, SocketFlags.None);
						if (Analyzer.instance().Add(this.h, this.i, this.e, buffer, num, true))
						{
							Analyzer.instance().Close(this.h, this.i, this.e);
						}
						if ((now - d).TotalSeconds > 5.0)
						{
							this.m = 0;
							this.e.BeginReceive(this.k, 11680, SocketFlags.None, new AsyncCallback(this.b), null);
							return;
						}
						d = now;
					}
					else
					{
						this.e();
					}
				}
				catch (Exception arg_113_0)
				{
					Logging.LogUsefulException(arg_113_0);
					flag = true;
					break;
				}
			}
			if (flag)
			{
				this.e();
				return;
			}
		}

		private void a(IAsyncResult A_0)
		{
			if (this.f)
			{
				return;
			}
			try
			{
				int num = this.d.EndReceive(A_0);
				if (num > 0)
				{
					this.a((double)this.n);
					this.e.Send(this.l, num, SocketFlags.None);
					if (Analyzer.instance().Add(this.h, this.i, this.e, this.l, num, false))
					{
						Analyzer.instance().Close(this.h, this.i, this.e);
					}
					this.d.BeginReceive(this.l, 11680, SocketFlags.None, new AsyncCallback(this.a), null);
				}
				else
				{
					this.e();
				}
			}
			catch (Exception arg_B0_0)
			{
				Logging.LogUsefulException(arg_B0_0);
				this.e();
			}
		}

		private void a(ProxySocketTun A_0)
		{
			lock (this)
			{
				if (A_0 != null)
				{
					try
					{
						A_0.Shutdown(SocketShutdown.Both);
					}
					catch
					{
					}
					try
					{
						A_0.Close();
					}
					catch
					{
					}
				}
			}
		}

		public void e()
		{
			lock (this)
			{
				if (this.f)
				{
					return;
				}
				this.f = true;
			}
			this.a(0.0);
			Thread.Sleep(100);
			Analyzer.instance().Close(this.h, this.i, this.e);
			this.a(this.e);
			this.a(this.d);
		}

		public override void Shutdown()
		{
			new IHandler.InvokeHandler(this.a).BeginInvoke(null, null);
		}

		[CompilerGenerated]
		private void a()
		{
			this.e();
		}
	}

	private const int a = 1;

	private const int b = 2;

	private const int c = 0;

	private Configuration d;

	public b(Configuration A_0)
	{
		this.d = A_0;
	}

	public bool b(byte[] A_0, int A_1, Socket A_2)
	{
		return this.a(A_0, A_1, A_2, null);
	}

	public bool a(byte[] A_0, int A_1, Socket A_2, string A_3)
	{
		if (this.a(A_0, A_1, A_2) > 0)
		{
			new b.a().a(this.d, A_0, A_1, A_2, A_3, false);
			return true;
		}
		return false;
	}

	public int a(byte[] A_0, int A_1, Socket A_2)
	{
		return 2;
	}
}
