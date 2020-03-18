using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameViewer
{
    class TileSprite
    {
        Vector2 offset;
        Vector2 imageZero;
        Vector2 imageSize;

        bool hasColor = true;
        bool hasShade = true;
        bool hasLine = true;

        Vector2 imageCenter = Vector2.Zero;
        Vector2 origin = Vector2.Zero;

        public List<Point> point = new List<Point>();

        Texture2D image;

        VertexPositionColorTexture[] frameVertex = new VertexPositionColorTexture[4];

        VertexPositionColor[] originVertex = new VertexPositionColor[4];

        public TileSprite(Vector2 center, bool hasColor, bool hasLine, bool hasShade, Texture2D image)
        {
            this.image = image;
            this.hasColor = hasColor;
            this.hasShade = hasShade;
            this.hasLine = hasLine;

            this.origin = center;

            imageSize = new Vector2(image.Width, image.Height);

            UpdateFrame();
        }

        public void UpdateFrame()
        {
            offset = new Vector2(-origin.X, -origin.Y);
            imageZero = new Vector2(imageSize.X - origin.X, imageSize.Y - origin.Y);
            imageCenter = new Vector2(imageSize.X / 2, imageSize.Y / 2) + offset;
            SetupFrameVertices();
        }

        public void SetupFrameVertices()
        {
            //TOP_LEFT
            frameVertex[0] = new VertexPositionColorTexture(new Vector3(offset.X, offset.Y, 0), Color.White, Vector2.Zero);
            //TOP_RIGHT
            frameVertex[1] = new VertexPositionColorTexture(new Vector3(imageZero.X, offset.Y, 0), Color.White, new Vector2(1, 0));
            //BOTTOM_RIGHT
            frameVertex[2] = new VertexPositionColorTexture(new Vector3(imageZero.X, imageZero.Y, 0), Color.White, Vector2.One);
            //BOTTOM_LEFT
            frameVertex[3] = new VertexPositionColorTexture(new Vector3(offset.X, imageZero.Y, 0), Color.White, new Vector2(0, 1));
        }
    }
}