using Lidgren.Network;

namespace MultiplayerGame.Networking.Messages
{
    public interface IGameMessage
    {
        GameMessageTypes MessageType { get; }

        void Encode(NetOutgoingMessage om);

        void Decode(NetIncomingMessage im);
    }
}