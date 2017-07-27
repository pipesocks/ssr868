using socks5.Controller;
using socks5.Model;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;

internal class f
{
	private class a : d.b
	{
		private delegate void b();

		[CompilerGenerated]
		private sealed class a
		{
			public byte[] a;

			public int b;

			public Socket c;

			public f.a d;

			internal void e()
			{
				new c(this.d.a, this.a, this.b, this.c);
			}
		}

		private Configuration a;

		public a(Configuration A_0)
		{
			this.a = A_0;
		}

		protected bool a(byte[] A_0, int A_1)
		{
			return A_1 >= 2 && (A_0[0] == 5 || A_0[0] == 4 || (A_1 > 8 && A_0[0] == 67 && A_0[1] == 79 && A_0[2] == 78 && A_0[3] == 78 && A_0[4] == 69 && A_0[5] == 67 && A_0[6] == 84 && A_0[7] == 32));
		}

		public bool a(byte[] A_0, int A_1, Socket A_2)
		{
			f.a.a a = new f.a.a();
			a.d = this;
			a.a = A_0;
			a.b = A_1;
			a.c = A_2;
			if (!this.a(a.a, a.b))
			{
				return false;
			}
			new f.a.b(a.e).BeginInvoke(null, null);
			return true;
		}
	}

	private d a;

	private Configuration b;

	private bool c;

	protected void b()
	{
		this.b = new Configuration();
		bool flag = false;
		for (int i = 1; i <= 5; i++)
		{
			try
			{
				if (this.a != null && !this.a.a(this.b))
				{
					f.a value = new f.a(this.b);
					this.a.b()[0] = value;
				}
				else
				{
					if (this.a != null)
					{
						this.a.a();
						this.a = null;
					}
					f.a item = new f.a(this.b);
					this.a = new d(new List<d.b>
					{
						item
					});
					this.a.a(this.b, 0);
				}
				break;
			}
			catch (Exception ex)
			{
				if (ex is SocketException && ((SocketException)ex).SocketErrorCode == SocketError.AccessDenied)
				{
					ex = new Exception("Port already in use" + string.Format(" {0}", this.b.localPort), ex);
				}
				Logging.LogUsefulException(ex);
				if (!flag)
				{
					break;
				}
				Thread.Sleep(1000 * i * i);
				if (this.a != null)
				{
					this.a.a();
					this.a = null;
				}
			}
		}
		global::a.c();
	}

	public void a()
	{
		if (this.c)
		{
			return;
		}
		this.c = true;
		if (this.a != null)
		{
			this.a.a();
		}
	}

	private static void a(string[] A_0)
	{
		f expr_05 = new f();
		expr_05.b();
		Console.ReadLine();
		expr_05.a();
	}
}
