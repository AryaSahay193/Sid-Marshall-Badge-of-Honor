using Godot;
using System;

public partial class DoorState : ParentState {
	private SceneManager sceneManager;
	public event Action sceneChange;
	private Tween colorChange;
	private bool doorOpened = false;

    public override void _EnterTree() {
		sceneManager = GetNode<SceneManager>("/root/SceneManager");
		singletonReference = GetNode<GlobalData>("/root/GlobalData");
        singletonReference.doorState = this;
    }

	public override void EnterState() {
		playerController.debugText.Text = "[center]State: Door[/center]";
		EnterDoorAnimation();
	}
	
	public override void PhysicsUpdate(float delta) {
		playerController.Velocity = Vector2.Zero;
		SetPhysicsProcess(false);
		SetProcessInput(false);
	}

	public void ExitDoorAnimation() {
		playerAnimations.Play("Door_Exit_Pull");
		if(playerAnimations.IsPlaying()) {
			colorChange = GetTree().CreateTween();
			colorChange.TweenProperty(playerAnimations, "modulate", new Color("#ffffffff"), animationLengthOf("Door_Exit_Pull")); //Color goes back to normal.
		} playerAnimations.AnimationFinished += () => finiteStateMachine.ChangeStateTo("IdleState"); 
		SetPhysicsProcess(true);
	}

	private void EnterDoorAnimation() {
		if(!doorOpened) {
			playerAnimations.Play("Door_Open");
			doorScript.openDoor(); //Executes only once.
			playerAnimations.Play("Door_Enter_Push");
			if(playerAnimations.IsPlaying()) {
				colorChange = GetTree().CreateTween();
				colorChange.TweenProperty(playerAnimations, "modulate", new Color("#2d1e2f"), animationLengthOf("Door_Enter_Push")); //Color changes to inside color.
			} doorOpened = true;
		} sceneManager.sceneTransition(doorScript.sceneLocation);
	}
}
