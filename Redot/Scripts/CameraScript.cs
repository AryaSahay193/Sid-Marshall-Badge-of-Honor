using Godot;
using System;

public partial class CameraScript : Camera2D {
	private Vector2 cameraDirection, cameraPosition, playerPosition, cameraVelocity = new Vector2(0.0f, 0.0f);
	private CharacterBody2D playerReference;
	private Camera2D camera;
	
	

	private bool canMoveCamera;
	
	[Export] private float cameraAcceleration; //2.0f
	[Export] private float cameraFriction; //3.5f
	[Export] private float cameraSpeed; //10.0f

	public override void _Ready() {	
		cameraDirection = Input.GetVector("camera_left", "camera_right", "camera_up", "camera_down");
		playerReference = GetNode<CharacterBody2D>("SidMarshall");
		playerPosition = playerReference.GlobalPosition; //Reference to Sid's position so the camera can return back.
		cameraPosition = camera.Position;
	}

	public override void _Process(double delta) {
		if(cameraDirection.X > 0.0f) cameraPosition.X += cameraSpeed;
		else if(cameraDirection.X < 0.0f) cameraPosition.X -= cameraSpeed;
		else if(cameraDirection.Y < 0.0f) cameraPosition.Y += cameraSpeed;
		else if(cameraDirection.Y > 0.0f) cameraPosition.Y -= cameraSpeed;
		else if(Input.IsActionJustPressed("camera_activate")) {
			cameraPosition.X = Mathf.Lerp(cameraPosition.X, playerPosition.X, cameraSpeed);
			cameraPosition.Y = Mathf.Lerp(cameraPosition.Y, playerPosition.Y, cameraSpeed);
		}
	}
}
