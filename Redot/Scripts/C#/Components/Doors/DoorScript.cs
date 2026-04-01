using Godot;
using System;
using System.Collections.Generic;

public partial class DoorScript : Node2D {
	public event Action insideHouse;
	
	public bool areaDetected, doorEntered = false;
	private GlobalData singletonReference;
	private SceneManager sceneManager;
	private PlayerController playerController;
	private DoorState doorState;

	[ExportGroup("Animations")]
	[Export] public AnimatedSprite2D doorAnimations;
	
	[ExportGroup("Collisions")]
	[Export] private Area2D playerDetection;

	[ExportGroup("Sound Effects")]
	[Export] private AudioStreamPlayer doorOpenSFX, doorCloseSFX;

	[ExportGroup("References")]
	[Export] public Marker2D spawnLocation;
	[Export(PropertyHint.File, "*.tscn")] public String sceneLocation; //Accepts any file type of .tscn

    public override void _EnterTree() {
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
		singletonReference = GetNode<GlobalData>("/root/GlobalData");
		playerController = singletonReference.playerController;
		doorState = singletonReference.doorState;
		singletonReference.doorScript = this;
    }

	public override void _Ready() {
		doorState.sceneChange += () => sceneManager.sceneTransition(sceneLocation);
		if(singletonReference.insideBuilding) doorAnimations.Play("Default_Inside");
		else doorAnimations.Play("Default_Outside");

		playerDetection.BodyEntered += playerEntered; //Signal which checks if player enters in front of door.
		playerDetection.BodyExited += playerExited; //Signal which checks if player exits the door.
		//sceneManager.sceneChanged += doorClosed;
		//GD.Print(areaDetected);
	}

	private void playerEntered(Node2D player) {
		if(player is PlayerController) {
			areaDetected = true;
			//GD.Print("Instance " + Name + " detected the player");
		} else return;
	}

	private void playerExited(Node2D player) {
		if(player is PlayerController) {
			areaDetected = false;
			//GD.Print(areaDetected);
		} else return;
	}

	public void openDoor() {
		if(!doorEntered) {
			GD.Print("Instance " + Name + " is opening");
			doorAnimations.Play("Enter_Inside"); 
			doorOpenSFX.Play();
		} doorEntered = true;
	}

	public void closeDoor() {
		if(!doorEntered) {
			doorCloseSFX.Play();
			doorAnimations.PlayBackwards("Enter_Outside"); 
			insideHouse?.Invoke(); //Signal emitted when animation is finished playing.	
		} doorEntered = true;
	}
}
