using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldemClient
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.SetWindowSize(80,25);
			Client client = new Client();
			client.Run();
		}
	}
}
