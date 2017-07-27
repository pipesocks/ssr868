using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;

namespace socks5.Controller
{
	public class Logging
	{
		public static string LogFile;

		public static string LogFilePath;

		public static string LogFileName;

		protected static string date;

		private static FileStream a;

		private static StreamWriterWithTimestamp b;

		private static object c = new object();

		public static bool OpenLogFile()
		{
			bool result;
			try
			{
				Logging.a();
				string text = Path.Combine(Path.GetDirectoryName(global::a.a()), "temp");
				Logging.LogFilePath = text;
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				string str = DateTime.Now.ToString("yyyy-MM");
				Logging.LogFileName = "shadowsocks_" + str + ".log";
				Logging.LogFile = Path.Combine(text, Logging.LogFileName);
				Logging.a = new FileStream(Logging.LogFile, FileMode.Append);
				Logging.b = new StreamWriterWithTimestamp(Logging.a);
				Logging.b.AutoFlush = true;
				Console.SetError(Logging.b);
				Logging.date = str;
				result = true;
			}
			catch (IOException arg_A5_0)
			{
				Console.WriteLine(arg_A5_0.ToString());
				result = false;
			}
			return result;
		}

		private static void a()
		{
			StreamWriterWithTimestamp expr_05 = Logging.b;
			if (expr_05 != null)
			{
				expr_05.Dispose();
			}
			FileStream expr_15 = Logging.a;
			if (expr_15 != null)
			{
				expr_15.Dispose();
			}
			Logging.b = null;
			Logging.a = null;
		}

		public static void Clear()
		{
			Logging.a();
			if (Logging.LogFile != null)
			{
				File.Delete(Logging.LogFile);
			}
			Logging.OpenLogFile();
		}

		public static void Error(object o)
		{
			Logging.Log(LogLevel.Error, o);
		}

		public static void Info(object o)
		{
			Logging.Log(LogLevel.Info, o);
		}

		[Conditional("DEBUG")]
		public static void Debug(object o)
		{
			Logging.Log(LogLevel.Debug, o);
		}

		private static string a(StackFrame[] A_0)
		{
			string text = string.Empty;
			for (int i = 0; i < A_0.Length; i++)
			{
				StackFrame stackFrame = A_0[i];
				text += string.Format("{0}\r\n", stackFrame.GetMethod().ToString());
			}
			return text;
		}

		protected static void UpdateLogFile()
		{
			if (DateTime.Now.ToString("yyyy-MM") != Logging.date)
			{
				object obj = Logging.c;
				lock (obj)
				{
					if (DateTime.Now.ToString("yyyy-MM") != Logging.date)
					{
						Logging.OpenLogFile();
					}
				}
			}
		}

		public static void LogUsefulException(Exception e)
		{
			Logging.UpdateLogFile();
			if (e is SocketException)
			{
				SocketException ex = (SocketException)e;
				if (ex.SocketErrorCode != SocketError.ConnectionAborted && ex.SocketErrorCode != SocketError.ConnectionReset && ex.SocketErrorCode != SocketError.NotConnected && ex.SocketErrorCode != (SocketError)(-2147467259) && ex.SocketErrorCode != SocketError.Shutdown && ex.SocketErrorCode != SocketError.Interrupted)
				{
					Logging.Error(e);
					return;
				}
			}
			else
			{
				Logging.Error(e);
			}
		}

		public static bool LogSocketException(string remarks, string server, Exception e)
		{
			Logging.UpdateLogFile();
			if (e is NullReferenceException)
			{
				return true;
			}
			if (e is ObjectDisposedException)
			{
				return true;
			}
			if (!(e is SocketException))
			{
				return false;
			}
			SocketException ex = (SocketException)e;
			if (ex.SocketErrorCode == (SocketError)(-2147467259))
			{
				return true;
			}
			if (ex.ErrorCode == 11004)
			{
				Logging.Log(LogLevel.Warn, string.Concat(new string[]
				{
					"Proxy server [",
					remarks,
					"(",
					server,
					")] DNS lookup failed"
				}));
				return true;
			}
			if (ex.SocketErrorCode == SocketError.HostNotFound)
			{
				Logging.Log(LogLevel.Warn, string.Concat(new string[]
				{
					"Proxy server [",
					remarks,
					"(",
					server,
					")] Host not found"
				}));
				return true;
			}
			if (ex.SocketErrorCode == SocketError.ConnectionRefused)
			{
				Logging.Log(LogLevel.Warn, string.Concat(new string[]
				{
					"Proxy server [",
					remarks,
					"(",
					server,
					")] connection refused"
				}));
				return true;
			}
			if (ex.SocketErrorCode == SocketError.NetworkUnreachable)
			{
				Logging.Log(LogLevel.Warn, string.Concat(new string[]
				{
					"Proxy server [",
					remarks,
					"(",
					server,
					")] network unreachable"
				}));
				return true;
			}
			if (ex.SocketErrorCode == SocketError.TimedOut)
			{
				return true;
			}
			if (ex.SocketErrorCode == SocketError.Shutdown)
			{
				return true;
			}
			Logging.Log(LogLevel.Info, string.Concat(new string[]
			{
				"Proxy server [",
				remarks,
				"(",
				server,
				")] ",
				Convert.ToString(ex.SocketErrorCode),
				":",
				ex.Message
			}));
			return true;
		}

		public static void Log(LogLevel level, object s)
		{
			Logging.UpdateLogFile();
			string[] array = new string[]
			{
				"Debug",
				"Info",
				"Warn",
				"Error",
				"Assert"
			};
			Console.Error.WriteLine(string.Format("[{0}] {1}", array[(int)level], s));
		}

		[Conditional("DEBUG")]
		public static void LogBin(LogLevel level, string info, byte[] data, int length)
		{
		}
	}
}
