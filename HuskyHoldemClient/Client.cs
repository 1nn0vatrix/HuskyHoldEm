using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Xml.Serialization;
using static HuskyHoldEm.NetworkUtils;
using HuskyHoldEm;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HuskyHoldemClient
{
    /**
	 * Client Endpoint
	 */
    public class Client
    {
        private const string LOCAL_HOST_IP = "127.0.0.1";
        private const int PORT = 8070;
        private const string MENU = "Welcome to Husky Hold'Em!\n"
            + "  _____\n | A .  | _____\n |  /.\\ || A ^  | _____\n"
            + " | (_._)||  / \\ || A _  | _____\n |   |  ||  \\ / ||  ( ) || A_ _ |\n"
            + " | ____V||   .  || (_'_)|| ( v )|\n         | ____V||   |  ||  \\ / |\n"
            + "                 | ____V||   .  |\n                         | ____V|\n"
            + "\nPlease Pick from the following options:\n"
            + "1. Register\n2. Join A Game\n3. Create a Game\n4. Unregister\n5. Exit";

        public static void Main()
        {
            try
            {
                Console.WriteLine("Connecting.....");

                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(LOCAL_HOST_IP, PORT);
                Console.WriteLine("Connected");

                Console.WriteLine(MENU);

                while (true)
                {
                    int selection = GetUserInput();
                    string jsonRequest = JsonConvert.SerializeObject(new Packet((Command)selection, false, new List<object>() { "CLIENT SENDING PACKET TO SERVER TEST" }));
                    WritePacket(tcpClient.Client, jsonRequest);
                    Console.WriteLine("Transmitting.....");
                    // Get response back from server
                    Packet packet = ReadPacket(tcpClient.Client);
                    Console.WriteLine($"Packet Command: {packet.Command}, command success: {packet.Success}, response message: {(string)packet.DataList[0]}");
                    switch ((Command)selection)
                    {
                        case Command.REGISTER_USER:
                            Register(tcpClient);
                            break;
                        case Command.JOIN_GAME:
                            break;
                        case Command.CREATE_GAME:
                            break;
                        case Command.UNREGISTER_USER:
                            Unregister(tcpClient);
                            break;
                        case Command.CLOSE_SOCKET:
                            packet = ReadPacket(tcpClient.Client);
                            Console.WriteLine((string)packet.DataList[0]);
                            break;
                        case Command.VIEW_LEADERBOARD:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.StackTrace}");
            }
        }

        private static int GetUserInput()
        {
            int selection = 0;
            do
            {
                Console.Write("\nEnter an option: ");
                try
                {
                    selection = int.Parse(Console.ReadLine().Trim());
                }
                catch (Exception) { }
            }
            while (selection <= 0 || selection > 5);

            return selection;
        }
        /**
		 * Register - allows user to pick a username and adds them to the server player
		 *            list.
		 */
        public static void Register(TcpClient tcpClient)
        {
            // read from server (is player registered)
            Packet packet = ReadPacket(tcpClient.Client);
            // output message
            Console.WriteLine((string)packet.DataList[0]);
            // if the user already exists leave the function
            if (!packet.Success)
            {
                return;
            }
            // otherwise request the username and write the success message
            else
            {
                // read input from player for username
                string jsonRequest = JsonConvert.SerializeObject(new Packet(Command.CHANGE_NAME, true, new List<object>() { Console.ReadLine() }));
                // send it to the server
                WritePacket(tcpClient.Client, jsonRequest);
            }
            // read response from server
            packet = ReadPacket(tcpClient.Client);
            // output the success message
            Console.WriteLine((string)packet.DataList[0]);
            return;
        }

        /**
		 * Unregister - unregisters a player, removes them from server list
		 */
        public static void Unregister(TcpClient tcpClient)
        {
            // read from server (is player not currently registered)
            Packet packet = ReadPacket(tcpClient.Client);
            // output message
            Console.WriteLine((string)packet.DataList[0]);

        }
    }
}
