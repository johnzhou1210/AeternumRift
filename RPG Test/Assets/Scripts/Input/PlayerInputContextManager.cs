using System;
using UnityEngine;

public class PlayerInputContextManager : MonoBehaviour {
    [SerializeField] private PlayerDungeonInputManager dungeonInput;
    [SerializeField] private PlayerUIInputManager uiInput;
    [SerializeField] private PlayerBattleUIInputManager battleInput;
    
    
    public IInputManager currentInput;

    private void Start() {
        SwitchToDungeon();
    }

    public void SwitchToDungeon() {
        SetInputContext(dungeonInput);
    }

    public void SwitchToUI() {
        SetInputContext(uiInput);
    }

    public void SwitchToBattle() {
        SetInputContext(battleInput);
    }

    private void SetInputContext(IInputManager mgr) {
        currentInput?.Disable();
        currentInput = mgr;
        currentInput?.Enable();
    }
    
}
