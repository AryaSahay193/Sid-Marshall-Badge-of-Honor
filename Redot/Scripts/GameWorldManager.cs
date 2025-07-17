using Godot;
using System;

public partial class GameWorldManager : Node {
	private CanvasLayer cameraPanUI, playerUI, achievementUI;
	private EventManager eventHandler;
	private Camera2D playerCamera;
	private int jigsawCounter = 0; //By default.

	public override void _Ready() {
		//Initializing Nodes.
		playerCamera = GetNode<Camera2D>("SidMarshall/Camera");
		achievementUI = GetNode<CanvasLayer>("UIElements/AchievementUI");
		cameraPanUI = GetNode<CanvasLayer>("UIElements/CameraUI");
		playerUI = GetNode<CanvasLayer>("UIElements/PlayerUI");
	}

	public void jigsawCollected(int amount) {
		jigsawCounter += amount; //Adds one to the Jigsaw counter everytime a Jigsaw is collected.
		eventHandler.EmitSignal("puzzleCollected", jigsawCounter); //Sends a signal with the current method, and the counter.
	}

	public override void _Process(double delta) {

	}
}
