// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExplosionManager.cs" company="">
//   
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiplayerGame.Managers
{
    using System.Collections.Generic;
    using System.Threading;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    using MultiplayerGame.Entities;
    using MultiplayerGame.RandomNumbers;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExplosionManager
    {
        #region Constants and Fields

        /// <summary>
        /// The explosion particles.
        /// </summary>
        private readonly List<Particle> explosionParticles = new List<Particle>();

        /// <summary>
        /// The final color.
        /// </summary>
        private readonly Color finalColor = new Color(0f, 0f, 0f, 0f);

        /// <summary>
        /// The initial color.
        /// </summary>
        private readonly Color initialColor = new Color(1.0f, 0.3f, 0f) * 0.5f;

        /// <summary>
        /// The piece rectangles.
        /// </summary>
        private readonly List<Rectangle> pieceRectangles = new List<Rectangle>();

        /// <summary>
        /// The point rectangle.
        /// </summary>
        private readonly Rectangle pointRectangle = new Rectangle(0, 450, 2, 2);

        /// <summary>
        /// The random number generator.
        /// </summary>
        private readonly IRandomNumberGenerator randomNumberGenerator;

        /// <summary>
        /// The sound manager.
        /// </summary>
        private readonly SoundManager soundManager;

        /// <summary>
        /// The explosion id counter.
        /// </summary>
        private static long explosionIdCounter;

        /// <summary>
        /// The duration count.
        /// </summary>
        private int durationCount = 90;

        /// <summary>
        /// The explosion collision radius.
        /// </summary>
        private int explosionCollisionRadius;

        /// <summary>
        /// The explosion initial frame.
        /// </summary>
        private Rectangle explosionInitialFrame = new Rectangle(0, 100, 50, 50);

        /// <summary>
        /// The explosion max speed.
        /// </summary>
        private float explosionMaxSpeed = 30f;

        /// <summary>
        /// The max piece count.
        /// </summary>
        private int maxPieceCount = 6;

        /// <summary>
        /// The max point count.
        /// </summary>
        private int maxPointCount = 30;

        /// <summary>
        /// The min piece count.
        /// </summary>
        private int minPieceCount = 3;

        /// <summary>
        /// The min point count.
        /// </summary>
        private int minPointCount = 20;

        /// <summary>
        /// The no of explosion frames.
        /// </summary>
        private int noOfExplosionFrames = 1;

        /// <summary>
        /// The piece count.
        /// </summary>
        private int pieceCount = 3;

        /// <summary>
        /// The piece speed scale.
        /// </summary>
        private float pieceSpeedScale = 6f;

        /// <summary>
        /// The point speed max.
        /// </summary>
        private int pointSpeedMax = 30;

        /// <summary>
        /// The point speed min.
        /// </summary>
        private int pointSpeedMin = 15;

        /// <summary>
        /// The sprite sheet.
        /// </summary>
        private Texture2D spriteSheet;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosionManager"/> class.
        /// </summary>
        /// <param name="soundManager">
        /// The sound manager.
        /// </param>
        /// <param name="randomNumberGenerator">
        /// The random number generator.
        /// </param>
        public ExplosionManager(SoundManager soundManager, IRandomNumberGenerator randomNumberGenerator)
        {
            this.soundManager = soundManager;
            this.randomNumberGenerator = randomNumberGenerator;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add explosion.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="momentum">
        /// The momentum.
        /// </param>
        public void AddExplosion(long id, Vector2 position, Vector2 momentum)
        {
            Vector2 pieceLocation = position
                                    - new Vector2(this.pieceRectangles[0].Width / 2, this.pieceRectangles[0].Height / 2);

            int pieces = this.randomNumberGenerator.Next(this.minPieceCount, this.maxPieceCount + 1);

            for (int x = 0; x < pieces; x++)
            {
                this.explosionParticles.Add(
                    new Particle(
                        id, 
                        this.spriteSheet, 
                        this.pieceRectangles[this.randomNumberGenerator.Next(0, this.pieceRectangles.Count - 1)], 
                        this.noOfExplosionFrames, 
                        this.explosionCollisionRadius, 
                        new EntityState
                            {
                                Position = pieceLocation, 
                                Velocity = this.RandomDirection(this.pieceSpeedScale) + momentum, 
                                Rotation = 0
                            }, 
                        Vector2.Zero, 
                        this.explosionMaxSpeed, 
                        this.durationCount, 
                        this.initialColor, 
                        this.finalColor));
            }

            int points = this.randomNumberGenerator.Next(this.minPointCount, this.maxPointCount + 1);
            for (int x = 0; x < points; x++)
            {
                this.explosionParticles.Add(
                    new Particle(
                        id, 
                        this.spriteSheet, 
                        this.pointRectangle, 
                        this.noOfExplosionFrames, 
                        this.explosionCollisionRadius, 
                        new EntityState
                            {
                                Position = position, 
                                Velocity =
                                    this.RandomDirection(
                                        this.randomNumberGenerator.Next(this.pointSpeedMin, this.pointSpeedMax))
                                    + momentum, 
                                Rotation = 0
                            }, 
                        Vector2.Zero, 
                        this.explosionMaxSpeed, 
                        this.durationCount, 
                        this.initialColor, 
                        this.finalColor));
            }

            this.soundManager.PlayExplosion();
        }

        /// <summary>
        /// The add explosion.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="momentum">
        /// The momentum.
        /// </param>
        public void AddExplosion(Vector2 position, Vector2 momentum)
        {
            this.AddExplosion(Interlocked.Increment(ref explosionIdCounter), position, momentum);
        }

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="spriteBatch">
        /// The sprite batch.
        /// </param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particle particle in this.explosionParticles)
            {
                particle.Draw(spriteBatch);
            }
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

            for (int x = 0; x < this.pieceCount; x++)
            {
                this.pieceRectangles.Add(
                    new Rectangle(
                        this.explosionInitialFrame.X + (this.explosionInitialFrame.Width * x), 
                        this.explosionInitialFrame.Y, 
                        this.explosionInitialFrame.Width, 
                        this.explosionInitialFrame.Height));
            }
        }

        /// <summary>
        /// The random direction.
        /// </summary>
        /// <param name="scale">
        /// The scale.
        /// </param>
        /// <returns>
        /// </returns>
        public Vector2 RandomDirection(float scale)
        {
            Vector2 direction;
            do
            {
                direction = new Vector2(
                    this.randomNumberGenerator.Next(0, 101) - 50, this.randomNumberGenerator.Next(0, 101) - 50);
            }
            while (direction.Length() == 0);

            direction.Normalize();
            direction *= scale;

            return direction;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        public void Update(GameTime gameTime)
        {
            for (int x = this.explosionParticles.Count - 1; x >= 0; x--)
            {
                if (this.explosionParticles[x].IsActive)
                {
                    this.explosionParticles[x].Update(gameTime);
                }
                else
                {
                    this.explosionParticles.RemoveAt(x);
                }
            }
        }

        #endregion
    }
}