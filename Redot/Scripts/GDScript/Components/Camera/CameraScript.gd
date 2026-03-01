class_name CameraScript
extends Node2D

@export_group("Icons")
@export var mouseOpen : Resource
@export var mouseGrab : Resource

@onready var singletonReference = get_node("/root/GlobalData"); # GlobalData singleton.
@onready var inputManager = get_node("/root/InputManager"); # InputManager singleton.
@onready var playerReference = singletonReference.playerController; # PlayerController script.
@onready var cameraUI;

var horizontalMargin : float = 150.0; var verticalMargin : float = 100.0; var cameraReturnSpeed : float = 0.7; # Float types
var cameraSpeed : float = 1.219; var mouseMultiplier : float = 1.06; # Float types
var cameraDirection : Vector2; var playerPosition : Vector2; # Vector2 types.
var mouseDragged : bool; var screenCaptured : bool; var inCameraMode : bool; # Booleans

func _ready() -> void:
	playerPosition = playerReference.globalposition
	cameraDirection = inputManager.cameraInput();

func _process(_delta: float) -> void:
	var horizontalDirection = cameraDirection.x
	var verticalDirection = cameraDirection.y
	
	if cameraUI.Visible : inCameraMode = true
	else : inCameraMode = false

	if inCameraMode :
		playerReference.SetProcess(false)
		if inputManager.cameraInput() != 0 :
			global_position += Vector2(cameraSpeed * horizontalDirection, cameraSpeed * verticalDirection)
			if horizontalDirection < 0.0 :
				cameraUI.buttonLeft.ButtonDown.Connect(cameraUI.leftPressed)
				cameraUI.buttonLeft.ButtonUp.Connect(cameraUI.leftReleased)
			else : if horizontalDirection > 0.0 :
				cameraUI.buttonRight.ButtonDown.Connect(cameraUI.rightPressed)
				cameraUI.buttonRight.ButtonUp.Connect(cameraUI.rightReleased)
			else : if verticalDirection < 0.0 :
				cameraUI.buttonUp.ButtonDown.Connect(cameraUI.upPressed)
				cameraUI.buttonUp.ButtonUp.Connect(cameraUI.upReleased)
			else : if verticalDirection > 0.0 :
				cameraUI.buttonDown.ButtonDown.Connect(cameraUI.downPressed)
				cameraUI.buttonDown.ButtonUp.Connect(cameraUI.downReleased)
	else :
		returnToPlayer()
		playerReference.SetProcess(true)

func _physics_process(_delta: float) :
	if inCameraMode :
		playerReference.SetPhysicsProcess(false)
		playerReference.SetProcessInput(false)
	else :
		playerReference.SetPhysicsProcess(true)
		playerReference.SetProcessInput(true)

# func _input(event: InputEvent) :
#	if inCameraMode :
		# bool buttonHovered = (cameraUI.mouseButton.IsHovered() || cameraUI.buttonLeft.IsHovered() || cameraUI.buttonRight.IsHovered() || cameraUI.buttonUp.IsHovered() || cameraUI.buttonDown.IsHovered())

func returnToPlayer() : # Function used for the camera to return to player's position.
	if global_position != playerPosition :
		global_position = to_local(playerPosition) #Camera will snap back to the player's location.
		global_position = playerReference.GlobalPosition #Sets camera position to follow the player again.
	else : return

#public partial class CameraScript : Node2D {
#
#	//Redot built-in method that handles input. This function handles mouse input for dragging the camera and zooming in, along with custom pointers.
#    public override void _Input(InputEvent @event) {
#        if(inCameraMode) {
#			bool buttonHovered = cameraUI.mouseButton.IsHovered() || cameraUI.buttonLeft.IsHovered() || cameraUI.buttonRight.IsHovered() || cameraUI.buttonUp.IsHovered() || cameraUI.buttonDown.IsHovered();
#			if(buttonHovered) Input.SetCustomMouseCursor(null);
#			else Input.SetCustomMouseCursor(mouseOpen);
#			if(@event is InputEventMouseButton mouseButton) {
#				if(mouseButton.ButtonIndex == MouseButton.Left) mouseDragged = mouseButton.Pressed;
#			} if(@event is InputEventMouseMotion mouseMotion && mouseDragged == true && !buttonHovered) {
#				if(mouseDragged) Input.SetCustomMouseCursor(mouseGrab, Input.CursorShape.Drag);
#				GlobalPosition += (-mouseMotion.Relative * mouseMultiplier);
#			}
#		} else Input.SetCustomMouseCursor(null); //Sets cursor back to default.
#    }
#}
