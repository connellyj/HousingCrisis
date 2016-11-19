using System;
using System.Collections.Generic;

public class Node : IComparable<Node> {

    public int personLoc;
    public Direction direction;
    private Node parent;
    private Heuristic.HeuristicType hType;
    private Random rng = new Random();

    public Node(int personLoc, Heuristic.HeuristicType hType) {
        this.parent = null;
        this.direction = Direction.NONE;
        this.personLoc = personLoc;
        this.hType = hType;
    }

    public Node(Node parent, Direction direction, int personLoc) {
        this.parent = parent;
        this.direction = direction;
        this.personLoc = personLoc;
        this.hType = parent.hType;
    }

    public bool isExit() {
        return GridManager.exits.Contains(personLoc);
    }

    public List<Node> GetExpandedNodes() {
        List<Node> expandedNodes = new List<Node>();
        if(CellIsPath(personLoc + 1)) expandedNodes.Add(new Node(this, Direction.EAST, personLoc + 1));
        if(CellIsPath(personLoc - 1)) expandedNodes.Add(new Node(this, Direction.WEST, personLoc - 1));
        if(CellIsPath(personLoc + GridManager.MAX_COL)) expandedNodes.Add(new Node(this, Direction.SOUTH, personLoc + GridManager.MAX_COL));
        if(CellIsPath(personLoc - GridManager.MAX_COL)) expandedNodes.Add(new Node(this, Direction.NORTH, personLoc - GridManager.MAX_COL));
        return Shuffle(expandedNodes);
    }

    public static bool CellIsPath(int idx) {
        if(idx < 0 || idx >= GridManager.MAX_COL * GridManager.MAX_ROW) return false;
        if(GridManager.paths.Contains(idx)) return true;
        return false;
    }

    public List<Direction> extractPlan() {
        List<Direction> plan = new List<Direction>();
        Node n = this;
        while(n.parent != null) {
            plan.Add(n.direction);
            n = n.parent;
        }
        return plan;
    }

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

    public int CompareTo(Node n) {
        return Heuristic.H(this, hType) - Heuristic.H(n, hType);
    }

    public enum Direction { NORTH, SOUTH, WEST, EAST, NONE }
}