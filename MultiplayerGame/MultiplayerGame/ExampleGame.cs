// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExampleGame.cs" company="">
//   
// </copyright>
// <summary>
//   This is the main type for your game
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiplayerGame
{
    using System;

    using Lidgren.Network;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using MultiplayerGame.Entities;
    using MultiplayerGame.Managers;
    using MultiplayerGame.Networking;
    using MultiplayerGame.Networking.Messages;
    using MultiplayerGame.RandomNumbers;

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ExampleGame : Game
    {
        #region Constants and Fields

        /// <summary>
        /// The graphics.
        /// </summary>
        private readonly GraphicsDeviceManager graphics;

        /// <summary>
        /// The network manager.
        /// </summary>
        private readonly INetworkManager networkManager;

        /// <summary>
        /// The resolution manager.
        /// </summary>
        private readonly ResolutionManager resolutionManager;

        /// <summary>
        /// The asteroid manager.
        /// </summary>
        private AsteroidManager asteroidManager;

        /// <summary>
        /// The enemy manager.
        /// </summary>
        private EnemyManager enemyManager;

        /// <summary>
        /// The input manager.
        /// </summary>
        private InputManager inputManager;

        /// <summary>
        /// The player manager.
        /// </summary>
        private PlayerManager playerManager;

        /// <summary>
        /// The shot manager.
        /// </summary>
        private ShotManager shotManager;

        /// <summary>
        /// The sound manager.
        /// </summary>
        private SoundManager soundManager;

        /// <summary>
        /// The sprite batch.
        /// </summary>
        private SpriteBatch spriteBatch;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleGame"/> class.
        /// </summary>
        /// <param name="networkManager">
        /// The network manager.
        /// </param>
        public ExampleGame(INetworkManager networkManager)
        {
            this.graphics = new GraphicsDeviceManager(this)
                {
                   PreferMultiSampling = true, PreferredBackBufferWidth = 800, PreferredBackBufferHeight = 600 
                };

            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += this.WindowClientSizeChanged;

            this.graphics.ApplyChanges();

            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;

            this.resolutionManager = new ResolutionManager();
            this.resolutionManager.Init(ref this.graphics);
            this.resolutionManager.SetVirtualResolution(800, 600);
            this.resolutionManager.SetResolution(800, 600, false);

            this.networkManager = networkManager;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether IsHost.
        /// </summary>
        private bool IsHost
        {
            get
            {
                return this.networkManager is ServerNetworkManager;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values.
        /// </param>
        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.Black);

            this.spriteBatch.Begin();

            this.asteroidManager.Draw(this.spriteBatch);
            this.playerManager.Draw(this.spriteBatch);
            this.shotManager.Draw(this.spriteBatch);
            this.enemyManager.Draw(this.spriteBatch);

            this.spriteBatch.End();

            base.Draw(gameTime);
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
            this.networkManager.Connect();

            var randomNumberGenerator = new MersenneTwister();
            this.inputManager = new InputManager(this, this.resolutionManager);

            this.soundManager = new SoundManager(randomNumberGenerator);
            this.shotManager = new ShotManager(this.resolutionManager, this.soundManager);
            this.shotManager.ShotFired += (sender, e) => this.networkManager.SendMessage(new ShotFiredMessage(e.Shot));

            this.asteroidManager = new AsteroidManager(this.resolutionManager, randomNumberGenerator, this.IsHost);
            if (this.IsHost)
            {
                this.asteroidManager.AsteroidStateChanged +=
                    (sender, e) => this.networkManager.SendMessage(new UpdateAsteroidStateMessage(e.Asteroid));
            }

            this.playerManager = new PlayerManager(
                this.resolutionManager, randomNumberGenerator, this.inputManager, this.shotManager, this.IsHost);
            this.playerManager.PlayerStateChanged +=
                (sender, e) => this.networkManager.SendMessage(new UpdatePlayerStateMessage(e.Player));

            this.enemyManager = new EnemyManager(
                randomNumberGenerator, this.shotManager, this.playerManager, this.IsHost);
            if (this.IsHost)
            {
                this.enemyManager.EnemySpawned +=
                    (sender, e) => this.networkManager.SendMessage(new EnemySpawnedMessage(e.Enemy));
            }

            this.Components.Add(this.inputManager);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

            // TODO: use this.Content to load your game content here
            this.resolutionManager.FullViewport();
            this.resolutionManager.ResetViewport();

            this.soundManager.LoadContent(this.Content);
            this.shotManager.LoadContent(this.Content);
            this.asteroidManager.LoadContent(this.Content);
            this.playerManager.LoadContent(this.Content);
            this.enemyManager.LoadContent(this.Content);

            if (this.IsHost)
            {
                this.playerManager.AddPlayer(true);
            }
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
        /// <param name="gameTime">
        /// Provides a snapshot of timing values.
        /// </param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (this.inputManager.IsKeyPressed(Keys.Escape))
            {
                this.Exit();
            }

            // TODO: Add your update logic here
            this.ProcessNetworkMessages();

            this.asteroidManager.Update(gameTime);
            this.playerManager.Update(gameTime);
            this.shotManager.Update(gameTime);
            this.enemyManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// The handle enemy spawned message.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        private void HandleEnemySpawnedMessage(NetIncomingMessage im)
        {
            var message = new EnemySpawnedMessage(im);

            var timeDelay = (float)(NetTime.Now - im.SenderConnection.GetLocalTime(message.MessageTime));

            Vector2 adjustedPosition = message.Position + (message.Velocity * timeDelay);

            this.enemyManager.SpawnEnemy(message.Id, message.Path, adjustedPosition, message.Velocity, message.Rotation);
        }

        /// <summary>
        /// The handle shot fired message.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        private void HandleShotFiredMessage(NetIncomingMessage im)
        {
            var message = new ShotFiredMessage(im);

            var timeDelay = (float)(NetTime.Now - im.SenderConnection.GetLocalTime(message.MessageTime));

            Vector2 adjustedPosition = message.Position + (message.Velocity * timeDelay);

            this.shotManager.FireShot(
                message.Id, adjustedPosition, message.Velocity, message.FiredById, message.FiredByPlayer);
        }

        /// <summary>
        /// The handle update asteroid state message.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        private void HandleUpdateAsteroidStateMessage(NetIncomingMessage im)
        {
            var message = new UpdateAsteroidStateMessage(im);

            var timeDelay = (float)(NetTime.Now - im.SenderConnection.GetLocalTime(message.MessageTime));

            Asteroid asteroid = this.asteroidManager.GetAsteroid(message.Id)
                                ??
                                this.asteroidManager.AddAsteroid(
                                    message.Id, message.Position, message.Velocity, message.Rotation);

            // asteroid.EnableSmoothing = true;
            if (asteroid.LastUpdateTime < message.MessageTime)
            {
                asteroid.SimulationState.Position = message.Position += message.Velocity * timeDelay;
                asteroid.SimulationState.Velocity = message.Velocity;
                asteroid.SimulationState.Rotation = message.Rotation;
                asteroid.LastUpdateTime = message.MessageTime;
            }
        }

        /// <summary>
        /// The handle update player state message.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        private void HandleUpdatePlayerStateMessage(NetIncomingMessage im)
        {
            var message = new UpdatePlayerStateMessage(im);

            var timeDelay = (float)(NetTime.Now - im.SenderConnection.GetLocalTime(message.MessageTime));

            Player player = this.playerManager.GetPlayer(message.Id)
                            ??
                            this.playerManager.AddPlayer(
                                message.Id, message.Position, message.Velocity, message.Rotation, false);

            player.EnableSmoothing = true;

            if (player.LastUpdateTime < message.MessageTime)
            {
                player.SimulationState.Position = message.Position += message.Velocity * timeDelay;
                player.SimulationState.Velocity = message.Velocity;
                player.SimulationState.Rotation = message.Rotation;

                player.LastUpdateTime = message.MessageTime;
            }
        }

        /// <summary>
        /// The process network messages.
        /// </summary>
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
                                if (!this.IsHost)
                                {
                                    var message = new UpdatePlayerStateMessage(im.SenderConnection.RemoteHailMessage);
                                    this.playerManager.AddPlayer(
                                        message.Id, message.Position, message.Velocity, message.Rotation, true);
                                    Console.WriteLine("Connected to {0}", im.SenderEndpoint);
                                }
                                else
                                {
                                    Console.WriteLine("{0} Connected", im.SenderEndpoint);
                                }

                                break;
                            case NetConnectionStatus.Disconnected:
                                Console.WriteLine(
                                    this.IsHost ? "{0} Disconnected" : "Disconnected from {0}", im.SenderEndpoint);
                                break;
                            case NetConnectionStatus.RespondedAwaitingApproval:
                                NetOutgoingMessage hailMessage = this.networkManager.CreateMessage();
                                new UpdatePlayerStateMessage(this.playerManager.AddPlayer(false)).Encode(hailMessage);
                                im.SenderConnection.Approve(hailMessage);
                                break;
                        }

                        break;
                    case NetIncomingMessageType.Data:
                        var gameMessageType = (GameMessageTypes)im.ReadByte();
                        switch (gameMessageType)
                        {
                            case GameMessageTypes.UpdateAsteroidState:
                                this.HandleUpdateAsteroidStateMessage(im);
                                break;
                            case GameMessageTypes.UpdatePlayerState:
                                this.HandleUpdatePlayerStateMessage(im);
                                break;
                            case GameMessageTypes.ShotFired:
                                this.HandleShotFiredMessage(im);
                                break;
                            case GameMessageTypes.EnemySpawned:
                                this.HandleEnemySpawnedMessage(im);
                                break;
                        }

                        break;
                }

                this.networkManager.Recycle(im);
            }
        }

        /// <summary>
        /// The window client size changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WindowClientSizeChanged(object sender, System.EventArgs e)
        {
            this.resolutionManager.SetResolution(this.Window.ClientBounds.Width, this.Window.ClientBounds.Height, false);
        }

        #endregion
    }
}