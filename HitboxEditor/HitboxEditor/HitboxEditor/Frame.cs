using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HitboxEditor
{
    class Frame
    {
        Vector2 offset;
        Vector2 imageZero;

        string name;
        int rate;
        bool hasHitbox = false;

        Vector2 imageCenter;
        Vector2 origin;

        public List<string> info = new List<string>();
        public List<HitBox> hitbox = new List<HitBox>();
        public List<Point> point = new List<Point>();

        Texture2D frameTexture;
        VertexPositionColorTexture[] frameVertex = new VertexPositionColorTexture[4];

        VertexPositionColor[] originVertex = new VertexPositionColor[4];

        public Frame(Texture2D frameTexture, List<string> info, string name, int rate)
        {
            this.rate = rate;
            this.name = name;
            this.info = info;
            this.frameTexture = frameTexture;

            ConvertInfo();
            UpdateFrame();
        }

        public void UpdateFrame()
        {
            offset = new Vector2(-origin.X, -origin.Y);
            imageZero = new Vector2(frameTexture.Width - origin.X, frameTexture.Height - origin.Y);
            imageCenter = new Vector2(frameTexture.Width / 2, frameTexture.Height / 2) + offset;
            SetupFrameVertices();
            OriginMarker();
        }


        public void Update()
        {
            offset = new Vector2(-origin.X, -origin.Y);
            imageZero = new Vector2(frameTexture.Width - origin.X, frameTexture.Height - origin.Y);
            imageCenter = new Vector2(frameTexture.Width / 2, frameTexture.Height / 2) + offset;
            SetupFrameVertices();
            OriginMarker();

            for (int i = 0; i < hitbox.Count; i++)
            {
                hitbox[i].UpdateVertex();
            }

            for (int i = 0; i < point.Count; i++)
            {
                point[i].UpdatePoint();
            }
        }

        public void ConvertInfo()
        {
            for (int i = 0; i < info.Count; i++)
            {
                if (info[i].Contains("Center:"))
                {
                    string[] f = info[i].Split(' ');
                    origin = new Vector2(Convert.ToInt32(f[1]), Convert.ToInt32(f[2]));
                }
                if (info[i].Contains("Points:"))
                {
                    i += 1;
                    int cNum = 0;
                    while (info[i].Contains("[end]") == false)
                    {
                        string[] f = info[i].Split(' ');
                        point.Add(new Point(f[0], new Vector2(Convert.ToInt32(f[1]), Convert.ToInt32(f[2])), origin, cNum));
                        i += 1; cNum += 1;
                    }
                }
                if (info[i].Contains("Hitbox:"))
                {
                    i += 1;
                    while (info[i].Contains("[end]") == false)
                    {
                        string[] f = info[i].Split(' ');
                        hitbox.Add(new HitBox(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(f[2]), Convert.ToInt32(f[3]), Convert.ToInt32(f[4])));
                        i += 1;
                    }
                }
            }
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

        public void OriginMarker()
        {
            //UP
            originVertex[0] = new VertexPositionColor(new Vector3(0, 0 + 6, 0), Color.Black);
            //RIGHT
            originVertex[1] = new VertexPositionColor(new Vector3(0 + 6, 0, 0), Color.Black);
            //DOWN
            originVertex[2] = new VertexPositionColor(new Vector3(0, 0 - 6, 0), Color.Black);
            //LEFT
            originVertex[3] = new VertexPositionColor(new Vector3(0 - 6, 0, 0), Color.Black);
        }

        public int Rate
        { get { return rate; } }

        public string Name
        { get { return name; } }

        public bool HasHitbox
        { get { return hasHitbox; } set { hasHitbox = value; } }

        public Vector2 Center
        { get { return imageCenter; } }

        public Vector2 Offset
        { get { return offset; } }

        public Vector2 Origin
        { get { return origin; } set { origin = value; } }

        public Texture2D FrameTexture
        { get { return frameTexture; } }

        public VertexPositionColorTexture[] VertexFrame
        { get { return frameVertex; } }

        public VertexPositionColor[] OriginPoint
        { get { return originVertex; } }
    }
}
