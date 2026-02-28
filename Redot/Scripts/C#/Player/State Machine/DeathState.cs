using Godot;
using System;

public partial class DeathState : BaseStateClass {
    public override void EnterState() {
		playerReference.characterVelocity = Vector2.Zero;
		playerReference.playerAnimations.Play("Death");
	}
}
