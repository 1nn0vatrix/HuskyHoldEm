using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldemServer
{
	class Program
	{
		public static void Main(string[] args)
		{
			Server server = new Server();
			server.Run();
		}
	}
}
