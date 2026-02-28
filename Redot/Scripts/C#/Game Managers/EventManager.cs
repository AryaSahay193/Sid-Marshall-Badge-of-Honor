using Godot;
using System;

//Class that handles events in game, such as collection of collectibles, hitting or defeating enemies, talking to NPCs, etc.
public partial class EventManager : Node { //Manager that deals with events in the game, such as story-events or collectables.
	[Signal] public delegate void AchievementActivatedEventHandler(bool achievementFlag); //Sginal that is sent when an achievement triggers.
	[Signal] public delegate void EncounterStartedEventHandler(bool inBattleMode); //Signal that is sent once Sid enters a Gang Encounter.
	[Signal] public delegate void JigsawCollectedEventHandler(int amount); //Signal that is sent when a Collectible is collected.
	[Signal] public delegate void PlayerDamageEventHandler(float amount); //Signal that handles damage given by the player.
	[Signal] public delegate void EnemyDamageEventHandler(float amount); //Signal that handles damage given by the enemy.
}
