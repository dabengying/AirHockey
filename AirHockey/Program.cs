using System;
using GameLib;
using System.Collections;

using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using IrrKlang;
using System.Collections.Generic;
using OpenTK.Input;


namespace AirHockey
{
    //puck integrates velocity to get new position, paddle differentiates position to get new velocity 
    class Puck
    {
        //Size 3.25" diam.
        public Vector2 position;
        public Vector2 velocity;
    }

    class Paddle 
    {
        //Size 4-1/16" diam.
        public Vector2 position;
        public Vector2 oldPosition;
        public Vector2 velocity;
        public int score;
    }

    class Table
        //Size 8'x4'

    {   // centered at 0,0
        public float width;
        public float height;
        public float elasticity;
    }




    public enum GameState
    {
        Menu, 
        NewGame,
        Playing,
        BeginPlay,
        Winner,
        Loser,
        GameOver
    }

   
    public class AppWindow : Program 
    {
        //units are feet, seconds

        float viewWidth;
        float viewHeight;

        int[] numberTextures;

        Animation readyGoAnimation;

        Animation winAnimation;
        Animation loseAnimation;

        StartMenu startMenu;
        GameOverMenu gameOverMenu;

        float paddleRadius;
        float puckRadius;

        float drag;

        const String mediaPath = "..\\..\\media\\";

        ISoundEngine soundEngine;

        Renderer renderer;
        Puck puck;
        Paddle[] paddles;
        Table table;
        float maxPuckSpeed;
        float paddleElasticity;
        float goalWidth;

        GameState gameState;

        int tableTexture;

        float centerLineOverlap; // as a fraction of the paddle radius

        Animation newGameAnimation;
        Physics physics;

        AI ai;

        public AppWindow()
            : base(640/2, 1136/2)
        {
            viewWidth = 4.5f;
            viewHeight = 9.0f;
            puck = new Puck();
            puck.position = new Vector2(0.0f, 0.0f);
            puckRadius = 3.25f/12.0f; // 3.25 inches
            puck.velocity = new Vector2(0, 0);

            paddles = new Paddle[2];
            paddles[0] = new Paddle();
            paddles[1] = new Paddle();
            paddles[0].position = new Vector2(0.0f, -0.7f);
            paddles[1].position = new Vector2(0.0f, 0.7f);
            paddleRadius =  (4.0f + 1.0f/16.0f) / 12.0f; // 4 1/16 inches

            ai = new AI();

            renderer = new Renderer();
            renderer.Viewport(Width, Height);
            renderer.OrthoCentered(viewWidth, viewHeight);

            table = new Table();
            table.width = 4.0f; // 8x4 feet
            table.height = 8.0f;
            table.elasticity = 0.9f;

            paddleElasticity = 0.9f;
            
            maxPuckSpeed = 16.0f;
            drag = 0.99f;

            goalWidth = table.width / 2.0f;

            centerLineOverlap = 0.8f;

            startMenu = new StartMenu(renderer, this);
            gameOverMenu = new GameOverMenu(renderer, this);


            physics = new Physics();

            soundEngine = new ISoundEngine();
            
        }


        public void SetGameState(GameState state)
        {
            gameState = state;

            if (gameState == GameState.NewGame)
            {
                newGameAnimation.Play();
                paddles[0].score = 0;
                paddles[1].score = 0;
            }
            if(gameState == GameState.BeginPlay)
                readyGoAnimation.Play();
            if(gameState == GameState.Winner)
                winAnimation.Play();
            if (gameState == GameState.Loser)
                loseAnimation.Play();


        }

        protected override void OnLoad(EventArgs e)
        {
            tableTexture = renderer.CreateTextureFromFile("Table.png");

            OpenTK.Box2 box = new OpenTK.Box2(new OpenTK.Vector2(-table.width * 0.5f, table.height * 0.5f), new OpenTK.Vector2(table.width * 0.5f, -table.height * 0.5f));
            readyGoAnimation = new Animation();
            readyGoAnimation.AddFrame(renderer.CreateTextureFromFile("ready.png"), 0.3f, box );
            readyGoAnimation.AddFrame(renderer.CreateTextureFromFile("go.png"), 0.2f, box);

            newGameAnimation = new Animation();
            newGameAnimation.AddFrame(renderer.CreateTextureFromFile("newGame.png"), 0.3f, box);
            newGameAnimation.AddFrame(renderer.CreateTextureFromFile("countDown3.png"), 1.0f, box);
            newGameAnimation.AddFrame(renderer.CreateTextureFromFile("countDown2.png"), 1.0f, box);
            newGameAnimation.AddFrame(renderer.CreateTextureFromFile("countDown1.png"), 1.0f, box);

            winAnimation = new Animation();
            winAnimation.AddFrame(renderer.CreateTextureFromFile("winner.png"), 2.0f, box);

            loseAnimation = new Animation();
            loseAnimation.AddFrame(renderer.CreateTextureFromFile("loser.png"), 2.0f, box);



            SetGameState(GameState.Menu);

        }

        void DrawPuck()
        {
            renderer.DrawCircle(puck.position, puckRadius, new Color4(1, 1, 0, 1), new Color4(1, 0, 1, 0));
        }
        void DrawPaddle(int player)
        {
            renderer.DrawCircle(paddles[player].position, paddleRadius, new Color4(1, 0, 0, 1), new Color4(1, 1, 1, 0));
        }

        void DrawTable()
        {
            renderer.DrawTexturedQuad(tableTexture, new OpenTK.Box2(new OpenTK.Vector2(-table.width * 0.5f, table.height * 0.5f), new OpenTK.Vector2(table.width * 0.5f, -table.height * 0.5f)));

            Vector2 tl = new Vector2(-table.width * 0.5f, table.height * 0.5f);
            Vector2 tr = new Vector2(table.width * 0.5f, table.height * 0.5f);
            Vector2 bl = new Vector2(-table.width * 0.5f, -table.height * 0.5f);
            Vector2 br = new Vector2(table.width * 0.5f, -table.height * 0.5f);

            renderer.DrawLine(tl, tr, new Color4(1, 0, 0, 1));
            renderer.DrawLine(tr, br, new Color4(1, 0, 0, 1));
            renderer.DrawLine(br, bl, new Color4(1, 0, 0, 1));
            renderer.DrawLine(bl, tl, new Color4(1, 0, 0, 1));

            renderer.DrawLine(new Vector2(-goalWidth * 0.5f, table.height * 0.5f), new Vector2(goalWidth * 0.5f, table.height * 0.5f), new Color4(1, 0, 1, 1));
            renderer.DrawLine(new Vector2(-goalWidth * 0.5f, -table.height * 0.5f), new Vector2(goalWidth * 0.5f, -table.height * 0.5f), new Color4(1, 0, 1, 1));
            renderer.DrawLine(new Vector2(-table.width * 0.5f, 0), new Vector2(table.width * 0.5f, 0), new Color4(1, 0, 0, 1));
        }

        protected override void OnRenderFrame(float deltaTime)
        {
            renderer.Clear();

            if (gameState == GameState.Menu)
                startMenu.Draw(renderer);
            else
            {

                DrawTable();
                DrawPuck();
                DrawPaddle(0);
                DrawPaddle(1);

                if (gameState == GameState.BeginPlay)
                    readyGoAnimation.Draw(renderer);
                if (gameState == GameState.NewGame)
                    newGameAnimation.Draw(renderer);
                if (gameState == GameState.Winner)
                    winAnimation.Draw(renderer);
                if (gameState == GameState.Winner)
                    winAnimation.Draw(renderer);
                if (gameState == GameState.Loser)
                    loseAnimation.Draw(renderer);
                if (gameState == GameState.GameOver)
                    gameOverMenu.Draw(renderer);

                //TODO: Draw score.
            }

            SwapBuffers();
        }


        Vector2 ClampPaddlePosition(int player) //player 0 is on bottom, player 1 on top 
        {
            Vector2 position = paddles[player].position;
            float radius = paddleRadius;

            if (player == 0)
                if (position.Y - radius * centerLineOverlap > 0)
                    position.Y = radius * centerLineOverlap;
            if (player == 1)
                if (position.Y + radius * centerLineOverlap < 0)
                    position.Y = -radius * centerLineOverlap;

            if (position.X + radius > table.width * 0.5f)
                position.X = table.width * 0.5f - radius;
            if (position.X - radius < -table.width * 0.5f)
                position.X = -table.width * 0.5f + radius;

            if (position.Y + radius> table.height * 0.5f)
                position.Y = table.height * 0.5f - radius;
            if (position.Y - radius < -table.height * 0.5f)
                position.Y = -table.height * 0.5f + radius;

            return position;
        }

        public Vector2 ClientToView(int x, int y)
        {
            return new Vector2(viewWidth * (float)x / (float)Width - viewWidth * 0.5f, (float)viewHeight * (Height - y) / (float)Height - viewHeight * 0.5f);
        }

        protected override void OnUpdateFrame(float deltaTime)
        {
            if (gameState == GameState.Menu)
            {
                startMenu.Update(MouseX, MouseY, deltaTime);
            }
            else if (gameState == GameState.GameOver)
            {
                gameOverMenu.Update(MouseX, MouseY, deltaTime);
            }
            else if (gameState == GameState.NewGame)
            {
                newGameAnimation.Update(deltaTime);
                if (!newGameAnimation.IsPlaying)
                {
                    SetGameState(GameState.BeginPlay);
                }

            }
            else if (gameState == GameState.BeginPlay)
            {

                readyGoAnimation.Update(deltaTime);
                if (!readyGoAnimation.IsPlaying)
                    SetGameState(GameState.Playing);
            }
            else if (gameState == GameState.Winner)
            {
                winAnimation.Update(deltaTime);
                if (!winAnimation.IsPlaying)
                {
                    SetGameState(GameState.GameOver);

                }
            }
            else if (gameState == GameState.Loser)
            {
                loseAnimation.Update(deltaTime);
                if (!loseAnimation.IsPlaying)
                {
                    SetGameState(GameState.GameOver);

                }
            }
            else if (gameState == GameState.Playing)
            {
                PlayGame(deltaTime);
            }
        }

        void PlayGame(float deltaTime)
        {
            //careful, changing the order of intergrations can break things 
            paddles[0].oldPosition = paddles[0].position;
            paddles[0].position = ClientToView(MouseX, MouseY);
            paddles[0].position = ClampPaddlePosition(0);
            paddles[0].velocity = (paddles[0].position - paddles[0].oldPosition) / deltaTime;

            ai.Update(deltaTime, table, puck.position, puckRadius, paddleRadius);

            paddles[1].position = ai.position;
            paddles[1].velocity = ai.velocity;

            paddles[1].position = ClampPaddlePosition(1);

            physics.DoPaddleCollision(0, puck, puckRadius, paddles, paddleRadius, paddleElasticity, maxPuckSpeed, ai);
            physics.DoPaddleCollision(1, puck, puckRadius, paddles, paddleRadius, paddleElasticity, maxPuckSpeed, ai);

            puck.velocity *= drag;

            puck.position = puck.position + puck.velocity * deltaTime;

            int player = physics.DoTableCollision(puck, puckRadius, table, goalWidth);

            if (player != -1)
            {
                paddles[player].score++;

                if (paddles[player].score >= 7)
                {
                    if (player == 0)
                        SetGameState(GameState.Winner);
                    else if (player == 1)
                        SetGameState(GameState.Loser);

                }
                else
                {
                    SetGameState(GameState.BeginPlay);
                }
                puck.position = new Vector2(0, 0);
                puck.velocity = new Vector2(0, 0);
            }
        }



        [STAThread]
        public static void Main()
        {
            AppWindow window = new AppWindow();
            window.Run();
        }
    }
}