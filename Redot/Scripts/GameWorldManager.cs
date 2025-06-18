using Godot;
using System;

public partial class GameWorldManager : Node {
	private Node2D sceneCandyland, sceneSubwayStation, sceneSeaport;
	public CanvasLayer cameraPanUI, playerUI;
	public CharacterBody2D sidMarshall;
	private Camera2D playerCamera;

	public override void _Ready() {
		//Initializing Scenes
		sceneSubwayStation = GetNode<Node2D>("Areas/SubwayStation");
		sceneCandyland = GetNode<Node2D>("Areas/Candyland");
		sceneSeaport = GetNode<Node2D>("Areas/Seaport");
		
		//Initializing UI Nodes
		playerCamera = GetNode<Camera2D>("SidMarshall/Camera");
		cameraPanUI = GetNode<CanvasLayer>("UIElements/CameraUI");
		playerUI = GetNode<CanvasLayer>("UIElements/PlayerUI");
	}
}
