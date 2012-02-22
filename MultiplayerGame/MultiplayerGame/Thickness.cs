// -----------------------------------------------------------------------
// <copyright file="Thickness.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace MultiplayerGame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Thickness
    {
        #region Constructors and Destructors

        public Thickness(int uniformLength)
        {
            this.Left = uniformLength;
            this.Top = uniformLength;
            this.Right = uniformLength;
            this.Bottom = uniformLength;
        }

        public Thickness(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        #endregion

        #region Properties

        public int Bottom { get; set; }

        public int Left { get; set; }

        public int Right { get; set; }

        public int Top { get; set; }

        #endregion
    }
}
