using System;
using System.Collections.Generic;

internal class h
{
	public const int a = 2048;

	public long[] b = new long[2049];

	public long[] c = new long[2049];

	public int d;

	public int e;

	private int f;

	private List<byte[]> g = new List<byte[]>();

	public bool h;

	public bool f(byte[] A_0, int A_1)
	{
		this.d++;
		if (A_1 > 2048)
		{
			A_1 = 2048;
		}
		this.b[A_1] += 1L;
		return (this.d + this.e) % 100 == 0;
	}

	public bool a(byte[] A_0, int A_1)
	{
		this.e++;
		this.f++;
		this.c[A_1] += 1L;
		if (this.f == 1 && A_1 > 3 && A_0[0] == 22 && A_0[1] == 3 && A_0[2] == 1)
		{
			this.h = true;
		}
		return (this.d + this.e) % 100 == 0;
	}
}
