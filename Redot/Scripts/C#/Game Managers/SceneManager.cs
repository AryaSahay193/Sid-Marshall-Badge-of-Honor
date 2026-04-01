using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public sealed partial class SceneManager : CanvasLayer {
	public event Action sceneChanged; //Signal emitted when the scene finishes loading.

	[ExportGroup("Properties")]
	[Export] private float fadeDuration;
	[Export] private ColorRect screenTransition;

	[ExportGroup("References")]
	[Export(PropertyHint.File, "*.tscn")] private string doorPath; //Accepts any .tscn file.
	
	private PlayerController playerController;
	private GlobalData singletonReference;
	private DoorScript doorScript;
	private Node nextScene, previousScene;
	private Node2D sceneStorage;
	private Tween fadeTransition;
	private String nodePath;

	public override void _Ready() {
		singletonReference = GetNode<GlobalData>("/root/GlobalData");
		sceneStorage = singletonReference.sceneStorage; //Path to stored currentScene
		playerController = singletonReference.playerController;
		doorScript = singletonReference.doorScript;
		nodePath = doorScript.sceneLocation;
	}

	public void sceneTransition(String sceneDirectory) {
		//Tween screenFade = GetTree().CreateTween();
		//screenFade.TweenProperty(screenTransition, "modulate", new Color("#000000"), fadeDuration); //Fades to black.
		//screenTransition.Visible = true;
		sceneStorage.GetChild(0).QueueFree();
		instantiateScene(sceneDirectory); //Adds the new scene as a child.
		spawnPlayerAtDoor();
		sceneChanged?.Invoke(); //Sends signal to finish Door State. 
		//screenFade.TweenProperty(screenTransition, "modulate", new Color("#00000000"), fadeDuration); //Fades to alpha/transparent.
	}
	
	/*public void menuTransition(PackedScene nextScreen) {
		fadeTransition = GetTree().CreateTween();
		fadeTransition.TweenProperty(screenTransition, "modulate", new Color("#000000"), fadeDuration); //Fades to black.
		GetTree().ChangeSceneToPacked(nextScreen);
		fadeTransition.TweenProperty(screenTransition, "modulate", new Color("#00000000"), fadeDuration); //Fades to alpha/transparent.
    }*/

	private void instantiateScene(String pathToScene) {
		Node2D sceneBlueprint = GD.Load<PackedScene>(pathToScene).Instantiate<Node2D>();
		sceneBlueprint.GlobalPosition = playerController.GlobalPosition; //Sets the scene at the player's location.
		sceneStorage.AddChild(sceneBlueprint); //Adds the newly instantiated object as a child of the scene tree.
		nextScene = sceneBlueprint;
	}

	private async void spawnPlayerAtDoor() {
		nextScene.FindChild(doorPath);
		if(nextScene.HasNode(doorPath)) playerController.GlobalPosition = doorScript.spawnLocation.GlobalPosition;
		else GD.Print("This scene does not contain a door.");
	}
}
