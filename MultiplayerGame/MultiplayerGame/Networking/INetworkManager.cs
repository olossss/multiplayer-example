namespace MultiplayerGame.Networking
{
    using System;

    using Lidgren.Network;

    public interface INetworkManager : IDisposable
    {
        void Connect();

        void Disconnect();

        NetIncomingMessage ReadMessage();

        void Recycle(NetIncomingMessage im);

        NetOutgoingMessage CreateMessage();
    }
}