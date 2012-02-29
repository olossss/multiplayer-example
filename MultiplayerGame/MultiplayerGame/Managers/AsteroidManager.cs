// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsteroidManager.cs" company="">
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
    using System.Linq;
    using System.Threading;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    using MultiplayerGame.Entities;
    using MultiplayerGame.EventArgs;
    using MultiplayerGame.RandomNumbers;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AsteroidManager
    {
        #region Constants and Fields

        /// <summary>
        /// The asteroids.
        /// </summary>
        private readonly Dictionary<long, Asteroid> asteroids = new Dictionary<long, Asteroid>();

        /// <summary>
        /// The is host.
        /// </summary>
        private readonly bool isHost;

        /// <summary>
        /// The resolution manager.
        /// </summary>
        private readonly ResolutionManager resolutionManager;

        /// <summary>
        /// The screen padding.
        /// </summary>
        private readonly Thickness screenPadding = new Thickness(50);

        /// <summary>
        /// The asteroid id counter.
        /// </summary>
        private static long asteroidIdCounter;

        /// <summary>
        /// The asteroid collision radius.
        /// </summary>
        private int asteroidCollisionRadius = 15;

        /// <summary>
        /// The asteroid count.
        /// </summary>
        private int asteroidCount = 10;

        /// <summary>
        /// The hearbeat timer.
        /// </summary>
        private GameTimer hearbeatTimer;

        /// <summary>
        /// The initial asteroid frame.
        /// </summary>
        private Rectangle initialAsteroidFrame = new Rectangle(0, 0, 50, 50);

        /// <summary>
        /// The max asteroid speed.
        /// </summary>
        private float maxAsteroidSpeed = 60;

        /// <summary>
        /// The min asteroid speed.
        /// </summary>
        private float minAsteroidSpeed = 30;

        /// <summary>
        /// The no of asteroid frames.
        /// </summary>
        private int noOfAsteroidFrames = 20;

        /// <summary>
        /// The random number generator.
        /// </summary>
        private IRandomNumberGenerator randomNumberGenerator = NullRandomNumberGenerator.Instance;

        /// <summary>
        /// The sprite sheet.
        /// </summary>
        private Texture2D spriteSheet;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AsteroidManager"/> class.
        /// </summary>
        /// <param name="resolutionManager">
        /// The resolution manager.
        /// </param>
        /// <param name="randomNumberGenerator">
        /// The random number generator.
        /// </param>
        /// <param name="isHost">
        /// The is host.
        /// </param>
        public AsteroidManager(
            ResolutionManager resolutionManager, IRandomNumberGenerator randomNumberGenerator, bool isHost)
        {
            this.isHost = isHost;
            this.resolutionManager = resolutionManager;
            this.randomNumberGenerator = randomNumberGenerator;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The asteroid state changed.
        /// </summary>
        public event EventHandler<AsteroidStateChangedArgs> AsteroidStateChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Asteroids.
        /// </summary>
        public IEnumerable<Asteroid> Asteroids
        {
            get
            {
                return this.asteroids.Values;
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
        /// The add asteroid.
        /// </summary>
        /// <returns>
        /// </returns>
        public Asteroid AddAsteroid()
        {
            EntityState physicsState = this.SelectRandomEntityState();

            Asteroid asteroid = this.AddAsteroid(
                Interlocked.Increment(ref asteroidIdCounter), 
                physicsState.Position, 
                physicsState.Velocity, 
                physicsState.Rotation);

            this.OnAsteroidStateChanged(asteroid);

            return asteroid;
        }

        /// <summary>
        /// The add asteroid.
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
        /// <returns>
        /// </returns>
        public Asteroid AddAsteroid(long id, Vector2 position, Vector2 velocity, float rotation)
        {
            if (this.asteroids.ContainsKey(id))
            {
                return this.asteroids[id];
            }

            var asteroid = new Asteroid(
                id, 
                this.spriteSheet, 
                this.initialAsteroidFrame, 
                this.noOfAsteroidFrames, 
                this.asteroidCollisionRadius, 
                new EntityState { Position = position, Rotation = rotation, Velocity = velocity });

            this.asteroids.Add(asteroid.Id, asteroid);

            return asteroid;
        }

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="spriteBatch">
        /// The sprite batch.
        /// </param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Asteroid asteroid in this.asteroids.Values)
            {
                asteroid.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// The get asteroid.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// </returns>
        public Asteroid GetAsteroid(long id)
        {
            if (this.asteroids.ContainsKey(id))
            {
                return this.asteroids[id];
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
        }

        /// <summary>
        /// The select random entity state.
        /// </summary>
        /// <returns>
        /// </returns>
        public EntityState SelectRandomEntityState()
        {
            var physicsState = new EntityState();
            bool physicsStateIsOk = true;
            int tryCount = 0;

            physicsState.Rotation = MathHelper.ToRadians(this.randomNumberGenerator.Next(0, 360));

            do
            {
                physicsStateIsOk = true;
                Vector2 velocity = Vector2.Zero;

                switch (this.randomNumberGenerator.Next(0, 2))
                {
                    case 0:
                        physicsState.Position = new Vector2(
                            -this.screenPadding.Left, 
                            this.randomNumberGenerator.Next(0, this.resolutionManager.DisplayViewport.Height));

                        velocity = new Vector2(
                            this.randomNumberGenerator.Next(0, 50), this.randomNumberGenerator.Next(0, 100) - 50);
                        break;

                    case 1:
                        physicsState.Position = new Vector2(
                            this.resolutionManager.DisplayViewport.Width, 
                            this.randomNumberGenerator.Next(0, this.resolutionManager.DisplayViewport.Height));

                        velocity = new Vector2(
                            this.randomNumberGenerator.Next(0, 50) - 50, this.randomNumberGenerator.Next(0, 100) - 50);
                        break;

                    case 2:
                        physicsState.Position =
                            new Vector2(
                                this.randomNumberGenerator.Next(0, this.resolutionManager.DisplayViewport.Width), 
                                -this.screenPadding.Top);

                        velocity = new Vector2(
                            this.randomNumberGenerator.Next(0, 100) - 50, this.randomNumberGenerator.Next(0, 50));
                        break;
                }

                velocity.Normalize();
                velocity *= this.randomNumberGenerator.Next((int)this.minAsteroidSpeed, (int)this.maxAsteroidSpeed);
                physicsState.Velocity = velocity;

                foreach (Asteroid asteroid in this.asteroids.Values)
                {
                    if (
                        asteroid.Bounds.Intersects(
                            new Rectangle(
                                (int)physicsState.Position.X, 
                                (int)physicsState.Position.Y, 
                                this.initialAsteroidFrame.Width, 
                                this.initialAsteroidFrame.Height)))
                    {
                        physicsStateIsOk = false;
                    }
                }

                tryCount++;
                if ((tryCount > 5) && physicsStateIsOk == false)
                {
                    physicsState.Position = new Vector2(-500, -500);
                    physicsStateIsOk = true;
                }
            }
            while (physicsStateIsOk == false);

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
            foreach (Asteroid asteroid in this.Asteroids)
            {
                asteroid.Update(gameTime);
                if (this.isHost)
                {
                    if (!this.IsOnScreen(asteroid))
                    {
                        asteroid.ResetAsteroidState(this.SelectRandomEntityState());
                        this.OnAsteroidStateChanged(asteroid);
                    }
                }
            }

            var processedList = new List<Asteroid>();
            foreach (Asteroid a1 in this.Asteroids)
            {
                processedList.Add(a1);
                foreach (Asteroid a2 in this.asteroids.Values.Except(processedList))
                {
                    if (a1.IsCircleColliding(a2.Center, a2.CollisionRadius))
                    {
                        this.BounceAsteroids(a1, a2);
                    }
                }
            }

            if (this.isHost)
            {
                for (int i = this.asteroids.Count; i < this.asteroidCount; i++)
                {
                    this.AddAsteroid();
                }

                if (this.hearbeatTimer.Stopwatch(200))
                {
                    foreach (Asteroid asteroid in this.Asteroids)
                    {
                        this.OnAsteroidStateChanged(asteroid);
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on asteroid state changed.
        /// </summary>
        /// <param name="asteroid">
        /// The asteroid.
        /// </param>
        protected virtual void OnAsteroidStateChanged(Asteroid asteroid)
        {
            EventHandler<AsteroidStateChangedArgs> asteroidEntityStateChanged = this.AsteroidStateChanged;
            if (asteroidEntityStateChanged != null)
            {
                asteroidEntityStateChanged(this, new AsteroidStateChangedArgs(asteroid));
            }
        }

        /// <summary>
        /// The bounce asteroids.
        /// </summary>
        /// <param name="asteroid1">
        /// The asteroid 1.
        /// </param>
        /// <param name="asteroid2">
        /// The asteroid 2.
        /// </param>
        private void BounceAsteroids(Asteroid asteroid1, Asteroid asteroid2)
        {
            Vector2 cOfMass = (asteroid1.SimulationState.Velocity + asteroid2.SimulationState.Velocity) / 2;

            Vector2 normal1 = asteroid2.Center - asteroid1.Center;
            normal1.Normalize();

            Vector2 normal2 = asteroid1.Center - asteroid2.Center;
            normal2.Normalize();

            Vector2 newVelocity = asteroid1.SimulationState.Velocity - cOfMass;
            newVelocity = Vector2.Reflect(newVelocity, normal1);
            asteroid1.SimulationState.Velocity = newVelocity + cOfMass;

            newVelocity = asteroid2.SimulationState.Velocity - cOfMass;
            newVelocity = Vector2.Reflect(newVelocity, normal2);
            asteroid2.SimulationState.Velocity = newVelocity + cOfMass;

            this.OnAsteroidStateChanged(asteroid1);
            this.OnAsteroidStateChanged(asteroid2);
        }

        /// <summary>
        /// The is on screen.
        /// </summary>
        /// <param name="asteroid">
        /// The asteroid.
        /// </param>
        /// <returns>
        /// The is on screen.
        /// </returns>
        private bool IsOnScreen(Asteroid asteroid)
        {
            return
                asteroid.Bounds.Intersects(
                    new Rectangle(
                        -this.screenPadding.Left, 
                        -this.screenPadding.Top, 
                        this.resolutionManager.DisplayViewport.Width + this.screenPadding.Right, 
                        this.resolutionManager.DisplayViewport.Height + this.screenPadding.Bottom));
        }

        #endregion
    }
}