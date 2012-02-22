// -----------------------------------------------------------------------
// <copyright file="NullRandomNumberGenerator.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace MultiplayerGame.RandomNumbers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public sealed class NullRandomNumberGenerator : IRandomNumberGenerator
    {
        #region Constants and Fields

        private static readonly NullRandomNumberGenerator instance = new NullRandomNumberGenerator();

        #endregion

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit

        #region Constructors and Destructors

        static NullRandomNumberGenerator()
        {
        }

        private NullRandomNumberGenerator()
        {
        }

        #endregion

        #region Properties

        public static NullRandomNumberGenerator Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IRandomNumberGenerator

        public int Next(int maxValue)
        {
            return maxValue;
        }

        public int Next(int minValue, int maxValue)
        {
            return maxValue;
        }

        #endregion

        #endregion
    }
}
