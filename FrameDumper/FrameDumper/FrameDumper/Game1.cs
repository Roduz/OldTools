using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FrameDumper
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Effect pixelEffect;
        RenderTarget2D renderImage;

        public PaletteManager palette;
        public FrameManager frames;

        Texture2D image;

        //List<string> dirList;
        //List<string> dumpList;
        string dirList;
        string dumpList;

        string pathExe = Environment.CurrentDirectory + "\\";
        string pathData;
        string pathPal;
        string pmapName;
        string palName;
        int palSize;
        bool showScreen;

        int dcount = 0;
        int fcount = 0;

        static float scale = 0.5f;
        Matrix scaleMatrix = Matrix.CreateScale(scale);
        Vector2 palOffset = Vector2.Zero;

        //public Game1(List<string> dirList, List<string> dumpList, string path, string pathData, string pathPal, string palName, int palSize)
        public Game1(string dirList, string dumpList, string pathData, string pmapName, string pathPal, string palName, int palSize, bool showScreen)
        {
            this.dirList = dirList;
            this.dumpList = dumpList;
            this.pathData = pathData;
            this.pmapName = pmapName;
            this.pathPal = pathPal;
            this.palName = palName;
            this.palSize = palSize;
            this.showScreen = showScreen;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = false;

            palette = new PaletteManager(ref graphics, pathPal, pmapName, palName, palSize);
            //frames = new FrameManager(ref graphics, dirList[dcount], palette.pmapColor);
            frames = new FrameManager(ref graphics, dirList, palette.pmapColor, palette.div);
            //frames.AssignFrames(fcount);
            //image = frames.texture;
            //renderImage = new RenderTarget2D(GraphicsDevice, image.Width, image.Height, false, SurfaceFormat.Color, DepthFormat.None);
            //
            //graphics.PreferredBackBufferWidth = (int)(image.Width * scale);
            //graphics.PreferredBackBufferHeight = (int)(image.Height * scale);

            graphics.PreferredBackBufferWidth = 400;
            graphics.PreferredBackBufferHeight = 50;

            //Rectangle myClientBounds = this.Window.ClientBounds;
            //myClientBounds.X = 10;
            //myClientBounds.Y = 10;
            //this.Window.ClientBounds = myClientBounds;

            graphics.ApplyChanges();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pixelEffect = Content.Load<Effect>("Pixel");
        }
        /*
        private void LoadUpdate()
        {
            dcount++;
            fcount = 0;
            frames.ClearData();
            frames = null;
            frames = new FrameManager(ref graphics, dirList[dcount], palette.pmapColor);
            //frames.ReloadFrame(dirList[dcount]);
            //image = frames.AssignFrames(fcount);
            frames.AssignFrames(fcount);
            image = frames.texture;

            graphics.PreferredBackBufferWidth = (int)(image.Width * scale);
            graphics.PreferredBackBufferHeight = (int)(image.Height * scale);
            graphics.ApplyChanges();

            renderImage = null;
            GC.Collect();
            renderImage = new RenderTarget2D(GraphicsDevice, image.Width, image.Height, false, SurfaceFormat.Color, DepthFormat.None);
        }
        */
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || (Keyboard.GetState().IsKeyDown(Keys.Escape)))
                this.Exit();
            /*
            if (dcount >= dirList.Count)
                this.Exit();
            */
            if (fcount >= frames.frameAmount)
            {
                //LoadUpdate();
                this.Exit();
            }
            else
                Window.Title = "Frame <" + frames.fileName[fcount] + ">    Palette <" + palette.palName + ">";

            base.Update(gameTime);
        }
        public void SaveUpdate(Texture2D t, string dir, string file)
        {
            if (!Directory.Exists(pathExe + dir))
                Directory.CreateDirectory(pathExe + dir);

            using (Stream stream = File.OpenWrite(pathExe + dir + file))
            {
                t.SaveAsPng(stream, t.Width, t.Height);
                Console.WriteLine("Saved " + file);
            }
            fcount++;
        }
        void PreDraw()
        {
            //image = frames.AssignFrames(fcount);
            frames.AssignFrames(fcount);
            image = frames.texture;

            renderImage = new RenderTarget2D(GraphicsDevice, image.Width, image.Height, false, SurfaceFormat.Color, DepthFormat.None);

            if (showScreen)
            {
                graphics.PreferredBackBufferWidth = (int)(image.Width * scale);
                graphics.PreferredBackBufferHeight = (int)(image.Height * scale);
                graphics.ApplyChanges();
            }

            pixelEffect.Parameters["imageTexture"].SetValue(image);
            pixelEffect.Parameters["paletteTile"].SetValue(palette.paletteTable);
            pixelEffect.Parameters["paletteTileLine"].SetValue(palette.paletteTableLine);
        }
        protected override void Draw(GameTime gameTime)
        {
            if (fcount < frames.frameAmount)
            {
                PreDraw();
            }

            GraphicsDevice.SetRenderTarget(renderImage);
            GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null, pixelEffect);
            spriteBatch.Draw(image, Vector2.Zero, Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Gray);
            if (showScreen)
            {
                spriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null, null, scaleMatrix);
                spriteBatch.Draw((Texture2D)renderImage, Vector2.Zero, Color.White);
                spriteBatch.End();
            }

            if (fcount < frames.frameAmount)
            {
                SaveUpdate((Texture2D)renderImage, dumpList, frames.fileName[fcount] + "_" + palette.palName + ".png");
                //SaveUpdate((Texture2D)renderImage, dumpList[dcount], frames.fileName[fcount] + "_" + palette.palName + ".png");
            }

            base.Draw(gameTime);
        }
    }
}
