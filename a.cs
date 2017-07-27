using socks5.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

internal class a
{
	private delegate IPHostEntry b(string A_0);

	public enum a
	{
		a = 117,
		b
	}

	private static LRUCache<string, IPAddress> a = new LRUCache<string, IPAddress>(3600);

	private static Process b = Process.GetCurrentProcess();

	public static LRUCache<string, IPAddress> d()
	{
		return global::a.a;
	}

	public static void c()
	{
		GC.Collect(GC.MaxGeneration);
		GC.WaitForPendingFinalizers();
		if (UIntPtr.Size == 4)
		{
			global::a.SetProcessWorkingSetSize(global::a.b.Handle, (UIntPtr)4294967295u, (UIntPtr)4294967295u);
			return;
		}
		if (UIntPtr.Size == 8)
		{
			global::a.SetProcessWorkingSetSize(global::a.b.Handle, (UIntPtr)18446744073709551615uL, (UIntPtr)18446744073709551615uL);
		}
	}

	public static void a(byte[] A_0, int A_1)
	{
		byte[] array = new byte[A_1];
		new RNGCryptoServiceProvider().GetBytes(array);
		array.CopyTo(A_0, 0);
	}

	public static uint b()
	{
		byte[] array = new byte[4];
		new RNGCryptoServiceProvider().GetBytes(array);
		return BitConverter.ToUInt32(array, 0);
	}

	public static void a<a>(IList<a> A_0, Random A_1)
	{
		int i = A_0.Count;
		while (i > 1)
		{
			int index = A_1.Next(i);
			i--;
			a value = A_0[index];
			A_0[index] = A_0[i];
			A_0[i] = value;
		}
	}

	public static bool a(byte[] A_0, int A_1, byte[] A_2, int A_3, int A_4)
	{
		for (int i = 0; i < A_4; i++)
		{
			if (A_0[A_1 + i] != A_2[A_3 + i])
			{
				return false;
			}
		}
		return true;
	}

	public static int a(byte[] A_0, int A_1, byte[] A_2)
	{
		if (A_2.Length != 0 && A_1 >= A_2.Length)
		{
			for (int i = 0; i <= A_1 - A_2.Length; i++)
			{
				if (A_0[i] == A_2[0])
				{
					int num = 1;
					while (num < A_2.Length && A_0[i + num] == A_2[num])
					{
						num++;
					}
					if (num >= A_2.Length)
					{
						return i;
					}
				}
			}
		}
		return -1;
	}

	public static bool a(IPAddress A_0, IPAddress A_1, int A_2)
	{
		byte[] addressBytes = A_0.GetAddressBytes();
		byte[] addressBytes2 = A_1.GetAddressBytes();
		int i = 8;
		int num = 0;
		while (i < A_2)
		{
			if (addressBytes[num] != addressBytes2[num])
			{
				return false;
			}
			i += 8;
			num++;
		}
		return addressBytes[num] >> i - A_2 == addressBytes2[num] >> i - A_2;
	}

	public static bool a(IPAddress A_0, string A_1)
	{
		string[] array = A_1.Split(new char[]
		{
			'/'
		});
		IPAddress iPAddress = IPAddress.Parse(array[0]);
		if (A_0.AddressFamily == iPAddress.AddressFamily)
		{
			try
			{
				bool result = global::a.a(A_0, iPAddress, (int)Convert.ToInt16(array[1]));
				return result;
			}
			catch
			{
				bool result = false;
				return result;
			}
			return false;
		}
		return false;
	}

	public static bool b(IPAddress A_0)
	{
		byte[] addressBytes = A_0.GetAddressBytes();
		if (addressBytes.Length == 4)
		{
			string[] array = new string[]
			{
				"127.0.0.0/8",
				"169.254.0.0/16"
			};
			for (int i = 0; i < array.Length; i++)
			{
				string a_ = array[i];
				if (global::a.a(A_0, a_))
				{
					return true;
				}
			}
			return false;
		}
		if (addressBytes.Length == 16)
		{
			string[] array = new string[]
			{
				"::1/128"
			};
			for (int i = 0; i < array.Length; i++)
			{
				string a_2 = array[i];
				if (global::a.a(A_0, a_2))
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	public static bool b(Socket A_0)
	{
		return global::a.b(((IPEndPoint)A_0.RemoteEndPoint).Address);
	}

	public static bool a(IPAddress A_0)
	{
		byte[] addressBytes = A_0.GetAddressBytes();
		if (addressBytes.Length == 4)
		{
			if (A_0.Equals(new IPAddress(0L)))
			{
				return false;
			}
			string[] array = new string[]
			{
				"0.0.0.0/8",
				"10.0.0.0/8",
				"127.0.0.0/8",
				"169.254.0.0/16",
				"172.16.0.0/12",
				"192.168.0.0/16"
			};
			for (int i = 0; i < array.Length; i++)
			{
				string a_ = array[i];
				if (global::a.a(A_0, a_))
				{
					return true;
				}
			}
			return false;
		}
		else
		{
			if (addressBytes.Length == 16)
			{
				string[] array = new string[]
				{
					"::1/128",
					"fc00::/7",
					"fe80::/10"
				};
				for (int i = 0; i < array.Length; i++)
				{
					string a_2 = array[i];
					if (global::a.a(A_0, a_2))
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}
	}

	public static bool a(Socket A_0)
	{
		return global::a.a(((IPEndPoint)A_0.RemoteEndPoint).Address);
	}

	public static string a(DateTime A_0)
	{
		return A_0.ToString("yyyyMMddHHmmssffff");
	}

	public static string a(string A_0)
	{
		string text = "";
		for (int i = 0; i < A_0.Length; i++)
		{
			if (A_0[i] == '%' && i < A_0.Length - 2)
			{
				string arg_34_0 = A_0.Substring(i + 1, 2).ToLower();
				int num = 0;
				char c = arg_34_0[0];
				char c2 = arg_34_0[1];
				num += (int)((c < 'a') ? (c - '0') : ('\n' + (c - 'a')));
				num *= 16;
				num += (int)((c2 < 'a') ? (c2 - '0') : ('\n' + (c2 - 'a')));
				text += ((char)num).ToString();
				i += 2;
			}
			else if (A_0[i] == '+')
			{
				text += " ";
			}
			else
			{
				text += A_0[i].ToString();
			}
		}
		return text;
	}

	public static void b<a>(ref a[] A_0, int A_1)
	{
		if (A_1 > A_0.Length)
		{
			Array.Resize<a>(ref A_0, A_1);
		}
	}

	public static void a<a>(ref a[] A_0, int A_1)
	{
		if (A_1 > A_0.Length)
		{
			Array.Resize<a>(ref A_0, A_1 * 2);
		}
	}

	public static IPAddress a(string A_0, string A_1, bool A_2 = false)
	{
		IPAddress result = null;
		try
		{
			global::a.b b = new global::a.b(Dns.GetHostEntry);
			IAsyncResult asyncResult = b.BeginInvoke(A_0, null, null);
			if (asyncResult.AsyncWaitHandle.WaitOne(10000, true))
			{
				IPHostEntry iPHostEntry = b.EndInvoke(asyncResult);
				IPAddress[] addressList = iPHostEntry.AddressList;
				int i;
				for (i = 0; i < addressList.Length; i++)
				{
					IPAddress iPAddress = addressList[i];
					if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
					{
						IPAddress result2 = iPAddress;
						return result2;
					}
				}
				addressList = iPHostEntry.AddressList;
				i = 0;
				if (i < addressList.Length)
				{
					IPAddress result2 = addressList[i];
					return result2;
				}
			}
		}
		catch
		{
		}
		return result;
	}

	public static string a()
	{
		return Assembly.GetExecutingAssembly().Location;
	}

	[DllImport("kernel32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool SetProcessWorkingSetSize(IntPtr A_0, UIntPtr A_1, UIntPtr A_2);
}
