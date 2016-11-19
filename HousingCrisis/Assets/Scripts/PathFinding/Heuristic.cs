using UnityEngine;
using System.Collections.Generic;

public class Heuristic {

    private static int[] exitDistanceMap;
    private static int[] houseDistanceMap;
    private static List<int> visited;

    public static int H(Node n, HeuristicType type) {
        return exitDistanceMap[n.personLoc];
    }

    public static void InitDistanceMap() {
        exitDistanceMap = new int[GridManager.MAX_ROW * GridManager.MAX_COL];
        houseDistanceMap = new int[GridManager.MAX_ROW * GridManager.MAX_COL];
        CreateDistanceMap(GridManager.exits);
        CreateDistanceMap(GridManager.houses);

        print(exitDistanceMap);
    }

    private static void CreateDistanceMap(List<int> startingLocations) {
        foreach(int loc in startingLocations) {
            visited = new List<int>();
            exitDistanceMap[loc] = 1;
            visited.Add(loc);
            CreateDistanceMap(loc);
        }
    }

    private static void CreateDistanceMap(int idx) {
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

    private static List<int> FindAdjacent(int idx) {
        List<int> adj = new List<int>();
        if(Node.CellIsPath(idx + 1) && !visited.Contains(idx + 1)) adj.Add(idx + 1);
        if(Node.CellIsPath(idx - 1) && !visited.Contains(idx - 1)) adj.Add(idx - 1);
        if(Node.CellIsPath(idx + GridManager.MAX_COL) && !visited.Contains(idx + GridManager.MAX_COL)) adj.Add(idx + GridManager.MAX_COL);
        if(Node.CellIsPath(idx - GridManager.MAX_COL) && !visited.Contains(idx - GridManager.MAX_COL)) adj.Add(idx - GridManager.MAX_COL);
        return adj;
    }

    public enum HeuristicType { EXIT, HOUSE, NONE }

    private static void print(int[] lst) {
        for(int i = 0; i < GridManager.MAX_ROW * GridManager.MAX_COL; i += GridManager.MAX_COL) {
            string toPrint = "";
            for(int j = i; j < GridManager.MAX_COL + i; j++) {
                toPrint += lst[j] + "  ";
            }
            Debug.Log(toPrint);
        }
    }
}
