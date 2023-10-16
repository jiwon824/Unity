using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSprintingState : PlayerMovingState
{
    private PlayerSprintData sprintData;
    private float startTime;
    private bool keepSprinting;

    public PlayerSprintingState(PlayerMovementStateMachine _playerMovementStateMachine) : base(_playerMovementStateMachine)
    {
        sprintData = movementData.sprintData;
    }
    #region IState Methods
    public override void Enter()
    {
        base.Enter();
        stateMachine.reusableData.movementSpeedModifier = sprintData.speedModifier;
        startTime = Time.time;
    }
    public override void Update()
    {
        base.Update();
        if (keepSprinting) return;
        // this is the case, it means not enough time has passed for us to transition to the "Running State"
        if (Time.time < startTime + sprintData.sprintToRunTime)
        {
            return;
        }
        StopSprinting();
    }
    public override void Exit()
    {
        base.Exit();
        keepSprinting = false;
    }

    #endregion
    #region Main Methods
    private void StopSprinting()
    {
        if(stateMachine.reusableData.movementInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.idlingState);
            return;
        }
        stateMachine.ChangeState(stateMachine.runningState);
    }
    #endregion

    #region Reusable Methods
    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        stateMachine.player.input.playerActions.Sprint.performed += OnSprintPerformed;

    }


    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        stateMachine.player.input.playerActions.Sprint.performed -= OnSprintPerformed;
    }
    #endregion

    #region Input Methods
    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        keepSprinting = true;

    }
    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.hardStoppingState);
    }
    #endregion
}
