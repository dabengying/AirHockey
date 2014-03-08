

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
        Paddle[] paddles;



        const int port = 54545;

        const string broadcastAddress = "255.255.255.255";


        public Networking(Paddle[] p)
        {
            paddles = p;
        }


        public void InitializeSender()
        {



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
        IPAddress opponentIPAddressFromBroadcast;
        IPAddress opponentIPAddressFromAckListener;

        IPAddress opponentIPAddress;

        IPAddress localIPAddress;

        public void StartBroadcast()
        {
            localIPAddress = GetLocalIPAddress();

            Thread sendThread = new Thread(GameSearchSend);
            sendThread.Start();

            Thread listenThread = new Thread(GameSearchListen);
            listenThread.Start();

            Thread ackListenThread = new Thread(GameSearchAckListen);
            ackListenThread.Start();
        }

        bool searchBroadcastReceived;
        bool ackAckReceived;

        //LOOP SEND BROADCAST
        //RECEIVE BROADCAST, LOOP SEND-BROADCAST RECEIVED
        //RECEIVE BROADCAST-RECEIVED LOOP SEND BROADCAST-RECEIVED-ACK 
        //RECEIVE BROADCAST-RECEIVED-ACK, START GAME

        // how to find an opponent:
        // when a network game is started, continuously broadcast a search message at a regular interval.
        // at the same time listen for the opponents search broadcast
        // also listen for the opponents ACK 
        // when a search broadcast is recived, send an ACK back to the opponent. 
        // then the opponent sends back an ACK-ACK
        // when an ACK-ACK is received then the search broadcast can be stopped and the game can be started.
        void GameSearchSend() 
        {
            UdpClient client = new UdpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.EnableBroadcast = true;
            client.Client.Bind(new IPEndPoint(IPAddress.Any, 1500));

            IPAddress broadcastAddress = IPAddress.Parse("192.168.1.255");

            IPEndPoint remoteEp = new IPEndPoint(broadcastAddress, 1500);
            while (!searchBroadcastReceived)
            {
                byte[] bytes = Encoding.ASCII.GetBytes("AirHockey-Search " + localIPAddress.ToString());

                client.Send(bytes, bytes.Length, remoteEp);

                Thread.Sleep(1000);
            }

            client.Close();
        }

        const int ackPort = 2000;

        private void GameSearchAckListen()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, ackPort);
            listener.Start();
            Socket socket = listener.AcceptSocket();

            byte[] bytes = new byte[100];
            int k = socket.Receive(bytes);

            String ackString = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            if(ackString == "AirHockey-ACK")
            {

            }

            socket.Close();
            listener.Stop();

        }
    
        

        private void GameSearchListen()
        {
            UdpClient client = new UdpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.EnableBroadcast = true;
            client.Client.Bind(new IPEndPoint(IPAddress.Any, 1500));

            IPEndPoint remoteEp = new IPEndPoint(IPAddress.Any, 15000);

            while (!searchBroadcastReceived)
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
                            searchBroadcastReceived = true;
                        }
                    }
                }

                if(splitMessage[0] == "AirHockey-ACK")
                {

                }

                Thread.Sleep(1000);
            }

            // send ACK

            while(!ackAckReceived)
            {
                byte[] sendBytes = Encoding.ASCII.GetBytes("AirHockey-ACK " + localIPAddress.ToString());

                IPEndPoint opponentEP = new IPEndPoint(opponentIPAddress, 1500);
                client.Send(sendBytes, sendBytes.Length, opponentEP);
            }

            //listen for the ACK-ACK

            Thread ackAckListenerThread = new Thread(AckAckListener);
            ackAckListenerThread.Start();
        }

        void AckAckListener()
        {
            UdpClient client = new UdpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.EnableBroadcast = true;
            client.Client.Bind(new IPEndPoint(IPAddress.Any, 1500));

            IPEndPoint remoteEp = new IPEndPoint(IPAddress.Any, 15000);

            while (!ackAckReceived)
            {
                byte[] bytes = client.Receive(ref remoteEp);
                string message = Encoding.ASCII.GetString(bytes);

                String[] splitMessage = message.Split(' ');

                if (splitMessage[0] == "AirHockey-ACK-ACK")
                {
                    IPAddress ipAddress;


                    if (IPAddress.TryParse(splitMessage[1], out ipAddress))
                    {

                        if(ipAddress.Equals(opponentIPAddress))
                        {
                            //START THE GAME!
                        }
                    }
                }

                Thread.Sleep(1000);
            }
        }




    }
}



