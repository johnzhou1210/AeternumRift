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
    }
    
}
