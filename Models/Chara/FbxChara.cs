using Godot;
using System;
using System.Collections.Generic;

public partial class FbxChara : Node3D
{
	enum States : int
	{
		AtkL1,
		AtkL2,
		Move,
		Idle,
		Guard
	}

	private States? stateBuffer;
	private float atkSpeed = 5f;
	private float AnimationTimer = 0f;
	private States state = States.Idle;
	private Vector3 velocity;
	private float velocityScale = 0.1f;
	private AnimationPlayer anims;
	private Node3D mesh;
	private Node3D rig;
	private Camera3D cam;

	[Export]
	public int RotationSpeed { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rig = (Node3D)GetNode("CamRig");
		mesh = (Node3D)GetNode("TestCharaAnims");
		cam = (Camera3D)GetNode("CamRig/Camera3D");
		foreach (Node n in this.GetChild(0).GetChildren())
		{
			if (n.GetType() == typeof(AnimationPlayer))
			{
				anims = (AnimationPlayer)n;
			}
		}
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustReleased("Forward"))
		{
			velocity.Z = 0;
		}
		if (Input.IsActionJustReleased("Back"))
		{
			velocity.Z = 0;
		}
		if (Input.IsActionJustReleased("Left"))
		{
			velocity.X = 0;
		}
		if (Input.IsActionJustReleased("Right"))
		{
			velocity.X = 0;
		}
		if (!Input.IsActionPressed("Forward") && !Input.IsActionPressed("Back") && !Input.IsActionPressed("Left") && !Input.IsActionPressed("Right") && AnimationTimer == 0)
		{
			stateBuffer = States.Idle;
		}
		if (state == States.Idle || state == States.Move)
		{
			anims.Play("rig|MidIdle");

			var actions = InputMap.GetActions();

			if (Input.IsActionJustPressed("Forward"))
			{
				velocity.Z += velocityScale;
				stateBuffer = States.Move;
			}
			if (Input.IsActionJustPressed("Back"))
			{
				velocity.Z -= velocityScale;
				stateBuffer = States.Move;
			}
			if (Input.IsActionJustPressed("Left"))
			{
				velocity.X += velocityScale;
				stateBuffer = States.Move;
			}
			if (Input.IsActionJustPressed("Right"))
			{
				velocity.X -= velocityScale;
				stateBuffer = States.Move;
			}
			if (Input.IsActionJustPressed("Attack"))
			{
				anims.Stop();
				anims.Play("rig|MidAtkL", customSpeed: atkSpeed);
				stateBuffer = States.AtkL1;
				velocity.X = 0;
				velocity.Z = 0;
				AnimationTimer = 0;
			}
		}


		if (state == States.AtkL1)
		{
			anims.Play("rig|MidAtkL", customSpeed: atkSpeed);
			if (Input.IsActionJustPressed("Attack"))
			{

				stateBuffer = States.AtkL2;
				velocity.X = 0;
				velocity.Z = 0;
				AnimationTimer = 0;
			}
			AnimationTimer += (float)delta;
			if (AnimationTimer >= 0.5833 / atkSpeed)
			{
				stateBuffer ??= States.Idle;
				anims.Stop();
				AnimationTimer = 0;
			}
		}
		if (state == States.AtkL2)
		{
			anims.Play("rig|MidAtkL2", customSpeed: atkSpeed);
			if (Input.IsActionJustPressed("Attack"))
			{
				stateBuffer = States.AtkL1;
				velocity.X = 0;
				velocity.Z = 0;
				AnimationTimer = 0;
			}
			AnimationTimer += (float)delta;
			if (AnimationTimer >= 0.5833 / atkSpeed)
			{
				stateBuffer ??= States.Idle;
				anims.Stop();
				AnimationTimer = 0;
			}

		}
		if (AnimationTimer == 0)
		{
			this.state = stateBuffer ?? States.Idle;
			stateBuffer = null;
		}
		Vector3 i = new Vector3( (float) (( Math.Sin(rig.Rotation.Y) * velocity.Z) + (Math.Cos(rig.Rotation.Y) * velocity.X)) , 
		 	velocity.Y, 
		 	(float) ((Math.Cos(rig.Rotation.Y) * velocity.Z) + (Math.Sin(rig.Rotation.Y) * velocity.X * -1)));
		this.Position += i;
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			mesh.RotateY(-0.01f * mouseMotion.Relative.X * RotationSpeed);
			rig.RotateY(-0.01f * mouseMotion.Relative.X * RotationSpeed);

			/*if (rig.Rotation.X > 60 && mouseMotion.Relative.Y > 0)
			{
				cam.Position.Slide(new Vector3(0, 0, -0.01f * mouseMotion.Relative.Y * RotationSpeed));
			}
			else if (rig.Rotation.X < 0 && mouseMotion.Relative.Y < 0)
			{
				cam.Position.Slide(new Vector3(0, 0, -0.01f * mouseMotion.Relative.Y * RotationSpeed));
			}
			else
			{
				//rig.RotateObjectLocal(new Vector3(-0.01f * mouseMotion.Relative.Y * RotationSpeed, 0, 0));
				rig.RotateObjectLocal(new Vector3(1, 0, 0), (-0.01f * mouseMotion.Relative.Y * RotationSpeed));
			}*/
		}
		
		if (@event.IsActionPressed("escape"))
		{
			ToggleMouseMode();
		}
	}

	private void ToggleMouseMode()
	{
		if (Input.MouseMode == Input.MouseModeEnum.Visible)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		else
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}
}
