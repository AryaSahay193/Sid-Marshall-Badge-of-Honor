using Godot;
using System;

//Class that handles events in game, such as collection of collectibles, hitting or defeating enemies, talking to NPCs, etc.
public partial class EventManager : Node { //Manager that deals with events in the game, such as story-events or collectables.
	[Signal] public delegate void JigsawCollectedEventHandler(int amount); //Signal that is sent when a Collectible is collected.
	[Signal] public delegate void EncounterStartedEventHandler(bool inBattleMode); //Signal that is sent once Sid enters a Gang Encounter.
	[Signal] public delegate void EncounterEndedEventHandler(bool inBattleMode); //Sginal that is sent when the Gang Encounter ends.

	public override void _Ready() {
	}

	public override void _Process(double delta) {
	}
}
