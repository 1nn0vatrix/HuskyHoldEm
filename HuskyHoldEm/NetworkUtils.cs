using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldEm
{
	public static class NetworkUtils
	{
		public static string ReadPacket(Socket socket)
		{
			NetworkStream networkStream = new NetworkStream(socket);
			byte[] sizeBuffer = new byte[2];
			int bytes = networkStream.Read(sizeBuffer, 0, 2);
			ushort size = BitConverter.ToUInt16(sizeBuffer, 0);
			Console.WriteLine("size = " + size);
			byte[] readBuffer = new byte[size];
			int bytesRead = networkStream.Read(readBuffer, 0, readBuffer.Length);
			for (int i = 0; i < bytesRead; i++)
			{
				Console.Write(Convert.ToChar(readBuffer[i]));
			}
			networkStream.Flush();
			return Encoding.ASCII.GetString(readBuffer, 0, bytesRead);
		}

		// TODO: might be good to have the "string input" be a Packet object
		// The packet object would have commands like "CHAT" "FOLD" "REGISTER" "JOIN" "CREATE" etc
		// The first line/word in the string could be the command to do
		// The rest of the string is the data for that command
		// Like a really simple HTTP
		public static void WritePacket(Socket socket, string input)
		{
			byte[] messageBuffer = ASCIIEncoding.ASCII.GetBytes(input);
			Console.WriteLine(input.Length);
			ushort size = (ushort)input.Length;
			byte[] sizeBuffer = new byte[2];
			sizeBuffer[0] = (byte)size;
			sizeBuffer[1] = (byte)(size >> 8);
			socket.Send(sizeBuffer);
			socket.Send(messageBuffer);
		}
	}
}
