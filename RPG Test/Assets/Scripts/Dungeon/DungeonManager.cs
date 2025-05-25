using System;
using UnityEngine;

public class DungeonManager : MonoBehaviour {
    public static DungeonManager Instance;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        EncounterEvents.InvokeOnResetEncounterSteps();
    }


}
