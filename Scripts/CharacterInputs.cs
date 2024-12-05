using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputs : MonoBehaviour {

    [Header("Input-Action Names")]
    [SerializeField] InputActionAsset characterControls;
    [SerializeField] private string actionMap = "Player";
    [SerializeField] private string move = "Movement";
    [SerializeField] private string run = "Run";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string crouch = "Crouch";
    [SerializeField] private string attack = "Attack";
    [SerializeField] private string doorEnter = "Door Enter";

    [Header("Input-Actions References")]
    private InputAction movingAction;
    private InputAction runningAction;
    private InputAction jumpingAction;
    private InputAction crouchingAction;
    private InputAction attackingAction;

    //Input-Action Property Types (Header here does not work for some reason).
    public Vector2 moveButton {get; private set;}
    public float runButton {get; private set;}
    public bool jumpButton {get; private set;}
    public bool attackButton {get; private set;}
    public bool crouchButton {get; private set;}

    public static CharacterInputs InputsInstance {get; private set;}

    private void Awake() {
        if(InputsInstance == null) { //Ensures that we only have one file, no duplicates
            InputsInstance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        movingAction = characterControls.FindActionMap(actionMap).FindAction(move);
        runningAction = characterControls.FindActionMap(actionMap).FindAction(run);
        jumpingAction = characterControls.FindActionMap(actionMap).FindAction(jump);
        crouchingAction = characterControls.FindActionMap(actionMap).FindAction(crouch);
        attackingAction = characterControls.FindActionMap(actionMap).FindAction(attack);
        RegisterActions();
    }

    private void RegisterActions() {
        movingAction.performed += context => moveButton = context.ReadValue<Vector2>();
        movingAction.canceled += context => moveButton = Vector2.zero;
        runningAction.performed += context => runButton = context.ReadValue<float>();
        runningAction.canceled += context => runButton = 0.0f;
        jumpingAction.performed += context => jumpButton = true;
        jumpingAction.canceled += context => jumpButton = false;
        crouchingAction.performed += context => jumpButton = true;
        crouchingAction.canceled += context => jumpButton = false;
        attackingAction.performed += context => jumpButton = true;
        attackingAction.canceled += context => jumpButton = false;
    }
    
    private void Enable() {
        movingAction.Enable();
        runningAction.Enable();
        jumpingAction.Enable();
        crouchingAction.Enable();
        attackingAction.Enable();
    }
    
    private void Disable() {
        movingAction.Disable();
        runningAction.Disable();
        jumpingAction.Disable();
        crouchingAction.Disable();
        attackingAction.Disable();
    }
}