using Godot;
using System;

public partial class Ladder : Area2D {
    bool climbing = false; //By default, climbing is switched off.
    Vector2 velocity = new Vector2();
    
    //Exported Variables (from Player-script)
    [Export] CharacterBody2D SidMarshall; //Grabs the CharacterBody2D Node from the player script and imports it here.
    [Export] int climbingSpeed; //Grabs the climbingSpeed variable from the player script and imports it here.
    [Export] int gravity; //Grabs the jumpingSpeed variable from the player script and imports it here.

    private void OnBodyEntered(Node2D Character) {
        if(Character.IsInGroup("Player")) {
            if(climbing == false) {
                climbing = true;
                velocity.Y = 0; //Stationary player.
                if(Input.IsActionPressed("ui_up")) {
                    velocity.Y = -climbingSpeed;    
                } else if(Input.IsActionPressed("ui_down")) {
                    velocity.Y = climbingSpeed;
                }
            }

                if(Input.IsActionPressed("ui_left") || Input.IsActionPressed("ui_right")) {
                climbing = false;
                velocity.Y += gravity; //Does regular gravity
            }
        } 
    }

    private void OnBodyExited(Node2D Character) {
        if(Character.IsInGroup("Player")) {
            if(climbing == true) {
                climbing = false;
            }
        }
    }
}
