// -----------------------------------------------------------------------
// <copyright file="ClientNetworkManager.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace MultiplayerGame.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;

    using Lidgren.Network;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ClientNetworkManager : INetworkManager
    {
        private bool isDisposed;

        private NetClient netClient;

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

        public void Connect()
        {
            var config = new NetPeerConfiguration("Asteroid")
            {
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

            this.netClient = new NetClient(config);
            this.netClient.Start();

            this.netClient.Connect(new IPEndPoint(NetUtility.Resolve("127.0.0.1"), Convert.ToInt32("14242")));
        }

        public void Disconnect()
        {
            this.netClient.Disconnect("Bye");
        }

        public NetIncomingMessage ReadMessage()
        {
            return this.netClient.ReadMessage();
        }

        public void Recycle(NetIncomingMessage im)
        {
            this.netClient.Recycle(im);
        }

        public NetOutgoingMessage CreateMessage()
        {
            return this.netClient.CreateMessage();
        }
    }
}
