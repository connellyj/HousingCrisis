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
    }	

    void Start() {
        ParseGrid();
        Debug.Log(paths.Contains(45));
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.S)) {
            Debug.Log(Search.DoSearch(new Strategy.DFS(), new Node(45, Heuristic.HeuristicType.EXIT)).Count);
        }
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
    }
}
