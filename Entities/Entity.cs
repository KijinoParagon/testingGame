using Godot;
using Customs;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;


public class ICharacterAction {
	protected Entity parentEntity;
	protected int LockoutLevel;
	protected double LockoutMaxDuration;
	protected double LockoutTimer;
	public string Name;
	public virtual void Start() {
		this.LockoutTimer = this.LockoutMaxDuration;
	}
	public virtual void End() {
	}
	public virtual int Update(double delta) {
		if (LockoutTimer <= 0) {
			return 0;
		} else {
			this.LockoutTimer -= delta;
			return this.LockoutLevel;
		}
	}
}

public class JumpAction : ICharacterAction {
	float jumpVerticalMomentum = 0;
	float jumpMaxVerticalMomentum = 9;
	public override void Start() {
		jumpVerticalMomentum = 0.0f;
	}
	public override int Update(double delta) {
		parentEntity.anims.PlayAnimation("Jump");
		jumpVerticalMomentum += (float)delta;
		return base.Update(delta);
	}
	public JumpAction(int LockoutLevel, string Name, double LockoutMaxDuration, Entity parentEntity) {
		this.LockoutLevel = LockoutLevel;
		this.Name = Name;
		this.LockoutMaxDuration = LockoutMaxDuration;
		this.parentEntity = parentEntity;
	}
}


public abstract class IActionController {
	public abstract void Update(double delta);
}
public class ActionController : IActionController {
	private ICharacterAction PrimaryActionSlot;
	private ICharacterAction PrimaryActionSlotQueue;
	private double PrimaryActionSlotQueueTimer;
	private ICharacterAction SecondaryActionSlot;
	private ICharacterAction SecondaryActionSlotQueue;
	private double SecondaryActionSlotQueueTimer;
	private CharacterBody3D parentEntity;
	public List<String> signals;
	public override void Update(double delta) {
		if (PrimaryActionSlotQueue != null) {
			PrimaryActionSlotQueueTimer -= delta;
			if (PrimaryActionSlotQueueTimer <= 0) {
				PrimaryActionSlot = PrimaryActionSlotQueue;
				PrimaryActionSlotQueue = null;
			}
		}
	}
	public ActionController(ICharacterAction defaultAction) {
		defaultAction.Start();
		this.PrimaryActionSlot = defaultAction;
		PrimaryActionSlotQueueTimer = 0;
		SecondaryActionSlotQueueTimer = 0;
	}
}
public class EntityPhysicsController {
	private float Friction = 3;
	private Entity parent;
	private Vector3 Momentum;
	private Vector3 Velocity;
	public void ApplyMomentum(Vector3 Momentum) {
		this.Momentum += Momentum;
	}
	public void Update(double delta) {
		Velocity = Momentum;
		float FrameFriction = Friction * (float)delta;
		//add friction/gravity here
		if (Momentum.X != 0.0f) {
			Momentum.X = (Momentum.X > FrameFriction) ? Momentum.X - FrameFriction : 0;
		}
		if (Momentum.Y != 0) {

		}
		if (Momentum.Z != 0.0f) {
			Momentum.Z = (Momentum.Z > FrameFriction) ? Momentum.Z - FrameFriction : 0;
		}
		GD.Print(Momentum);
		parent.Velocity = this.Velocity;
		parent.MoveAndSlide();

	}
	public EntityPhysicsController(Entity parent) {
		this.parent = parent;
	}
}
public class EntityAnimationController {
	private AnimationPlayer anims;
	public void PlayAnimation(string Name) {
		anims.Play("rig_002|" + Name);
	}
	public EntityAnimationController(AnimationPlayer anims) {
		this.anims = anims;
	}
}
public class EntitySignal {
	public double signalLifetime;
	public string Name;
	public Node Target;
	public Vector3 Momentum;

}
public class EntitySingalController {
	private List<EntitySignal> Signals;
	public void AddSignal(EntitySignal signal) {
		Signals.Add(signal);
	}
	public void Update(double delta) {

	}
	public EntitySingalController() {

	}
}
public partial class Entity : CharacterBody3D {
	private List<ICharacterAction> Actions = new List<ICharacterAction>();
	private ActionController actionController;
	private EntityPhysicsController phys;
	public EntityAnimationController anims;
	public override void _Ready() {
		Actions.Add(new JumpAction(0, "Jump", 0, this));
		anims = new EntityAnimationController((AnimationPlayer)GetNode("Model/AnimationPlayer"));
		phys = new EntityPhysicsController(this);
		actionController = new ActionController(Actions[0]);
	}
	public override void _Process(double delta) {
		if (Input.IsActionJustPressed("GWalk")) {
			phys.ApplyMomentum(new Vector3(6, 0, 0));
		}
		actionController.Update(delta);
		phys.Update(delta);
	}

}

/*
	Example Actions:
		Mount
			Follow mount
				-If current action is mounted, physics follows mount
		Grapple
			Move grapple to location
				-Physics
		Move
			Move character
				-
		RemoteFire
			Fire remote object
				-Spawn object at player's location

		
	Signal Handler
		//Potentially a target?
		//Spawn at player
		//Apply momentum to player
		//Cause animation
		//Signal despawn to hitbox
		//Needs an End() method
*/
