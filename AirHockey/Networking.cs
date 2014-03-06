

﻿using GameLib;
using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;

namespace AirHockey
{
    class Networking
    {
        Paddle[] paddles;

        public delegate void ReceiveUpdate(int player, Vector2 position, Vector2 velocity);

        public ReceiveUpdate UpdateReceiver;

        const int port = 54545;

        const string broadcastAddress = "255.255.255.255";

        UdpClient receivingClient;

        UdpClient sendingClient;
        Thread receivingThread;

        public Networking(Paddle[] p)
        {
            paddles = p;
        }


        public void InitializeSender()
        {

            sendingClient = new UdpClient();
            sendingClient.ExclusiveAddressUse = false;
            sendingClient.Connect(broadcastAddress, port);
            sendingClient.EnableBroadcast = true;

        }

        public IPAddress GetLocalIPAddress()
        {
            // Disregard virtual addresses.
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                var addr = ni.GetIPProperties().GatewayAddresses.FirstOrDefault();
                if (addr != null)
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                return ip.Address;
                            }
                        }
                    }
                }
            }

            return null;
            /*
            IPHostEntry host;
            IPAddress localIP = null;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip;
                }
            }
            return localIP;
             */
        }

        bool opponentFound;
        IPAddress opponentIPAddress;
        IPAddress localIPAddress;

        public void StartBroadcast()
        {
            localIPAddress = GetLocalIPAddress();

            Thread sendThread = new Thread(GameSearchSend);
            sendThread.Start();

            Thread listenThread = new Thread(GameSearchListen);
            listenThread.Start();
        }


        void GameSearchSend()
        {
            UdpClient client = new UdpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.Client.Bind(new IPEndPoint(IPAddress.Any, 1500));

            IPEndPoint remoteEp = new IPEndPoint(IPAddress.Broadcast, 1500);
            while (!opponentFound)
            {
                byte[] bytes = Encoding.ASCII.GetBytes("AirHockey-Search " + localIPAddress.ToString());

                client.Send(bytes, bytes.Length, remoteEp);

                Thread.Sleep(1000);
            }

            client.Close();
        }

        private void GameSearchListen()
        {
            UdpClient gameSearchUdpClient = new UdpClient();
            gameSearchUdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            gameSearchUdpClient.Client.Bind(new IPEndPoint(IPAddress.Any, 1500));

            IPEndPoint remoteEp = new IPEndPoint(IPAddress.Any, 15000);

            while (!opponentFound)
            {
                byte[] bytes = gameSearchUdpClient.Receive(ref remoteEp);
                string message = Encoding.ASCII.GetString(bytes);

                String[] splitMessage = message.Split(' ');

                if (splitMessage[0] == "AirHockey-Search")
                {
                    IPAddress ipAddress;


                    if (IPAddress.TryParse(splitMessage[1], out ipAddress))
                    {
                        if (!ipAddress.Equals(localIPAddress))
                        {
                            opponentIPAddress = ipAddress;
                            System.Console.Write(opponentIPAddress.ToString());
                        }
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public void InitializeReceiver()
        {

            receivingClient = new UdpClient();
            receivingClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            receivingClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));
           
            ThreadStart start = new ThreadStart(Receiver);

            receivingThread = new Thread(start);

            receivingThread.IsBackground = true;

            receivingThread.Start();

        }

        public void SendUpdate(int player, Paddle paddle)
        {
            //        string toSend = userName + ":\n" + tbSend.Text;
            var data = Encoding.ASCII.GetBytes(player.ToString() + ' ' + paddle.position.X.ToString() + ' ' + paddle.position.Y + ' ' + 
                paddle.velocity.X.ToString() + ' ' + paddle.velocity.Y.ToString()); 


            //sendingClient.Send(data, data.Length);

        }

        public void Receiver()
        {

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);


            while (true)
            {

                byte[] data = receivingClient.Receive(ref endPoint);

                string message = Encoding.ASCII.GetString(data);

                String[] splits = message.Split(' ');

                Vector2 position = new Vector2();
                Vector2 velocity = new Vector2();

                int player = int.Parse(splits[0]);
                position.X = float.Parse(splits[1]);
                position.Y = float.Parse(splits[2]);
                velocity.X = float.Parse(splits[3]);
                velocity.Y = float.Parse(splits[4]);

                UpdateReceiver(player, position, velocity);
            }

        }


    }
}



