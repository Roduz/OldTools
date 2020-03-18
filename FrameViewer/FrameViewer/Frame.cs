using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameViewer
{
    public class Frame
    {
        Vector2 offset;
        Vector2 imageZero;
        Vector2 imageSize;

        string name;
        int rate;

        bool hasColor = true;
        bool hasShade = true;
        bool hasLine = true;

        Vector2 imageCenter = Vector2.Zero;
        Vector2 origin = Vector2.Zero;

        Texture2D image;

        public Frame(string name, Vector2 center, int rate, bool hasColor, bool hasLine, bool hasShade, Texture2D image)
        {
            this.rate = rate;
            this.name = name;
            this.image = image;
            this.hasColor = hasColor;
            this.hasShade = hasShade;
            this.hasLine = hasLine;

            this.origin = new Vector2((int)center.X, (int)center.Y);

            imageSize = new Vector2(image.Width, image.Height);

            //UpdateFrame();
        }

        public void UpdateFrame()
        {
            offset = new Vector2(-origin.X, -origin.Y);
            imageZero = new Vector2(imageSize.X - origin.X, imageSize.Y - origin.Y);
            imageCenter = new Vector2(imageSize.X / 2, imageSize.Y / 2) + offset;
        }

        public int Rate
        { get { return rate; } }

        public string Name
        { get { return name; } }

        public Vector2 Center
        { get { return imageCenter; } }

        public Vector2 Size
        { get { return imageSize; } }

        public Vector2 Offset
        { get { return offset; } }

        public Vector2 Origin
        { get { return origin; } set { origin = value; } }

        public bool HasColor
        { get { return hasColor; } }

        public bool HasLine
        { get { return hasLine; } }

        public bool HasShade
        { get { return hasShade; } }

        public Texture2D ImageTexture
        { get { return image; } }
    }
}