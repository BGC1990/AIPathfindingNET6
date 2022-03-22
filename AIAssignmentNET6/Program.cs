
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
        private NodeList<T> nodeSet;

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
                if (x != "Start")
                {
                    totalCosts.Add(node, double.PositiveInfinity)    //add and put might not be the same thing
                }
            }

            while (!minPQ.Is())
            {
                Node newSmallest = minPQ.DelMin();

                foreach (Node connectedNode in newSmallest.ConnectedNodes)
                {
                    if (!visited.Contains(connectedNode))
                    {
                        int altPath = totalCosts.TryGetValue(newSmallest) + distance(newSmallest, connectedNode);

                        if (altPath < totalCosts.TryGetValue(connectedNode))
                        {
                            totalCosts.Add(connectedNode, altPath);
                            previousNodes.Add(connectedNode, newSmallest);

                            minPQ.d
                        }
                    }
                }
            }




            List<Object> results = new List<Object>();
            return results;
        }

        public static void Main(string[] args)
        {
            // Display the number of command line arguments.
            Console.WriteLine(args.Length);
        }
    }
}



/*public List Dijkstra(Graph graph, Node start)
{
    Dictionary<GraphNode, double> totalCosts = new Dictionary<Node, double>();
    Dictionary<Node, Node> previousNodes = new Dictionary<Node, Node>();
    MinPQ<Node> minPQ = new MinPQ<Node>();
    HashSet<Node> visited = new HashSet<Node>();
    totalCosts.Add(start, 0);
    minPQ.Add(start);
    foreach (Node node in graph.Nodes)
    {
        if (node != start)
        {
            totalCosts.Add(node, double.PositiveInfinity)
            }
    }
    while (!minPQ.IsEmpty())
    {
        Node newSmallest = minPQ.deleteMin();
    }
}
}
*/