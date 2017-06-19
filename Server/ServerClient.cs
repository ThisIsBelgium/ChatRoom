﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Server
{
    public class ServerClient/*:IClient*/
    {
        NetworkStream stream;
        TcpClient client;
        public string UserId;
        public string UserName;
        public ServerClient(NetworkStream Stream, TcpClient Client)
        {
            stream = Stream;
            client = Client;
            UserId = Guid.NewGuid().ToString();
        }
        public void Send(string Message)
        {
            byte[] message = Encoding.ASCII.GetBytes(Message);
            stream.Write(message, 0, message.Count());
        }
        public void Recieve(bool serverState,Queue<string> messages)
        {
            string recievedMessageString = null;
            while (serverState == true)
            {
                byte[] recievedMessage = new byte[256];
                stream.Read(recievedMessage, 0, recievedMessage.Length);
                recievedMessageString = Encoding.ASCII.GetString(recievedMessage);
                Console.WriteLine(recievedMessageString);
                messages.Enqueue(recievedMessageString);
            }
        }
        public void GetUserName(bool serverState)
        {
            UserName = RecieveUserName();
        }
        private string RecieveUserName()
        {
            byte[] recievedMessage = new byte[256];
            stream.Read(recievedMessage, 0, recievedMessage.Length);
            string recievedMessageString = Encoding.ASCII.GetString(recievedMessage);
            Console.WriteLine(recievedMessageString);
            return recievedMessageString;
        }
        private void NotifyUserLog()
        {

        }

    }
}
