// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityState.cs" company="">
//   
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiplayerGame.Entities
{
    using System;

    using Microsoft.Xna.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EntityState : ICloneable
    {
        #region Constants and Fields

        /// <summary>
        /// The rotation.
        /// </summary>
        private float rotation;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Position.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets Rotation.
        /// </summary>
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

        /// <summary>
        /// Gets or sets Velocity.
        /// </summary>
        public Vector2 Velocity { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public object Clone()
        {
            return new EntityState { Position = this.Position, Rotation = this.Rotation, Velocity = this.Velocity };
        }

        #endregion
    }
}