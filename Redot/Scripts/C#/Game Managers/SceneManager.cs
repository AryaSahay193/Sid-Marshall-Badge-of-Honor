using Godot;
using System;

public sealed partial class SceneManager : CanvasLayer {
	public event Action sceneChanged; //Signal emitted when the scene finishes loading.

	[ExportGroup("Properties")]
	[Export] private float transitionDuration;
	[Export] private AnimationPlayer screenFade;
	[Export] private ColorRect screenTransition;

	[ExportGroup("References")]
	[Export(PropertyHint.File, "*.tscn")] private string playerPath, doorPath; //Accepts any .tscn file.
	
	private DoorScript doorScript;
	private Node sceneLocation, currentScene, previousScene;
	private PlayerController sidMarshall;
	private GlobalData singletonReference;
	private Tween fadeTransition;
	private String scenePath;

	public override void _Ready() {
		singletonReference = GetNode<GlobalData>("/root/GlobalData");
		sceneLocation = GetNode<Node2D>("/root/GameWorld/Areas/"); //Path to stored currentScene
		sidMarshall = singletonReference.playerController;
		doorScript = singletonReference.doorScript;
		scenePath = doorScript.sceneLocation;
	}

    public override void _Process(double delta) {
        doorScript.changeToScene += (scenePath) => transitionToArea(scenePath);
    }


	public void transitionToArea(String scenePath) {
		screenFade.Play("Fade");
		screenTransition.Visible = true;
		previousScene = sceneLocation.GetChild(1); //Stores current scene.
		if(sceneLocation != null) {
			var nextScene = ResourceLoader.Load<Node2D>(scenePath); //Instantiating scene as Node2D
			if(sceneLocation.GetChildCount() >= 1) { //Delete every single area under Main world Areas path to add new scene.
				for(int childAmount = 0; childAmount <= sceneLocation.GetChildCount(); childAmount++) {
					if(sceneLocation.GetChildCount() != 0.0f) sceneLocation.GetChild(childAmount).QueueFree(); //Deletes all scenes if more than one.
					else sceneLocation.AddChild(nextScene);
				}
			} 
			instantiatePlayer(playerPath);
			currentScene = nextScene;
		} else instantiatePlayer(playerPath);
		GetTree().ChangeSceneToFile(scenePath); //Changes scene.
		screenTransition.Visible = false;
		screenFade.PlayBackwards("Fade");
	}
	
	public void transitionToScreen(PackedScene nextScreen) {
		fadeTransition = GetTree().CreateTween();
		//fadeTransition.SetPauseMode(Tween.TweenProcessMode.Process);
		fadeTransition.TweenProperty(screenTransition, "modulate", new Color("#000000"), transitionDuration); //Fades to black.
		GetTree().ChangeSceneToPacked(nextScreen);
		fadeTransition.TweenProperty(screenTransition, "modulate", new Color("#00000000"), transitionDuration); //Fades to alpha.
    }

	private void instantiatePlayer(String pathToPlayer) {
		var playerScene = ResourceLoader.Load<PackedScene>(pathToPlayer); //Loads the player.
		sidMarshall = playerScene.Instantiate<PlayerController>(); //Creates a new instance of the player.
		sceneLocation.GetChild(1).AddChild(sidMarshall); //Adds the player as a child to the new scene.
	}
}
