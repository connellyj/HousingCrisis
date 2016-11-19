using System.Collections.Generic;

public abstract class Strategy {

    private List<Node> explored;

    public Strategy() {
        explored = new List<Node>();
    }

    public void addToExplored(Node n) {
        explored.Add(n);
    }

    public bool isExplored(Node n) {
        return explored.Contains(n);
    }

    public int countExplored() {
        return explored.Count;
    }

    public abstract Node getAndRemoveLeaf();

    public abstract void addToFrontier(Node n);

    public abstract bool inFrontier(Node n);

    public abstract int countFrontier();

    public abstract bool frontierIsEmpty();


    public class Greedy : Strategy {

        private PriorityQueue<Node> frontier;

        public Greedy() : base() {
            frontier = new PriorityQueue<Node>();
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


    public class DFS : Strategy {

        private Stack<Node> frontier;

        public DFS() : base() {
            frontier = new Stack<Node>();
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


    public class BFS : Strategy {

        private Queue<Node> frontier;

        public BFS() : base() {
            frontier = new Queue<Node>();
        }

        public override Node getAndRemoveLeaf() {
            return frontier.Dequeue();
        }

        public override void addToFrontier(Node n) {
            frontier.Enqueue(n);
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