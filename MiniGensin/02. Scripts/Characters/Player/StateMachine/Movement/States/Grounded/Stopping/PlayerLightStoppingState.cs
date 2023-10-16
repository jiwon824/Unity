using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLightStoppingState : PlayerStoppingState
{
    public PlayerLightStoppingState(PlayerMovementStateMachine _playerMovementStateMachine) : base(_playerMovementStateMachine)
    {
    }

    #region IState Methods
    public override void Enter()
    {
        base.Enter();
        stateMachine.reusableData.movementDecelerationForce = movementData.stopData.lightDecelerationForce;
    }
    #endregion
}
