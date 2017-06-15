using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        Dictionary<String, string> users = new Dictionary<string, string>();
        public static ServerClient client;
        TcpListener server;
        bool awaitingConnection;
        public Server()
        {
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), 9999);
            server.Start();
        }
        public void Run()
        {
            do
            {
                awaitingConnection = server.Pending();
                if (awaitingConnection == true)
                {
                    AcceptClient();
                }
                else if (users.Count == 0)
                {
                    AcceptClient();
                }
                string message = client.Recieve();
                Respond(message);
            }
            while (users.Count>0);
        }
        private void AcceptClient()
        {
            TcpClient clientSocket = default(TcpClient);
            clientSocket = server.AcceptTcpClient();
            Console.WriteLine("Connected");
            NetworkStream stream = clientSocket.GetStream();
            client = new ServerClient(stream, clientSocket);
            client.UserName = client.Recieve();
            users.Add(client.UserName, client.UserId);
        }
        private void Respond(string body)
        {
             client.Send(body);
        }
    }
}
