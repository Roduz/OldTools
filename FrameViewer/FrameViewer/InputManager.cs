using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrameViewer
{
    class InputManager
    {
        Vector2 mouseCoord;

        public InputManager()
        {
        }

        public void InputCamera(ref MouseState mouseStatePrevious, ref Camera2D camera, ref Vector2 mouseClick, ref Vector2 mouseDrag)
        {
            mouseCoord = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            // Camera manipulation and control
            // Scroll down, zoom out
            if (Mouse.GetState().ScrollWheelValue < mouseStatePrevious.ScrollWheelValue)
            {
                if (camera.Zoom > .5f)
                    camera.Zoom -= .1f;
                else
                    camera.Zoom = .5f;

                camera.Zoom = (float)Math.Round(camera.Zoom, 2, MidpointRounding.AwayFromZero);
            }
            // Scroll up, zoom in
            if (Mouse.GetState().ScrollWheelValue > mouseStatePrevious.ScrollWheelValue)
            {
                if (camera.Zoom < 4.0f)
                    camera.Zoom += .1f;
                else
                    camera.Zoom = 4.0f;

                camera.Zoom = (float)Math.Round(camera.Zoom, 2, MidpointRounding.AwayFromZero);
            }
            // Middle mouse button press and pan the camera around
            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                mouseClick = new Vector2(mouseStatePrevious.X, mouseStatePrevious.Y);
                mouseDrag.X = -(mouseClick.X - mouseCoord.X);
                mouseDrag.Y = (mouseCoord.Y - mouseClick.Y);
                camera.Position -= mouseDrag;
            }
        }
    }
}
