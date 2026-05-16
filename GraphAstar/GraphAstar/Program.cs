using System.ComponentModel.Design.Serialization;

namespace GraphAstar;

public class Graph
{
    public class Edge
    {
        public Node From { get; }
        public Node To { get; }
        public int W { get; }
        public Edge(Node from, Node to, int w)
        {
            From = from;
            To = to;
            W = w;
        }
    }
    public class Node
    {
        public string Name { get; init; }
        // Heuristic Distance
        public int H { get; set; }
        // Actual Distance
        public int G { get; set; } = int.MaxValue;
        public Node? Parent { get; set; }
        public Node(string name, int h)
        {
            Name = name;
            H = h;
        }
        public override string ToString() => Name;
    }
    private readonly Dictionary<Node, Dictionary<Node, int>> _adjacencyList;

    public Graph()
    {
        _adjacencyList = new();
    }
    public void AddVertex(Node v)
    {
        if (!_adjacencyList.ContainsKey(v))
        {
            _adjacencyList[v] = new();
        }
    }
    public void AddEdge(Node s, Node d, int w)
    {
        if (!_adjacencyList.ContainsKey(s)
        || !_adjacencyList.ContainsKey(d))
        {
            return;
        }
        _adjacencyList[s].Add(d, w);
        _adjacencyList[d].Add(s, w);
    }
    public void RemoveEdge(Node s, Node d)
    {
        if (_adjacencyList.ContainsKey(s) && _adjacencyList.ContainsKey(d))
        {
            _adjacencyList[s].Remove(d);
            _adjacencyList[d].Remove(s);
        }
    }
    public void RemoveVertex(Node v)
    {
        if (!_adjacencyList.ContainsKey(v)) return;

        // 1. Visit all neighbors and remove the connection to v
        foreach (var neighbor in _adjacencyList[v].Keys)
        {
            _adjacencyList[neighbor].Remove(v);
        }

        // 2. Delete the vertex itself
        _adjacencyList.Remove(v);
    }
    public bool HasEdge(Node s, Node d)
    {
        return _adjacencyList.ContainsKey(s) && _adjacencyList[s].ContainsKey(d);
    }
    public Dictionary<Node, int>? GetNeighbors(Node v)
    {
        return _adjacencyList.TryGetValue(v, out var value) ? value : null;
    }
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

            foreach (var n in _adjacencyList[current].Keys)
            {
                if (!visited.Contains(n))
                {
                    visited.Add(n);
                    queue.Enqueue(n);
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
            foreach (var n in _adjacencyList[current].Keys)
            {
                if (!visited.Contains(n))
                {

                    stack.Push(n);
                }
            }
        }
    }
    public int? GetWeight(Node s, Node d)
    {
        if (!_adjacencyList.ContainsKey(s)
        || !_adjacencyList.ContainsKey(d))
        {
            return null;
        }
        if (_adjacencyList[s].ContainsKey(d))
            return _adjacencyList[s][d];

        return null;

    }
    // helper method
    public static void p<T>(T t) => Console.Write($"   {t}");
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
                foreach (var neighbor in _adjacencyList[current].Keys)
                {
                    if (!visited.Contains(neighbor))
                    {
                        FindPathsRecursive(neighbor, destination, visited, currentPath, allPaths);
                    }
                }
            }
        }

        // backtracking: this is the "magic" part
        currentPath.RemoveAt(currentPath.Count - 1);
        visited.Remove(current);
    }
    //Dijkstra's algorithm to find the shortest path from s to d
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
        int l1 = 0, lA2 = 0, lB2 = 0, l3 = 0;

        foreach (var v in _adjacencyList.Keys)
        {
            l1++;
            distance[v] = int.MaxValue;
            previous[v] = null;
        }
        distance[s] = 0;
        pq.Enqueue(s, 0);
        while (pq.Count > 0)
        {
            lA2++;
            var current = pq.Dequeue();
            if (current == d) break;
            foreach (var neighbor in _adjacencyList[current])
            {
                lB2++;
                int alt = distance[current] + neighbor.Value;
                if (alt < distance[neighbor.Key])
                {
                    distance[neighbor.Key] = alt;
                    previous[neighbor.Key] = current;
                    pq.Enqueue(neighbor.Key, alt);
                }
            }
        }
        for (Node? at = d; at != null; at = previous.GetValueOrDefault(at))
        {
            l3++;
            path.Add(at);
        }
        path.Reverse();
        Console.Write("Dijkstra steps: ");
        int l2 = lA2 * lB2;
        Console.Write(l1 + l2 + l3);
        Console.WriteLine();

        return path.Count > 0 && path[0] == s ? path : new List<Node>();
    }
    // brute force implementation of Bellman-Ford algorithm
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
        int l1 = 0;
        int l2A = 0, l2B = 0, l2C = 0;

        int l3 = 0;
        int l4 = 0;


        foreach (var v in _adjacencyList.Keys)
        {
            l1++;
            distance[v] = int.MaxValue;
            previous[v] = null;
        }
        distance[s] = 0;
        var num_vertices = _adjacencyList.Count;


        for (int i = 1; i <= num_vertices - 1; i++)
        {
            l2A++;
            bool anyChange = false;
            foreach (var kvp in _adjacencyList)
            {
                l2B++;
                var from = kvp.Key;
                foreach (var neighbor in kvp.Value)
                {
                    l2C++;
                    var edge = new Edge(from, neighbor.Key, neighbor.Value);
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
            l3++;
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
            l4++;
            path.Add(at);
        }
        path.Reverse();
        Console.Write("BellMan-ford steps: ");
        int l2 = l2A * l2B * l2C;
        Console.Write(l1 + l2 + l3 + l4);
        Console.WriteLine();
        return path.Count > 0 && path[0] == s ? path : new List<Node>();
    }

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
        int lA1 = 0, lB1 = 0;

        int l2 = 0;
        while (openSet.Count > 0)

        {
            lA1++;
            var current = openSet.Dequeue();
            if (current == d) break;


            foreach (var (neighbor, weight) in _adjacencyList[current])
            {
                lB1++;
                int tentativeGScore = GScore[current] + weight;
                if (tentativeGScore < GScore.GetValueOrDefault(neighbor, int.MaxValue))
                {
                    cameFrom[neighbor] = current;
                    GScore[neighbor] = tentativeGScore;

                    int fScore = tentativeGScore + neighbor.H;
                    openSet.Enqueue(neighbor, fScore);
                }
            }
        }
        var path = new List<Node>();
        for (var at = d; at != null; at = cameFrom.GetValueOrDefault(at))
        {
            l2++;
            path.Add(at);
        }
        path.Reverse();
        Console.Write("A* steps: ");
        int l1 = lA1 * lB1;
        Console.Write(l1 + l2);
        Console.WriteLine();
        return path.Count > 0 && path[0] == s ? path : new List<Node>();
    }
}

public static class Program
{
    static void p<T>(T t) => Console.Write($"   {t}");
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








        graph.GetShortestPathDij(A, F).ForEach(s => p(s));
        Console.WriteLine();
        graph.GetShortestPathBell(A, F).ForEach(s => p(s));
        Console.WriteLine();
        graph.GetShoretestPathAstar(A, F).ForEach(s => p(s));


    }
}
