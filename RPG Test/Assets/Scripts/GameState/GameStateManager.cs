using System;
using UnityEngine;

public enum GameState {
    MAIN_MENU,
    IN_GAME_MENU,
    TOWN,
    ABYSS,
    BATTLE,
    CUTSCENE,
}

public class GameStateManager : MonoBehaviour {
    public static GameStateManager Instance;
    
    public GameState CurrentGameState { get; private set; }
    [SerializeField] private PlayerInputContextManager playerInputContextManager;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        SetGameState(GameState.ABYSS);
    }

    public void SetGameState(GameState gameState) {
        CurrentGameState = gameState;
        switch (CurrentGameState) {
            case GameState.ABYSS:
                playerInputContextManager.SwitchToDungeon();
            break;
            case GameState.IN_GAME_MENU:
                playerInputContextManager.SwitchToUI();
                break;
            case GameState.TOWN:
                playerInputContextManager.SwitchToUI();
            break;
            case GameState.BATTLE:
                playerInputContextManager.SwitchToBattle();
                break;
            case GameState.CUTSCENE:
                playerInputContextManager.SwitchToUI();
                break;
            default:
                break;
        }
    }
    
}
