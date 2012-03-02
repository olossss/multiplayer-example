// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpawnEnemyArgs.cs" company="">
//   
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiplayerGame.EventArgs
{
    using System;

    using MultiplayerGame.Entities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SpawnEnemyArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnEnemyArgs"/> class.
        /// </summary>
        /// <param name="enemy">
        /// The enemy.
        /// </param>
        public SpawnEnemyArgs(Enemy enemy)
        {
            this.Enemy = enemy;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Enemy.
        /// </summary>
        public Enemy Enemy { get; private set; }

        #endregion
    }
}