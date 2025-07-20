using Godot;
using System;

public partial class DeathState : BaseStateClass {
    private BaseStateClass baseState;

	public override void _Ready() => baseState = new BaseStateClass();

    public void EnterState() {
		playerReference.characterVelocity = Vector2.Zero;
		baseState.playerAnimations.Play("Death");
	}
}
