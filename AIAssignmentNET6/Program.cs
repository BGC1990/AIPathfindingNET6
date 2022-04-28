
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

    public class GraphNode<T> : Node<T>
    {
        private List<int> costs;

        public GraphNode() : base() { }
        public GraphNode(T value) : base(value) { }
        public GraphNode(T value, NodeList<T> connectedNodes) : base(value, connectedNodes) { }

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
        //possible to combine these two methods perhaps?
        public void AddNode(T value)
        {
            nodeSet.Add(new GraphNode<T>(value));
        }

        public void AddUndirectedEdge(GraphNode<T> from, GraphNode<T> to, int cost) //tinker with this
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

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    /*public class GenericDictionary
    {
        private Dictionary<string, double> _dict = new Dictionary<string, double>();

        public void Add<T>(T key, double value) where T : class
        {
            _dict.Add(key, value);
        }

        public T GetValue<T>(string key) where T : class
        {
            return _dict[key] as T;
        }
    }*/

    

    public class Dijkstra
    {
        public List<Object> Results(Graph<string> graph, GraphNode<string> node)
        {
            Dictionary<GraphNode<string>, double> totalCosts = new Dictionary<GraphNode<string>, double>();
            Dictionary<GraphNode<string>, GraphNode<string>> previousNodes = new Dictionary<GraphNode<string>, GraphNode<string>>();
            PriorityQueue<GraphNode<string>, int> minPQ = new PriorityQueue<GraphNode<string>, int>();
            HashSet<GraphNode<string>> visited = new HashSet<GraphNode<string>>();

            totalCosts.Add(graph.nodeSet.Find("Start"), 0); //not sure if strings are what is needed here
            minPQ.Enqueue( graph.nodeSet.Find("Start"), 0);
              

            foreach (GraphNode<string> x in graph.Nodes)
            {
                if (x.Value != "Start")
                {
                    totalCosts.Add(x, double.PositiveInfinity);   //add and put might not be the same thing
                }
            }

            while (minPQ.Count >= 1)
            {
                GraphNode<string> newSmallest = minPQ.Dequeue();

                int index = 0;

                foreach (GraphNode<string> connectedNode in newSmallest.ConnectedNodes) //thinks Start has no connected nodes??????????
                {
                    if (!visited.Contains(connectedNode))
                    {
                        double altPath = totalCosts.GetValueOrDefault(newSmallest) + newSmallest.Costs.ElementAt(index); //this might not be right
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
            // Console.WriteLine(x.Results(graph, Start));

            var temp = x.Results(graph, Start);

            Dictionary<GraphNode<string>,double> costs = (Dictionary<GraphNode<string>, double> )temp.ElementAt(0);
            Dictionary<GraphNode<string>, GraphNode<string>> paths = (Dictionary<GraphNode<string>, GraphNode<string>>)temp.ElementAt(1);

            foreach(var c in costs)
            {
                Console.WriteLine("The path from {0} to {1} Costs {2}", Start.Value, c.Key.Value, c.Value);
                List<string> path = new List<string>();

                GraphNode<string> currentPos = c.Key;

                while(!currentPos.Value.Equals(Start.Value))
                {
                    path.Add(currentPos.Value);
                    currentPos = paths.GetValueOrDefault(currentPos);
                }

                path.Reverse();
                bool first = true;
                foreach(var j in path)
                {

                    if (!first)
                    {
                        Console.Write(" , ");
                    }

                    Console.Write(" {0} ", j);

                    first = false;
                }

                Console.WriteLine();
            }

            


        }
    }
}