using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class EncounterManager : MonoBehaviour {
    private int _stepsUntilEncounter;
    [SerializeField] public Vector2Int MinMaxStepsForEncounter { get; private set; } = new(15, 30);

    private int _currentEncounterTotalRequiredSteps;


    private void OnEnable() {
        EncounterEvents.OnIncrementStep += IncrementStep;
        EncounterEvents.OnResetEncounterSteps += ResetEncounterSteps;

        EncounterFuncs.GetStepsUntilEncounter = () => _stepsUntilEncounter;
        EncounterFuncs.GetCurrentEncounterTotalRequiredSteps = () => _currentEncounterTotalRequiredSteps;
    }

    private void OnDisable() {
        EncounterEvents.OnIncrementStep -= IncrementStep;
        EncounterEvents.OnResetEncounterSteps -= ResetEncounterSteps;

        EncounterFuncs.GetStepsUntilEncounter = null;
        EncounterFuncs.GetCurrentEncounterTotalRequiredSteps = null;
    }

    public void ResetEncounterSteps() {
        _stepsUntilEncounter = Random.Range(MinMaxStepsForEncounter.x, MinMaxStepsForEncounter.y + 1);
        _currentEncounterTotalRequiredSteps = _stepsUntilEncounter;
    }

    public void StartBattle() {
        GameStateManager.Instance.SetGameState(GameState.BATTLE);
        Debug.LogWarning("Battle starts!");
        LensDistortionEffect.Instance.DoEncounterEffect();
        
        StartCoroutine(BattleStartTiming());
    }

    private IEnumerator BattleStartTiming() {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlayMusic(Resources.Load<AudioClip>("Audio/MUSIC/Strife"));
        UIManager.Instance.SetActiveUI(0, true, true);
    }

    public void IncrementStep() {
        _stepsUntilEncounter -= 1;
        if (_stepsUntilEncounter <= 0) {
            StartBattle();
            Invoke(nameof(EndBattleTest), 9f);
        }
    }

    private void EndBattleTest() {
        GameStateManager.Instance.SetGameState(GameState.ABYSS);
        ResetEncounterSteps();
        AudioManager.Instance.StopMusic();
        SceneUtility.Instance.UnloadScene("BattleScene");
        UIManager.Instance.SetActiveUI(0, false, true);
    }
    
    

    
}