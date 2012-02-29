// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sprite.cs" company="">
//   
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiplayerGame.Entities
{
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Sprite
    {
        #region Constants and Fields

        /// <summary>
        /// The frames.
        /// </summary>
        protected readonly List<Rectangle> frames = new List<Rectangle>();

        /// <summary>
        /// The collision radius.
        /// </summary>
        protected int collisionRadius;

        /// <summary>
        /// The frame timer.
        /// </summary>
        private readonly GameTimer frameTimer = new GameTimer();

        /// <summary>
        /// The current frame index.
        /// </summary>
        private int currentFrameIndex;

        /// <summary>
        /// The tint color.
        /// </summary>
        private Color tintColor = Color.White;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class.
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
        internal Sprite(
            long id, 
            Texture2D texture, 
            Rectangle initialFrame, 
            int frameCount, 
            int collisionRadius, 
            EntityState simulationState)
        {
            this.Id = id;
            this.Texture = texture;
            this.InitialFrame = initialFrame;
            this.collisionRadius = collisionRadius;

            this.frames.Add(initialFrame);

            for (int x = 1; x < frameCount; x++)
            {
                this.frames.Add(
                    new Rectangle(
                        initialFrame.X + (initialFrame.Width * x), 
                        initialFrame.Y, 
                        initialFrame.Width, 
                        initialFrame.Height));
            }

            this.SimulationState = simulationState;
            this.DisplayState = (EntityState)simulationState.Clone();
            this.PrevDisplayState = (EntityState)simulationState.Clone();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Bounds.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    (int)this.SimulationState.Position.X, 
                    (int)this.SimulationState.Position.Y, 
                    this.InitialFrame.Width, 
                    this.InitialFrame.Height);
            }
        }

        /// <summary>
        /// Gets Center.
        /// </summary>
        public Vector2 Center
        {
            get
            {
                return this.SimulationState.Position
                       + new Vector2(this.InitialFrame.Width / 2, this.InitialFrame.Height / 2);
            }
        }

        /// <summary>
        /// Gets or sets CollisionRadius.
        /// </summary>
        public int CollisionRadius
        {
            get
            {
                return this.collisionRadius;
            }

            set
            {
                this.collisionRadius = value;
            }
        }

        /// <summary>
        /// Gets CurrentFrame.
        /// </summary>
        public Rectangle CurrentFrame
        {
            get
            {
                return this.frames[this.currentFrameIndex];
            }
        }

        /// <summary>
        /// Gets or sets DisplayState.
        /// </summary>
        public EntityState DisplayState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether EnableSmoothing.
        /// </summary>
        public bool EnableSmoothing { get; set; }

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets InitialFrame.
        /// </summary>
        public Rectangle InitialFrame { get; set; }

        /// <summary>
        /// Gets or sets LastUpdateTime.
        /// </summary>
        public double LastUpdateTime { get; set; }

        /// <summary>
        /// Gets or sets PrevDisplayState.
        /// </summary>
        public EntityState PrevDisplayState { get; set; }

        /// <summary>
        /// Gets or sets SimulationState.
        /// </summary>
        public EntityState SimulationState { get; set; }

        /// <summary>
        /// Gets or sets Texture.
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// Gets or sets TintColor.
        /// </summary>
        public Color TintColor
        {
            get
            {
                return this.tintColor;
            }

            set
            {
                this.tintColor = value;
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
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (this.EnableSmoothing)
            {
                spriteBatch.Draw(
                    this.Texture, 
                    this.SimulationState.Position
                    + new Vector2(this.InitialFrame.Width / 2, this.InitialFrame.Height / 2), 
                    this.CurrentFrame, 
                    Color.DarkSlateGray, 
                    this.SimulationState.Rotation, 
                    new Vector2(this.InitialFrame.Width / 2, this.InitialFrame.Height / 2), 
                    1.0f, 
                    SpriteEffects.None, 
                    0.0f);
            }

            spriteBatch.Draw(
                this.Texture, 
                this.DisplayState.Position + new Vector2(this.InitialFrame.Width / 2, this.InitialFrame.Height / 2), 
                this.CurrentFrame, 
                this.TintColor, 
                this.DisplayState.Rotation, 
                new Vector2(this.InitialFrame.Width / 2, this.InitialFrame.Height / 2), 
                1.0f, 
                SpriteEffects.None, 
                0.0f);
        }

        /// <summary>
        /// The is circle colliding.
        /// </summary>
        /// <param name="otherCenter">
        /// The other center.
        /// </param>
        /// <param name="otherRadius">
        /// The other radius.
        /// </param>
        /// <returns>
        /// The is circle colliding.
        /// </returns>
        public bool IsCircleColliding(Vector2 otherCenter, float otherRadius)
        {
            return Vector2.Distance(this.Center, otherCenter) < (this.CollisionRadius + otherRadius);
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        public virtual void Update(GameTime gameTime)
        {
            var elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (this.frameTimer.Stopwatch(100))
            {
                this.currentFrameIndex = (this.currentFrameIndex + 1) % this.frames.Count;
            }

            this.SimulationState.Position += this.SimulationState.Velocity * elapsedSeconds;

            if (this.EnableSmoothing)
            {
                this.PrevDisplayState.Position += this.PrevDisplayState.Velocity * elapsedSeconds;

                this.ApplySmoothing(1 / 12f);
            }
            else
            {
                this.DisplayState = (EntityState)this.SimulationState.Clone();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The apply smoothing.
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        private void ApplySmoothing(float delta)
        {
            this.DisplayState.Position = Vector2.Lerp(
                this.PrevDisplayState.Position, this.SimulationState.Position, delta);

            this.DisplayState.Velocity = Vector2.Lerp(
                this.PrevDisplayState.Velocity, this.SimulationState.Velocity, delta);

            this.DisplayState.Rotation = MathHelper.Lerp(
                this.PrevDisplayState.Rotation, this.SimulationState.Rotation, delta);

            this.PrevDisplayState = (EntityState)this.DisplayState.Clone();
        }

        #endregion
    }
}