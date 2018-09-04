using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ReceiveEvent.Instance.AddEvent(cw);
            ConnServer.Initial("127.0.0.1", 9002);
            ConnServer.Instance.Conn();

            while (true)
            {
                string str = Console.ReadLine();
                ConnServer.Instance.SendMessage(str);
            }
        }
        static void cw(string n)
        {
            Console.WriteLine(n);
        }
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    public class ConnServer
    {
        public static ConnServer Instance;
        private Socket client;
        private string ip;
        private int port;
        private IPAddress ipaddress;
        private byte[] result = new byte[1024];
        /// <summary>
        /// 初始化参数
        /// </summary>
        private ConnServer(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            ipaddress = IPAddress.Parse(ip);
            client = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        public void Conn()
        {
            if (!client.Connected)
            {
                client.Connect(new IPEndPoint(ipaddress, port));
                Thread startReceive = new Thread(ReceiveMessage);
                startReceive.IsBackground = true;
                startReceive.Start();
            }
        }
        /// <summary>
        /// 接收来自服务器的信息
        /// </summary>
        public void ReceiveMessage()
        {
            while (true)
            {
                int mesLength = client.Receive(result);
                ReceiveEvent.Instance.play(Encoding.UTF8.GetString(result, 0, mesLength));
            }
        }
        /// <summary>
        /// 向服务器发送信息
        /// </summary>
        public void SendMessage(string info)
        {
            client.Send(Encoding.UTF8.GetBytes(info));
        }
        /// <summary>
        /// 初始化参数
        /// </summary>
        public static void Initial(string ip, int port)
        {
            Instance = new ConnServer(ip, port);
        }
    }

    /// <summary>
    /// 添加事件，有信息传入可直接调取方法
    /// </summary>
    public class ReceiveEvent
    {
        private static ReceiveEvent instance;
        public static ReceiveEvent Instance
        {
            get
            {
                if (instance == null)
                    instance = new ReceiveEvent();
                return instance;
            }
        }
        public delegate void Del(string info);
        private Del Receive;
        /// <summary>
        /// 添加一个方法到委托中
        /// </summary>
        public void AddEvent(Del e)
        {
            Receive += e;
        }
        /// <summary>
        /// 执行委托中的事件
        /// </summary>
        public void play(string info)
        {
            Receive(info);
        }
    }

}
