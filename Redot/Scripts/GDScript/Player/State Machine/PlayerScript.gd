class_name PlayerScript
extends CharacterBody2D

# signal DamageToEnemy # Int type signal sent for when the player damages enemies.
# signal StaminaDeplete; # Float type signal sent for stamina-value changes.
# signal readyToChangeScene; # Signal emits when animation finishes, to change scene.
# signal sidDoorOpen; # Signal sent for door animations should play.

@export_group("Debug")
@export var debugText : RichTextLabel

@export_group("Animations")
@export var playerAnimations : AnimatedSprite2D

@export_group("Collision")
@export var playerCollision : CollisionShape2D
@export var normalCollision : Shape2D # Regular player collision.
@export var crouchCollision : Shape2D # Collisions used for crouching.

@export_group("Effects")
@export var dustParticles : CPUParticles2D

@export_group("Raycasts") # GroundDetection used for Landing state, WallDetection used for wall-jump direction.
@export var ceilingDetection : RayCast2D 
@export var wallDetection : RayCast2D 

@export_group("Timers") # For organization purposes in the game engine.
@export var airborneTimer : Timer
@export var inputBuffer : Timer
@export var wallJumpTimer : Timer

@export_group("References")
@export var cameraReference : CameraScript
@export var stateMachine : FiniteStateMachine

var coyoteCounter : float = 0.53; var coyoteTime : float; var verticalAcceleration : float = 7.95; var floorAngle : float; var maxFloorAngle : float
var lightAttackStamina : float = -0.1; var heavyAttackStamina : float = -0.5; var restingStamina : float = 0.5;
var wallRayDirection : float; var gravityValue : float = 313.23;
var currentHealth : float; var maximumHealth : float = 100.0;
var signalSent : bool = false;

func _ready() -> void:
	GlobalData.player = self # Initializes this as the script reference.
	wallRayDirection = wallDetection.scale.x
	currentHealth = maximumHealth
	maxFloorAngle = floor_max_angle

func _process(delta: float) -> void:
	stateMachine._process(delta) # Initializes Process method, so the states can directly change characteristics.
	flipCharacter();

func _physics_process(delta : float) :
	stateMachine._physics_process(delta) # Sets movement logic within the main Physics Process of this script.
	if !is_on_floor() : velocity = Vector2(0, move_toward(velocity.y, pow(gravityValue, 1.1), verticalAcceleration)) # Sets the character gravity.
	move_and_slide() # A necessary method to make movement work in Redot engine.

func _input(event: InputEvent) : stateMachine._unhandled_input(event)

func flipCharacter() : # Flips the player (Raycasts and animations).
	if InputManager.horizontalButton() != 0.0 :
		if InputManager.horizontalButton() > 0.0 :
			wallRayDirection = 1.0
			wallDetection.scale = Vector2(wallRayDirection, wallDetection.scale.y)
			playerAnimations.flip_h = false
		else :
			wallRayDirection = -1.0
			wallDetection.scale = Vector2(wallRayDirection, wallDetection.scale.y)
			playerAnimations.flip_h = true

func calculateVelocity(startingVelocity : float, endingVelocity : float, increment : float) :
	var oneAxisSpeed : float = move_toward(startingVelocity, endingVelocity * InputManager.horizontalButton(), increment)
	return oneAxisSpeed

# /*private float animationLengthOf(string animationName) {
# 	float framesPerSecond = (float)playerAnimations.SpriteFrames.GetAnimationSpeed(animationName);
# 	float numberOfFrames = playerAnimations.SpriteFrames.GetFrameCount(animationName);
# 	float animationDuration = framesPerSecond/numberOfFrames;
# 	return animationDuration;
# }*/
