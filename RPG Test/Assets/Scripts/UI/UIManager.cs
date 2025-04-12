using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;
    
    #region UI References
    [SerializeField] private List<GameObject> UIElements;
    #endregion

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void SetActiveUI(GameObject uiObj, bool activeVal, bool allowStacking = false) {
        if (!allowStacking) {
            foreach (GameObject obj in UIElements) {
                obj.SetActive(false);
            }
        }
        uiObj.SetActive(activeVal);
    }
    
}
