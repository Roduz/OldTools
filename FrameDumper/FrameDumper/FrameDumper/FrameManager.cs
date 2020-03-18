using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameDumper
{
    public class FrameManager
    {
        string path;
        GraphicsDeviceManager lGraphics;
        public List<string> fileName = new List<string>();

        public int frameAmount;
        Color[] pmapColor;
        int palDiv;

        int iSizeX = 1;
        int iSizeY = 1;

        public Texture2D texture;

        public FrameManager(ref GraphicsDeviceManager graphics, string path, Color[] pmapColor, int palDiv)
        {
            this.path = path;
            lGraphics = graphics;
            this.pmapColor = pmapColor;
            this.palDiv = palDiv;

            GetFromDir(path);
            frameAmount = fileName.Count;
        }
        public void ReloadFrame(string path)
        {
            this.path = path;
            fileName = null;
            fileName = new List<string>();
            GetFromDir(path);
            frameAmount = fileName.Count;
        }
        void GetFromDir(string path)
        {
            string[] dir = Directory.GetFiles(path, "*.pcx");

            string n = "";
            int l = 0;

            for (int i = 0; i < dir.Length; i++)
            {
                if (IsExt(dir[i], "_colors.pcx"))
                {
                    FileInfo file = new FileInfo(dir[i]);
                    n = file.Name;
                    n = n.Remove(n.Length - 11);
                    dir[i] = n;
                }
                else if (IsExt(dir[i], "_lines.pcx"))
                {
                    FileInfo file = new FileInfo(dir[i]);
                    n = file.Name;
                    n = n.Remove(n.Length - 10);
                    dir[i] = n;
                }
                else if (IsExt(dir[i], "_shade.pcx"))
                {
                    FileInfo file = new FileInfo(dir[i]);
                    n = file.Name;
                    n = n.Remove(n.Length - 10);
                    dir[i] = n;
                }
                else if (IsExt(dir[i], "_dye.pcx"))
                {
                    FileInfo file = new FileInfo(dir[i]);
                    n = file.Name;
                    n = n.Remove(n.Length - 8);
                    dir[i] = n;
                }
                else
                    dir[i] = "";
            }
            for (int i = 0; i < dir.Length; i++)
            {
                if (dir[i] != "")
                {
                    if (i == 0)
                        fileName.Add(dir[i]);
                    else
                    {
                        if (dir[i] != dir[i - 1])
                            fileName.Add(dir[i]);
                    }
                }
            }
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
        public void AssignFrames(int i)
        {
            iSizeX = 1;
            iSizeY = 1;

            bool hasLine = true;
            bool hasShade = true;
            bool hasColor = true;
            bool hasDye = true;

            Color[] masterI = new Color[iSizeX * iSizeY];

            //Get the suffix for the file name
            string pathFileName = path + fileName[i];

            if (File.Exists(pathFileName + "_lines.pcx"))
                ConvertPCX(ref masterI, 1, path, fileName[i] + "_lines.pcx");
            else
            {
                Console.WriteLine("FILE " + fileName[i] + "_lines NOT FOUND");
                hasLine = false;
            }

            if (File.Exists(pathFileName + "_colors.pcx"))
                ConvertPCX(ref masterI, 2, path, fileName[i] + "_colors.pcx");
            else
            {
                Console.WriteLine("FILE " + fileName[i] + "_colors NOT FOUND");
                hasColor = false;
            }

            if (File.Exists(pathFileName + "_shade.pcx"))
                ConvertPCX(ref masterI, 3, path, fileName[i] + "_shade.pcx");
            else
            {
                Console.WriteLine("FILE " + fileName[i] + "_shade NOT FOUND");
                hasShade = false;
            }

            if (File.Exists(pathFileName + "_dye.pcx"))
                ConvertPCX(ref masterI, 4, path, fileName[i] + "_dye.pcx");
            else
            {
                Console.WriteLine("FILE " + fileName[i] + "_dye NOT FOUND");
                hasDye= false;
            }


            if (!hasLine)
                for (int c = 0; c < masterI.Length; c++)
                    masterI[c].R = 255;
            if (!hasColor)
                for (int c = 0; c < masterI.Length; c++)
                    masterI[c].G = 255;
            if (!hasShade)
                for (int c = 0; c < masterI.Length; c++)
                    masterI[c].B = 0;

            //SaveToPNG(textureOut, path, fileName[i] + "_merged.png");
            //Create a Texture2D

            //Texture2D textureOut = new Texture2D(lGraphics.GraphicsDevice, iSizeX, iSizeY, false, SurfaceFormat.Color);
            //textureOut.SetData(masterI);
            //masterI = null;
            //return textureOut;

            //CropImage(ref masterI);
            texture = new Texture2D(lGraphics.GraphicsDevice, iSizeX, iSizeY, false, SurfaceFormat.Color);
            texture.SetData(masterI);
            masterI = null;

            iSizeX = 1;
            iSizeY = 1;
        }
        static bool IsExt(string f, string e)
        {
            return f != null && f.EndsWith(e, StringComparison.Ordinal);
        }
        void CropImage(ref Color[] masterI)
        {
            int brdr = 10;
            int x1 = 0;
            int y1 = 0;
            int x2 = iSizeX;
            int y2 = iSizeY;

            Color blank = new Color(255, 0, 254, 0);

            //Crop Right
            for (int x = 0; x < iSizeX; x++)
            {
                bool hit = false;
                for (int y = 0; y < iSizeY; y++)
                {
                    if (masterI[x + (y * iSizeX)] != blank)
                    {
                        hit = true;
                        break;
                    }
                }
                if (hit)
                {
                    if (x > brdr)
                        x1 = x - brdr;
                    else
                        x1 = x;
                    break;
                }
            }
            //Crop Top
            for (int y = 0; y < iSizeY; y++)
            {
                bool hit = false;
                for (int x = x1; x < iSizeX; x++)
                {
                    if (masterI[x + (y * iSizeX)] != blank)
                    {
                        hit = true;
                        break;
                    }
                }
                if (hit)
                {
                    if (y > brdr)
                        y1 = y - brdr;
                    else
                        y1 = y;
                    break;
                }
            }
            //Crop Left
            for (int x = iSizeX - 1; x > x1; x--)
            {
                bool hit = false;
                for (int y = y1; y < iSizeY; y++)
                {
                    if (masterI[x + (y * iSizeX)] != blank)
                    {
                        hit = true;
                        break;
                    }
                }
                if (hit)
                {
                    if ((iSizeX - x) > brdr)
                        x2 = x + brdr;
                    else
                        x2 = x;
                    break;
                }
            }
            //Crop Low
            for (int y = iSizeY - 1; y > y1; y--)
            {
                bool hit = false;
                for (int x = x1; x < x2; x++)
                {
                    if (masterI[x + (y * iSizeX)] != blank)
                    {
                        hit = true;
                        break;
                    }
                }
                if (hit)
                {
                    if ((iSizeY - y) > brdr)
                        y2 = y + brdr;
                    else
                        y2 = y;
                    break;
                }
            }

            //x1 = 0; y1 = 0; x2 = 1352; y2 = 1004;
            //x1 = 390; y1 = 130; x2 = 1080; y2 = 920;
            //Crop the master image
            int cSizeX = x2 - x1;
            int cSizeY = y2 - y1;
            Color[] cropI = new Color[cSizeX * cSizeY];

            for (int y = 0; y < cSizeY; y++)
            {
                for (int x = 0; x < cSizeX; x++)
                {
                    int mX = x + x1;
                    int mY = (y + y1) * iSizeX;

                    cropI[x + (y * cSizeX)] = masterI[mX + mY];

                }
            }
            iSizeX = x2 - x1;
            iSizeY = y2 - y1;
            masterI = new Color[iSizeX * iSizeY];
            masterI = cropI;
        }
        void ConvertPCX(ref Color[] masterI, byte type, string path, string ImageName)
        {
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

            if (IsOdd(xMax))
                xSize = xMax - xMin + 1;
            else
                xSize = xMax - xMin + 2;

            ySize = yMax - yMin + 1;
            /*
            if (IsOdd(yMax))
                ySize = yMax - yMin + 1;
            else
                ySize = yMax - yMin + 2;
            */

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

            //Convert the pallet data to the color class.
            Color[] colorRGB = new Color[256];
            int palPos = 0;
            for (int i = 0; i < 256; i++)
            {
                colorRGB[i] = new Color(palette[palPos], palette[palPos + 1], palette[palPos + 2], 255);
                palPos += 3;
            }

            //Add to the Master Image
            float shdCon = 255f / 153f;

            iSizeX = xSize;
            iSizeY = ySize;

            if (masterI.Length <= 1)
                masterI = new Color[iSizeX * iSizeY];

            for (int i = 0; i < masterI.Length; i++)
            {
                Color colTemp = colorRGB[(int)imageBytes[i]];
                if (type == 1)
                {
                    //Embed Line info to the Red channel
                    masterI[i].R = colTemp.R;
                }
                else if (type == 2)
                {
                    //Embed Pal Color info to the Green channel
                    for (int c = 0; c < pmapColor.Length; c++)
                    {
                        if (colTemp == pmapColor[c])
                        {
                            masterI[i].G = (byte)((256 / palDiv) * c);
                        }
                    }
                }
                else if (type == 3)
                {
                    if (colTemp.B < 102)
                        colTemp.B = 102;

                    masterI[i].B = (byte)((colTemp.B - 102) * shdCon);
                }
                else if (type == 4)
                {
                    //Embed Pal Color info to the Alpha channel
                    for (int c = 0; c < pmapColor.Length; c++)
                    {
                        if (colTemp == pmapColor[c])
                        {
                            masterI[i].A = (byte)((256 / palDiv) * c);
                        }
                    }
                }
            }
            //Output
            Console.WriteLine("Loaded " + ImageName);

            fileHeader = null;
            imageBytes = null;
            scanLine = null;
            palette = null;
            colorRGB = null;
        }
        public void SaveToPNG(Texture2D t, string f, string s)
        {
            using (Stream stream = File.OpenWrite(f + s))
            {
                t.SaveAsPng(stream, t.Width, t.Height);
                Console.WriteLine("Saved " + s);
            }
        }
        bool IsOdd(int value)
        {
            return value % 2 != 0;
        }
        public void ClearData()
        {
            path = null;
            lGraphics = null;
            frameAmount = 0;
            iSizeX = 0;
            iSizeY = 0;

            fileName.Clear();
            fileName = null;
            pmapColor = null;
            texture = null;
        }
        ~FrameManager()
        {
            path = null;
            lGraphics = null;
            frameAmount = 0;
            iSizeX = 0;
            iSizeY = 0;

            //fileName.Clear();
            fileName = null;
            pmapColor = null;
            texture = null;
        }
    }
}