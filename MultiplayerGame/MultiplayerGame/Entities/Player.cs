// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Player.cs" company="">
//   
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiplayerGame.Entities
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Player : Sprite
    {
        #region Constants and Fields

        /// <summary>
        /// The invulnerable timer.
        /// </summary>
        private readonly GameTimer invulnerableTimer = new GameTimer();

        /// <summary>
        /// The is invulnerable.
        /// </summary>
        private bool isInvulnerable;

        /// <summary>
        /// The lives remaining.
        /// </summary>
        private int livesRemaining = 3;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="texture">
        /// The texture.
        /// </param>
        /// <param name="initialFrame">
        /// The initial frame.
        /// </param>
        /// <param name="frameCount">
        /// The frame count.
        /// </param>
        /// <param name="collisionRadius">
        /// The collision radius.
        /// </param>
        /// <param name="simulationState">
        /// The simulation state.
        /// </param>
        internal Player(
            long id, 
            Texture2D texture, 
            Rectangle initialFrame, 
            int frameCount, 
            int collisionRadius, 
            EntityState simulationState)
            : base(id, texture, initialFrame, frameCount, collisionRadius, simulationState)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether IsDestroyed.
        /// </summary>
        public bool IsDestroyed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsInvulnerable.
        /// </summary>
        public bool IsInvulnerable
        {
            get
            {
                return this.isInvulnerable;
            }

            set
            {
                if (this.isInvulnerable == value)
                {
                    return;
                }

                this.isInvulnerable = value;
                if (this.isInvulnerable)
                {
                    this.invulnerableTimer.Reset();
                }
            }
        }

        /// <summary>
        /// Gets or sets LivesRemaining.
        /// </summary>
        public int LivesRemaining
        {
            get
            {
                return this.livesRemaining;
            }

            set
            {
                this.livesRemaining = value;
            }
        }

        /// <summary>
        /// Gets or sets Score.
        /// </summary>
        public int Score { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="spriteBatch">
        /// The sprite batch.
        /// </param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            this.TintColor = this.IsInvulnerable ? Color.DarkSlateGray : Color.White;

            base.Draw(spriteBatch);
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        public override void Update(GameTime gameTime)
        {
            if (this.IsDestroyed)
            {
                this.LivesRemaining--;

                if (this.LivesRemaining > 0)
                {
                    this.IsDestroyed = false;
                    this.IsInvulnerable = true;
                }
            }

            if (this.IsInvulnerable && this.invulnerableTimer.Stopwatch(1000))
            {
                this.IsInvulnerable = false;
            }

            if (!this.IsDestroyed)
            {
                base.Update(gameTime);
            }
        }

        #endregion
    }
}