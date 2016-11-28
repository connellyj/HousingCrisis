﻿using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {

    public GameObject[] pathTiles;
    public int gridHeight;
    public int gridWidth;

    public static int MAX_ROW;
    public static int MAX_COL;
    public static List<int> exits;
    public static List<int> houses;
    public static List<int> paths;
    
    void Awake() {
        exits = new List<int>();
        houses = new List<int>();
        paths = new List<int>();
        MAX_ROW = gridHeight;
        MAX_COL = gridWidth;
        ParseGrid();
    }	

    void ParseGrid() {
        int x;
        int y;
        int index;
        foreach(GameObject tile in pathTiles) {
            x = (int) Mathf.Round(tile.transform.position.x);
            y = (int) Mathf.Round(tile.transform.position.y);
            index = coordsToIndex(x, y);
            paths.Add(index);
            if(x == 0 || y == 0 || x == MAX_COL - 1 || y == MAX_ROW - 1) exits.Add(index);
        }
    }
    
    public static bool CellIsPath(int idx) {
        if(idx < 0 || idx >= MAX_COL * MAX_ROW) return false;
        if(paths.Contains(idx)) return true;
        return false;
    }

    public static int coordsToIndex(int x, int y) {
        return (MAX_ROW - 1 - y) * MAX_COL + x;
    }

    public static List<Direction> GetAdjacentPaths(int x, int y, GameObject tmp) {
        int data = coordsToIndex(x, y);
        List<Direction> adjacentPaths = new List<Direction>();
        if(CellIsPath(data + 1)) adjacentPaths.Add(Direction.EAST);
        if(CellIsPath(data - 1)) adjacentPaths.Add(Direction.WEST);
        if(CellIsPath(data + MAX_COL)) adjacentPaths.Add(Direction.SOUTH);
        if(CellIsPath(data - MAX_COL)) adjacentPaths.Add(Direction.NORTH);
        return adjacentPaths;
    }
}
