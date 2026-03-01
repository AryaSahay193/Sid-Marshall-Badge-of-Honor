class_name PlayerScript
extends CharacterBody2D

signal DamageToEnemy # Int type signal sent for when the player damages enemies.
signal StaminaDeplete; # Float type signal sent for stamina-value changes.
signal readyToChangeScene; # Signal emits when animation finishes, to change scene.
signal sidDoorOpen; # Signal sent for door animations should play.

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

@onready var singletonReference : GlobalData = get_node("/root/GlobalData")
@onready var inputManager : InputManager = get_node("/root/InputManager")
@onready var sceneManager : SceneManager = get_node("/root/SceneManager")
@onready var enemyBlueprint : EnemyParent = singletonReference.enemyBlueprint
@onready var doorScript : DoorScript = singletonReference.doorScript

var coyoteCounter : float = 0.53; var coyoteTime : float; var verticalAcceleration : float = 7.95; var floorAngle : float; var maxFloorAngle : float
var lightAttackStamina : float = -0.1; var heavyAttackStamina : float = -0.5; var restingStamina : float = 0.5;
var wallRayDirection : float; var gravityValue : float = 313.23;
var currentHealth : float; var maximumHealth : float = 100.0;
var isInBattleMode : bool = false; var signalSent : bool = false;

func _ready() -> void:
	isInBattleMode = singletonReference.isInBattleMode;
	wallRayDirection = wallDetection.Scale.x;
	currentHealth = maximumHealth;
	maxFloorAngle = floor_max_angle;

func _process(delta: float) -> void:
	# stateMachine?._Process((float)delta); //Initializes Process method, so the states can directly change characteristics.
	flipCharacter();

func _physics_process(delta : float) :
	# stateMachine?._PhysicsProcess((float)delta); //Sets movement logic within the main Physics Process of this script.
	# if(!IsOnFloor()) Velocity = Velocity with {Y = Mathf.MoveToward(Velocity.Y, Mathf.Pow(gravityValue, 1.1f), verticalAcceleration)}; //Sets the character gravity.
	# MoveAndSlide(); //A necessary method to make movement work in Redot engine.
	pass

func _input(event: InputEvent) :
	# stateMachine?._UnhandledInput(@event);
	pass

func flipCharacter() : # Flips the player (Raycasts and animations).
	if inputManager.horizontalButton() != 0.0 :
		if inputManager.horizontalButton() > 0.0 :
			wallRayDirection = 1.0
			wallDetection.scale = Vector2(wallRayDirection, wallDetection.scale.y)
			playerAnimations.flip_h = false
		else :
			wallRayDirection = -1.0
			wallDetection.scale = Vector2(wallRayDirection, wallDetection.scale.y)
			playerAnimations.flip_h = true

func calculateVelocity(startingVelocity : float, endingVelocity : float, increment : float) :
	var oneAxisSpeed : float = move_toward(startingVelocity, endingVelocity * inputManager.horizontalButton(), increment)
	return oneAxisSpeed

func playAnimationOnce(animationName : String) :
	var animationPlaying : bool = playerAnimations.is_playing()
	if animationPlaying :
		playerAnimations.play(animationName)
		animationPlaying = !animationPlaying

func pauseInputOnAnimation() :
	if playerAnimations.is_playing() :
		set_physics_process(false)
		set_process_input(false)
	else : set_process_input(true)

# /*private void openDoorAnimation() {
# 	sidDoorOpen?.Invoke(); //Sends signal to DoorScript to play Door Animation in a timely manner.
# 	sidDoorOpen += () => playerAnimations.Play("Door_Enter_Push");
# 	Tween colorChange = GetTree().CreateTween();
# 	colorChange.TweenProperty(playerAnimations, "modulate", new Color("#2d1e2f"), animationLengthOf("Door_Enter_Push"));
# 	readyToChangeScene?.Invoke(); //Emits signal when animation finishes
# 	SetPhysicsProcess(false);
# 	SetProcessInput(false);    
# }*/

# /*private float animationLengthOf(string animationName) {
# 	float framesPerSecond = (float)playerAnimations.SpriteFrames.GetAnimationSpeed(animationName);
# 	float numberOfFrames = playerAnimations.SpriteFrames.GetFrameCount(animationName);
# 	float animationDuration = framesPerSecond/numberOfFrames;
# 	return animationDuration;
# }*/