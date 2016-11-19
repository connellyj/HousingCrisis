using UnityEngine;
using System.Collections.Generic;

public class Heuristic {

    private static Heuristic instance;

    private int[] exitDistanceMap;
    private int[] houseDistanceMap;
    private List<int> visited;

    public enum HeuristicType { EXIT, HOUSE, NONE }

    public static int H(Node n, HeuristicType type) {
        return instance.exitDistanceMap[coordsToIdx(n.personRow, n.personColumn)];
    }

    public static void initH() {
        if(instance == null) instance = new Heuristic();
    }

    private Heuristic() {
        InitDistanceMap();
    }

    private static int coordsToIdx(int row, int col) {
        return row * Node.gridWidth + col;
    }

    public void InitDistanceMap() {
        exitDistanceMap = new int[GridManager.MAX_ROW * GridManager.MAX_COL];
        houseDistanceMap = new int[GridManager.MAX_ROW * GridManager.MAX_COL];
        CreateDistanceMap(GridManager.exits);
        CreateDistanceMap(GridManager.houses);

        for(int i = 0; i < GridManager.MAX_ROW * GridManager.MAX_COL; i += GridManager.MAX_COL) {
            string toPrint = "";
            for(int j = i; j < GridManager.MAX_COL + i; j++) {
                toPrint += exitDistanceMap[j] + "  ";
            }
            Debug.Log(toPrint);
        }
    }

    private void CreateDistanceMap(List<int> startingLocations) {
        foreach(int loc in startingLocations) {
            visited = new List<int>();
            exitDistanceMap[loc] = 1;
            visited.Add(loc);
            CreateDistanceMap(loc);
        }
    }

    private void CreateDistanceMap(int idx) {
        List<int> adj = FindAdjacent(idx);
        if(adj.Count == 0) return;
        foreach(int loc in adj) {
            visited.Add(loc);
            if(exitDistanceMap[loc] == 0 || exitDistanceMap[idx] + 1 < exitDistanceMap[loc]) {
                exitDistanceMap[loc] = exitDistanceMap[idx] + 1;
            }
            CreateDistanceMap(loc);
        }
    }

    private List<int> FindAdjacent(int idx) {
        List<int> adj = new List<int>();
        if(CellIsPath(idx + 1) && !visited.Contains(idx + 1)) adj.Add(idx + 1);
        if(CellIsPath(idx - 1) && !visited.Contains(idx - 1)) adj.Add(idx - 1);
        if(CellIsPath(idx + GridManager.MAX_COL) && !visited.Contains(idx + GridManager.MAX_COL)) adj.Add(idx + GridManager.MAX_COL);
        if(CellIsPath(idx - GridManager.MAX_COL) && !visited.Contains(idx - GridManager.MAX_COL)) adj.Add(idx - GridManager.MAX_COL);
        return adj;
    }

    private bool CellIsPath(int idx) {
        if(idx < 0 || idx >= GridManager.MAX_COL * GridManager.MAX_ROW) return false;
        if(GridManager.paths.Contains(idx)) return true;
        return false;
    }
}
