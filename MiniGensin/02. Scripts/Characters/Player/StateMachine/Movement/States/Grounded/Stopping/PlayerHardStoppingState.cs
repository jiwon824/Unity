using UnityEngine.InputSystem;

public class PlayerHardStoppingState : PlayerStoppingState
{
    public PlayerHardStoppingState(PlayerMovementStateMachine _playerMovementStateMachine) : base(_playerMovementStateMachine)
    {
    }
    #region IState Methods
    public override void Enter()
    {
        base.Enter();
        stateMachine.reusableData.movementDecelerationForce = movementData.stopData.hardDecelerationForce;
    }
    #endregion

    #region Reusable Methods
    protected override void OnMove()
    {
        if (stateMachine.reusableData.shouldWalk)
        {
            return;
        }
        stateMachine.ChangeState(stateMachine.runningState);
    }
    #endregion
}
