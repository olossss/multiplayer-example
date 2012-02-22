using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MultiplayerGame;

namespace Client
{
    using MultiplayerGame.Networking;

    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(1000);

            using (var game = new ExampleGame(new ClientNetworkManager()))
            {
                game.Run();
            }
        }
    }
}
