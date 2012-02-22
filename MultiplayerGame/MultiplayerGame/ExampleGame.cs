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
using MultiplayerGame.Entities;
using MultiplayerGame.Managers;
using MultiplayerGame.Networking.Messages;
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

        private readonly ResolutionManager resolutionManager;

        private AsteroidManager asteroidManager;

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

        private bool IsHost { get { return this.networkManager is ServerNetworkManager; } }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.networkManager.Connect();

            var randomNumberGenerator = new MersenneTwister();

            this.asteroidManager = new AsteroidManager(this.resolutionManager, randomNumberGenerator, this.IsHost);
            if (this.IsHost)
            {
                this.asteroidManager.AsteroidStateChanged += (sender, e) => this.networkManager.SendMessage(new UpdateAsteroidStateMessage(e.Asteroid));
            }

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

            // TODO: use this.Content to load your game content here
            resolutionManager.FullViewport();
            resolutionManager.ResetViewport();

            this.asteroidManager.LoadContent(this.Content);
        }

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

            this.asteroidManager.Update(gameTime);

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
                    case NetIncomingMessageType.Data:
                        var gameMessageType = (GameMessageTypes)im.ReadByte();
                        switch(gameMessageType)
                        {
                            case GameMessageTypes.UpdateAsteroidState:
                                this.HandleUpdateAsteroidStateMessage(im);
                                break;
                        }
                        break;
                }

                this.networkManager.Recycle(im);
            }
        }

        private void HandleUpdateAsteroidStateMessage(NetIncomingMessage im)
        {
            var message = new UpdateAsteroidStateMessage(im);

            var timeDelay = (float) (NetTime.Now - im.SenderConnection.GetLocalTime(message.MessageTime));

            Asteroid asteroid = this.asteroidManager.GetAsteroid(message.Id) ??
                                this.asteroidManager.AddAsteroid(
                                    message.Id, message.Position, message.Velocity, message.Rotation);

            asteroid.EnableSmoothing = true;

            if (asteroid.LastUpdateTime < message.MessageTime)
            {
                asteroid.PrevDisplayState = (EntityState)asteroid.DisplayState.Clone();

                asteroid.SimulationState.Position = message.Position += (message.Velocity * timeDelay);

                asteroid.SimulationState.Velocity = message.Velocity;
                asteroid.SimulationState.Rotation = message.Rotation;

                asteroid.LastUpdateTime = message.MessageTime;
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

            this.asteroidManager.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void WindowClientSizeChanged(object sender, System.EventArgs e)
        {
            resolutionManager.SetResolution(Window.ClientBounds.Width, Window.ClientBounds.Height, false);
        }
    }
}
