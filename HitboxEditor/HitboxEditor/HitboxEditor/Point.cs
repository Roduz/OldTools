using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HitboxEditor
{
    class Point
    {
        Vector2 position;
        Vector2 origin;
        string name;
        int cNum;
        List<Color> color = new List<Color>();
        bool selected = false;
        VertexPositionColor[] pointVertex = new VertexPositionColor[4];

        public Point(string name, Vector2 position, Vector2 origin, int cNum)
        {
            this.position = position;
            this.name = name;
            this.origin = origin;
            this.cNum = cNum;

            if (cNum > 4)
                cNum = cNum % 5;

            color.Add(Color.Red);
            color.Add(Color.Orange);
            color.Add(Color.Yellow);
            color.Add(Color.Blue);
            color.Add(Color.Fuchsia);

            UpdatePoint();
        }

        public void UpdatePoint()
        {
            int x = 0; int y = 0;
            x = (int)(-origin.X + position.X);
            y = (int)(-origin.Y + position.Y);

            if (selected)
            {
                //UP
                pointVertex[0] = new VertexPositionColor(new Vector3(x, y + 6, 0), Color.HotPink);
                //RIGHT
                pointVertex[1] = new VertexPositionColor(new Vector3(x + 6, y, 0), Color.HotPink);
                //DOWN
                pointVertex[2] = new VertexPositionColor(new Vector3(x, y - 6, 0), Color.HotPink);
                //LEFT
                pointVertex[3] = new VertexPositionColor(new Vector3(x - 6, y, 0), Color.HotPink);
            }
            else
            {
                //UP
                pointVertex[0] = new VertexPositionColor(new Vector3(x, y + 6, 0), color[cNum]);
                //RIGHT
                pointVertex[1] = new VertexPositionColor(new Vector3(x + 6, y, 0), color[cNum]);
                //DOWN
                pointVertex[2] = new VertexPositionColor(new Vector3(x, y - 6, 0), color[cNum]);
                //LEFT
                pointVertex[3] = new VertexPositionColor(new Vector3(x - 6, y, 0), color[cNum]);
            }
        }

        public bool Selected
        { get { return selected; } set { selected = value; } }

        public VertexPositionColor[] PointVertex
        { get { return pointVertex; } }

        public Vector2 Position
        { get { return position; } }

        public string Name
        { get { return name; } }
    }
}
