class_name Johnny
extends EnemyParent

signal InitiateBattle

@onready var singletonReference : GlobalData = get_node("/root/GlobalData")
@onready var currentState : JohnnyState = JohnnyState.Idle
@onready var enemyVelocity : Vector2 = velocity

@export_group("Animations")
@export var johnnyAnimations : AnimatedSprite2D

@export_group("Audio")
@export var walkSFX : AudioStreamPlayer2D

@export_group("Collisions")
@export var enemyCollision : CollisionShape2D
@export var johnnyHurtBox : Area2D
@export var playerDetection : Area2D

@export_group("Timers")
@export var encounterTimer : Timer
@export var wanderTimer : Timer

enum JohnnyState { Idle, Move, Alert, Throw, Attack, Hit, Stun, Death }
var walkingSpeed : float = 75.0; var battleSpeed : float = 45.0; # Float types
var isAlerted : bool = false; # Boolean type
var enemyPosition : Vector2 # Vector2 types
var enemyDirection : int # Int type
var entity : Node2D # Node2D type

func _ready() -> void:
	if singletonReference.randomDecimal() <= 0.5 : wanderTimer.wait_time = singletonReference.randomDecimal() * 10.0
	enemyDirection = singletonReference.generateSign()

func _process(delta: float) -> void:
	if enemyVelocity.x != 0.0 : johnnyAnimations.play("Walk")
	else : johnnyAnimations.play("Idle")
	if entity is PlayerScript :
		playerDetection.area_entered.connect(isAlerted = true)
		playerDetection.area_exited.connect(isAlerted = true)
	print(isAlerted); # wanderTimer.Timeout += enemyMovement;

func _physics_process(delta: float) :
	self.global_position += Vector2(walkingSpeed * enemyDirection, get_gravity().y) * (float)delta # Enemy will move from one end of the screen to another.
	move_and_slide()

func _exit_tree() :
	playerDetection.area_entered.disconnect(isAlerted)
	playerDetection.area_exited.disconnect(isAlerted)

func StateConditions() :
	if is_on_floor() :
		if isAlerted : currentState = JohnnyStates.Alert;
		else :
			if enemyDirection != 0.0 : currentState = JohnnyStates.Move
			else : currentState = JohnnyStates.Idle

func flipEnemy() :
	pass