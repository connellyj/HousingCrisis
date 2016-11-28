using System.Collections.Generic;
using System;

public class Pathfinder {

    private static int[] exitDistanceArray;
    private List<int> visited;

    public enum GoalType { EXIT, RANDOM_HOUSE, NONE }

    public Pathfinder() {
        InitExitDistanceArray();
    }

    // Conducts a graph search to find a path from the given location to a goal
    public List<Direction> FindPath(Person.PersonState state, int personLoc, GoalType goalType) {

        Node initialState = new Node(personLoc, goalType);

        Strategy strategy;
        if(state == Person.PersonState.PANIC) strategy = new Strategy.Greedy();
        else strategy = new Strategy.DFS();

        if(!strategy.ignoreFirstGoal && initialState.isGoal()) {
            return initialState.extractPlan();
        }
        strategy.addToFrontier(initialState);

        while(true) {
            if(strategy.frontierIsEmpty()) {
                return null;
            }

            Node leafNode = strategy.getAndRemoveLeaf();

            strategy.addToExplored(leafNode);
            foreach(Node n in leafNode.GetExpandedNodes()) {
                if(!strategy.isExplored(n) && !strategy.inFrontier(n)) {
                    if(n.isGoal()) {
                        return n.extractPlan();
                    }
                    strategy.addToFrontier(n);
                }
            }
        }
    }

    // Returns the heuristic value of the given node
    // which is the distance from the node to the closest exit
    private static int H(Node n) {
        switch(n.gType) {
            case GoalType.EXIT:
                return exitDistanceArray[n.data];
            default:
                return 0;
        }
    }

    // Creates an array that contains the distances from each location to the closest exit
    private void InitExitDistanceArray() {
        exitDistanceArray = new int[GridManager.MAX_ROW * GridManager.MAX_COL];
        CreateDistanceArray(GridManager.exits);
    }

    // Helper method for creating distance array
    private void CreateDistanceArray(List<int> startingLocations) {
        foreach(int loc in startingLocations) {
            visited = new List<int>();
            exitDistanceArray[loc] = 1;
            visited.Add(loc);
            CreateDistanceArray(loc);
        }
    }

    // Helper method for creating distance array
    private void CreateDistanceArray(int idx) {
        List<int> adj = FindAdjacent(idx);
        if(adj.Count == 0) return;
        foreach(int loc in adj) {
            visited.Add(loc);
            if(exitDistanceArray[loc] == 0 || exitDistanceArray[idx] + 1 < exitDistanceArray[loc]) {
                exitDistanceArray[loc] = exitDistanceArray[idx] + 1;
            }
            CreateDistanceArray(loc);
        }
    }

    // Returns all the locations adjacent to the given location that are paths
    private List<int> FindAdjacent(int idx) {
        List<int> adj = new List<int>();
        if(GridManager.CellIsPath(idx + 1) && !visited.Contains(idx + 1)) adj.Add(idx + 1);
        if(GridManager.CellIsPath(idx - 1) && !visited.Contains(idx - 1)) adj.Add(idx - 1);
        if(GridManager.CellIsPath(idx + GridManager.MAX_COL) && !visited.Contains(idx + GridManager.MAX_COL)) adj.Add(idx + GridManager.MAX_COL);
        if(GridManager.CellIsPath(idx - GridManager.MAX_COL) && !visited.Contains(idx - GridManager.MAX_COL)) adj.Add(idx - GridManager.MAX_COL);
        return adj;
    }



    // Used to conduct searches and find paths
    public class Node : IComparable<Node> {

        private int goal;
        public int data;
        public GoalType gType;
        private Direction direction;
        private Node parent;
        private Random rng = new Random();

        public Node(int personLoc, GoalType hType) {
            this.parent = null;
            this.direction = Direction.NONE;
            this.data = personLoc;
            this.gType = hType;
            if(hType == GoalType.RANDOM_HOUSE) {
                this.goal = GridManager.houses[rng.Next(0, GridManager.houses.Count)];
            }
        }

        public Node(Node parent, Direction direction, int personLoc) {
            this.parent = parent;
            this.direction = direction;
            this.data = personLoc;
            this.gType = parent.gType;
            this.goal = parent.goal;
        }

        // Returns whether or not the current node is a goal state
        public bool isGoal() {
            if(gType == GoalType.EXIT) return GridManager.exits.Contains(data);
            if(gType == GoalType.RANDOM_HOUSE) return data == goal;
            return false;
        }

        // Returns all the path nodes that are adjacent to this node in a random order
        public List<Node> GetExpandedNodes() {
            List<Node> expandedNodes = new List<Node>();
            if(GridManager.CellIsPath(data + 1)) expandedNodes.Add(new Node(this, Direction.EAST, data + 1));
            if(GridManager.CellIsPath(data - 1)) expandedNodes.Add(new Node(this, Direction.WEST, data - 1));
            if(GridManager.CellIsPath(data + GridManager.MAX_COL)) expandedNodes.Add(new Node(this, Direction.SOUTH, data + GridManager.MAX_COL));
            if(GridManager.CellIsPath(data - GridManager.MAX_COL)) expandedNodes.Add(new Node(this, Direction.NORTH, data - GridManager.MAX_COL));
            return Shuffle(expandedNodes);
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
            return H(this) - H(n);
        }
    }



    // Used to determine what sort of graph search to conduct
    public abstract class Strategy {

        private List<int> explored;
        public bool ignoreFirstGoal;

        public Strategy() {
            explored = new List<int>();
        }

        public void addToExplored(Node n) {
            explored.Add(n.data);
        }

        public bool isExplored(Node n) {
            return explored.Contains(n.data);
        }

        public int countExplored() {
            return explored.Count;
        }

        public abstract Node getAndRemoveLeaf();

        public abstract void addToFrontier(Node n);

        public abstract bool inFrontier(Node n);

        public abstract int countFrontier();

        public abstract bool frontierIsEmpty();


        // Used when shortest path required
        public class Greedy : Strategy {

            private PriorityQueue<Node> frontier;

            public Greedy() : base() {
                frontier = new PriorityQueue<Node>();
                ignoreFirstGoal = false;
            }

            public override Node getAndRemoveLeaf() {
                return frontier.Dequeue();
            }

            public override void addToFrontier(Node n) {
                frontier.Enqueue(n);
            }

            public override int countFrontier() {
                return frontier.Count();
            }

            public override bool frontierIsEmpty() {
                return frontier.Count() == 0;
            }

            public override bool inFrontier(Node n) {
                return frontier.Contains(n);
            }
        }


        // Used for a random path
        public class DFS : Strategy {

            private Stack<Node> frontier;

            public DFS() : base() {
                frontier = new Stack<Node>();
                ignoreFirstGoal = true;
            }

            public override Node getAndRemoveLeaf() {
                return frontier.Pop();
            }

            public override void addToFrontier(Node n) {
                frontier.Push(n);
            }

            public override int countFrontier() {
                return frontier.Count;
            }

            public override bool frontierIsEmpty() {
                return frontier.Count == 0;
            }

            public override bool inFrontier(Node n) {
                return frontier.Contains(n);
            }
        }
    }
}