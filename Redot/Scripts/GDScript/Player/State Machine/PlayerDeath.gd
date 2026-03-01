# Handles code when the player dies.
class_name PlayerDeath
extends StateParent

func EnterState() : 
	playerController.set_physics_process(false)
	playerController.set_process_input(false)
	playerController.playerAnimations.Play("Death")