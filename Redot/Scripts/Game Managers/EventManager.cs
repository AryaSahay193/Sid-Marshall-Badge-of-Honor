using Godot;
using System;

public partial class EventManager : Node { //Manager that deals with events in the game, such as story-events or collectables.
	[Signal] public delegate void JigsawCollectedEventHandler(int amount); //Signal of type integer.

	public override void _Ready() {
	}

	public override void _Process(double delta) {
	}
}
