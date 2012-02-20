// -----------------------------------------------------------------------
// <copyright file="ServerNetworkManager.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace MultiplayerGame.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Lidgren.Network;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ServerNetworkManager : INetworkManager
    {
        private NetServer netServer;

        private bool isDisposed;

        public void Connect()
        {
            var config = new NetPeerConfiguration("Asteroid")
            {
                Port = Convert.ToInt32("14242"),
                //SimulatedMinimumLatency = 0.2f, 
                //SimulatedLoss = 0.1f 
            };
            config.EnableMessageType(NetIncomingMessageType.WarningMessage);
            config.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            config.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            config.EnableMessageType(NetIncomingMessageType.Error);
            config.EnableMessageType(NetIncomingMessageType.DebugMessage);
            config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);

            netServer = new NetServer(config);
            netServer.Start();
        }

        public void Disconnect()
        {
            netServer.Shutdown("Bye");
        }

        public NetIncomingMessage ReadMessage()
        {
            return this.netServer.ReadMessage();
        }

        public void Recycle(NetIncomingMessage im)
        {
            this.netServer.Recycle(im);
        }

        public NetOutgoingMessage CreateMessage()
        {
            return this.netServer.CreateMessage();
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.Disconnect();
                }

                this.isDisposed = true;
            }
        }
    }
}
