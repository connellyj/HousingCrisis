/* Written by Julia Connelly, 11/22/2016
 * 
 * Nodes used to represent the graph during search
 */

using System;
using System.Collections.Generic;

public class Node : IComparable<Node> {

    public int data;
    public Heuristic.HeuristicType hType;
    private Direction direction;
    private Node parent;
    private Random rng = new Random();

    public Node(int personLoc, Heuristic.HeuristicType hType) {
        this.parent = null;
        this.direction = Direction.NONE;
        this.data = personLoc;
        this.hType = hType;
    }

    public Node(Node parent, Direction direction, int personLoc) {
        this.parent = parent;
        this.direction = direction;
        this.data = personLoc;
        this.hType = parent.hType;
    }

    // Depending on the heuristic, returns whether or not the current node is a goal state
    public bool isGoal() {
        if(hType == Heuristic.HeuristicType.EXIT) return GridManager.exits.Contains(data);
        return GridManager.houses.Contains(data + 1) || GridManager.houses.Contains(data - 1) || 
            GridManager.houses.Contains(data + GridManager.MAX_COL) || GridManager.houses.Contains(data - GridManager.MAX_COL);
    }

    // Returns all the path nodes that are adjacent to this node in a random order
    public List<Node> GetExpandedNodes() {
        List<Node> expandedNodes = new List<Node>();
        if(CellIsPath(data + 1)) expandedNodes.Add(new Node(this, Direction.EAST, data + 1));
        if(CellIsPath(data - 1)) expandedNodes.Add(new Node(this, Direction.WEST, data - 1));
        if(CellIsPath(data + GridManager.MAX_COL)) expandedNodes.Add(new Node(this, Direction.SOUTH, data + GridManager.MAX_COL));
        if(CellIsPath(data - GridManager.MAX_COL)) expandedNodes.Add(new Node(this, Direction.NORTH, data - GridManager.MAX_COL));
        return Shuffle(expandedNodes);
    }

    // Returns true if the given node is a path
    public static bool CellIsPath(int idx) {
        if(idx < 0 || idx >= GridManager.MAX_COL * GridManager.MAX_ROW) return false;
        if(GridManager.paths.Contains(idx)) return true;
        return false;
    }

    // Returns the list of directions, aka the path solution
    public List<Direction> extractPlan() {
        List<Direction> plan = new List<Direction>();
        Node n = this;
        while(n.parent != null) {
            plan.Add(n.direction);
            n = n.parent;
        }
        plan.Reverse();
        return plan;
    }

    // Helper method to shuffle a list
    private List<Node> Shuffle(List<Node> list) {
        int n = list.Count;
        while(n > 1) {
            n--;
            int k = rng.Next(n + 1);
            Node value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    // Compares nodes using their heuristic value
    public int CompareTo(Node n) {
        return Heuristic.H(this) - Heuristic.H(n);
    }
}