using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security;
using System.Text;

namespace Advanced_Data_Structure
{
    public class SegmentTree
    {
        private class Node
        {
            public int Value { get; set; }
            public int Lazy { get; set; }
            public Node? Left { get; set; }
            public Node? Right { get; set; }
            public Node(int value)
            {
                Value = value;
            }
            public Node(int value, Node? left, Node? right)
            {
                Value = value;
                Left = left;
                Right = right;
            }
        }
        private readonly List<Node> roots = new();
        private readonly int _n;
        public int VersionCount => roots.Count;
        public SegmentTree(int[] items)
        {
            _n = items.Length;
            roots.Add(Build(items, 0, _n - 1));
        }
        private static Node Build(int[] items, int start, int end)
        {
            if (start == end)
            {
                return new Node(items[start]);
            }
            else
            {
                int mid = start + (end - start) / 2;
                Node leftChild = Build(items, start, mid);
                Node rightChild = Build(items, mid + 1, end);
                Node root = new(leftChild.Value + rightChild.Value)
                {
                    Left = leftChild,
                    Right = rightChild,
                };
                return root;
            }
        }
        private static int Query(Node? node, int start, int end, int l, int r)
        {

            if (node == null || r < start || end < l)
            {
                return 0;
            }

            if (l <= start && end <= r)
            {
                return node.Value;

            }
            int mid = start + (end - start) / 2;
            int leftSum = Query(node.Left, start, mid, l, r);
            int rightSum = Query(node.Right, mid + 1, end, l, r);

            return leftSum + rightSum;


        }
        public int Query(int version, int left, int right)
        {
            if (version < 0 || version >= VersionCount)
            {
                throw new ArgumentOutOfRangeException(nameof(version), "required version is not exist.");
            }
            return Query(roots[version], 0, _n - 1, left, right);
        }
        private static Node Push(Node node, int start, int end)
        {
            if (node.Lazy != 0)
            {
                int mid = start + (end - start) / 2;
                int leftRangeSize = mid - start + 1;
                int newLeftValue = (node.Left?.Value ?? 0) + (node.Lazy * leftRangeSize);
                Node newLeft = new(newLeftValue)
                {
                    Lazy = (node.Left?.Lazy ?? 0) + node.Lazy,
                    Left = node.Left?.Left,
                    Right = node.Left?.Right,
                };

                int rightRangeSize = end - (mid + 1) + 1;
                int newRightValue = (node.Right?.Value ?? 0) + (node.Lazy * rightRangeSize);
                Node newRight = new(newRightValue)
                {
                    Lazy = (node.Right?.Lazy ?? 0) + node.Lazy,
                    Left = node.Right?.Left,
                    Right = node.Right?.Right,
                };

                return new Node(node.Value)
                {
                    Lazy = 0,
                    Left = newLeft,
                    Right = newRight,
                };
            }

            return node;
        }
        private static Node UpdateRange(Node? node, int start, int end, int l, int r, int val)
        {

            node ??= new Node(0);
            Node updatedNode = Push(node, start, end);
            if (start > r || end < l)
            {
                return updatedNode;
            }
            if (start >= l && end <= r)
            {
                int rangeSize = end - start + 1;
                Node leafTarget = new(updatedNode.Value + (val * rangeSize))
                {
                    Lazy = updatedNode.Lazy + val,
                    Left = updatedNode.Left,
                    Right = updatedNode.Right,
                };
                return leafTarget;
            }
            int mid = start + (end - start) / 2;
            Node? newLeft = UpdateRange(updatedNode.Left, start, mid, l, r, val);
            Node? newRight = UpdateRange(updatedNode.Right, mid + 1, end, l, r, val);


            return new Node((newLeft?.Value ?? 0) + (newRight?.Value ?? 0))
            {
                Left = newLeft,
                Right = newRight,
                Lazy = 0
            };

        }
        public void UpdateRange(int left, int right, int val)
        {
            Node currentRoot = roots[^1];

            Node newRoot = UpdateRange(currentRoot, 0, _n - 1, left, right, val);
            roots.Add(newRoot);
        }
        private static Node Update(Node? oldNode, int start, int end, int index, int val)
        {
            if (start == end)
            {
                return new Node(val);
            }
            int mid = start + (end - start) / 2;
            if (index <= mid)
            {
                Node newLeft = Update(oldNode?.Left, start, mid, index, val);
                Node newNode = new(newLeft.Value + (oldNode?.Right?.Value ?? 0))
                {
                    Left = newLeft,
                    Right = oldNode?.Right,
                };
                return newNode;
            }
            else
            {
                Node newRight = Update(oldNode?.Right, mid + 1, end, index, val);
                Node newNode = new((oldNode?.Left?.Value ?? 0) + newRight.Value)
                {
                    Left = oldNode?.Left,
                    Right = newRight,
                };
                return newNode;
            }

        }
        public void Update(int index, int val)
        {
            Node currentRoot = roots[^1];
            Node newRoot = Update(currentRoot, 0, _n - 1, index, val);
            roots.Add(newRoot);
        }
        public void Undo()
        {
            if (roots.Count > 1)
                roots.RemoveAt(roots.Count - 1);

        }

        
    }
}
