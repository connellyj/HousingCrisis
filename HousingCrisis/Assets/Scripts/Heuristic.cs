using UnityEngine;
using System.Collections.Generic;

public class Heuristic {

    private static Heuristic instance;

    private int[] distances;

    public enum HeuristicType { EXIT, HOUSE, NONE }

    public static int H(Node n, HeuristicType type) {
        return instance.distances[coordsToIdx(n.personRow, n.personColumn)];
    }

    public static void initH() {
        if(instance == null) instance = new Heuristic();
    }

    private Heuristic() {
        initDistanceMap();
    }

    private int[] idxToCoords(int idx) {
        int[] result = new int[2];
        result[0] = idx / Node.gridHeight; // row
        result[1] = idx % Node.gridWidth; // col
        return result;
    }

    private static int coordsToIdx(int row, int col) {
        return row * Node.gridWidth + col;
    }

    public void initDistanceMap() {
        List<int> exits = FindExits();
        distances = new int[Node.gridHeight * Node.gridWidth];
        List<int> visited;
        foreach(int exit in exits) {
            visited = new List<int>();
            distances[exit] = 1;
            visited.Add(exit);
            makeDistanceMap(exit, visited);
        }
        for(int i = 0; i < Node.gridHeight * Node.gridWidth; i += Node.gridWidth) {
            Debug.Log(distances[i] + "  " + distances[i + 1] + "  " + distances[i + 2] + "  " + distances[i + 3] + "  " + distances[i + 4] + "  " + distances[i + 5] + "  " +
                distances[i + 6] + "  " + distances[i + 7] + "  " + distances[i + 8] + "  " + distances[i + 9] + "  " + distances[i + 10] + "  " + distances[i + 11]);
        }
    }

    private List<int> FindExits() {
        List<int> exits = new List<int>();
        for(int r = 0; r < Node.gridHeight; r++) {
            for(int c = 0; c < Node.gridWidth; c++) {
                if((r == 0 || r == Node.gridHeight - 1 || c == 0 || c == Node.gridWidth - 1) && Node.gridPath[r][c]) {
                    int tmp = coordsToIdx(r, c);
                    if(!exits.Contains(tmp)) exits.Add(tmp);
                }
            }
        }
        return exits;
    }

    private List<int> makeDistanceMap(int idx, List<int> visited) {
        List<int> adj = FindAdjacent(idx, visited);
        if(adj.Count == 0) return visited;
        foreach(int loc in adj) {
            visited.Add(loc);
            if(distances[loc] == 0 || distances[idx] + 1 < distances[loc]) {
                distances[loc] = distances[idx] + 1;
            }
            visited = makeDistanceMap(loc, visited);
        }
        return visited;
    }

    private List<int> FindAdjacent(int idx, List<int> visited) {
        List<int> adj = new List<int>();
        int[] coords = idxToCoords(idx);
        if(CellIsPath(coords[0] + 1, coords[1]) && !visited.Contains(coordsToIdx(coords[0] + 1, coords[1]))) adj.Add(coordsToIdx(coords[0] + 1, coords[1]));
        if(CellIsPath(coords[0] - 1, coords[1]) && !visited.Contains(coordsToIdx(coords[0] - 1, coords[1]))) adj.Add(coordsToIdx(coords[0] - 1, coords[1]));
        if(CellIsPath(coords[0], coords[1] + 1) && !visited.Contains(coordsToIdx(coords[0], coords[1] + 1))) adj.Add(coordsToIdx(coords[0], coords[1] + 1));
        if(CellIsPath(coords[0], coords[1] - 1) && !visited.Contains(coordsToIdx(coords[0], coords[1] - 1))) adj.Add(coordsToIdx(coords[0], coords[1] - 1));
        return adj;
    }

    private bool CellIsPath(int r, int c) {
        if(r >= Node.gridHeight || r < 0 || c < 0 || c >= Node.gridWidth) return false;
        if(Node.gridPath[r][c]) return true;
        return false;
    }
}
