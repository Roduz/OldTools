using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HitboxEditor
{
    class HitBox
    {
        int type;
        int x1;
        int x2;
        int y1;
        int y2;
        int width;
        int height;

        Vector2 boxCenter;
        Rectangle boundingBox;
        VertexPositionColor[] vertexBox = new VertexPositionColor[4];
        VertexPositionColor[] vertexCenter = new VertexPositionColor[4];

        bool selected = false;

        public HitBox(int type, int x1, int y1, int x2, int y2)
        {
            this.type = type;
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;

            UpdateVertex();
        }

        public void UpdateVertex()
        {
            FindCenter();

            boundingBox = new Rectangle((int)boxCenter.X - 8, (int)boxCenter.Y - 8, 16, 16);

            Color color = Color.White;
            if (Type == 0)
            { color = Color.Red; }
            else if (Type == 1)
            { color = Color.Green; }
            else if (Type == 2)
            { color = Color.MediumBlue; }
            else if (Type == 3)
            { color = Color.White; }
            else if (Type == 4)
            { color = Color.Black; }
            else
            { color = Color.Transparent; }

            //CREATE THE SOLID CENTER BOX
            //TOP_LEFT
            vertexCenter[0] = new VertexPositionColor(new Vector3(boxCenter.X - 2, boxCenter.Y + 2, 0), color);
            //TOP_RIGHT
            vertexCenter[1] = new VertexPositionColor(new Vector3(boxCenter.X + 2, boxCenter.Y + 2, 0), color);
            //BOTTOM_RIGHT
            vertexCenter[2] = new VertexPositionColor(new Vector3(boxCenter.X + 2, boxCenter.Y - 2, 0), color);
            //BOTTOM_LEFT
            vertexCenter[3] = new VertexPositionColor(new Vector3(boxCenter.X - 2, boxCenter.Y - 2, 0), color);

            //CREATE THE TRANSPARENT HITBOX AND THE OUTLINE
            if (selected == true)
                color = Color.Gold;

            color.A = 50;

            //TOP_LEFT
            vertexBox[0] = new VertexPositionColor(new Vector3(X1, Y1, 0), color);
            //TOP_RIGHT
            vertexBox[1] = new VertexPositionColor(new Vector3(X2, Y1, 0), color);
            //BOTTOM_RIGHT
            vertexBox[2] = new VertexPositionColor(new Vector3(X2, Y2, 0), color);
            //BOTTOM_LEFT
            vertexBox[3] = new VertexPositionColor(new Vector3(X1, Y2, 0), color);
        }

        private void FindCenter()
        {
            width = Math.Abs(x1 - x2);
            height = Math.Abs(y1 - y2);
            boxCenter = new Vector2(x1 + (width / 2), y1 + (height / 2));
        }

        public void MoveHitBox(Vector2 NewCenter)
        {
            boxCenter = NewCenter;
            x1 = (int)boxCenter.X - (width / 2);
            y1 = (int)boxCenter.Y - (height / 2);
            x2 = x1 + width;
            y2 = y1 + height;
            UpdateVertex();
        }

        public int X1
        { get { return x1; } }
        public int Y1
        { get { return y1; } }
        public int X2
        { get { return x2; } }
        public int Y2
        { get { return y2; } }
        public int Type
        { get { return type; } }
        public int Width
        { get { width = Math.Abs(x1 - x2); return width; } }
        public int Height
        { get { height = Math.Abs(y1 - y2); return height; } }
        public VertexPositionColor[] VertexBox
        { get { return vertexBox; } }
        public VertexPositionColor[] VertexCenter
        { get { return vertexCenter; } }
        public bool Selected
        { get { return selected; } set { selected = value; } }
        public Vector2 Center
        { get { return boxCenter; } }
        public Rectangle BoundingBox
        { get { return boundingBox; } }
    }
}