using Godot;
using System;
using System.Diagnostics;

public partial class jigsawpuzzle : Node2D {
	playerdata inventoryInformation; //Gets the class playerdata from the "playerdata" script.
	//The playerdata script is a universal script that stores the player's inventory and counts the number of collectibles you found.
    public void OnJigsawEntered() {
		QueueFree(); //Destroys the object if the player touches it.
		inventoryUpdate(inventoryInformation);
	}

	private void inventoryUpdate(playerdata inventoryInformation) {
		inventoryInformation = GetTree().Root.GetNode<playerdata>("playerdata"); //Gets information from the "playerdata" script.
		inventoryInformation.JigsawCounter(1); //Adds one everytime you collect a Jigsaw-puzzle piece.
	}
}
