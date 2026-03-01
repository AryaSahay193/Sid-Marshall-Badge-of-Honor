class_name EnemyParent
extends CharacterBody2D

signal InitiateBattle

@export_group("Attributes")
@export var playerDetection : Area2D
@export var encounterTimer : Timer

@onready var singletonReference : GlobalData = get_node("/root/GlobalData")
var entity : Node

func _process(delta: float) -> void:
	if entity is PlayerScript :
		playerDetection.area_entered.connect(encounterTimer.start)
		playerDetection.area_exited.connect(encounterTimer.stop)
	# encounterTimer.timeout.connect(InitiateBattle.Emit())

func _exit_tree() :
	playerDetection.area_entered.disconnect(encounterTimer.start)
	playerDetection.area_exited.disconnect(encounterTimer.stop)