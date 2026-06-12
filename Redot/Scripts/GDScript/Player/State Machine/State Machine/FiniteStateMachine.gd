class_name FiniteStateMachine
extends Node

var currentState : StateParent
var stateList : Dictionary = {}
@export var startingState : StateParent #Starting path is Idle, in Redot Inspector.

func _ready() -> void:
	for node in get_children() :
		if node is StateParent :
			stateList[node.name.to_lower()] = node
			node.initiateStateChange.connect(changeToState)

	if startingState :
		startingState.EnterState()
		currentState = startingState

# Delegates the methods from BaseStateClass to methods commonly used.
func _process(delta : float) -> void : 
	if currentState : currentState.UpdateState(delta)

func _physics_process(delta : float) -> void : 
	if currentState : currentState.PhysicsUpdate(delta)

func _unhandled_input(event: InputEvent) -> void: 
	if currentState : currentState.HandleInput(event)

func changeToState(stateName : String) :
	var nextState = stateList.get(stateName.to_lower())
	if currentState :
		currentState.ExitState()
	nextState.EnterState()
