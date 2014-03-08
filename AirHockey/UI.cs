using System;
using System.Collections.Generic;
using AirHockey;
using OpenTK.Input;

namespace AirHockey
{

    class Animation
    {

        public Animation()
        {
            frames = new List<Frame>();
            isPlaying = false;
            currentFrame = 0;
            frameTime = 0.0f;
        }
        public void Play()
        {
            currentFrame = 0;
            frameTime = 0.0f;
            isPlaying = true;
        }

        public bool IsPlaying
        {
            get { return isPlaying; }
        }

        void Stop()
        {
            isPlaying = false;
            currentFrame = 0;
            frameTime = 0.0f;
        }

        public void Draw(Renderer renderer)
        {
            if (currentFrame >= frames.Count)
                return;
            renderer.DrawTexturedQuad(frames[currentFrame].texture, frames[currentFrame].aabb);
        }

        public void Update(float deltaTime)
        {
            frameTime += deltaTime;
            while (frameTime >= frames[currentFrame].time && currentFrame < frames.Count)
            {

                frameTime = frameTime - frames[currentFrame].time;
                currentFrame++;
                if (currentFrame >= frames.Count)
                {
                    isPlaying = false;
                    break;
                }
            }

        }

        public void AddFrame(int texture, float time, OpenTK.Box2 aabb)
        {
            Frame frame = new Frame();
            frame.texture = texture;
            frame.time = time;
            frame.aabb = aabb;
            frames.Add(frame);

        }


        bool isPlaying;
        float frameTime;
        int currentFrame;

        struct Frame
        {
            public int texture;
            public float time;
            public OpenTK.Box2 aabb;
        }

        List<Frame> frames;
    }

    class Button
    {

        int texture;
        OpenTK.Box2 box;

        public Button(int tex, OpenTK.Box2 b)
        {
            texture = tex;
            box = b;
            press = false;
        }

        public void Update(float mouseX, float mouseY, float deltaTime)
        {
            if (OpenTK.Input.Mouse.GetState()[MouseButton.Left])
                if (mouseX > box.Left && mouseX < box.Right && mouseY < box.Top && mouseY > box.Bottom)
                {
                    press = true;

                }
            if (OpenTK.Input.Mouse.GetState()[MouseButton.Left] == false && press == true)
                if (mouseX > box.Left && mouseX < box.Right && mouseY < box.Top && mouseY > box.Bottom)
                {
                    Clicked(this, null);
                    press = false;
                }
            if (mouseX <= box.Left || mouseX >= box.Right || mouseY >= box.Top || mouseY <= box.Bottom)
            {
                press = false;
            }
        }

        public void Draw(Renderer renderer)
        {
            renderer.DrawTexturedQuad(texture, box);
            renderer.DrawBox(box, new Color4(1, 1, 1, 0), new Color4(1, 0, 0, 1));
        }
        public EventHandler Clicked;


        bool press;
    }

    class StartMenu
    {
        public StartMenu(Renderer renderer, AppWindow app)
        {

            appWindow = app;
            startGameButton = new Button(renderer.CreateTextureFromFile("start.png"), new OpenTK.Box2(-1, 1, 1, -1));
            quitButton = new Button(renderer.CreateTextureFromFile("quit.png"), new OpenTK.Box2(-1, -2, 1, -4));

            startGameButton.Clicked += StartGame;
            quitButton.Clicked += QuitGame;
        }

        void StartGame(Object sender, EventArgs args)
        {
            appWindow.SetGameState(GameState.NewGame);
        }

        void QuitGame(Object sender, EventArgs args)
        {
            Environment.Exit(0);
        }

        public void Update(int mouseX, int mouseY, float deltaTime)
        {
            Vector2 vmouse = appWindow.ClientToView(mouseX, mouseY);
            startGameButton.Update(vmouse.X, vmouse.Y, deltaTime);
            quitButton.Update(vmouse.X, vmouse.Y, deltaTime);
        }

        public void Draw(Renderer renderer)
        {
            startGameButton.Draw(renderer);
            quitButton.Draw(renderer);
        }

        AppWindow appWindow;
        Button startGameButton;
        Button quitButton;
    }

    class GameOverMenu
    {
        public GameOverMenu(Renderer renderer, AppWindow app)
        {

            appWindow = app;
            playAgainButton = new Button(renderer.CreateTextureFromFile("start.png"), new OpenTK.Box2(-1, 2, 1, 1));
            mainMenuButton = new Button(renderer.CreateTextureFromFile("mainMenu.png"), new OpenTK.Box2(-1, 1, 1, -1));
            quitButton = new Button(renderer.CreateTextureFromFile("quit.png"), new OpenTK.Box2(-1, -2, 1, -4));

            playAgainButton.Clicked += StartGame;
            quitButton.Clicked += QuitGame;
            mainMenuButton.Clicked += MainMenu;
        }

        void StartGame(Object sender, EventArgs args)
        {
            appWindow.SetGameState(GameState.NewGame);
        }

        void QuitGame(Object sender, EventArgs args)
        {
            Environment.Exit(0);
        }

        void MainMenu(Object sender, EventArgs args)
        {
            appWindow.SetGameState(GameState.Menu);
        }

        public void Update(int mouseX, int mouseY, float deltaTime)
        {
            Vector2 vmouse = appWindow.ClientToView(mouseX, mouseY);
            playAgainButton.Update(vmouse.X, vmouse.Y, deltaTime);
            quitButton.Update(vmouse.X, vmouse.Y, deltaTime);
            mainMenuButton.Update(vmouse.X, vmouse.Y, deltaTime);
        }

        public void Draw(Renderer renderer)
        {
            playAgainButton.Draw(renderer);
            quitButton.Draw(renderer);
            mainMenuButton.Draw(renderer);
        }

        AppWindow appWindow;
        Button playAgainButton;
        Button mainMenuButton;
        Button quitButton;
    }

}
