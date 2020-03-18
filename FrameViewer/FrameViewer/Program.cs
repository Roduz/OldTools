using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FrameViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathExe = Environment.CurrentDirectory + "\\";
            string path = "";
            string pathPal = "";
            string pmapName = "";
            int palSize = 32;
            int spriteSize = 1;
            bool checkPath = false;
            bool checkPal = false;

            bool error = true;

            if (args.Length >= 4)
            {
                path = args[0] + "\\";
                pathPal = args[1] + "\\";
                pmapName = args[2];
                palSize = Convert.ToInt32(args[3]);
                if (args.Length > 4)//if (args[4] != null)
                {
                    spriteSize = Convert.ToInt32(args[4]);

                    if (spriteSize >= 100)
                        spriteSize = 100;
                    else if (spriteSize <= 5)
                        spriteSize = 5;

                    spriteSize = 100 / spriteSize;
                }

                error = false;
            }
            else
            {
                error = true;
                Console.WriteLine("Missing [directory to frames]  [directory to palattes] [pmap file name] [palette quality]");
                /*
                path = "C:\\Skullgirls\\trunk\\ArtAssets\\squigly\\data\\assist\\tag_in\\frames\\";
                pathPal = "C:\\Skullgirls\\trunk\\ArtAssets\\squigly\\palettes\\";
                pmapName = "squigly_FOR_GAME";
                error = false;
                */
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
                //Directory Check
                if (!Directory.Exists(path))
                    Console.WriteLine("Frame directory not found " + path);
                else
                    checkPath = true;

                if (!Directory.Exists(pathPal))
                    Console.WriteLine("Palette directory not found " + pathPal);
                else
                    checkPal = true;

                if (checkPath && checkPal)
                {
                    Console.WriteLine(path);
                    Console.WriteLine(pathPal);
                    Console.WriteLine("Palette Index size at " + palSize);
                    using (Game1 game = new Game1(path, pathPal, pmapName, palSize, spriteSize))
                    {
                        game.Run();
                    }
                }
            }
        }
    }
}
