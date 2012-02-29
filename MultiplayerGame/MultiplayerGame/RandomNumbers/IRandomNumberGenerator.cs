// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRandomNumberGenerator.cs" company="">
//   
// </copyright>
// <summary>
//   The i random number generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiplayerGame.RandomNumbers
{
    /// <summary>
    /// The i random number generator.
    /// </summary>
    public interface IRandomNumberGenerator
    {
        #region Public Methods and Operators

        /// <summary>
        /// The next.
        /// </summary>
        /// <param name="maxValue">
        /// The max value.
        /// </param>
        /// <returns>
        /// The next.
        /// </returns>
        int Next(int maxValue);

        /// <summary>
        /// The next.
        /// </summary>
        /// <param name="minValue">
        /// The min value.
        /// </param>
        /// <param name="maxValue">
        /// The max value.
        /// </param>
        /// <returns>
        /// The next.
        /// </returns>
        int Next(int minValue, int maxValue);

        #endregion
    }
}