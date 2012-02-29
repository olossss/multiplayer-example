// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShotFiredArgs.cs" company="">
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
    public class ShotFiredArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShotFiredArgs"/> class.
        /// </summary>
        /// <param name="shot">
        /// The shot.
        /// </param>
        public ShotFiredArgs(Shot shot)
        {
            this.Shot = shot;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Shot.
        /// </summary>
        public Shot Shot { get; private set; }

        #endregion
    }
}