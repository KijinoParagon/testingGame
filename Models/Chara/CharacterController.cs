using Godot;
using System;

public partial class CharacterController : Node3D
{
	private AnimationPlayer anims;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		foreach(Node n in this.GetChildren()) {
			if (n.GetType() == typeof(AnimationPlayer)){
				anims = (AnimationPlayer) n;
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta){
		anims.Play("MidIdle");
	}
}
