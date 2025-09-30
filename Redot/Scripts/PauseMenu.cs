using Godot;
using System;

public partial class PauseMenu : CanvasLayer { //In Redot, make sure the PROCESS is set to "When Paused" for this script to work.
	[Export] private Button resumeButton, optionsButton, achievementButton, menuButton, quitButton;
	private InputManager inputManager;
	private GameManager gameManager;
	private ColorRect pauseFade;
	private bool gamePaused;

	public override void _Ready() {
		inputManager = GetNode<InputManager>("/root/InputManager");
		gameManager = GetNode<GameManager>("/root/GameManager");
		pauseFade = GetNode<ColorRect>("PauseMenu/PauseColor");
		gamePaused = GetTree().Paused;
	}

	public override void _Process(double delta) {
		if(inputManager.menuButton() && !gamePaused) pauseFunction();
		else if(inputManager.menuButton() && gamePaused) resumeFunction();
		quitButton.Pressed += quitPressed;
	}

	public override void _ExitTree() {
		quitButton.ButtonUp -= quitPressed;		
	}

	private void resumeFunction() { 
		this.Visible = false;
		gameManager.currentGameState = GameManager.GameState.Normal;
		GetTree().Paused = false;
	}

	private void pauseFunction() {
		this.Visible = true;
		gameManager.currentGameState = GameManager.GameState.Paused;
		GetTree().Paused = true;
	}

	//Signal methods.
	private void resumePressed() => resumeFunction(); //Signal method for when the RESUME button is pressed.
	private void quitPressed() => GetTree().Quit(); //Signal method for when the QUIT button is pressed.
}