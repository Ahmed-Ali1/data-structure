using System;
using System.Collections.Generic;
using System.Text;

namespace Advanced_Data_Structure
{
    public class RandomLootBox
    {
        private readonly string[] itemNames;
        private readonly FenwickTree itemWeights;
        private readonly Random random;
        // Utility Property
        private readonly string[] options;
        public RandomLootBox(string[] names, int[] weights)
        {
            itemNames = names;
            itemWeights = new FenwickTree(weights);
            random = new Random();
            // Utility Property
            options = ["Roll", "Upgrade", "Exit"];
        }
        public string Roll()
        {
            //1. calculate total current weights
            int totalWeights = itemWeights.Query(itemNames.Length - 1);
            //2. generate random number
            int rnd = random.Next(1, totalWeights+1);

            //3. get index and return the name
            int index = itemWeights.LowerBound(rnd);
            return itemNames[index];
        }
        public bool Update(string itemName, int delta)
        {
            //1. get itemName index
            int index = -1;
            for(int i = 0; i < itemNames.Length; i++)
            {
                if (itemNames[i].Equals(itemName,StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }
            //2. call Fenwick Update with index, delta 
            if (index != -1)
            {
                itemWeights.Update(index, delta);
                return true;
            }
            return false;
        }

        // ================ Utilites Game Functions For Console ===================
        private static void HandleUpgradeOption(RandomLootBox game)
        {
            Console.WriteLine("\n--- Upgrade Menu (Type 'back' to return) ---");
            string giftToUpgrade = Utilities.Get_string("Enter the name of the gift you want to upgrade").Trim();
            if (giftToUpgrade.Equals("back", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Returning to main menu...");
                return;
            }
            int amount = Utilities.Get_int("Enter the amount to increase weight by", 1, 100);
            bool isSuccess = game.Update(giftToUpgrade, amount);
            if (isSuccess)
            {
                Console.WriteLine($"\nSuccess! Increased {giftToUpgrade} weight by {amount}.");
            }
            else
            {
                Console.WriteLine("\nError: Gift name not found!");
            }
        }
        public static void Start(RandomLootBox game)
        {
            Console.WriteLine("Welcome To LootBoxGame!");
            while (true)
            {
                Console.WriteLine("\n--- Main Menu ---");
                string input = Utilities.GetOption(game.options).ToLower().Trim();

                if (input == "exit" || input == "quit")
                {
                    Console.WriteLine("\nThank you for playing!");
                    return;
                }
                switch (input)
                {
                    case "roll":
                        string rolledGift = game.Roll();
                        Console.WriteLine($"\nCongrats! You got: {rolledGift}");
                        break;
                    case "upgrade":
                        HandleUpgradeOption(game);
                        break;
                    default:
                        Console.WriteLine("\nInvalid choice! Please type Roll, Upgrade, or Exit.");
                        break;
                }

            }
        }
    }
}
