extends Node # Autoload script

var doublePressDuration : float; var doublePressDelay : float # Float types
var horizontalDirection : float; var verticalDirection : float # Float types
var isPressed : bool; var isTapped : bool; var isDoublePressed : bool # Boolean types

func _process(delta: float) -> void:
	doublePressDuration -= delta
	
func horizontalButton() -> float : # Handles left and right input, important for movement.
	horizontalDirection = Input.get_axis("player_left", "player_right")
	return horizontalDirection

func verticalButton() -> float : # Handles up and down input, important for climbing ladders and crouching/sliding.
	verticalDirection = Input.get_axis("player_up", "player_down")
	return verticalDirection

func runButton() -> bool :
	var isRunning : bool = Input.is_action_pressed("player_run")
	if Input.is_action_just_released("player_run") : isRunning = false
	return isRunning

func jumpButton() -> bool :
	var isJumping : bool = Input.is_action_just_pressed("player_jump")
	return isJumping

func crouchButton() -> bool :
	var isCrouching : bool = Input.is_action_pressed("player_down")
	if Input.is_action_just_released("player_down") : isCrouching = false
	return isCrouching

func lightAttackButton() -> bool : # Input which lets Sid punch enemies. Only available in the Battle state.
	var lightAttackPressed : bool = Input.is_action_just_pressed("player_light_attack")
	return lightAttackPressed

func heavyAttackButton() -> bool : # Input which lets Sid kick enemies. Only available in the Battle state.
	var heavyAttackPressed : bool = Input.is_action_just_pressed("player_heavy_attack")
	return heavyAttackPressed

func grabButton() -> bool : # Input which lets Sid grab enemies. Only available in the Battle state.
	var isGrabbing : bool = Input.is_action_just_pressed("player_grab")
	return isGrabbing

func interactButton() -> bool : # Input responsible for interacting with in-game objects, such as NPCs, doors, etc.
	var isInteracting : bool = Input.is_action_just_pressed("player_interact")
	return isInteracting

func menuButton() -> bool : # Input responsible for pausing the game, or bringing up menus.
	var menuPressed : bool = Input.is_action_just_pressed("player_pause")
	return menuPressed

func cameraButton() -> bool : # Input responsible for activating camera pan, or look mode.
	var cameraPressed : bool = Input.is_action_just_pressed("camera_activate")
	return cameraPressed

func mouseClick() -> bool :
	var mousePressed : bool = Input.is_mouse_button_pressed(0)
	return mousePressed

func directionalInput() -> Vector2 :
	var axisDirections : Vector2 = Input.get_vector("player_left", "player_right", "player_up", "player_down")
	return axisDirections

func cameraInput() -> Vector2 :
	var horizontalInput : float = Input.get_axis("camera_left", "camera_right")
	var verticalInput : float = Input.get_axis("camera_up", "camera_down")
	var cameraDirections : Vector2 = Vector2(horizontalInput, verticalInput)
	return cameraDirections

# public override void _Input(InputEvent @event) {
#	if(Input.IsAnythingPressed()) GetViewport.SetInputAsHandled();
# }

#Method that will handle double-press inputs.
#public void doublePressed(InputEvent event) {
#	if(Input.IsActionJustPressed(actionName)) {
#		if(Input.IsActionJustPressed(actionName)) isDoublePressed = true;
#	} else isDoublePressed = false;
#	return isDoublePressed;
#}