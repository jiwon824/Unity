using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkingState : PlayerMovingState
{
    public PlayerWalkingState(PlayerMovementStateMachine _playerMovementStateMachine) : base(_playerMovementStateMachine)
    {
    }
    #region IState Methods
    public override void Enter()
    {
        base.Enter();
        stateMachine.reusableData.movementSpeedModifier = movementData.walkData.speedModifier;
    }

    #endregion

    

    #region Input Methods
    protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        base.OnWalkToggleStarted(context);
        stateMachine.ChangeState(stateMachine.runningState);
    }
    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.lightStoppingState);
    }

    #endregion
}
