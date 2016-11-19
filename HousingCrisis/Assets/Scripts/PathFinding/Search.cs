using System.Collections.Generic;

public class Search {

    public static List<Node.Direction> DoSearch(Strategy strategy, Node initialState) {
        Heuristic.InitDistanceMap();
        if(initialState.isExit()) {
            return initialState.extractPlan();
        }
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