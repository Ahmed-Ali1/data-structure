

namespace Advanced_Data_Structure;

public class FenwickTree
{
    private readonly int[] tree;
    private readonly int size;
    public FenwickTree(int[] array)
    {
        size = array.Length;
        tree = new int[size + 1];
        Build(array);

    }

    private void Build(int[] array)
    {

        for (int i = 0; i < size; i++)
        {
            Update(i, array[i]);
        }
    }
    public void Update(int index, int delta)
    {
        index++;
        for (; index <= size; index += (index & -index))
        {
            tree[index] += delta;
        }
    }
    public int Query(int index)
    {
        index++;
        int sum = 0;
        for (; index > 0; index -= (index & -index))
        {
            sum += tree[index];
        }
        return sum;
    }
    public int RangeQuery(int left,int right)
    {
        return Query(right) - Query(left-1);
    }

    public int Read(int index)
    {
        return Query(index) - Query(index - 1);
    }

    public void Set(int index, int newValue)
    {
        int current = Read(index);
        int delta = newValue - current;
        Update(index, delta);

    }
    private int HighestPowerOfTwo(int n)
    {
        int power = 1;

        while (power <= n)
        {
            power <<= 1;
        }

        return power >> 1;
    }
    public int LowerBound(int target)
    {
        int index = 0;
        int bitMask = HighestPowerOfTwo(size);

        while (bitMask != 0)
        {
            int next = index + bitMask;

            if (next <= size && tree[next] < target)
            {
                target -= tree[next];
                index = next;
            }

            bitMask >>= 1;
        }

        return index;
    }
    // ========= debugging function
    public void PrintTree()
    {
        for(int i = 1; i <= size; i++)
        {
            Console.Write($"{tree[i]} ");
        }
        Console.WriteLine();
    }
    

}
