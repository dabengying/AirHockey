


using System;
using AirHockey;
using System.Collections;

using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using IrrKlang;
using AirHockey.Graphics;

namespace AirHockey
{
    public class AppWindow : Program 
    {
        StateManager stateManager;
        Renderer renderer;
        Input input;
        ISoundEngine soundEngine;
        ResourceManager resourceManager;

  
        public AppWindow()
            : base(640/2, 1136/2)
        {
            renderer = new Renderer();
            renderer.Viewport(Width, Height);
            renderer.OrthoCentered(Constants.viewWidth, Constants.viewHeight);

            soundEngine = new ISoundEngine();
            resourceManager = new ResourceManager(renderer, soundEngine);

            input = new Input(this, renderer);
            stateManager = new StateManager();

        }

        protected override void OnLoad(EventArgs e)
        {

            AnimationState beginPlayState, winnerState, loserState, newGameState;
            StartMenuState startMenuState;
            GameOverState gameOverState;
            PlayingState playingState;

            Animation readyGoAnimation;
            Animation newGameAnimation;
            Animation winAnimation;
            Animation loseAnimation;

            OpenTK.Box2 box = new OpenTK.Box2(
                new OpenTK.Vector2(-Constants.tableWidth * 0.5f, Constants.tableHeight * 0.5f), 
                new OpenTK.Vector2(Constants.tableWidth * 0.5f, -Constants.tableHeight * 0.5f));
            Geometry boxGeometry = GeometryFactory.CreateBox(box);

            readyGoAnimation = new Animation();
            readyGoAnimation.AddFrame(new Visual(boxGeometry,
                new TextureEffect("ready.png")), 0.3f, box );

            readyGoAnimation.AddFrame(new Visual(boxGeometry,
                new TextureEffect("go.png")), 0.2f, box);

            newGameAnimation = new Animation();
            newGameAnimation.AddFrame(new Visual(boxGeometry,
                new TextureEffect("newGame.png")), 0.1f, box);
            newGameAnimation.AddFrame(new Visual(boxGeometry,
                new TextureEffect("countDown3.png")), 0.1f, box);
            newGameAnimation.AddFrame(new Visual(boxGeometry,
                new TextureEffect("countDown2.png")), 0.1f, box);
            newGameAnimation.AddFrame(new Visual(boxGeometry,
                new TextureEffect("countDown1.png")), 0.1f, box);

            winAnimation = new Animation();
            winAnimation.AddFrame(new Visual(boxGeometry,
                new TextureEffect("winner.png")), 2.0f, box);

            loseAnimation = new Animation();
            loseAnimation.AddFrame(new Visual(boxGeometry,
                new TextureEffect("loser.png")), 2.0f, box);

            beginPlayState = new AnimationState(stateManager, soundEngine, readyGoAnimation, "", StateId.Pop);
            winnerState = new AnimationState(stateManager, soundEngine, winAnimation,"", StateId.GameOver);
            loserState = new AnimationState(stateManager, soundEngine, loseAnimation, "", StateId.GameOver);
            newGameState = new AnimationState(stateManager, soundEngine, newGameAnimation, ResourceManager.MediaPath + "countdown.wav",
                StateId.BeginPlay);
            startMenuState = new StartMenuState(stateManager, resourceManager);
            playingState = new PlayingState(stateManager, soundEngine, renderer, resourceManager);
            gameOverState = new GameOverState(stateManager, resourceManager);

            stateManager.Add(StateId.Menu, startMenuState);
            stateManager.Add(StateId.Playing, playingState);
            stateManager.Add(StateId.Winner, winnerState);
            stateManager.Add(StateId.Loser, loserState);
            stateManager.Add(StateId.BeginPlay, beginPlayState);
            stateManager.Add(StateId.GameOver, gameOverState);
            stateManager.Add(StateId.NewGame, newGameState);



            stateManager.Change(StateId.Menu);

        }

        protected override void OnRenderFrame(float deltaTime)
        {
            renderer.Clear();
            Scene scene = new Scene();
            stateManager.OnCreateScene(scene);
            renderer.DrawScene(scene);
            SwapBuffers();
        }

        protected override void OnUpdateFrame(float deltaTime)
        {
            stateManager.Update(input, soundEngine, deltaTime);
        }

        [STAThread]
        public static void Main()
        {
            AppWindow window = new AppWindow();
            window.Run();
        }
    }
}