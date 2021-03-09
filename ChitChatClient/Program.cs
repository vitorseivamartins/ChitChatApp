using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatClient
{
    class Program
    {
        private static string _userName = null;
        private static TcpClient _client = new TcpClient("127.0.0.1", 1302);
        private static bool _cnxOpen = true;

        static void Main(string[] args)
        {
            Task.Run(() => ReceiveData());
            while (true)
            {
                SendData(Console.ReadLine());
            }

            

            //Console.WriteLine("*** Welcome to our chat server. Please provide a nickname:");
            //while (_userName == null)
            //    SetNickName();

            //Task.Run(() => ReceiveData());

            //while (_cnxOpen == true)
            //    dialog();

            //_client.Close();      
        }

        static private void dialog()
        {
            //connection:
            try
            {
                var message = Console.ReadLine();

                if(message.Contains("/exit"))
                {
                    _cnxOpen = false;
                    Console.WriteLine("*** Disconected. Bye!");
                    return;
                }

                SendData(message);
            }
            catch (Exception e)
            {
                Console.WriteLine("failed to connect...");
                //goto connection;
            }
        }

        static async Task ReceiveData()
        {
            while(_cnxOpen)
            {
                var response = GetData();
                Console.WriteLine(response);
            }
        }

        static private void SetNickName()
        {
            var nickname = Console.ReadLine();
            SendData("n"+nickname);

            var response = GetData();

            if (response.Substring(0, 1) == "1")
                _userName = nickname;
            Console.WriteLine(response.Substring(1, response.Length - 1));         
        }

        static private void SendData(string data)
        {
            byte[] sendData = Encoding.ASCII.GetBytes(data);

            NetworkStream stream = _client.GetStream();
            stream.Write(sendData, 0, sendData.Length);
        }

        static private string GetData()
        {
            NetworkStream stream = _client.GetStream();

            StreamReader sr = new StreamReader(stream);
            return sr.ReadLine();
            //string response = sr.ReadLine();
            //Console.WriteLine(response);
        }

        static private void GetDataa()
        {
            NetworkStream stream = _client.GetStream();

            StreamReader sr = new StreamReader(stream);
            //return sr.ReadLine();
            string response = sr.ReadLine();
            Console.WriteLine(response);
        }
    }
}