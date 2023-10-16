using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementState : IState
{
    protected PlayerMovementStateMachine stateMachine;

    protected PlayerGroundedData movementData;

    public PlayerMovementState (PlayerMovementStateMachine _playerMovementStateMachine)
    {
        stateMachine = _playerMovementStateMachine;
        movementData = stateMachine.player.data.groundedData;
        InitializeData();
    }

    private void InitializeData()
    {
        SetBaseRotationData();
    }

    #region IState
    public virtual void Enter()
    {
        Debug.Log($"State: {GetType().Name}");
        AddInputActionsCallbacks();
    }

    public virtual void Exit()
    {
        RemoveInputActionsCallbacks();
    }
    
    public virtual void HandleInput()
    {
        ReadMovementInput();
    }


    public virtual void Update()
    {
    }

    public virtual void PhysicsUpdate()
    {
        Move();
    }

    public virtual void OnAnimationEnterEvent()
    {

    }

    public virtual void OnAnimationExitEvent()
    {

    }

    public virtual void OnAnimationTransitionEvent()
    {

    }

    #endregion

    #region Main Methods
    private void ReadMovementInput()
    {
        stateMachine.reusableData.movementInput = stateMachine.player.input.playerActions.Movement.ReadValue<Vector2>();
    }

    private void Move()
    {
        if (stateMachine.reusableData.movementInput == Vector2.zero || stateMachine.reusableData.movementSpeedModifier == 0f) return;
        Vector3 movementDirection = GetMovementInputDirection();
        float targetRotationYAngle = Rotate(movementDirection);
        Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);

        float movementSpeed = GetMovemetSpeed();

        Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();
        stateMachine.player.rb.AddForce(targetRotationDirection  * movementSpeed - currentPlayerHorizontalVelocity, ForceMode.VelocityChange);
    }

    private float Rotate(Vector3 direction)
    {
        float directionAngle = UpdateTargetRotation(direction);
        RotateTowardsTargetRotation();
        return directionAngle;
    }

    private void UpdateTargetRotationData(float targetAngle)
    {
        stateMachine.reusableData.CurrentTargetRotation.y = targetAngle;
        //reset the passed time
        stateMachine.reusableData.DampedTargetRotationPassedTime.y = 0f;

    }

    private float AddCameraRotationToAngle(float angle)
    {
        // If you add the direction you are moving and the angle of the camera, you will move a few degrees around the camera.
        // we need to use "eulerAngles" and not "rotation", as "rotation" returns a Quaternion.
        // The "y" axis of a Camera  is the horizontal rotation.
        angle += stateMachine.player.mainCameraTransform.eulerAngles.y;
        if (angle > 360f)
        {
            angle -= 360f;
        }

        return angle;
    }

    private static float GetDirectionAngle(Vector3 direction)
    {
        // player is moving towards the sum of our "Input" Direction plus the Rotation of the Camera.
        float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        // Atan2 returns Radian and it ranges from -180 to 180
        if (directionAngle < 0f)
        {
            directionAngle += 360f;
        }

        return directionAngle;
    }

    #endregion


    #region Reusable Methods
    protected virtual void AddInputActionsCallbacks()
    {
        stateMachine.player.input.playerActions.WalkToggle.started += OnWalkToggleStarted; 
    }
    protected virtual void RemoveInputActionsCallbacks()
    {
        stateMachine.player.input.playerActions.WalkToggle.started -= OnWalkToggleStarted;
    }


    protected Vector3 GetMovementInputDirection()
    {
        return new Vector3(stateMachine.reusableData.movementInput.x, 0f, stateMachine.reusableData.movementInput.y);
    }

    protected float GetMovemetSpeed()
    {
        // * stateMachine.reusableData.movementSpeedModifier
        return movementData.baseSpeed * stateMachine.reusableData.movementSpeedModifier * stateMachine.reusableData.movementOnSlopeSpeedModifier;
    }
    protected Vector3 GetPlayerHorizontalVelocity()
    {
        Vector3 playerHorizontalVelocity = stateMachine.player.rb.velocity;
        playerHorizontalVelocity.y = 0f;
        return playerHorizontalVelocity;
    }
    protected Vector3 GetPlayerVerticalVelocity()
    {
        return new Vector3(0f, stateMachine.player.rb.velocity.y, 0f);
    }
    protected Vector3 GetTargetRotationDirection(float targetAngle)
    {
        // We pass in "Vector3.forward" here because  we always want a rotation from the "z" axis,
        return Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
    }

    protected float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation=true)
    {
        float directionAngle = GetDirectionAngle(direction);
        if (shouldConsiderCameraRotation)
        {
            directionAngle = AddCameraRotationToAngle(directionAngle);
        }


        if (directionAngle != stateMachine.reusableData.CurrentTargetRotation.y)
        {
            UpdateTargetRotationData(directionAngle);
        }

        return directionAngle;
    }
    protected void RotateTowardsTargetRotation()
    {
        float currentYAngle = stateMachine.player.rb.rotation.eulerAngles.y;
        if (currentYAngle == stateMachine.reusableData.CurrentTargetRotation.y) return;
        float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, stateMachine.reusableData.CurrentTargetRotation.y,
            ref stateMachine.reusableData.DampedTargetRotationCurrentVelocity.y, stateMachine.reusableData.TimeToReachTargetRotation.y - stateMachine.reusableData.DampedTargetRotationPassedTime.y);
        stateMachine.reusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;

        // playerRotation
        Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);

        stateMachine.player.rb.MoveRotation(targetRotation);
    }

    protected void ResetVelocity()
    {
        stateMachine.player.rb.velocity = Vector3.zero;
    }

    protected void DecelerateHorizontally()
    {
        // we only need to Decelerate  our Player in the Horizontal Axis.
        Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();
        stateMachine.player.rb.AddForce(-playerHorizontalVelocity * stateMachine.reusableData.movementDecelerationForce, ForceMode.Acceleration);
    }
    protected bool IsMovingHorizontally(float minimumMagnitude = 0.1f)
    {
        Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();
        Vector2 playerHorizontalMovement = new Vector2(playerHorizontalVelocity.x, playerHorizontalVelocity.z);
        return playerHorizontalMovement.magnitude < minimumMagnitude;
    }

    protected void SetBaseRotationData()
    {
        // The reason why we've made this a new method is because we need to make sure we set our rotation data back when we "Exit" the "Dashing State", as otherwise our rotation would  keep on being "0.02" seconds.
        stateMachine.reusableData.rotationData = movementData.baseRotationData;
        stateMachine.reusableData.TimeToReachTargetRotation = stateMachine.reusableData.rotationData.targetRotationReachTime;
    }
    #endregion

    #region Input Methods
    protected virtual void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        stateMachine.reusableData.shouldWalk = !stateMachine.reusableData.shouldWalk;
    }

    
    #endregion

}