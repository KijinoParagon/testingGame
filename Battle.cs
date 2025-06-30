using Godot;
using System;

public partial class Battle : Node3D
{
	Control optionMenu;
	FbxChara cha;
	NewGorilla gor;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.AddChild(ResourceLoader.Load<PackedScene>("res://Menus/OptionsMenu.tscn").Instantiate());
		optionMenu = this.GetChild<Control>(-1);
		optionMenu.Visible = false;

		gor = GetNode<NewGorilla>("GorillaEnemy");
		cha = GetNode<FbxChara>("FBX");

		gor.setPlayer(cha);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Menu"))
		{
			if (optionMenu.Visible)
			{
				optionMenu.Visible = false;
				cha.ToggleCameraMode();
			}
			else
			{
				optionMenu.Visible = true;
				cha.ToggleCameraMode();
			}
				
		}
	}
}
