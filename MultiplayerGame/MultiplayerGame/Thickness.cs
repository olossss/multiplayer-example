// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Thickness.cs" company="">
//   
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiplayerGame
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Thickness
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Thickness"/> class.
        /// </summary>
        /// <param name="uniformLength">
        /// The uniform length.
        /// </param>
        public Thickness(int uniformLength)
        {
            this.Left = uniformLength;
            this.Top = uniformLength;
            this.Right = uniformLength;
            this.Bottom = uniformLength;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Thickness"/> class.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="top">
        /// The top.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <param name="bottom">
        /// The bottom.
        /// </param>
        public Thickness(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Bottom.
        /// </summary>
        public int Bottom { get; set; }

        /// <summary>
        /// Gets or sets Left.
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// Gets or sets Right.
        /// </summary>
        public int Right { get; set; }

        /// <summary>
        /// Gets or sets Top.
        /// </summary>
        public int Top { get; set; }

        #endregion
    }
}