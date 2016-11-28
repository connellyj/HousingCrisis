/* Written by Julia Connelly, 11/22/2016
 * 
 * Contains information about the grid
 */

using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {

    public string[] gridAsString;

    public static int MAX_ROW;
    public static int MAX_COL;
    public static List<int> exits;
    public static List<int> houses;
    public static List<int> paths;
    
    void Awake() {
        exits = new List<int>();
        houses = new List<int>();
        paths = new List<int>();
        ParseGrid();
    }	

    void ParseGrid() {
        int idx = 0;
        foreach(string str in gridAsString) {
            foreach(char ch in str) {
                if(ch == 'o' || ch == 'O') paths.Add(idx);
                else if(ch == 'e' || ch == 'E') {
                    exits.Add(idx);
                    paths.Add(idx);
                } else if(ch == 'h' || ch == 'H') houses.Add(idx);
                idx++;
            }
        }
        MAX_ROW = gridAsString.Length;
        MAX_COL = idx / MAX_ROW;
        foreach(int i in paths) Debug.Log(i);
    }

    // Returns true if the given node is a path
    public static bool CellIsPath(int idx) {
        if(idx < 0 || idx >= MAX_COL * MAX_ROW) return false;
        if(paths.Contains(idx)) return true;
        return false;
    }

    public static List<Direction> GetAdjacentPaths(int x, int y, GameObject tmp) {
        int data = (MAX_ROW - 1 - y) * MAX_COL + x;
        List<Direction> adjacentPaths = new List<Direction>();
        if(CellIsPath(data + 1)) adjacentPaths.Add(Direction.EAST);
        if(CellIsPath(data - 1)) adjacentPaths.Add(Direction.WEST);
        if(CellIsPath(data + MAX_COL)) adjacentPaths.Add(Direction.SOUTH);
        if(CellIsPath(data - MAX_COL)) adjacentPaths.Add(Direction.NORTH);
        return adjacentPaths;
    }
}
