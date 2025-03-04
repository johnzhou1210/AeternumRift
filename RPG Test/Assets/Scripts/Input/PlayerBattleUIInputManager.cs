using System;
using System.Collections.Generic;
using DG.Tweening;
using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public enum WheelAction {
    Attack,
    Skill,
    Item,
    Defend,
    Flee
}

public class PlayerBattleUIInputManager : MonoBehaviour {
    public WheelAction currentWheelAction { get; private set; } = 0;
    [SerializeField, Self] private PlayerInput playerInput;
    [SerializeField] private float navigateCooldown = .25f;

    [Header("Wheel UI Objects")] 
    [SerializeField] private GameObject actionWheelContainer;
    [SerializeField] private RectTransform rotatableWheel;
    [SerializeField] private GameObject activeActionHighlight;
    [SerializeField] private GameObject wheelPointerContainer;
    [SerializeField] private TextMeshProUGUI activeWheelActionText;
    
    [SerializeField] private GameObject wheelAttackActive, wheelAttackInactive;
    [SerializeField] private GameObject wheelSkillActive, wheelSkillInactive;
    [SerializeField] private GameObject wheelItemActive, wheelItemInactive;
    [SerializeField] private GameObject wheelDefendActive, wheelDefendInactive;
    [SerializeField] private GameObject wheelFleeActive, wheelFleeInactive;
    
    

    private List<WheelAction> wheelActions = new List<WheelAction> {
        WheelAction.Attack,
        WheelAction.Skill,
        WheelAction.Item,
        WheelAction.Defend,
        WheelAction.Flee
    };

    private InputAction submitAction;
    private InputAction navigateAction;


    private float navigateTimer = 0f;


    private void OnValidate() { this.ValidateRefs(); }


    private void Awake() {
        submitAction = playerInput.actions["Submit"];
        navigateAction = playerInput.actions["Navigate"];
        playerInput.SwitchCurrentActionMap("UI");
    }

    private void Start() { submitAction.performed += OnSubmit; }


    private void OnSubmit(InputAction.CallbackContext context) {
        if (context.performed) {
            Debug.Log("Submit action triggered!");
        }
    }

    private void OnDisable() { submitAction.performed -= OnSubmit; }

    private void Update() {
        if (navigateTimer < navigateCooldown) {
            navigateTimer += Time.deltaTime;
        }

        if (navigateAction.IsPressed()) {
            OnNavigate();
        } else if (navigateAction.WasReleasedThisFrame()) {
            navigateTimer = navigateCooldown;
        }
    }

    private void OnNavigate() {
        if (navigateTimer < navigateCooldown) return;
        navigateTimer = 0f;

        if (navigateAction.ReadValue<Vector2>().y > 0) {
            Debug.Log("Navigate up action triggered!");
            currentWheelAction = wheelActions[(int)currentWheelAction + 1 == wheelActions.Count ? 0 : (int)currentWheelAction + 1];
            Debug.Log(72f * (int)currentWheelAction);
            
            rotatableWheel.DOLocalRotate(new Vector3(0, 0, 72f * (int)currentWheelAction), .175f, RotateMode.Fast).SetEase(Ease.OutBack).OnComplete(OnRotateWheelComplete).OnStart(OnRotateWheelStart);
        } else if (navigateAction.ReadValue<Vector2>().y < 0) {
            Debug.Log("Navigate down action triggered!");
            currentWheelAction = wheelActions[(int)currentWheelAction - 1 == -1 ? wheelActions.Count - 1 : (int)currentWheelAction - 1];
            rotatableWheel.DOLocalRotate(new Vector3(0, 0, 72f * (int)currentWheelAction), .175f, RotateMode.Fast).SetEase(Ease.OutBack).OnComplete(OnRotateWheelComplete).OnStart(OnRotateWheelStart);
        } else if (navigateAction.ReadValue<Vector2>().x > 0) {
            Debug.Log("Navigate left action triggered!");
        } else if (navigateAction.ReadValue<Vector2>().x < 0) {
            Debug.Log("Navigate right action triggered!");
        }
    }

    private void OnRotateWheelStart() {
        activeActionHighlight.SetActive(false);
        SetAllWheelItemsInactive();
    }
    
    private void OnRotateWheelComplete() {
        activeActionHighlight.SetActive(true);
        switch ((int)currentWheelAction) {
            case 0:
                activeWheelActionText.text = "ATTACK";
                wheelAttackActive.SetActive(true);
            break;
            case 1:
                activeWheelActionText.text = "SKILL";
                wheelSkillActive.SetActive(true);
            break;
            case 2:
                activeWheelActionText.text = "ITEM";
                wheelItemActive.SetActive(true);
            break;
            case 3:
                activeWheelActionText.text = "DEFEND";
                wheelDefendActive.SetActive(true);
            break;
            case 4:
                activeWheelActionText.text = "FLEE";
                wheelFleeActive.SetActive(true);
            break;
        }
    }

    private void SetAllWheelItemsInactive() {
        wheelAttackActive.SetActive(false); wheelAttackInactive.SetActive(true);
        wheelSkillActive.SetActive(false); wheelSkillInactive.SetActive(true);
        wheelItemActive.SetActive(false); wheelItemInactive.SetActive(true);
        wheelDefendActive.SetActive(false); wheelDefendInactive.SetActive(true);
        wheelFleeActive.SetActive(false); wheelFleeInactive.SetActive(true);
    }
    
}