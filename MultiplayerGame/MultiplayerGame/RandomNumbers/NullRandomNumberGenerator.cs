// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullRandomNumberGenerator.cs" company="">
//   
// </copyright>
// <summary>
//   The null random number generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiplayerGame.RandomNumbers
{
    /// <summary>
    /// The null random number generator.
    /// </summary>
    public sealed class NullRandomNumberGenerator : IRandomNumberGenerator
    {
        #region Constants and Fields

        /// <summary>
        /// The instance.
        /// </summary>
        private static readonly NullRandomNumberGenerator instance = new NullRandomNumberGenerator();

        #endregion

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="NullRandomNumberGenerator"/> class.
        /// </summary>
        static NullRandomNumberGenerator()
        {
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="NullRandomNumberGenerator"/> class from being created.
        /// </summary>
        private NullRandomNumberGenerator()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Instance.
        /// </summary>
        public static NullRandomNumberGenerator Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion

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
        public int Next(int maxValue)
        {
            return maxValue;
        }

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
        public int Next(int minValue, int maxValue)
        {
            return maxValue;
        }

        #endregion
    }
}