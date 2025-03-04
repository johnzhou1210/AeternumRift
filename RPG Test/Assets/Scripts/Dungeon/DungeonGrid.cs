using UnityEngine;

public class DungeonGrid {
    public Cell[,] Grid { get; private set; }
    public int GridSize {get; private set; }

    public DungeonGrid(int size) {
        GridSize = size;
        Grid = new Cell[size, size];
        for (int row = 0; row < size; row++) {
            for (int col = 0; col < size; col++) {
                Grid[row, col] = new(GetX(col), GetY(row));
            }
        }
    }
    
    public Cell GetCellByXY(Vector2Int position) {
        return Grid[GetRow(position.y), GetCol(position.x)];
    }
    
    public int GetX(int col) {
        return col - (GridSize / 2);
    }

    public int GetY(int row) {
        return (GridSize / 2) - row;
    }

    private int GetCol(int x) {
        return x + (GridSize / 2);
    }

    private int GetRow(int y) {
        return y + (GridSize / 2);
    }
    
}