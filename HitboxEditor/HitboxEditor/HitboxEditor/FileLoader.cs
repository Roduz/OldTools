using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HitboxEditor
{
    class FileLoader
    {
        GraphicsDeviceManager graphics;
        string path;

        public int frameAmount;

        public List<string> fileName = new List<string>();
        //public List<string> fileType = new List<string>();

        public List<int> frameRate = new List<int>();

        public FileLoader(string path)
        {
            this.path = path;
        }
        public FileLoader(GraphicsDeviceManager graphics, string path)
        {
            this.graphics = graphics;
            this.path = path;

            GetDirContent(path);
            frameAmount = fileName.Count;
        }

        public Texture2D ConvertPNG(string ImageName)
        {
            Texture2D textureOut;
            Stream fileStream = new FileStream(ImageName, FileMode.Open, FileAccess.Read, FileShare.Read);
            textureOut = Texture2D.FromStream(graphics.GraphicsDevice, fileStream);
            fileStream.Close();
            return textureOut;
        }
        public Texture2D ConvertPCX(string path, string ImageName)
        {
            Texture2D textureOut;
            char bitsPerPixel; //Bits per Pixel
            int xMin; // Left of image
            int yMin; // Top of Image
            int xMax; // Right of Image
            int yMax; // Bottom of image
            int xRes; //Horizontal Resolution
            int yRes; //Vertical Resolution
            int xSize; //Horizontal Screen Size
            int ySize; //Vertical Screen Size
            byte numBitPlanes; //Number of Bit Planes
            int bytesPerLine; //bytes per Scan-line
            byte[] palette = new byte[768]; //256-Color Palette

            //read header
            byte[] fileHeader = new byte[128];
            FileStream fileStream = new FileStream(path + ImageName, FileMode.Open, FileAccess.Read, FileShare.None);
            fileStream.Read(fileHeader, 0, 128);

            //assign header variables
            bitsPerPixel = BitConverter.ToChar(fileHeader, 3);
            xMin = BitConverter.ToInt16(fileHeader, 4);
            yMin = BitConverter.ToInt16(fileHeader, 6);
            xMax = BitConverter.ToInt16(fileHeader, 8);
            yMax = BitConverter.ToInt16(fileHeader, 10);
            xRes = BitConverter.ToInt16(fileHeader, 12);
            yRes = BitConverter.ToInt16(fileHeader, 14);
            numBitPlanes = fileHeader[65];
            bytesPerLine = BitConverter.ToInt16(fileHeader, 66);

            xSize = xMax - xMin + 1;
            ySize = yMax - yMin + 1;

            //read the palette at end of the file
            fileStream.Seek(-768, SeekOrigin.End);
            for (int i = 0; i < 768; i++)
            {
                palette[i] = (byte)fileStream.ReadByte();
            }

            //pixel data from the file
            fileStream.Seek(128, SeekOrigin.Begin);

            int totalBytesPerLine = numBitPlanes * bytesPerLine;
            int linePaddingSize = ((bytesPerLine * numBitPlanes) * (8 / bitsPerPixel)) - ((xMax - xMin) + 1);

            byte[] scanLine = new byte[totalBytesPerLine];
            byte nRepeat;
            byte pColor;
            int pIndex = 0;
            byte[] imageBytes = new byte[totalBytesPerLine * ySize];

            for (int iY = 0; iY < ySize; iY++)
            {
                int iX = 0;
                while (iX < totalBytesPerLine)
                {
                    nRepeat = (byte)fileStream.ReadByte();
                    if (nRepeat > 192)
                    {
                        nRepeat -= 192;
                        pColor = (byte)fileStream.ReadByte();
                        for (int j = 0; j < nRepeat; j++)
                        {
                            if (iX < scanLine.Length)
                            {
                                scanLine[iX] = pColor;
                                imageBytes[pIndex] = pColor;
                            }
                            iX++;
                            pIndex++;
                        }
                    }
                    else
                    {
                        if (iX < scanLine.Length)
                        {
                            scanLine[iX] = nRepeat;
                            imageBytes[pIndex] = nRepeat;
                        }
                        iX++;
                        pIndex++;
                    }
                }
            }
            fileStream.Close();

            //Convert the pallet data to the color array.
            Color[] colorRGB = new Color[256];
            int palPos = 0;
            for (int i = 0; i < 256; i++)
            {
                colorRGB[i] = new Color(palette[palPos], palette[palPos + 1], palette[palPos + 2], 255);
                palPos += 3;
            }

            //Create a Texture2D
            textureOut = new Texture2D(graphics.GraphicsDevice, xSize, ySize, false, SurfaceFormat.Color);
            Color[] color = new Color[xSize * ySize];
            for (int i = 0; i < color.Length; i++)
            {
                color[i] = colorRGB[(int)imageBytes[i]];
            }
            textureOut.SetData(color);

            return textureOut;
        }

        public List<string> ReadFRM(string path, string name)
        {
            Console.WriteLine("Reading: " + name + ".frm");
            List<string> info = new List<string>();
            string line;
            StreamReader file = new StreamReader(path + name + ".frm");
            while ((line = file.ReadLine()) != null)
            {
                if (line.StartsWith("#") || line.StartsWith("/") || line.StartsWith(" ") || line.StartsWith("(") || line.StartsWith("-"))
                { continue; }
                else if (line.Contains("Center:") || line.Contains("Points:") || line.Contains("Hitbox:") || line.Contains("[end]") ||
                    line.Contains("0") || line.Contains("1") || line.Contains("2") || line.Contains("3") || line.Contains("4") ||
                    line.Contains("5") || line.Contains("6") || line.Contains("7") || line.Contains("8") || line.Contains("9"))
                {
                    info.Add(line);
                }
                else
                { continue; }
            }
            file.Close();
            return info;
        }

        public void GetDirContent(string path)
        {
            string[] dir = Directory.GetFiles(path);
            string n = "";
            int l = 0;


            for (int i = 0; i < dir.Length; i++)
            {
                if (IsExt(dir[i], ".pcx"))
                {
                    FileInfo file = new FileInfo(dir[i]);
                    n = file.Name;
                    l = 6;
                    n = n.Remove(n.Length - 4);
                    fileName.Add(n);
                    frameRate.Add(l);

                    //Get the suffix for the file name
                    if (!File.Exists(path + n + ".frm"))
                    {
                        List<string> line = new List<string>();
                        line.Clear();
                        line.Add("Center: " + 0 + " " + 0);
                        line.Add("Points:");
                        line.Add("Head " + 10 + " " + 10);
                        line.Add("Shoulder " + 20 + " " + 20);
                        line.Add("Waist " + 30 + " " + 30);
                        line.Add("Knee " + 40 + " " + 40);
                        line.Add("Feet " + 50 + " " + 50);
                        line.Add("[end]");
                        line.Add("Hitbox:");
                        line.Add("[end]");

                        Console.WriteLine("Created: " + n + ".frm");
                        SaveFRM(path, n, line);
                    }
                }
            }
        }
        static bool IsExt(string f, string e)
        {
            return f != null && f.EndsWith(e, StringComparison.Ordinal);
        }

        public string GetFrameName(string line)
        {
            string s;
            int a;
            if (line.Contains(" ") && line.Contains("-"))
            {
                a = line.IndexOf(" ");
                s = line.Substring(0, a);
            }
            else if (line.Contains("-") && !line.Contains(" "))
            {
                a = line.IndexOf("-");
                s = line.Substring(0, a);
            }
            else
                s = line;

            return s;
        }
        public Vector2 GetFrameCenter(string line)
        {
            Vector2 amount = Vector2.Zero;
            int a = line.IndexOf(",") - line.IndexOf("(") - 1;
            int z = line.IndexOf(")") - line.IndexOf(",") - 2;

            amount.X = Convert.ToInt32(line.Substring(line.IndexOf("(") + 1, a));
            amount.Y = Convert.ToInt32(line.Substring(line.IndexOf(",") + 2, z));

            return amount;
        }

        public Texture2D AssignImage(string p, string f)
        {
            Texture2D t = ConvertPCX(p, f);

            return t;
        }

        public void SaveFRM(string path, string name, List<string> info)
        {
            System.IO.File.WriteAllLines(path + "/" + name + ".frm ", info);
            Console.WriteLine("Saved: " + name + ".frm");
        }
    }
}
