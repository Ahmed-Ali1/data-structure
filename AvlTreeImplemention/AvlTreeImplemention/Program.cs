

namespace BinaryTrees;

public class AvlTree<T>
{
    public class Node
    {
        public T Key;
        public int Height;
        public Node? Left, Right;
        public Node(T value)
        {
            Key = value;
            Height = 0;
            Left = null;
            Right = null;

        }
    }
    public Node? Root;
    public int Size { get; private set; }
    public T Min => MinValue(Root!);
    private static T MinValue(Node node)
    {
        if (node == null) throw new NullReferenceException();
        while (node.Left != null)
        {
            node = node.Left;
        }
        return node.Key;
    }
    public T Max => MaxValue(Root!);
    private static T MaxValue(Node node)
    {
        if (node == null) throw new NullReferenceException();
        while (node.Right != null)
        {
            node = node.Right;

        }
        return node.Key;
    }
    public AvlTree(T value)
    {
        Insert(value);
    }
    public AvlTree()
    {

    }
    private static void UpdateHeight(Node root)
    {
        if (root != null)
        {
            root.Height = Math.Max(GetHeight(root.Left), GetHeight(root.Right)) + 1;
        }
    }
    private static int GetHeight(Node root)
    {
        return (root == null ? -1 : root.Height);
    }
    public void Insert(T value)
    {
        Root = InsertRecursively(Root, value);

    }
    private Node InsertRecursively(Node? root, T value)
    {
        // Base Case
        if (root == null)
        {
            Size++;
            return new Node(value);
        }
        int cmp = Comparer<T>.Default.Compare(value, root.Key);
        // Navigate tree

        // case <
        if (cmp < 0)
            root.Left = InsertRecursively(root.Left, value);
        // case >
        else if (cmp > 0)
        {
            root.Right = InsertRecursively(root.Right, value);
        }
        // Update Heights after insertion
        //UpdateHeight(root);

        // case ==
        // ignore duplicates noting
        return Rebalance(root);
    }
    public bool Contains(T value)
    {
        return ContainsRecursively(Root, value);
    }
    private static bool ContainsRecursively(Node? root, T value)
    {
        // Base case 
        if (root == null) return false;
        // get comparision result
        int cmp = Comparer<T>.Default.Compare(value, root.Key);
        // Navigate Tree
        // Case <
        if (cmp < 0)
        {
            return ContainsRecursively(root.Left, value);
        }
        // Case >
        else if (cmp > 0)
        {
            return ContainsRecursively(root.Right, value);
        }
        // Case ==
        else
        {
            return true;
        }
    }
    public void Delete(T value)
    {
        Root = DeleteRecursively(Root, value);

    }
    private Node? DeleteRecursively(Node? root, T value)
    {
        // base case tree is empty
        if (root == null) return root;
        // navigation the tree
        // case <
        int cmp = Comparer<T>.Default.Compare(value, root.Key);

        if (cmp < 0)
        {
            root.Left = DeleteRecursively(root.Left, value);
        }
        // case >
        else if (cmp > 0)
        {
            root.Right = DeleteRecursively(root.Right, value);
        }
        // case == found the node
        else // handle deletion cases 
        {

            // node has 0 or 1 child
            if (root.Left == null || root.Right == null)
            {
                Size--;
                root = root.Left ?? root.Right;
            }

            else // node has 2 childs
            {
                // get the in order successor
                root.Key = MinValue(root.Right);
                root.Right = DeleteRecursively(root.Right, root.Key);
            }
            // check if the tree had only one node it's null now
            if (root == null) return null;
            // update heights  after deletion
            //UpdateHeight(root);
        }
        return Rebalance(root);
    }
    private static void TraverseInOrder(Node? node)
    {
        if (node != null)
        {
            TraverseInOrder(node.Left);
            Console.Write($"{node.Key} ");
            TraverseInOrder(node.Right);
        }
    }
    public void PrintInOrder() => TraverseInOrder(Root);
    private static void TraversePreOrder(Node? node)
    {
        if (node != null)
        {
            Console.Write($"{node.Key} ");
            TraversePreOrder(node.Left);
            TraversePreOrder(node.Right);
        }
    }
    public void PrintPreOrder() => TraversePreOrder(Root);

    private static void TraversePostOrder(Node? node)
    {
        if (node != null)
        {
            TraversePostOrder(node.Left);
            TraversePostOrder(node.Right);
            Console.Write($"{node.Key} ");
        }
    }
    public void PrintPostOrder() => TraversePostOrder(Root);


    private static Node RotateRight(Node root)
    {
        // capture the left child and its right subtree
        var x = root.Left;
        var t2 = x.Right;
        // assign new root and rearrange subtrees
        x.Right = root;
        root.Left = t2;
        // update heights
        UpdateHeight(root);
        UpdateHeight(x);

        return x!;
    }
    private static Node RotateLeft(Node root)
    {
        // capture the right child and its left subtree
        var x = root.Right;
        var t2 = x.Left;
        // assign new root and rearrange subtrees
        x.Left = root;
        root.Right = t2;
        // update heights 
        UpdateHeight(root);
        UpdateHeight(x);
        // return new sub-root
        return x;
    }

    private static int GetBalanceFactor(Node root)
    {
        // return the balace factor height(root.left)-hight(root.right)
        return GetHeight(root.Left) - GetHeight(root.Right);
    }
    private static Node Rebalance(Node root)
    {
        // call update height 
        UpdateHeight(root);
        // calculate balance factor
        int bf = GetBalanceFactor(root);

        if (bf > 1) // left heavy
        {
            if (GetBalanceFactor(root.Left) < 0)
            {
                root.Left = RotateLeft(root.Left); // lr
            }
            return RotateRight(root); // ll
        }
        if (bf < -1) // right heavy
        {
            if (GetBalanceFactor(root.Right) > 0)
            {
                root.Right = RotateRight(root.Right); // rl
            }
            return RotateLeft(root); // rr
        }

        // no rotation needed
        return root;
    }
    public void GetHeightt(Node root)
    {
        Console.WriteLine($"   {GetHeight(root)}");
    }
    public void Display()
    {
        Display(Root, "", true);
    }

    private void Display(Node? node, string indent, bool isLast)
    {
        if (node == null)
            return;

        Console.Write(indent);

        if (isLast)
        {

            Console.Write("└───");
            indent += "    ";
        }
        else
        {
            Console.Write("├───");
            indent += "│   ";
        }

        Console.WriteLine(node.Key);

        // Left first, then right (natural reading order)
        Display(node.Left, indent, false);
        Display(node.Right, indent, true);
    }

}
public static class Program
{
    public static void BFS(AvlTree<int>.Node root)
    {
        if (root == null) return;

        Queue<AvlTree<int>.Node> queue = new Queue<AvlTree<int>.Node>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            AvlTree<int>.Node current = queue.Dequeue();
            Console.Write(current.Key + " ");

            if (current.Left != null) queue.Enqueue(current.Left);
            if (current.Right != null) queue.Enqueue(current.Right);
        }
    }
    public static void p<T>(T t) => Console.Write($"   {t}");
    public static void p() => Console.WriteLine();

    public static void Main()
    {

        AvlTree<int> avl = new AvlTree<int>(5);
        avl.Insert(3);
        avl.Insert(4);
        avl.Insert(6);
        avl.Insert(8);
        avl.Insert(9);
        avl.Insert(7);
        avl.PrintInOrder();
        Console.WriteLine();
        avl.PrintPostOrder();
        Console.WriteLine();
        avl.PrintPreOrder();
        Console.WriteLine();
        BFS(avl.Root!);
        Console.WriteLine();
        avl.Display();
    }
}
