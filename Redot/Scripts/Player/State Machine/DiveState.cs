using Godot;
using System;

public partial class DiveState : BaseStateClass {
	private BaseStateClass baseState;

	public override void _Ready() => baseState = new BaseStateClass();
    
	public void EnterState() => baseState.playerAnimations.Play("Dive_Roll");

	public void PhysicsUpdate(float delta) {
	}
	
	public void ExitState() {
	}
}
