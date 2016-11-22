/* Written by Julia Connelly, 11/22/2016
 * 
 * Searches through a graph using the given strategy
 */

using System.Collections.Generic;

public class Search {

    public static List<Direction> DoSearch(Strategy strategy, Node initialState) {
        Heuristic.InitDistanceMap();
        if(initialState.isGoal()) {
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
                        if (n.isGoal()) {
                            return n.extractPlan();
                        }
                        strategy.addToFrontier(n);
                    }
                }
        }
    }
}