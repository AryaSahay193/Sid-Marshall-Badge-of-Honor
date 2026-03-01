# Interface class, meaning it's a blueprint for all the children. Only empty methods will be here, that will be "borrowed" by children classes.
# This class will not be used directly, it only serves as a base structure for each state for the player. 
class_name StateParent
extends Node

@onready var singletonReference : GlobalData = get_node("/root/GlobalData")
@onready var inputManager : InputManager = get_node("/root/InputManager")
@onready var playerController : PlayerScript = singletonReference.playerScript
@onready var finiteStateMachine : FiniteStateMachine

@export_group("Attributes")
@export var animations : AnimatedSprite2D

func _process(delta: float) -> void:
	pass

func EnterState() : pass # Base method that executes code when entering the state.

func ExitState() : pass # Base method that executes code when exiting the current state.

func UpdateState(delta : float) : pass # Base method that handles logic of the current state.

func PhysicsUpdate(delta : float) : pass # Base method that usually handles physics and time-based calculations rather than frame-based.

func HandleInput(event : InputEvent) : pass # Base method that handles input.