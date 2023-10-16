using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdlingState : PlayerGroundedState
{
    public PlayerIdlingState(PlayerMovementStateMachine _playerMovementStateMachine) : base(_playerMovementStateMachine)
    {
    }
    #region Istate Methods
    public override void Enter()
    {
        base.Enter();
        stateMachine.reusableData.movementSpeedModifier = 0f;
        ResetVelocity();
    }
    public override void Update()
    {
        base.Update();
        if(stateMachine.reusableData.movementInput == Vector2.zero)
        {
            return;
        }
        OnMove();
    }

    
    #endregion
}
