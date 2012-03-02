// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Enemy.cs" company="">
//   
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiplayerGame.Entities
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Enemy : Sprite
    {
        #region Constants and Fields

        /// <summary>
        /// The waypoints.
        /// </summary>
        private readonly Queue<Vector2> waypoints = new Queue<Vector2>();

        /// <summary>
        /// The current waypoint.
        /// </summary>
        private Vector2 currentWaypoint = Vector2.Zero;

        /// <summary>
        /// The is destroyed.
        /// </summary>
        private bool isDestroyed;

        /// <summary>
        /// The previous location.
        /// </summary>
        private Vector2 previousPosition = Vector2.Zero;

        /// <summary>
        /// The speed.
        /// </summary>
        private float speed = 120f;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Enemy"/> class.
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
        /// <param name="path">
        /// The path.
        /// </param>
        internal Enemy(
            long id, 
            Texture2D texture, 
            Rectangle initialFrame, 
            int frameCount, 
            int collisionRadius, 
            EntityState simulationState, 
            int path)
            : base(id, texture, initialFrame, frameCount, collisionRadius, simulationState)
        {
            this.Path = path;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether IsActive.
        /// </summary>
        public bool IsActive
        {
            get
            {
                if (this.isDestroyed)
                {
                    return false;
                }

                if (this.waypoints.Count > 0)
                {
                    return true;
                }

                if (this.WaypointReached())
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsDestroyed.
        /// </summary>
        public bool IsDestroyed
        {
            get
            {
                return this.isDestroyed;
            }

            set
            {
                this.isDestroyed = value;
            }
        }

        /// <summary>
        /// Gets Path.
        /// </summary>
        public int Path { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add waypoiny.
        /// </summary>
        /// <param name="waypoint">
        /// The waypoint.
        /// </param>
        public void AddWaypoiny(Vector2 waypoint)
        {
            this.waypoints.Enqueue(waypoint);
        }

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="spriteBatch">
        /// The sprite batch.
        /// </param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.IsActive)
            {
                base.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        public override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                Vector2 heading = this.currentWaypoint - this.SimulationState.Position;
                if (heading != Vector2.Zero)
                {
                    heading.Normalize();
                }

                heading *= this.speed;
                this.SimulationState.Velocity = heading;
                this.previousPosition = this.SimulationState.Position;

                base.Update(gameTime);

                this.SimulationState.Rotation =
                    (float)
                    Math.Atan2(
                        this.SimulationState.Position.Y - this.previousPosition.Y, 
                        this.SimulationState.Position.X - this.previousPosition.X);

                if (this.WaypointReached())
                {
                    if (this.waypoints.Count > 0)
                    {
                        this.currentWaypoint = this.waypoints.Dequeue();
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The waypoint reached.
        /// </summary>
        /// <returns>
        /// The waypoint reached.
        /// </returns>
        private bool WaypointReached()
        {
            if (Vector2.Distance(this.SimulationState.Position, this.currentWaypoint) < (float)this.Bounds.Width / 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}