using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FrameViewer
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        Effect pixelEffect;
        BasicEffect effect;
        SpriteBatch spriteBatch;

        RenderTarget2D renderImage;

        bool screenCapture = false;
        bool saveCapture = false;

        string pathExe = Environment.CurrentDirectory + "\\";
        string path;
        string pathPal;
        string pmapName;

        int palSize;
        int spriteSize;

        public Loader loader;
        public PaletteManager palette;

        List<Sprite> sprite = new List<Sprite>();

        bool showColor = true;
        bool showShade = true;
        bool showLine = true;
        bool linearSmooth = false;
        bool isActive = false;
        bool inView = true;
        bool drawPoint = false;

        Matrix world = Matrix.Identity;

        Camera2D camera;

        KeyboardState keyStatePrevious;
        MouseState mouseStatePrevious;

        InputManager input;

        Vector2 mouseClick = Vector2.Zero;
        Vector2 mouseDrag = Vector2.Zero;
        Vector3 bgColor = new Vector3(0.4f, 0.4f, 0.4f);
        Vector2 palOffset = Vector2.Zero;

        bool hudText = true;
        bool helpText = false;
        bool play = false;
        int fNum = 0;
        int pNum = 0;
        int palNum = 1;
        int palineNum = 1;
        int fCount = 0;

        //time since last FPS update in seconds
        float fpsTime = 0;

        //Font Varibles
        Vector3 textColor = Vector3.One;
        SpriteFont font;
        string textoutput = "";
        string helpoutput = "";

        public Game1(string path, string pathPal, string pmapName, int palSize, int spriteSize)
        {
            this.path = path;
            this.pathPal = pathPal;
            this.pmapName = pmapName;
            this.palSize = palSize;
            this.spriteSize = spriteSize;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = true;

            input = new InputManager();

            // Basic Effect for 3D rendering
            effect = new BasicEffect(graphics.GraphicsDevice);

            graphics.PreferredBackBufferWidth = (int)(GraphicsDevice.DisplayMode.Width * .6f);
            graphics.PreferredBackBufferHeight = (int)(GraphicsDevice.DisplayMode.Height * .9f);
            graphics.ApplyChanges();

            camera = new Camera2D();
            camera.Position = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 8);

            font = Content.Load<SpriteFont>("Font");
            HelpOutput();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            palette = new PaletteManager(ref graphics, pathPal, pmapName, palSize);
            loader = new Loader(ref graphics, path, ref palette, ref sprite, spriteSize);

            if (sprite.Count == 0)
                Console.WriteLine("No frame PNG or PCX files found!");

            if (palette.palAmount <= 1)
            {
                Console.WriteLine("No PAL or palette files found!");
                palNum = 0;
            }
            if (palette.palAmountLine <= 1)
            {
                Console.WriteLine("No DYE_PAL or palette dye files found!");
                palineNum = 0;
            }

            spriteBatch = new SpriteBatch(GraphicsDevice);
            pixelEffect = Content.Load<Effect>("Pixel");
            pixelEffect.CurrentTechnique = pixelEffect.Techniques[0];

            //renderImage = new RenderTarget2D(GraphicsDevice, (int)sprite[0].frame[0].Size.X, (int)sprite[0].frame[0].Size.Y, false, SurfaceFormat.Color, DepthFormat.None);
            loader = null;
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            if (sprite.Count == 0)
            {
                Console.WriteLine("Restart FrameViewer with files in the path.");
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || (Keyboard.GetState().IsKeyDown(Keys.Escape)))
                    this.Exit();
            }

            //Checks to see if it is the Active window.
            isActive = this.IsActive;

            pNum = fNum;

            //Checks to see if the mouse is over the window
            inView = (Mouse.GetState().X <= GraphicsDevice.Viewport.Width && Mouse.GetState().Y <= GraphicsDevice.Viewport.Height && Mouse.GetState().X >= 0 && Mouse.GetState().Y >= 0);

            if (isActive)
            {
                if (inView)
                {
                    // Camera manipulation and control
                    input.InputCamera(ref mouseStatePrevious, ref camera, ref mouseClick, ref mouseDrag);
                    // UPDATE THE CAMERA TO SHOW NEW VIEW AND RESIZE
                    camera.Origin = sprite[0].frame[fNum].Origin;
                    // Updates the Real time number feedback and info text
                    TextOutput();
                }

                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || (Keyboard.GetState().IsKeyDown(Keys.Escape)))
                    this.Exit();

                // Input to toggle ON/OFF of the layers
                if (keyStatePrevious.IsKeyUp(Keys.D1) && keyStatePrevious.IsKeyUp(Keys.D2) && keyStatePrevious.IsKeyUp(Keys.D3))
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.D1))
                        showLine = !showLine;

                    if (Keyboard.GetState().IsKeyDown(Keys.D2))
                        showColor = !showColor;

                    if (Keyboard.GetState().IsKeyDown(Keys.D3))
                        showShade = !showShade;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.OemTilde) && keyStatePrevious.IsKeyUp(Keys.OemTilde))
                    linearSmooth = !linearSmooth;

                if (Keyboard.GetState().IsKeyDown(Keys.Right) && keyStatePrevious.IsKeyUp(Keys.Right))
                {
                    if (fNum == sprite[0].frame.Count - 1)
                        fNum = 0;
                    else
                        fNum += 1;

                    play = false;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Left) && keyStatePrevious.IsKeyUp(Keys.Left))
                {
                    if (fNum == 0)
                        fNum = sprite[0].frame.Count - 1;
                    else
                        fNum -= 1;

                    play = false;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Space) && keyStatePrevious.IsKeyUp(Keys.Space))
                {
                    play = !play;
                }

                // Background Color Editting
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    if (bgColor.X < 1)
                        bgColor.X += 0.01f;
                    else
                        bgColor.X = 0;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.G))
                {
                    if (bgColor.Y < 1)
                        bgColor.Y += 0.01f;
                    else
                        bgColor.Y = 0;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.B))
                {
                    if (bgColor.Z < 1)
                        bgColor.Z += 0.01f;
                    else
                        bgColor.Z = 0;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.F))
                {
                    bgColor = Vector3.Zero;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.H))
                {
                    bgColor = Vector3.One;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.T))
                {
                    bgColor = new Vector3(0.4f, 0.4f, 0.4f);
                }

                if (bgColor == new Vector3(0.4f, 0.4f, 0.4f))
                    textColor = Vector3.One;
                else
                    textColor = Vector3.One - bgColor;

                // Playback Toggle
                if (play)
                    Playback();

                if (Keyboard.GetState().IsKeyDown(Keys.P) && keyStatePrevious.IsKeyUp(Keys.P))
                    drawPoint = !drawPoint;

                //Palette Controls
                if (Keyboard.GetState().IsKeyDown(Keys.OemPlus) && keyStatePrevious.IsKeyUp(Keys.OemPlus))
                {
                    if (palNum < palette.paletteTable.Count - 1)
                        palNum += 1;
                    else
                        palNum = palette.paletteTable.Count - 1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.OemMinus) && keyStatePrevious.IsKeyUp(Keys.OemMinus))
                {
                    if (palNum > 0)
                        palNum -= 1;
                    else
                        palNum = 0;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.OemCloseBrackets) && keyStatePrevious.IsKeyUp(Keys.OemCloseBrackets))
                {
                    if (palineNum < palette.paletteTableLine.Count - 1)
                        palineNum += 1;
                    else
                        palineNum = palette.paletteTableLine.Count - 1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.OemOpenBrackets) && keyStatePrevious.IsKeyUp(Keys.OemOpenBrackets))
                {
                    if (palineNum > 0)
                        palineNum -= 1;
                    else
                        palineNum = 0;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.F5) && keyStatePrevious.IsKeyUp(Keys.F5))
                {
                    palette.ReloadPalette(ref graphics);

                    if (palette.paletteTable.Count < palNum)
                        palNum = 0;
                    if (palette.paletteTableLine.Count < palineNum)
                        palineNum = 0;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.F11) && keyStatePrevious.IsKeyUp(Keys.F11))
                {
                    SaveToPNG(palette.paletteTable[palNum], pathExe, palette.palName[palNum] + "_pal.png");
                    SaveToPNG(palette.paletteTableLine[palineNum], pathExe, palette.palNameLine[palineNum] + "_pal.png");
                }
                if (Keyboard.GetState().IsKeyDown(Keys.F12) && keyStatePrevious.IsKeyUp(Keys.F12))
                {
                    renderImage = new RenderTarget2D(GraphicsDevice, (int)sprite[0].frame[fNum].Size.X, (int)sprite[0].frame[fNum].Size.Y, false, SurfaceFormat.Color, DepthFormat.None);
                    screenCapture = true;
                }
                if (saveCapture)
                {
                    SaveToPNG((Texture2D)renderImage, pathExe, sprite[0].frame[fNum].Name + "_" + palette.palName[palNum] + ".png");
                    saveCapture = false;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.F2) && keyStatePrevious.IsKeyUp(Keys.F2))
                {
                    hudText = !hudText;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.F1) && keyStatePrevious.IsKeyUp(Keys.F1))
                {
                    helpText = !helpText;
                }
            }

            Window.Title = "Frame Viewer <" + sprite[0].frame[fNum].Name + ">";

            if (fpsTime > 1)
                fpsTime -= 1;

            //Pixel Texture Check
            pixelEffect.Parameters["paletteTile"].SetValue(palette.paletteTable[palNum]);
            pixelEffect.Parameters["paletteTileLine"].SetValue(palette.paletteTableLine[palineNum]);
            pixelEffect.Parameters["showColor"].SetValue(showColor);
            pixelEffect.Parameters["showShade"].SetValue(showShade);
            pixelEffect.Parameters["showLine"].SetValue(showLine);

            // Play Back
            if (play) Playback();

            // Previous input state for future comparison
            mouseStatePrevious = Mouse.GetState();
            keyStatePrevious = Keyboard.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (screenCapture)
            {
                //Draw the sprite.
                graphics.GraphicsDevice.SetRenderTarget(renderImage);
                graphics.GraphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(0, BlendState.NonPremultiplied, null, null, null, pixelEffect);
                spriteBatch.Draw(sprite[0].frame[fNum].ImageTexture, Vector2.Zero, Color.White);
                spriteBatch.End();
                saveCapture = true;
                screenCapture = false;
            }
            graphics.GraphicsDevice.SetRenderTarget(null);

            if (helpText)
            {
                graphics.GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                spriteBatch.DrawString(font, helpoutput, Vector2.Zero, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.End();
            }
            else
            {
                graphics.GraphicsDevice.Clear(new Color(bgColor.X, bgColor.Y, bgColor.Z, 1));
                pixelEffect.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied, null, null, null, pixelEffect, camera.Transform(GraphicsDevice));

                //Rectangle rec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                float width = (GraphicsDevice.Viewport.Width * 0.5f) - sprite[0].frame[fNum].Origin.X;
                float height = (GraphicsDevice.Viewport.Height * 0.5f) - sprite[0].frame[fNum].Origin.Y;
                Vector2 spritePos = new Vector2(width, height);

                pixelEffect.Parameters["imageTexture"].SetValue(sprite[0].frame[fNum].ImageTexture);
                spriteBatch.Draw(sprite[0].frame[fNum].ImageTexture, spritePos, Color.White);

                for (int i = 1; i < sprite.Count; ++i)
                {
                    if (fNum < sprite[i].frame.Count)
                    {
                        float tw = (GraphicsDevice.Viewport.Width * 0.5f) - sprite[i].frame[fNum].Origin.X;
                        float th = (GraphicsDevice.Viewport.Height * 0.5f) - sprite[i].frame[fNum].Origin.Y;
                        Vector2 pos = new Vector2(tw, th);
                        //pos.X = (GraphicsDevice.Viewport.Width * 0.5f) - sprite[i].frame[fNum].Origin.X;
                        //pos.Y = (GraphicsDevice.Viewport.Height * 0.5f) - sprite[i].frame[fNum].Origin.Y;

                        pixelEffect.Parameters["imageTexture"].SetValue(sprite[i].frame[fNum].ImageTexture);
                        spriteBatch.Draw(sprite[i].frame[fNum].ImageTexture, spritePos, Color.White);
                    }
                }
                spriteBatch.End();

                if (hudText)
                {
                    //Draw the text
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                    spriteBatch.DrawString(font, textoutput, new Vector2(5, 0), new Color(textColor.X, textColor.Y, textColor.Z, 1), 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                    spriteBatch.End();
                }
            }
            base.Draw(gameTime);

            //Draw the origin point
            //if (drawPoint)
        }

        public void Playback()
        {
            linearSmooth = false;
            if (sprite[0].frame[fNum].Rate + ((int)sprite[0].frame[fNum].Rate / 2) == fCount)
            {
                fCount = 0;
                if (fNum == sprite[0].frame.Count - 1)
                    fNum = 0;
                else
                    fNum += 1;
            }
            else
                fCount += 1;
        }

        public void SaveToPNG(Texture2D t, string f, string s)
        {
            using (Stream stream = File.OpenWrite(f + s))
            {
                t.SaveAsPng(stream, t.Width, t.Height);
                Console.WriteLine("Saved frame " + s);
            }
        }

        public Texture2D OriginMarker(int size, Color col)
        {
            Color[] marker = new Color[size * size];

            int half = (size + 1) / 2;

            for (int i = 0; i < (size * size); ++i)
            {
                marker[i] = Color.Transparent;

                if (i < size)
                    marker[i] = col;

                if (i > size * (size - 1))
                    marker[i] = col;

                if ((i > size * half) && (i < size * (half +1)))
                    marker[i] = col;
            }
            Texture2D textureOut = new Texture2D(GraphicsDevice, size, size, false, SurfaceFormat.Color);
            textureOut.SetData(marker);

            return textureOut;
        }
        public void TextOutput()
        {
            // +"Origin <" + sprite[0].frame[fNum].Origin + ">";

            string frameName = "";
            string paletteName = "";
            string toggles = "";

            // Draw Text Info
            frameName = "Frame <" + sprite[0].frame[fNum].Name + ">\n";
            paletteName = "Palette <" + palette.palName[palNum] + ">\nDye <" + palette.palNameLine[palineNum] + ">\n";
            toggles = "Line <" + showLine + ">\nColor <" + showColor + ">\nShade <" + showShade + ">\nZoom <" + camera.Zoom + ">\n";
            textoutput = (frameName + paletteName + toggles);
        }
        public void HelpOutput()
        {
            string h1 = "\n\tKeys:\n\t~ = Toggle Line and Shade layer smoothing (Disabled for the moment)\n\t1 = Toggle the Line layer\n\t2 = Toggle the Color layer\n\t3 = Toggle the Shade layer\n\t+ = Next palette color\n\t- = Previous palette color\n\t] = Next Dye palette color, if the frame supports it.\n\t[ = Previous Dye palette color, if the frame supports it.\n\t";
            string h2 = "F1 = Toggles help text off/on\n\tF2 = Toggles HUD text off/on\n\tF5 = Reload all palette colors\n\tF11 = Saves palette to a png file in the frameviewer directory.\n\tF12 = Saves frame to a png file in the frameviewer directory.\n\tLeft Mouse Button = Pans the Camera\n\tMiddle Mouse Button = Pans the Camera\n\tMiddle Mouse Scroll = Zooms in or out\n\tRight Arrow = Next frame\n\tLeft Arrow = Previous Frame\n\t";
            string h3 = "Spacebar = Toggles fixed frame playback\n\tR = Adjusts the Red value of the background\n\tG = Adjusts the Green value of the background\n\tB = Adjusts the Blue value of the background\n\tF = Makes the background Black\n\tH = Makes the background White\n\tT = Resets the background to the default color\n\tP = Displays origin point (Not working at the moment)\n\tEsc = Exits FrameView (only if the draw window is up)\n\t";
            helpoutput = h1+h2+h3;
        }
    }
}
