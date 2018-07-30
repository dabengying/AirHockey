using System.Collections.Generic;
using AirHockey.Graphics;
using IrrKlang;

namespace AirHockey
{

    abstract class State
    {
        public State(StateManager stateManager)
        {
            Scene = new Scene();
            this.stateManager = stateManager;
        }
        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnCreateScene(Scene scene);
        public abstract void OnUpdate(Input input, ISoundEngine soundEngine, float dt);

        public Scene Scene { get; set; }
        protected StateManager stateManager;

    }

    class NullState : State
    {
        public NullState(StateManager stateManager) : base(stateManager)
        {
            
        }
        public override void OnEnter() { }
        public override void OnExit() { }
        public override void OnCreateScene(Scene scene) { }
        public override void OnUpdate(Input input, ISoundEngine soundEngine, float dt) { }
    }

    enum StateId
    {
        Null,
        Menu,
        Playing,
        Winner, 
        Loser, 
        BeginPlay,
        GameOver, 
        NewGame,         
        Pop
    }

    class StateManager
    {
        public StateManager()
        {
            stateStack = new List<State>();
            stateStack.Add(new NullState(this));
            stateTable = new Dictionary<StateId, State>();
            stateTable.Add(StateId.Null, new NullState(this));
        }

        public void Change(StateId id)
        {
            Pop();
            Push(id);
        }

        public void Push(StateId id)
        {
            State state = stateTable[id];
            stateStack.Add(state);
            state.OnEnter();
        }

        public void Pop()
        {
            stateStack[stateStack.Count - 1].OnExit();
            stateStack.RemoveAt(stateStack.Count - 1);
        }
        public void Add(StateId id, State state)
        {
            stateTable.Add(id, state);
        }
        public void Update(Input input, ISoundEngine soundEngine, float dt)
        {
            stateStack[stateStack.Count - 1].OnUpdate(input, soundEngine, dt);
        }

        public void OnCreateScene(Scene scene)
        {
            for (int i = 0; i < stateStack.Count; i++)
            {
                stateStack[i].OnCreateScene(scene);
            }
        }

        List<State> stateStack;
        Dictionary<StateId, State> stateTable;

    }
}