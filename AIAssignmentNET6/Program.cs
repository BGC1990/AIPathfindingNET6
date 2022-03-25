
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AIAssignment
{
    public class Node<T>
    {
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

    public class NodeList<T> : Collection<Node<T>>
    {
        public NodeList() : base() { } //not sure if have to invoke the base class to construct

        public NodeList(int initSize)
        {
            for (int i = 0; i < initSize; i++)
            {
                base.Items.Add(default(Node<T>));
            }
        }

        public Node<T> Find(T contents)
        {
            foreach (Node<T> node in Items)
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

    public class GenericDictionary
    {
        private Dictionary<object, double> _dict = new Dictionary<object, double>();

        public void Add<T>(T key, double value) where T : class
        {
            _dict.Add(key, value);
        }

       // public T GetValue<T>(string key) where T : class
       // {
       //     return dict[key] as T;
      //  }
    }

    

    public class Dijkstra
    {
        public List<Object> Results(Graph<string> graph, GraphNode<string> node)
        {
            Dictionary<GraphNode<string>, double> totalCosts = new Dictionary<GraphNode<string>, double>();
            Dictionary<GraphNode<string>, GraphNode<string>> previousNodes = new Dictionary<GraphNode<string>, GraphNode<string>>();
            PriorityQueue<GraphNode<string>, int> minPQ = new PriorityQueue<GraphNode<string>, int>();
            HashSet<Node<string>> visited = new HashSet<Node<string>>();

            totalCosts.Add(new GraphNode<string>("Start"), 0); //not sure if strings are what is needed here
            minPQ.Enqueue(new GraphNode<string>("Start"), 0);
              

            foreach (GraphNode<string> x in graph.Nodes)
            {
                if (!x.Equals("Start"))
                {
                    totalCosts.Add(node, double.PositiveInfinity);   //add and put might not be the same thing
                }
            }

            while (minPQ.Count >= 1)
            {
                GraphNode<string> newSmallest = minPQ.Dequeue();

                foreach (GraphNode<string> connectedNode in newSmallest.ConnectedNodes)
                {
                    if (!visited.Contains(connectedNode))
                    {
                        double altPath = totalCosts.GetValueOrDefault(newSmallest) + totalCosts.GetValueOrDefault(newSmallest) + totalCosts.GetValueOrDefault(connectedNode); //this might not be right
                        int altPathInt = Convert.ToInt32(altPath);
                        if (altPath < totalCosts.GetValueOrDefault(connectedNode))
                        {
                            totalCosts.Add(connectedNode, altPath);
                            previousNodes.Add(connectedNode, newSmallest);

                            minPQ.Enqueue(connectedNode, altPathInt);
                        }
                    }
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
            graph.AddNode("Start");
            graph.AddNode("A");
            graph.AddNode("B");
            graph.AddNode("C");
            graph.AddNode("D");
            graph.AddNode("E");
            graph.AddNode("F");
            graph.AddNode("G");
            graph.AddNode("H");
            graph.AddNode("End");
            graph.AddUndirectedEdge("Start", "A", 3);
            graph.AddUndirectedEdge("A", "D", 12);
            graph.AddUndirectedEdge("A", "C", 15);
            graph.AddUndirectedEdge("A", "B", 11);
            graph.AddUndirectedEdge("B", "C", 2);
            graph.AddUndirectedEdge("D", "C", 12);
            graph.AddUndirectedEdge("C", "G", 16);
            graph.AddUndirectedEdge("C", "H", 15);
            graph.AddUndirectedEdge("G", "End", 18);
            graph.AddUndirectedEdge("H", "End", 10);
            Dijkstra x = new Dijkstra();
            x.Results(graph, "Start");
        }
    }
}