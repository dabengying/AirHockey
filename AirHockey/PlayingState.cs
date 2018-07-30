using IrrKlang;
using System.Net;
using AirHockey.Graphics;
using System;
using OpenTK;
using System.Collections.Generic;

namespace AirHockey
{
    class Body
    {
        public Vector2 position;
        public Vector2 velocity;
    }

    class PlayingState : State
    {
        public PlayingState(StateManager stateManager,
             ISoundEngine soundEngine,
             Renderer renderer,
            ResourceManager resourceManager) : base(stateManager)
        {
            physics = new Physics();
            contacts = new List<Physics.Contact>();

            physics.Collision += (sender, contact) =>
            {
                if (contact.collider == Collider.Paddle)
                {
                    soundEngine.Play2D(ResourceManager.MediaPath + "puckHitPaddle.wav");
                    contacts.Add(contact);
                }
                if (contact.collider == Collider.Table)
                {
                    soundEngine.Play2D(ResourceManager.MediaPath + "puckHitWall.wav");
                    contacts.Add(contact);
                }
            };


            tableGraphicsObject = new Visual(
                GeometryFactory.CreateBox(
                    new Box2(-Constants.tableWidth * 0.5f, Constants.tableHeight * 0.5f, 
                Constants.tableWidth * 0.5f, -Constants.tableHeight * 0.5f)),
                new TextureLightEffect("table.png"));
            puckGraphicsObject = new Visual(
                GeometryFactory.CreateCircle(Constants.puckRadius, new Color4(1, 0, 0, 1)),
                 new VertexColorEffect());
            paddleGraphicsObjects = new Visual[2];
            paddleGraphicsObjects[0] = new Visual(
                GeometryFactory.CreateCircle(Constants.paddleRadius, new Color4(1, 1, 1, 1)),
                new VertexColorLightEffect());
            paddleGraphicsObjects[1] = new Visual(
                GeometryFactory.CreateCircle(Constants.paddleRadius, new Color4(1, 1, 1, 1)),
                new VertexColorEffect());

            gameFrame = new GameFrame();
            ai = new AIPlayer();
        }

        void HandleInput(Input input, float deltaTime)
        {
            gameFrame.Paddles[0].velocity = (input.ViewPosition - gameFrame.Paddles[0].position) / deltaTime;
        }


        public override void OnUpdate(Input input, ISoundEngine soundEngine, float deltaTime)
        {
            HandleInput(input, deltaTime);
            ai.Update(gameFrame, deltaTime);
            physics.Step(gameFrame, deltaTime);

            int player = physics.CheckForScore(gameFrame);

            if (player != -1)
            {
                scores[player]++;

                if (scores[player] >= 7)
                {
                    if (player == 0)
                        stateManager.Push(StateId.Winner);
                    else if (player == 1)
                        stateManager.Push(StateId.Loser);

                    scores[0] = 0;
                    scores[1] = 0;
                }
                else
                {
                    gameFrame.DropPuck();
                    stateManager.Push(StateId.BeginPlay);
                }

            }
        }

        public override void OnEnter() { }
        public override void OnExit() { }



        public override void OnCreateScene(Scene scene)
        {


            puckGraphicsObject.WorldMatrix.M13 = gameFrame.Puck.position.X;
            puckGraphicsObject.WorldMatrix.M23 = gameFrame.Puck.position.Y;

            paddleGraphicsObjects[0].WorldMatrix.M13 = gameFrame.Paddles[0].position.X;
            paddleGraphicsObjects[0].WorldMatrix.M23 = gameFrame.Paddles[0].position.Y;

            paddleGraphicsObjects[1].WorldMatrix.M13 = gameFrame.Paddles[1].position.X;
            paddleGraphicsObjects[1].WorldMatrix.M23 = gameFrame.Paddles[1].position.Y;

            scene.AddVisual(tableGraphicsObject);
            scene.AddVisual(puckGraphicsObject);
            scene.AddVisual(paddleGraphicsObjects[0]);
            scene.AddVisual(paddleGraphicsObjects[1]);


            scene.Ambient = new Color4(0.0f, 0.0f, 0.0f, 1.0f);

                float x = 6.28f *  DateTime.Now.Millisecond * 0.001f;

            Light light2 = new Light();
            light2.Position = gameFrame.Paddles[0].position;
            light2.Color = new Color4(1, 1, 1, 1);
            light2.K = 0.4f;
            light2.P = 3.0f;
            scene.AddLight(light2);
            scene.AddShadow(new Shadow(light2.Position, gameFrame.Puck.position, Constants.puckRadius));
        }


        List<Physics.Contact> contacts;

        GameFrame gameFrame;
        Physics physics;
        public int[] scores = new int[2];
        AIPlayer ai;

        Visual tableGraphicsObject;
        Visual puckGraphicsObject;
        Visual[] paddleGraphicsObjects;
            

    }
}