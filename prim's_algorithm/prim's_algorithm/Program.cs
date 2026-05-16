


using static prim_s_algorithm.Graph;

namespace prim_s_algorithm;

// Graph Data Structure: Undirected, Weighted Graph using Adjacency List
public class Graph
{
    public class Edge
    {
        public Node From { get; }
        public Node To { get; }
        public int W { get; }
        public bool Visited = false;
        public Edge(Node from, Node to, int w)
        {
            From = from;
            To = to;
            W = w;
        }
        public Edge(Node from, Node to)
        {
            From = from;
            To = to;
        }
        public Edge() { }

        public override string ToString()
        {
            return $"{From} <--{W}--> {To}";
        }
        public override bool Equals(object? obj)
        {
            if (obj is not Edge other) return false;
            bool sameDirection = Equals(From, other.From) && Equals(To, other.To);
            bool oppositeDirection = Equals(From, other.To) && Equals(To, other.From);
            return sameDirection || oppositeDirection;
        }
        public override int GetHashCode()
        {
            var first = From.GetHashCode();
            var second = To.GetHashCode();

            var h1 = Math.Min(first, second);
            var h2 = Math.Max(first, second);
            return HashCode.Combine(h1, h2, W);

        }

    }
    public class Node
    {
        public string Name { get; init; }
        // Heuristic Distance
        public int H { get; set; }
        // Actual Distance
        public int G { get; set; } = int.MaxValue;
        public Node(string name, int h)
        {
            Name = name;
            H = h;
        }
        public override string ToString() => Name;
    }
    private readonly Dictionary<Node, List<Edge>> _adjacencyList;
    public int VerticesCount => _adjacencyList.Count;


    public Graph()
    {
        _adjacencyList = new();

    }
    // Graph Operations
    public void AddVertex(Node v)
    {
        if (!_adjacencyList.ContainsKey(v))
        {
            _adjacencyList[v] = new();
        }
    }
    public void AddEdge(Node s, Node d, int w)
    {
        if (_adjacencyList.ContainsKey(s)
        && _adjacencyList.ContainsKey(d))
        {
            var esd = new Edge(s, d, w);
            var eds = new Edge(d, s, w);
            _adjacencyList[s].Add(esd);
            _adjacencyList[d].Add(eds);
        }

    }
    public void RemoveEdge(Node s, Node d)
    {
        if (_adjacencyList.ContainsKey(s) && _adjacencyList.ContainsKey(d))
        {
            var edge = new Edge();
            if (TryGetEdge(s, d, out edge))
            {


                _adjacencyList[s].Remove(edge);
                _adjacencyList[d].Remove(edge);
            }

        }
    }
    private void removeEdge(Edge edge, List<Edge> lst)
    {
        lst.Remove(edge);
    }
    public void RemoveVertex(Node v)
    {
        if (!_adjacencyList.ContainsKey(v)) return;

        // 1. Visit all neighbors and remove the connection to v
        var edges = _adjacencyList[v];
        for (int i = 0; i < edges.Count - 1; i++)
        {
            removeEdge(edges[i], _adjacencyList[v]);
        }

        // 2. Delete the vertex itself
        _adjacencyList.Remove(v);
    }
    public bool TryGetEdge(Node s, Node d, out Edge? edge)
    {
        if (!_adjacencyList.ContainsKey(s))
        {
            edge = null;
            return false;
        }
        edge = _adjacencyList[s].FirstOrDefault(e => e.To == d);

        return edge != null ? true : false;
    }
    public List<Edge>? GetNeighbors(Node v)
    {
        var nieghbors = new List<Edge>();
        foreach (var edge in _adjacencyList[v])
        {
            nieghbors.Add(edge);

        }
        return nieghbors;
    }

    public bool TryGetWeight(Node s, Node d, out int? weight)
    {
        if (!_adjacencyList.ContainsKey(s)
        || !_adjacencyList.ContainsKey(d))
        {
            weight = null;
            return false;
        }
        weight = _adjacencyList[s]?.FirstOrDefault(e => e.To == d)?.W;
        return weight.HasValue ? true : false;

    }


    // ============= Algorithms =============
    public void BreadFirstSearch(Node v)
    {
        if (!_adjacencyList.ContainsKey(v)) return;

        HashSet<Node> visited = new();
        Queue<Node> queue = new();
        visited.Add(v);
        queue.Enqueue(v);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            p(current);

            foreach (var n in _adjacencyList[current])
            {
                if (!visited.Contains(n.To))
                {
                    visited.Add(n.To);
                    queue.Enqueue(n.To);
                }
            }
        }
    }
    public void DepthFirstSearch(Node v)
    {
        if (!_adjacencyList.ContainsKey(v)) return;

        HashSet<Node> visited = new();
        Stack<Node> stack = new();
        stack.Push(v);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (!visited.Contains(current))
            {
                visited.Add(current);
                p(current);
            }
            foreach (var e in _adjacencyList[current])
            {
                if (!visited.Contains(e.To))
                {

                    stack.Push(e.To);
                }
            }
        }
    }
    // very heavy method: get all paths from s to d using backtracking 
    public List<List<Node>> GetAllPaths(Node s, Node d)
    {
        List<List<Node>> allPaths = new();
        HashSet<Node> visited = new();
        List<Node> currentPath = new();

        FindPathsRecursive(s, d, visited, currentPath, allPaths);
        return allPaths;
    }
    // helper method for GetAllPaths: this is the backtracking part
    private void FindPathsRecursive(Node current, Node destination,
        HashSet<Node> visited, List<Node> currentPath, List<List<Node>> allPaths)
    {
        visited.Add(current);
        currentPath.Add(current);

        if (current == destination)
        {
            // we must create a NEW list copy, otherwise it changes as we backtrack
            allPaths.Add(new List<Node>(currentPath));
        }
        else
        {
            if (_adjacencyList.ContainsKey(current))
            {
                foreach (var edge in _adjacencyList[current])
                {
                    if (!visited.Contains(edge.To))
                    {
                        FindPathsRecursive(edge.To, destination, visited, currentPath, allPaths);
                    }
                }
            }
        }

        // backtracking: this is the "magic" part
        currentPath.RemoveAt(currentPath.Count - 1);
        visited.Remove(current);
    }
    // ============= Dijkstra's algorithm =============
    public List<Node> GetShortestPathDij(Node s, Node d)
    {
        if (!_adjacencyList.ContainsKey(s) || !_adjacencyList.ContainsKey(d))
        {
            return new List<Node>();
        }
        var distance = new Dictionary<Node, int>();
        var previous = new Dictionary<Node, Node?>();
        var pq = new PriorityQueue<Node, int>();
        var path = new List<Node>();


        foreach (var v in _adjacencyList.Keys)
        {

            distance[v] = int.MaxValue;
            previous[v] = null;
        }
        distance[s] = 0;
        pq.Enqueue(s, 0);
        while (pq.Count > 0)
        {

            var current = pq.Dequeue();
            if (current == d) break;
            foreach (var edge in _adjacencyList[current])
            {

                int alt = distance[current] + edge.W;
                if (alt < distance[edge.To])
                {
                    distance[edge.To] = alt;
                    previous[edge.To] = current;
                    pq.Enqueue(edge.To, alt);
                }
            }
        }
        for (Node? at = d; at != null; at = previous.GetValueOrDefault(at))
        {

            path.Add(at);
        }
        path.Reverse();


        return path.Count > 0 && path[0] == s ? path : new List<Node>();
    }
    // ============= Bell-man ford algorithm =============
    public List<Node> GetShortestPathBell(Node s, Node d)
    {
        if (!_adjacencyList.ContainsKey(s) || !_adjacencyList.ContainsKey(d))
        {
            return new List<Node>();
        }
        var distance = new Dictionary<Node, int>();
        var previous = new Dictionary<Node, Node?>();
        var path = new List<Node>();
        var edges = new List<Edge>();


        foreach (var v in _adjacencyList.Keys)
        {

            distance[v] = int.MaxValue;
            previous[v] = null;
        }
        distance[s] = 0;
        var num_vertices = _adjacencyList.Count;


        for (int i = 1; i <= num_vertices - 1; i++)
        {

            bool anyChange = false;
            foreach (var kvp in _adjacencyList)
            {

                var from = kvp.Key;
                foreach (var edge in kvp.Value)
                {


                    if (distance[edge.From] != int.MaxValue)
                    {
                        var alt = distance[edge.From] + edge.W;
                        if (alt < distance[edge.To])
                        {
                            distance[edge.To] = alt;
                            previous[edge.To] = edge.From;
                            anyChange = true;
                        }
                    }
                }
                if (!anyChange) break;
            }
        }
        foreach (var edge in edges)
        {

            if (distance[edge.From] != int.MaxValue)
            {
                if (distance[edge.From] + edge.W < distance[edge.To])
                {
                    throw new Exception("Graph contains a negative weight cycle");
                }
            }
        }
        for (Node at = d; at != null; at = previous.GetValueOrDefault(at)!)
        {

            path.Add(at);
        }
        path.Reverse();

        return path.Count > 0 && path[0] == s ? path : new List<Node>();
    }
    // ============= A* algorithm =============
    public List<Node> GetShoretestPathAstar(Node s, Node d)
    {
        if (!_adjacencyList.ContainsKey(s) || !_adjacencyList.ContainsKey(d))
        {
            return new List<Node>();
        }
        var openSet = new PriorityQueue<Node, int>();
        // tracks the best g-score (cost from start) for each node
        var GScore = new Dictionary<Node, int>();
        GScore[s] = 0;
        // tracks the path: neighbor -> the node we came from
        var cameFrom = new Dictionary<Node, Node?>();
        openSet.Enqueue(s, s.H);

        while (openSet.Count > 0)

        {
            var current = openSet.Dequeue();
            if (current == d) break;


            foreach (var e in _adjacencyList[current])
            {
                int tentativeGScore = GScore[current] + e.W;
                if (tentativeGScore < GScore.GetValueOrDefault(e.To, int.MaxValue))
                {
                    cameFrom[e.To] = current;
                    GScore[e.To] = tentativeGScore;

                    int fScore = tentativeGScore + e.To.H;
                    openSet.Enqueue(e.To, fScore);
                }
            }
        }
        var path = new List<Node>();
        for (var at = d; at != null; at = cameFrom.GetValueOrDefault(at))
        {

            path.Add(at);
        }
        path.Reverse();

        return path.Count > 0 && path[0] == s ? path : new List<Node>();

    }
    // ============= Prim's algorithm =============
    public List<Edge> GetMSTPrim(Node startNode)
    {
        var mstEdges = new List<Edge>();
        var inMst = new HashSet<Node>();
        var pq = new PriorityQueue<Edge, int>();
        inMst.Add(startNode);
        foreach (var edge in _adjacencyList[startNode])
        {
            pq.Enqueue(edge, edge.W);
        }
        while(pq.Count > 0)
        {
            var currentEdge = pq.Dequeue();
            if (inMst.Contains(currentEdge.To)) continue;
            mstEdges.Add(currentEdge);
            inMst.Add(currentEdge.To);
            foreach(var edge in _adjacencyList[currentEdge.To])
            {
                if (!inMst.Contains(edge.To))
                {
                    pq.Enqueue(edge, edge.W);
                }
            }
        }
        int distance = 0;
        foreach (var edge in mstEdges)
        {
            distance += edge.W;
        }
        Console.WriteLine($"total distance:{distance}");
        return mstEdges;
    }
    public List<Edge> GetMSTKruskal()
    {
        var pq = new PriorityQueue<Edge, int>();
        var inKruskal = new HashSet<Edge>();
        var result = new List<Edge>();
        foreach(var vertex in _adjacencyList.Keys)
        {
            
            foreach (var edge in _adjacencyList[vertex])
            {
                if(!inKruskal.Contains(edge))
                {
                    pq.Enqueue(edge, edge.W);
                    inKruskal.Add(edge);
                }
            }
        }
        var parent = new Dictionary<Node, Node>();
        foreach(var vertex in _adjacencyList.Keys)
        {
            parent[vertex] = vertex;
        }
        Node Find(Node n)
        {
            if (parent[n] == n) return n;
            return parent[n] = Find(parent[n]);
        }
        while (pq.Count > 0 && result.Count < VerticesCount - 1)
        {
            var edge = pq.Dequeue();
            Node root1 = Find(edge.From);
            Node root2 = Find(edge.To);
            if(root1 != root2)
            {
                result.Add(edge);
                parent[root1] = root2;
            }
                
        }
        return result;
    }

    // helper method
    public void DisplayGraph(Graph g)
    {

        HashSet<Edge> hs = new HashSet<Edge>();
        foreach (var v in g._adjacencyList.Keys)
        {
            foreach (var e in g._adjacencyList[v])
            {
                hs.Add(e);
            }
        }
        foreach (var x in hs)
        {
            p(x); p();
        }
    }
    public void Display_Route(List<Node> route)
    {
        int totalDistance = 0;
        for (int i = 0; i < route.Count - 1; i++)
        {
            if (TryGetEdge(route[i], route[i + 1], out var edge))
            {
                p(edge);
                totalDistance += edge.W;
            }
        }
        p($"Total Distance: {totalDistance}");
    }
    public static void p() => Console.WriteLine();
    public static void p<T>(T t) => Console.Write($"   {t}");
}

public static class Program
{
    static void p<T>(T t) => Console.Write($"   {t}");
    static void p() => Console.WriteLine();
    public static void Main()
    {
        Graph graph = new();
        var A = new Graph.Node("A", 15);
        var A1 = new Graph.Node("A1", 15);
        var C = new Graph.Node("C", 12);
        var B = new Graph.Node("B", 10);
        var D = new Graph.Node("D", 8);
        var G = new Graph.Node("G", 10);
        var E = new Graph.Node("E", 4);
        var F = new Graph.Node("F", 0);


        graph.AddVertex(A);
        graph.AddVertex(A1);
        graph.AddVertex(B);
        graph.AddVertex(C);
        graph.AddVertex(D);
        graph.AddVertex(E);
        graph.AddVertex(F);
        graph.AddVertex(G);



        graph.AddEdge(A, A1, 5);
        graph.AddEdge(A, C, 4);

        graph.AddEdge(A1, B, 5);
        graph.AddEdge(A1, D, 10);
        graph.AddEdge(A1, C, 4);


        graph.AddEdge(C, D, 2);
        graph.AddEdge(C, G, 12);

        graph.AddEdge(D, B, 7);
        graph.AddEdge(D, G, 11);
        graph.AddEdge(D, E, 5);

        graph.AddEdge(B, E, 8);
        graph.AddEdge(B, F, 15);

        graph.AddEdge(E, F, 5);

        graph.AddEdge(F, G, 10);

        //graph.RemoveEdge(A, A1);


        //graph.GetNeighbors(A).ForEach(e=> p(e));

        p();
        p("Prim");
        //graph.Display_Route(graph.GetShoretestPathAstar(A, F));
        graph.GetMSTPrim(G).ForEach(e=> p(e));
        p();
        p("Kruskal");
        graph.GetMSTKruskal().ForEach(e => p(e));
    }
}

