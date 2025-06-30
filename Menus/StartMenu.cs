using Godot;
using System;

public partial class StartMenu : Control
{
	PackedScene optionsMenuScene;
	Control optionMenu;
	Button Options;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Options = this.GetNode<Button>("Options");
		optionsMenuScene = ResourceLoader.Load<PackedScene>("res://Menus/OptionsMenu.tscn");
		this.AddChild(optionsMenuScene.Instantiate());
		optionMenu = this.GetChild<Control>(-1);
		optionMenu.Visible = false;
		Options.Pressed += OptionsClick;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void OptionsClick()
	{
		if (optionMenu.Visible)
			optionMenu.Visible = false;
		else
			optionMenu.Visible = true;
	}
}
