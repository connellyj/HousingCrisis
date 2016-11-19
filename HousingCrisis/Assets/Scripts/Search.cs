using System.Collections.Generic;
using UnityEngine;

public class Search : MonoBehaviour {

    void Start() {
        Node.gridPath = new bool[12][];
        Node.gridPath[0] = new bool[12] { false, false, false, true, false, false, false, false, false, true, false, false };
        Node.gridPath[1] = new bool[12] { true, true, false, true, true, true, true, true, false, true, true, true };
        Node.gridPath[2] = new bool[12] { false, true, false, true, false, false, true, true, false, true, true, true };
        Node.gridPath[3] = new bool[12] { false, true, false, true, false, false, false, true, false, true, false, false };
        Node.gridPath[4] = new bool[12] { false, true, true, true, false, false, false, true, false, true, false, false };
        Node.gridPath[5] = new bool[12] { false, false, false, false, false, false, false, true, false, true, false, false };
        Node.gridPath[6] = new bool[12] { false, false, false, true, false, false, false, false, false, true, false, false };
        Node.gridPath[7] = new bool[12] { true, true, false, true, true, true, true, true, false, true, true, true };
        Node.gridPath[8] = new bool[12] { false, true, false, true, false, false, true, true, false, true, true, true };
        Node.gridPath[9] = new bool[12] { false, true, false, true, false, false, false, true, false, true, false, false };
        Node.gridPath[10] = new bool[12] { false, true, true, true, false, false, false, true, false, true, false, false };
        Node.gridPath[11] = new bool[12] { false, false, false, false, false, false, false, true, false, true, false, false };
        Heuristic.initH();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.S)) {
            Debug.Log(DoSearch(new Strategy.Greedy(), new Node(Node.Direction.WEST, 1, 1, Heuristic.HeuristicType.EXIT)).Count);
        }
    }

    public List<Node.Direction> DoSearch(Strategy strategy, Node initialState) {
        if(initialState.isExit()) return initialState.extractPlan();
        strategy.addToFrontier(initialState);
        
        while (true) {
            if (strategy.frontierIsEmpty()) {
                return null;
            }

            Node leafNode = strategy.getAndRemoveLeaf();

            strategy.addToExplored(leafNode);
                foreach (Node n in leafNode.GetExpandedNodes()) {
                    if (!strategy.isExplored(n) && !strategy.inFrontier(n)) {
                        if (n.isExit()) {
                            return n.extractPlan();
                        }
                        strategy.addToFrontier(n);
                    }
                }
        }
    }
}