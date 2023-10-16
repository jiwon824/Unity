using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMediumStoppingState : PlayerStoppingState
{
    public PlayerMediumStoppingState(PlayerMovementStateMachine _playerMovementStateMachine) : base(_playerMovementStateMachine)
    {
    }
    #region IState Methods
    public override void Enter()
    {
        base.Enter();
        stateMachine.reusableData.movementDecelerationForce = movementData.stopData.mediumDecelerationForce;
    }
    #endregion
}
