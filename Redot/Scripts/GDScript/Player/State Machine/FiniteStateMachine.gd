class_name FiniteStateMachine
extends Node

@onready var currentState : StateParent = StateParent.new() # Reference to the State class.
@onready var stateName : Dictionary = {} # Nodepath, as a string, which is stored in a Dictionary
@export var startingState : NodePath #Starting path is Idle, in Redot Inspector.

func _ready() -> void:
	var state : StateParent
	for node : Node in get_children() :
		if node is StateParent :
			stateName[node.name] = state
			state.finiteStateMachine = self
			state._ready(); # Initialize states.
			state.ExitState(); # Reset the states.

	currentState = get_node(startingState)
	currentState.EnterState();

# Delegates the methods from BaseStateClass to methods commonly used.
func _process(delta : float) -> void : currentState.UpdateState(delta)
func _physics_process(delta : float) -> void : currentState.PhysicsUpdate(delta)
func _unhandled_input(event: InputEvent) -> void: currentState.HandleInput(event)

func StateTransition(dictionaryKey : String) :
	if !stateName.has(dictionaryKey) || currentState == stateName[dictionaryKey] : return # If key is not found, or if key is already in the dictionary, do nothing.
	currentState.ExitState()
	currentState = stateName[dictionaryKey]
	currentState.EnterState()