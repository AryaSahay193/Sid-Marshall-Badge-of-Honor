extends Node # Autoload script

signal achievementActivated # Signal which activates when achievement criteria is met.

@onready var singletonReference : GlobalData = get_node("/root/GlobalData")
@onready var playerUI : PlayerUI =  singletonReference.playerInfo
@onready var popUpTranslate : Tween = get_tree().create_tween();

@export_group("Attributes") 
@export var achievementBar : Sprite2D
@export var achievementIcon : Sprite2D
@export var achievementHeader : RichTextLabel
@export var achievementDescription : TextEdit

var achievementTriggered : bool; var screenshotOnPopUp : bool;

func _process(_delta: float) -> void:
	achievementActivated.connect(achievementPopUp)

func achievementPopUp() :
	self.visible = true;
	playerUI.visible = false;