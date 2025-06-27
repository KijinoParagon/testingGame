using Godot;
using System;

namespace Customs;

public static class CustAng
{
	/// <summary>
	/// Returns the shortest angle from the RotationObject to the Target in radians. 
	/// Can be given a MaxRotationSpeed and delta to clamp the angle for non immediate turning.
	/// </summary>
	public static double GetShortestAngle(Node3D RotationObject, Node3D Target, double? MaxRotationSpeed = null, double delta = 1)
	{
		//Our position vs player position
		double angle = Math.Atan2(-(Target.Position.X - RotationObject.Position.X), Target.Position.Z - RotationObject.Position.Z);
		angle = ((angle + Math.PI) + (RotationObject.Rotation.Y + Math.PI)) % (Math.PI * 2);
		if (angle > Math.PI)
		{
			angle = (angle % Math.PI) - Math.PI;
		}
		if (MaxRotationSpeed != null)
		{
			angle = Math.Clamp(angle, -1 * (double)MaxRotationSpeed * delta, (double)MaxRotationSpeed * delta);
		}
		return angle * -1;
	}
}