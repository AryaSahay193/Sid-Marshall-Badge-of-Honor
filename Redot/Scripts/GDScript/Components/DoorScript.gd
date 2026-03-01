class_name DoorScript
extends Node2D

# signal changeState

@export_group("Animations")
@export var doorAnimations : AnimatedSprite2D

@export_group("Collisions")
@export var playerDetection : Area2D

@export_group("Sound Effects")
@export var doorOpenSFX : AudioStreamPlayer
@export var doorCloseSFX : AudioStreamPlayer

@export_group("References")
@export_file("*.tscn") var sceneLocation : String # Accepts any file type of .tscn
@export var playerInstance : PackedScene
@export var spawnLoaction : Marker2D

@onready var singletonReference : GlobalData = get_node("/root/GlobalData")
@onready var inputManager : InputManager = get_node("/root/InputManager")
@onready var sceneManager : SceneManager = get_node("/root/SceneManager")
var playerScript : PlayerScript = singletonReference.playerScript
var areaDetected : bool = false; var doorOpened : bool = false

func _ready() -> void:
	playerDetection.area_entered.connect(areaEntered)
	playerDetection.area_exited.connect(areaExited)

	if areaDetected && !doorOpened && inputManager.verticalButton() > 0.0 :
		# changeState?.Invoke();
		doorOpened = true
		if singletonReference.insideBuilding : doorAnimations.Play("Default_Inside");
		else : doorAnimations.Play("Default_Outside");

func _process(delta: float) -> void:
	if areaDetected && !doorOpened : # DoorOpened - Flag boolean which will only make the door open once. 
		if inputManager.verticalButton() < 0.0 :
			# playerScript.sidDoorOpen += doorUnlocked;
			# playerScript.readyToChangeScene += () => changeToScene?.Invoke(sceneLocation); 
			pass
		else : return;
		doorOpened = true;  # Sets door open after opening. 

func doorUnlocked() :
	doorAnimations.Play("Enter_Inside");
	doorOpenSFX.Play();

func areaEntered(sidMarshall : Node2D) :
	if sidMarshall is PlayerScript : areaDetected = true;
	else : return

func areaExited(sidMarshall : Node2D) :
	if sidMarshall is PlayerScript : areaDetected = false;