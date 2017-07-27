using System;

namespace socks5.Model
{
	[Serializable]
	public class Configuration
	{
		public bool shareOverLan;

		public int localPort;

		public Configuration()
		{
			this.localPort = 1081;
			this.shareOverLan = true;
		}
	}
}
