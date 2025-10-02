using Godot;
using System;

public partial class JigsawPuzzle : Node2D {
	private int jigsawCounter = 0, jigsawValue = 1; //Value of a jigsaw-puzzle.
	private float tweenDistance = 12.0f, tweenDuration = 0.75f;
	private Vector2 startingPosition, finalPosition, currentPosition;
	private AnimatedSprite2D collectibleAnimations;
	private AudioStreamPlayer pickupSound;
	private GlobalData singletonReference;
	private Tween jigsawTween;

	public override void _Ready() {
		singletonReference = GetNode<GlobalData>("/root/GlobalData");
		jigsawCounter = singletonReference.jigsawCounter;
		collectibleAnimations = GetNode<AnimatedSprite2D>("Animation");
		pickupSound = GetNode<AudioStreamPlayer>("PickupSound");
		jigsawTween = CreateTween().SetLoops();
	}

	public override void _PhysicsProcess(double delta) {
		startingPosition = Position;
		finalPosition = new Vector2(startingPosition.X, startingPosition.Y - tweenDistance);
		tweenMovement();
	}

	private void tweenMovement() { 
		jigsawTween.TweenProperty(this, "position", finalPosition, tweenDuration).From(startingPosition); //Moves the animation up
		jigsawTween.TweenProperty(this, "position", startingPosition, tweenDuration).From(finalPosition); //Moves the animation down.
		jigsawTween.Finished += tweenRepeat;
	}

	//Signal method to call the method to repeat the Tween.
	private void tweenRepeat() => jigsawTween.TweenCallback(Callable.From(tweenMovement));
	
	//Signal method for when the puzzle piece is caught.
	private void whenBody2DEntered(CharacterBody2D body) {
		if(body is PlayerController) {
			jigsawCounter += jigsawValue; //Adds one to the counter.
			pickupSound.Play(); //Plays a sound when caught.
			this.QueueFree(); //Deletes the collectible.
		}
	}
}
