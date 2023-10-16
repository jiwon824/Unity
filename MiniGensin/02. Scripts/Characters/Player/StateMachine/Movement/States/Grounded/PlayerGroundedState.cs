using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundedState : PlayerMovementState
{
    private SlopData slopData;
    public PlayerGroundedState(PlayerMovementStateMachine _playerMovementStateMachine) : base(_playerMovementStateMachine)
    {
        slopData = stateMachine.player.colliderUtility.slopData;
    }
    #region IState Methods
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        FloatCapsule();
    }

    #endregion

    #region Main Methods
    private void FloatCapsule()
    {
        // which returns the center in world space
        Vector3 capsuleColliderCenterInWorldSpace = stateMachine.player.colliderUtility.capsuleColliderData.collider.bounds.center;

        // Vector3.down is always ve in world space, while "-transform.up" will be relative to the transform rotation.
        Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

        // We could pass in its layer number or string but that isn't a great way of doing it.
        // create a new C# Script named "PlayerLayerData" to hold this data.
        // QueryTriggerInteraction.Ignore: which ignores objects that have the "Environment" Layer but are Trigger Colliders, as we don't want to consider triggers a "Ground" we walk on. 
        if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, slopData.floatRayDistance,
            stateMachine.player.layerData.groundLayer, QueryTriggerInteraction.Ignore))
        {
            float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

            float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groundAngle);

            // This makes it so we don't float on Grounds that have an angle that's too high
            // , which makes it so that we can't walk on them and will fall or slide instead.
            if (slopeSpeedModifier == 0)
            {
                return;
            }
            //This is of course the distance from the center of the capsule collider to the ground.
            float distanceToFloatingPoint = stateMachine.player.colliderUtility.capsuleColliderData.colliderCenterInLocalSpace.y
                                            * stateMachine.player.transform.localScale.y - hit.distance;
            if (distanceToFloatingPoint == 0f) return;
            float amountToLift = distanceToFloatingPoint *slopData.stepReachForce - GetPlayerVerticalVelocity().y;
            // need to convert this float force into a Vector3 force
            Vector3 liftForce = new Vector3(0f, amountToLift, 0f);

            // this one is a vertical force while the other one is an horiazontal force
            stateMachine.player.rb.AddForce(liftForce, ForceMode.VelocityChange);
        }

    }

    private float SetSlopeSpeedModifierOnAngle(float angle)
    {
        //PlayerSO Animation Curve SlopeSpeedAngle
        float slopeSpeedModifier = movementData.slopeSpeedAngles.Evaluate(angle);
        // now setting our slope speed modifier using an Animation Curve 
        stateMachine.reusableData.movementOnSlopeSpeedModifier = slopeSpeedModifier;

        return slopeSpeedModifier;

    }

    #endregion
    #region Reusable Methods
    protected virtual void OnMove()
    {
        if (stateMachine.reusableData.shouldWalk)
        {
            stateMachine.ChangeState(stateMachine.walkingState);
            return;
        }
        stateMachine.ChangeState(stateMachine.runningState);

    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        stateMachine.player.input.playerActions.Movement.canceled += OnMovementCanceled;

        stateMachine.player.input.playerActions.Dash.started += OnDashStarted;
    }


    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        stateMachine.player.input.playerActions.Movement.canceled -= OnMovementCanceled;
        stateMachine.player.input.playerActions.Dash.started -= OnDashStarted;

    }
    #endregion
    #region Input Methods
    
    protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.idlingState);
    }
    protected virtual void OnDashStarted(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.dashingState);
    }
     
    #endregion
}
