
namespace Graphs;
// Undirected Weighted Graph Implementation
public class Graph
{
    // The graph is represented as an adjacency list,
    // where each vertex maps to a dictionary of its neighbors and the corresponding edge weights
    private readonly Dictionary<string, Dictionary<string, int>> _adjacencyList;
    public Graph()
    {
        _adjacencyList = new();
    }
    // Adds a vertex to the graph if it doesn't already exist
    public void AddVertex(string v)
    {
        if (!_adjacencyList.ContainsKey(v))
        {
            _adjacencyList[v] = new();
        }
    }
    // The graph is undirected, so we add the edge in both directions
    public void AddEdge(string s, string d, int w)
    {
        if (!_adjacencyList.ContainsKey(s)
        || !_adjacencyList.ContainsKey(d))
        {
            return;
        }
        _adjacencyList[s].Add(d, w);
        _adjacencyList[d].Add(s, w);
    }
    // Removes the edge between s and d if it exists
    public void RemoveEdge(string s, string d)
    {
        if (_adjacencyList.ContainsKey(s) && _adjacencyList.ContainsKey(d))
        {
            _adjacencyList[s].Remove(d);
            _adjacencyList[d].Remove(s);
        }
    }
    // Removes a vertex and all its associated edges
    public void RemoveVertex(string v)
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
    // Checks if an edge exists between s and d
    public bool HasEdge(string s, string d)
    {
        return _adjacencyList.ContainsKey(s) && _adjacencyList[s].ContainsKey(d);
    }
    // Returns the neighbors of a vertex along with the weights of the edges
    public Dictionary<string, int>? GetNeighbors(string v)
    {
        return _adjacencyList.TryGetValue(v, out var value) ? value : null;
    }
    // Breadth First Search using a queue (iterative)
    public void BFS(string v)
    {
        if (!_adjacencyList.ContainsKey(v)) return;
        HashSet<string> visited = new();
        Queue<string> queue = new();
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
    // Depth First Search using a stack (iterative)
    public void DFS(string v)
    {
        if (!_adjacencyList.ContainsKey(v)) return;
        HashSet<string> visited = new();
        Stack<string> stack = new();
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
    // Returns the weight of the edge between s and d if it exists, otherwise null
    public int? GetWeight(string s, string d)
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
    public static void p<T>(T t) => Console.WriteLine($"   {t}");
    // This is a backtracking algorithm to find all paths from s to d
    // Heavy & Discouraged
    public List<List<string>> GetAllPaths(string s, string d)
    {
        List<List<string>> allPaths = new();
        HashSet<string> visited = new();
        List<string> currentPath = new();
        FindPathsRecursive(s, d, visited, currentPath, allPaths);
        return allPaths;
    }
    private void FindPathsRecursive(string current, string destination,
        HashSet<string> visited, List<string> currentPath, List<List<string>> allPaths)
    {
        visited.Add(current);
        currentPath.Add(current);
        if (current == destination)
        {
            // We must create a NEW list copy, otherwise it changes as we backtrack
            allPaths.Add(new List<string>(currentPath));
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
        // BACKTRACK: This is the "magic" part
        currentPath.RemoveAt(currentPath.Count - 1);
        visited.Remove(current);
    }
}
public static class Program
{
    static void p<T>(T t) => Console.Write($"   {t}");
    static void p() => Console.WriteLine();
    public static void Main()
    {
        Graph graph = new();
        graph.AddVertex("A");
        graph.AddVertex("B");
        graph.AddVertex("C");
        graph.AddVertex("D");
        graph.AddVertex("E");
        graph.AddVertex("F");
        graph.AddVertex("G");
        graph.AddEdge("A", "B", 5);
        graph.AddEdge("A", "C", 4);
        graph.AddEdge("A", "D", 6);
        graph.AddEdge("B", "D", 7);
        graph.AddEdge("B", "E", 8);
        graph.AddEdge("C", "F", 12);
        graph.AddEdge("C", "D", 2);
        graph.AddEdge("D", "G", 11);
        graph.AddEdge("E", "G", 5);
        graph.AddEdge("F", "G", 10);
        graph.GetNeighbors("D")?.ToList().ForEach(neighbor => p(neighbor));
        graph.DFS("A");
        p();
        graph.BFS("A");
        p();
        
    }
}
