class_name PlayerUI
extends CanvasLayer

signal DepletedEnergy

@export_group("Animations")
@export var healthBarAnimations : AnimatedSprite2D

@export_group("Components")
@export var staminaBar : ProgressBar
@export var healthBar : ProgressBar
@export var damageBar : ProgressBar

@onready var singletonReference = get_node("/root/GlobalData")
@onready var playerReference = singletonReference.playerController

var playerDirection; var staminaValue; # Float types
var playerVelocity; # Vector2 types
var isGrounded; # Boolean type

func _ready() -> void:
	healthBar.value = playerReference.currentHealth # Sets player-health to current-health
	playerVelocity = playerReference.Velocity # Sets the player velocity as a variable

func _process(_delta: float) :
	# public override void _Process(double delta) {
	# 	playerController.StaminaDeplete += (staminaValue) => staminaBar.Value += staminaValue * delta;
	# 	if(staminaBar.Value == 0.0f) DepletedEnergy?.Invoke();
	# 	} //healthBarAnimations.AnimationFinished += () => healthBarAnimations.Play("Default");
	# }
	pass
