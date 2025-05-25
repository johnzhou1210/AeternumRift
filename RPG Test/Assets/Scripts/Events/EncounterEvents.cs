using System;
using UnityEngine;

public class EncounterEvents {
    public static event Action OnIncrementStep, OnResetEncounterSteps;

    public static void InvokeOnIncrementStep() {
        OnIncrementStep?.Invoke();
    }

    public static void InvokeOnResetEncounterSteps() {
        OnResetEncounterSteps?.Invoke();
    }
    
}
