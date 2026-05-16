using System;
using System.Collections.Generic;
using System.Text;

namespace Advanced_Data_Structure
{        
    // ================ Utilites Functions For Console ===================
    internal class Utilities
    {
        public static string Get_string(string msg)
        {
            while (true)
            {
                Console.Write(msg + ": ");
                string? result = Console.ReadLine();
                if (!string.IsNullOrEmpty(result))
                {

                    return result;
                }
            }
        }
        public static int Get_int(string msg, int min, int max)
        {
            while (true)
            {
                Console.Write(msg + ": ");
                string? input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Input cannot be empty!");
                    continue;
                }
                else if (int.TryParse(input, out int result))
                {
                    if (result < min || result > max)
                    {
                        Console.WriteLine($"Input between {min} and {max} only!");
                        continue;
                    }
                    return result;
                }
                Console.WriteLine("Invalid input!");
            }
        }
        public static string GetOption(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine($"{i}- {args[i]}");
            }
            string result = Get_string("Your Choice").ToLower().Trim();

            return result;
        }

    }
}
