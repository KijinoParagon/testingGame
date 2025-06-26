using Godot;
using System;

public partial class Battle : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		NewGorilla gor = GetNode<NewGorilla>("GorillaEnemy");
		FbxChara cha = GetNode<FbxChara>("FBX");
		gor.setPlayer(cha);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
