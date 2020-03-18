using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HitboxEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Environment.CurrentDirectory + "\\";

            bool error = true;
            bool checkPath = false;

            if (args.Length >= 1)
            {
                path = args[0] + "\\";
                error = false;
            }
            else
            {
                error = true;
                Console.WriteLine("Missing [directory to frames]");
            }

            if (error)
            {
                Console.WriteLine("Press ESCAPE to Exit");
                ConsoleKeyInfo keyInput;

                do
                    keyInput = Console.ReadKey();
                while
                    (keyInput.Key != ConsoleKey.Escape);
            }
            else
            {
                if (!Directory.Exists(path))
                    Console.WriteLine("Frame directory not found " + path);
                else
                    checkPath = true;

                if (checkPath)
                {
                    Console.WriteLine(path);
                    using (Game1 game = new Game1(path))
                    {
                        game.Run();
                    }
                }
            }
        }
    }
}