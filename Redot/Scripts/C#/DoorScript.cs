using Godot;
using System;

public partial class DoorScript : Node2D {
	public bool areaDetected = false, doorOpened = false;
	public event Action initiateDoorOpen;
	public event Action<string> transitionToArea;

	private GlobalData singletonReference;
	private InputManager inputManager;
	private SceneManager sceneManager;
	private PlayerController playerController;

	[ExportGroup("Animations")]
	[Export] private AnimatedSprite2D doorAnimations;
	
	[ExportGroup("Collisions")]
	[Export] private Area2D playerDetection;

	[ExportGroup("Sound Effects")]
	[Export] private AudioStreamPlayer doorOpenSFX, doorCloseSFX;

	[ExportGroup("References")]
	[Export] public Marker2D spawnLocation;
	[Export] private PackedScene playerInstance;
	[Export(PropertyHint.File, "*.tscn")] public String sceneLocation; //Accepts any file type of .tscn

	public override void _Ready() {
        singletonReference = GetNode<GlobalData>("/root/GlobalData");
		inputManager = GetNode<InputManager>("/root/InputManager");
		sceneManager = GetNode<SceneManager>("/root/SceneManager");
		playerController = singletonReference.playerController;
		playerDetection.BodyEntered += rangeEntered;
		playerDetection.BodyExited += rangeExited;

		if(singletonReference.insideBuilding) doorAnimations.Play("Default_Inside");
		else doorAnimations.Play("Default_Outside");
    }

	public override void _Process(double delta) {
		if(areaDetected && !doorOpened) { //DoorOpened - Flag boolean which will only make the door open once. 
			if(inputManager.verticalButton() < 0.0f) {
				initiateDoorOpen?.Invoke(); //playerController.currentState = PlayerState.Door;
				doorAnimations.Play("Enter_Inside");
				doorOpenSFX.Play();
				playerController.readyToChangeScene += () => sceneManager.transitionToArea(sceneLocation);
			} else return;
			doorOpened = true; //Sets door open after opening. 
		} 
    }

	public void rangeEntered(Node2D sidMarshall) {
		if(sidMarshall is PlayerController) areaDetected = true;
		else return;
	}

	public void rangeExited(Node2D sidMarshall) {
		if(sidMarshall is PlayerController) areaDetected = false;
	}

	/*private void instantiatePlayer() {
		playerInstance.Instantiate(); //Creates a new instance of Sid Marshall.
		GetTree().CurrentScene.AddChild(playerReference); //Adds Sid as a child to the current scene.
		playerReference.Position = spawnLocation.Position;
	}*/
}
