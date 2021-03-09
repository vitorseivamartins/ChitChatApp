using ChitChatApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatTest
{
    [TestClass]
    public class TestCase
    {
        private static TcpClient _client = new TcpClient("127.0.0.1", 666);

        #region username
        [TestMethod]
        public void NickNameRegister()
        {
            SetUpServer();
            var response = SendNickName();
            Assert.AreEqual("NickName has joined #general", response);
        }

        [TestMethod]
        public void NickNameRegisterAlreadyTaken()
        {
            SetUpServer();
            SendNickName();
            var response = SendNickName(new TcpClient("127.0.0.1", 666));

            Assert.AreEqual("*** Sorry, the nickname NickName is already taken. Please choose a differente one:", response);
        }
        #endregion

        #region messages
        [TestMethod]
        public void SendMessage()
        {
            SetUpServer();
            SendNickName();
            SendData("random message");
            var response = GetData();

            Assert.AreEqual("NickName says: random message", response);
        }

        [TestMethod]
        public void SendReallyLonggggMessage()
        {
            SetUpServer();
            SendNickName();
            string longggText = new string('A', 1024);
            SendData(longggText);
            var response = GetData();

            Assert.AreEqual(string.Format("NickName says: {0}", longggText), response);
        }

        [TestMethod]
        public void SendMessageToUser()
        {
            SetUpServer();
            SendNickName();
            SendData("/p anotherUser random message");
            var response = GetData();

            Assert.AreEqual("NickName says to anotherUser: random message", response);
        }
        #endregion

        #region exit
        [TestMethod]
        public void ExitChat()
        {
            SetUpServer();
            SendNickName();
            SendData("/exit");
            var response = GetData();

            Assert.AreEqual("*** Disconected. Bye!", response);
        }
        #endregion

        #region methods
        static public void SendData(string data, TcpClient client = null)
        {
            if (client == null) client = _client;

            byte[] sendData = Encoding.ASCII.GetBytes(data);
            NetworkStream stream = client.GetStream();
            stream.Write(sendData, 0, sendData.Length);
        }

        static public string GetData(TcpClient client = null)
        {
            if (client == null) client = _client;

            NetworkStream stream = client.GetStream();
            StreamReader sr = new StreamReader(stream);
            return sr.ReadLine();
        }

        public void SetUpServer()
        {
            Task.Run(() => Program.Main(null));
            GetData();
        }

        public string SendNickName(TcpClient client = null)
        {
            if (client == null) client = _client;

            var nickName = "NickName";
            SendData(nickName, client);
            GetData(client);
            return GetData(client);
        }
        #endregion
    }
}
