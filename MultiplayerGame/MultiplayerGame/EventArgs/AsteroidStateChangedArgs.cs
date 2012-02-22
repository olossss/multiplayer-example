// -----------------------------------------------------------------------
// <copyright file="AsteroidStateChangedArgs.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using MultiplayerGame.Entities;

namespace MultiplayerGame.EventArgs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AsteroidStateChangedArgs : EventArgs
    {
        #region Constructors and Destructors

        public AsteroidStateChangedArgs(Asteroid asteroid)
        {
            this.Asteroid = asteroid;
        }

        #endregion

        #region Properties

        public Asteroid Asteroid { get; private set; }

        #endregion
    }
}
