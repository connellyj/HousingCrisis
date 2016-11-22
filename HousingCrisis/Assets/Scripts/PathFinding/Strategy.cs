/* Written by Julia Connelly, 11/22/2016
 * 
 * Different search strategies
 */

using System.Collections.Generic;

public abstract class Strategy {

    private List<int> explored;

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


    // Used for wandering across the map
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
}