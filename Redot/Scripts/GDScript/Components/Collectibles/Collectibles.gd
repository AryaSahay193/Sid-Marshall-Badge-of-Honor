class_name Collectible # Menu which connects from the Pause Menu.
extends Node2D

@onready var singletonReference : GlobalData = get_node("/root/GlobalData")

@export_group("Components")
@export var collectibleAnimation : AnimatedSprite2D
@export var pickupSFX : AudioStreamPlayer

var collectibleSpeed : float = 5.0; var collectibleAmplitude : float = 12.0; var movementDuration : float = 0.5; # Float types
var collectibleCounter : int = 0; var collectibleAmount : int = 1; # Int types, values of collectibles
var collectiblePosition : Vector2;

func _ready() :
	collectibleCounter = singletonReference.jigsawCounter;
	collectiblePosition = global_position;

func collectibleCaught(body : CharacterBody2D) : # Signal method for when the puzzle piece is caught.
	if body is PlayerScript :
		collectibleCounter += collectibleAmount # Adds the value to the counter.
		pickupSFX.play() # Plays a sound when caught.
		self.queue_free() # Deletes the collectible.