using System.Text;

namespace Tries;

public class Trie
{
    private class Node
    {
        public Dictionary<char, Node> Children;
        public bool IsEndOfWord;
        public int PrefixCount { get; set; }
        public Node()
        {
            Children = new();
            IsEndOfWord = false;
        }
    }
    private Node root;
    public int WordCount => root.PrefixCount;
    public bool IsEmpty => root.PrefixCount == 0;
    public Trie()
    {
        root = new Node();
    }
    public bool Insert(string text)
    {
        if (string.IsNullOrEmpty(text)) return false;
        if (Search(text)) return false;
        var current = root;
        current.PrefixCount++;
        for (int i = 0; i < text.Length; i++)
        {

            if (!current.Children.TryGetValue(text[i], out var nextNode))
            {
                nextNode = new Node();
                current.Children[text[i]] = nextNode;

            }

            current = nextNode;
            current.PrefixCount++;

        }
        if (!current.IsEndOfWord)
        {
            current.IsEndOfWord = true;

        }
        return true;
    }
    public bool Insert(params string[] words)
    {
        if (words == null) return false;
        bool result = true;
        foreach (var word in words)
        {
            result = result && Insert(word);
        }
        return result;
    }
    public bool Insert(IEnumerable<string> words)
    {
        if (words == null) return false;
        bool result = true;

        foreach (var word in words)
        {
            result = result && Insert(word);
        }
        return result;
    }
    public bool Search(string word)
    {
        if (string.IsNullOrEmpty(word)) return false;
        var current = root;
        for (int i = 0; i < word.Length; i++)
        {
            if (!current.Children.TryGetValue(word[i], out var nextNode))
            {
                return false;
            }
            current = nextNode;
        }
        return current.IsEndOfWord;
    }
    public bool StartsWith(string word)
    {
        return FindNode(word) != null;
    }
    private Node? FindNode(string word)
    {
        if (string.IsNullOrEmpty(word)) return null;
        var current = root;
        for (int i = 0; i < word.Length; i++)
        {
            if (!current.Children.TryGetValue(word[i], out var nextNode))
            {
                return null;
            }
            current = nextNode;
        }
        return current;
    }
    public bool Delete(string word)
    {
        if (!Search(word)) return false;
        DeleteRecursive(root, word, 0);
        return true;

    }
    private bool DeleteRecursive(Node current, string word, int index)
    {
        current.PrefixCount--;
        if (index == word.Length)
        {
            current.IsEndOfWord = false;
            return current.Children.Count == 0;

        }
        var nextNode = current.Children[word[index]];

        if (DeleteRecursive(nextNode, word, index + 1))
        {
            current.Children.Remove(word[index]);

        }
        return !current.IsEndOfWord && current.Children.Count == 0;

    }
    public bool Delete(params string[] words)
    {
        if (words == null) return false;
        bool result = true;
        foreach (var word in words)
        {
            result = result && Delete(word);
        }

        return result;
    }
    public bool Delete(IEnumerable<string> words)
    {
        if (words == null) return false;
        bool result = true;
        foreach (var word in words)
        {
            result = result && Delete(word);
        }

        return result;
    }
    public bool Update(string oldWord, string newWord)
    {
        if (!Search(oldWord) || Search(newWord) || !Delete(oldWord))
        {
            return false;
        }

        if (!Insert(newWord))
        {
            Insert(oldWord);
            return false;

        }
        return true;
    }
    public List<string> GetWordsWithPrefix(string word)
    {
        var currentNode = FindNode(word);
        var result = new List<string>();
        if (currentNode == null)
        {
            return result;
        }
        else
        {
            var sb = new StringBuilder(word);
            TraverseAndCollect(currentNode, sb, result);

            return result;
        }
    }
    private void TraverseAndCollect(Node node, StringBuilder currentWord, List<string> result)

    {
        if (node.IsEndOfWord)
        {
            result.Add(currentWord.ToString());
        }

        foreach (var (c, nextNode) in node.Children)
        {
            currentWord.Append(c);
            TraverseAndCollect(nextNode, currentWord, result);
            currentWord.Length--;
        }

    }
    public int CountWordsWithPrefix(string prefix)
    {
        var current = FindNode(prefix);
        if (current == null)
        {
            return 0;
        }
        else return current.PrefixCount;
    }
    public void Clear()
    {
        root = new Node();
    }
}



public static class Program
{
    public static void p<T>(T t) => Console.Write($"   {t}");
    public static void Main()
    {
        var trie = new Trie();
        
        trie.Insert("Apple");
        trie.Insert("Apply");
        trie.Insert("Bananna");

        trie.Insert("App");

        p(trie.Search("App"));
        trie.GetWordsWithPrefix("App").ForEach(e => p(e));





    }
}




