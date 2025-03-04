using System;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class RenderUIMap : MonoBehaviour {
   [SerializeField] private GameObject playerMarker;
   [SerializeField] private GameObject fullMapUI;

   [SerializeField] private GameObject layoutPrefab;
   [SerializeField] private GameObject cellUIPrefab;
   [SerializeField] private GameObject playerObject;

   [SerializeField] public float MinMapScale { get; private set; } = 1.75f;
   [SerializeField] public float MaxMapScale { get; private set; } = 5f;
   
   public DungeonGrid Grid { get; private set; } = new(41);

   private GameObject currPlayerMarker;
   
   private void Start() {
      PopulateGrid();
      
      Transform floorContainer = layoutPrefab.transform.Find("Floor");
      Transform wallContainer = layoutPrefab.transform.Find("Walls");
      Transform doorContainer = layoutPrefab.transform.Find("Doors");
      
      // Import map grid layout
      foreach (Transform floorChild in floorContainer) {
         Vector2Int currCoords = new((int)floorChild.transform.localPosition.x, (int)floorChild.transform.localPosition.z);
         GameObject currCellUI = fullMapUI.transform.Find(currCoords.x + ", " + currCoords.y).gameObject;
         currCellUI.GetComponent<Image>().color = new Color(.5f,.5f,.5f,.5f);
         Cell currCellObj = Grid.GetCellByXY(currCoords);
         currCellObj.SetTerrainType(TerrainType.Floor);
      }

      foreach (Transform wallsChild in wallContainer) {
         Vector2Int currCoords = new((int)wallsChild.transform.localPosition.x, (int)wallsChild.transform.localPosition.z);
         GameObject currCellUI = fullMapUI.transform.Find(currCoords.x + ", " + currCoords.y).gameObject;
         currCellUI.GetComponent<Image>().color = new Color(1f,.25f,.25f,1f);
         Cell currCellObj = Grid.GetCellByXY(currCoords);
         currCellObj.SetTerrainType(TerrainType.Wall);
      }

      foreach (Transform doorsChild in doorContainer) {
         Vector2Int currCoords = new((int)doorsChild.transform.localPosition.x, (int)doorsChild.transform.localPosition.z);
         GameObject currCellUI = fullMapUI.transform.Find(currCoords.x + ", " + currCoords.y).gameObject;
         currCellUI.GetComponent<Image>().color = new Color(.25f,1f,.25f,1f);
         Cell currCellObj = Grid.GetCellByXY(currCoords);
         currCellObj.SetTerrainType(TerrainType.Door);
      }
      
      // Place player marker
      Vector2Int playerPos = new((int)playerObject.transform.localPosition.x, (int)playerObject.transform.localPosition.z);
      Transform playerMarkerParent = fullMapUI.transform.Find(playerPos.x + ", " + playerPos.y).Find("EntityMarkerLayer");
      currPlayerMarker = Instantiate(playerMarker, Vector3.zero, Quaternion.identity, playerMarkerParent);
      PlayerDungeonInputManager.OnUpdatePlayerMarkerPosition += UpdatePlayerMarkerPosition;
      PlayerDungeonInputManager.OnUpdatePlayerMarkerRotation += UpdatePlayerMarkerRotation;


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
            newCellUI.name = Grid.GetX(col) + ", " + Grid.GetY(row);
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
