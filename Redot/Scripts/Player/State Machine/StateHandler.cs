using Godot;
using System;
using System.Collections.Generic;

public partial class StateHandler : Node {
	[Export] public NodePath startingState;
	private Dictionary<String, BaseStateClass> stateName;
	private BaseStateClass currentState;

	public override void _Ready() {
		stateName = new Dictionary<String, BaseStateClass>();
		foreach(Node node in GetChildren()) {
			if(node is BaseStateClass state) {
				stateName[node.Name] = state;
				state.finiteStateMachine = this;
				state._Ready();
				state.ExitState();
			}
		}

		currentState = GetNode<BaseStateClass>(startingState);
		currentState.EnterState();
	}

	public override void _Process(double delta) => currentState.Update((float)delta);
	public override void _PhysicsProcess(double delta) => currentState.PhysicsUpdate((float)delta);
	public override void _UnhandledInput(InputEvent @event) => currentState.InputHandler(@event);

	public void StateTransition(string dictionaryKey) {
		if(!stateName.ContainsKey(dictionaryKey) || currentState == stateName[dictionaryKey]) return; //If key is not found, or if key is already in the dictionary, do nothing.
		currentState.ExitState();
		currentState = stateName[dictionaryKey];
		currentState.EnterState();
	}
}