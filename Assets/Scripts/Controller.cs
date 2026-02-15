using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour, PlayerController.IPlayerMapActions
{
    public PlayerController PlayerController {  get; private set; }
    public event Action action;
    public Vector2 inputMoveDirection;
    public Vector2 inputRotationDirection;

    private void OnEnable()
    {
        PlayerController = new PlayerController();
        PlayerController.PlayerMap.SetCallbacks(this);
        PlayerController.Enable();
    }

    private void OnDisable()
    {
        PlayerController.Disable();
    }

    public void Action (InputAction.CallbackContext context)
    {
        action?.Invoke();
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        inputMoveDirection = context.ReadValue<Vector2>();
    }
    public void OnRotate(InputAction.CallbackContext context)
    {
        inputRotationDirection = context.ReadValue<Vector2>();
    }
}
