// -----------------------------------------------------------------------
// <copyright file="PlayerManager.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MultiplayerGame.Entities;
using MultiplayerGame.EventArgs;
using MultiplayerGame.RandomNumbers;

namespace MultiplayerGame.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PlayerManager
    {
        #region Constants and Fields

        private readonly Rectangle initialPlayerFrame = new Rectangle(0, 150, 50, 50);

        private readonly InputManager inputManager;

        private readonly Dictionary<long, Player> players = new Dictionary<long, Player>();

        private readonly ResolutionManager resolutionManager;

        private static long playerIdCounter;

        private Player localPlayer;

        private int noOfPlayerFrames = 3;

        private int playerCollisionRadius = 15;

        private float playerSpeed = 160f;

        private IRandomNumberGenerator randomNumberGenerator = NullRandomNumberGenerator.Instance;

        private Texture2D spriteSheet;

        private GameTimer hearbeatTimer;

        private bool isHost;

        #endregion

        #region Constructors and Destructors

        public PlayerManager(
            ResolutionManager resolutionManager,
            IRandomNumberGenerator randomNumberGenerator,
            InputManager inputManager,
            bool isHost
            )
        {
            this.resolutionManager = resolutionManager;
            this.randomNumberGenerator = randomNumberGenerator;
            this.inputManager = inputManager;
            this.isHost = isHost;
        }

        #endregion

        #region Events

        public event EventHandler<PlayerStateChangedArgs> PlayerStateChanged;

        #endregion

        #region Properties

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

        public IEnumerable<Player> Players
        {
            get
            {
                return this.players.Values;
            }
        }

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

        #region Public Methods

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

        public Player AddPlayer(bool isLocal)
        {
            EntityState physicsState = this.SelectRandomEntityState();

            var player = this.AddPlayer(
                Interlocked.Increment(ref playerIdCounter),
                physicsState.Position,
                physicsState.Velocity,
                physicsState.Rotation,
                isLocal);

            return player;
        }

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

        public Player GetPlayer(long id)
        {
            if (this.players.ContainsKey(id))
            {
                return this.players[id];
            }

            return null;
        }

        public void LoadContent(ContentManager contentManager)
        {
            this.spriteSheet = contentManager.Load<Texture2D>(@"Textures\SpriteSheet");
            this.hearbeatTimer = new GameTimer();
        }

        public void RemovePlayer(long id)
        {
            if (this.players.ContainsKey(id))
            {
                this.players.Remove(id);
            }
        }

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

        public void Update(GameTime gameTime)
        {
            if ((this.localPlayer != null) && (!this.localPlayer.IsDestroyed))
            {
                var velocityChanged = this.HandlePlayerMovement();

                if (velocityChanged)
                {
                    this.OnPlayerStateChanged(this.localPlayer);
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
                foreach (var player in this.Players)
                {
                    this.OnPlayerStateChanged(player);
                }
            }
        }

        #endregion

        #region Methods

        protected void OnPlayerStateChanged(Player player)
        {
            EventHandler<PlayerStateChangedArgs> playerStateChanged = this.PlayerStateChanged;
            if (playerStateChanged != null)
            {
                playerStateChanged(this, new PlayerStateChangedArgs(player));
            }
        }

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
