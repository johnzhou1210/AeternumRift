using System;
using UnityEngine;

public class MapManager : MonoBehaviour
{
   public static MapManager Instance;
   public static readonly int GRID_SIZE = 41;
   [SerializeField] private GameObject layoutPrefab;

   private void Awake() {
      if (Instance == null) {
         Instance = this;
         DontDestroyOnLoad(this);
      } else {
         Destroy(this);
      }
   }

   private void Start() {
      CurrentMap = ImportMap(layoutPrefab);
   }

   public DungeonGrid CurrentMap { get; private set; }
   
   public DungeonGrid ImportMap(GameObject layoutPrefab) { // Takes in a structured map layout prefab GameObject and replaces map with this data, returning a DungeonGrid.
      DungeonGrid importedMap = new DungeonGrid(GRID_SIZE);
      
      Transform floorContainer = layoutPrefab.transform.Find("Floor");
      Transform wallContainer = layoutPrefab.transform.Find("Walls");
      Transform doorContainer = layoutPrefab.transform.Find("Doors");
      
      // Import map grid layout
      foreach (Transform wallsChild in wallContainer) {
         Vector2Int currCoords = new((int)wallsChild.transform.localPosition.x, (int)wallsChild.transform.localPosition.z);
         importedMap.GetCellByXY(currCoords).SetTerrainType(TerrainType.Wall);
      }
      
      foreach (Transform floorChild in floorContainer) {
         Vector2Int currCoords = new((int)floorChild.transform.localPosition.x, (int)floorChild.transform.localPosition.z);
         importedMap.GetCellByXY(currCoords).SetTerrainType(TerrainType.Floor);
      }
      
      foreach (Transform doorsChild in doorContainer) {
         Vector2Int currCoords = new((int)doorsChild.transform.localPosition.x, (int)doorsChild.transform.localPosition.z);
         importedMap.GetCellByXY(currCoords).SetTerrainType(TerrainType.Door);;
         
      }

      return importedMap;
   }
   
}
