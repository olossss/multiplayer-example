// -----------------------------------------------------------------------
// <copyright file="Sprite.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

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

        protected readonly List<Rectangle> frames = new List<Rectangle>();

        protected int collisionRadius;

        private readonly GameTimer frameTimer = new GameTimer();

        private int currentFrameIndex;

        private Color tintColor = Color.White;

        #endregion

        #region Constructors and Destructors

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

        #region Properties

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

        public Vector2 Center
        {
            get
            {
                return this.SimulationState.Position +
                       new Vector2(this.InitialFrame.Width / 2, this.InitialFrame.Height / 2);
            }
        }

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

        public Rectangle CurrentFrame
        {
            get
            {
                return this.frames[this.currentFrameIndex];
            }
        }

        

       

        public long Id { get; set; }

        public Rectangle InitialFrame { get; set; }

        public double LastUpdateTime { get; set; }

        public EntityState PrevDisplayState { get; set; }

        public EntityState SimulationState { get; set; }

        public EntityState DisplayState { get; set; }

        public bool EnableSmoothing { get; set; }

        public Texture2D Texture { get; set; }

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

        #region Public Methods

        

        public bool IsCircleColliding(Vector2 otherCenter, float otherRadius)
        {
            return Vector2.Distance(this.Center, otherCenter) < (this.CollisionRadius + otherRadius);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (this.EnableSmoothing)
            {
                spriteBatch.Draw(
                    this.Texture,
                    this.SimulationState.Position + new Vector2(this.InitialFrame.Width / 2, this.InitialFrame.Height / 2),
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

                this.ApplySmoothing(1/12f);
            }
            else
            {
                this.DisplayState = (EntityState)this.SimulationState.Clone();
            }
        }

        #endregion

        #region Methods

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