using Godot;
using System;

public partial class State : StateMachine {
	[Export] AnimatedSprite2D sprite; //Brings in all methods into this script of AnimatedSprite2D
	[Export] Vector2 direction;
	public StateMachine finiteStateMachine;
	public virtual void EnterState() {
		sprite.Play("Idle");
	}
	public virtual void ExitState() {
	}
	public virtual void UpdateState(float delta) {
		if(direction.X != 0) {
			//Transition to Move State.
			//currentState = Move;
		} if(Input.IsActionJustPressed("ui_jump")) {
			//Transition to Jump State
			//currentState = Jump;
		} if(direction.X == 0) {
			//Transition to Idle State.
			//currentState = Idle;
		}
	}

	public virtual void StartState() {}
	public virtual void PhysicsUpdate(float delta) {}
	public virtual void HandleInput(InputEvent @event) {}
}