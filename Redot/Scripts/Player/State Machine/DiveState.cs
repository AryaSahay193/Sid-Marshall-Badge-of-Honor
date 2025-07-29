using Godot;
using System;

public partial class DiveState : BaseStateClass { 
	public override void EnterState() => playerReference.playerAnimations.Play("Dive_Roll");

	public override void PhysicsUpdate(float delta) {
	}
	
	public override void ExitState() {
	}
}
