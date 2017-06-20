using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Server
{
    public class ServerClient:IClient
    {
        NetworkStream stream;
        TcpClient client;
        bool clientState = true;
        public string UserId;
        public string userName;
        public ServerClient(NetworkStream Stream, TcpClient Client)
        {
            stream = Stream;
            client = Client;
            UserId = Guid.NewGuid().ToString();
        }
        public void Send(string Message)
        {
            byte[] message = Encoding.ASCII.GetBytes(Message);
            try
            {
                stream.Write(message, 0, message.Count());
            }
            catch
            {

            }
        }
        public void Recieve(Queue<string> messages, List<IClient> userClients, Dictionary<String, string> users)
        {
            string recievedMessageString = null;
            while (clientState == true)
            {
                byte[] recievedMessage = new byte[256];
                try
                {
                    stream.Read(recievedMessage, 0, recievedMessage.Length);
                    recievedMessageString = Encoding.ASCII.GetString(recievedMessage).Replace("\0", string.Empty);
                }
                catch
                {
                    recievedMessageString = "has logged out";
                    users.Remove(userName);
                    clientState = false;
                }
                if(recievedMessageString == "!logout")
                {
                    client.GetStream().Close();
                    client.Close();
                }
                Console.WriteLine(userName +": " + recievedMessageString);
                messages.Enqueue(userName + ": " + recievedMessageString);
            }
        }
        public void GetUserName(bool serverState)
        {
            userName = RecieveUserName();
        }
        private string RecieveUserName()
        {
            byte[] recievedMessage = new byte[256];
            stream.Read(recievedMessage, 0, recievedMessage.Length);
            string recievedMessageString = Encoding.ASCII.GetString(recievedMessage).Replace("\0", string.Empty);
            Console.WriteLine(recievedMessageString);
            return recievedMessageString;
        }
        public void Notify(IClient client,string body)
        {
            Send(body);
        }
    }
}
