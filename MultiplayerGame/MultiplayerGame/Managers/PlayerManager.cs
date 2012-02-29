// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerManager.cs" company="">
//   
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiplayerGame.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using MultiplayerGame.Entities;
    using MultiplayerGame.EventArgs;
    using MultiplayerGame.RandomNumbers;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PlayerManager
    {
        #region Constants and Fields

        /// <summary>
        /// The gun offset.
        /// </summary>
        private readonly Vector2 gunOffset = new Vector2(25, 10);

        /// <summary>
        /// The initial player frame.
        /// </summary>
        private readonly Rectangle initialPlayerFrame = new Rectangle(0, 150, 50, 50);

        /// <summary>
        /// The input manager.
        /// </summary>
        private readonly InputManager inputManager;

        /// <summary>
        /// The is host.
        /// </summary>
        private readonly bool isHost;

        /// <summary>
        /// The players.
        /// </summary>
        private readonly Dictionary<long, Player> players = new Dictionary<long, Player>();

        /// <summary>
        /// The resolution manager.
        /// </summary>
        private readonly ResolutionManager resolutionManager;

        /// <summary>
        /// The shot manager.
        /// </summary>
        private readonly ShotManager shotManager;

        /// <summary>
        /// The player id counter.
        /// </summary>
        private static long playerIdCounter;

        /// <summary>
        /// The hearbeat timer.
        /// </summary>
        private GameTimer hearbeatTimer;

        /// <summary>
        /// The local player.
        /// </summary>
        private Player localPlayer;

        /// <summary>
        /// The no of player frames.
        /// </summary>
        private int noOfPlayerFrames = 3;

        /// <summary>
        /// The player collision radius.
        /// </summary>
        private int playerCollisionRadius = 15;

        /// <summary>
        /// The player speed.
        /// </summary>
        private float playerSpeed = 160f;

        /// <summary>
        /// The random number generator.
        /// </summary>
        private IRandomNumberGenerator randomNumberGenerator = NullRandomNumberGenerator.Instance;

        /// <summary>
        /// The shot timer.
        /// </summary>
        private GameTimer shotTimer;

        /// <summary>
        /// The sprite sheet.
        /// </summary>
        private Texture2D spriteSheet;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerManager"/> class.
        /// </summary>
        /// <param name="resolutionManager">
        /// The resolution manager.
        /// </param>
        /// <param name="randomNumberGenerator">
        /// The random number generator.
        /// </param>
        /// <param name="inputManager">
        /// The input manager.
        /// </param>
        /// <param name="shotManager">
        /// The shot manager.
        /// </param>
        /// <param name="isHost">
        /// The is host.
        /// </param>
        public PlayerManager(
            ResolutionManager resolutionManager, 
            IRandomNumberGenerator randomNumberGenerator, 
            InputManager inputManager, 
            ShotManager shotManager, 
            bool isHost)
        {
            this.resolutionManager = resolutionManager;
            this.randomNumberGenerator = randomNumberGenerator;
            this.inputManager = inputManager;
            this.shotManager = shotManager;
            this.isHost = isHost;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The player state changed.
        /// </summary>
        public event EventHandler<PlayerStateChangedArgs> PlayerStateChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets PlayerAreaLimit.
        /// </summary>
        public Rectangle PlayerAreaLimit
        {
            get
            {
                return new Rectangle(
                    0, 
                    this.resolutionManager.DisplayViewport.Height / 2, 
                    this.resolutionManager.DisplayViewport.Width, 
                    this.resolutionManager.DisplayViewport.Height / 2);
            }
        }

        /// <summary>
        /// Gets Players.
        /// </summary>
        public IEnumerable<Player> Players
        {
            get
            {
                return this.players.Values;
            }
        }

        /// <summary>
        /// Gets or sets RandomNumberGenerator.
        /// </summary>
        public IRandomNumberGenerator RandomNumberGenerator
        {
            get
            {
                return this.randomNumberGenerator;
            }

            set
            {
                this.randomNumberGenerator = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add player.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="velocity">
        /// The velocity.
        /// </param>
        /// <param name="rotation">
        /// The rotation.
        /// </param>
        /// <param name="isLocal">
        /// The is local.
        /// </param>
        /// <returns>
        /// </returns>
        public Player AddPlayer(long id, Vector2 position, Vector2 velocity, float rotation, bool isLocal)
        {
            if (this.players.ContainsKey(id))
            {
                return this.players[id];
            }

            var player = new Player(
                id, 
                this.spriteSheet, 
                this.initialPlayerFrame, 
                this.noOfPlayerFrames, 
                this.playerCollisionRadius, 
                new EntityState { Position = position, Rotation = rotation, Velocity = velocity });

            this.players.Add(player.Id, player);

            if (isLocal)
            {
                this.localPlayer = player;
            }

            return player;
        }

        /// <summary>
        /// The add player.
        /// </summary>
        /// <param name="isLocal">
        /// The is local.
        /// </param>
        /// <returns>
        /// </returns>
        public Player AddPlayer(bool isLocal)
        {
            EntityState physicsState = this.SelectRandomEntityState();

            Player player = this.AddPlayer(
                Interlocked.Increment(ref playerIdCounter), 
                physicsState.Position, 
                physicsState.Velocity, 
                physicsState.Rotation, 
                isLocal);

            return player;
        }

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="spriteBatch">
        /// The sprite batch.
        /// </param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Player player in this.Players)
            {
                if (!player.IsDestroyed)
                {
                    player.Draw(spriteBatch);
                }
            }
        }

        /// <summary>
        /// The get player.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// </returns>
        public Player GetPlayer(long id)
        {
            if (this.players.ContainsKey(id))
            {
                return this.players[id];
            }

            return null;
        }

        /// <summary>
        /// The load content.
        /// </summary>
        /// <param name="contentManager">
        /// The content manager.
        /// </param>
        public void LoadContent(ContentManager contentManager)
        {
            this.spriteSheet = contentManager.Load<Texture2D>(@"Textures\SpriteSheet");
            this.hearbeatTimer = new GameTimer();
            this.shotTimer = new GameTimer();
        }

        /// <summary>
        /// The payer is local.
        /// </summary>
        /// <param name="player">
        /// The player.
        /// </param>
        /// <returns>
        /// The payer is local.
        /// </returns>
        public bool PayerIsLocal(Player player)
        {
            return this.localPlayer != null && this.localPlayer.Id == player.Id;
        }

        /// <summary>
        /// The remove player.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public void RemovePlayer(long id)
        {
            if (this.players.ContainsKey(id))
            {
                this.players.Remove(id);
            }
        }

        /// <summary>
        /// The select random entity state.
        /// </summary>
        /// <returns>
        /// </returns>
        public EntityState SelectRandomEntityState()
        {
            var physicsState = new EntityState
                {
                    Position =
                        new Vector2(
                        this.randomNumberGenerator.Next(0, this.PlayerAreaLimit.Width), 
                        this.randomNumberGenerator.Next(this.PlayerAreaLimit.Top, this.PlayerAreaLimit.Bottom))
                };

            return physicsState;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        public void Update(GameTime gameTime)
        {
            if ((this.localPlayer != null) && (!this.localPlayer.IsDestroyed))
            {
                bool velocityChanged = this.HandlePlayerMovement();

                if (velocityChanged)
                {
                    this.OnPlayerStateChanged(this.localPlayer);
                }

                if (this.inputManager.IsKeyDown(Keys.Space))
                {
                    this.FireShot();
                }
            }

            foreach (Player player in this.Players)
            {
                player.Update(gameTime);

                if (!player.IsDestroyed)
                {
                    this.ImposeMovementLimits(player);
                }
            }

            if (this.isHost && this.hearbeatTimer.Stopwatch(1000))
            {
                foreach (Player player in this.Players)
                {
                    this.OnPlayerStateChanged(player);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on player state changed.
        /// </summary>
        /// <param name="player">
        /// The player.
        /// </param>
        protected void OnPlayerStateChanged(Player player)
        {
            EventHandler<PlayerStateChangedArgs> playerStateChanged = this.PlayerStateChanged;
            if (playerStateChanged != null)
            {
                playerStateChanged(this, new PlayerStateChangedArgs(player));
            }
        }

        /// <summary>
        /// The fire shot.
        /// </summary>
        private void FireShot()
        {
            if (this.shotTimer.Stopwatch(200))
            {
                this.shotManager.FireShot(
                    this.localPlayer.SimulationState.Position + this.gunOffset, 
                    new Vector2(0, -1), 
                    this.localPlayer.Id, 
                    true);
            }
        }

        /// <summary>
        /// The handle player movement.
        /// </summary>
        /// <returns>
        /// The handle player movement.
        /// </returns>
        private bool HandlePlayerMovement()
        {
            bool velocityChanged = false;

            this.localPlayer.SimulationState.Velocity = Vector2.Zero;

            if (this.inputManager.IsKeyDown(Keys.Up))
            {
                this.localPlayer.SimulationState.Velocity += new Vector2(0, -1);
                if (this.inputManager.IsKeyReleased(Keys.Up))
                {
                    velocityChanged = true;
                }
            }

            if (this.inputManager.IsKeyPressed(Keys.Up))
            {
                this.localPlayer.SimulationState.Velocity = new Vector2(this.localPlayer.SimulationState.Velocity.X, 0);
                velocityChanged = true;
            }

            if (this.inputManager.IsKeyDown(Keys.Down))
            {
                this.localPlayer.SimulationState.Velocity += new Vector2(0, 1);
                if (this.inputManager.IsKeyReleased(Keys.Down))
                {
                    velocityChanged = true;
                }
            }

            if (this.inputManager.IsKeyPressed(Keys.Down))
            {
                this.localPlayer.SimulationState.Velocity = new Vector2(this.localPlayer.SimulationState.Velocity.X, 0);
                velocityChanged = true;
            }

            if (this.inputManager.IsKeyDown(Keys.Left))
            {
                this.localPlayer.SimulationState.Velocity += new Vector2(-1, 0);
                if (this.inputManager.IsKeyReleased(Keys.Left))
                {
                    velocityChanged = true;
                }
            }

            if (this.inputManager.IsKeyPressed(Keys.Left))
            {
                this.localPlayer.SimulationState.Velocity = new Vector2(0, this.localPlayer.SimulationState.Velocity.Y);
                velocityChanged = true;
            }

            if (this.inputManager.IsKeyDown(Keys.Right))
            {
                this.localPlayer.SimulationState.Velocity += new Vector2(1, 0);
                if (this.inputManager.IsKeyReleased(Keys.Right))
                {
                    velocityChanged = true;
                }
            }

            if (this.inputManager.IsKeyPressed(Keys.Right))
            {
                this.localPlayer.SimulationState.Velocity = new Vector2(0, this.localPlayer.SimulationState.Velocity.Y);
                velocityChanged = true;
            }

            this.localPlayer.SimulationState.Velocity.Normalize();
            this.localPlayer.SimulationState.Velocity *= this.playerSpeed;

            return velocityChanged;
        }

        /// <summary>
        /// The impose movement limits.
        /// </summary>
        /// <param name="player">
        /// The player.
        /// </param>
        private void ImposeMovementLimits(Player player)
        {
            Vector2 position = player.SimulationState.Position;

            if (position.X < this.PlayerAreaLimit.X)
            {
                position.X = this.PlayerAreaLimit.X;
            }

            if (position.X > (this.PlayerAreaLimit.Right - player.Bounds.Width))
            {
                position.X = this.PlayerAreaLimit.Right - player.Bounds.Width;
            }

            if (position.Y < this.PlayerAreaLimit.Y)
            {
                position.Y = this.PlayerAreaLimit.Y;
            }

            if (position.Y > (this.PlayerAreaLimit.Bottom - player.Bounds.Height))
            {
                position.Y = this.PlayerAreaLimit.Bottom - player.Bounds.Height;
            }

            player.SimulationState.Position = position;
        }

        #endregion
    }
}