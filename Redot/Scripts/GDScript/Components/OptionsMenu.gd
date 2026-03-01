class_name OptionsMenu # Menu which connects from the Pause Menu.
extends CanvasLayer

@export_group("Buttons")
@export var audioButton : Button
@export var videoButton : Button
@export var controlButton : Button
@export var returnButton : Button

@export_group("Properites")
@export var pauseFade : ColorRect

@export_group("References")
@export var pauseMenu : CanvasLayer

@onready var inputManager : InputManager = get_node("/root/InputManager")
@onready var gamePaused : bool = get_tree().paused

func _process(_delta: float) -> void:
	if inputManager.menuButton() :
		if !gamePaused : optionsFunction()
		else : returnToPause()

func _exit_tree() :
	returnButton.ButtonUp.disconnect(optionsFunction)

func optionsFunction() :
	get_tree().paused = true
	self.visible = false
	# optionsMenu.visible = true

func returnToPause() :
	self.visible = false
	pauseMenu.visible = true