using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class EncounterManager : MonoBehaviour {
    public static EncounterManager Instance;
    public int StepsUntilEncounter { get; private set; }
    [SerializeField] public Vector2Int MinMaxStepsForEncounter { get; private set; } = new(15, 30);

    public int CurrentEncounterTotalRequiredSteps { get; private set; }
    
    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    public void ResetEncounterSteps() {
        StepsUntilEncounter = Random.Range(MinMaxStepsForEncounter.x, MinMaxStepsForEncounter.y + 1);
        CurrentEncounterTotalRequiredSteps = StepsUntilEncounter;
    }
    
    public void StartBattle() {
        GameStateManager.Instance.SetGameState(GameState.BATTLE);
        Debug.LogWarning("Battle starts!");
        AudioManager.Instance.PlayMusic(Resources.Load<AudioClip>("Audio/MUSIC/Strife"));
        SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);
    }

    public void IncrementStep() {
        StepsUntilEncounter -= 1;
        if (StepsUntilEncounter <= 0) StartBattle();
    }
    
    private void EndBattleTest() {
        GameStateManager.Instance.SetGameState(GameState.ABYSS);
        ResetEncounterSteps();
        AudioManager.Instance.StopMusic();
        SceneUtility.Instance.UnloadScene("BattleScene");
    }

    
}
