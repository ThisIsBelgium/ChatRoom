﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        public bool loggedIn = true;
        TcpClient clientSocket;
        NetworkStream stream;
        public Client(string IP, int port)
        {
            clientSocket = new TcpClient();
            clientSocket.Connect(IPAddress.Parse(IP), port);
            stream = clientSocket.GetStream();
        }
        public void Send()
        {
            while (loggedIn == true)
            {
                string messageString = UI.GetInput();
                if (messageString == "!logout")
                {
                    loggedIn = false;
                }
                byte[] message = Encoding.ASCII.GetBytes(messageString);
                stream.Write(message, 0, message.Count());
            }
        }
        public void Recieve()
        {
            while (loggedIn==true)
            {
                byte[] recievedMessage = new byte[256];
                stream.Read(recievedMessage, 0, recievedMessage.Length);
                UI.DisplayMessage(Encoding.ASCII.GetString(recievedMessage).Replace("\0", string.Empty));
            }      
                
        }
        public void Login()
        {
            Console.WriteLine("Enter your username");
            string messageString = UI.GetInput();
            byte[] message = Encoding.ASCII.GetBytes(messageString);
            stream.Write(message, 0, message.Count());

        }
    }
}
