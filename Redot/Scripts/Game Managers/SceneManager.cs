using Godot;
using System;

public partial class SceneManager : Node {
	private Node2D sceneCandyland, sceneSubwayStation, sceneSeaport;
	
	public override void _Ready() {
		//Initializing Scenes
		sceneSubwayStation = GetNode<Node2D>("/root/GameWorld/Areas/SubwayStation");
		sceneCandyland = GetNode<Node2D>("/root/GameWorld/Areas/Candyland");
		sceneSeaport = GetNode<Node2D>("/root/GameWorld/Areas/Seaport");
	}

	public override void _Process(double delta) {
	}
}
