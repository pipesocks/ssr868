using socks5.Controller;
using socks5.Model;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal class c
{
	private Configuration a;

	private byte[] b;

	private int c;

	private Socket d;

	private Socket e;

	private string f;

	protected const int g = 16384;

	protected byte[] h = new byte[32768];

	public byte i;

	protected byte[] j;

	public c(Configuration A_0, byte[] A_1, int A_2, Socket A_3)
	{
		int arg_27_0 = ((IPEndPoint)A_3.LocalEndPoint).Port;
		this.a = A_0;
		this.b = A_1;
		this.c = A_2;
		this.d = A_3;
		A_3.NoDelay = true;
		this.g();
	}

	private void a(ref Socket A_0)
	{
		lock (this)
		{
			if (A_0 != null)
			{
				Socket socket = A_0;
				A_0 = null;
				try
				{
					socket.Shutdown(SocketShutdown.Both);
				}
				catch
				{
				}
				try
				{
					socket.Close();
				}
				catch
				{
				}
			}
		}
	}

	private void h()
	{
		this.a(ref this.d);
		this.a(ref this.e);
		this.a = null;
	}

	private bool a(Socket A_0, string A_1, string A_2)
	{
		return true;
	}

	private void g()
	{
		try
		{
			if (this.c > 1)
			{
				if (this.b[0] == 5 && this.c >= 3)
				{
					this.f();
				}
				else
				{
					this.h();
				}
			}
			else
			{
				this.h();
			}
		}
		catch (Exception arg_35_0)
		{
			Logging.LogUsefulException(arg_35_0);
			this.h();
		}
	}

	private void f()
	{
		byte[] expr_06 = new byte[2];
		expr_06[0] = 5;
		byte[] array = expr_06;
		if (this.b[0] != 5)
		{
			array = new byte[]
			{
				0,
				91
			};
			Console.WriteLine("socks 4/5 protocol error");
			this.d.Send(array);
			this.h();
			return;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		for (int i = 0; i < (int)this.b[1]; i++)
		{
			if (this.b[2 + i] == 0)
			{
				flag = true;
				flag3 = true;
			}
			else if (this.b[2 + i] == 2)
			{
				flag2 = true;
				flag3 = true;
			}
		}
		if (!flag3)
		{
			Console.WriteLine("Socks5 no acceptable auth method");
			this.h();
			return;
		}
		if (flag2 || !flag)
		{
			array[1] = 2;
			this.d.Send(array);
			this.e();
			return;
		}
		if (flag)
		{
			this.d.Send(array);
			this.d();
			return;
		}
		Console.WriteLine("Socks5 Auth failed");
		this.h();
	}

	private void e()
	{
		try
		{
			if (this.d.Receive(this.h, 1024, SocketFlags.None) >= 3)
			{
				byte b = this.h[1];
				byte count = this.h[(int)(b + 2)];
				byte[] expr_34 = new byte[2];
				expr_34[0] = 1;
				byte[] buffer = expr_34;
				string @string = Encoding.UTF8.GetString(this.h, 2, (int)b);
				string string2 = Encoding.UTF8.GetString(this.h, (int)(b + 3), (int)count);
				if (this.a(this.d, @string, string2))
				{
					this.d.Send(buffer);
					this.d();
				}
			}
			else
			{
				Console.WriteLine("failed to recv data in HandshakeAuthReceiveCallback");
				this.h();
			}
		}
		catch (Exception arg_9A_0)
		{
			Logging.LogUsefulException(arg_9A_0);
			this.h();
		}
	}

	private void d()
	{
		try
		{
			int num = this.d.Receive(this.h, 5, SocketFlags.None);
			if (num >= 5)
			{
				this.i = this.h[1];
				this.j = new byte[num - 3];
				Array.Copy(this.h, 3, this.j, 0, this.j.Length);
				int num2 = 0;
				if (this.j[0] == 1)
				{
					num2 = 3;
				}
				else if (this.j[0] == 4)
				{
					num2 = 15;
				}
				else if (this.j[0] == 3)
				{
					num2 = (int)this.j[1];
				}
				if (num2 == 0)
				{
					throw new Exception("Wrong socks5 addr type");
				}
				this.b(num2 + 2);
			}
			else
			{
				Console.WriteLine("failed to recv data in HandshakeReceive2Callback");
				this.h();
			}
		}
		catch (Exception arg_B2_0)
		{
			Logging.LogUsefulException(arg_B2_0);
			this.h();
		}
	}

	private void b(int A_0)
	{
		try
		{
			int num = this.d.Receive(this.h, A_0, SocketFlags.None);
			if (num > 0)
			{
				Array.Resize<byte>(ref this.j, this.j.Length + num);
				Array.Copy(this.h, 0, this.j, this.j.Length - num, num);
				if (this.i == 3)
				{
					this.a(num);
				}
				else
				{
					this.f = "socks5";
					this.b();
				}
			}
			else
			{
				Console.WriteLine("failed to recv data in HandshakeReceive3Callback");
				this.h();
			}
		}
		catch (Exception arg_81_0)
		{
			Logging.LogUsefulException(arg_81_0);
			this.h();
		}
	}

	private void a(int A_0)
	{
		bool flag = this.d.AddressFamily == AddressFamily.InterNetworkV6;
		int num = 0;
		if (A_0 >= 9)
		{
			flag = (this.j[0] == 4);
			if (!flag)
			{
				num = (int)this.j[5] * 256 + (int)this.j[6];
			}
			else
			{
				num = (int)this.j[17] * 256 + (int)this.j[18];
			}
		}
		if (!flag)
		{
			this.j = new byte[7];
			this.j[0] = 9;
			this.j[5] = (byte)(num / 256);
			this.j[6] = (byte)(num % 256);
		}
		else
		{
			this.j = new byte[19];
			this.j[0] = 12;
			this.j[17] = (byte)(num / 256);
			this.j[18] = (byte)(num % 256);
		}
		int i = 0;
		IPAddress iPAddress = flag ? IPAddress.IPv6Any : IPAddress.Any;
		this.e = new Socket(iPAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
		while (i < 65536)
		{
			try
			{
				this.e.Bind(new IPEndPoint(iPAddress, i));
				break;
			}
			catch (Exception)
			{
			}
			i++;
		}
		i = ((IPEndPoint)this.e.LocalEndPoint).Port;
		if (!flag)
		{
			byte[] array = new byte[]
			{
				5,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				(byte)(i / 256),
				(byte)(i % 256)
			};
			Array.Copy(((IPEndPoint)this.d.LocalEndPoint).Address.GetAddressBytes(), 0, array, 4, 4);
			this.d.Send(array);
			this.b();
			return;
		}
		byte[] array2 = new byte[]
		{
			5,
			0,
			0,
			4,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			(byte)(i / 256),
			(byte)(i % 256)
		};
		Array.Copy(((IPEndPoint)this.d.LocalEndPoint).Address.GetAddressBytes(), 0, array2, 4, 16);
		this.d.Send(array2);
		this.b();
	}

	private void c()
	{
		if (this.d.AddressFamily == AddressFamily.InterNetwork)
		{
			byte[] expr_15 = new byte[10];
			expr_15[0] = 5;
			expr_15[3] = 1;
			byte[] buffer = expr_15;
			this.d.Send(buffer);
			return;
		}
		byte[] expr_33 = new byte[22];
		expr_33[0] = 5;
		expr_33[3] = 4;
		byte[] buffer2 = expr_33;
		this.d.Send(buffer2);
	}

	private void b()
	{
		int arg_15_0 = ((IPEndPoint)this.d.LocalEndPoint).Port;
		if (this.e == null)
		{
			new b(this.a).a(this.j, this.j.Length, this.d, this.f);
		}
		this.a();
		this.h();
	}

	private void a()
	{
		this.b = null;
		this.d = null;
		this.e = null;
		this.h = null;
		this.j = null;
	}
}
