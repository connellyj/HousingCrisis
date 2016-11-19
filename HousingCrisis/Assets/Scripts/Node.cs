using System;
using System.Collections.Generic;

public class Node : IComparable<Node> {

    public static readonly int gridWidth = 12;
    public static readonly int gridHeight = 12;

    private Random rng = new Random();

    public static bool[][] gridPath;

    public int personRow;
    public int personColumn;
    public Direction direction;
    private Node parent;
    private Heuristic.HeuristicType hType;

    public Node(Direction direction, int row, int column, Heuristic.HeuristicType hType) {
        this.parent = null;
        this.direction = direction;
        personColumn = column;
        personRow = row;
        this.hType = hType;
    }

    public Node(Node parent, Direction direction, int row, int column) {
        this.parent = parent;
        this.direction = direction;
        this.personRow = row;
        this.personColumn = column;
        this.hType = parent.hType;
    }

    public bool isExit() {
        return personColumn == 0 || personRow == 0 || personColumn == gridWidth - 1 || personRow == gridHeight - 1;
    }

    public List<Node> GetExpandedNodes() {
        List<Node> expandedNodes = new List<Node>();
        if(personRow - 1 >= 0) {
            if(gridPath[personRow - 1][personColumn]) {
                expandedNodes.Add(new Node(this, Direction.NORTH, personRow - 1, personColumn));
            }
        }
        if(personRow + 1 < gridHeight) {
            if(gridPath[personRow + 1][personColumn]) {
                expandedNodes.Add(new Node(this, Direction.SOUTH, personRow + 1, personColumn));
            }
        }
        if(personColumn - 1 >= 0) {
            if(gridPath[personRow][personColumn - 1]) {
                expandedNodes.Add(new Node(this, Direction.WEST, personRow, personColumn - 1));
            }
        }
        if(personColumn + 1 < gridWidth) {
            if(gridPath[personRow][personColumn + 1]) {
                expandedNodes.Add(new Node(this, Direction.EAST, personRow, personColumn + 1));
            }
        }
        return Shuffle(expandedNodes);
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

    public enum Direction { NORTH, SOUTH, WEST, EAST }

    public int CompareTo(Node n) {
        return Heuristic.H(this, hType) - Heuristic.H(n, hType);
    }

    private static int coordsToIdx(int row, int col) {
        return row * gridWidth + col;
    }
}