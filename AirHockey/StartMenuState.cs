using System;
using AirHockey.Graphics;
using IrrKlang;

namespace AirHockey
{
    class StartMenuState : State
    {
        public StartMenuState(StateManager stateManager, ResourceManager resourceManager) : base(stateManager)
        {
            startGameButton = new Button(new TextureEffect("start.png"), 
                new OpenTK.Box2(-1, 1, 1, -1));
            quitButton = new Button(new TextureEffect("quit.png"),
                new OpenTK.Box2(-1, -2, 1, -4));

            startGameButton.Clicked += StartGame;
            quitButton.Clicked += QuitGame;
        }

        void StartGame(Object sender, EventArgs args)
        {
            stateManager.Change(StateId.Playing);
            stateManager.Push(StateId.NewGame);
        }

        void QuitGame(Object sender, EventArgs args)
        {
            Environment.Exit(0);
        }

        public override void OnUpdate(Input input, ISoundEngine soundEngine, float deltaTime)
        {
            startGameButton.Update(input, soundEngine, deltaTime);
            quitButton.Update(input, soundEngine, deltaTime);
        }

        public override void OnCreateScene(Scene scene)
        {
            scene.AddVisual(startGameButton.GetGraphicsObject());
            scene.AddVisual(quitButton.GetGraphicsObject());
        }

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

        Button startGameButton;
        Button quitButton;
    }
}