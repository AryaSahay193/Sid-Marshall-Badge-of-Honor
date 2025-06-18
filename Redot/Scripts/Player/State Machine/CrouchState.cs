using Godot;
using System;

public partial class CrouchState : BaseStateClass {
	private float groundSlideSpeed = 3.5f;

	public void EnterState() => playerAnimations.Play("Crouch");
	public void UpdateState(float delta) {
	}

	public void ExitState() {
	}
}
