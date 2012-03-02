// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollisionManager.cs" company="">
//   
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiplayerGame.Managers
{
    using System.Linq;

    using Microsoft.Xna.Framework;

    using MultiplayerGame.Entities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CollisionManager
    {
        #region Constants and Fields

        /// <summary>
        /// The asteroid manager.
        /// </summary>
        private readonly AsteroidManager asteroidManager;

        /// <summary>
        /// The enemy manager.
        /// </summary>
        private readonly EnemyManager enemyManager;

        /// <summary>
        /// The explosion manager.
        /// </summary>
        private readonly ExplosionManager explosionManager;

        /// <summary>
        /// The off screen.
        /// </summary>
        private readonly Vector2 offScreen = new Vector2(-500, -500);

        /// <summary>
        /// The player manager.
        /// </summary>
        private readonly PlayerManager playerManager;

        /// <summary>
        /// The shot manager.
        /// </summary>
        private readonly ShotManager shotManager;

        /// <summary>
        /// The shot to asteroid impact.
        /// </summary>
        private readonly Vector2 shotToAsteroidImpact = new Vector2(0, -20);

        /// <summary>
        /// The enemy point value.
        /// </summary>
        private int enemyPointValue = 100;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionManager"/> class.
        /// </summary>
        /// <param name="asteroidManager">
        /// The asteroid manager.
        /// </param>
        /// <param name="playerManager">
        /// The player manager.
        /// </param>
        /// <param name="enemyManager">
        /// The enemy manager.
        /// </param>
        /// <param name="explosionManager">
        /// The explosion manager.
        /// </param>
        /// <param name="shotManager">
        /// The shot manager.
        /// </param>
        public CollisionManager(
            AsteroidManager asteroidManager, 
            PlayerManager playerManager, 
            EnemyManager enemyManager, 
            ExplosionManager explosionManager, 
            ShotManager shotManager)
        {
            this.asteroidManager = asteroidManager;
            this.playerManager = playerManager;
            this.enemyManager = enemyManager;
            this.explosionManager = explosionManager;
            this.shotManager = shotManager;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The check collisions.
        /// </summary>
        /// <param name="gameTime">
        /// The game Time.
        /// </param>
        public void Update(GameTime gameTime)
        {
            this.CheckShotToEnemyCollisions();
            this.CheckShotToAsteroidCollisions();

            foreach (Player player in this.playerManager.Players)
            {
                if ((!player.IsDestroyed) && (!player.IsInvulnerable))
                {
                    this.CheckShotToPlayerCollisions(player);
                    this.CheckEnemyToPlayerCollisions(player);
                    this.CheckAsteroidToPlayerCollisions(player);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The check asteroid to player collisions.
        /// </summary>
        /// <param name="player">
        /// The player.
        /// </param>
        private void CheckAsteroidToPlayerCollisions(Player player)
        {
            foreach (Asteroid asteroid in this.asteroidManager.Asteroids)
            {
                if (asteroid.IsCircleColliding(player.Center, player.CollisionRadius))
                {
                    this.explosionManager.AddExplosion(asteroid.Center, asteroid.SimulationState.Velocity / 10);
                    asteroid.SimulationState.Position = this.offScreen;
                    player.IsDestroyed = true;
                    this.explosionManager.AddExplosion(player.Center, Vector2.Zero);
                }
            }
        }

        /// <summary>
        /// The check enemy to player collisions.
        /// </summary>
        /// <param name="player">
        /// The player.
        /// </param>
        private void CheckEnemyToPlayerCollisions(Player player)
        {
            foreach (Enemy enemy in this.enemyManager.Enemies)
            {
                if (enemy.IsCircleColliding(player.Center, player.CollisionRadius))
                {
                    enemy.IsDestroyed = true;
                    this.explosionManager.AddExplosion(enemy.Center, enemy.SimulationState.Velocity / 10);
                    player.IsDestroyed = true;
                    this.explosionManager.AddExplosion(player.Center, Vector2.Zero);
                }
            }
        }

        /// <summary>
        /// The check shot to asteroid collisions.
        /// </summary>
        private void CheckShotToAsteroidCollisions()
        {
            foreach (Shot shot in this.shotManager.Shots.Where(x => x.FiredByPlayer))
            {
                foreach (Asteroid asteroid in this.asteroidManager.Asteroids)
                {
                    if (shot.IsCircleColliding(asteroid.Center, asteroid.CollisionRadius))
                    {
                        shot.SimulationState.Position = this.offScreen;
                        asteroid.SimulationState.Velocity += this.shotToAsteroidImpact;
                    }
                }
            }
        }

        /// <summary>
        /// The check shot to enemy collisions.
        /// </summary>
        private void CheckShotToEnemyCollisions()
        {
            foreach (Shot shot in this.shotManager.Shots.Where(x => x.FiredByPlayer))
            {
                foreach (Enemy enemy in this.enemyManager.Enemies)
                {
                    if (shot.IsCircleColliding(enemy.Center, enemy.CollisionRadius))
                    {
                        shot.SimulationState.Position = this.offScreen;
                        enemy.IsDestroyed = true;
                        this.playerManager.GetPlayer(shot.FiredById).Score += this.enemyPointValue;
                        this.explosionManager.AddExplosion(enemy.Center, enemy.SimulationState.Velocity / 10);
                    }
                }
            }
        }

        /// <summary>
        /// The check shot to player collisions.
        /// </summary>
        /// <param name="player">
        /// The player.
        /// </param>
        private void CheckShotToPlayerCollisions(Player player)
        {
            foreach (Shot shot in this.shotManager.Shots.Where(x => !x.FiredByPlayer))
            {
                if (shot.IsCircleColliding(player.Center, player.CollisionRadius))
                {
                    shot.SimulationState.Position = this.offScreen;
                    player.IsDestroyed = true;
                    this.explosionManager.AddExplosion(player.Center, Vector2.Zero);
                }
            }
        }

        #endregion
    }
}