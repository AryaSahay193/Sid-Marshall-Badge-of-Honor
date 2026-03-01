class_name CameraUI
extends CanvasLayer

@export_group("Animations")
@export var animationLeft : AnimatedSprite2D
@export var animationRight : AnimatedSprite2D
@export var animationUp : AnimatedSprite2D
@export var animationDown : AnimatedSprite2D
@export var mouseAnimations : AnimatedSprite2D

@export_group("Buttons")
@export var buttonLeft : Button
@export var buttonRight : Button
@export var buttonUp : Button
@export var buttonDown : Button
@export var mouseButton : Button

@onready var singletonReference = get_node("/root/GlobalData")
@onready var inputManager = get_node("/root/InputManager")
@onready var playerReference = singletonReference.playerController
@onready var cameraReference : CameraScript
@onready var playerUI : PlayerUI 

var cameraPanActive : bool; var scenePaused : bool = false; var buttonToggled : bool  = false # Boolean types

func _ready() -> void:
	self.visible = false

func _process(_delta: float) -> void:
	if inputManager.cameraButton() : cameraPanActive = !cameraPanActive
	if(inputManager.cameraButton()) :
		if cameraPanActive :
			playerUI.visible = true
			self.visible = false
			mouseButton.pressed.connect(mouseButtonToggle)
			buttonLeft.ButtonDown.connect(leftPressed)
			buttonLeft.ButtonUp.connect(leftReleased)	
			buttonRight.ButtonDown.connect(rightPressed)
			buttonRight.ButtonUp.connect(rightReleased)
			buttonUp.ButtonDown.connect(upPressed)
			buttonUp.ButtonUp.connect(upReleased)
			buttonDown.ButtonDown.connect(downPressed)
			buttonDown.ButtonUp.connect(downReleased)
		else :
			playerUI.visible = true
			self.visible = false

func leftPressed() :
	animationLeft.Play("Press_Left")
	cameraReference.global_position += Vector2(cameraReference.cameraSpeed, 0.0)
	buttonRight.Disabled = true

func leftReleased() :
	animationLeft.Play("Release_Left")
	buttonRight.Disabled = false

func rightPressed() :
	animationRight.Play("Press_Right")
	cameraReference.global_position -= Vector2(cameraReference.cameraSpeed, 0.0)
	buttonLeft.Disabled = true

func rightReleased() :
	animationRight.Play("Release_Right")
	buttonLeft.Disabled = false

func upPressed() :
	animationUp.Play("Press_Up")
	cameraReference.global_position += Vector2(0.0, cameraReference.cameraSpeed)
	buttonDown.Disabled = true

func upReleased() :
	animationUp.Play("Release_Up")
	buttonDown.Disabled = false

func downPressed() :
	animationDown.Play("Press_Down")
	cameraReference.global_position -= Vector2(0.0, cameraReference.cameraSpeed)
	buttonUp.Disabled = true

func downReleased() :
	animationDown.Play("Release_Down")
	buttonUp.Disabled = false

func mouseButtonToggle() :
	mouseButton.ToggleMode = buttonToggled
	if buttonToggled : mouseAnimations.Play("Pressed")
	else : mouseAnimations.Play("Released")