using UnityEngine;

public interface IInputManager {
    bool Active { get; set; }
    void Enable();
    void Disable();
}
