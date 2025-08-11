using Godot;
using System;

//Script that lets other scripts access data globally, a singleton that will be autoloaded on Game start.
public sealed partial class GlobalData : Node {
	public float playerHealth, maximumHealth = 100.0f; //Health will carry over through the entire game.
	public int jigsawCounter = 0, documentCounter = 0; //Collectibles will carry over throughout the game.
	public Camera2D playerCameraReference; //Only one player-camera instance.
	public SidMarshall playerReference; //Only one player instance.
	public ProgressBar playerHealthBar; //One player-health isntance.
	
	public override void _Ready() {
		playerCameraReference = GetNode<Camera2D>("/root/GameWorld/SidMarshall/PlayerCamera");
		playerHealthBar = GetNode<ProgressBar>("/root/GameWorld/UIElements/PlayerUI/Health");
		playerReference = GetNode<SidMarshall>("/root/GameWorld/SidMarshall");
		maximumHealth = playerReference.maximumHealth;
		playerHealth = playerReference.currentHealth; 
	}

	public override void _Process(double delta) {}
}
