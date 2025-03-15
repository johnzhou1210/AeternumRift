using UnityEngine;

public enum TerrainType {
    None,
    Floor,
    Wall,
    Door,
}

public class Cell {
    public Vector2Int Position { get; private set; }
    public TerrainType Terrain { get; private set; } = TerrainType.None;
    public bool IsVisited { get; private set; }
    
    public Cell(int x, int y) {
        Position = new Vector2Int(x, y);
    }

    public bool SetVisited(bool val) {
        IsVisited = val;
        return IsVisited;
    }

    public bool SetTerrainType(TerrainType val) {
        TerrainType lastTerrainType = Terrain;
        Terrain = val;
        if (lastTerrainType != TerrainType.None) {
            Debug.LogWarning(lastTerrainType + " overriden with " + Terrain);
        }
        return Terrain != lastTerrainType;
    }
    
}
