// -----------------------------------------------------------------------
// <copyright file="UpdateAsteroidStateMessage.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using MultiplayerGame.Entities;

namespace MultiplayerGame.Networking.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UpdateAsteroidStateMessage : IGameMessage
    {
         #region Constructors and Destructors

        public UpdateAsteroidStateMessage(NetIncomingMessage im)
        {
            this.Decode(im);
        }

        public UpdateAsteroidStateMessage()
        {
        }

        public UpdateAsteroidStateMessage(Asteroid asteroid)
        {
            this.Id = asteroid.Id;
            this.Position = asteroid.SimulationState.Position;
            this.Velocity = asteroid.SimulationState.Velocity;
            this.Rotation = asteroid.SimulationState.Rotation;
            this.MessageTime = NetTime.Now;
        }

        #endregion

        #region Properties

        public long Id { get; set; }

        public double MessageTime { get; set; }

        public Vector2 Position { get; set; }

        public float Rotation { get; set; }

        public Vector2 Velocity { get; set; }

        #endregion

        #region Public Methods

        public GameMessageTypes MessageType
        {
            get { return GameMessageTypes.UpdateAsteroidState; }
        }

        public void Decode(NetIncomingMessage im)
        {
            this.Id = im.ReadInt64();
            this.MessageTime = im.ReadDouble();
            this.Position = im.ReadVector2();
            this.Velocity = im.ReadVector2();
            this.Rotation = im.ReadSingle();
        }

        public void Encode(NetOutgoingMessage om)
        {
            om.Write(this.Id);
            om.Write(this.MessageTime);
            om.Write(this.Position);
            om.Write(this.Velocity);
            om.Write(this.Rotation);
        }

        #endregion
    }
}
