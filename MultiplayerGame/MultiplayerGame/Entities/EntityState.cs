// -----------------------------------------------------------------------
// <copyright file="EntityState.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Xna.Framework;

namespace MultiplayerGame.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EntityState : ICloneable
    {
        #region Constants and Fields

        private float rotation;

        #endregion

        #region Properties

        public Vector2 Position { get; set; }

        public float Rotation
        {
            get
            {
                return this.rotation;
            }
            set
            {
                if (this.rotation == value % MathHelper.TwoPi)
                {
                    return;
                }

                this.rotation = value % MathHelper.TwoPi;
            }
        }

        public Vector2 Velocity { get; set; }

        #endregion

        #region Implemented Interfaces

        #region ICloneable

        public object Clone()
        {
            return new EntityState { Position = this.Position, Rotation = this.Rotation, Velocity = this.Velocity };
        }

        #endregion

        #endregion
    }
}
