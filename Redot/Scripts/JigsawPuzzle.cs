using Godot;
using System;

public partial class JigsawPuzzle : Node {
	private GameWorldManager gameManager;
	private AnimatedSprite2D collectibleAnimations;
	private AudioStreamPlayer pickupSound;
	private EventManager eventManager;
	private CollisionShape2D sidCollision;
	private Tween jigsawMovement;
	private int jigsawValue = 1; //Value of a jigsaw-puzzle.

	public override void _Ready() {
		collectibleAnimations = GetNode<AnimatedSprite2D>("Animation");
		pickupSound = GetNode<AudioStreamPlayer>("PickupSound");
		//jigsawMovement = CreateTween();
	}

	public override void _PhysicsProcess(double delta) {
		//jigsawMovement.TweenProperty(this, "position");
	}

	//Signal method for when the puzzle piece is caught.
	private void whenBody2DEntered(CharacterBody2D body) {
		if(body is SidMarshall) {
			gameManager.jigsawCollected(jigsawValue); //Adds one to the Jigsaw Counter.
			//jigsawMovement.Stop(); //Stops the Tween.
			pickupSound.Play(); //Plays sound for collectible when picked up.
			this.QueueFree(); //Deletes the object from the world.
		}
	}
}
