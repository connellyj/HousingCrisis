/* Written by Julia Connelly, 11/22/2016
 * 
 * Used for heuristic precomputation
 */

using UnityEngine;
using System.Collections.Generic;

public class Heuristic {

    private static int[] exitDistanceMap;
    private static int[] houseDistanceMap;
    private static List<int> visited;

    // Returns the heuristic value of the given node
    public static int H(Node n) {
        switch(n.hType) {
            case HeuristicType.EXIT:
                return exitDistanceMap[n.data];
            case HeuristicType.HOUSE:
                return houseDistanceMap[n.data];
            case HeuristicType.NONE:
                return 0;
            default:
                return 0;
        }
    }

    // Creates the arrays where each location in the path contains the distance to the closest goal
    public static void InitDistanceMap() {
        exitDistanceMap = new int[GridManager.MAX_ROW * GridManager.MAX_COL];
        houseDistanceMap = new int[GridManager.MAX_ROW * GridManager.MAX_COL];
        CreateDistanceMap(GridManager.exits, exitDistanceMap);
        CreateDistanceMap(GridManager.houses, houseDistanceMap);

        //Debug.Log("exit map");
        //print(exitDistanceMap);
        //Debug.Log("==============================================================================");
        //Debug.Log("house map");
        //print(houseDistanceMap);
    }

    // Helper method for creating distance maps
    private static void CreateDistanceMap(List<int> startingLocations, int[] distanceMap) {
        foreach(int loc in startingLocations) {
            visited = new List<int>();
            distanceMap[loc] = 1;
            visited.Add(loc);
            CreateDistanceMap(loc, distanceMap);
        }
    }

    // Helper method for creating distance maps
    private static void CreateDistanceMap(int idx, int[] distanceMap) {
        List<int> adj = FindAdjacent(idx);
        if(adj.Count == 0) return;
        foreach(int loc in adj) {
            visited.Add(loc);
            if(distanceMap[loc] == 0 || distanceMap[idx] + 1 < distanceMap[loc]) {
                distanceMap[loc] = distanceMap[idx] + 1;
            }
            CreateDistanceMap(loc, distanceMap);
        }
    }

    // Returns all the nodes adjacent to the given node that are paths
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
