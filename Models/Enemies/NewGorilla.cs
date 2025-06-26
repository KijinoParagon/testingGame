using Godot;
using System;

public partial class NewGorilla : Node3D
{
    /// <summary>
	/// Action state determines what the character can do.
	/// </summary>
    private enum ActionStates
    {
        Grounded,
        Midair,
        Climbing,
        Staggered,
        Prone,
        Stunned
    }
    private ActionStates actionState;
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

    private FbxChara player;

    public void setPlayer(FbxChara p) {
        player = p;
    }
    public override void _Ready()
    {
        actionState = ActionStates.Midair;
    }

    public override void _Process(double delta)
    {
        if (player != null)
        {
            GD.Print(player.Position);
        }
        //Handle input
            HandleInput();
        //Handle signals
        //Handle Physics
        //      Should account for switching states depending on inputs
    }

    private void HandleInput()
    {
        if (actionState == ActionStates.Grounded)
        {
            if (Input.IsActionPressed("GWalkForward"))
            {
                //Handle walk. Walk if walk is pressed, regardless of start/stop.
                //You would want to mess with velocity here.
            }
            if (Input.IsActionJustPressed("GBackHop"))
            {
                //Have them jump. Does not trigger if they hold, only if they press.
            }
        }
        /*if (Input.IsActionJustPressed("GStrafeL"))
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
		}*/
    }
}
