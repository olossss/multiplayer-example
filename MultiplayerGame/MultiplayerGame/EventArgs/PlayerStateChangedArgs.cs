// -----------------------------------------------------------------------
// <copyright file="PlayerStateChangedArgs.cs" company="Microsoft">
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
    public class PlayerStateChangedArgs : EventArgs
    {
        #region Constructors and Destructors

        public PlayerStateChangedArgs(Player player)
        {
            this.Player = player;
        }

        #endregion

        #region Properties

        public Player Player { get; private set; }

        #endregion
    }
}
