

﻿using AirHockey;
using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;


//NONE OF THIS WORKS YET...

namespace AirHockey
{
    class Networking
    {
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
        const int broadcastPort = 1500;
        const int tcpPort = 2000;

        IPAddress localIPAddress;

        public void StartGameSearch()
        {
            localIPAddress = GetLocalIPAddress();

            Thread sendThread = new Thread(GameSearchBroadcast);
            sendThread.Start();

            Thread listenThread = new Thread(GameSearchBroadcastListen);
            listenThread.Start();

            Thread ackListenThread = new Thread(GameSearchAckListen);
            ackListenThread.Start();
        }

        void GameSearchBroadcast() 
        {
            UdpClient client = new UdpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.EnableBroadcast = true;
            client.Client.Bind(new IPEndPoint(IPAddress.Any, broadcastPort));

            IPAddress broadcastAddress = IPAddress.Parse("192.168.1.255");

            IPEndPoint remoteEp = new IPEndPoint(broadcastAddress, broadcastPort);
            while (!opponentFound)
            {
                byte[] bytes = Encoding.ASCII.GetBytes("AirHockey-Search " + localIPAddress.ToString());

                client.Send(bytes, bytes.Length, remoteEp);

                Thread.Sleep(1000);
            }

            client.Close();
        }

        private void GameSearchBroadcastListen()
        {
            UdpClient client = new UdpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.EnableBroadcast = true;
            client.Client.Bind(new IPEndPoint(IPAddress.Any, broadcastPort));

            IPEndPoint remoteEp = new IPEndPoint(IPAddress.Any, broadcastPort);

            while (!opponentFound) // figure out how to not broadcast to myself...
            {
                byte[] bytes = client.Receive(ref remoteEp);
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
                            opponentFound = true;
                        }
                    }
                }

            }

            client.Close();

            
            TcpClient tcpClient = new TcpClient(new IPEndPoint(IPAddress.Any, tcpPort));
            
            String ackMessage = "AirHockey-ACK " + localIPAddress;

            Byte[] data = System.Text.Encoding.ASCII.GetBytes(ackMessage);

            NetworkStream stream = tcpClient.GetStream();

            stream.Write(data, 0, data.Length);

            //START GAME
            System.Console.WriteLine("Start game, opponentIPAddress = {0}", opponentIPAddress);
        }

        private void GameSearchAckListen()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, tcpPort);

            tcpListener.Start();

            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String message = null;

            TcpClient client = tcpListener.AcceptTcpClient();
            
       
            NetworkStream stream = client.GetStream();
            stream.Read(bytes, 0, bytes.Length);

            message = System.Text.Encoding.ASCII.GetString(bytes);

            String[] splitMessage = message.Split(' ');

            if (splitMessage[0] == "AirHockey-Search")
            {
                IPAddress ipAddress;


                if (IPAddress.TryParse(splitMessage[1], out ipAddress))
                {
                    if (!ipAddress.Equals(localIPAddress))
                    {
                        opponentIPAddress = ipAddress;
                        opponentFound = true;
                    }
                }
            }

            client.Close();

            System.Console.WriteLine("Start game, opponentIPAddress = {0}", opponentIPAddress);
            //START GAME
        }
    }
}



