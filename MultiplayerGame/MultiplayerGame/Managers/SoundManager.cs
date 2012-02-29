// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoundManager.cs" company="">
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

    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;

    using MultiplayerGame.RandomNumbers;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SoundManager
    {
        #region Constants and Fields

        /// <summary>
        /// The explosions.
        /// </summary>
        private readonly List<SoundEffect> explosions = new List<SoundEffect>();

        /// <summary>
        /// The rand.
        /// </summary>
        private readonly IRandomNumberGenerator randomNumberGenerator;

        /// <summary>
        /// The enemy shot.
        /// </summary>
        private SoundEffect enemyShot;

        /// <summary>
        /// The explosion count.
        /// </summary>
        private int explosionCount = 4;

        /// <summary>
        /// The player shot.
        /// </summary>
        private SoundEffect playerShot;

        /// <summary>
        /// The sound is enabled.
        /// </summary>
        private bool soundIsEnabled = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundManager"/> class.
        /// </summary>
        /// <param name="randomNumberGenerator">
        /// The random number generator.
        /// </param>
        public SoundManager(IRandomNumberGenerator randomNumberGenerator)
        {
            this.randomNumberGenerator = randomNumberGenerator;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether SoundIsEnabled.
        /// </summary>
        public bool SoundIsEnabled
        {
            get
            {
                return this.soundIsEnabled;
            }

            set
            {
                this.soundIsEnabled = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public void LoadContent(ContentManager content)
        {
            try
            {
                this.playerShot = content.Load<SoundEffect>(@"Sounds\Shot1");
                this.enemyShot = content.Load<SoundEffect>(@"Sounds\Shot2");
                for (int x = 1; x <= this.explosionCount; x++)
                {
                    this.explosions.Add(content.Load<SoundEffect>(@"Sounds\Explosion" + x));
                }
            }
            catch
            {
                Console.WriteLine("SoundManager Initialization Failed");
            }
        }

        /// <summary>
        /// The play enemy shot.
        /// </summary>
        public void PlayEnemyShot()
        {
            try
            {
                if (this.SoundIsEnabled)
                {
                    this.enemyShot.Play();
                }
            }
            catch
            {
                Console.WriteLine("PlayEnemyShot Failed");
            }
        }

        /// <summary>
        /// The play explosion.
        /// </summary>
        public void PlayExplosion()
        {
            try
            {
                if (this.SoundIsEnabled)
                {
                    this.explosions[this.randomNumberGenerator.Next(0, this.explosionCount)].Play();
                }
            }
            catch
            {
                Console.WriteLine("PlayExplosion Failed");
            }
        }

        /// <summary>
        /// The play player shot.
        /// </summary>
        public void PlayPlayerShot()
        {
            try
            {
                if (this.SoundIsEnabled)
                {
                    this.playerShot.Play();
                }
            }
            catch
            {
                Console.WriteLine("PlayPlayerShot Failed");
            }
        }

        #endregion
    }
}