namespace Disjoint_Set;

public class DisjointSet
{
    private readonly int[] parent;
    private readonly int[] rank;
    public DisjointSet(int size)
    {
        parent = new int[size];
        rank = new int[size];

        for (int i = 0; i < size; i++)
        {
            parent[i] = i;
            rank[i] = 0;
        }
    }
    public int Find(int x)
    {
        if (x != parent[x])
        {
            parent[x] = Find(parent[x]);
        }

        return parent[x];
    }
    public void Union(int a, int b)
    {
        int rootA = Find(a);
        int rootB = Find(b);

        if (rootA == rootB)
        {
            return;
        }

        if (rank[rootA] < rank[rootB])
        {
            parent[rootA] = rootB;
        }
        else if (rank[rootA] > rank[rootB])
        {
            parent[rootB] = rootA;
        }
        else
        {
            parent[rootB] = rootA;
            rank[rootA]++;
        }
    }
    public bool Connected(int a, int b)
    {
        return Find(a) == Find(b);
    }
}

public class Program
{
    static void Main(string[] args)
    {
        DisjointSet ds = new DisjointSet(5);
        ds.Union(0, 1);
        ds.Union(1, 2);
        ds.Union(3, 4);
        Console.WriteLine(ds.Connected(0, 2)); // True
        Console.WriteLine(ds.Connected(0, 3)); // False
        Console.WriteLine(ds.Connected(3, 4)); // True
    }

}


