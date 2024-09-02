using Godot;
using System;

public partial class State : StateMachine {
	public FiniteStateMachine fsm;
	public virtual void EnterState(State previous = null) {}
	public virtual void ExitState() {}
	public virtual void Process(float delta) {}
	public virtual void PhysicsProcess(float delta) {}
}
