using Godot;
using System;

public partial class cameramovement : Camera2D {
	private Vector2 cameraPosition, originalCameraPosition, cameraVelocity = new Vector2(0.0f, 0.0f);
	private AnimatedSprite2D animationLeft, animationRight, animationUp, animationDown;
	private CharacterBody2D sidMarshall;
	private CanvasLayer cameraPanUI, playerUI;
	private Camera2D camera2D;

	private bool cameraPanMode = false;
	[Export] private float cameraAcceleration; //2.0f
	[Export] private float cameraFriction; //3.5f
	[Export] private float cameraSpeed; //10.0f

    public override void _Ready() {
		//cameraControls = Input.GetVector("camera_left", "camera_right", "camera_up", "camera_down"); //Type of Vector2
        cameraPanUI = GetNode<CanvasLayer>("CameraUI");
		playerUI = GetNode<CanvasLayer>("PlayerUI");
		animationLeft = GetNode<AnimatedSprite2D>("CameraUI/CameraLeft"); //Left arrow sprite.
		animationRight = GetNode<AnimatedSprite2D>("CameraUI/CameraRight"); //Right arrow sprite.
		animationUp = GetNode<AnimatedSprite2D>("CameraUI/CameraUp"); //Up arrow sprite.
		animationDown = GetNode<AnimatedSprite2D>("CameraUI/CameraDown"); //Down arrow sprite.
		
		cameraPanUI.Visible = false; //On startup, the UI does not show.
		originalCameraPosition = sidMarshall.GlobalPosition; //Reference to Sid's position so the camera can return back.
		cameraPosition = camera2D.GlobalPosition; 
    }
    
	public override void _Process(double delta) {
		if(Input.IsActionJustPressed("camera_activate")) {
			cameraPanUI.Visible = true;
			playerUI.Visible = false;
			cameraPanMode = true;
		} else if(cameraPanMode && Input.IsActionJustPressed("camera_activate")) {
			cameraPosition = sidMarshall.GlobalPosition;
			cameraPanUI.Visible = false;
			playerUI.Visible = true;
			cameraPanMode = false;
		}

		if(cameraPanMode) {
			//Camera Move Left
			if (Input.IsActionPressed("camera_left")) {
				cameraPosition.X -= (float)Mathf.MoveToward(cameraVelocity.X, -cameraSpeed, cameraAcceleration);
				animationLeft.Play("Camera Press Left");
			} else if(Input.IsActionJustReleased("camera_left")) {
				cameraPosition.X = (float)Mathf.MoveToward(-cameraSpeed, cameraVelocity.X, cameraFriction);
				animationLeft.Play("Camera Release Left");
			}

			//Camera Move Right
			if (Input.IsActionPressed("camera_right")) {
				cameraPosition.X += (float)Mathf.MoveToward(cameraVelocity.X, cameraSpeed, cameraAcceleration);
				animationRight.Play("Camera Press Right");
			} else if(Input.IsActionJustReleased("camera_right")) {
				cameraPosition.X = (float)Mathf.MoveToward(cameraSpeed, cameraVelocity.X, cameraFriction);
				animationRight.Play("Camera Release Right");
			}

			//Camera Move Up
			if (Input.IsActionPressed("camera_up")) {
				cameraPosition.Y -= (float)Mathf.MoveToward(cameraVelocity.Y, -cameraSpeed, cameraAcceleration);
				animationUp.Play("Camera Press Up");
			} else if(Input.IsActionJustReleased("camera_up")) {
				cameraPosition.X = (float)Mathf.MoveToward(-cameraSpeed, cameraVelocity.Y, cameraFriction);
				animationUp.Play("Camera Release Up");
			}

			//Camera Move Down
			if (Input.IsActionPressed("camera_down")) {
				cameraPosition.Y += (float)Mathf.MoveToward(cameraVelocity.Y, cameraSpeed, cameraAcceleration);
				animationDown.Play("Camera Press Down");
			} else if(Input.IsActionJustReleased("camera_down")) {
				cameraPosition.X = (float)Mathf.MoveToward(cameraSpeed, cameraVelocity.Y, cameraFriction);
				animationDown.Play("Camera Release Down");
			}
		}
	}
}
