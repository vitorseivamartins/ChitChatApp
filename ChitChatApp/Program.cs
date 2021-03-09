using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatApp
{
    class Program
    {
        private static Dictionary<TcpClient, string> _clients = new Dictionary<TcpClient, string>();

        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 666);
            listener.Start();
            Console.WriteLine("Server started");
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client accepted");

                _clients.Add(client, null);
                WelcomeUser(client);

                Task.Run(() => ReceiveData(client));      
            }
        }

        private static void WelcomeUser(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            StreamWriter sw = new StreamWriter(client.GetStream());
            sw.WriteLine("*** Welcome to our chat server. Please provide a nickname:");
            sw.Flush();
        }

        static async Task ReceiveData(TcpClient client)
        {
            while (true)
            {
                NetworkStream stream = client.GetStream();

                try
                {
                    byte[] buffer = new byte[1024];
                    stream.Read(buffer, 0, buffer.Length);
                    int recv = 0;
                    foreach (byte b in buffer)
                    {
                        if (b != 0)
                        {
                            recv++;
                        }
                    }

                    string request = Encoding.UTF8.GetString(buffer, 0, recv);

                    string response = null;
                    if (_clients[client] == null)
                    {
                        SendMessageToClient(client, ValidateUser(client, request));
                        if (_clients[client] != null)
                            SendMessageToAllClients(string.Format("{0} has joined #general", 
                                _clients[client]));
                    }
                    else if (request.Contains("/exit"))
                    {
                        ExitUser(client);
                        break;
                    }
                    else if (request.Contains("/p"))
                    {
                        var data = request.Split(" ");
                        response = string.Format("{0} says to {1}: {2}",
                            _clients[client],
                            data[1],
                            string.Join(" ", data.ToList().Skip(2))
                            );
                    }
                    else
                        response = string.Format("{0} says: {1}",
                            _clients[client],
                            request);

                    if (_clients[client] != null && !string.IsNullOrEmpty(response))
                        SendMessageToAllClients(response);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong.");
                    //sw.WriteLine(e.ToString());
                }
            }
        }
       
        private static void ExitUser(TcpClient client)
        {
            SendMessageToClient(client, "*** Disconected. Bye!");
            SendMessageToAllClients(string.Format("{0} has disconected from #general",
                    _clients[client]));
            _clients.Remove(client);
        }

        private static string ValidateUser(TcpClient client, string newUser)
        {
            if (_clients.ContainsValue(newUser))
                return string.Format("*** Sorry, the nickname {0} is already taken. Please choose a differente one:", newUser);
            else
            {
                _clients[client] = newUser;
                return string.Format("*** You are registered as {0}. Joining #general", newUser);
            }
        }

        private static void SendMessageToClient(TcpClient client, string message)
        {
            StreamWriter sw = new StreamWriter(client.GetStream());
            sw.WriteLine(message);
            sw.Flush();
        }

        private static void SendMessageToAllClients(string message)
        {
            foreach (var item in _clients)
                SendMessageToClient(item.Key, message);
        }
    }
}