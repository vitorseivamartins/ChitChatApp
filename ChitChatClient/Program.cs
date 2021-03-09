using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatClient
{
    class Program
    {
        private static TcpClient _client = new TcpClient("127.0.0.1", 666);

        static void Main(string[] args)
        {
            Task.Run(() => ReceiveData());
            while (true)
                SendData(Console.ReadLine());  
        } 

        static async Task ReceiveData()
        {
            while(true)
                GetData();
        }

        static private void SendData(string data)
        {
            byte[] sendData = Encoding.ASCII.GetBytes(data);
            NetworkStream stream = _client.GetStream();
            stream.Write(sendData, 0, sendData.Length);
        }

        static private void GetData()
        {
            NetworkStream stream = _client.GetStream();
            StreamReader sr = new StreamReader(stream);
            Console.WriteLine(sr.ReadLine());
        }
    }
}