extends Node # Autoload script

@export_group("Properties")
@export var transitionDuration : float
@export var screenFade : AnimationPlayer
@export var screenTransition : ColorRect

@export_group("References")
@export_file("*.tscn") var playerPath : String # Accepts any .tscn file.
@export_file("*.tscn") var doorPath : String # Accepts any .tscn file.

@onready var singletonReference : GlobalData = get_node("/root/GlobalData")
@onready var sidMarshall : PlayerScript = singletonReference.playerScript
@onready var doorScript : DoorScript = singletonReference.doorScript
@onready var sceneLocation : Node = get_node("/root/GameWorld/Areas/") # Path to stored currentScene
@onready var scenePath : String = doorScript.sceneLocation

var currentScene : Node; var previousScene : Node # Node types
var fadeTransition : Tween

func _ready() -> void:
	pass # Replace with function body.

func _process(delta: float) -> void:
	pass

func transitionToArea(scenePath : String) :
	screenFade.play("Fade");
	screenTransition.visible = true;
	previousScene = sceneLocation.get_child(1) # Stores current scene.
	doorScript.changeToScene.connect(get_tree().change_scene_to_file(scenePath)) # Changes scene.
	if sceneLocation != null :
		var nextScene = load(scenePath)
		if sceneLocation.GetChildCount() >= 1 :
			for childAmount in sceneLocation.get_child_count() : sceneLocation.get_child(childAmount).queue_free(); # Deletes all scenes if more than one.
		sceneLocation.add_child(nextScene);
		instantiatePlayer(playerPath);
		currentScene = nextScene;
	else : instantiatePlayer(playerPath);
	screenFade.play_backwards("Fade");
	screenTransition.visible = false;

func transitionToScreen(nextScreen : PackedScene) :
	fadeTransition = get_tree().create_tween();
	fadeTransition.tween_property(screenTransition, "modulate", Color("#000000"), transitionDuration) # Fades to black.
	get_tree().change_scene_to_packed(nextScreen);
	fadeTransition.TweenProperty(screenTransition, "modulate", Color("#00000000"), transitionDuration) # Fades to alpha.

func instantiatePlayer(pathToPlayer : String) :
	var playerScene = load(pathToPlayer) # Loads the player.
	sidMarshall = playerScene.instantiate("PlayerScript") # Creates a new instance of the player.
	sceneLocation.get_child(1).add_child(sidMarshall) # Adds the player as a child to the new scene.