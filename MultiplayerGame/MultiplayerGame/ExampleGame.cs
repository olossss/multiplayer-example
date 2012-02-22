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
using MultiplayerGame.Managers;
using MultiplayerGame.RandomNumbers;

namespace MultiplayerGame
{
    using Lidgren.Network;

    using MultiplayerGame.Networking;

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ExampleGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private readonly INetworkManager networkManager;

        public ExampleGame(INetworkManager networkManager)
        {
            graphics = new GraphicsDeviceManager(this) { PreferMultiSampling = true, PreferredBackBufferWidth = 800, PreferredBackBufferHeight = 600 };

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += WindowClientSizeChanged;

            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            resolutionManager = new ResolutionManager();
            resolutionManager.Init(ref graphics);
            resolutionManager.SetVirtualResolution(800, 600);
            resolutionManager.SetResolution(800, 600, false);

            this.networkManager = networkManager;
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            this.font = Content.Load<SpriteFont>(@"Fonts\Verdana10");

            // TODO: use this.Content to load your game content here
        }

        private SpriteFont font;

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

            this.networkManager.Disconnect();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here
            this.ProcessNetworkMessages();

            base.Update(gameTime);
        }

        private void ProcessNetworkMessages()
        {
            NetIncomingMessage im;

            while ((im = this.networkManager.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Console.WriteLine(im.ReadString());
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus)im.ReadByte())
                        {
                            case NetConnectionStatus.Connected:
                                Console.WriteLine("{0} Connected", im.SenderEndpoint);
                                break;
                            case NetConnectionStatus.Disconnected:
                                Console.WriteLine("{0} Disconnected", im.SenderEndpoint);
                                break;
                            case NetConnectionStatus.RespondedAwaitingApproval:
                                im.SenderConnection.Approve();
                                break;
                        }
                        break;
                }

                this.networkManager.Recycle(im);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.DrawString(this.font, string.Format("Hello World I am the {0}", this.networkManager is ClientNetworkManager ? "Client" : "Server"), new Vector2(100, 100), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void WindowClientSizeChanged(object sender, System.EventArgs e)
        {
            resolutionManager.SetResolution(Window.ClientBounds.Width, Window.ClientBounds.Height, false);
        }
    }
}
