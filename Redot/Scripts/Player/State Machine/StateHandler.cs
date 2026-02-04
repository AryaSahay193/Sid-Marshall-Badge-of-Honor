using Godot;
using System;
using System.Collections.Generic;

public partial class StateHandler : Node {
	public ParentState currentState; //Reference to the State class.
	[Export] public NodePath startingState; //Starting path is Idle, in Redot Inspector.
	private Dictionary<String, ParentState> stateName; //Nodepath, as a string, which is stored in a Dictionary.

	public override void _Ready() {
		currentState = new ParentState();
		stateName = new Dictionary<String, ParentState>();
		foreach(Node node in GetChildren()) {
			if(node is ParentState state) {
				stateName[node.Name] = state;
				state.finiteStateMachine = this;
				state._Ready(); //Initialize states.
				state.ExitState(); //Reset the states.
			}
		}
		currentState = GetNode<ParentState>(startingState);
		currentState.EnterState();
	}

	//Delegates the methods from BaseStateClass to methods commonly used.
	public override void _Process(double delta) => currentState.UpdateState((float)delta); 
	public override void _PhysicsProcess(double delta) => currentState.PhysicsUpdate((float)delta);
	public override void _UnhandledInput(InputEvent @event) => currentState.HandleInput(@event);

	public void StateTransition(string dictionaryKey) {
		if(!stateName.ContainsKey(dictionaryKey) || currentState == stateName[dictionaryKey]) return; //If key is not found, or if key is already in the dictionary, do nothing.
		currentState.ExitState();
		currentState = stateName[dictionaryKey];
		currentState.EnterState();
	}
}
