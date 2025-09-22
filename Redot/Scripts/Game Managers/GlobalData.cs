using Godot;
using System;

//Script that lets other scripts access data globally, a singleton that will be autoloaded on Game start.
public sealed partial class GlobalData : Node {
	public float playerHealth, maximumHealth = 100.0f; //Health will carry over through the entire game.
	public int jigsawCounter = 0, documentCounter = 0; //Collectibles will carry over throughout the game.
	public Camera2D playerCameraReference; //Only one player-camera instance.
	public PlayerController playerReference; //Only one player instance.
	public ProgressBar playerHealthBar; //One player-health isntance.
	public InputManager playerInputs; //Script that includes inputs.
	public EnemyState currentEnemyState; 

	public enum EnemyState { //Character States
        Idle, Move, Alert, //Traversal states (movement). 
        Reload, Attack, Dodge //Action states (evade, attacks, etc).
	}

	public override void _Ready() {
		playerCameraReference = GetNode<Camera2D>("/root/GameWorld/SidMarshall/PlayerCamera");
		playerHealthBar = GetNode<ProgressBar>("/root/GameWorld/UIElements/PlayerUI/PlayerUIAnchor/Health");
		playerReference = GetNode<PlayerController>("/root/GameWorld/SidMarshall");
		maximumHealth = playerReference.maximumHealth;
		playerHealth = playerReference.currentHealth;
		currentEnemyState = EnemyState.Idle; 
	}
	
	public override void _Process(double delta) {}
}
