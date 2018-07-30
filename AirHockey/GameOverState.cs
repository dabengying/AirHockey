using System;
using AirHockey.Graphics;
using IrrKlang;

namespace AirHockey
{
    class GameOverState : State
    {
        public GameOverState(StateManager stateManager, ResourceManager resourceManager) : base(stateManager)
        {
            playAgainButton = new Button(new TextureEffect("start.png"), new OpenTK.Box2(-1, 2, 1, 1));
            mainMenuButton = new Button(new TextureEffect("mainMenu.png"), new OpenTK.Box2(-1, 1, 1, -1));
            quitButton = new Button(new TextureEffect("quit.png"), new OpenTK.Box2(-1, -2, 1, -4));

            playAgainButton.Clicked += StartGame;
            quitButton.Clicked += QuitGame;
            mainMenuButton.Clicked += MainMenu;
        }

        void StartGame(Object sender, EventArgs args)
        {

            stateManager.Change(StateId.NewGame);
        }

        void QuitGame(Object sender, EventArgs args)
        {
            Environment.Exit(0);
        }

        void MainMenu(Object sender, EventArgs args)
        {
            stateManager.Pop();
            stateManager.Change(StateId.Menu);
        }

        public override void OnUpdate(Input input, ISoundEngine soundEngine, float deltaTime)
        {
            playAgainButton.Update(input, soundEngine, deltaTime);
            quitButton.Update(input, soundEngine, deltaTime);
            mainMenuButton.Update(input, soundEngine, deltaTime);
        }

        public override void OnCreateScene(Scene scene)
        {
            scene.AddVisual(playAgainButton.GetGraphicsObject());
            scene.AddVisual(quitButton.GetGraphicsObject());
            scene.AddVisual(mainMenuButton.GetGraphicsObject());
        }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }

        Button playAgainButton;
        Button mainMenuButton;
        Button quitButton;
    }
}