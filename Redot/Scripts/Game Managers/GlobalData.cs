using Godot;
using System;

//Script that lets other scripts access data globally, a singleton that will be autoloaded on Game start.
public sealed partial class GlobalData : Node {
	[Export] public CameraScript playerCamera ; //Only one instance of the player-camera.
	[Export] public DoorScript doorScript; //Publicly accessible door class.
	[Export] public PlayerController playerController; //Only one instance of the player controller.
	[Export] public PlayerUI playerInfo; //Only one instance of player-health isntance.

	public float playerHealth, maximumHealth = 100.0f; //Health will carry over through the entire game.
	public int jigsawCounter = 0, documentCounter = 0; //Collectibles will carry over throughout the game.
	public bool insideBuilding = false, isInBattleMode = false; //By default.
	
	public EnemyState currentEnemyState;
	public Vector2 playerVelocity;

	public enum EnemyState { //Character States
        Idle, Move, BattleIdle, BattleMove, Alert, //Traversal states (movement). 
        Reload, Attack, Dodge //Action states (evade, attacks, etc).
	}

	public override void _Ready() {
		insideBuilding = IsInGroup("Environment - Inside");
		playerVelocity = playerController.characterVelocity;
		maximumHealth = playerController.maximumHealth;
		playerHealth = playerController.currentHealth;
		currentEnemyState = EnemyState.Idle; 
		playerController.IsOnFloor();
	}

	public float randomDecimal() {
        Random randomDecimal = new Random();
		float decimalValue = (float)randomDecimal.NextDouble();
		return decimalValue;
    }

	public int randomNumber(int startingRange, int endingRange) {
        Random randomNumber = new Random();
		int generatedNumber = randomNumber.Next(startingRange, endingRange);
		return generatedNumber;
    }

	public int randomSign() {
		Random numberSign = new Random();
		int signValue = (int)Mathf.Pow(-1, numberSign.Next(0, 1));
		return signValue;
    }
}
