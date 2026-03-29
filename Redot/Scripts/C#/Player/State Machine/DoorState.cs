using Godot;
using System;

public partial class DoorState : ParentState {
	public event Action doorUnlocked, enteredScene;
	public DoorState doorStateReference;
	private bool doorOpened = false;

    public override void _EnterTree() {
		singletonReference = GetNode<GlobalData>("/root/GlobalData");
        singletonReference.doorState = this;
    }

	public override void EnterState() {
		playerController.debugText.Text = "[center]State: Door[/center]";
		playerAnimations.Play("Door_Open");
		playerAnimations.AnimationFinished += () => doorUnlocked?.Invoke();
		doorUnlocked += () => EnterDoorAnimation();
	}
	
	public override void PhysicsUpdate(float delta) {
		playerController.Velocity = playerController.Velocity with {X = 0.0f};
		SetPhysicsProcess(false);
		SetProcessInput(false);
	}

	public override void ExitState() {
		//playerAnimations.Play("Door_Exit_Pull");
	}

	private void EnterDoorAnimation() {
		if(!doorOpened) {
			playerAnimations.Play("Door_Enter_Push");
			if(playerAnimations.IsPlaying()) {
				Tween colorChange = GetTree().CreateTween();
				colorChange.TweenProperty(playerAnimations, "modulate", new Color("#2d1e2f"), animationLengthOf("Door_Enter_Push"));
			} playerAnimations.AnimationFinished += () => enteredScene?.Invoke();
		} doorOpened = true; 
	}
}