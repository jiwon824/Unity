using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateReusableData
{
    // 값을 설정할 수 있어야 하기 때문에 public set
    public Vector2 movementInput { get; set; }
    public float movementSpeedModifier { get; set; } = 1f;
    public float movementOnSlopeSpeedModifier { get; set; } = 1f;
    public float movementDecelerationForce { get; set; } = 1f;
    public bool shouldWalk { get; set; }

    /*
        Whenever we have a Vector3 property, we  cannot set its variables from another class.
        So creating a Vector3 here wouldn't allow us to change its "x", "y" and "z" variables from our States.
        This only happens when it is a property, because properties return a copy of the private variable they create when its Type is of "Value Type", which means changing its "x", "y", or "z"  wouldn't do anything in the original Vector3.
     */
    private Vector3 currentTargetRotation;
    private Vector3 timeToReachTargetRotation;
    private Vector3 dampedTargetRotationCurrentVelocity;
    private Vector3 dampedTargetRotationPassedTime;

    /*
     We could fix this by making it a  public variable instead of a property,
    but we'll actually fix it by making it  so that the property returns a reference of our private variable.
    This means that we can change its  variables as it's not a copy anymore
     */
    public ref Vector3 CurrentTargetRotation
    {
        get
        {
            return ref currentTargetRotation;
        }
    }

    public ref Vector3 TimeToReachTargetRotation
    {
        get
        {
            return ref timeToReachTargetRotation;
        }
    }

    public ref Vector3 DampedTargetRotationCurrentVelocity
    {
        get
        {
            return ref dampedTargetRotationCurrentVelocity;
        }
    }

    public ref Vector3 DampedTargetRotationPassedTime
    {
        get
        {
            return ref dampedTargetRotationPassedTime;
        }
    }

    public PlayerRotationData rotationData { get; set; }
    public Vector3 currentJumpForce { get; set; }
}
