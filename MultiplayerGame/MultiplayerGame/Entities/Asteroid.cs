// -----------------------------------------------------------------------
// <copyright file="Asteroid.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MultiplayerGame.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Asteroid : Sprite
    {
        #region Constructors and Destructors

        internal Asteroid(
            long id,
            Texture2D texture,
            Rectangle initialFrame,
            int frameCount,
            int collisionRadius,
            EntityState simulationState)
            : base(id, texture, initialFrame, frameCount, collisionRadius, simulationState)
        {
        }

        #endregion
    }
}
