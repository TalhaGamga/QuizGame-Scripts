using System;
using UnityEngine;

public abstract class PlayerInputHandlerBase : ScriptableObject, IPlayerInputHandler
{
    public abstract Action OnAnswerInput { get; set; }
    public abstract Action OnShootInput { get; set; }
    public abstract Action<int> OnTrackSelections { get; set; }
    public abstract void Init();
    public abstract void AddActionCallbacks();
    public abstract void RemoveActionCallbacks();
}
