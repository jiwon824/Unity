using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementStateMachine : StateMachine
{
    public Player player { get; }
    public PlayerStateReusableData reusableData { get; }

    public PlayerIdlingState idlingState { get; }
    public PlayerWalkingState walkingState { get; }
    public PlayerRunningState runningState { get; }
    public PlayerDashingState dashingState { get; }
    public PlayerSprintingState sprintingState { get; }

    public PlayerLightStoppingState lightStoppingState { get; }
    public PlayerMediumStoppingState mediumStoppingState { get; }
    public PlayerHardStoppingState hardStoppingState { get; }

    public PlayerJumpingState jumpingState { get; }

    public PlayerMovementStateMachine(Player _player)
    {
        player = _player;
        reusableData = new PlayerStateReusableData();

        idlingState = new PlayerIdlingState(this);
        walkingState = new PlayerWalkingState(this);
        runningState = new PlayerRunningState(this);
        dashingState = new PlayerDashingState(this);
        sprintingState = new PlayerSprintingState(this);

        lightStoppingState = new PlayerLightStoppingState(this);
        mediumStoppingState = new PlayerMediumStoppingState(this);
        hardStoppingState = new PlayerHardStoppingState(this);
        jumpingState = new PlayerJumpingState(this);
    }
}
