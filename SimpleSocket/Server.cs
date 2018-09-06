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
        public static ServerSide Instance;
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
        Socket clientTemp;
        private void ListenConn()
        {
            while (true)
            {
                clientTemp = server.Accept();
                Socket client = clientTemp;
                Client c = new Client(client, clientID);
                SaveClientConn.Add(clientID, c);
                Console.WriteLine("Client连入:" + clientID);
                clientID++;
            }
        }

        public Socket Get()
        {
            return server;
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
                try
                {
                    int length = client.Receive(result);
                    if (length == 0)  // 这里 Unity关闭后 会一直接受到Unity客户端的空包 未找到原因
                        continue;
                    DeleEvent.sendMessage -= SendMessage;
                    DeleEvent.sendMessage(clientID + ":" + Encoding.UTF8.GetString(result, 0, length));
                    DeleEvent.sendMessage += SendMessage;
                    //SendMessage("999:我知道了,你可以闭嘴了");
                }
                catch (Exception ex)
                {
                    client.Shutdown(SocketShutdown.Both);
                    DeleEvent.sendMessage -= SendMessage;
                    client.Close();
                    receive.Abort();
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
