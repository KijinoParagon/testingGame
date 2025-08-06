using Godot;
using System;
using Customs;
using System.Reflection;
/*
public partial class NewGorilla : CharacterBody3D {
	[Export(PropertyHint.Range, "0,10,0.1")]
	public double MaxRotationSpeed = 5;
	[Export(PropertyHint.Range, "-10,10,0.1")]
	public float movementSpeed = 5;
	[Export(PropertyHint.Range, "-50,50,0.1")]
	public float jumpVelocity = 5;
	[Export(PropertyHint.Range, "0,5,0.1")]
	public double ActionBufferTime = 1;
	/// <summary>
	/// Action state determines what the character can do.
	/// </summary>
	private enum ActionStates {
		Grounded,
		GStrafeL,
		GStrafeR,
		Idle,
		Midair,
		Climbing,
		Staggered,
		Prone,
		Stunned,
		JumpBack
	}

	private bool animationLocked = false;
	/// <summary>
	/// Struct for buffering actions.
	/// </summary>
	private struct ActionBuffer {
		private double queueTimer = 0;
		private double currentActionTimer = 0;
		private double actionBufferTime = 0;
		private bool actionLocked = false;
		public ActionStates? bufferedAction = null;
		private ActionStates actionState = ActionStates.Idle;

		void BufferAction(ActionStates action) {
			this.bufferedAction = action;
			this.queueTimer = this.actionBufferTime;
		}
		ActionStates GetAction() {
			return this.actionState;
		}
		ActionStates UpdateAction(double delta) {
			if (this.bufferedAction != null) {
				this.queueTimer -= delta;
				if (this.queueTimer <= 0) {
					this.bufferedAction = null;
				}
			}
			if (this.actionState == ActionStates.Idle) {
				if (bufferedAction != null) {
					actionState = bufferedAction ?? actionState;
					switch (actionState) {
						case ActionStates.GStrafeL:
							anims.Play(animPrefix + Animations.StrafeL, -1, movementSpeed);
							break;
						case ActionStates.GStrafeR:
							anims.Play(animPrefix + Animations.StrafeR, -1, movementSpeed);
							break;
						case ActionStates.JumpBack:
							anims.Play(animPrefix + Animations.JumpBackStart);
							this.momentum.Y += jumpVelocity;
							break;
					}
					bufferedAction = null;
					return this.actionState;
				} else if (Input.IsActionPressed("GWalk")) {
					//Handle walk. Walk if walk is pressed, regardless of start/stop.
					//You would want to mess with velocity here.
					movement.Z -= 100 * movementSpeed * (float)delta;
					anims.Play(animPrefix + Animations.WalkF);
				} else if (Input.IsActionPressed("GWalkBack")) {
					//Handle walk. Walk if walk is pressed, regardless of start/stop.
					//You would want to mess with velocity here.
					movement.Z += 100 * movementSpeed * (float)delta;
					anims.Play(animPrefix + Animations.WalkF);
				} else {
					anims.Play(animPrefix + Animations.Idle, 0.2);
				}
			}
			return this.actionState;
		}
		public ActionBuffer(double time) {
			this.actionBufferTime = time;
		}
	}
	private ActionBuffer actionBuffer;
	/// <summary>
	/// Stores the animation data for the character.
	/// </summary>
	private enum Animations {
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
		BackWalk
	}
	private string animPrefix = "rig_002|";
	private AnimationPlayer anims;

	private FbxChara player;
	private Vector3 momentum;
	private Vector3 movement;

	public void setPlayer(FbxChara p) {
		player = p;
	}
	public override void _Ready() {
		actionBuffer = new ActionBuffer(this.ActionBufferTime);
		momentum = new Vector3(0, -1.5f, 0);
		anims = (AnimationPlayer)GetNode("Model/AnimationPlayer");
		AnimationMixer.AnimationFinishedEventHandler AnimEndEvent = new AnimationMixer.AnimationFinishedEventHandler((Godot.StringName name) => { actionState = actionBuffer.bufferedAction ?? ActionStates.Idle; actionBuffer.bufferedAction = null; });
		anims.AnimationFinished += AnimEndEvent;
	}

	public override void _Process(double delta) {
		movement = new Vector3(0, 0, 0);
		//Handle input
		HandleInput(delta);
		//Handle signals
		//Handle Physics
		//      Should account for switching states depending on inputs
		if (actionState == ActionStates.GStrafeL) {
			movement.X += movementSpeed * (float)delta * 100;
		}
		if (actionState == ActionStates.GStrafeR) {
			movement.X -= movementSpeed * (float)delta * 100;
		}
		movement = movement.Rotated(new Vector3(0, 1, 0), Rotation.Y);
		Velocity = momentum + movement * new Vector3(movementSpeed, movementSpeed, movementSpeed);
		this.MoveAndSlide();
		for (int i = 0; i < this.GetSlideCollisionCount(); i++) {
			if (((StaticBody3D)this.GetSlideCollision(i).GetCollider()).GetCollisionLayerValue(1)) {
				BufferAction(ActionStates.Idle);
			}
		}

		//Face the character if you can
		if (actionState != ActionStates.Staggered && actionState != ActionStates.Stunned && actionState != ActionStates.Prone) {
			this.RotateY((float)CustAng.GetShortestAngle(this, player, MaxRotationSpeed, delta));
		}
		GD.Print(actionState);
	}
	private void HandleInput(double delta) {

		if (!animationLocked) {
			if (Input.IsActionPressed("GStrafeL")) {
				BufferAction(ActionStates.GStrafeL);
			}
			if (Input.IsActionPressed("GStrafeR")) {
				BufferAction(ActionStates.GStrafeR);
			}
			if (Input.IsActionJustPressed("JumpBack")) {
				BufferAction(ActionStates.JumpBack);
			}
			if (Input.IsActionJustPressed("Roll")) {
				BufferAction(ActionStates.JumpBack);
			}


			if (actionState == ActionStates.Midair) {
				if (actionBuffer.bufferedAction == ActionStates.Idle) {
					actionState = ActionStates.Idle;
					actionBuffer.bufferedAction = null;
					anims.Play(animPrefix + Animations.Idle);
				}
			}

			if (actionState == ActionStates.Idle) {
				if (actionBuffer.bufferedAction != null) {
					actionState = actionBuffer.bufferedAction ?? actionState;
					switch (actionState) {
						case ActionStates.GStrafeL:
							anims.Play(animPrefix + Animations.StrafeL, -1, movementSpeed);
							break;
						case ActionStates.GStrafeR:
							anims.Play(animPrefix + Animations.StrafeR, -1, movementSpeed);
							break;
						case ActionStates.JumpBack:
							anims.Play(animPrefix + Animations.JumpBackStart);
							this.momentum.Y += jumpVelocity;
							break;
					}
					actionBuffer.bufferedAction = null;
					return;
				} else if (Input.IsActionPressed("GWalk")) {
					//Handle walk. Walk if walk is pressed, regardless of start/stop.
					//You would want to mess with velocity here.
					movement.Z -= 100 * movementSpeed * (float)delta;
					anims.Play(animPrefix + Animations.WalkF);
				} else if (Input.IsActionPressed("GWalkBack")) {
					//Handle walk. Walk if walk is pressed, regardless of start/stop.
					//You would want to mess with velocity here.
					movement.Z += 100 * movementSpeed * (float)delta;
					anims.Play(animPrefix + Animations.WalkF);
				} else {
					anims.Play(animPrefix + Animations.Idle, 0.2);
				}
			}
		}
	}
}*/
