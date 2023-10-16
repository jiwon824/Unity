using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunningState : PlayerMovingState
{
    private PlayerSprintData sprintData;
    private float startTime;
    public PlayerRunningState(PlayerMovementStateMachine _playerMovementStateMachine) : base(_playerMovementStateMachine)
    {
        sprintData = movementData.sprintData;
    }
    #region IState Methods
    public override void Enter()
    {
        base.Enter();
        stateMachine.reusableData.movementSpeedModifier = movementData.runData.speedModifier;

        startTime = Time.time;
    }
    public override void Update()
    {
        base.Update();
        if (!stateMachine.reusableData.shouldWalk)
        {
            return;
        }
        if(Time.time < sprintData.runToWalkTime)
        {
            return;
        }
        StopRunning();
    }

    #endregion

    #region Main Methods
    private void StopRunning()
    {
        if(stateMachine.reusableData.movementInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.idlingState);
            return;
        }
        stateMachine.ChangeState(stateMachine.walkingState);
    }
    #endregion

    #region Input Methods
    protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        base.OnWalkToggleStarted(context);
        stateMachine.ChangeState(stateMachine.walkingState);
    }
    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.mediumStoppingState);
    }
    #endregion
}
