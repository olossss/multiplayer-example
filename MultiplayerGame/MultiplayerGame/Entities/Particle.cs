// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Particle.cs" company="">
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
    public class Particle : Sprite
    {
        #region Constants and Fields

        /// <summary>
        /// The acceleration.
        /// </summary>
        private readonly Vector2 acceleration;

        /// <summary>
        /// The final color.
        /// </summary>
        private readonly Color finalColor;

        /// <summary>
        /// The initial color.
        /// </summary>
        private readonly Color initialColor;

        /// <summary>
        /// The initial duration.
        /// </summary>
        private readonly int initialDuration;

        /// <summary>
        /// The max speed.
        /// </summary>
        private readonly float maxSpeed;

        /// <summary>
        /// The remaining duration.
        /// </summary>
        private int remainingDuration;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Particle"/> class.
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
        /// <param name="acceleration">
        /// The acceleration.
        /// </param>
        /// <param name="maxSpeed">
        /// The max speed.
        /// </param>
        /// <param name="duration">
        /// The duration.
        /// </param>
        /// <param name="initialColor">
        /// The initial color.
        /// </param>
        /// <param name="finalColor">
        /// The final color.
        /// </param>
        public Particle(
            long id, 
            Texture2D texture, 
            Rectangle initialFrame, 
            int frameCount, 
            int collisionRadius, 
            EntityState simulationState, 
            Vector2 acceleration, 
            float maxSpeed, 
            int duration, 
            Color initialColor, 
            Color finalColor)
            : base(id, texture, initialFrame, frameCount, collisionRadius, simulationState)
        {
            this.initialDuration = duration;
            this.remainingDuration = duration;
            this.acceleration = acceleration;
            this.initialColor = initialColor;
            this.maxSpeed = maxSpeed;
            this.finalColor = finalColor;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets DurationProgress.
        /// </summary>
        public float DurationProgress
        {
            get
            {
                return this.ElapsedDuration / (float)this.initialDuration;
            }
        }

        /// <summary>
        /// Gets ElapsedDuration.
        /// </summary>
        public int ElapsedDuration
        {
            get
            {
                return this.initialDuration - this.remainingDuration;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsActive.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return this.remainingDuration > 0;
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
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.IsActive)
            {
                base.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        public override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                this.SimulationState.Velocity += this.acceleration;
                if (this.SimulationState.Velocity.Length() > this.maxSpeed)
                {
                    this.SimulationState.Velocity.Normalize();
                    this.SimulationState.Velocity *= this.maxSpeed;
                }

                this.TintColor = Color.Lerp(this.initialColor, this.finalColor, this.DurationProgress);
                this.remainingDuration--;

                base.Update(gameTime);
            }
        }

        #endregion
    }
}