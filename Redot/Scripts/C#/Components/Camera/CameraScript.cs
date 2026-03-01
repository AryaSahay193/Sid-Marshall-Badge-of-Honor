using Godot;
using System;

public partial class CameraScript : Node2D {
	[ExportGroup("Icons")]
	[Export] private Resource mouseOpen, mouseGrab;

	[ExportGroup("References")]
	[Export] private CameraUI cameraUI;

	public float cameraSpeed = 1.219f, mouseMultiplier = 1.060f;
	private float horizontalMargin = 150.0f, verticalMargin = 100.0f, cameraReturnSpeed = 0.7f; 
	private Vector2 cameraDirection, playerPosition;
	private bool mouseDragged, screenCaptured;
	private PlayerController playerReference;
	private PlayerEffects playerAnimations;
	private GlobalData singletonReference;
	private InputManager inputManager;
	public bool inCameraMode;

	public override void _Ready() {
		singletonReference = GetNode<GlobalData>("/root/GlobalData"); //Autoload script that handles global variables.
		inputManager = GetNode<InputManager>("/root/InputManager"); //Autoload script that handles input.
		playerAnimations = singletonReference.playerEffects;
		playerReference = singletonReference.playerController;
		playerPosition = playerReference.GlobalPosition; //Reference to Sid's position so the camera can return back.
		cameraDirection = inputManager.cameraInput();
	}

	public override void _Process(double delta) {
		float horizontalDirection = inputManager.cameraInput().X;
		float verticalDirection = inputManager.cameraInput().Y;
		if(cameraUI.Visible) inCameraMode = true; 
		else inCameraMode = false;
		if(inCameraMode) {
			playerAnimations.SetProcess(false);
			if(inputManager.cameraInput() != Vector2.Zero) {
				GlobalPosition += new Vector2(cameraSpeed * horizontalDirection, cameraSpeed * verticalDirection);
				if(inputManager.cameraInput().X < 0.0f) {
					cameraUI.buttonLeft.ButtonDown += cameraUI.leftPressed;
					cameraUI.buttonLeft.ButtonUp += cameraUI.leftReleased;
				} else if(inputManager.cameraInput().X > 0.0f) {
					cameraUI.buttonRight.ButtonDown += cameraUI.rightPressed;
					cameraUI.buttonRight.ButtonUp += cameraUI.rightReleased;
				} else if(inputManager.cameraInput().Y < 0.0f) {
					cameraUI.buttonUp.ButtonDown += cameraUI.upPressed;
					cameraUI.buttonUp.ButtonUp += cameraUI.upReleased;
				} else if(inputManager.cameraInput().Y > 0.0f) {
					cameraUI.buttonDown.ButtonDown += cameraUI.downPressed;
					cameraUI.buttonDown.ButtonUp += cameraUI.downReleased;
				}
			} 
		} else {
			returnToPlayer();
			playerAnimations.SetProcess(true);
		}
	}

	public override void _PhysicsProcess(double delta) {
        if(inCameraMode) {
			playerReference.SetPhysicsProcess(false);
			playerReference.horizontalDirection = 0.0f;
		} else playerReference.SetPhysicsProcess(true);
    }

	//Redot built-in method that handles input. This function handles mouse input for dragging the camera and zooming in, along with custom pointers.
    public override void _Input(InputEvent @event) {
        if(inCameraMode) {
			bool buttonHovered = cameraUI.mouseButton.IsHovered() || cameraUI.buttonLeft.IsHovered() || cameraUI.buttonRight.IsHovered() || cameraUI.buttonUp.IsHovered() || cameraUI.buttonDown.IsHovered();
			if(buttonHovered) Input.SetCustomMouseCursor(null);
			else Input.SetCustomMouseCursor(mouseOpen);
			if(@event is InputEventMouseButton mouseButton) {
				if(mouseButton.ButtonIndex == MouseButton.Left) mouseDragged = mouseButton.Pressed;
			} if(@event is InputEventMouseMotion mouseMotion && mouseDragged == true && !buttonHovered) {
				if(mouseDragged) Input.SetCustomMouseCursor(mouseGrab, Input.CursorShape.Drag);
				GlobalPosition += (-mouseMotion.Relative * mouseMultiplier);
			}
		} else Input.SetCustomMouseCursor(null); //Sets cursor back to default.
    }

	private void returnToPlayer() { 
		if(GlobalPosition != playerPosition) {
            GlobalPosition = ToLocal(playerPosition); //Camera will snap back to the player's location.
			GlobalPosition = playerReference.GlobalPosition; //Sets camera position to follow the player again.
        } else return;
	}
}