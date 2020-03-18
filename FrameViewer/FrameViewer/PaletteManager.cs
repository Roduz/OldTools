using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameViewer
{
    public class PaletteManager
    {
        string pathPal;
        GraphicsDeviceManager pGraphics;

        public List<Texture2D> paletteTable = new List<Texture2D>();
        public List<Texture2D> paletteTableLine = new List<Texture2D>();
        //bool havePal = false;
        public List<string> palName = new List<string>();
        public List<string> palNameLine = new List<string>();
        Color[] colormap = new Color[256];
        public int colorAmount;
        public int palAmount;
        public int palAmountLine;
        string pmapName;
        int palSize;
        public List<int> pmapCol = new List<int>();
        public Color[] pmapColor;

        public int div = 16;

        public PaletteManager(ref GraphicsDeviceManager graphics, string pathPal, string pmapName, int palSize)
        {
            this.pathPal = pathPal;
            this.pmapName = pmapName;
            this.palSize = palSize;
            pGraphics = graphics;
            palName.Add("None");
            palNameLine.Add("No_Dye");
            GetPalContent(pathPal);
            palAmount = palName.Count;
            palAmountLine = palNameLine.Count;
            LoadAllTables();
        }
        public void ReloadPalette(ref GraphicsDeviceManager graphics)
        {
            Console.WriteLine("Reloading Palette files from " + pathPal);
            paletteTable = new List<Texture2D>();
            paletteTableLine = new List<Texture2D>();
            palName = new List<string>();
            palNameLine = new List<string>();
            colormap = new Color[256];
            palAmount = 0;
            palAmountLine = 0;
            colorAmount = 0;
            pmapCol = new List<int>();
            pmapColor = null;

            pGraphics = graphics;
            palName.Add("None");
            palNameLine.Add("No_Dye");
            GetPalContent(pathPal);
            palAmount = palName.Count;
            palAmountLine = palNameLine.Count;
            LoadAllTables();
        }
        void LoadAllTables()
        {
            for (int i = 0; i < palAmount; i++)
            {
                if (i == 0)
                    paletteTable.Add(MakeDefaultPalette(4));
                else
                {
                    paletteTable.Add(AssignPalette(i, palSize, true));
                    Console.WriteLine("Loaded " + palName[i] + ".pal");
                }
            }
            for (int i = 0; i < palAmountLine; i++)
            {
                if (i == 0)
                    paletteTableLine.Add(MakeBlackPalette(4));
                else
                {
                    paletteTableLine.Add(AssignPalette(i, 4, false));
                    Console.WriteLine("Loaded " + palNameLine[i] + ".pal");
                }
            }
        }
        void GetPalContent(string pathPal)
        {
            string[] tmpfile = Directory.GetFiles(pathPal, "*_colormap.pal");
            FileInfo file = new FileInfo(tmpfile[0]);

            string palname = file.Name;
            palname = palname.Remove(palname.Length - 13);

            if (File.Exists(pathPal + pmapName + ".pmap"))
                pmapCol = PmapInfo(pathPal + pmapName + ".pmap");
            else
                Console.WriteLine("Invalid " + pathPal + pmapName + ".pmap");

            //div = pmapCol.Count;
            if (pmapCol.Count <= 16)
                div = 16;
            else if (pmapCol.Count <= 32)
                div = 32;
            else if (pmapCol.Count <= 64)
                div = 64;
            else if (pmapCol.Count <= 128)
                div = 128;
            else
                div = 256;


            if (pmapCol.Count > 0)
            {
                //get colormap parse
                colormap = ColorPalParse(pathPal + file.Name);
                pmapColor = GetColorMapArray();
                GetPalNames(pathPal, palname);
            }
            else
                return;
        }
        List<int> PmapInfo(string filename)
        {
            List<int> count = new List<int>();
            string line;
            StreamReader file = new StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("#") || line.Length <= 2)
                {
                    continue;
                }
                else if (line.Contains("Light:"))
                {
                    string[] tmpLine = line.Split(' ');

                    if (tmpLine.Length > 2)
                    {
                        line = tmpLine[1];
                        count.Add(Convert.ToInt32(line));
                    }
                    else
                    {
                        Console.WriteLine("Error reading PMAP file!");
                        return null;
                    }
                }
            }
            file.Close();
            return count;
        }
        void GetPalNames(string pathPal, string name)
        {
            string[] dir = Directory.GetFiles(pathPal, "*.pal");
            List<string> palFiles = new List<string>();

            for (int i = 0; i < dir.Length; i++)
            {
                if (dir[i].Contains(name) && !dir[i].Contains("colormap"))
                {
                    string n;
                    FileInfo file = new FileInfo(dir[i]);
                    n = file.Name;
                    n = n.Remove(n.Length - 4);
                    dir[i] = n;

                    if (i == 0)
                        palFiles.Add(dir[i]);
                    else
                    {
                        if (dir[i] != dir[i - 1])
                            palFiles.Add(dir[i]);
                    }
                }
            }
            //Sort the file names into proper order
            for (int c = 0; c < palFiles.Count; c++)
            {
                for (int i = 0; i < palFiles.Count; i++)
                {
                    string tmp = "_" + (c + 1) + "p";
                    if (palFiles[i].Contains(tmp) && palFiles[i].Contains("_dye"))
                    {
                        palNameLine.Add(palFiles[i]);
                    }
                    else if (palFiles[i].Contains(tmp))
                    {
                        palName.Add(palFiles[i]);
                    }
                }
            }
            //havePal = true;
        }
        Color[] ColorPalParse(string filename)
        {
            Color[] map = new Color[256];
            List<string> lineList = new List<string>();
            string line;
            StreamReader file = new StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("#") || line.Contains("ntries") || line.Length <= 2)
                {
                    continue;
                }
                else if (line.StartsWith("0") || line.StartsWith("1") || line.StartsWith("2") || line.StartsWith("3") || line.StartsWith("4") || line.StartsWith("5") ||
                    line.StartsWith("6") || line.StartsWith("7") || line.StartsWith("8") || line.StartsWith("9"))
                {
                    string[] tmpLine = line.Split('#');
                    line = tmpLine[0].Trim();
                    lineList.Add(line);
                }
            }
            file.Close();
            for (int i = 0; i < 256; i++)
            {
                map[i] = Color.Black;
                if (i < lineList.Count)
                {
                    Color tmpCol = Color.Black;
                    string[] tmpLine = lineList[i].Split(' ');
                    int split = tmpLine.Length;

                    if (split < 3)
                    {
                        Console.WriteLine("Error reading colormap file! Check line with " + lineList[i]);
                        return null;
                    }

                    int chng = 0;
                    for (int c = 0; c < split; c++)
                    {
                        tmpLine[c] = tmpLine[c].Trim();

                        if (tmpLine[c] != "")
                        {
                            if (chng == 0)
                                map[i].R = Convert.ToByte(tmpLine[c]);
                            else if (chng == 1)
                                map[i].G = Convert.ToByte(tmpLine[c]);
                            else if (chng == 2)
                                map[i].B = Convert.ToByte(tmpLine[c]);
                            else if (chng == 3)
                                map[i].A = Convert.ToByte(tmpLine[c]);

                            chng++;
                        }
                    }
                }
            }
            return map;
        }
        void MergePmapMap()
        {
            int count = 0;
            for (int sec = 0; sec < pmapCol.Count; sec++)
            {
                int grp = pmapCol[sec];
                for (int i = 0 + count; i < grp + count; i++)
                {
                    if (colormap[i] != Color.Black)
                    {
                        //Found the color for this section, copy it over and break
                        for (int c = 0; c < grp; c++)
                        {
                            colormap[count + c] = colormap[i];
                        }
                    }
                }
                count += grp;
            }
        }
        Color[] GetColorMapArray()
        {
            Color[] colorArray = new Color[pmapCol.Count];
            int array = 0;
            int count = 0;
            for (int sec = 0; sec < pmapCol.Count; sec++)
            {
                int grp = pmapCol[sec];
                for (int i = 0 + count; i < grp + count; i++)
                {
                    if (colormap[i] != Color.Black)
                    {
                        //Found the color for this section, copy it over and break
                        colorArray[array] = colormap[i];
                        array++;
                    }
                }
                count += grp;
            }
            return colorArray;
        }
        Color[] CreateColorBand(Color[] colorStrip, int inAmount)
        {
            Color[] baseColors = new Color[inAmount];
            //Reverse the order to dark to light
            for (int i = 0; i < inAmount; i++)
            {
                baseColors[i] = colorStrip[inAmount - (i + 1)];
                colorStrip[inAmount - (i + 1)] = Color.Black;
            }

            float colLenght = colorStrip.Length;
            float amount = (float)(colLenght - 1) / (inAmount - 1);
            int count = 0;

            if (inAmount > 2)
            {
                for (int i = 0; i < (int)colLenght; i++)
                {
                    if (i > (amount * count) + amount)
                        count++;

                    float tRate = (float)(i / amount) - count;

                    if (count < inAmount - 1)
                        colorStrip[i] = Color.Lerp(baseColors[count], baseColors[count + 1], tRate);
                    else
                        colorStrip[i] = baseColors[inAmount - 1];
                }
            }
            else if (inAmount == 2)
            {
                for (int i = 0; i < (int)colLenght; i++)
                {
                    float tRate = (float)(i / (colLenght - 1));
                    colorStrip[i] = Color.Lerp(baseColors[0], baseColors[inAmount - 1], tRate);
                }
            }
            else
            {
                for (int i = 0; i < colorStrip.Length; i++)
                {
                    colorStrip[i] = baseColors[0];
                }
            }
            return colorStrip;
        }
        public Texture2D AssignPalette(int i, int palSize, bool regPal)
        {
            Color[] cpalette = new Color[palSize * div]; //new Color[palSize * pmapColor.Length];
            Color[] cmap;
            if (regPal)
                cmap = ColorPalParse(pathPal + palName[i] + ".pal");
            else
                cmap = ColorPalParse(pathPal + palNameLine[i] + ".pal");

            //pmapCol
            int count = 0;
            for (int p = 0; p < pmapCol.Count; ++p)
            {
                Color[] pmapTempHold = new Color[palSize];
                if (regPal)
                {
                    for (int h = 0; h < pmapCol[p]; ++h)
                        pmapTempHold[h] = cmap[count + h];
                    pmapTempHold = CreateColorBand(pmapTempHold, pmapCol[p]);
                }
                else
                {
                    for (int h = 0; h < palSize; ++h)
                        pmapTempHold[h] = cmap[count + pmapCol[p] - 1];
                    pmapTempHold = CreateColorBand(pmapTempHold, palSize);
                }

                for (int c = 0; c < palSize; c++)
                {
                    cpalette[c + (p * palSize)] = pmapTempHold[c];
                }
                count += pmapCol[p];
            }
            //colorAmount = pmapColor.Length;
            colorAmount = div;
            Texture2D palTex = new Texture2D(pGraphics.GraphicsDevice, palSize, colorAmount);
            palTex.SetData(cpalette);
            return palTex;
        }
        public Texture2D MakeDefaultPalette(int palSize)
        {
            Color[] cpalette = new Color[palSize * div]; //new Color[palSize * pmapColor.Length];

            for (int p = 0; p < pmapCol.Count; ++p)
            {
                Color[] pmapTempHold = new Color[palSize];
                pmapTempHold[0] = pmapColor[p];

                pmapTempHold = CreateColorBand(pmapTempHold, 1);

                for (int c = 0; c < palSize; ++c)
                {
                    cpalette[c + (p * palSize)] = pmapTempHold[c];
                }
            }
            Texture2D palTex = new Texture2D(pGraphics.GraphicsDevice, palSize, div);
            palTex.SetData(cpalette);
            return palTex;
        }
        public Texture2D MakeBlackPalette(int palSize)
        {
            Color[] cpalette = new Color[palSize * div];

            Texture2D palTex = new Texture2D(pGraphics.GraphicsDevice, palSize, div);
            palTex.SetData(cpalette);
            return palTex;
        }
        public void SaveToPNG(Texture2D t, string f, string s)
        {
            using (Stream stream = File.OpenWrite(f + s))
            {
                t.SaveAsPng(stream, t.Width, t.Height);
                Console.WriteLine("Saved " + s);
            }
        }
    }
}
