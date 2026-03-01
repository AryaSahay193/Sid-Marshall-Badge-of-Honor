# Class that handles events in game, such as collection of collectibles, hitting or defeating enemies, talking to NPCs, etc.
# Broadcast means to create the signal, emit the signal means sending the signal to another node/script, and connect the signal means to receive the emitted signal.

extends Node # Autoload script

signal AchievementActivate # Boolean type, activated when achievement criteria are met.
signal BattleInitiated # Boolean type, changes game state to battle mode.
signal DamageReceived # Float type, damage given from enemy.
signal DamageGiven # Float type, damage given from player.