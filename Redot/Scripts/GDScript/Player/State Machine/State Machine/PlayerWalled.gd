#Handles code when entering Wall-State.
class_name PlayerWalled
extends StateParent

var wallPushback : float = 343.44; var wallJumpHeight : float = -227.9; 
var wallSlideSpeed : float = 1298.50; var wallSlideAcceleration : float = 26.5;

func EnterState() :
	playerAnimations.play("Wall_Contact")
	# print("State: Walled")

func UpdateState(_delta : float) :
	if (playerReference.is_on_wall() && playerReference.wallDetection.is_colliding() && InputManager.horizontalButton() != 0.0) :
		if InputManager.jumpButton() : playerAnimations.play("Wall_Kick")
		else : playerAnimations.play("Wall_Slide")
	else : finiteStateMachine.changeToState("Fall")
	flipCharacter()

func PhysicsUpdate(_delta : float) :
	playerReference.velocity = Vector2(playerReference.velocity.x, 0.0) # Cancels gravity when holding towards a wall.
	if InputManager.jumpButton() : playerReference.velocity = Vector2((wallPushback * -InputManager.horizontalButton()), wallJumpHeight)
	else : playerReference.velocity = Vector2(playerReference.velocity.x, move_toward(playerReference.velocity.y, wallSlideSpeed, wallSlideAcceleration))
