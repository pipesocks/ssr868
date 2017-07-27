using System;
using System.Collections.Generic;

internal class g
{
	private string a;

	private long[] b = new long[2049];

	private long[] c = new long[2049];

	public int d;

	public int e;

	public int f;

	public int g;

	public int h = 500;

	private int i;

	private int j;

	private int k;

	public g(string A_0)
	{
		this.a = A_0;
	}

	public void a(h A_0)
	{
		for (int i = 0; i <= 2048; i++)
		{
			this.b[i] += A_0.b[i];
			this.c[i] += A_0.c[i];
			A_0.b[i] = 0L;
			A_0.c[i] = 0L;
		}
		this.d += A_0.d;
		A_0.d = 0;
		this.e += A_0.e;
		A_0.e = 0;
		this.f++;
		if (A_0.h)
		{
			this.g++;
		}
		bool flag = false;
		int num = 0;
		int num2 = 0;
		if (this.d + this.e > this.h && this.f >= 5)
		{
			flag = true;
			this.h += 200;
			int arg_118_0 = (int)((long)this.d - this.b[2048]);
			int num3 = (int)((long)this.e - this.c[2048]);
			if (arg_118_0 >= 100 && num3 >= 100)
			{
				int[] array = new int[200];
				int[] array2 = new int[200];
				e[] array3 = new e[200];
				for (int j = 0; j < 200; j++)
				{
					array3[j] = new e(j, this.c[j]);
				}
				Array.Sort<e>(array3);
				int num4 = 199;
				while (num4 > 170 && (array3[num4].a != array3[num4 - 1].a || array3[num4].a != array3[num4 - 2].a))
				{
					array[array3[num4].b] = 1;
					num4--;
				}
				for (int k = 0; k < 200; k++)
				{
					array3[k] = new e(k, this.b[k]);
				}
				Array.Sort<e>(array3);
				int num5 = 199;
				while (num5 > 170 && (array3[num5].a != array3[num5 - 1].a || array3[num5].a != array3[num5 - 2].a))
				{
					array2[array3[num5].b] = 1;
					num5--;
				}
				List<int> list = new List<int>();
				for (int l = 38; l < 170; l++)
				{
					if (array[l] == 1 && array[l + 8] == 1 && this.b[l] > 0L)
					{
						list.Add(l);
					}
				}
				if (list.Count > 0)
				{
					int num6 = list[0];
					if (list.Count < 6)
					{
						if (this.g < this.f / 1024 || this.g <= 1)
						{
							if (num6 == 38)
							{
								this.b("Remote [" + this.a + "] looks like a Shadowsocks Server");
								num2 = 1;
							}
							else if (num6 == 72)
							{
								this.b("Remote [" + this.a + "] looks like a Shadowsocks Server with AEAD cipher");
								num2 = 2;
							}
							else if (num6 == 44 && this.c[6] > (long)(num3 / 300))
							{
								this.b("Remote [" + this.a + "] looks like a V2ray Server with vmess + stream cipher");
								num2 = 11;
							}
							else if (num6 == 56 && this.c[18] > (long)(num3 / 300))
							{
								this.b("Remote [" + this.a + "] looks like a V2ray Server with vmess + AEAD cipher");
								num2 = 12;
							}
							else if (num6 == 52 && this.c[12] > (long)(num3 / 300))
							{
								this.b("Remote [" + this.a + "] looks like a V2ray Server with vmess MUX + stream cipher");
								num2 = 13;
							}
							else if (num6 == 64 && this.c[24] > (long)(num3 / 300))
							{
								this.b("Remote [" + this.a + "] looks like a V2ray Server with vmess MUX + AEAD cipher");
								num2 = 14;
							}
						}
						else if (this.g > this.f / 2)
						{
							num = 1;
							if (num6 == 43)
							{
								this.b("Remote [" + this.a + "] looks like a Shadowsocks Server with TLS obfs");
							}
							else if (num6 == 77)
							{
								this.b("Remote [" + this.a + "] looks like a Shadowsocks Server with AEAD cipher & TLS obfs");
							}
						}
					}
				}
			}
		}
		if (num2 > 0)
		{
			if (num == this.j && num2 == this.k)
			{
				this.i++;
				this.a(" by " + ((int)(100.0 - Math.Pow(0.4, (double)this.i) * 100.0)).ToString() + "%");
			}
			else
			{
				this.i = 0;
				this.j = num;
				this.k = num2;
				this.a(" by 20%");
			}
		}
		if (flag)
		{
			this.b = new long[2049];
			this.c = new long[2049];
			this.d = 0;
			this.e = 0;
			this.f = 0;
			this.g = 0;
			this.h = 500;
		}
	}

	private void b(string A_0)
	{
		Console.Write(string.Format("[{0}] {1}", DateTime.Now, A_0));
	}

	private void a(string A_0)
	{
		Console.WriteLine(A_0);
	}
}
