using Godot;
using System;

public partial class DoorScript : Node2D {
	public event Action doorEntered;
	private PlayerController playerReference;
	private GlobalData singletonReference;
	private InputManager inputManager;
	private SceneManager sceneManager;
	private bool areaDetected = false, doorOpened = false;
	
	[ExportGroup("Animations")]
	[Export] private AnimatedSprite2D doorAnimations;
	
	[ExportGroup("Collisions")]
	[Export] private Area2D playerDetection;

	[ExportGroup("Sound Effects")]
	[Export] private AudioStreamPlayer doorOpenSFX, doorCloseSFX;

	public override void _Ready() {
        singletonReference = GetNode<GlobalData>("/root/GlobalData");
		inputManager = GetNode<InputManager>("/root/InputManager");
		sceneManager = GetNode<SceneManager>("/root/SceneManager");
		playerReference = singletonReference.playerReference;
		playerDetection.BodyEntered += rangeEntered;
		playerDetection.BodyExited += rangeExited;
    }

	public override void _Process(double delta) {
		if(areaDetected) {
			if(!doorOpened) { //Flag boolean which will only make the door open once.
				if(inputManager.verticalButton() < 0.0f) {
					doorEntered?.Invoke(); //Emits signal
					doorAnimations.Play("Enter_Inside");
					doorOpenSFX.Play();
				} else return;
				doorOpened = true; //Sets door open after opening.
				//doorAnimations.AnimationFinished += sceneManager.transitionToScene();    
			} 
		} 
    }

	private void rangeEntered(Node2D sidMarshall) {
		if(sidMarshall is PlayerController) areaDetected = true;
		else return;
	}

	private void rangeExited(Node2D sidMarshall) {
		if(sidMarshall is PlayerController) areaDetected = false;
	}
}