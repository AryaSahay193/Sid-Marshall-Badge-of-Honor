# Interface class, meaning it's a blueprint for all the children. Only empty methods will be here, that will be "borrowed" by children classes.
# This class will not be used directly, it only serves as a base structure for each state for the player. 
class_name StateParent
extends Node

signal initiateStateChange(state : StateParent)
@onready var playerReference : PlayerScript = get_node("/root/GameWorld/Player/SidMarshall")
@onready var finiteStateMachine : FiniteStateMachine = get_node("/root/GameWorld/Player/SidMarshall/FiniteStateMachine")

func _process(_delta: float) -> void: pass # For delgation purposes
func _ready() -> void: pass # For delegation purposes

func EnterState() : pass # Base method that executes code when entering the state.
func ExitState() : pass # Base method that executes code when exiting the current state.
func UpdateState(_delta : float) : pass # Base method that handles logic of the current state.
func PhysicsUpdate(_delta : float) : pass # Base method that usually handles physics and time-based calculations rather than frame-based.
func HandleInput(_event : InputEvent) : pass # Base method that handles input.

func playAnimationOnce(animationName : String) :
	var animationPlaying : bool = playerReference.playerAnimations.is_playing()
	if animationPlaying :
		playerReference.playerAnimations.play(animationName)
		animationPlaying = !animationPlaying

func pauseInputOnAnimation() :
	if playerReference.playerAnimations.is_playing() :
		set_physics_process(false)
		set_process_input(false)
	else : set_process_input(true)
