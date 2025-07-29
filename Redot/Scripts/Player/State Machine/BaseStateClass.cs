using System;
using System.Collections.Generic;
using Godot;

//Interface class, meaning it's a blueprint for all the children. Only empty methods will be here, that will be "borrowed" by children classes.
//This class will not be used directly, it only serves as a base structure for each state for the player. 
public partial class BaseStateClass : Node {
	public const String DeathState = "Sid_Death";
	public const String IdleState = "Sid_Idle";
	public const String MoveState = "Sid_Move";
	public const String JumpState = "Sid_Jump";
	public const String FallState = "Sid_Fall";
	public const String PunchState = "Sid_Punch";
	public const String KickState = "Sid_Kick";

	[Export] public PlayerAttributes playerReference;
	[Export] public StateHandler finiteStateMachine;

	public override void _Ready() => finiteStateMachine = new StateHandler(); //Instantiates the StateHandler class.

	//Base method that executes code when entering the state.
	public virtual void EnterState() {}
	
	//Base method that executes code when exiting the current state.
	public virtual void ExitState() {}

	//Base method that handles logic of the current state.
	public virtual void UpdateState(float delta) {}
	
	//Base method that usually handles physics and time-based calculations rather than frame-based.
	public virtual void PhysicsUpdate(float delta) {}
}