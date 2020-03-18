using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HitboxEditor
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        BasicEffect effect;
        SpriteBatch spriteBatch;

        FileLoader fileLoader;
        string path;

        int[] indexData;
        int[] indexLine;
        int[] indexPoint;

        List<Frame> frame = new List<Frame>();

        Matrix originMatrix = Matrix.Identity;

        Camera2D camera;

        KeyboardState keyStatePrevious;
        MouseState mouseStatePrevious;

        InputManager input;

        //Vector2 mouseCoord;
        Vector2 mouseClick;
        Vector2 mouseDrag;
        Vector3 mouseSpace;
        Vector2 imageCoord;
        Rectangle mouseCursor;

        bool showFrame = true;

        bool mouseHoldR = false;
        bool mouseHoldL = false;

        bool mode = true;
        int boxSelected;

        HitBox displayBox = new HitBox(6, 0, 0, 0, 0);
        int boxType = 1;

        bool isActive = false;
        bool inView = true;
        bool play = false;
        int fNum = 0;
        int pNum = 0;
        int fCount = 1;

        //time since last FPS update in seconds
        float fpsTime = 0;

        //Font Varibles
        SpriteFont font;
        string textoutput = "";
        string modeOutput = "";
        string coordOutput = "";
        string pointOutput = "";
        string helpOutput = "";

        public Game1(string path)
        {
            this.path = path;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = true;

            isActive = this.IsActive;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");

            fileLoader = new FileLoader(graphics, path);
            input = new InputManager();
             
            // Basic Effect for 3D rendering
            effect = new BasicEffect(graphics.GraphicsDevice);

            // Sets the world to the this unique coordinate system
            originMatrix = Matrix.CreateRotationX((float)MathHelper.ToRadians(180));

            for (int i = 0; i < fileLoader.frameAmount; i++)
            {
                frame.Add(new Frame(fileLoader.AssignImage(path, fileLoader.fileName[i] + ".pcx"), fileLoader.ReadFRM(path, fileLoader.fileName[i]), fileLoader.fileName[i], fileLoader.frameRate[i]));
            }

            SetUpIndex();

            graphics.PreferredBackBufferWidth = (int)(GraphicsDevice.DisplayMode.Width / 1.5f);
            graphics.PreferredBackBufferHeight = (int)(GraphicsDevice.DisplayMode.Height / 1.5f);
            graphics.ApplyChanges();

            // Create the camera, the last thing you should initialize
            camera = new Camera2D(new Vector3(frame[0].Center, -420), GraphicsDevice.Viewport.AspectRatio, 100.0f);

            InitiateHelp();

            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            //Checks to see if it is the Active window.
            isActive = this.IsActive;
            //Checks to see if the mouse is over the window
            inView = (Mouse.GetState().X <= GraphicsDevice.Viewport.Width && Mouse.GetState().Y <= GraphicsDevice.Viewport.Height && Mouse.GetState().X >= 0 && Mouse.GetState().Y >= 0);
            //Gets hitbox and if zero it skips tryting to get info on them.

            pNum = fNum;

            if (frame[fNum].hitbox.Count > 0)
            {
                frame[fNum].HasHitbox = true;
            }
            else
                frame[fNum].HasHitbox = false;
            //Get mouse info
            mouseSpace = PointerPosition(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
            imageCoord = new Vector2(mouseSpace.X, mouseSpace.Y) - frame[fNum].Offset;
            mouseCursor = new Rectangle((int)mouseSpace.X - 4, (int)mouseSpace.Y - 4, 8, 8);

            if (isActive && inView)
            {
                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || (Keyboard.GetState().IsKeyDown(Keys.Escape)) && inView)
                    this.Exit();
                // Camera manipulation and control
                input.InputCamera(ref mouseStatePrevious, ref camera, ref mouseSpace, ref mouseClick, ref mouseDrag, ref inView);
                // UPDATE THE CAMERA TO SHOW NEW VIEW AND RESIZE
                camera.UpdateCamera(GraphicsDevice.Viewport.AspectRatio);
                // Play Back
                input.InputShowPlay(ref keyStatePrevious, ref showFrame, ref play, ref fNum, ref boxSelected, frame.Count);
                // SAVES TO FRM FILES
                SaveToFRM();
                if (!play)
                {
                    //Mode Selection Toggle
                    input.Selection(ref keyStatePrevious, ref mode, ref boxSelected);
                    //Hitbox Key detection and Selection
                    if (!mode) KeySelection();
                    //Hitbox Mouse detection and Selection
                    if (mode && !mouseHoldR && !mouseHoldL) MouseSelection();
                    // Changes the Type of Hitbox we want from keyboard keys
                    input.InputBoxType(ref boxType);
                    //Hitbox Creation with the Left Mouse Button, Hitbox Creation.
                    HitboxCreation();
                    //Hitbox grab and move by click and drag on the right mouse button
                    HitboxMove();
                    //Point grab and move by click and drag on the right mouse button and holding P
                    PointMove();
                    //Hitbox DELETE
                    DeleteHitbox();
                }
            }
            // Play Back
            if (play) Playback();
            //FRAME UPDATE
            frame[fNum].Update();
            // The time since Update was called last
            GetFPS(gameTime);
            // Updates the Real time number feedback and info text
            if (inView) TextOutput();
            // Previous input state for future comparison
            mouseStatePrevious = Mouse.GetState();
            keyStatePrevious = Keyboard.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(80, 80, 80, 255));

            if (showFrame)
            {
                //Draw textured plane
                GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.None;
                effect.View = camera.View;
                effect.Projection = camera.Projection;
                effect.VertexColorEnabled = true;

                effect.TextureEnabled = true;
                effect.CurrentTechnique.Passes[0].Apply();

                effect.Texture = frame[fNum].FrameTexture;
                if (pNum != fNum)
                    GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, frame[pNum].VertexFrame, 0, 4, indexData, 0, 2);
                else
                    GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, frame[fNum].VertexFrame, 0, 4, indexData, 0, 2);

                effect.TextureEnabled = false;
                effect.CurrentTechnique.Passes[0].Apply();

                //Draw the Hitboxes in the frame
                RenderHitbox(frame[fNum].hitbox);

                //Draw the origin point
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineStrip, frame[fNum].OriginPoint, 0, 4, indexPoint, 0, 7);

                //Draw the other points
                RenderPoint(frame[fNum].point);

                //Draw the text
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                //new Color(190, 0, 90, 255)
                spriteBatch.DrawString(font, textoutput, new Vector2(5, 0), Color.Black, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.End();
            }
            //Help text
            if (!showFrame)
            {
                //Draw the text
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                //new Color(190, 0, 90, 255)
                spriteBatch.DrawString(font, helpOutput, new Vector2(5, 0), Color.DarkRed, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void MouseSelection()
        {
            for (int i = 0; i < frame[fNum].hitbox.Count; i++)
            {
                frame[fNum].hitbox[i].Selected = false;
                if (frame[fNum].hitbox[i].BoundingBox.Intersects(mouseCursor))
                {
                    boxSelected = i;
                    frame[fNum].hitbox[boxSelected].Selected = true;
                    i = frame[fNum].hitbox.Count;
                }
            }
            for (int i = 0; i < frame[fNum].hitbox.Count; i++)
            {
                if (frame[fNum].hitbox[i].Selected && i != boxSelected)
                {
                    frame[fNum].hitbox[i].Selected = false;
                }
            }
        }

        private void KeySelection()
        {
            if (frame[fNum].hitbox.Count > 0)
            {
                for (int i = 0; i < frame[fNum].hitbox.Count; i++)
                {
                    frame[fNum].hitbox[i].Selected = false;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.OemComma) && keyStatePrevious.IsKeyUp(Keys.OemComma))
                {
                    if (boxSelected == 0)
                        boxSelected = frame[fNum].hitbox.Count - 1;
                    else
                        boxSelected -= 1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.OemPeriod) && keyStatePrevious.IsKeyUp(Keys.OemPeriod))
                {
                    if (boxSelected == frame[fNum].hitbox.Count - 1)
                        boxSelected = 0;
                    else
                        boxSelected += 1;
                }
                frame[fNum].hitbox[boxSelected].Selected = true;
            }
            else
                boxSelected = 0;
        }

        private void RenderHitbox(List<HitBox> hitbox)
        {
            for (int i = 0; i < hitbox.Count; i++)
            {
                //Draw the transparent fill
                GraphicsDevice.BlendState = BlendState.NonPremultiplied;
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, hitbox[i].VertexBox, 0, 4, indexData, 0, 2);
                //Draw the outline
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineStrip, hitbox[i].VertexBox, 0, 4, indexLine, 0, 4);
                //Draw the solid center
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, hitbox[i].VertexCenter, 0, 4, indexData, 0, 2);
            }
            //Draw the transparent fill
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, displayBox.VertexBox, 0, 4, indexData, 0, 2);
            //Draw the outline
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineStrip, displayBox.VertexBox, 0, 4, indexLine, 0, 4);
            //Draw the solid center
            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, displayBox.VertexCenter, 0, 4, indexData, 0, 2);
        }

        private void RenderPoint(List<Point> point)
        {
            for (int i = 0; i < point.Count; i++)
            {
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineStrip, frame[fNum].point[i].PointVertex, 0, 4, indexPoint, 0, 7);
            }
        }

        public void HitboxCreation()
        {
            //Hitbox Creation with the Left Mouse Button, Hitbox Creation.
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !mouseHoldL && inView)
            {
                mouseClick = new Vector2(mouseSpace.X, mouseSpace.Y);
                mouseHoldL = true;
            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && mouseHoldL && inView)
            {
                mouseDrag = new Vector2(mouseSpace.X, mouseSpace.Y);
                int a = (int)mouseClick.X; int b = (int)mouseClick.Y; int c = (int)mouseDrag.X; int d = (int)mouseDrag.Y;

                if ((int)mouseDrag.X < (int)mouseClick.X)
                {
                    a = (int)mouseDrag.X;
                    c = (int)mouseClick.X;
                }
                if ((int)mouseDrag.Y < (int)mouseClick.Y)
                {
                    b = (int)mouseDrag.Y;
                    d = (int)mouseClick.Y;
                }
                displayBox = new HitBox(boxType, a, b, c, d);
            }
            if (Mouse.GetState().LeftButton == ButtonState.Released && mouseHoldL && inView)
            {
                mouseHoldL = false;
                if (displayBox.Height > 10 && displayBox.Width > 10)
                {
                    if (mode || frame[fNum].hitbox.Count == 0)
                    {
                        frame[fNum].hitbox.Add(displayBox);
                    }
                    else
                    {
                        frame[fNum].hitbox[boxSelected] = displayBox;
                    }
                }
                displayBox = new HitBox(6, 0, 0, 0, 0);
            }
        }

        public void HitboxMove()
        {
            if (frame[fNum].HasHitbox)
            {
                //Hitbox grab and move by click and drag on the right mouse button
                if (frame[fNum].hitbox[boxSelected].Selected)
                {
                    if (Mouse.GetState().RightButton == ButtonState.Pressed && !mouseHoldR && inView)
                    {
                        mouseHoldR = true;
                    }
                    if (Mouse.GetState().RightButton == ButtonState.Pressed && mouseHoldR && inView)
                    {
                        mouseDrag = new Vector2(mouseSpace.X, mouseSpace.Y);
                        frame[fNum].hitbox[boxSelected].MoveHitBox(mouseDrag);
                    }
                    if (Mouse.GetState().RightButton == ButtonState.Released && mouseHoldR && inView)
                    {
                        mouseHoldR = false;
                    }
                }
            }
        }

        private void DeleteHitbox()
        {
            //Hitbox DELETE
            if (Keyboard.GetState().IsKeyDown(Keys.Delete) && keyStatePrevious.IsKeyUp(Keys.Delete) && (frame[fNum].hitbox.Count > 0))
            {
                if (frame[fNum].hitbox[boxSelected].Selected)
                {
                    frame[fNum].hitbox.RemoveAt(boxSelected);
                }
                if (boxSelected >= frame[fNum].hitbox.Count)
                    boxSelected = frame[fNum].hitbox.Count - 1;
            }
        }

        public void PointMove()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && Keyboard.GetState().IsKeyDown(Keys.LeftShift) && Keyboard.GetState().IsKeyDown(Keys.O) && keyStatePrevious.IsKeyUp(Keys.O))
                PointMoveType(true);
            else if ((Keyboard.GetState().IsKeyDown(Keys.LeftAlt) || Keyboard.GetState().IsKeyDown(Keys.RightAlt)) && Keyboard.GetState().IsKeyDown(Keys.O) && keyStatePrevious.IsKeyUp(Keys.O))
                PointMoveType(false);
            else if (Keyboard.GetState().IsKeyDown(Keys.O))
            {
                //Point grab and move by click and drag on the right mouse button
                if (Mouse.GetState().RightButton == ButtonState.Pressed && !mouseHoldR && inView)
                {
                    mouseHoldR = true;
                }
                if (Mouse.GetState().RightButton == ButtonState.Pressed && mouseHoldR && inView)
                {
                    mouseDrag = new Vector2(mouseSpace.X, mouseSpace.Y);
                    frame[fNum].Origin = new Vector2(mouseSpace.X, mouseSpace.Y);
                }
                if (Mouse.GetState().RightButton == ButtonState.Released && mouseHoldR && inView)
                {
                    mouseHoldR = false;
                }
            }
        }

        public void PointMoveType(bool all)
        {
            if (all)
                Console.WriteLine("All frames will be changed:");
            Console.WriteLine("Origin X: ");
            string line = Console.ReadLine(); // Read string from console
            int valueX;
            int.TryParse(line, out valueX);

            Console.WriteLine("Origin Y: ");
            line = Console.ReadLine();
            int valueY;
            int.TryParse(line, out valueY);

            if (all)
            {
                for (int i = 0; i < frame.Count; i++)
                {
                    Vector2 temp = new Vector2(frame[i].FrameTexture.Width - valueX, frame[i].FrameTexture.Height - valueY);
                    frame[i].Origin = temp;
                }
            }
            else
            {
                Vector2 temp = new Vector2(frame[fNum].FrameTexture.Width - valueX, frame[fNum].FrameTexture.Height - valueY);
                frame[fNum].Origin = temp;
            }

        }

        //Helper Functions
        private void SetUpIndex()
        {
            const int TOP_LEFT = 0;
            const int TOP_RIGHT = 1;
            const int BOTTOM_RIGHT = 2;
            const int BOTTOM_LEFT = 3;

            indexData = new int[6];
            indexData[0] = TOP_LEFT;
            indexData[1] = BOTTOM_RIGHT;
            indexData[2] = BOTTOM_LEFT;

            indexData[3] = TOP_LEFT;
            indexData[4] = TOP_RIGHT;
            indexData[5] = BOTTOM_RIGHT;

            indexLine = new int[5];
            indexLine[0] = TOP_LEFT;
            indexLine[1] = TOP_RIGHT;
            indexLine[2] = BOTTOM_RIGHT;
            indexLine[3] = BOTTOM_LEFT;
            indexLine[4] = TOP_LEFT;

            indexPoint = new int[8];
            indexPoint[0] = 0;
            indexPoint[1] = 1;
            indexPoint[2] = 3;
            indexPoint[3] = 2;
            indexPoint[4] = 0;
            indexPoint[5] = 3;
            indexPoint[6] = 1;
            indexPoint[7] = 2;
        }

        public void Playback()
        {
            if (frame[fNum].hitbox.Count > 0)
            {
                frame[fNum].hitbox[boxSelected].Selected = false;
                boxSelected = 0;
            }
            if (frame[fNum].Rate == fCount)
            {
                fCount = 1;
                if (fNum == frame.Count - 1)
                    fNum = 0;
                else
                    fNum += 1;
            }
            else
                fCount += 1;
        }

        public Vector3 PointerPosition(Vector2 mouse)
        {
            // create 2 positions in screenspace using the cursor position. 0 is as
            // close as possible to the camera, 10 is as far away as possible
            Vector3 nearSource = new Vector3(mouse, 1f);
            Vector3 farSource = new Vector3(mouse, 1200f);
            // find the two screen space positions in world space
            Vector3 nearPoint = graphics.GraphicsDevice.Viewport.Unproject(nearSource, camera.Projection, camera.View, Matrix.Identity);
            Vector3 farPoint = graphics.GraphicsDevice.Viewport.Unproject(farSource, camera.Projection, camera.View, Matrix.Identity);
            // compute normalized direction vector from nearPoint to farPoint
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            // create a ray using nearPoint as the source
            Ray r = new Ray(nearPoint, direction);
            // calculate the ray-plane intersection point
            Vector3 n = new Vector3(0f, 0f, 1f);
            Plane p = new Plane(n, 0f);
            // calculate distance of intersection point from r.origin
            float denominator = Vector3.Dot(p.Normal, r.Direction);
            float numerator = Vector3.Dot(p.Normal, r.Position) + p.D;
            float t = -(numerator / denominator);
            // calculate the picked position on the y = 0 plane
            Vector3 pointerPosition = nearPoint + direction * t;
            return pointerPosition;
        }

        public void GetFPS(GameTime gameTime)
        {
            // The time since Update was called last
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float fps = 1 / elapsed;
            fpsTime += elapsed;
            Window.Title = "Hitbox Editor  <" + fps.ToString() + "> FPS";
            if (fpsTime > 1)
            {
                fps.ToString();
                fpsTime -= 1;
            }
        }

        public void TextOutput()
        {
            // Draw Text Info
            modeOutput = (frame[fNum].Name + "\n");
            if (mode)
                modeOutput += ("Mode: Mouse\n");
            else
                modeOutput += ("Mode: Keys\n");

            if (frame[fNum].hitbox.Count == 0)
                modeOutput += ("Total: 0\n");
            else
                modeOutput += ("Total: " + frame[fNum].hitbox.Count + "\n");
            if (boxSelected > -1 && frame[fNum].HasHitbox)
            {
                if (frame[fNum].hitbox[boxSelected].Selected)
                    modeOutput += ("Hitbox: " + (int)(boxSelected + 1) + "\n");
                else
                    modeOutput += ("Hitbox: None\n");
            }
            else
                modeOutput += ("Hitbox: None\n");

            coordOutput = ("Image " + (int)imageCoord.X + " " + (int)imageCoord.Y + "\n" +
                "Space " + (int)mouseSpace.X + " " + (int)mouseSpace.Y + "\n" +
                "Center " + (int)frame[fNum].Origin.X + " " + (int)frame[fNum].Origin.Y + "\n");

            pointOutput = "";
            for (int i = 0; i < frame[fNum].point.Count; i++)
            {
                pointOutput += (frame[fNum].point[i].Name + " " + (int)frame[fNum].point[i].Position.X + " " + (int)frame[fNum].point[i].Position.Y + "\n");
            }

            textoutput = (modeOutput + coordOutput + pointOutput);
        }

        public void SaveToFRM()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && Keyboard.GetState().IsKeyDown(Keys.S) && keyStatePrevious.IsKeyUp(Keys.S))
            {
                frame[fNum].info.Clear();
                frame[fNum].info.Add("Center: " + (int)frame[fNum].Origin.X + " " + (int)frame[fNum].Origin.Y);
                frame[fNum].info.Add("Points:");
                for (int i = 0; i < frame[fNum].point.Count; i++)
                {
                    frame[fNum].info.Add(frame[fNum].point[i].Name + " " + (int)frame[fNum].point[i].Position.X + " " + (int)frame[fNum].point[i].Position.Y);
                }
                frame[fNum].info.Add("[end]");
                frame[fNum].info.Add("Hitbox:");
                for (int i = 0; i < frame[fNum].hitbox.Count; i++)
                {
                    frame[fNum].info.Add(frame[fNum].hitbox[i].Type.ToString() + " " +
                        frame[fNum].hitbox[i].X1.ToString() + " " + frame[fNum].hitbox[i].Y1.ToString() + " " +
                        frame[fNum].hitbox[i].X2.ToString() + " " + frame[fNum].hitbox[i].Y2.ToString());
                }
                frame[fNum].info.Add("[end]");

                fileLoader.SaveFRM(path, frame[fNum].Name, frame[fNum].info);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && Keyboard.GetState().IsKeyDown(Keys.LeftShift) && Keyboard.GetState().IsKeyDown(Keys.S) && keyStatePrevious.IsKeyUp(Keys.S))
            {
                for (int o = 0; o < frame.Count; o++)
                {
                    frame[o].info.Clear();
                    frame[o].info.Add("Center: " + (int)frame[o].Origin.X + " " + (int)frame[o].Origin.Y);
                    frame[o].info.Add("Points:");
                    for (int i = 0; i < frame[o].point.Count; i++)
                    {
                        frame[o].info.Add(frame[o].point[i].Name + " " + (int)frame[o].point[i].Position.X + " " + (int)frame[o].point[i].Position.Y);
                    }
                    frame[o].info.Add("[end]");
                    frame[o].info.Add("Hitbox:");
                    for (int i = 0; i < frame[o].hitbox.Count; i++)
                    {
                        frame[o].info.Add(frame[o].hitbox[i].Type.ToString() + " " +
                            frame[o].hitbox[i].X1.ToString() + " " + frame[o].hitbox[i].Y1.ToString() + " " +
                            frame[o].hitbox[i].X2.ToString() + " " + frame[o].hitbox[i].Y2.ToString());
                    }
                    frame[o].info.Add("[end]");

                    fileLoader.SaveFRM(path, frame[o].Name, frame[o].info);
                }
            }
        }
        public void InitiateHelp()
        {
            helpOutput = "Press F1 to exit the help menu.\n" +
                "(Space) Toggles between Key and Mouse select modes.\n" +
                "([) and (]) keys move to the previous and next frames.\n" +
                "(P) Toggle Play mode which plays and stops the animation.\n" +
                "(Ctrl + S) Saves the current frame.\n" +
                "(Ctrl + LShift + S) Saves all the frames.\n" +
                "(1)(2)(3) Selects the type of Hitbox to create.\n" +
                "(Middle Mouse Button) Pans around the frame.\n" +
                "(Middle Mouse Scroll) Zooms in and out on the frame.\n" +
                "(Right Mouse Button) Moves the current selected Hitbox.\n" +
                "(<) and (>) While in Key Select Mode selects the nex or previous hitbox.\n" +
                "(Left Mouse Button) In Mouse Mode creates a new Hitbox.\n" +
                "(Left Mouse Button) In Key Mode replaces the selected Hitbox with a new Hitbox.\n" +
                "(ESC) Quits the program.\n" +

                "(LeftControl + LeftShift + O) Input for Origin point on all frames. \n" +
                "(Alt + O) Input to move Origin point for current frame. \n" +
                "(O) Mouse move the Origin point on current frame. \n" +

                "\n" + "Note that the mouse cursor needs to be over the window for the program to recieve inputs";
        }
    }
}