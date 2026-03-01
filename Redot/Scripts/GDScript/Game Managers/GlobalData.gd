extends Node # Autoload script

@onready var playerScript : PlayerScript = get_node("/root/GameWorld/Player/SidMarshall")
@onready var playerCamera : CameraScript = get_node("/root/GameWorld/Player/SidMarshall/PlayerCamera/Camera")
@onready var playerInfo : PlayerUI = get_node("/root/GameWorld/Player/SidMarshall/PlayerCamera/Camera/PlayerUI")

@export_group("Enemies")
@export var enemyBlueprint : EnemyParent

@export_group("Scene Properties")
@export var doorScript : DoorScript
@export var spawnPoint : Marker2D

var playerHealth : float; var maximumHealth : float = 100.0; # Health Attributes (float types): it will carry over through the entire game
var jigsawCounter : int = 0; var cardCounter : int = 0; # Collectible Attributes (int types): it will carry over through the entire game
var insideBuilding : bool = false; var isInBattleMode : bool = false; # Boolean types, false by default
var playerVelocity : Vector2

func _ready() -> void:
	insideBuilding = is_in_group("Environment - Inside");
	maximumHealth = playerScript.maximumHealth;
	playerHealth = playerScript.currentHealth;

func generateRandomDecimal() :
	var randomDecimal : float = randf()
	return randomDecimal

func generateRandomNumber(startingRange: float, endingRange: float) :
	var randomInteger : float = randf_range(startingRange, endingRange)
	return randomInteger

func generateSign() :
	var signValue : int = randi_range(0, 1) * pow(1, randi_range(0, 1))
	return signValue