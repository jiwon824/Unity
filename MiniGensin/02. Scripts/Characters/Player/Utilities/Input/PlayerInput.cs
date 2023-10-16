using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public PlayerInputActions inputActions { get; private set; }
    public PlayerInputActions.PlayerActions playerActions { get; private set; }

    void Awake()
    {
        inputActions = new PlayerInputActions();
        playerActions = inputActions.Player;
    }
    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }
    public void DisableActionFor(InputAction action, float seconds)
    {
        StartCoroutine(DisableActoin(action, seconds));
    }

    private IEnumerator DisableActoin(InputAction action, float seconds)
    {
        action.Disable();
        yield return new WaitForSeconds(seconds);
        action.Enable();
    }
}
