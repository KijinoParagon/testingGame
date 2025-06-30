using Godot;
using System;
using Customs;
using System.Reflection;

public partial class NewGorilla : CharacterBody3D
{
	/// <summary>
	/// Action state determines what the character can do.
	/// </summary>
	private enum ActionStates
	{
		Grounded,
		GStrafeL,
		GStrafeR,
		Idle,
		Midair,
		Climbing,
		Staggered,
		Prone,
		Stunned
	}
	private ActionStates actionState;
	private bool AnimationLocked = false;
	/// <summary>
	/// Struct for buffering actions.
	/// </summary>
	private struct ActionBuffer
	{
		public ActionStates? bufferedAction;
		public double queueTimer;
		public ActionBuffer()
		{
			this.bufferedAction = null;
			this.queueTimer = 0;
		}
	}
	private ActionBuffer actionBuffer = new ActionBuffer();
	/// <summary>
	/// Stores the animation data for the character.
	/// </summary>
	private enum Animations
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
		AtkToss
	}
	private string AnimPrefix = "rig_002|";
	private AnimationPlayer anims;

	private FbxChara player;
	private Vector3 momentum;
	private Vector3 movement;

	[Export(PropertyHint.Range, "0,10,0.1")]
	public double MaxRotationSpeed = 5;
	[Export(PropertyHint.Range, "-10,10,0.1")]
	public float movementSpeed = 5;
	[Export(PropertyHint.Range, "0,5,0.1")]
	public double ActionBufferTime = 1;

	public void setPlayer(FbxChara p)
	{
		player = p;
	}
	public override void _Ready()
	{
		actionState = ActionStates.Midair;
		momentum = new Vector3(0, -1.5f, 0);
		anims = (AnimationPlayer)GetNode("Model/AnimationPlayer");
		AnimationMixer.AnimationFinishedEventHandler AnimEndEvent = new AnimationMixer.AnimationFinishedEventHandler((Godot.StringName name) => { actionState = actionBuffer.bufferedAction ?? ActionStates.Idle; actionBuffer.bufferedAction = null; });
		anims.AnimationFinished += AnimEndEvent;
	}

	public override void _Process(double delta)
	{
		movement = new Vector3(0, 0, 0);
		//Handle input
		HandleInput(delta);
		//Handle signals
		//Handle Physics
		//      Should account for switching states depending on inputs
		if (actionState == ActionStates.GStrafeL)
		{
			movement.X += movementSpeed * (float) delta * 100;
		}
		if (actionState == ActionStates.GStrafeR)
		{
			movement.X -= movementSpeed * (float) delta * 100;
		}
		movement = movement.Rotated(new Vector3(0, 1, 0), Rotation.Y);
		Velocity = momentum + movement * new Vector3(movementSpeed, movementSpeed, movementSpeed);
		this.MoveAndSlide();
		for (int i = 0; i < this.GetSlideCollisionCount(); i++)
		{
			if (((StaticBody3D)this.GetSlideCollision(i).GetCollider()).GetCollisionLayerValue(1))
			{
				BufferAction(ActionStates.Idle);
			}
		}

		//Face the character if you can
		if (actionState != ActionStates.Staggered && actionState != ActionStates.Stunned && actionState != ActionStates.Prone)
		{
			this.RotateY((float)CustAng.GetShortestAngle(this, player, MaxRotationSpeed, delta));
		}
		GD.Print(actionState);
	}
	private void HandleInput(double delta)
	{
		if (actionBuffer.bufferedAction != null)
		{
			actionBuffer.queueTimer -= delta;
			if (actionBuffer.queueTimer <= 0)
			{
				actionBuffer.bufferedAction = null;
			}
		}

		if (Input.IsActionPressed("GStrafeL"))
		{
			BufferAction(ActionStates.GStrafeL);
		}
		if (Input.IsActionPressed("GStrafeR"))
		{
			BufferAction(ActionStates.GStrafeR);
		}

		if (actionState == ActionStates.Midair)
		{
			if (actionBuffer.bufferedAction == ActionStates.Idle)
			{
				actionState = ActionStates.Idle;
				actionBuffer.bufferedAction = null;
				anims.Play(AnimPrefix + Animations.Idle);
			}
		}

		if (actionState == ActionStates.Idle)
		{
			if (actionBuffer.bufferedAction == ActionStates.GStrafeL)
			{
				actionState = ActionStates.GStrafeL;
				anims.Play(AnimPrefix + Animations.StrafeL, -1, movementSpeed);
				actionBuffer.bufferedAction = null;
				return;
			}
			else if (actionBuffer.bufferedAction == ActionStates.GStrafeR)
			{
				actionState = ActionStates.GStrafeR;
				anims.Play(AnimPrefix + Animations.StrafeR, -1, movementSpeed);
				actionBuffer.bufferedAction = null;
				return;
			}
			else if (Input.IsActionPressed("GWalk"))
			{
				//Handle walk. Walk if walk is pressed, regardless of start/stop.
				//You would want to mess with velocity here.
				movement.Z -= 100 * movementSpeed * (float)delta;
				anims.Play(AnimPrefix + Animations.WalkF);
			}
			else if (Input.IsActionPressed("GWalkBack"))
			{
				//Handle walk. Walk if walk is pressed, regardless of start/stop.
				//You would want to mess with velocity here.
				movement.Z += 100 * movementSpeed * (float)delta;
				anims.Play(AnimPrefix + Animations.WalkF);
			}
			else
			{
				anims.Play(AnimPrefix + Animations.Idle, 0.2);
			}
		}
	}
	/// <summary>
	/// Buffers an action for the character.
	/// </summary>
	private void BufferAction(ActionStates action)
	{
		actionBuffer.bufferedAction = action;
		actionBuffer.queueTimer = ActionBufferTime;
	}
}
