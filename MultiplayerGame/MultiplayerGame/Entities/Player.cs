// -----------------------------------------------------------------------
// <copyright file="Player.cs" company="Microsoft">
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
    public class Player : Sprite
    {
        private int livesRemaining = 3;

        public int LivesRemaining
        {
            get
            {
                return this.livesRemaining;
            }

            set
            {
                this.livesRemaining = value;
            }
        }

        public bool IsDestroyed { get; set; }

        private bool isInvulnerable;
        public bool IsInvulnerable
        {
            get { return isInvulnerable; }
            set
            {
                if (this.isInvulnerable == value)
                {
                    return;
                }

                isInvulnerable = value;
                if (this.isInvulnerable)
                {
                    this.invulnerableTimer.Reset();
                }
            }
        }

        public int Score { get; set; }

        private readonly GameTimer invulnerableTimer = new GameTimer();

        #region Constructors and Destructors

        internal Player(
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.TintColor = (this.IsInvulnerable) ? Color.DarkSlateGray : Color.White;

            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (this.IsDestroyed)
            {
                this.LivesRemaining--;

                if (this.LivesRemaining > 0)
                {
                    this.IsDestroyed = false;
                    this.IsInvulnerable = true;
                }
            }

            if (this.IsInvulnerable && this.invulnerableTimer.Stopwatch(1000))
            {
                this.IsInvulnerable = false;
            }

            if (!this.IsDestroyed)
            {
                base.Update(gameTime);
            }
        }
    }
}
