using Godot;
using System;

public partial class playerdata : Node2D {
	static int jigsawPuzzleAmount = 0; //Static variables can access all variables in all scripts.
	static int documentAmount = 0;
	static int totalJigsaw = 48; //There are a total of 48 Jigsaw Puzzles in the game.
	static int redJigsaw = 0; //Initially you start out with 0 red Jigsaw puzzles.
	static int orangeJigsaw = 0; //Initially you start out with 0 orange Jigsaw puzzles.
	static int yellowJigsaw = 0; //Initially you start out with 0 yellow Jigsaw puzzles.
	static int whiteJigsaw = 0; //Initially you start out with 0 white Jigsaw puzzles.
	static int brownJigsaw = 0; //Initially you start out with 0 brown Jigsaw puzzles.
	static int ashyJigsaw = 0; //Initially you start out with 0 black Jigsaw puzzles.

	public void JigsawCounter(int jigsawPuzzleAmount) {
		jigsawPuzzleAmount += 1;
		
		//ACHIEVEMENT: Piecing It Together
		if(jigsawPuzzleAmount == 1) {
			//Set Achievement: Piecing It Together
		} 
		
		//ACHIEVEMENT: The Missing Piece
		if(jigsawPuzzleAmount == totalJigsaw) {
			//Set Achievement: The Missing Piece
		}
	}

	public void DocumentCounter(int documentAmount) {
		documentAmount += 1;
		if(documentAmount == 17) {
			//Set Achievement: 
		}
	}

	//Inventory System coming soon.
}
