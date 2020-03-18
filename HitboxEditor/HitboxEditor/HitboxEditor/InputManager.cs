using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HitboxEditor
{
    class InputManager
    {
        Vector2 mouseCoord;
        //Vector2 mouseClick;
        //Vector2 mouseDrag;

        public InputManager()//KeyboardState keyState, MouseState mouseState)
        {
            //this.keyState = keyState;
            //this.mouseState = mouseState;
        }

        public void InputCamera(ref MouseState mouseStatePrevious, ref Camera2D camera, ref Vector3 mouseSpace, ref Vector2 mouseClick, ref Vector2 mouseDrag, ref bool inView)
        {
            if (inView)
            {
                mouseCoord = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                // Camera manipulation and control
                // Scroll down, zoom out
                if (Mouse.GetState().ScrollWheelValue < mouseStatePrevious.ScrollWheelValue)
                {
                    if (camera.Position.Z > -1020.0f)
                        camera.Position -= new Vector3(0, 0, 40.0f);
                }
                // Scroll up, zoom in
                if (Mouse.GetState().ScrollWheelValue > mouseStatePrevious.ScrollWheelValue)
                {
                    if (camera.Position.Z < -20.0f)
                        camera.Position += new Vector3(0, 0, 40.0f);
                }
                // Middle mouse button press and pan the camera around
                if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
                {
                    mouseClick = new Vector2(mouseStatePrevious.X, mouseStatePrevious.Y);
                    mouseDrag.X = -(mouseClick.X - mouseCoord.X);
                    mouseDrag.Y = (mouseCoord.Y - mouseClick.Y);
                    camera.Position -= new Vector3(mouseDrag, 0);
                }
            }
        }

        public void Selection(ref KeyboardState keyStatePrevious, ref bool mode, ref int boxSelected)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && keyStatePrevious.IsKeyUp(Keys.Space))
            {
                if (mode)
                    mode = false;
                else
                    mode = true;
            }
        }

        public void InputShowPlay(ref KeyboardState keyStatePrevious, ref bool showFrame, ref bool play, ref int fNum, ref int boxSelected, int frameCount)
        {
            //Show Frame and Hitbox or else show help
            if (Keyboard.GetState().IsKeyDown(Keys.F1) && keyStatePrevious.IsKeyUp(Keys.F1))
            {
                if (showFrame)
                    showFrame = false;
                else
                    showFrame = true;
            }
            //Play Back
            if (Keyboard.GetState().IsKeyDown(Keys.P) && keyStatePrevious.IsKeyUp(Keys.P))
            {
                if (play)
                    play = false;
                else
                    play = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemCloseBrackets) && keyStatePrevious.IsKeyUp(Keys.OemCloseBrackets))
            {
                boxSelected = 0;
                if (fNum == frameCount - 1)
                    fNum = 0;
                else
                    fNum += 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemOpenBrackets) && keyStatePrevious.IsKeyUp(Keys.OemOpenBrackets))
            {
                boxSelected = 0;
                if (fNum == 0)
                    fNum = frameCount - 1;
                else
                    fNum -= 1;
            }
        }

        public void InputBoxType(ref int boxType)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
                boxType = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.D2))
                boxType = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D3))
                boxType = 2;
        }

        public void Update()
        {
            /*timeSinceLastInput += ((float)gameTime.ElapsedGameTime.TotalSeconds * 2);

            if (Keyboard.GetState().IsKeyDown(Keys.D9) && timeSinceLastInput >= MinTimeSinceLastInput)
            {
                boxType = 1;
                timeSinceLastInput = 0.0f;
                if (drawFrame)
                    drawFrame = false;
                else
                    drawFrame = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D0) && timeSinceLastInput >= MinTimeSinceLastInput)
            {
                boxType = 2;
                timeSinceLastInput = 0.0f;
                if (drawBoxes)
                    drawBoxes = false;
                else
                    drawBoxes = true;
            }

            //Cursor and hitbox detection
            if (!mouseHoldR && !keepSelect)
            {
                for (int i = 0; i < boxAmount; i++)
                {
                    hitbox[i].Selected = false;
                    if (hitbox[i].Detect.Intersects(mouseCursor))
                    {
                        boxSelected = i;
                        hitbox[boxSelected].Selected = true;
                        i = boxAmount;
                    }
                }
            }
            //Hitbox grab and move by click and drag on the right mouse button
            if (mouseStateCurrent.RightButton == ButtonState.Pressed && !mouseHoldR && !keepSelect && inView)
            {
                mouseHoldR = true;
            }
            if (mouseStateCurrent.RightButton == ButtonState.Pressed && mouseHoldR && hitbox[boxSelected].Selected && inView)
            {
                hitbox[boxSelected].Center = originCoord;
            }
            if (mouseStateCurrent.RightButton == ButtonState.Released && mouseHoldR && inView)
            {
                mouseHoldR = false;
                hitbox[boxSelected].Selected = false;
            }
            //Hitbox Selection mode for Editing
            if ((Keyboard.GetState().IsKeyDown(Keys.Space)) && hitbox[boxSelected].Selected && !keepSelect && timeSinceLastInput >= MinTimeSinceLastInput)
            {
                keepSelect = true;
                timeSinceLastInput = 0.0f;
            }
            if ((Keyboard.GetState().IsKeyDown(Keys.Space)) && hitbox[boxSelected].Selected && keepSelect && timeSinceLastInput >= MinTimeSinceLastInput)
            {
                hitbox[boxSelected].Selected = false;
                keepSelect = false;
                timeSinceLastInput = 0.0f;
            }
            //Hitbox Delete
            if (((Keyboard.GetState().IsKeyDown(Keys.Back)) || (Keyboard.GetState().IsKeyDown(Keys.Delete))) && hitbox[boxSelected].Selected
                && keepSelect && timeSinceLastInput >= MinTimeSinceLastInput)
            {
                hitbox[boxSelected] = hitbox[boxAmount - 1];
                boxTexture[boxSelected] = boxTexture[boxAmount - 1];
                if (boxAmount <= 0)
                {
                    boxAmount = 0;
                }
                else
                    boxAmount -= 1;
                drawAmount = boxAmount;
                keepSelect = false;
                timeSinceLastInput = 0.0f;
            }
            */
        }
    }
}
