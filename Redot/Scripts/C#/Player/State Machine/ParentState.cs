using System;
using System.Collections.Generic;
using Godot;

//Interface class, meaning it's a blueprint for all the children. Only empty methods will be here, that will be "borrowed" by children classes.
//This class will not be used directly, it only serves as a base structure for each state for the player. 
public partial class ParentState : Node {
	[ExportGroup("Collisions")]
	[Export] public CollisionShape2D playerCollision;
	[Export] public Shape2D normalCollision;
	
	[ExportGroup("References")]
	[Export] public CharacterBody2D playerReference;
	[Export] public AnimatedSprite2D playerAnimations;
	
	public event Action stateChange;
	public PlayerController playerController;
	public StateHandler finiteStateMachine;
	public GlobalData singletonReference;	
	public InputManager inputManager;
	public Vector2 characterVelocity;

	public override void _Ready() {
		singletonReference = GetNode<GlobalData>("/root/GlobalData");
		inputManager = GetNode<InputManager>("/root/InputManager");
		playerController = GetOwner<PlayerController>();
		characterVelocity = playerController.characterVelocity;
	}

	//Base method that executes code when entering the state.
	public virtual void EnterState() {}
	
	//Base method that executes code when exiting the current state.
	public virtual void ExitState() {}

	//Base method that handles logic of the current state.
	public virtual void UpdateState(float delta) {}
	
	//Base method that usually handles physics and time-based calculations rather than frame-based.
	public virtual void PhysicsUpdate(float delta) {}

	//Base method that handles input.
	public virtual void HandleInput(InputEvent @event) {}

	public void flipCharacter() { //Flips the player (Raycasts, animations will be flipped in Animation script).
        if(inputManager.horizontalButton() != 0.0f) {
            if(inputManager.horizontalButton() > 0.0f) playerAnimations.FlipH = false;
            else playerAnimations.FlipH = true;
            /*if(playerVelocity.X != 0.0f && playerDirection != -playerDirection) {
                dustParticles.Direction = (playerVelocity * -playerDirection);
                playerAnimations.Play("Skid");
            }*/
        }
    }

	public void playAnimationOnce(String animationName) {
		bool animationPlaying = playerAnimations.IsPlaying();
		if(animationPlaying) {
			playerAnimations.Play(animationName);
			animationPlaying = !animationPlaying;
		}
	}

    public void pauseInputOnAnimation() {
        if(playerAnimations.IsPlaying()) {
            SetPhysicsProcess(false);
            SetProcessInput(false);
        } else SetProcessInput(true);
	}
}
