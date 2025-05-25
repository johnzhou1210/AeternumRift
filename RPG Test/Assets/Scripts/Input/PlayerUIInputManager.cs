using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIInputManager : MonoBehaviour, IInputManager {
    public bool Active { get; set; } = false;

    public void Enable() {
        Active = true;
    }
    public void Disable() {
        Active = false;
    }
}
