// -----------------------------------------------------------------------
// <copyright file="GameTimer.cs" company="Microsoft">
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
    public class GameTimer
    {
        #region Constants and Fields

        private long stopwatchStart;

        #endregion

        #region Constructors and Destructors

        public GameTimer()
        {
            this.Reset();
        }

        #endregion

        #region Public Methods

        public bool Stopwatch(int ms)
        {
            if (this.TimeGetTime() > this.stopwatchStart + ms)
            {
                this.Reset();
                return true;
            }

            return false;
        }

        #endregion

        #region Methods

        public void Reset()
        {
            this.stopwatchStart = this.TimeGetTime();
        }

        private long TimeGetTime()
        {
            return DateTime.Now.Ticks / 10000; //convert ticks to milliseconds. 10,000 ticks in 1 millisecond.
        }

        #endregion
    }
}
