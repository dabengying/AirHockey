using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AirHockey
{
    class Player
    {
        public virtual void Update(Paddle playerPaddle, Paddle opponentPadde, Puck puck, float deltaTime) { }
    }

    class LocalPlayer : Player 
    {
        public delegate int MouseX();
        public delegate int MouseY();
        public delegate Vector2 ClientToView(int x, int y);


        MouseX mouseX;
        MouseY mouseY;
        ClientToView clientToView;

        public LocalPlayer(MouseX mX, MouseY mY, ClientToView ctv)
        {
            mouseX = mX;
            mouseY = mY;
            clientToView = ctv;
        }

        public override void Update(Paddle playerPaddle, Paddle opponentPadde, Puck puck, float deltaTime)
        {
            playerPaddle.velocity = (clientToView(mouseX(), mouseY()) - playerPaddle.position) / deltaTime;

        }
    }

    class LANGameLocalPlayer : Player
    {
        UdpClient client;
        public delegate int MouseX();
        public delegate int MouseY();
        public delegate Vector2 ClientToView(int x, int y);


        MouseX mouseX;
        MouseY mouseY;
        ClientToView clientToView;

        int port;

        public LANGameLocalPlayer(MouseX mX, MouseY mY, ClientToView ctv, IPAddress opponentIPAddress)
        {
            port = 5555; // TODO: move this somewhere better since it's used in a few spots...

            mouseX = mX;
            mouseY = mY;
            clientToView = ctv;
        
            client = new UdpClient();

            client.Connect(opponentIPAddress, port);
        }

        public override void Update(Paddle playerPaddle, Paddle opponentPadde, Puck puck, float deltaTime)
        {
            playerPaddle.velocity = (clientToView(mouseX(), mouseY()) - playerPaddle.position) / deltaTime;

            //        string toSend = userName + ":\n" + tbSend.Text;
            var data = Encoding.ASCII.GetBytes(playerPaddle.position.X.ToString() + ' ' + playerPaddle.position.Y + ' ' +
                playerPaddle.velocity.X.ToString() + ' ' + playerPaddle.velocity.Y.ToString());


            client.Send(data, data.Length);
        }
    }

    class LANGameOpponentPlayer : Player 
    {
        UdpClient receivingClient;
        Thread receivingThread;
        Paddle paddle;

        int port;

        public LANGameOpponentPlayer()
        {
            port = 5555; //TODO: Move to constants or networkign or something...

            receivingClient = new UdpClient();
            receivingClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));


            receivingThread = new Thread(Receiver);

            receivingThread.IsBackground = true;

            receivingThread.Start();

            paddle = new Paddle();
        }

        public override void Update(Paddle playerPaddle, Paddle opponentPadde, Puck puck, float deltaTime)
        {
            playerPaddle.position = paddle.position;
            playerPaddle.velocity = paddle.velocity;
        }

        void Update(Vector2 position, Vector2 velocity)
        {
            paddle.position = position;
            paddle.velocity = velocity;
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

                position.X = float.Parse(splits[0]);
                position.Y = float.Parse(splits[1]);
                velocity.X = float.Parse(splits[2]);
                velocity.Y = float.Parse(splits[3]);

                Update(position, velocity);
            }

        }


    }
}
