using System;

internal class e : IComparable
{
	public long a;

	public int b;

	public e(int A_0, long A_1)
	{
		this.a = A_1;
		this.b = A_0;
	}

	public int CompareTo(object obj)
	{
		return this.a.CompareTo(((e)obj).a);
	}
}
