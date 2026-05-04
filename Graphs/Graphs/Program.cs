using System;
using System.Linq;
using System.Collections.Generic;
using Graphs;

namespace Graphs;
public class Graph
{
    private readonly Dictionary<string, Dictionary<string, int>> _adjacencyList;

    public Graph()
    {
        _adjacencyList = new();
    }
    public void AddVertex(string v)
    {
        if (!_adjacencyList.ContainsKey(v))
        {
            _adjacencyList[v] = new();
        }
    }
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
    public void RemoveEdge(string s, string d)
    {
        if (_adjacencyList.ContainsKey(s) && _adjacencyList.ContainsKey(d))
        {
            _adjacencyList[s].Remove(d);
            _adjacencyList[d].Remove(s);
        }
    }
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
    public bool HasEdge(string s, string d)
    {
        return _adjacencyList.ContainsKey(s) && _adjacencyList[s].ContainsKey(d);
    }
    public Dictionary<string, int>? GetNeighbors(string v)
    {
        return _adjacencyList.TryGetValue(v, out var value) ? value : null;
    }
    public void BreadFirstSearch(string v)
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
    public void DepthFirstSearch(string v)
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
    // helper method
    public static void p<T>(T t) => Console.Write($"   {t}");
    // very heavy method: get all paths from s to d using backtracking 
    public List<List<string>> GetAllPaths(string s, string d)
    {
        List<List<string>> allPaths = new();
        HashSet<string> visited = new();
        List<string> currentPath = new();

        FindPathsRecursive(s, d, visited, currentPath, allPaths);
        return allPaths;
    }
    // helper method for GetAllPaths: this is the backtracking part
    private void FindPathsRecursive(string current, string destination,
        HashSet<string> visited, List<string> currentPath, List<List<string>> allPaths)
    {
        visited.Add(current);
        currentPath.Add(current);

        if (current == destination)
        {
            // we must create a NEW list copy, otherwise it changes as we backtrack
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

        // backtracking: this is the "magic" part
        currentPath.RemoveAt(currentPath.Count - 1);
        visited.Remove(current);
    }
    //Dijkstra's algorithm to find the shortest path from s to d
    public List<string> GetShortestPathDij(string s, string d)
    {
        if (!_adjacencyList.ContainsKey(s) || !_adjacencyList.ContainsKey(d))
        {
            return new List<string>();
        }
        var distance = new Dictionary<string, int>();
        var previous = new Dictionary<string, string?>();
        var pq = new PriorityQueue<string, int>();
        var path = new List<string>();
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
            foreach (var neighbor in _adjacencyList[current])
            {
                int alt = distance[current] + neighbor.Value;
                if (alt < distance[neighbor.Key])
                {
                    distance[neighbor.Key] = alt;
                    previous[neighbor.Key] = current;
                    pq.Enqueue(neighbor.Key, alt);
                }
            }
        }
        for (string? at = d; at != null; at = previous.GetValueOrDefault(at))
        {
            path.Add(at);
        }
        path.Reverse();
        path.Add(distance[d].ToString());
        return path.Count > 0 && path[0] == s ? path : new List<string>();
    }
    private class Edge
    {
        public string From { get; }
        public string To {get;}
        public int W { get; }
        public Edge(string from, string to, int w)
        {
            From = from;
            To = to;
            W = w;
        }
    }
    public List<string> GetShortestPathBell(string s, string d)
    {
        var distance = new Dictionary<string, int>();
        var previous = new Dictionary<string, string?>();
        var path = new List<string>();
        var edges = new List<Edge>();
        foreach (var v in _adjacencyList.Keys)
        {
            distance[v] = int.MaxValue;
            previous[v] = null;
        }
        distance[s] = 0;
        var num_vertices = _adjacencyList.Count;
        
        foreach(var kvp in _adjacencyList)
        {
            var from = kvp.Key;
            foreach(var neighbor in kvp.Value)
            {
                edges.Add(new Edge(from, neighbor.Key, neighbor.Value));
            }
        }
        for(int i = 1; i <=num_vertices - 1; i++)
        {
            bool anyChange = false;
            foreach (var edge in edges)
            {
                if(distance[edge.From] != int.MaxValue)
                {
                var alt = distance[edge.From] + edge.W;
                if(alt < distance[edge.To])
                {
                    distance[edge.To] = alt;
                    previous[edge.To] = edge.From;
                    anyChange = true;
                }
                }
            }
            if(!anyChange) break;
        }
        foreach(var edge in edges)
        {
            if(distance[edge.From] != int.MaxValue)
            {
            if(distance[edge.From] + edge.W < distance[edge.To])
            {
                throw new Exception("Graph contains a negative weight cycle");
            }
            }
        }
        for(string at = d; at != null; at = previous.GetValueOrDefault(at)!)
        {
            path.Add(at);
        }
        path.Reverse();
        return path.Count > 0 && path[0] == s ? path : new List<string>();
    }
}
public static class Program
{
    static void p<T>(T t) => Console.Write($"   {t}");
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
        graph.AddVertex("Z");
        graph.AddVertex("X");


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


        graph.GetShortestPathDij("G", "A").ForEach(s => p(s));
        Console.WriteLine();
        graph.GetShortestPathBell("G", "A").ForEach(s => p(s));
        

    }
}
