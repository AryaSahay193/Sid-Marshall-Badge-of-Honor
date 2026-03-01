class_name PauseMenu
extends CanvasLayer

@export_group("Buttons")
@export var achievementButton : Button
@export var optionsButton : Button
@export var resumeButton : Button
@export var menuButton : Button
@export var quitButton : Button

@export_group("Properites")
@export var pauseFade : ColorRect

@export_group("References")
@export var optionsMenu : CanvasLayer

@onready var inputManager : InputManager = get_node("/root/InputManager")
@onready var gamePaused : bool = get_tree().paused

func _process(_delta: float) -> void:
	if inputManager.menuButton() :
		if !gamePaused : # If not paused and pause button is pressed.
			pauseFunction(); 
			resumeButton.ButtonDown.connect(resumeFunction);
			optionsButton.ButtonDown.connect(optionsFunction);
			quitButton.ButtonDown.connect(get_tree().quit)
		else : resumeFunction() # If paused and pause button is pressed.

func _exit_tree() :
	resumeButton.ButtonUp.disconnect(resumeFunction)
	optionsButton.ButtonUp.disconnect(optionsFunction)

func resumeFunction() : 
	self.visible = false;
	get_tree().paused = false;

func pauseFunction() :
	self.visible = true;
	get_tree().paused = true;
	resumeButton.grab_focus();

func optionsFunction() :
	get_tree().paused = true;
	self.visible = false;
	optionsMenu.visible = true;