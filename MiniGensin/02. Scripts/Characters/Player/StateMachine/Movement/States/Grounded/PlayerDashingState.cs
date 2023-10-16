using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDashingState : PlayerGroundedState
{
    private PlayerDashData dashData;

    private float startTime;
    private int consecutiveDashesUsed; // count how many consecutive dashes we've done
    private bool shouldKeepRotating;
    
    public PlayerDashingState(PlayerMovementStateMachine _playerMovementStateMachine) : base(_playerMovementStateMachine)
    {
        dashData = movementData.dashData;
    }

    #region IState Methods
    public override void Enter()
    { 
        base.Enter();
        // This does it for when we're  transitioning from a "Moving State" to dashingState
        stateMachine.reusableData.movementSpeedModifier = dashData.speedModifier;

        stateMachine.reusableData.rotationData = dashData.rotationData;
        // so for the from a "Stopping State"(to dashingState) case
        AddForceOnTransitionFromStationaryState();

        // This will become true if we  are pressing any "Movement" Key or false if we're pressing none.
        shouldKeepRotating = stateMachine.reusableData.movementInput != Vector2.zero;

        UpdateConsecutiveDashes();
        startTime = Time.time;
    }

    public override void Exit()
    {
        base.Exit();
        SetBaseRotationData();
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (!shouldKeepRotating) return;
        RotateTowardsTargetRotation();
    }

    public override void OnAnimationTransitionEvent()
    {
        if(stateMachine.reusableData.movementInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.hardStoppingState); 
            return;
        }
            stateMachine.ChangeState(stateMachine.sprintingState);
    }
    #endregion

    #region Main Methods
    private void UpdateConsecutiveDashes()
    {
        // we'll start by checking if the Dash  we're in is a consecutive dash
        if (!IsConsecutive())
        {
            // reset our dash count if  it's not a consecutive dash.
            consecutiveDashesUsed = 0;
        }
        ++consecutiveDashesUsed;

        // We now need to check if our used dashes count  is equal to our dash limit amount
        if(consecutiveDashesUsed == dashData.consecutiveDashesLimitAmount)
        {
            consecutiveDashesUsed = 0;
            // also need to disable our DashInput for a few seconds
            stateMachine.player.input.DisableActionFor(stateMachine.player.input.playerActions.Dash, dashData.dashLimitReachedCooldown); 
        }
    }

    private bool IsConsecutive()
    {
        // So, if we entered the current "Dashing State"  not too long after our previous "Dashing State",
        // we'll consider the current  "Dash" a "Consecutive Dash".
        // Time.time: current game time
        return Time.time<startTime+dashData.timeToBeConsideredConsecutive;
    }

    private void AddForceOnTransitionFromStationaryState()
    {
        // we only want to add a force  if there was no Input by the time we got into the "Dashing State"
        if (stateMachine.reusableData.movementInput != Vector2.zero)
        {
            return;
        }
        Vector3 characterRotationDirection = stateMachine.player.transform.forward;
        // we only need our horizontal rotation direction
        characterRotationDirection.y =  0f;

        // For the direction, pass in  "characterRotationDirection".
        // For the second argument, we'll pass in "false" as we don't want to consider the camera rotation(but only the Player "Forward Direction"  or our "Movement Input Direction").
        UpdateTargetRotation(characterRotationDirection, false);

        // We'll set the force through rigidbody.velocity because we want to be able to move during the dash
        // this means that it's possible that our "Move  AddForce" will be called before "Dashing".
        stateMachine.player.rb.velocity = characterRotationDirection * GetMovemetSpeed();
    }
    #endregion

    #region Reusable Methods
    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        stateMachine.player.input.playerActions.Movement.performed += OnMovementPerformed;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        stateMachine.player.input.playerActions.Movement.performed -= OnMovementPerformed;
    }
    #endregion

    #region Input Methods
    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {

    }
    protected override void OnDashStarted(InputAction.CallbackContext context)
    {

    }
    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        shouldKeepRotating = true;
    }

    #endregion
}
