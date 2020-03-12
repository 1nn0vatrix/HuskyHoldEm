using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

public enum Command
{
	CLOSE_SOCKET = 0,

	REGISTER_USER = 1,
	CHANGE_NAME = 2,
	UNREGISTER_USER = 3,

	SHOW_GAMES = 4,
	JOIN_GAME = 5,
	CREATE_GAME = 6,
	UPDATE_GAME = 7,

	VIEW_LEADERBOARD = 8,
	CHAT = 9,

	GAME_RAISE = 10,
	GAME_STAY = 11,
	GAME_FOLD = 12,

	DISPLAY = 13 // todo
};

namespace HuskyHoldEm
{
	public static class NetworkUtils
	{
		public static Packet ReadPacket(Socket socket)
		{
			NetworkStream networkStream = new NetworkStream(socket);
			
			// message length buffer
			byte[] sizeBuffer = new byte[2];
			sizeBuffer = ReadAllBytes(networkStream, 2);

			ushort length = BitConverter.ToUInt16(sizeBuffer, 0);
			Console.WriteLine("[READ] Message Length = " + length);	// todo: debugging, remove before final submission
			
			// message buffer
			byte[] readBuffer = new byte[length];
			readBuffer = ReadAllBytes(networkStream, length);

			// clear for next packet
			networkStream.Flush();
			
			// deserialize back into a packet
			string jsonResponse = Encoding.ASCII.GetString(readBuffer, 0, readBuffer.Length);
			return JsonConvert.DeserializeObject<Packet>(jsonResponse);
		}

		public static void WritePacket(Socket socket, string message)
		{
			byte[] messageBuffer = ASCIIEncoding.ASCII.GetBytes(message);
			Console.WriteLine($"[WRITE] Message Length: {message.Length}");	// todo: debugging, remove before final submission
			
			// send message length
			byte[] sizeBuffer = new byte[2];
			ushort size = (ushort)message.Length;
			sizeBuffer[0] = (byte)size;
			sizeBuffer[1] = (byte)(size >> 8);
			socket.Send(sizeBuffer);
			
			// send message
			socket.Send(messageBuffer);
		}

		public static void SendError(Socket socket, Command command, string errorMessage)
		{
			string jsonError = JsonConvert.SerializeObject(new Packet(command, false, new List<object>() { errorMessage }));
			WritePacket(socket, jsonError);
		}

		// Makes sure we read the exact number of bytes we need to in the message. 
		// See https://stackoverflow.com/questions/7542235/read-specific-number-of-bytes-from-networkstream/7542291#7542291
		private static byte[] ReadAllBytes(NetworkStream networkStream, int bytesToRead)
		{
			byte[] readBuffer = new byte[bytesToRead];
			int read = 0, offset = 0;
			int toRead = bytesToRead;
			while (toRead > 0 && (read = networkStream.Read(readBuffer, offset, toRead)) > 0)
			{
				toRead -= read;
				offset += read;
			}
			if (toRead > 0) throw new EndOfStreamException();
			return readBuffer;
		}
	}

	/**
	 * Data packet sent between client and server.
	 */
	public class Packet
	{
		public Command Command { get; set; }
		public bool Success { get; set; }
		public List<object> DataList { get; set; }

		public Packet(Command command, bool success = true, List<object> dataList = null)
		{
			Command = command;
			Success = success;
			DataList = dataList;
		}

		public string PacketToString()
		{
			return $"[PACKET] Command: {Command}, Success: {Success}, Data: {DataList?[0]}";
		}

		public List<string> DataToString()
		{
			List<string> result = new List<string>();
			foreach (object data in DataList)
			{
				result.Add(JsonConvert.SerializeObject(data));
			}
			return result;
		}
	}
}
