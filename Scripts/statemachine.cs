using Godot;
using System;
using System.Collections.Generic;

public partial class StateMachine : Node {
    public class FiniteStateMachine {
        protected Dictionary<string, State> states = new Dictionary<string, State>();
        public State CurrentState {get; private set;}
        public string CurrentStateName {get; private set;}
        public string previousStateName {get; set;}

        public void Add(string key, State state) {
            states[key] = state;
            state.fsm = this;
        }

        public void ExecuteStatePhysics(float delta) => CurrentState.PhysicsProcess(delta);
        public void ExecuteProcess(float delta) => CurrentState.Process(delta);

        public void InitialiseState(string newState) {
            CurrentState = states[newState];
            CurrentStateName = newState;
            CurrentState.EnterState();
        }

        public void ChangeState(string newState, State previous = null) {
            CurrentState.ExitState();
            CurrentState = states[newState];
            CurrentStateName = newState;
            CurrentState.EnterState(previous);
        }
    }

    public override void _Ready() {
    }
    public override void _PhysicsProcess(double delta) {
    }
}
