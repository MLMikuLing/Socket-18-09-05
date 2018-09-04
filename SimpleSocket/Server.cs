using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace SimpleSocket
{
    class Server
    {
        static void Main(string[] args)
        {
            ServerSide.Initial("127.0.0.1", 9002);
            Console.ReadLine();
        }
    }
    public class ServerSide
    {
        private Dictionary<int, Client> SaveClientConn;
        private int clientID;
        private Thread listenClientConn;
        private Socket server;
        private IPAddress ipAddress;
        private static ServerSide Instance;
        private ServerSide(string ip, int port)
        {
            DeleEvent.sendMessage += Write;
            SaveClientConn = new Dictionary<int, Client>();
            clientID = 0;
            ipAddress = IPAddress.Parse(ip);
            server = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            server.Bind(new IPEndPoint(ipAddress, port));
            server.Listen(10);
            listenClientConn = new Thread(ListenConn);
            listenClientConn.IsBackground = true;
            listenClientConn.Start();
        }
        public static void Initial(string ip, int port)
        {
            Instance = new ServerSide(ip, port);
        }

        private void ListenConn()
        {
            while (true)
            {
                Socket client = server.Accept();
                Client c = new Client(client, clientID);
                SaveClientConn.Add(clientID, c);
                clientID++;
            }
        }

        private void Write(string clientMes)
        {
            Console.WriteLine(clientMes);
        }
    }
    class Client
    {
        private Socket client;
        private int clientID;
        private Thread receive;
        private byte[] result = new byte[1024];
        public Client(Socket client, int clientID)
        {
            DeleEvent.AddEvent(SendMessage);
            this.client = client;
            this.clientID = clientID;
            receive = new Thread(ReceiveMessage);
            receive.IsBackground = true;
            receive.Start();
        }
        private void ReceiveMessage()
        {
            while (true)
            {
                try { 
                    int length = client.Receive(result);
                    Console.WriteLine("Client-{0}:{1}", clientID, Encoding.UTF8.GetString(result, 0, length));
                    DeleEvent.sendMessage -= SendMessage;
                    DeleEvent.sendMessage(client + ":"+Encoding.UTF8.GetString(result, 0, length));
                    DeleEvent.sendMessage += SendMessage;
                    SendMessage("Server:我知道了,你可以闭嘴了");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Client-{0}关闭连接:Info-{1}", clientID, ex.Message);
                    break;
                }
            }
        }
        private void SendMessage(string info)
        {
            client.Send(Encoding.UTF8.GetBytes(info));
        }
    }

    class DeleEvent
    {
        public delegate void dele(string info);
        public static dele sendMessage;
               
        public static void AddEvent(dele e)
        {
            sendMessage += e;
        }
    }


}
