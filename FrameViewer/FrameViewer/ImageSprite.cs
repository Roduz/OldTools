using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameViewer
{
    class ImageSprite
    {
        List<Color> colorArray = new List<Color>();
        List<int> indexArray = new List<int>();
        int width;
        int height;

        public ImageSprite(List<Color> colorArray, List<int> indexArray, int width, int height)
        {
            this.colorArray = colorArray;
            this.indexArray = indexArray;
            this.width = width;
            this.height = height;
        }

        public List<Color> ColorArray
        { get { return colorArray; } }

        public List<int> IndexArray
        { get { return indexArray; } }

        public int Width
        { get { return width; } }

        public int Height
        { get { return height; } }
    }
}

/*
        private Texture2D ConvertImageSprite(ImageSprite imageCode)
        {
            Texture2D textureOut;

            //Create a Texture2D
            textureOut = new Texture2D(graphics.GraphicsDevice, imageCode.Width, imageCode.Height, false, SurfaceFormat.Color);
            Color[] color = new Color[imageCode.Width * imageCode.Height];

            int t = 0;
            int i = 0;
            int a = 0;

            while (i < color.Length)
            {
                for (a = 0; a < imageCode.IndexArray.Count; a++)
                {
                    for (t = 0; t < imageCode.IndexArray[a]; t++)
                    {
                        if (i < color.Length)
                        {
                            color[i] = imageCode.ColorArray[a];
                            i++;
                        }
                    }
                }
            }

            textureOut.SetData(color);
            return textureOut;
        }
*/

/*
            //Create a ImageSprite
            List<Color> colorArray = new List<Color>();
            List<int> indexArray = new List<int>();
            Color colorC;
            Color colorP = new Color(colorRGB[(int)imageBytes[0]].R, colorRGB[(int)imageBytes[0]].G, colorRGB[(int)imageBytes[0]].B, 255);
            int count = 0;
            int track = 0;

            colorArray.Add(colorP);
            indexArray.Add(count);

            for (int i = 0; i < xSize * ySize; i++)
            {
                colorC = new Color(colorRGB[(int)imageBytes[i]].R, colorRGB[(int)imageBytes[i]].G, colorRGB[(int)imageBytes[i]].B, 255);

                if (colorC != colorP)
                {
                    count = 1;
                    colorArray.Add(colorC);
                    indexArray.Add(count);
                    track++;
                }
                else
                {
                    count++;
                    indexArray[track] = count;
                }

                colorP = colorC;
            }
            textureOut = new ImageSprite(colorArray, indexArray, xSize, ySize);
*/