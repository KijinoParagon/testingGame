using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class StartButton : Button
{
	PackedScene Battle = ResourceLoader.Load<PackedScene>("res://Battle.tscn");
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Pressed += ClickStart;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void ClickStart()
	{
		GetTree().ChangeSceneToPacked(Battle);
	}
}
