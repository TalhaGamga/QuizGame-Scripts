using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "FirstPlayerInputHandler", menuName = "Scriptable Objects/InputHandler/FirstPlayerInputHandler")]
public class FirstPlayerInputHandler : PlayerInputHandlerBase
{
    private FirstPlayerControls playerControls;
    public override Action OnAnswerInput { get; set; }
    public override Action OnShootInput { get; set; }
    public override Action<int> OnTrackSelections { get; set; }

    public override void Init()
    {
        playerControls = new FirstPlayerControls();
        playerControls?.Enable();
    }

    public override void AddActionCallbacks()
    {
        playerControls.Player.Answer.performed += answer;
        playerControls.Player.Shoot.performed += shoot;
        playerControls.Player.TrackSelections.performed += trackSelections;
    }

    public override void RemoveActionCallbacks()
    {
        playerControls.Player.Answer.performed -= answer;
        playerControls.Player.Shoot.performed -= shoot;
        playerControls.Player.TrackSelections.performed -= trackSelections;

        playerControls?.Disable();
    }

    private void answer(InputAction.CallbackContext context)
    {
        OnAnswerInput?.Invoke();
    }

    private void shoot(InputAction.CallbackContext context)
    {
        OnShootInput?.Invoke();
    }

    private void trackSelections(InputAction.CallbackContext context)
    {
        int value = (int)context.ReadValue<float>();

        OnTrackSelections(value);
    }
}
