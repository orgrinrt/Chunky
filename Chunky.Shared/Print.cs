using System;

namespace Chunky.Shared
{
    public static class Print
    {
        public static void Line(ConsoleColor color, string msg)
        {
            ConsoleColor original = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = original;
        }
        
        public static void Line(ConsoleColor color, Exception e)
        {
            ConsoleColor original = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(e);
            Console.ForegroundColor = original;
        }

        public static void Inline(ConsoleColor color, string msg)
        {
            ConsoleColor original = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(msg);
            Console.ForegroundColor = original;
        }
        
        public static void Inline(string msg)
        {
            ConsoleColor original = Console.ForegroundColor;
            Inline(original, msg);
        }
        
        public static void Line(string msg = "")
        {
            Line(Console.ForegroundColor, msg);
        }

        public static void Line(Exception e)
        {
            Line(Console.ForegroundColor, e);
        }

        public static void Hr(ConsoleColor color = ConsoleColor.White, int length = 10, char c = '-')
        {
            string hr = "";
            for (int i = 0; i < length; i++)
            {
                hr += c;
            }
            Console.WriteLine(hr);
        }

        public static string GetHrString(int length = 10, char c = '-')
        {
            string hr = "";
            
            for (int i = 0; i < length; i++)
            {
                hr += c;
            }

            return hr;
        }
    }
}