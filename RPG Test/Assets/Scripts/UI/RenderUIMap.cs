using System;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class RenderUIMap : MonoBehaviour {
   [SerializeField] private GameObject playerMarker, mapMarker;
   [SerializeField] private GameObject fullMapUI;

   [SerializeField] private GameObject layoutPrefab;
   [SerializeField] private GameObject cellUIPrefab;
   [SerializeField] private GameObject playerObject;

   [SerializeField] public float MinMapScale { get; private set; } = .5f;
   [SerializeField] public float MaxMapScale { get; private set; } = 6f;
   
   public DungeonGrid Grid { get; private set; } = new(41);

   private GameObject currPlayerMarker;
   private Sprite[] wallSpriteAtlas;
   
   private void Start() {
      PopulateGrid();
      

      wallSpriteAtlas = Resources.LoadAll<Sprite>("UI/SpriteAtlasses/wall-combinations");
      
     

      foreach (Cell cell in Grid.Grid) {
         // fullMapUI.transform.Find(cell.Position.x + ", " + cell.Position.y).GetComponent<Image>().sprite;
      }
      
      
      Invoke(nameof(InitializeMapContent), 1f);
      


   }

   private void InitializeMapContent() {
      fullMapUI.GetComponent<GridLayoutGroup>().enabled = false;
      
      
      Transform floorContainer = layoutPrefab.transform.Find("Floor");
      Transform wallContainer = layoutPrefab.transform.Find("Walls");
      Transform doorContainer = layoutPrefab.transform.Find("Doors");
      
      // Import map grid layout
      foreach (Transform wallsChild in wallContainer) {
         Vector2Int currCoords = new((int)wallsChild.transform.localPosition.x, (int)wallsChild.transform.localPosition.z);
         GameObject currCellUI = fullMapUI.transform.Find(currCoords.x + ", " + currCoords.y).gameObject;
         // currCellUI.GetComponent<Image>().color = new Color(1f,.25f,.25f,0f);
         Cell currCellObj = Grid.GetCellByXY(currCoords);
         currCellObj.SetTerrainType(TerrainType.Wall);
      }
      
      foreach (Transform floorChild in floorContainer) {
         Vector2Int currCoords = new((int)floorChild.transform.localPosition.x, (int)floorChild.transform.localPosition.z);
         GameObject currCellUI = fullMapUI.transform.Find(currCoords.x + ", " + currCoords.y).gameObject;
         currCellUI.GetComponent<Image>().color = new Color(.5f,.5f,.5f,.5f);
         Cell currCellObj = Grid.GetCellByXY(currCoords);
         currCellObj.SetTerrainType(TerrainType.Floor);
         SetUICellWallImage(currCellUI, currCellObj);
      }

      

      foreach (Transform doorsChild in doorContainer) {
         Vector2Int currCoords = new((int)doorsChild.transform.localPosition.x, (int)doorsChild.transform.localPosition.z);
         GameObject currCellUI = fullMapUI.transform.Find(currCoords.x + ", " + currCoords.y).gameObject;
         currCellUI.GetComponent<Image>().color = new Color(.5f,.5f,.5f,.5f);
         GameObject doorMarker = Instantiate(mapMarker, currCellUI.transform.Find("MapMarkerLayer"));
         
         doorMarker.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/DungeonMarkers");
         Cell currCellObj = Grid.GetCellByXY(currCoords);
         currCellObj.SetTerrainType(TerrainType.Door);
         SetUICellWallImage(currCellUI, currCellObj);
      }
      
      // Place player marker
      Vector2Int playerPos = new((int)playerObject.transform.localPosition.x, (int)playerObject.transform.localPosition.z);
      Transform playerMarkerParent = fullMapUI.transform.Find(playerPos.x + ", " + playerPos.y).Find("EntityMarkerLayer");
      currPlayerMarker = Instantiate(playerMarker, playerMarkerParent);
      PlayerDungeonInputManager.OnUpdatePlayerMarkerPosition += UpdatePlayerMarkerPosition;
      PlayerDungeonInputManager.OnUpdatePlayerMarkerRotation += UpdatePlayerMarkerRotation;
   }

   private void SetUICellWallImage(GameObject cellUI, Cell cellObj) {
      int indexToGet = (int)Grid.EvaluateNeighborWallSituation(cellObj);
      print($"{cellObj.Position.ToString()} chose index {indexToGet} {Grid.EvaluateNeighborWallSituation(cellObj).ToString()}");
      if (indexToGet == 15) return;
      Transform cellUIWallImageTransform = cellUI.transform.Find("WallSprite");
      Image cellUIImage = cellUIWallImageTransform.GetComponent<Image>();
      cellUIImage.enabled = true;
      print($"Setting {cellUI.name} {cellObj.Position.ToString()} to {wallSpriteAtlas[indexToGet].name}");
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
      for (int row = 0; row < Grid.GridSize; row++) {
         for (int col = 0; col < Grid.GridSize; col++) {
            GameObject newCellUI = Instantiate(cellUIPrefab, Vector3.zero, Quaternion.identity, fullMapUI.transform);
            // newCellUI.GetComponent<RectTransform>().localPosition = new Vector3(Grid.GetX(col), 0f, Grid.GetY(row));
            newCellUI.name = Grid.GetX(col) + ", " + Grid.GetY(row);
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
