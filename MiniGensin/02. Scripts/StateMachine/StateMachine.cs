using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine
{
    protected IState currentState;

    public void ChangeState(IState newState)
    {
        // null-conditional operator
        // same to if (currentState != null) currentState.Exit();
        currentState?.Exit();

        currentState = newState;
        currentState.Enter();
    }
     
    public void HandleInput()
    {
        currentState?.HandleInput();
    }
    public void Update()
    {
        currentState?.Update();
    }
    public void PhysicsUpdate()
    {
        currentState?.PhysicsUpdate();
    }
    public void OnAnimationEnterEvent()
    {
        currentState?.OnAnimationEnterEvent();
    }
    public void OnAnimationExitEvent()
    {
        currentState?.OnAnimationEnterEvent();
    }
    public void OnAnimationTransitionEvent()
    {
        currentState?.OnAnimationEnterEvent();
    }
}
