// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnemyManager.cs" company="">
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
    public class EnemyManager
    {
        #region Constants and Fields

        /// <summary>
        /// The enemies.
        /// </summary>
        private readonly List<Enemy> enemies = new List<Enemy>();

        /// <summary>
        /// The gun offset.
        /// </summary>
        private readonly Vector2 gunOffset = new Vector2(25, 25);

        /// <summary>
        /// The initial enemy frame.
        /// </summary>
        private readonly Rectangle initialEnemyFrame = new Rectangle(0, 200, 50, 50);

        /// <summary>
        /// The is host.
        /// </summary>
        private readonly bool isHost;

        /// <summary>
        /// The no of enemy frames.
        /// </summary>
        private readonly int noOfEnemyFrames = 6;

        /// <summary>
        /// The path waypoints.
        /// </summary>
        private readonly List<List<Vector2>> pathWaypoints = new List<List<Vector2>>();

        /// <summary>
        /// The player manager.
        /// </summary>
        private readonly PlayerManager playerManager;

        /// <summary>
        /// The random number genertor.
        /// </summary>
        private readonly IRandomNumberGenerator randomNumberGenertor;

        /// <summary>
        /// The shot manager.
        /// </summary>
        private readonly ShotManager shotManager;

        /// <summary>
        /// The wave spawns.
        /// </summary>
        private readonly Dictionary<int, int> waveSpawns = new Dictionary<int, int>();

        /// <summary>
        /// The enemy id counter.
        /// </summary>
        private static long enemyIdCounter;

        /// <summary>
        /// The enemy collision radius.
        /// </summary>
        private int enemyCollisionRadius = 15;

        /// <summary>
        /// The is active.
        /// </summary>
        private bool isActive;

        /// <summary>
        /// The max ships per wave.
        /// </summary>
        private int maxShipsPerWave = 8;

        /// <summary>
        /// The min ships per wave.
        /// </summary>
        private int minShipsPerWave = 5;

        /// <summary>
        /// The next wave timer.
        /// </summary>
        private GameTimer nextWaveTimer;

        /// <summary>
        /// The ship shot chance.
        /// </summary>
        private float shipShotChance = 0.2f;

        /// <summary>
        /// The ship spawn timer.
        /// </summary>
        private GameTimer shipSpawnTimer;

        /// <summary>
        /// The sprite sheet.
        /// </summary>
        private Texture2D spriteSheet;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyManager"/> class.
        /// </summary>
        /// <param name="randomNumberGenerator">
        /// The random number generator.
        /// </param>
        /// <param name="shotManager">
        /// The shot manager.
        /// </param>
        /// <param name="playerManager">
        /// The player manager.
        /// </param>
        /// <param name="isHost">
        /// The is host.
        /// </param>
        public EnemyManager(
            IRandomNumberGenerator randomNumberGenerator, 
            ShotManager shotManager, 
            PlayerManager playerManager, 
            bool isHost)
        {
            this.isHost = isHost;
            this.randomNumberGenertor = randomNumberGenerator;
            this.shotManager = shotManager;
            this.playerManager = playerManager;

            this.SetUpWaypoints();

            this.isActive = true;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The enemy spawned.
        /// </summary>
        public event EventHandler<SpawnEnemyArgs> EnemySpawned;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Enemies.
        /// </summary>
        public List<Enemy> Enemies
        {
            get
            {
                return this.enemies;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsActive.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                this.isActive = value;
            }
        }

        /// <summary>
        /// Gets or sets MaxShipsPerWave.
        /// </summary>
        public int MaxShipsPerWave
        {
            get
            {
                return this.maxShipsPerWave;
            }

            set
            {
                this.maxShipsPerWave = value;
            }
        }

        /// <summary>
        /// Gets or sets MinShipsPerWave.
        /// </summary>
        public int MinShipsPerWave
        {
            get
            {
                return this.minShipsPerWave;
            }

            set
            {
                this.minShipsPerWave = value;
            }
        }

        /// <summary>
        /// Gets or sets ShipShotChance.
        /// </summary>
        public float ShipShotChance
        {
            get
            {
                return this.shipShotChance;
            }

            set
            {
                this.shipShotChance = value;
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
            foreach (Enemy enemy in this.enemies)
            {
                enemy.Draw(spriteBatch);
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
            this.shipSpawnTimer = new GameTimer();
            this.nextWaveTimer = new GameTimer();
        }

        /// <summary>
        /// The spawn enemy.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="path">
        /// The path.
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
        public Enemy SpawnEnemy(long id, int path, Vector2 position, Vector2 velocity, float rotation)
        {
            var enemy = new Enemy(
                id, 
                this.spriteSheet, 
                this.initialEnemyFrame, 
                this.noOfEnemyFrames, 
                this.enemyCollisionRadius, 
                new EntityState { Position = position, Velocity = velocity, Rotation = rotation }, 
                path);

            for (int x = 0; x < this.pathWaypoints[path].Count; x++)
            {
                enemy.AddWaypoiny(this.pathWaypoints[path][x]);
            }

            this.enemies.Add(enemy);

            return enemy;
        }

        /// <summary>
        /// The spawn wave.
        /// </summary>
        /// <param name="waveType">
        /// The wave type.
        /// </param>
        public void SpawnWave(int waveType)
        {
            this.waveSpawns[waveType] += this.randomNumberGenertor.Next(this.minShipsPerWave, this.maxShipsPerWave + 1);
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        public void Update(GameTime gameTime)
        {
            for (int x = this.enemies.Count - 1; x >= 0; x--)
            {
                this.enemies[x].Update(gameTime);
                if (this.enemies[x].IsActive == false)
                {
                    this.enemies.RemoveAt(x);
                }
                else
                {
                    if (this.isHost)
                    {
                        if ((float)this.randomNumberGenertor.Next(0, 1000) / 10 <= this.shipShotChance)
                        {
                            Vector2 fireLoc = this.enemies[x].SimulationState.Position;
                            fireLoc += this.gunOffset;

                            Vector2 shotDirection =
                                this.playerManager.Players.ToList()[
                                    this.randomNumberGenertor.Next(0, this.playerManager.Players.Count() - 1)].Center
                                - fireLoc;
                            shotDirection.Normalize();

                            this.shotManager.FireShot(fireLoc, shotDirection, this.enemies[x].Id, false);
                        }
                    }
                }
            }

            if (this.isActive && this.isHost)
            {
                this.UpdateWaveSpawns();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on enemy spawned.
        /// </summary>
        /// <param name="enemy">
        /// The enemy.
        /// </param>
        protected void OnEnemySpawned(Enemy enemy)
        {
            EventHandler<SpawnEnemyArgs> enemySpawned = this.EnemySpawned;
            if (enemySpawned != null)
            {
                enemySpawned(this, new SpawnEnemyArgs(enemy));
            }
        }

        /// <summary>
        /// The set up waypoints.
        /// </summary>
        private void SetUpWaypoints()
        {
            var path0 = new List<Vector2> { new Vector2(850, 300), new Vector2(-100, 300) };
            this.pathWaypoints.Add(path0);
            this.waveSpawns[0] = 0;

            var path1 = new List<Vector2> { new Vector2(-50, 225), new Vector2(850, 225) };
            this.pathWaypoints.Add(path1);
            this.waveSpawns[1] = 0;

            var path2 = new List<Vector2> {
                    new Vector2(-100, 50), 
                    new Vector2(150, 50), 
                    new Vector2(200, 75), 
                    new Vector2(200, 125), 
                    new Vector2(150, 150), 
                    new Vector2(150, 175), 
                    new Vector2(200, 200), 
                    new Vector2(600, 200), 
                    new Vector2(850, 600)
                };
            this.pathWaypoints.Add(path2);
            this.waveSpawns[2] = 0;

            var path3 = new List<Vector2> {
                    new Vector2(600, -100), 
                    new Vector2(600, 250), 
                    new Vector2(580, 275), 
                    new Vector2(500, 250), 
                    new Vector2(500, 200), 
                    new Vector2(450, 175), 
                    new Vector2(400, 150), 
                    new Vector2(-100, 150)
                };
            this.pathWaypoints.Add(path3);
            this.waveSpawns[3] = 0;
        }

        /// <summary>
        /// The spawn enemy.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        private Enemy SpawnEnemy(int path)
        {
            Enemy enemy = this.SpawnEnemy(
                Interlocked.Increment(ref enemyIdCounter), path, Vector2.Zero, Vector2.Zero, 0);
            this.OnEnemySpawned(enemy);
            return enemy;
        }

        /// <summary>
        /// The update wave spawns.
        /// </summary>
        private void UpdateWaveSpawns()
        {
            if (this.shipSpawnTimer.Stopwatch(500))
            {
                for (int x = this.waveSpawns.Count - 1; x >= 0; x--)
                {
                    if (this.waveSpawns[x] > 0)
                    {
                        this.waveSpawns[x]--;
                        this.SpawnEnemy(x);
                    }
                }
            }

            if (this.nextWaveTimer.Stopwatch(8000))
            {
                this.SpawnWave(this.randomNumberGenertor.Next(0, this.pathWaypoints.Count - 1));
            }
        }

        #endregion
    }
}