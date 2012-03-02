// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnemySpawnedMessage.cs" company="">
//   
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiplayerGame.Networking.Messages
{
    using Lidgren.Network;
    using Lidgren.Network.Xna;

    using Microsoft.Xna.Framework;

    using MultiplayerGame.Entities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EnemySpawnedMessage : IGameMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemySpawnedMessage"/> class.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        public EnemySpawnedMessage(NetIncomingMessage im)
        {
            this.Decode(im);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemySpawnedMessage"/> class.
        /// </summary>
        /// <param name="enemy">
        /// The enemy.
        /// </param>
        public EnemySpawnedMessage(Enemy enemy)
        {
            this.Id = enemy.Id;
            this.Position = enemy.SimulationState.Position;
            this.Velocity = enemy.SimulationState.Velocity;
            this.Rotation = enemy.SimulationState.Rotation;
            this.MessageTime = NetTime.Now;
            this.Path = enemy.Path;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets MessageTime.
        /// </summary>
        public double MessageTime { get; set; }

        /// <summary>
        /// Gets MessageType.
        /// </summary>
        public GameMessageTypes MessageType
        {
            get
            {
                return GameMessageTypes.EnemySpawned;
            }
        }

        /// <summary>
        /// Gets or sets Path.
        /// </summary>
        public int Path { get; set; }

        /// <summary>
        /// Gets or sets Position.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets Rotation.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Gets or sets Velocity.
        /// </summary>
        public Vector2 Velocity { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        public void Decode(NetIncomingMessage im)
        {
            this.Id = im.ReadInt64();
            this.MessageTime = im.ReadDouble();
            this.Position = im.ReadVector2();
            this.Velocity = im.ReadVector2();
            this.Rotation = im.ReadSingle();
            this.Path = im.ReadInt32();
        }

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="om">
        /// The om.
        /// </param>
        public void Encode(NetOutgoingMessage om)
        {
            om.Write(this.Id);
            om.Write(this.MessageTime);
            om.Write(this.Position);
            om.Write(this.Velocity);
            om.Write(this.Rotation);
            om.Write(this.Path);
        }

        #endregion
    }
}