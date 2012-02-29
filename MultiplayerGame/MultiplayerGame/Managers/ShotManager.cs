// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShotManager.cs" company="">
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

    using MultiplayerGame.Entities;
    using MultiplayerGame.EventArgs;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ShotManager
    {
        #region Constants and Fields

        /// <summary>
        /// The initial shot frame.
        /// </summary>
        private readonly Rectangle initialShotFrame = new Rectangle(0, 300, 5, 5);

        /// <summary>
        /// The no of shot frames.
        /// </summary>
        private readonly int noOfShotFrames = 4;

        /// <summary>
        /// The resolution manager.
        /// </summary>
        private readonly ResolutionManager resolutionManager;

        /// <summary>
        /// The shot collision radius.
        /// </summary>
        private readonly int shotCollisionRadius = 2;

        /// <summary>
        /// The shot speed.
        /// </summary>
        private readonly float shotSpeed = 150f;

        /// <summary>
        /// The shots.
        /// </summary>
        private readonly List<Shot> shots = new List<Shot>();

        /// <summary>
        /// The sound manager.
        /// </summary>
        private readonly SoundManager soundManager;

        /// <summary>
        /// The shot id counter.
        /// </summary>
        private static long shotIdCounter;

        /// <summary>
        /// The sprite sheet.
        /// </summary>
        private Texture2D spriteSheet;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShotManager"/> class.
        /// </summary>
        /// <param name="resolutionManager">
        /// The resolution manager.
        /// </param>
        /// <param name="soundManager">
        /// The sound manager.
        /// </param>
        public ShotManager(ResolutionManager resolutionManager, SoundManager soundManager)
        {
            this.resolutionManager = resolutionManager;
            this.soundManager = soundManager;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The shot fired.
        /// </summary>
        public event EventHandler<ShotFiredArgs> ShotFired;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Shots.
        /// </summary>
        public List<Shot> Shots
        {
            get
            {
                return this.shots;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="spriteBatch">
        /// The sprite batch.
        /// </param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Shot shot in this.shots)
            {
                shot.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// The fire shot.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="velocity">
        /// The velocity.
        /// </param>
        /// <param name="firedById">
        /// The fired by id.
        /// </param>
        /// <param name="playerFired">
        /// The player fired.
        /// </param>
        /// <returns>
        /// </returns>
        public Shot FireShot(Vector2 position, Vector2 velocity, long firedById, bool playerFired)
        {
            Shot shot = this.FireShot(
                Interlocked.Increment(ref shotIdCounter), position, velocity * this.shotSpeed, firedById, playerFired);
            this.OnShotFired(shot);
            return shot;
        }

        /// <summary>
        /// The fire shot.
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
        /// <param name="firedById">
        /// The fired by id.
        /// </param>
        /// <param name="playerFired">
        /// The player fired.
        /// </param>
        /// <returns>
        /// </returns>
        public Shot FireShot(long id, Vector2 position, Vector2 velocity, long firedById, bool playerFired)
        {
            var shot = new Shot(
                id, 
                this.spriteSheet, 
                this.initialShotFrame, 
                this.noOfShotFrames, 
                this.shotCollisionRadius, 
                new EntityState { Position = position, Velocity = velocity }, 
                firedById, 
                playerFired);
            this.shots.Add(shot);

            if (playerFired)
            {
                this.soundManager.PlayPlayerShot();
            }
            else
            {
                this.soundManager.PlayEnemyShot();
            }

            return shot;
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
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        public void Update(GameTime gameTime)
        {
            for (int x = this.shots.Count - 1; x >= 0; x--)
            {
                this.shots[x].Update(gameTime);
                if (!this.resolutionManager.DisplayViewport.Bounds.Intersects(this.shots[x].Bounds))
                {
                    this.shots.RemoveAt(x);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on shot fired.
        /// </summary>
        /// <param name="shot">
        /// The shot.
        /// </param>
        protected void OnShotFired(Shot shot)
        {
            EventHandler<ShotFiredArgs> shotFired = this.ShotFired;
            if (shotFired != null)
            {
                shotFired(this, new ShotFiredArgs(shot));
            }
        }

        #endregion
    }
}