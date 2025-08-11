using Godot;
using System;

//Game Manager is responsible for managing the game world, such as scene-transitions, game state (pause, menu), and game flow.
public partial class GameWorldManager : Node {
	public CanvasLayer cameraPanUI, playerUI, achievementUI;
	private GlobalData singletonReference;
	private EventManager eventHandler;
	private Camera2D playerCamera;

	public override void _Ready() {
		//Initializing Nodes.
		playerCamera = GetNode<Camera2D>("/root/GameWorld/SidMarshall/PlayerCamera");
		achievementUI = GetNode<CanvasLayer>("/root/GameWorld/UIElements/AchievementUI");
		cameraPanUI = GetNode<CanvasLayer>("/root/GameWorld/UIElements/CameraUI");
	}

	public void jigsawCollected(int amount) {
		singletonReference.jigsawCounter += amount; //Adds one to the Jigsaw counter everytime a Jigsaw is collected.
		eventHandler.EmitSignal("jigsawCollected", singletonReference.jigsawCounter); //Sends a signal with the current method, and the counter.
	}

	public void gangInitiated(bool inBattleMode) {
		eventHandler.EmitSignal("gangInitiated", inBattleMode);
		inBattleMode = true;
	}
}
