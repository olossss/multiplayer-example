// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameMessageTypes.cs" company="">
//   
// </copyright>
// <summary>
//   The game message types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiplayerGame.Networking.Messages
{
    /// <summary>
    /// The game message types.
    /// </summary>
    public enum GameMessageTypes
    {
        /// <summary>
        /// The update asteroid state.
        /// </summary>
        UpdateAsteroidState, 

        /// <summary>
        /// The update player state.
        /// </summary>
        UpdatePlayerState, 

        /// <summary>
        /// The shot fired.
        /// </summary>
        ShotFired
    }
}