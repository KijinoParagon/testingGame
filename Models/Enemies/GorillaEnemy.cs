using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class GorillaEnemy : Node3D
{
	/// <summary>
	/// Determines if the Gorilla is an NPC. Otherwise controlled by player.
	/// </summary>
	private enum States
	{
		Idle,
		WalkF,
		StrafeL,
		StrafeR,
		JumpBackStart,
		JumpBackMid,
		JumpBackEnd,
		AtkHigh1,
		AtkLowSweep,
		AtkOverhead,
		AtkToss,
		Stagger

	}

	/// <summary>
	/// Determines if the Gorilla is an NPC. Otherwise controlled by player.
	/// </summary>
	[Export]
	public bool npc;


	private string AnimPrefix = "rig_002|";
	private AnimationPlayer anims;
	private CharacterBody3D charBody;
	private States state;
	private Vector3 velocity;
	private double behaviorSwitchTime = 0;
	private double currentJumpTime = 0;
	public double walkSpeed = 2.5;
	public double strafeSpeed = 2.5;
	private Variant grav = ProjectSettings.GetSetting("physics/3d/default_gravity");

	[Export(PropertyHint.Range, "-5,5,0.1")]
	public double maxJumpTime { get; set; } = 1;
	[Export(PropertyHint.Range, "-5,5,0.1")]
	public double maxDownVelocity { get; set; } = -2;
	[Export(PropertyHint.Range, "-5,5,0.1")]
	public double jumpAcceleration { get; set; } = 0.2;

	[Export(PropertyHint.Range, "-5,5,0.1")]
	public double StrafeSpeed { get; set; } = 1;
	[Export(PropertyHint.Range, "-5,5,0.1")]
	public double WalkSpeed { get; set; } = 1;
	[Export(PropertyHint.Range, "-5,5,0.1")]
	public double Speed { get; set; } = 1;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		anims = (AnimationPlayer)GetNode("CharacterBody3D/Model/AnimationPlayer");
		charBody = (CharacterBody3D)GetNode("CharacterBody3D");
		state = States.Idle;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//First, handle npc behavior or player input
		if (npc)
		{
			NPCBehavior();
		}
		else
		{
			HandleInput();
		}


		if (state == States.JumpBackStart)
		{
			currentJumpTime += delta;
		}

		if (currentJumpTime > maxJumpTime && state == States.JumpBackStart)
		{
			state = States.JumpBackMid;
		}





		switch (state)
		{
			case States.Idle:
				break;
			case States.WalkF:
				//position.Z -= (float)(walkSpeed * delta * Speed);
				break;
			case States.StrafeL:
				//position.X += (float)(strafeSpeed * delta * Speed);
				break;
			case States.StrafeR:
				//position.X -= (float)(strafeSpeed * delta * Speed);
				break;
		}
		velocity.Y = -(float)((float)grav * delta);
		charBody.Velocity = velocity;
		charBody.MoveAndCollide(velocity);
		anims.Queue(AnimPrefix + state.ToString());
		anims.SpeedScale = (float)Speed;
		behaviorSwitchTime += delta;
	}

	private void HandleInput()
	{
		if (Input.IsActionJustPressed("GStrafeL"))
		{
			state = States.StrafeL;
			anims.Play(AnimPrefix + state.ToString(), 0.2, (float)Speed);
		}

		if (Input.IsActionJustPressed("GStrafeR"))
		{
			state = States.StrafeR;
			anims.Play(AnimPrefix + state.ToString(), 0.2, (float)Speed);
		}

		if (Input.IsActionJustReleased("GStrafeL"))
		{
			state = States.Idle;
			anims.Play(AnimPrefix + state.ToString(), 0.2, (float)Speed);
		}

		if (Input.IsActionJustReleased("GStrafeR"))
		{
			state = States.Idle;
			anims.Play(AnimPrefix + state.ToString(), 0.2, (float)Speed);
		}

		if (Input.IsActionJustPressed("GWalk"))
		{
			state = States.WalkF;
			anims.Play(AnimPrefix + state.ToString(), 0.2, (float)Speed);
		}
		if (Input.IsActionJustReleased("GWalk"))
		{
			state = States.Idle;
			anims.Play(AnimPrefix + state.ToString(), 0.2, (float)Speed);
		}
		if (Input.IsActionJustPressed("GBackHop"))
		{
			state = States.JumpBackStart;
			currentJumpTime = 0;
			velocity.Y += (float)jumpAcceleration;
			anims.Play(AnimPrefix + state.ToString(), 0.2, (float)Speed);
		}
		if (Input.IsActionJustReleased("GBackHop") && state == States.JumpBackStart)
		{
			state = States.JumpBackMid;
			anims.Play(AnimPrefix + state.ToString(), 0.2, (float)Speed);
		}

	}


	/// <summary>
	/// Handles behavior for if this character is an NPC.
	/// </summary>
	private void NPCBehavior()
	{
		if (behaviorSwitchTime >= 2)
		{
			anims.ClearQueue();
			switch (state)
			{
				case States.Idle:
					state = States.WalkF;
					break;
				case States.WalkF:
					state = States.StrafeL;
					break;
				case States.StrafeL:
					state = States.StrafeR;
					break;
				case States.StrafeR:
					state = States.Idle;
					break;
			}
			behaviorSwitchTime = 0;
			anims.Play(AnimPrefix + state.ToString(), 0.2, (float)Speed);
		}
	}
}
