﻿using System;
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
        List<IClient> userClients = new List<IClient>();
        Dictionary<String, string> users = new Dictionary<string, string>();
        public Queue<string> messages = new Queue<string>();
        public static ServerClient client;
        TcpListener server;
        public bool serverState = true;
        public Server()
        {
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), 9999);
            server.Start();
            Task serverShutdown = Task.Run(() => EndServer());
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
                try
                {
                    if (server.Pending() == true)
                    {
                        TcpClient clientSocket = default(TcpClient);
                        clientSocket = server.AcceptTcpClient();
                        Console.WriteLine("Connected");
                        NetworkStream stream = clientSocket.GetStream();
                        client = new ServerClient(stream, clientSocket);
                        AddUser(serverState, client);
                        string body = client.userName + " has joined the room";
                        foreach (ServerClient client in userClients)
                        {
                            client.Notify(client, body);
                        }
                        Task newUserRecieve = Task.Run(() => client.Recieve(messages, userClients, users));
                    }
                }
                catch
                {

                }
            }
            while (serverState == true);
        }
        private void RespondToAll(string body)
        {
            foreach (ServerClient client in userClients)
            {
                if (body.Contains(client.userName))
                {

                }
                else if (body.Contains("!logout"))
                {

                }
                else
                {
                    client.Send(body);
                }
            }
        }
        private void FirstClient()
        {
            TcpClient clientSocket = default(TcpClient);
            try
            {
                clientSocket = server.AcceptTcpClient();
                Console.WriteLine("Connected");
                NetworkStream stream = clientSocket.GetStream();
                client = new ServerClient(stream, clientSocket);
                AddUser(serverState, client);
                Task newUserRecieve = Task.Run(() => client.Recieve(messages, userClients, users));
            }
            catch
            {

            }

        }
        private void AddUser(bool serverState, ServerClient client)
        {
            client.GetUserName(serverState);
            users.Add(client.userName, client.UserId);
            userClients.Add(client);

        }
        private void EndServer()
        {
            while (serverState == true)
            {
                string logoutMessage = Console.ReadLine();
                if (logoutMessage == "!end")
                {
                    serverState = false;
                    server.Stop();
                }

            }

        }
        private void LogServer()
        {

        }
    }
}
