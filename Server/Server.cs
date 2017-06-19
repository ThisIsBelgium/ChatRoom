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
    public class Server
    {
        List<ServerClient> userClients = new List<ServerClient>();
        Dictionary<String, string> users = new Dictionary<string, string>();
        public Queue<string> messages = new Queue<string>();
        public static ServerClient client;
        TcpListener server;
        public bool serverState = true;
        public Server()
        {
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), 9999);
            server.Start();
        }
        public void Run()
        {
            string retrievedMessage = null;
            if (users.Count == 0)
            {
                FirstClient();
                Task acceptClient = Task.Run(() => AcceptClient());
            }
            while (serverState == true)
            {
                if (messages.Count > 0)
                {
                    retrievedMessage = messages.Dequeue();
                    RespondToAll(retrievedMessage);
                }
            }
        }
        private void AcceptClient()
        {
            do
            {
                if (server.Pending() == true)
                {
                    TcpClient clientSocket = default(TcpClient);
                    clientSocket = server.AcceptTcpClient();
                    Console.WriteLine("Connected");
                    NetworkStream stream = clientSocket.GetStream();
                    client = new ServerClient(stream, clientSocket);
                    AddUser(serverState, client);
                    Task newUserRecieve = Task.Run(() => client.Recieve(serverState, messages));
                }
            }
            while (serverState == true);
        }
        private void RespondToAll(string body)
        {
            foreach(ServerClient client in userClients)
            {
                client.Send(body);
            }    
        }
        private void FirstClient()
        {
            TcpClient clientSocket = default(TcpClient);
            clientSocket = server.AcceptTcpClient();
            Console.WriteLine("Connected");
            NetworkStream stream = clientSocket.GetStream();
            client = new ServerClient(stream, clientSocket);
            AddUser(serverState, client);
            Task newUserRecieve = Task.Run(() => client.Recieve(serverState, messages));
        }
        private void AddUser(bool serverState, ServerClient client)
        {
            client.GetUserName(serverState);
            users.Add(client.UserName, client.UserId);
            userClients.Add(client);

        }
    }
}
