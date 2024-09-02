using Godot;
using System;
using System.Collections.Generic;

public partial class StateMachine : Node {
    [Export] public NodePath initialState;
	private Dictionary<string, State> sidStates;
	private State currentState;
    public override void _Ready() {
		sidStates = new Dictionary<string, State>();
		foreach(Node node in GetChildren()) {
			if(node is State thisState) {
				sidStates[node.Name] = thisState;
				thisState.finiteStateMachine = this;
				thisState.StartState();
				thisState.ExitState();
			}
		}
		currentState = GetNode<State>(initialState);
		currentState.EnterState();
    }

    public override void _PhysicsProcess(double delta) {
		currentState.PhysicsUpdate((float)delta);
    }

    public override void _UnhandledInput(InputEvent @event) {
        currentState.HandleInput(@event);
    }

	public void TransitionTo(string key) {
		if(!sidStates.ContainsKey(key) || currentState == sidStates[key]) {
			return;
		}
		currentState.ExitState();
		currentState = sidStates[key];
		currentState.EnterState();
	}
}