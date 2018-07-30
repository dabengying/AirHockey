using AirHockey.Graphics;
using IrrKlang;

namespace AirHockey
{
    class AnimationState : State
    {
        public AnimationState(StateManager stateManager, 
            ISoundEngine soundEngine,
            Animation animation,
            string sound, 
            StateId transitionState) : base(stateManager)
        {
            this.soundEngine = soundEngine;
            this.sound = sound;
            this.animation = animation;
            this.transitionState = transitionState;
        }
        public override void OnEnter()
        {
            animation.Play();
            if(!string.IsNullOrEmpty(sound))
                soundEngine.Play2D(sound);
        }

        public override void OnExit()
        {
            
        }

        public override void OnCreateScene(Scene scene)
        {
            scene.AddVisual(animation.GetGraphicsObject());
        }

        public override void OnUpdate(Input input, ISoundEngine soundEngine, float dt)
        {
            animation.Update(dt);
            if (!animation.IsPlaying)
            {
                if (transitionState == StateId.Pop)
                    stateManager.Pop();
                else
                    stateManager.Change(transitionState);
            }
        }

        Animation animation;
        StateId transitionState;
        string sound;
        ISoundEngine soundEngine;
    }
}