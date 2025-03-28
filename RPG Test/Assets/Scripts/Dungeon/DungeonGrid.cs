using System;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.ProBuilder;

public enum Direction {
    North,
    South,
    East,
    West,
}

public enum DirectionCombination {
    NSEW,
    NSE,
    NSW,
    NE,
    N,
    SW,
    W,
    SE,
    S,
    E,
    NS,
    NEW,
    SEW,
    EW,
    NW,
    None
}

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
        Debug.Log($"Trying to get cell at position {position.ToString()} [Row {GetRow(position.y)} and Col {GetCol(position.x)}]");
        Cell resultCell = Grid[GetRow(position.y), GetCol(position.x)]; 
        return resultCell;
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
        return (GridSize / 2) - y;
    }

    private bool IsWithinBounds(Vector2Int position) {
        return position.x <= (int)Mathf.Abs(GridSize / 2f) && position.y <= (int)Mathf.Abs(GridSize / 2f);
    }

    public Cell GetNeighbor(Cell cell, Direction dir) {
        Vector2Int neighborPos = cell.Position;
        switch (dir) {
            case Direction.North:
                neighborPos += new Vector2Int(0, 1);
            break;
            case Direction.South:
                neighborPos += new Vector2Int(0, -1);
            break;
            case Direction.East:
                neighborPos += new Vector2Int(1, 0);
            break;
            case Direction.West:
                neighborPos += new Vector2Int(-1, 0);
            break;
        }
        if (neighborPos == cell.Position) throw new InvalidEnumArgumentException("Invalid direction");
        if (!IsWithinBounds(neighborPos)) return null;
        return GetCellByXY(neighborPos);
    }
    

    public DirectionCombination EvaluateNeighborWallSituation(Cell cell) {
        
        Cell northNeighbor = GetNeighbor(cell, Direction.North);
        Cell southNeighbor = GetNeighbor(cell, Direction.South);
        Cell eastNeighbor = GetNeighbor(cell, Direction.East);
        Cell westNeighbor = GetNeighbor(cell, Direction.West);
        
        int n = northNeighbor is { Terrain: TerrainType.Wall } ? 1 : 0;
        int s = southNeighbor is { Terrain: TerrainType.Wall } ? 1 : 0;
        int e = eastNeighbor is {Terrain: TerrainType.Wall} ? 1 : 0;
        int w = westNeighbor is {Terrain : TerrainType.Wall} ? 1 : 0;
        
        string evaluation = n.ToString() + s.ToString() + e.ToString() + w.ToString();
        
        Debug.Log($"{cell.Position.ToString()} is: Evaluation: {evaluation}");
        switch (evaluation) {
            case "0000":
                return DirectionCombination.None;
            case "0001":
                return DirectionCombination.W;
            case "0010":
                return DirectionCombination.E;
            case "0011":
                return DirectionCombination.EW;
            case "0100":
                return DirectionCombination.S;
            case "0101":
                return DirectionCombination.SW;
            case "0110":
                return DirectionCombination.SE;
            case "0111":
                return DirectionCombination.SEW;
            case "1000":
                return DirectionCombination.N;
            case "1001":
                return DirectionCombination.NW;
            case "1010":
                return DirectionCombination.NE;
            case "1011":
                return DirectionCombination.NEW;
            case "1100":
                return DirectionCombination.NS;
            case "1101":
                return DirectionCombination.NSW;
            case "1110":
                return DirectionCombination.NSE;
            case "1111":
                return DirectionCombination.NSEW;
        }
        throw new Exception("Could not evaluate direction string");
    }


    public Vector3 ConvertDirectionToEulerAngles(Direction dir) {
        switch (dir) {
            case Direction.North:
                return new(0, 0, 0);
            break;
            case Direction.South:
                return new(0, 180, 0);
            break;
            case Direction.East:
                return new(0, 90, 0);
            break;
            case Direction.West:
                return new(0, 0, 270);
            break;
        }

        throw new InvalidEnumArgumentException("Invalid direction enum!");
    }
    
}