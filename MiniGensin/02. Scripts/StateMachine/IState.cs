using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interfaces are not implemented.
// If you want to implement it, use an abstract method.
public interface IState
{
    public void Enter();
    public void Exit();

    // 입력 읽기와 관련된 모든 로직을 실행할 수 있게 하는 함수
    public void HandleInput();
    public void Update(); // same as Update
    public void PhysicsUpdate(); // same as FixedUpdate()
    public void OnAnimationEnterEvent();
    public void OnAnimationExitEvent();
    public void OnAnimationTransitionEvent();
}
