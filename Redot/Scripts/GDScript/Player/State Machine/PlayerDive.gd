# Handles code when entering the second attack combo.
class_name PlayerDive
extends StateParent

func EnterState() : 
	playerController.playerAnimations.play("Dive_Roll")