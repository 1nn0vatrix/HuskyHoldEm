﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldEm
{
	public static class DebugUtils
	{
		// Only displays the Console.WriteLine if the build is in Debug mode
		[System.Diagnostics.Conditional("DEBUG")]
		public static void WriteLine(string message = "\n")
		{
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		// Only displays the Console.Write if the build is in Debug mode
		[System.Diagnostics.Conditional("DEBUG")]
		public static void Write(string message)
		{
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.Write(message);
			Console.ResetColor();
		}
	}
}
