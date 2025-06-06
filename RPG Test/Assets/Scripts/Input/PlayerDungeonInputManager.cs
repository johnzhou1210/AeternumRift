using System;
using System.Collections;
using DG.Tweening;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerDungeonInputManager : MonoBehaviour, IInputManager {
    public bool Active { get; set; } = false;
    [SerializeField, Self] private PlayerInput playerInput;
    [SerializeField] private float movementCooldown = .5f;

    private InputAction wasdAction;
    private InputAction strafeLeftAction, strafeRightAction;
    private InputAction mapZoomInAction, mapZoomOutAction, panMapAction, toggleMapViewAction, openMenuAction;

    public Transform PlayerTransform { get; private set; }

    public static event Action<Vector2Int> OnUpdatePlayerMarkerPosition;
    public static event Action<int> OnUpdatePlayerMarkerRotation;

    [SerializeField, Scene] private RenderUIMap uiMapRender;
    [SerializeField] private float uiMapPanSpeed = 10f, uiMapZoomSpeed = .5f;
    [SerializeField] private GameObject fullMapContainer, minimapContainer, fullMap, fullMapGrid;

    private float movementTimer = 0f, fullMapGridLastSize = 1f;
    private Vector3 minimapLastPosition, fullMapGridLastPosition = Vector3.zero;
    
    
    private void OnValidate() {
        this.ValidateRefs();
    }

    private void Awake() {
        wasdAction = playerInput.actions["Move"];
        strafeLeftAction = playerInput.actions["StrafeLeft"];
        strafeRightAction = playerInput.actions["StrafeRight"];
        mapZoomInAction = playerInput.actions["ZoomIn"];
        mapZoomOutAction = playerInput.actions["ZoomOut"];
        panMapAction = playerInput.actions["PanMap"];
        toggleMapViewAction = playerInput.actions["ToggleMapView"];
    }

    private void Start() {
        StartCoroutine(FindPlayerObj());
        
    }

    void OnEnable() {
        StartCoroutine(FindPlayerObj());

        PlayerInputEvents.OnSetMinimapView += SetMinimapView;

    }
    
    void OnDisable() {
        PlayerInputEvents.OnSetMinimapView -= SetMinimapView;
        
        PlayerInputFuncs.GetPlayerTransform = null;
    }

    private void SetMinimapView(bool val) {
        if (val) {
            SendBackToMinimap();
        } else {
            ShowFullMap();
        }
    }
    

    private IEnumerator FindPlayerObj() {
        yield return new WaitUntil(() => {
            print(GameObject.FindGameObjectWithTag("Player"));
            return GameObject.FindGameObjectWithTag("Player") != null;
        });
        PlayerTransform = GameObject.FindWithTag("Player").transform;
        PlayerInputFuncs.GetPlayerTransform = () => PlayerTransform;
    }
    

    private DungeonGrid GetGrid() {
        return MapManager.Instance.CurrentMap;
    }


    private void OnWASDAction() {
        if (PlayerTransform == null) return;
        if (movementTimer < movementCooldown) return;
        movementTimer = 0f;
        if (wasdAction.ReadValue<Vector2>().x < 0) {
            RotateLeft();
        } else if (wasdAction.ReadValue<Vector2>().x > 0) {
            RotateRight();
        } else if (wasdAction.ReadValue<Vector2>().y < 0) {
            MoveBackward();
        } else if (wasdAction.ReadValue<Vector2>().y > 0) {
            MoveForward();
        }
    }

    private void OnStrafeLeftAction() {
        if (PlayerTransform == null) return;
        if (movementTimer < movementCooldown) return;
        movementTimer = 0f;
        StrafeLeft();
    }

    private void OnStrafeRightAction() {
        if (PlayerTransform == null) return;
        if (movementTimer < movementCooldown) return;
        movementTimer = 0f;
        StrafeRight();
    }

    private void RotateLeft() {
        if (PlayerTransform == null) return;
        PlayerTransform.DOLocalRotate(new Vector3(0, PlayerTransform.rotation.eulerAngles.y - 90, 0), movementCooldown, RotateMode.Fast).OnComplete(OnRotateComplete);
    }

    private void RotateRight() {
        if (PlayerTransform == null) return;
        PlayerTransform.DOLocalRotate(new Vector3(0, PlayerTransform.rotation.eulerAngles.y + 90, 0), movementCooldown, RotateMode.Fast).OnComplete(OnRotateComplete);
    }

    private void MoveForward() {
        Vector3 positionOfCellInFront = PlayerTransform.localPosition + PlayerTransform.forward;
        Vector2Int positionOfCellInFront2D = new (Mathf.RoundToInt(positionOfCellInFront.x), Mathf.RoundToInt(positionOfCellInFront.z));
        Cell cellInFront = GetGrid().GetCellByXY(positionOfCellInFront2D);
        if (cellInFront.Terrain == TerrainType.Floor) {
            FootstepSound();
            PlayerTransform.DOLocalMove(positionOfCellInFront, movementCooldown).OnComplete(OnMoveComplete);    
            // print("Move success! " + positionOfCellInFront2D + " is a " + cellInFront.Terrain + " cell!");
        } else {
            // print("Cannot move forward! " + positionOfCellInFront2D + " is a "+ cellInFront.Terrain +" cell!");
        }
        
    }

    private void MoveBackward() {
        Vector3 positionOfCellBehind = PlayerTransform.localPosition - PlayerTransform.forward;
        Vector2Int positionOfCellBehind2D = new (Mathf.RoundToInt(positionOfCellBehind.x), Mathf.RoundToInt(positionOfCellBehind.z));
        Cell cellBehind = GetGrid().GetCellByXY(positionOfCellBehind2D);
        if (cellBehind.Terrain == TerrainType.Floor) {
            FootstepSound();
            PlayerTransform.DOLocalMove(positionOfCellBehind, movementCooldown).OnComplete(OnMoveComplete);    
            // print("Move success! " + positionOfCellBehind2D + " is a " + cellBehind.Terrain + " cell!");
        } else {
            // print("Cannot move backward! " + positionOfCellBehind2D + " is a " +cellBehind.Terrain+" cell!");
        }
        
    }

    private void StrafeLeft() {
        Vector3 positionOfCellToLeft = PlayerTransform.localPosition - PlayerTransform.right;
        Vector2Int positionOfCellToLeft2D = new (Mathf.RoundToInt(positionOfCellToLeft.x), Mathf.RoundToInt(positionOfCellToLeft.z));
        Cell cellToLeft = GetGrid().GetCellByXY(positionOfCellToLeft2D);
        if (cellToLeft.Terrain == TerrainType.Floor) {
            FootstepSound();
            PlayerTransform.DOLocalMove(positionOfCellToLeft, movementCooldown).OnComplete(OnMoveComplete);
            // print("Move success! " + positionOfCellToLeft2D + " is a " + cellToLeft.Terrain + " cell!");
        } else {
            // print("Cannot strafe left! " + positionOfCellToLeft2D + " is a " +cellToLeft.Terrain+" cell!");
        }
    }

    private void StrafeRight() {
        Vector3 positionOfCellToRight = PlayerTransform.localPosition + PlayerTransform.right;
        Vector2Int positionOfCellToRight2D = new (Mathf.RoundToInt(positionOfCellToRight.x), Mathf.RoundToInt(positionOfCellToRight.z));
        Cell cellToRight = GetGrid().GetCellByXY(positionOfCellToRight2D);
        if (cellToRight.Terrain == TerrainType.Floor) {
            FootstepSound();
            PlayerTransform.DOLocalMove(positionOfCellToRight, movementCooldown).OnComplete(OnMoveComplete);    
            // print("Move success! " + positionOfCellToRight2D + " is a " + cellToRight.Terrain + " cell!");
        } else {
            // print("Cannot strafe right !" + positionOfCellToRight2D + " is a " +cellToRight.Terrain+" cell!");
        }
    }

    private void OnMoveComplete() {
        if (OnUpdatePlayerMarkerPosition != null)
            OnUpdatePlayerMarkerPosition.Invoke(new(Mathf.RoundToInt(PlayerTransform.localPosition.x), Mathf.RoundToInt(PlayerTransform.localPosition.z)));
        
        EncounterEvents.InvokeOnIncrementStep();
        int stepsUntilEncounter = EncounterFuncs.GetStepsUntilEncounter?.Invoke() ?? -1;
        int currentEncounterTotalRequiredSteps = EncounterFuncs.GetCurrentEncounterTotalRequiredSteps?.Invoke() ?? -1;
        
        Image minimapImage = minimapContainer.GetComponent<Image>();
        if (minimapImage == null) return;
        if (stepsUntilEncounter >= currentEncounterTotalRequiredSteps * .85f) {
            minimapImage.DOColor(new(0 / 255f, 0 / 255f, 60 / 255f, 183 / 255f), 1f);
            Debug.LogWarning("blue");
        } else if (stepsUntilEncounter >= currentEncounterTotalRequiredSteps * .7f) {
            minimapImage.DOColor(new(0/255f, 60/255f, 60/255f, 183/255f), 1f);
            Debug.LogWarning("teal");
        } else if (stepsUntilEncounter >= currentEncounterTotalRequiredSteps * .5f) {
            minimapImage.DOColor(new(30/255f, 60/255f, 0/255f, 183/255f), 1f);
            Debug.LogWarning("green");
        } else if (stepsUntilEncounter >= currentEncounterTotalRequiredSteps * .33f) {
            minimapImage.DOColor(new(60/255f, 60/255f, 0/255f, 183/255f), 1f);
            Debug.LogWarning("yellow");
        } else if (stepsUntilEncounter >= currentEncounterTotalRequiredSteps * .20f) {
            minimapImage.DOColor(new(60/255f, 30/255f, 0/255f, 183/255f), 1f);
            Debug.LogWarning("orange");
        } else {
            minimapImage.DOColor(new(60/255f, 0/255f, 0/255f, 183/255f), 1f);
            Debug.LogWarning("red");
        }
    }

    private void OnRotateComplete() {
        if (OnUpdatePlayerMarkerRotation != null)
            OnUpdatePlayerMarkerRotation.Invoke(Mathf.RoundToInt(PlayerTransform.localRotation.eulerAngles.y));
        print(PlayerTransform.localRotation.eulerAngles.y);
    }

    private void OnMapZoomInAction() {
        if (MapLockedToMinimap()) return;
        uiMapRender.SetScaleOfFullMap(uiMapRender.GetMapScale() + (.25f * uiMapZoomSpeed) );
    }

    private void OnMapZoomOutAction() {
        if (MapLockedToMinimap()) return;
        uiMapRender.SetScaleOfFullMap(uiMapRender.GetMapScale() - (.25f * uiMapZoomSpeed));
    }

    private void OnPanMapAction() {
        if (MapLockedToMinimap()) return;
        Vector2 inputVal = panMapAction.ReadValue<Vector2>();
        Vector3 inputVal3D = new(inputVal.x, inputVal.y, 0f);
        uiMapRender.transform.Find("FullMapGrid").GetComponent<RectTransform>().localPosition -= inputVal3D * uiMapPanSpeed;
    }

    private void OnToggleMapView(InputAction.CallbackContext context) {
        if (!context.performed) return; 
        if (!MapLockedToMinimap()) {
            // Send back to minimap
           SendBackToMinimap();
        } else {
            // Show full map
            ShowFullMap();
        }
    }

    private void SendBackToMinimap() {
        if (fullMap.transform.parent == minimapContainer.transform) return;
        fullMapGridLastPosition = fullMapGrid.GetComponent<RectTransform>().localPosition;
        fullMapGridLastSize = fullMapGrid.GetComponent<RectTransform>().localScale.x;
        fullMap.transform.SetParent(minimapContainer.transform, false);
        fullMap.GetComponent<RectTransform>().localScale = Vector3.one * .93f;
        fullMapGrid.GetComponent<RectTransform>().localScale = Vector3.one;
        fullMapGrid.GetComponent<RectTransform>().localPosition = Vector3.zero;
    }

    private void ShowFullMap() {
        if (fullMap.transform.parent == fullMapContainer.transform) return;
        minimapLastPosition = fullMap.GetComponent<RectTransform>().localPosition;
        fullMap.transform.SetParent(fullMapContainer.transform, false);
        fullMap.transform.SetAsLastSibling();
        fullMapGrid.GetComponent<RectTransform>().localScale = Vector3.one * fullMapGridLastSize;
        fullMapGrid.GetComponent<RectTransform>().localPosition = fullMapGridLastPosition;
        fullMap.GetComponent<RectTransform>().localScale = Vector3.one;
    }
    
    private bool MapLockedToMinimap() {
        return fullMap.transform.parent == minimapContainer.transform;
    }

    private void FootstepSound() {
        AudioManager.Instance.PlaySFXAtPoint(PlayerTransform.position, Resources.Load<AudioClip>("Audio/SFX/RIFT/FOOTSTEP_GRASS"));
    }

    private void Update() {
        if (!Active) return;
        if (movementTimer < movementCooldown) {
            movementTimer += Time.deltaTime;
        }
        
        if (GameStateManager.Instance.CurrentGameState != GameState.ABYSS) return;

        if (wasdAction.IsPressed()) {
            OnWASDAction();
        } else if (strafeLeftAction.IsPressed()) {
            OnStrafeLeftAction();
        } else if (strafeRightAction.IsPressed()) {
            OnStrafeRightAction();
        }

        if (mapZoomInAction.IsPressed()) {
            OnMapZoomInAction();
        } else if (mapZoomOutAction.IsPressed()) {
            OnMapZoomOutAction();
        } else if (panMapAction.IsPressed()) {
            OnPanMapAction();
        }
        
    }


    public void Enable() {
        Active = true;
        toggleMapViewAction.performed += OnToggleMapView;
    }
    public void Disable() {
        Active = false;
        toggleMapViewAction.performed -= OnToggleMapView;
    }
}
