using Godot;
using System;

public partial class DoorState : ParentState {
	public event Action sceneChange;
	private bool doorOpened = false;

    public override void _EnterTree() {
		singletonReference = GetNode<GlobalData>("/root/GlobalData");
        singletonReference.doorState = this;
    }

	public override void EnterState() {
		playerController.debugText.Text = "[center]State: Door[/center]";
		playerAnimations.Play("Door_Open");
		playerAnimations.AnimationFinished += () => EnterDoorAnimation();
		doorScript.insideHouse += ExitState; //Executes only once.
	}
	
	public override void PhysicsUpdate(float delta) {
		playerController.Velocity = Vector2.Zero;
		SetPhysicsProcess(false);
		SetProcessInput(false);
	}

	public override void ExitState() {
		playerAnimations.Play("Door_Exit_Pull");
		playerAnimations.AnimationFinished += () => SetPhysicsProcess(true);
		finiteStateMachine.ChangeStateTo("IdleState");
	}

	private void EnterDoorAnimation() {
		if(!doorOpened) {
			playerAnimations.Play("Door_Enter_Push");
			doorScript.openDoor(); //Executes only once.
			if(playerAnimations.IsPlaying()) {
				Tween colorChange = GetTree().CreateTween();
				colorChange.TweenProperty(playerAnimations, "modulate", new Color("#2d1e2f"), animationLengthOf("Door_Enter_Push"));
			} playerAnimations.AnimationFinished += () => sceneChange?.Invoke(); //Signal emitted to initiate scene change.
		} doorOpened = true; 
	}
}
