using Godot;
using System;

public partial class InputManager : Node {
	public float horizontalButton() {
		float horizontalDirection = Input.GetAxis("player_left", "player_right");
		return horizontalDirection;
	}

	public bool runButton() {
		bool isRunning = Input.IsActionPressed("player_run");
		return isRunning;
	}

	public bool jumpButton() {
		bool isJumping = Input.IsActionJustPressed("player_jump");
		return isJumping;
	}

	public bool crouchButton() {
		bool isCrouching = Input.IsActionPressed("player_down");
		return isCrouching;
	}

	public bool punchButton() {
		bool isPunching = Input.IsActionJustPressed("player_punch");
		return isPunching;
	}

	public bool kickButton() {
		bool isKicking = Input.IsActionJustPressed("player_kick");
		return isKicking;
	}

	public bool grabButton() {
		bool isGrabbing = Input.IsActionJustPressed("player_grab");
		return isGrabbing;
	}

	public bool interactButton() {
		bool isInteracting = Input.IsActionJustPressed("player_interact");
		return isInteracting;
	}
}
