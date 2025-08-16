using Godot;
using System;

public partial class PlayerComponents : PlayerController {
	private AudioStreamPlayer jumpSoundEffect, grassWalkSoundEffect, grassRunSoundEffect;
	private AnimatedSprite2D playerAnimations;
	private GlobalData singletonReference;
	private CpuParticles2D dustParticles;
	
	public override void _Ready() {
		singletonReference = GetNode<GlobalData>("/root/GlobalData");
		playerAnimations = GetNode<AnimatedSprite2D>("Animations");
		dustParticles = GetNode<CpuParticles2D>("DustParticles");
	}

	public override void _Process(double delta) {
		StateConditions((float)delta);
		switch(currentState) {
			case PlayerController.PlayerState.Idle :
				playerAnimations.Play("Idle");
				break;
			case PlayerController.PlayerState.Move :
				if(characterVelocity.X != 0.0f) {
					if(characterVelocity.X > walkingSpeed || characterVelocity.X < -walkingSpeed) playerAnimations.Play("Skid");
					if(Input.IsActionPressed("player_run")) {
						dustParticles.Emitting = true;
						playerAnimations.Play("Run");
					} else playerAnimations.Play("Walk");
				}
				break;
			case PlayerController.PlayerState.Jump :
				playerAnimations.Play("Jump");
				break;
			case PlayerController.PlayerState.Fall :
				playerAnimations.Play("Fall");
				break;
		} whenAnimationFinished();
	}

	//Signal Method for AnimatedSprite2D, when one animation plays the next one will play when it's done.
	private void whenAnimationFinished() {
		String animationName = playerAnimations.Animation; //Shorthand version of the expression.
		if(playerAnimations.IsPlaying()) return; //Lets the animation play (does nothing).
		playerAnimations.Stop(); //This way the animation does not loop.
		if(IsOnFloor()) {
			if(horizontalDirection == 0.0f && characterVelocity.X == 0.0f) playerAnimations.Play("Idle"); //If the player is still or finishes the skid animation, play idle.
			if(animationName == "Skid" && characterVelocity.X != 0.0f) return;
			if(animationName == "Slide") playerAnimations.Play("Slide_Loop");
			else if(animationName == "Slide_Loop") playerAnimations.Play("Slide_Recover");
			else if(animationName == "Slide_Recover") playerAnimations.Play("Idle");
		} else if(IsOnWallOnly()) if(animationName == "Wall_Contact") playerAnimations.Play("Wall_Slide");
		else if(characterVelocity.Y > 0.0f) playerAnimations.Play("Fall");
	}
}