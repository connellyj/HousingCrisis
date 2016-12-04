using UnityEngine;
using System;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {

    public GameObject[] pathTiles;
    public int gridHeight;
    public int gridWidth;

    public static int MAX_ROW;
    public static int MAX_COL;
    public static List<int> exits;
    public static List<int> paths;
    
    void Awake() {
        exits = new List<int>();
        paths = new List<int>();
        MAX_ROW = gridHeight;
        MAX_COL = gridWidth;
        ParseGrid();
    }	

    // Parses the provided info into data structures
    void ParseGrid() {
        int x;
        int y;
        int index;
        foreach(GameObject tile in pathTiles) {
            x = (int) Mathf.Round(tile.transform.position.x);
            y = (int) Mathf.Round(tile.transform.position.y);
            index = CoordsToIndex(x, y);
            paths.Add(index);
            if(x == 0 || y == 0 || x == MAX_COL - 1 || y == MAX_ROW - 1) exits.Add(index);
        }
        Pathfinder.InitExitDistanceArray();
    }
    
    // Returns whether or not the given index is a path
    public static bool CellIsPath(int idx) {
        if(idx < 0 || idx >= MAX_COL * MAX_ROW) return false;
        if(paths.Contains(idx)) return true;
        return false;
    }

    // Converts (x, y) coordinates to grid index
    public static int CoordsToIndex(int x, int y) {
        return (MAX_ROW - 1 - y) * MAX_COL + x;
    }

    // Returns the directions in which the given coordinates has adjacent paths
    public static List<Direction> GetAdjacentPathDirections(int x, int y) {
        int data = CoordsToIndex(x, y);
        List<Direction> adjacentPaths = new List<Direction>();
        if(CellIsPath(data + 1)) adjacentPaths.Add(Direction.EAST);
        if(CellIsPath(data - 1)) adjacentPaths.Add(Direction.WEST);
        if(CellIsPath(data + MAX_COL)) adjacentPaths.Add(Direction.SOUTH);
        if(CellIsPath(data - MAX_COL)) adjacentPaths.Add(Direction.NORTH);
        return adjacentPaths;
    }

    // Returns the indeces adjacent to the given index
    public static List<int> GetAdjacentIndeces(int idx) {
        List<int> adj = new List<int>();
        adj.Add(idx + 1);
        adj.Add(idx - 1);
        adj.Add(idx + MAX_COL);
        adj.Add(idx - MAX_COL);
        return adj;
    }

    // Converts from Direction to Vector3
    public static Vector3 DirectionToVector(Direction d) {
        switch (d) {
            case Direction.NORTH:
                return Vector3.up;
            case Direction.SOUTH:
                return Vector3.down;
            case Direction.WEST:
                return Vector3.left;
            case Direction.EAST:
                return Vector3.right;
            default:
                throw new InvalidOperationException("Direction cannot be converted to vector");
        }
    }
}
