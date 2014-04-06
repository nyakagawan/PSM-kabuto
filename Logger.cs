using System;

namespace kabuto
{
	public static class Logger
	{
		public static void Debug(string format, params object[] param) {
			format = "[DEBUG] " + format;
			Console.WriteLine(format, param);
		}
	}
}

