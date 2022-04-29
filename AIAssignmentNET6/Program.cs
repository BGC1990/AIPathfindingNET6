
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AIAssignment
{
    //Node<T> class, taken from https://docs.microsoft.com/en-us/previous-versions/ms379572(v=vs.80) 
    //the Node class provides the base class from which GraphNode can inherit
    public class Node<T>
    {
        //the Node class makes use of C# generics, any type of data can theoretically be used 
        private T data;
        private NodeList<T> connectedNodes = null;

        public Node() { }
        public Node(T data) : this(data, null) { }
        public Node(T data, NodeList<T> connectedNodes)
        {
            this.data = data;
            this.connectedNodes = connectedNodes;
        }

        public T Value
        {
            get { return data; }
            set { data = value; }
        }

        public NodeList<T> ConnectedNodes
        {
            get { return connectedNodes; }
            set { connectedNodes = value; }
        }

    }

    //The NodeList class extends the Collection class from the .NET framework, allowing for a custom collection to be implemented
    //taken from https://docs.microsoft.com/en-us/previous-versions/ms379572(v=vs.80)
    public class NodeList<T> : Collection<GraphNode<T>>
    {
        public NodeList() : base() { }

        public NodeList(int initSize)
        {
            for (int i = 0; i < initSize; i++)
            {
                base.Items.Add(new GraphNode<T>());
            }
        }

        //Find method iterates through all items within the selected NodeList.
        //If the node is found, the GraphNode is returned.
        //If not, null is returned. 
        public GraphNode<T> Find(T contents)
        {
            foreach (GraphNode<T> node in Items)
            {
                if (node.Value.Equals(contents))
                {
                    return node;
                }
            }
            return null;
        }
    }

    //GraphNode class. Inherits from Node class.
    //taken from https://docs.microsoft.com/en-us/previous-versions/ms379574(v=vs.80)?redirectedfrom=MSDN#datastructures20_5_topic3
    public class GraphNode<T> : Node<T>
    {
        private List<int> costs;

        public GraphNode() : base() { }
        public GraphNode(T value) : base(value) { }
        public GraphNode(T value, NodeList<T> connectedNodes) : base(value, connectedNodes) { }

        //creates a new NodeList of ConnectedNodes if no list exists
        new public NodeList<T> ConnectedNodes
        {
            get
            {
                if (base.ConnectedNodes == null)
                {
                    base.ConnectedNodes = new NodeList<T>();
                }
                return base.ConnectedNodes;
            }
        }

       //getter for costs
        public List<int> Costs
        {
            get
            {
                if (costs == null)
                {
                    costs = new List<int>();
                }
                return costs;
            }
        }
    }

    //The Graph class, modified from https://docs.microsoft.com/en-us/previous-versions/ms379574(v=vs.80)?redirectedfrom=MSDN#datastructures20_5_topic3
    //extends the IEnumerable class 
    public class Graph<T> : IEnumerable<T>
    {
        public NodeList<T> nodeSet;

        public Graph() : this(null) { }
        public Graph(NodeList<T> nodeSet)
        {
            if (nodeSet == null)
            {
                this.nodeSet = new NodeList<T>();
            }
            else
            {
                this.nodeSet = nodeSet;
            }
        }

        public void AddNode(GraphNode<T> node)
        {
            nodeSet.Add(node);
        }
        
        public void AddNode(T value)
        {
            nodeSet.Add(new GraphNode<T>(value));
        }

        //method which adds UndirectedEdges to NodeList and the costs to Costs
        //makes use of the "from" and "to" keywords
        public void AddUndirectedEdge(GraphNode<T> from, GraphNode<T> to, int cost) 
        {
            from.ConnectedNodes.Add(to);
            from.Costs.Add(cost);

            to.ConnectedNodes.Add(from);
            to.Costs.Add(cost);
        }

        public bool Contains(T value)
        {
            return nodeSet.Find(value) != null;  //FindByValue not recognised, changed to Find
        }

        public bool Remove(T value)
        {
            // first remove the node from the nodeset
            GraphNode<T> nodeToRemove = (GraphNode<T>)nodeSet.Find(value); //FindByVale not recognised, changed to Find
            if (nodeToRemove == null)
                // node wasn't found
                return false;

            // otherwise, the node was found
            nodeSet.Remove(nodeToRemove);

            // enumerate through each node in the nodeSet, removing edges to this node
            foreach (GraphNode<T> gnode in nodeSet)
            {
                int index = gnode.ConnectedNodes.IndexOf(nodeToRemove);
                if (index != -1)
                {
                    // remove the reference to the node and associated cost
                    gnode.ConnectedNodes.RemoveAt(index);
                    gnode.Costs.RemoveAt(index);
                }
            }

            return true;
        }

        public NodeList<T> Nodes
        {
            get
            {
                return nodeSet;
            }
        }

        public int Count
        {
            get { return nodeSet.Count; }
        }

        //these methods must be implented by .NET6 in order to compile
        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    //the Djikstra class, modified heavily from https://www.youtube.com/watch?v=FSm1zybd0Tk
    public class Dijkstra
    {
        public List<Object> Results(Graph<string> graph, GraphNode<string> node)
        {
            Dictionary<GraphNode<string>, double> totalCosts = new Dictionary<GraphNode<string>, double>();
            Dictionary<GraphNode<string>, GraphNode<string>> previousNodes = new Dictionary<GraphNode<string>, GraphNode<string>>();
            //.NET6 had to be used due to PriorityQueue not featuring in previous versions
            PriorityQueue<GraphNode<string>, int> minPQ = new PriorityQueue<GraphNode<string>, int>();
            HashSet<GraphNode<string>> visited = new HashSet<GraphNode<string>>();

            totalCosts.Add(graph.nodeSet.Find("Start"), 0); 
            minPQ.Enqueue( graph.nodeSet.Find("Start"), 0);
              

            foreach (GraphNode<string> x in graph.Nodes)
            {
                if (x.Value != "Start")
                {
                    totalCosts.Add(x, double.PositiveInfinity);  
                }
            }

            while (minPQ.Count >= 1)
            {
                GraphNode<string> newSmallest = minPQ.Dequeue();

                int index = 0;

                foreach (GraphNode<string> connectedNode in newSmallest.ConnectedNodes) 
                {
                    if (!visited.Contains(connectedNode))
                    {
                        double altPath = totalCosts.GetValueOrDefault(newSmallest) + newSmallest.Costs.ElementAt(index); 
                        int altPathInt = Convert.ToInt32(altPath);
                        if (altPath < totalCosts.GetValueOrDefault(connectedNode))
                        {
                            totalCosts.Remove(connectedNode);
                            previousNodes.Remove(connectedNode);
                            totalCosts.Add(connectedNode, altPath);
                            previousNodes.Add(connectedNode, newSmallest);
                            minPQ.Enqueue(connectedNode, altPathInt);
                        }
                    }
                    visited.Add(newSmallest);
                    index++;
                }
            }

            List<Object> results = new List<Object>();
            results.Add(totalCosts);
            results.Add(previousNodes);
            return results;
        }

        public static void Main(string[] args)
        {
            Graph<string> graph = new Graph<string>();

            var Start = new GraphNode<string>("Start");
            var A = new GraphNode<string>("A");
            var B = new GraphNode<string>("B");
            var C = new GraphNode<string>("C");
            var D = new GraphNode<string>("D");
            var E = new GraphNode<string>("E");
            var F = new GraphNode<string>("F");
            var G = new GraphNode<string>("G");
            var H = new GraphNode<string>("H");
            var End = new GraphNode<string>("End");

            graph.AddNode(Start);
            graph.AddNode(A);
            graph.AddNode(B);
            graph.AddNode(C);
            graph.AddNode(D);
            graph.AddNode(E);
            graph.AddNode(F);
            graph.AddNode(G);
            graph.AddNode(H);
            graph.AddNode(End);
            
            graph.AddUndirectedEdge(Start, A, 3);
            graph.AddUndirectedEdge(A, D, 12);
            graph.AddUndirectedEdge(A, C, 15);
            graph.AddUndirectedEdge(A, B, 11);
            graph.AddUndirectedEdge(B, C, 2);
            graph.AddUndirectedEdge(D, C, 12);
            graph.AddUndirectedEdge(C, G, 16);
            graph.AddUndirectedEdge(C, H, 15);
            graph.AddUndirectedEdge(D, E, 9);
            graph.AddUndirectedEdge(D, F, 14);
            graph.AddUndirectedEdge(H, E, 6);
            graph.AddUndirectedEdge(E, F, 12);
            graph.AddUndirectedEdge(G, H, 6);
            graph.AddUndirectedEdge(G, End, 18);
            graph.AddUndirectedEdge(H, End, 10);
            graph.AddUndirectedEdge(F, End, 6);

            Dijkstra x = new Dijkstra();

            var temp = x.Results(graph, Start);

            Dictionary<GraphNode<string>,double> costs = (Dictionary<GraphNode<string>, double> )temp.ElementAt(0);
            Dictionary<GraphNode<string>, GraphNode<string>> paths = (Dictionary<GraphNode<string>, GraphNode<string>>)temp.ElementAt(1);

            foreach(var cost in costs)
            {
                Console.WriteLine("The path from " + Start.Value + " to " + cost.Key.Value + " costs " + cost.Value);
                List<string> path = new List<string>();

                GraphNode<string> currentPosition = cost.Key;

                while(!currentPosition.Value.Equals(Start.Value))
                {
                    path.Add(currentPosition.Value);
                    currentPosition = paths.GetValueOrDefault(currentPosition);
                }
                path.Reverse();
                bool first = true;
                foreach(var node in path)
                {
                    if (!first)
                    {
                        Console.Write(" , ");
                    }
                    Console.Write(node);
                    first = false;
                }
                Console.WriteLine();
            }

            


        }
    }
}