

using System.Runtime.CompilerServices;

namespace Advanced_Data_Structure;

public static class Program

{
    static void p<T>(T t, bool end = false)
    {
        Console.Write(end ? $"{t}\n" : $"{t} ");
    }

    static void p() => Console.WriteLine();
    

    public static void Main()
    {
        string[] gifts = ["Gems", "Gold", "Wood"];
        int[] weights = [1, 10, 89];
        RandomLootBox game = new(gifts, weights);
        RandomLootBox.Start(game);
    }

}


