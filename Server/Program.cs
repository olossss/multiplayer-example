using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiplayerGame;

namespace Server
{
    using MultiplayerGame.Networking;

    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new ExampleGame(new ServerNetworkManager()))
            {
                game.Run();
            }
        }
    }
}
