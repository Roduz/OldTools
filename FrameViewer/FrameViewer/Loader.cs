using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace FrameViewer
{
    public class Loader
    {
        string path;
        GraphicsDeviceManager lGraphics;
        List<string> fileName = new List<string>();
        List<Vector2> frameCenter = new List<Vector2>();
        List<int> frameRate = new List<int>();
        
        //public int frameAmount;
        PaletteManager lPalette;
        int spriteSize;

        int iSizeX = 1;
        int iSizeY = 1;

        bool txtFile = false;
        bool frmFile = false;

        public Loader()
        { }
        public Loader(string path)
        {
            this.path = path;
        }
        public Loader(ref GraphicsDeviceManager graphics, string path, ref PaletteManager refPalette, ref List<Sprite> sprite, int spriteSize)
        {
            this.path = path;
            lGraphics = graphics;
            lPalette = refPalette;
            this.spriteSize = spriteSize;

            string[] textfile = Directory.GetFiles(path, "*.fvd");

            if (textfile.Length > 0)
                txtFile = true;

            if (txtFile)
                GetFromTxt(textfile[0], ref sprite);
            else
                GetFromDir(path, ref sprite);
        }
        void GetFromDir(string path, ref List<Sprite> sprite)
        {
            string[] dir = Directory.GetFiles(path, "*.png");
            if (dir.Length == 0)
                dir = Directory.GetFiles(path, "*.pcx");

            for (int i = 0; i < dir.Length; i++)
            {
                if (dir[i].EndsWith("_colors.pcx") || dir[i].EndsWith("_colors.png") || dir[i].EndsWith("_colors2.pcx") || dir[i].EndsWith("_colors2.png") || dir[i].EndsWith("_colors3.pcx") || dir[i].EndsWith("_colors3.png"))
                {
                    FileInfo file = new FileInfo(dir[i]);
                    fileName.Add(file.Name);
                }
            }
            dir = Directory.GetFiles(path, "*.frm");
            if (dir.Length == fileName.Count)
            {
                frmFile = true;
                for (int i = 0; i < fileName.Count; i++)
                    frameCenter.Add(ReadFRM(path, fileName[i]));
            }
            sprite.Add(new Sprite());
            for (int i = 0; i < fileName.Count; i++)
                sprite[0].frame.Add(AssignFrames(i));
        }
        void GetFromTxt(string textfile, ref List<Sprite> sprite)
        {
            string line;
            //List for all the valid entries from the text file
            //Read the file and display it line by line.
            List<string> info = new List<string>();
            StreamReader file = new StreamReader(textfile);
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("#") || line.StartsWith("=") || line.StartsWith("@") || line.StartsWith("/") || line.Length <= 1)
                    continue;
                else
                    info.Add(line);
            }
            file.Close();
            frmFile = true;
            for (int i = 0; i < info.Count; ++i)
            {
                if (info[i].Contains("{main}"))
                {
                    while (!info[i].Contains("{end}"))
                    {
                        if (info[i].Contains("_colors") && info[i].Contains("["))
                            FVDNameRate(info[i]);

                        ++i;
                    }
                    if (fileName.Count > 0)
                    {
                        sprite.Add(new Sprite());
                        for (int j = 0; j < fileName.Count; j++)
                        {
                            frameCenter.Add(ReadFRM(path, fileName[j]));
                            sprite[sprite.Count - 1].frame.Add(AssignFrames(j));
                        }
                        //frmFile = true;
                    }
                    fileName.Clear();
                    frameCenter.Clear();
                    frameRate.Clear();
                }

                if (info[i].Contains("{effect}"))
                {
                    int layer = 0;
                    int startF = 0;
                    string startN = "";
                    info[i] = info[i].Trim();
                    while (!info[i].Contains("{end}"))
                    {
                        if (info[i].StartsWith("layer:"))
                        {
                            string[] tmp = info[i].Split(' ');
                            tmp[1] = tmp[1].Trim();
                            layer = Convert.ToInt32(tmp[1]);
                        }

                        if (info[i].StartsWith("start:"))
                        {
                            string[] tmp = info[i].Split(' ');
                            startN = tmp[1];

                            if (info[i].Contains("@"))
                            {
                                tmp[2] = tmp[2].TrimStart('@');
                                startF = Convert.ToInt32(tmp[2]);
                            }
                        }

                        if (info[i].Contains("_colors") && info[i].Contains("["))
                            FVDNameRate(info[i]);

                        ++i;
                    }
                    if (fileName.Count > 0)
                    {
                        sprite.Add(new Sprite(layer, startN, startF));
                        for (int j = 0; j < fileName.Count; j++)
                        {
                            frameCenter.Add(ReadFRM(path, fileName[j]));
                            sprite[sprite.Count - 1].frame.Add(AssignFrames(j));
                        }
                        //frmFile = true;
                    }
                    fileName.Clear();
                    frameCenter.Clear();
                    frameRate.Clear();
                }
            }
            if (fileName.Count > 0)
            {
                for (int i = 0; i < fileName.Count; i++)
                    frameCenter.Add(ReadFRM(path, fileName[i]));

                frmFile = true;
            }
            //Make a better way of assigning & loading frames
            //for (int i = 0; i < fileName.Count; i++)
            //    sprite[0].frame.Add(AssignFrames(i));
        }
        public void FVDNameRate(string line)
        {
            line = line.Trim();

            string[] tmp = line.Split(' ');
            fileName.Add(tmp[0] + ".pcx");

            int a = tmp[1].IndexOf("]") - tmp[1].IndexOf("[") - 1;
            string rate = tmp[1].Substring(tmp[1].IndexOf("[") + 1, a);
            if (rate == "x" || rate == "X")
                rate = "30";

            frameRate.Add(Convert.ToInt32(rate));
        }
        public Vector2 ReadFRM(string path, string name)
        {
            name = name.Remove(name.Length - 4);
            Console.WriteLine("Reading: " + name + ".frm");
            string line;
            StreamReader file = new StreamReader(path + name + ".frm");
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("#") || line.StartsWith("/") || line.StartsWith("(") || line.StartsWith("-"))
                { continue; }
                else if (line.Contains("Center:"))
                {
                    line = line.Remove(0, 8);
                    break;
                }
                else
                { continue; }
            }
            file.Close();
            Vector2 center = Vector2.Zero;
            string[] tmp = line.Split(' ');
            center.X = Convert.ToInt32(tmp[0]);
            center.Y = Convert.ToInt32(tmp[1]);
            return center;
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
        public Frame AssignFrames(int i)
        {
            iSizeX = 1;
            iSizeY = 1;

            Vector2 center;

            Frame frame;
            Color[] masterI = new Color[iSizeX * iSizeY];

            bool hasLine = true;
            bool hasShade = true;
            bool hasColor = true;
            int setNum = 1;
            string name = "";

            if (fileName[i].EndsWith("_colors.pcx") || fileName[i].EndsWith("_colors.png"))
            {
                name = fileName[i].Remove(fileName[i].Length - 11);
            }
            else if (fileName[i].EndsWith("_colors2.pcx") || fileName[i].EndsWith("_colors2.png"))
            {
                name = fileName[i].Remove(fileName[i].Length - 12);
                setNum = 2;
            }
            else if (fileName[i].EndsWith("_colors3.pcx") || fileName[i].EndsWith("_colors3.png"))
            {
                name = fileName[i].Remove(fileName[i].Length - 12);
                setNum = 3;
            }

            //Determine when a color2 is taken in.
            if (setNum == 2)
            {
                if (File.Exists(path + name + "_colors2.pcx"))
                {
                    AssignImage(ref masterI, 2, path, name, "_colors2.pcx");
                }
                else if (File.Exists(path + name + "_colors2.png"))
                {
                    AssignImage(ref masterI, 2, path, name, "_colors2.png");
                }
                else
                {
                    Console.WriteLine("FILE " + fileName[i] + "_colors2 NOT FOUND");
                    hasColor = false;
                }

                if (File.Exists(path + name + "_lines2.pcx"))
                {
                    AssignImage(ref masterI, 1, path, name, "_lines2.pcx");
                }
                else if (File.Exists(path + name + "_lines2.png"))
                {
                    AssignImage(ref masterI, 1, path, name, "_lines2.png");
                }
                else
                {
                    Console.WriteLine("FILE " + fileName[i] + "_lines2 NOT FOUND");
                    hasLine = false;
                }

                if (File.Exists(path + name + "_shade2.pcx"))
                {
                    AssignImage(ref masterI, 3, path, name, "_shade2.pcx");
                }
                else if (File.Exists(path + name + "_shade2.png"))
                {
                    AssignImage(ref masterI, 3, path, name, "_shade2.png");
                }
                else
                {
                    Console.WriteLine("FILE " + fileName[i] + "_shade2 NOT FOUND");
                    hasShade = false;
                }

                if (File.Exists(path + name + "_dye2.pcx"))
                {
                    AssignImage(ref masterI, 4, path, name, "_dye2.pcx");
                }
                else if (File.Exists(path + name + "_dye2.png"))
                {
                    AssignImage(ref masterI, 4, path, name, "_dye2.png");
                }
                else
                {
                    Console.WriteLine("FILE " + fileName[i] + "_dye2 NOT FOUND");
                }
            }
            else
            {
                if (File.Exists(path + name + "_colors.pcx"))
                {
                    AssignImage(ref masterI, 2, path, name, "_colors.pcx");
                }
                else if (File.Exists(path + name + "_colors.png"))
                {
                    AssignImage(ref masterI, 2, path, name, "_colors.png");
                }
                else
                {
                    Console.WriteLine("FILE " + fileName[i] + "_colors NOT FOUND");
                    hasColor = false;
                }

                if (File.Exists(path + name + "_lines.pcx"))
                {
                    AssignImage(ref masterI, 1, path, name, "_lines.pcx");
                }
                else if (File.Exists(path + name + "_lines.png"))
                {
                    AssignImage(ref masterI, 1, path, name, "_lines.png");
                }
                else
                {
                    Console.WriteLine("FILE " + fileName[i] + "_lines NOT FOUND");
                    hasLine = false;
                }

                if (File.Exists(path + name + "_shade.pcx"))
                {
                    AssignImage(ref masterI, 3, path, name, "_shade.pcx");
                }
                else if (File.Exists(path + name + "_shade.png"))
                {
                    AssignImage(ref masterI, 3, path, name, "_shade.png");
                }
                else
                {
                    Console.WriteLine("FILE " + fileName[i] + "_shade NOT FOUND");
                    hasShade = false;
                }

                if (File.Exists(path + name + "_dye.pcx"))
                {
                    AssignImage(ref masterI, 4, path, name, "_dye.pcx");
                }
                else if (File.Exists(path + name + "_dye.png"))
                {
                    AssignImage(ref masterI, 4, path, name, "_dye.png");
                }
                else
                {
                    Console.WriteLine("FILE " + fileName[i] + "_dye NOT FOUND");
                }

            }

            if (!hasColor)
                for (int c = 0; c < masterI.Length; c++)
                    masterI[c].G = 255;
            if (!hasLine)
                for (int c = 0; c < masterI.Length; c++)
                    masterI[c].R = 255;
            if (!hasShade)
                for (int c = 0; c < masterI.Length; c++)
                    masterI[c].B = 0;

            if (!txtFile)
                frameRate.Add(4);
            if (!frmFile)
                frameCenter.Add(new Vector2((int)iSizeX / 2, (int)iSizeY * .9f));

            //Size it
            if (iSizeX > 1 || iSizeY > 1)
                if (spriteSize > 1)
                {
                    HalfSizeImage(ref masterI);
                    frameCenter[i] = new Vector2((int)(frameCenter[i].X / spriteSize), (int)(frameCenter[i].Y / spriteSize));
                }

            //Create a Texture2D
            center = frameCenter[i];
            CropImage(ref masterI, ref center);
            frameCenter[i] = center;

            Texture2D textureOut;

            int swap = 1;
            if (swap == 0)
            {
                Bgra4444[] mBGRA = new Bgra4444[masterI.Length];
                for (int m = 0; m < masterI.Length; m++)
                {
                    float b = (masterI[m].B + 1) / 2;
                    float g = (masterI[m].G + 1) / 2;
                    float r = (masterI[m].R + 1) / 2;
                    mBGRA[m] = new Bgra4444(r, g, b, 0);
                }
                textureOut = new Texture2D(lGraphics.GraphicsDevice, iSizeX, iSizeY, false, SurfaceFormat.Bgra4444);
                textureOut.SetData(mBGRA);
            }
            else
            {
                textureOut = new Texture2D(lGraphics.GraphicsDevice, iSizeX, iSizeY, false, SurfaceFormat.Color);
                textureOut.SetData(masterI);
            }
            //SaveToPNG(textureOut, path + "..\\out\\", fileName[i] + "_merged.png");

            masterI = null;
            iSizeX = 1;
            iSizeY = 1;

            frame = new Frame(name, frameCenter[i], frameRate[i], hasColor, hasLine, hasShade, textureOut);
            return frame;
        }
        static bool IsExt(string f, string e)
        {
            return f != null && f.EndsWith(e, StringComparison.Ordinal);
        }
        void CropImage(ref Color[] masterI, ref Vector2 center)
        {
            int brdr = 1;
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
            center.X -= x1;
            center.Y -= y1;
            masterI = new Color[iSizeX * iSizeY];
            masterI = cropI;
        }
        void AssignImage(ref Color[] masterI, byte type, string p, string f, string l)    
        {
            if (l.EndsWith(".png"))
                ConvertPNG(ref masterI, type, p, f + l);
            else
                ConvertPCX(ref masterI, type, p, f + l);
        }
        void HalfSizeImage(ref Color[] masterI)
        {
            int hSizeX = (int)(iSizeX / spriteSize);
            int hSizeY = (int)(iSizeY / spriteSize);

            Color[] hColor = new Color[hSizeX * hSizeY];
            for (int y = 0; y < hSizeY; y++)
            {
                for (int x = 0; x < hSizeX; x++)
                {
                    int sX = x * spriteSize;
                    int sY = y * iSizeX * spriteSize;
                    hColor[x + (y * hSizeX)].R = (byte)((masterI[sX + sY].R + masterI[sX + sY + 1].R + masterI[sX + sY + iSizeX].R + masterI[sX + sY + 1 + iSizeX].R) / 4);
                    hColor[x + (y * hSizeX)].B = (byte)((masterI[sX + sY].B + masterI[sX + sY + 1].B + masterI[sX + sY + iSizeX].B + masterI[sX + sY + 1 + iSizeX].B) / 4);

                    hColor[x + (y * hSizeX)].G = masterI[sX + sY].G;
                    hColor[x + (y * hSizeX)].A = masterI[sX + sY].A;
                }
            }
            iSizeX = hSizeX;
            iSizeY = hSizeY;

            masterI = new Color[hSizeX * hSizeY];
            masterI = hColor;
        }
        void ConvertPNG(ref Color[] masterI, byte type, string path, string ImageName)
        {
            //Get the PNG texture, Slow method
            Texture2D texture;
            Stream fileStream = new FileStream(path + ImageName, FileMode.Open, FileAccess.Read, FileShare.Read);
            texture = Texture2D.FromStream(lGraphics.GraphicsDevice, fileStream);
            fileStream.Close();

            iSizeX = texture.Width;
            iSizeY = texture.Height;

            //masterI
            Color[] colTemp = new Color[iSizeX * iSizeY];
            texture.GetData(colTemp);

            //Add to the Master Image
            float shdCon = 255f / 153f;

            if (masterI.Length <= 1)
                masterI = new Color[iSizeX * iSizeY];

            for (int i = 0; i < masterI.Length; i++)
            {
                if (type == 1)
                {
                    //Embed Line info to the Red channel
                    masterI[i].R = colTemp[i].R;
                }
                else if (type == 2)
                {
                    //Embed Pal Color info to the Green channel
                    for (int c = 0; c < lPalette.pmapColor.Length; c++)
                    {
                        if (colTemp[i] == lPalette.pmapColor[c])
                        {
                            masterI[i].G = (byte)((256 / lPalette.div) * c);
                        }
                    }
                    //Create a height map from the color info
                    if (colTemp[i] == Color.White)
                        masterI[i].A = 0;
                    else
                        masterI[i].A = 130;
                }
                else if (type == 3)
                {
                    if (colTemp[i].B < 102)
                        colTemp[i].B = 102;
                    masterI[i].B = (byte)((colTemp[i].B - 102) * shdCon);
                }
                else if (type == 4)
                {
                    //Embed Pal Color info to the Alpha channel
                    for (int c = 0; c < lPalette.pmapColor.Length; c++)
                    {
                        if (colTemp[i] == lPalette.pmapColor[c])
                        {
                            masterI[i].A = (byte)((256 / lPalette.div) * c);
                        }
                    }
                }
            }

            Console.WriteLine("Loaded " + ImageName);
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
                if (type == 2)
                {
                    //Embed Pal Color info to the Green channel
                    for (int c = 0; c < lPalette.pmapColor.Length; c++)
                    {
                        if (colTemp == lPalette.pmapColor[c])
                        {
                            masterI[i].G = (byte)((256 / lPalette.div) * c);
                        }
                    }
                }
                else if (type == 1)
                {
                    //Embed Line info to the Red channel
                    masterI[i].R = colTemp.R;
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
                    for (int c = 0; c < lPalette.pmapColor.Length; c++)
                    {
                        if (colTemp == lPalette.pmapColor[c])
                        {
                            masterI[i].A = (byte)((256 / lPalette.div) * c);
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
            if (!Directory.Exists(f))
                Directory.CreateDirectory(f);
            using (Stream stream = File.OpenWrite(f + s))
            {
                t.SaveAsPng(stream, t.Width, t.Height);
                Console.WriteLine("Saved texture " + s);
            }
        }
        bool IsOdd(int value)
        {
            return value % 2 != 0;
        }
    }
}