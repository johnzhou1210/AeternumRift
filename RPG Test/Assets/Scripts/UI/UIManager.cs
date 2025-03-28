using System;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;
    
    #region UI References
    [SerializeField] private GameObject BattleUI, MapUI;
    #endregion

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
}
