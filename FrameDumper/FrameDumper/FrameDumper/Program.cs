using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FrameDumper
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "";
            string pmapName = "";
            string palName = "";
            int palSize = 16;
            bool showScreen = false;

            string dirList = "";
            string dumpList = "";

            string pathData = "";
            string pathPal = "";

            bool error = true;

            if (args.Length == 5)
            {
                string[] words = args[0].Split('\\');

                int count = 0;
                foreach (string s in words)
                {
                    if (s == args[1])
                        break;
                    else
                        ++count;
                }

                for (int i = 0; i <= count; ++i)
                {
                    path += words[i] + "/";
                }

                for (int i = count; i < words.Count(); ++i)
                {
                    dumpList += words[i] + "/";
                }

                // Split on directory separator
                for (int i = 0; i < words.Length; ++i)
                    dirList += words[i] + '/';

                pmapName = args[1];
                palName = args[2];
                palSize = Convert.ToInt32(args[3]);

                if (args[4] == "true" || args[4] == "True" || args[4] == "true\n" || args[4] == "True\n")
                    showScreen = true;

                pathData = path + "data/";
                pathPal = path + "palettes/";

                error = false;
            }
            else
            {
                error = true;
                Console.WriteLine("Missing [character directory] [directory to frames] [pmap file name] [pal file name] [palette quality] [show screen bool]");

                /*
                path = "C:\\Skullgirls\\trunk\\ArtAssets\\filia\\";
                dumpList = "filia\\data\\assist\\tag_in\\frames\\";

                // Split on directory separator
                string tmpdir = "";
                string[] dir = dumpList.Split('\\');
                for (int i = 1; i < dir.Length - 1; i++)
                    tmpdir += dir[i] +'\\';

                dirList = path + tmpdir;
                pmapName = "filia";
                palName = "filia_1p";
                palSize = 64;
                showScreen = false;
                pathData = path + "data\\";
                pathPal = path + "palettes\\";
                error = false;
                */
            }

            if (error)
            {
                Console.WriteLine("Error with command line arguments!");
                Console.WriteLine("Press ESCAPE to Exit");
                ConsoleKeyInfo keyInput;
                do
                    keyInput = Console.ReadKey();
                while
                    (keyInput.Key != ConsoleKey.Escape);
            }
            else
            {
                //(string dirList, string dumpList, string pathData, string pmapName, string pathPal, string palName, int palSize, bool showScreen)
                using (Game1 game = new Game1(dirList, dumpList, pathData, pmapName, pathPal, palName, palSize, showScreen))
                {
                    game.Run();
                }
            }
        }
    }
}
