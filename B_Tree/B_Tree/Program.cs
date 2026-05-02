namespace B_Tree;

public class BTree
{
    public class Node
    {
        public int[] Keys;
        public Node[] Children;
        public int KeyCount; // number of keys
        public bool IsLeaf;
        public Node(int m, bool isLeaf = true)
        {
            Children = new Node[m + 1];
            Keys = new int[m];
            IsLeaf = isLeaf;
            KeyCount = 0;
        }
    }
    public Node Root;
    public int m;
    public BTree(int m)
    {
        this.m = m;
        Root = new Node(m);
    }
    private void SplitChild(Node parent, int i, Node fullChild)
    {
        // parent will receive the median key
        // i == index of fullChild
        // fullChild n = m-1 and need split
        // |10|20|30| example
        // 1. Create new sibling node
        var sibling = new Node(m, fullChild.IsLeaf);
        // 2. How many keys to move 
        int t = m / 2;
        // assign number of keys in sibling
        sibling.KeyCount = (m - 1) - t;
        // 3. Move the last(t-1) keys from fullChild
        // to sibling
        for (int j = 0; j < sibling.KeyCount; j++)
        {
            sibling.Keys[j] = fullChild.Keys[j + t];
        }
        // 4. If fullChild is not a leaf
        // move its children too
        if (!fullChild.IsLeaf)
        {
            for (int j = 0; j < sibling.KeyCount + 1; j++)
            {
                sibling.Children[j] = fullChild.Children[j + t];
                fullChild.Children[j + t] = null;
            }
        }
        // 5.Reduce the number of keys in fullChild
        fullChild.KeyCount = t - 1;
        // 6. Shift parent's children to make room
        // for sibling
        for (int j = parent.KeyCount; j >= i + 1; j--)
        {
            parent.Children[j + 1] = parent.Children[j];
        }
        parent.Children[i + 1] = sibling;
        // 7. Shift parent's keys to take the median key from fullChild
        for (int j = parent.KeyCount - 1; j >= i; j--)
        {
            parent.Keys[j + 1] = parent.Keys[j];
        }
        parent.Keys[i] = fullChild.Keys[t - 1]; // The Median

        parent.KeyCount++;
    }
    public void Insert(int key)
    {
        if (Root == null)
        {
            Root = new Node(m, true);
            Root.Keys[0] = key;
            Root.KeyCount = 1;
            return;
        }
        // pointer to root
        var r = Root;
        if (r.KeyCount == m - 1)
        // Check if Root is full
        {
            // new Parent
            var s = new Node(m, false);
            // new parent as root
            Root = s;
            // set old root as first child
            // of new root
            s.Children[0] = r;
            // split the old full root
            SplitChild(s, 0, r);
            // insert on non full tree
            InsertNonFull(s, key);
        }
        else
        {
            // insert on non full tree
            InsertNonFull(r, key);
        }
    }
    private void InsertNonFull(Node node, int key)
    {
        //Initialize i: Start at the rightmost key index (node.KeyCount - 1).
        var i = node.KeyCount - 1;
        //Case 1: The Node is a Leaf
        // |10|20|30|40| |
        if (node.IsLeaf)
        {
            // Shift all keys greater than key one position to the right.
            while (i >= 0 && node.Keys[i] > key)
            {
                node.Keys[i + 1] = node.Keys[i];
                i--;
            }
            // Insert k into its sorted position. i+1
            node.Keys[i + 1] = key;
            // Increment the key count (node.KeyCount).
            node.KeyCount++;
        }
        // Case 2: The Node is NOT a Leaf internal node
        else
        {
            // Find the correct child C that should
            // contain key by comparing key with keys in node.
            //1
            while (i >= 0 && node.Keys[i] > key)
            {
                i--;
            }
            //Check ahead: Is the child C full (C.KeyCount == m-1)?
            if (node.Children[i + 1].KeyCount == m - 1)
            {
                //If Yes: Call SplitChild(node, i, Child). 
                //After the split, determine 
                // which of the two new children is the correct one for k.

                SplitChild(node, i + 1, node.Children[i + 1]);
                if (node.Keys[i + 1] < key)
                {
                    i++;
                }
            }
            //Recurse: Call InsertNonFull on the (now guaranteed non-full) child.
            InsertNonFull(node.Children[i + 1], key);
        }
    }
    public (Node node, int index)? Search(int key)
    {
        return Root == null ? null : Search(Root, key);
    }
    private (Node node, int index)? Search(Node root, int key)
    {
        int i = 0;
        while (i < root.KeyCount && key > root.Keys[i])
        {
            i++;
        }
        if (i < root.KeyCount && key == root.Keys[i])
        {
            return (root, i);
        }
        if (root.IsLeaf)
        {
            return null;
        }
        return Search(root.Children[i], key);
    }
    // Helper method Fill
    private void Fill(Node parentNode, int targetIndex)
    {
        Node childToFill = parentNode.Children[targetIndex];
        var thershold = Math.Ceiling(m / 2.0) - 1;
        // 1. check left sibling spare

        if (targetIndex != 0)
        {
            var leftSibling = parentNode.Children[targetIndex - 1];
            if (leftSibling.KeyCount > thershold)
            {
                BorrowFromPrev(parentNode, targetIndex);
                return;
            }
        }
        // 2. check right sibling spare
        if (targetIndex < parentNode.KeyCount)
        {
            var rightSibling = parentNode.Children[targetIndex + 1];
            if (rightSibling.KeyCount > thershold)
            {
                BorrowFromNext(parentNode, targetIndex);
                return;
            }
        }
        // 3. Both siblings are poor 
        if (targetIndex < parentNode.KeyCount)
            // merge right sibling
            Merge(parentNode, targetIndex);
        else
        {
            // merge left sibling
            Merge(parentNode, targetIndex - 1);
        }
    }
    private void Merge(Node parentNode, int targetIndex)
    {
        Node leftChild = parentNode.Children[targetIndex];
        Node rightChild = parentNode.Children[targetIndex + 1];

        // Keep track of where the parent key goes
        int parentKeyIndex = leftChild.KeyCount;

        // 1. Move the parent key into the left child
        // In a B-Tree of order m, threshold is (m/2)-1. 
        // After merge, the node will have (threshold + threshold + 1) keys.
        leftChild.Keys[parentKeyIndex] = parentNode.Keys[targetIndex];


        // 2. Copy Right Child's keys into Left Child
        for (int i = 0; i < rightChild.KeyCount; i++)
        {
            leftChild.Keys[parentKeyIndex + i + 1] = rightChild.Keys[i];
        }

        // 3. Copy Right Child's child pointers (if it's not a leaf)
        if (!leftChild.IsLeaf)
        {
            for (int i = 0; i <= rightChild.KeyCount; i++)
            {
                leftChild.Children[parentKeyIndex + i + 1] = rightChild.Children[i];
            }
        }
        // Update the count for the newly fused node
        leftChild.KeyCount += rightChild.KeyCount + 1;

        // 4. Shrink the Parent
        // Shift parent keys to the left
        for (int i = targetIndex; i < parentNode.KeyCount - 1; i++)
        {
            parentNode.Keys[i] = parentNode.Keys[i + 1];
        }
        // Shift parent child pointers to the left
        for (int i = targetIndex + 1; i < parentNode.KeyCount; i++)
        {
            parentNode.Children[i] = parentNode.Children[i + 1];
        }

        parentNode.KeyCount--;
    }
    private void BorrowFromNext(Node parentNode, int targetIndex)
    {
        Node child = parentNode.Children[targetIndex];
        Node rightSibling = parentNode.Children[targetIndex + 1];

        // 1. Parent key moves down to the end of the child
        child.Keys[child.KeyCount] = parentNode.Keys[targetIndex];

        // 2. Right sibling's first child moves to the end of the child (if not a leaf)
        if (!child.IsLeaf)
        {
            child.Children[child.KeyCount + 1] = rightSibling.Children[0];
        }
        // 3. Right sibling's first key moves up to the parent
        parentNode.Keys[targetIndex] = rightSibling.Keys[0];

        // 4. Update key counts
        child.KeyCount++;
        rightSibling.KeyCount--;

        // 5. Shift everything in rightSibling to the left to fill the gap
        for (int i = 0; i < rightSibling.KeyCount; i++)
        {
            rightSibling.Keys[i] = rightSibling.Keys[i + 1];
        }

        if (!rightSibling.IsLeaf)
        {
            for (int i = 0; i <= rightSibling.KeyCount; i++)
            {
                rightSibling.Children[i] = rightSibling.Children[i + 1];
            }
        }
    }
    private void BorrowFromPrev(Node parentNode, int targetIndex)
    {
        Node child = parentNode.Children[targetIndex];
        Node leftSibling = parentNode.Children[targetIndex - 1];

        // 1. Shift all keys in child to the right by 1 to make room at index 0
        for (int i = child.KeyCount - 1; i >= 0; i--)
        {
            child.Keys[i + 1] = child.Keys[i];
        }

        // 2. Shift all child pointers in child to the right by 1
        if (!child.IsLeaf)
        {
            for (int i = child.KeyCount; i >= 0; i--)
            {
                child.Children[i + 1] = child.Children[i];
            }
        }
        // 3. Parent key moves down into child's first slot
        child.Keys[0] = parentNode.Keys[targetIndex - 1];

        // 4. Sibling's last child becomes child's first child
        if (!child.IsLeaf)
        {
            child.Children[0] = leftSibling.Children[leftSibling.KeyCount];
        }

        // 5. Sibling's last key moves up to the parent
        parentNode.Keys[targetIndex - 1] = leftSibling.Keys[leftSibling.KeyCount - 1];

        // 6. Update counts
        child.KeyCount++;
        leftSibling.KeyCount--;
    }
    public bool Delete(int key)
    {
        if (Root == null) return false;

        // Start the recursive deletion
        bool deleted = Delete(Root, key);

        // If the root has 0 keys after deletion, 
        // it means it was merged. Update the root.
        if (Root.KeyCount == 0 && !Root.IsLeaf)
        {
            Root = Root.Children[0];
        }
        return deleted;
    }
    private bool Delete(Node node, int key)
    {
        int i = 0;
        // Find the first key greater than or equal to 'key'
        while (i < node.KeyCount && node.Keys[i] < key)
        {
            i++;
        }

        // CASE 1: The key is in THIS node
        if (i < node.KeyCount && node.Keys[i] == key)
        {
            if (node.IsLeaf)
                DeleteFromLeaf(node, i);
            else
                DeleteFromNonLeaf(node, i);
            return true;
        }
        // CASE 2: The key is NOT in this node
        else
        {
            // If it's a leaf, the key doesn't exist in the tree
            if (node.IsLeaf) return false;

            // CRITICAL: Before going down, check if the child is "too thin"
            var threshold = (int)Math.Ceiling(m / 2.0) - 1;
            if (node.Children[i].KeyCount <= threshold)
            {
                // Use the Fill function we built to beef up the child!
                Fill(node, i);

                // After Fill, the key might have moved. 
                // We re-adjust 'i' if the key we want is now in a different child.
                if (i > node.KeyCount) i--;
            }

            // Recurse down to the (now guaranteed safe) child
            return Delete(node.Children[i], key);
        }
    }
    private void DeleteFromLeaf(Node node, int index)
    {
        for (int i = index + 1; i < node.KeyCount; i++)
        {
            node.Keys[i - 1] = node.Keys[i];
        }
        node.KeyCount--;
    }
    private void DeleteFromNonLeaf(Node node, int index)
    {
        int key = node.Keys[index];
        var threshold = (int)Math.Ceiling(m / 2.0) - 1;

        // Try to get a replacement from the left child
        if (node.Children[index].KeyCount > threshold)
        {
            int pred = GetPredecessor(node, index);
            node.Keys[index] = pred;
            Delete(node.Children[index], pred);
        }
        // Try to get a replacement from the right child
        else if (node.Children[index + 1].KeyCount > threshold)
        {
            int succ = GetSuccessor(node, index);
            node.Keys[index] = succ;
            Delete(node.Children[index + 1], succ);
        }
        // Both children are poor: Merge them, then delete from the merged node
        else
        {
            Merge(node, index);
            Delete(node.Children[index], key);
        }
    }
    private int GetPredecessor(Node node, int index)
    {
        // Go to the left child
        Node current = node.Children[index];

        // Keep moving to the right-most child until you hit a leaf
        while (!current.IsLeaf)
        {
            current = current.Children[current.KeyCount];
        }

        // The predecessor is the last key in that leaf
        return current.Keys[current.KeyCount - 1];
    }
    private int GetSuccessor(Node node, int index)
    {
        // Go to the right child
        Node current = node.Children[index + 1];

        // Keep moving to the left-most child (index 0) until you hit a leaf
        while (!current.IsLeaf)
        {
            current = current.Children[0];
        }
        // The successor is the first key in that leaf
        return current.Keys[0];
    }
    public void PrintTreeAdvanced()
    {
        Console.WriteLine();
        PrintNodeAdvanced(Root, "", true);
    }
    private void PrintNodeAdvanced(Node node, string indent, bool isLast)
    {
        if (node == null) return;

        // Print current node
        Console.Write(indent);

        if (isLast)
        {
            Console.Write("  └── ");
            indent += "      ";
        }
        else
        {
            Console.Write("  ├── ");
            indent += "  │   ";
        }

        // Print keys
        Console.Write("[ ");
        for (int i = 0; i < node.KeyCount; i++)
        {
            Console.Write(node.Keys[i]);
            if (i < node.KeyCount - 1) Console.Write(" | ");
        }
        Console.WriteLine(" ]");

        // If leaf stop
        if (node.IsLeaf) return;

        // Traverse children
        for (int i = 0; i <= node.KeyCount; i++)
        {
            bool childIsLast = (i == node.KeyCount);
            PrintNodeAdvanced(node.Children[i], indent, childIsLast);
        }
    }
    public int CountKeys()
    {
        return CountKeys(Root);
    }
    private int CountKeys(Node node)
    {
        if (node == null) return 0;

        int count = node.KeyCount;

        if (!node.IsLeaf)
        {
            for (int i = 0; i <= node.KeyCount; i++)
            {
                count += CountKeys(node.Children[i]);
            }
        }
        return count;
    }
}
public static class Program
{
    static void p<T>(T t, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.Write($"   {t}");
        Console.ForegroundColor = ConsoleColor.White;
    }
    static void p() => Console.WriteLine();
    public static void DelTest(BTree bt)
    {
        if (bt == null)
        {
            p("Unintialized tree.", ConsoleColor.Red);
            p();
            return;
        }
        if (Get_Int("Enter key to Delete: ", 0, 900) is not int key)
            return;
        bool result = bt.Delete(key);
        if (result)
        {
            p($"Key {key} has been deleted successfully.", ConsoleColor.Green);
            p();
        }
        else
        {
            p($"Key {key} not found in the B-Tree!", ConsoleColor.Red);
            p();
        }
    }
    public static void SearchTest(BTree bt)
    {
        if (bt == null)
        {
            p("Unintialized tree.", ConsoleColor.Red);
            p();
            return;
        }
        if (Get_Int("Enter a key to search: ", 0, 900) is not int key)
            return;
        var result = bt.Search(key);
        if (result.HasValue)
        {
            var activeKeys = result.Value.node.Keys.Take(result.Value.node.KeyCount);
            p($"Key {key} found in node with keys:\n\t\t [ {string.Join(" | ", activeKeys)} ] at index {result.Value.index}", ConsoleColor.Green);
        }
        else
        {
            p($"Key {key} not found in the B-Tree!", ConsoleColor.Red);
        }
        p();
    }
    static BTree IntializeTest()
    {

        if (Get_Int("Enter B-Tree degree Min= 4, Max = 8: ", 4, 8) is not int m)
            return null;
        var bt = new BTree(m);
        p("Tree has been initialized successfully.", ConsoleColor.Green);
        p();
        return bt;
    }
    static BTree BulkInsertionTest(BTree bt)
    {
        if (bt == null)
        {
            p("Unintialized tree.", ConsoleColor.Red);
            p();
            return  null;
        }
        if (Get_Int("Enter key count: Min = 1, Max = 80: ", 1, 80) is not int keycount)
            return null;
        p("Random Keys? Y/N: ");
        string input = Console.ReadLine();
        bool random = input.ToLower() == "y" ? true : false;
        
        var rand = new Random();
        for (int i = 0; i < keycount; i++)
        {
            var x = random ? rand.Next(0, 900) : i;
            bt.Insert(x);
        }
        p("Bulk Insertion done successfully.", ConsoleColor.Green);
        p();

        return bt;
    }
    static void PrintTest(BTree bt)
    {
        if (bt == null)
        {
            p("Unintialized tree.", ConsoleColor.Red);
            p();
            return;
        }
        bt.PrintTreeAdvanced();
    }
    static void TotalKeyTest(BTree bt)
    {
        if (bt == null)
        {
            p("Unintialized tree.", ConsoleColor.Red);
            p();
            return;
        }
        p($"B-Tree total key count: {bt.CountKeys()}"); p();
    }
    static void InsertTest(BTree bt)
    {
        if (bt == null)
        {
            p("Unintialized tree.", ConsoleColor.Red);
            p();
            return;
        }
        if (Get_Int("Enter a key to insert: ") is not int key)
            return;
        bt.Insert(key);
        p($"Key {key} has been inserted successfully.", ConsoleColor.Green);
        p();
    }

    static void MainMenu()
    {
        BTree bt = null;

        string input = "";
        while (true)
        {
           
            p("1- Initialize B-Tree with N Max Degree."); p();
            p("2- Bulk Insertion."); p();
            p("3- Insert key");p();
            p("4- Print the tree."); p();
            p("5- Search for key."); p();
            p("6- Delete key."); p();
            p("7- Get total key count."); p();
            p("8- Exit."); p();
            p("============"); p();
            p(".. ");

            input = Console.ReadLine();
            bool sucess = int.TryParse(input, out int result);
            if (sucess)
            {
                switch (result)
                {
                    case 1:
                        bt = IntializeTest();
                        break;

                    case 2:
                        BulkInsertionTest(bt);
                        break;
                    case 3:
                        InsertTest(bt);
                        break;
                    case 4:
                        PrintTest(bt);
                        break;

                    case 5:
                        SearchTest(bt);
                        break;

                    case 6:
                        DelTest(bt);
                        break;
                    case 7:
                        TotalKeyTest(bt);
                        break;
                    case 8:
                        return;
                    default:
                        p("Undefiend Command!", ConsoleColor.Red); p();
                        break;
                }
                ;
            }
            else
            {
                p("Invalid input. Please enter a valid choice.", ConsoleColor.Red); p();
            }
        }
    }
    static int? Get_Int(string msg, int min = 0, int max = int.MaxValue)
    {
        bool success;
        int result;
        do
        {
            p(msg);
            string input = Console.ReadLine();
            success = int.TryParse(input, out result);
            if (input.ToLower() == "back")
            {
                return null;
            }
            else if (!success)
            {
                p("Invalid Input.", ConsoleColor.Red); p();
            }
            else if (result < min || result > max)
            {
                success = false;
                p($"input must be greater than {min - 1} and less than {max + 1}!", ConsoleColor.Red);
                p();
            }
        }
        while (!success);
        return result;
    }
    public static void Main()
    {
        MainMenu();
    }
}