using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HitboxEditor
{
    class Camera2D
    {
        float ratio;
        Vector3 position = new Vector3(0,0,0);
        Matrix projection;
        Matrix view;
        float fieldOfView = 1.0f;

        public Camera2D(Vector3 position, float ratio, float fieldOfView)
        {
            this.position = position;
            this.ratio = ratio;
            this.fieldOfView = fieldOfView;

            UpdateCamera(ratio);
        }

        public void UpdateCamera(float ratio)
        {
            //Projection
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fieldOfView), ratio, 1.0f, 1200.0f);
            //View
            view = Matrix.CreateLookAt(position, new Vector3(position.X, position.Y, 0), Vector3.Down);
        }

        public Vector3 Position
        { get { return position; } set { position = value; } }

        public Matrix View
        { get { return view; } }

        public Matrix Projection
        { get { return projection; } }

        public float FieldView
        { get { return fieldOfView; } set { fieldOfView = value; } }
    }
}