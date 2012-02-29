// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Shot.cs" company="">
//   
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiplayerGame.Entities
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Shot : Sprite
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Shot"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="texture">
        /// The texture.
        /// </param>
        /// <param name="initialFrame">
        /// The initial frame.
        /// </param>
        /// <param name="frameCount">
        /// The frame count.
        /// </param>
        /// <param name="collisionRadius">
        /// The collision radius.
        /// </param>
        /// <param name="simulationState">
        /// The simulation state.
        /// </param>
        /// <param name="firedById">
        /// The fired by id.
        /// </param>
        /// <param name="firedByPlayer">
        /// The fired by player.
        /// </param>
        internal Shot(
            long id, 
            Texture2D texture, 
            Rectangle initialFrame, 
            int frameCount, 
            int collisionRadius, 
            EntityState simulationState, 
            long firedById, 
            bool firedByPlayer)
            : base(id, texture, initialFrame, frameCount, collisionRadius, simulationState)
        {
            this.FiredByPlayer = firedByPlayer;
            this.FiredById = firedById;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets FiredById.
        /// </summary>
        public long FiredById { get; private set; }

        /// <summary>
        /// Gets a value indicating whether FiredByPlayer.
        /// </summary>
        public bool FiredByPlayer { get; private set; }

        #endregion
    }
}