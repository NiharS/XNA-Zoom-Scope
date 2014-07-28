using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CameraTester
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		//Edit this field to change the zoom value.
		private const int ZOOM = 3;

		//Necessary variables
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Texture2D background;
		Texture2D crosshair;
		Matrix zoomValue;
		RenderTarget2D scene;
		RenderTarget2D zoomedView;
		Color[] crossHairData;
		Color[] scopeData;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 720;
			this.IsMouseVisible = true;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);


			//The picture of the background, and the crosshair which will have the zoomed values in it.
			//NOTE: As a workaround, the interior pixels of the crosshair should have low but non-zero values.
			background = Content.Load<Texture2D>("backgroundGradient");
			crosshair = Content.Load<Texture2D>("EnhancedCrosshair");

			//Matrix for the zooming.
			zoomValue = Matrix.CreateScale(new Vector3(ZOOM, ZOOM, 0));

			//The two render targets, the first being the background image and the second being the crosshair with the zoomed image.
			scene = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
			zoomedView = new RenderTarget2D(graphics.GraphicsDevice, crosshair.Width, crosshair.Height);

			//These are used later to remove excess pixel coloring from the corners of the crosshair.
			crossHairData = new Color[crosshair.Width * crosshair.Height];
			scopeData = new Color[crossHairData.Length];
			
			// TODO: use this.Content to load your game content here
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			base.Update(gameTime);
		}

		/// <summary>
		/// Draws the background image on the scene renderer.
		/// </summary>
		private void DrawRenderTarget()
		{
			GraphicsDevice.SetRenderTarget(scene);
			spriteBatch.Begin();
			Vector2 pos = Vector2.Zero;
			spriteBatch.Draw(background, pos, Color.White);
			spriteBatch.End();

			// Reset the device to the back buffer
			GraphicsDevice.SetRenderTarget(null);
		}
		
		/// <summary>
		/// Draws the crosshairs and the zoomed version of the scene onto the zoomedView renderer.
		/// </summary>
		private void DrawZoomedTarget()
		{
			GraphicsDevice.SetRenderTarget(zoomedView);
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, zoomValue);

			//Rectangle enclosing the crosshairs
			Rectangle crossRect = new Rectangle(
					Mouse.GetState().X - crosshair.Width / 2,
					Mouse.GetState().Y - crosshair.Height / 2,
					crosshair.Width,
					crosshair.Height);

			//Zoomed in clipping of the background
			spriteBatch.Draw((Texture2D)scene,
				Vector2.Zero,
				new Rectangle(crossRect.X + (crossRect.Width * (ZOOM-1) / (2*ZOOM)), 
					crossRect.Y + (crossRect.Height * (ZOOM-1) / (2 * ZOOM)),
					crossRect.Width,
					crossRect.Height),
				Color.White);
			spriteBatch.End();

			//Draws the crosshairs themselves, unzoomed
			spriteBatch.Begin();
			spriteBatch.Draw(crosshair, Vector2.Zero, Color.White);
			spriteBatch.End();
			GraphicsDevice.SetRenderTarget(null);


			/*
			 * This section is to get rid of pixels in the corners of the crosshair.
			 * For this to work, the corner pixels must have an alpha of 0
			 * while the pixels in the middle must have a small non-zero alpha.
			 * I recommend setting their alphas to 1 (on an alpha range of [0,255]).
			*/
			crosshair.GetData<Color>(crossHairData);
			zoomedView.GetData<Color>(scopeData);
			for (int i = 0; i < crossHairData.Length; i++)
			{
				if (crossHairData[i].A == 0)
				{
					scopeData[i].A = 0;
					scopeData[i].R = 0;
					scopeData[i].G = 0;
					scopeData[i].B = 0;
				}
			}
			zoomedView.SetData<Color>(scopeData);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			DrawRenderTarget();
			DrawZoomedTarget();
			GraphicsDevice.Clear(Color.CornflowerBlue);
			

			//Starts by drawing the scenery
			spriteBatch.Begin();
			spriteBatch.Draw((Texture2D)scene,
				new Vector2(0, 0),
				new Rectangle(0, 0, 1280, 720),
				Color.White
				);
			spriteBatch.End();

			//Next, draws the crosshairs with the zoomed pixels in the middle.
			//Will not work if pixels in scope have alphas of exactly 0.
			spriteBatch.Begin();
			spriteBatch.Draw((Texture2D)zoomedView, 
				new Rectangle(Mouse.GetState().X - crosshair.Width / 2,
					Mouse.GetState().Y - crosshair.Height / 2,
					crosshair.Width,
					crosshair.Height),
				Color.White);
			
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
