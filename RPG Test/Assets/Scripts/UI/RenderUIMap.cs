using System;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class RenderUIMap : MonoBehaviour {
   [SerializeField] private GameObject playerMarker, mapMarker;
   [SerializeField] private GameObject fullMapUI;
   
   [SerializeField] private GameObject cellUIPrefab;
   
   private GameObject layoutPrefab;

   [SerializeField] public float MinMapScale { get; private set; } = .5f;
   [SerializeField] public float MaxMapScale { get; private set; } = 6f;
   

   private GameObject currPlayerMarker;
   private Sprite[] wallSpriteAtlas;


   private void Start() {
      PopulateGrid();
      
      wallSpriteAtlas = Resources.LoadAll<Sprite>("UI/SpriteAtlasses/wall-combinations");
      
      Invoke(nameof(InitializeMapContent), 1f);
      


   }

   private void InitializeMapContent() {
      fullMapUI.GetComponent<GridLayoutGroup>().enabled = false;
      
      // Look through MapManager's current map to render everything
      foreach (Cell cellObj in MapManager.Instance.CurrentMap.Grid) {
         Vector2Int currCoords = cellObj.Position;
         GameObject currCellUI = fullMapUI.transform.Find(currCoords.x + ", " + currCoords.y).gameObject;
         Color cellColor = new Color(18/255f, 28/255f, 26/255f, 216/255f);
         
         switch (cellObj.Terrain) {
            case TerrainType.None:
            break;
            case TerrainType.Floor:
               cellColor = new Color(.5f,.5f,.5f,.5f);
               SetUICellWallImage(currCellUI, cellObj);
            break;
            case TerrainType.Wall:
            break;
            case TerrainType.Door:
              cellColor = new Color(.5f,.5f,.5f,.5f);
              GameObject doorMarker = Instantiate(mapMarker, currCellUI.transform.Find("MapMarkerLayer"));
              doorMarker.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/DungeonMarkers");
              SetUICellWallImage(currCellUI, cellObj);
           break;
         }
         
         currCellUI.GetComponent<Image>().color = cellColor;
         
      }
      
      
      
      
      // Place player marker
      Vector2Int playerPos = new((int)(PlayerInputFuncs.GetPlayerTransform.Invoke()).localPosition.x, (int)(PlayerInputFuncs.GetPlayerTransform.Invoke()).localPosition.z);
      Transform playerMarkerParent = fullMapUI.transform.Find(playerPos.x + ", " + playerPos.y).Find("EntityMarkerLayer");
      currPlayerMarker = Instantiate(playerMarker, playerMarkerParent);
      PlayerDungeonInputManager.OnUpdatePlayerMarkerPosition += UpdatePlayerMarkerPosition;
      PlayerDungeonInputManager.OnUpdatePlayerMarkerRotation += UpdatePlayerMarkerRotation;
   }

   private void SetUICellWallImage(GameObject cellUI, Cell cellObj) {
      int indexToGet = (int)MapManager.Instance.CurrentMap.EvaluateNeighborWallSituation(cellObj);
      if (indexToGet == 15) return;
      Transform cellUIWallImageTransform = cellUI.transform.Find("WallSprite");
      Image cellUIImage = cellUIWallImageTransform.GetComponent<Image>();
      cellUIImage.enabled = true;
      cellUIImage.sprite = wallSpriteAtlas[indexToGet];
      cellUI.transform.SetAsLastSibling();
   }
   

   private void UpdatePlayerMarkerPosition(Vector2Int playerPos) {
      // Move marker to new parent
      Transform playerMarkerParent = fullMapUI.transform.Find(playerPos.x + ", " + playerPos.y).Find("EntityMarkerLayer");
      currPlayerMarker.transform.SetParent(playerMarkerParent);
      currPlayerMarker.transform.GetComponent<RectTransform>().localPosition = Vector3.zero;
   }

   private void UpdatePlayerMarkerRotation(int newRotation) {
      currPlayerMarker.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, -newRotation);
   }


   private void PopulateGrid() {
      // Create initial UI render
      for (int row = 0; row < MapManager.GRID_SIZE; row++) {
         for (int col = 0; col < MapManager.GRID_SIZE; col++) {
            GameObject newCellUI = Instantiate(cellUIPrefab, Vector3.zero, Quaternion.identity, fullMapUI.transform);
            newCellUI.name = MapManager.Instance.CurrentMap.GetX(col) + ", " + MapManager.Instance.CurrentMap.GetY(row);
            newCellUI.GetComponent<Image>().color = new(18/255f,28f/255,26/255f,216/255f);
         }
      }
   }
   
   
   
   private Vector2Int ParseCoordinates(string input) {
        string[] parts = input.Split(", ");
        if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y)) {
           return new Vector2Int(x, y);
        }
        throw new FormatException("Invalid input format");
   }
   

   private bool WithinRange(int n, int min, int max) {
      return n >= min && n <= max;
   }

   

   private GameObject GetCellByVector2Int(Vector2Int position) {
      return null;
   }

   public void SetScaleOfFullMap(float scale) {
      scale = Mathf.Clamp(scale, MinMapScale, MaxMapScale);
      fullMapUI.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
   }

   public float GetMapScale() {
      return fullMapUI.GetComponent<RectTransform>().localScale.x;
   }

   private void OnDestroy() {
      PlayerDungeonInputManager.OnUpdatePlayerMarkerPosition -= UpdatePlayerMarkerPosition;
      PlayerDungeonInputManager.OnUpdatePlayerMarkerRotation -= UpdatePlayerMarkerRotation;
   }
}
