using System;

namespace socks5.Controller
{
	public abstract class IHandler
	{
		public delegate void InvokeHandler();

		public abstract void Shutdown();
	}
}
