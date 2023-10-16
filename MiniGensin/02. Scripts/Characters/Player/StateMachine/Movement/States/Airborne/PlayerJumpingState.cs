using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerAirborneState
{
    public PlayerJumpingState(PlayerMovementStateMachine _playerMovementStateMachine) : base(_playerMovementStateMachine)
    {
    }

    #region IState Methods
    public override void Enter()
    {
        base.Enter();
        stateMachine.reusableData.movementSpeedModifier = 0f;
        Jump();
    }

    #endregion

    #region Main Methods
    private void Jump()
    {
        // The reason why we're adding a temporary variable here is because the jump force will need to be changed depending on the slope angle as well as the Jump Direction. 
        Vector3 jumpForce = stateMachine.reusableData.currentJumpForce;

    }
    #endregion
}
