using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameDumper
{
    public class PaletteManager
    {
        string pathPal;
        GraphicsDeviceManager pGraphics;
        public Texture2D paletteTable;
        public Texture2D paletteTableLine;
        string pmapName = "";
        public string palName = "";
        public string palNameLine = "";
        Color[] colormap = new Color[256];
        public int colorAmount;
        int palSize;
        public List<int> pmapCol = new List<int>();
        public Color[] pmapColor;
        public int div = 16;

        public PaletteManager(ref GraphicsDeviceManager graphics, string pathPal, string pmapName, string palName, int palSize)
        {
            this.pathPal = pathPal;
            this.palSize = palSize;
            pGraphics = graphics;
            this.pmapName = pmapName;
            this.palName = palName;
            palNameLine = palName + "_dye";
            GetPalContent(pathPal);
            paletteTable = AssignPalette(true);
            paletteTableLine = AssignPalette(false);
        }
        void GetPalContent(string pathPal)
        {
            string[] tmpfile = Directory.GetFiles(pathPal, "*_colormap.pal");
            FileInfo file = new FileInfo(tmpfile[0]);
            //string pmapname = file.Name;
            //pmapname = pmapname.Remove(pmapname.Length - 5);

            if (File.Exists(pathPal + pmapName + ".pmap"))
                pmapCol = PmapInfo(pathPal + pmapName + ".pmap");
            else
                Console.WriteLine("Invalid " + pathPal + pmapName + ".pmap");

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
                //GetPalNames(pathPal, pmapname);
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
        public Texture2D AssignPalette(bool regPal)
        {
            Color[] cpalette = new Color[palSize * div]; //new Color[palSize * pmapColor.Length];
            Color[] cmap;
            if (regPal)
                cmap = ColorPalParse(pathPal + palName + ".pal");
            else
                cmap = ColorPalParse(pathPal + palNameLine + ".pal");

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