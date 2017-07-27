using System;

namespace socks5.Controller
{
	public class CallbackState
	{
		public byte[] buffer;

		public int size;

		public int protocol_size;

		public object state;
	}
}
