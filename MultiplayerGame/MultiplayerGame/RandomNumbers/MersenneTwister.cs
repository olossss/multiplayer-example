// -----------------------------------------------------------------------
// <copyright file="MersenneTwister.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace MultiplayerGame.RandomNumbers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Class MersenneTwister generates random numbers from a uniform distribution using
    /// the Mersenne Twister algorithm.
    /// </summary>
    /// <remarks>Caution: MT is for MonteCarlo, and is NOT SECURE for CRYPTOGRAPHY 
    /// as it is.</remarks>
    public class MersenneTwister : IRandomNumberGenerator
    {
        // Period parameters.

        #region Constants and Fields

        private const uint LOWER_MASK = 0x7fffffffU; // least significant r bits

        private const int M = 397;

        private const uint MATRIX_A = 0x9908b0dfU; // constant vector a

        /*
                private const int MAX_RAND_INT = 0x7fffffff;
        */

        private const int N = 624;

        private const uint UPPER_MASK = 0x80000000U; // most significant w-r bits

        // mag01[x] = x * MATRIX_A  for x=0,1
        private readonly uint[] mag01 = { 0x0U, MATRIX_A };

        // the array for the state vector
        private readonly uint[] mt = new uint[N];

        // mti==N+1 means mt[N] is not initialized
        private int mti = N + 1;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Creates a random number generator using the time of day in milliseconds as
        /// the seed.
        /// </summary>
        public MersenneTwister()
        {
            this.init_genrand((uint)DateTime.Now.Millisecond);
        }

        /// <summary>
        /// Creates a random number generator initialized with the given seed. 
        /// </summary>
        /// <param name="seed">The seed.</param>
        public MersenneTwister(int seed)
        {
            this.init_genrand((uint)seed);
        }

        /// <summary>
        /// Creates a random number generator initialized with the given array.
        /// </summary>
        /// <param name="init">The array for initializing keys.</param>
        public MersenneTwister(int[] init)
        {
            var initArray = new uint[init.Length];
            for (int i = 0; i < init.Length; ++i)
            {
                initArray[i] = (uint)init[i];
            }

            this.init_by_array(initArray, (uint)initArray.Length);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the maximum random integer value. All random integers generated
        /// by instances of this class are less than or equal to this value. This
        /// value is <c>0x7fffffff</c> (<c>2,147,483,647</c>).
        /// </summary>
        public static int MaxRandomInt
        {
            get
            {
                return 0x7fffffff;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reinitializes the random number generator using the time of day in
        /// milliseconds as the seed.
        /// </summary>
        public void Initialize()
        {
            this.init_genrand((uint)DateTime.Now.Millisecond);
        }

        /// <summary>
        /// Reinitializes the random number generator with the given seed.
        /// </summary>
        /// <param name="seed">The seed.</param>
        public void Initialize(int seed)
        {
            this.init_genrand((uint)seed);
        }

        /// <summary>
        /// Reinitializes the random number generator with the given array.
        /// </summary>
        /// <param name="init">The array for initializing keys.</param>
        public void Initialize(int[] init)
        {
            var initArray = new uint[init.Length];
            for (int i = 0; i < init.Length; ++i)
            {
                initArray[i] = (uint)init[i];
            }

            this.init_by_array(initArray, (uint)initArray.Length);
        }

        /// <summary>
        /// Returns a random integer greater than or equal to zero and
        /// less than or equal to <c>MaxRandomInt</c>. 
        /// </summary>
        /// <returns>The next random integer.</returns>
        public int Next()
        {
            return this.genrand_int31();
        }

        /// <summary>
        /// Generates a random number on <c>[0,1)</c> with 53-bit resolution.
        /// </summary>
        /// <returns>A random number on <c>[0,1)</c> with 53-bit resolution</returns>
        public double Next53BitRes()
        {
            return this.genrand_res53();
        }

        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// </summary>
        /// <returns>A double-precision floating point number greater than or equal to 0.0, 
        /// and less than 1.0.</returns>
        public double NextDouble()
        {
            return this.genrand_real2();
        }

        /// <summary>
        /// Returns a random number greater than or equal to zero, and either strictly
        /// less than one, or less than or equal to one, depending on the value of the
        /// given boolean parameter.
        /// </summary>
        /// <param name="includeOne">
        /// If <c>true</c>, the random number returned will be 
        /// less than or equal to one; otherwise, the random number returned will
        /// be strictly less than one.
        /// </param>
        /// <returns>
        /// If <c>includeOne</c> is <c>true</c>, this method returns a
        /// single-precision random number greater than or equal to zero, and less
        /// than or equal to one. If <c>includeOne</c> is <c>false</c>, this method
        /// returns a single-precision random number greater than or equal to zero and
        /// strictly less than one.
        /// </returns>
        public double NextDouble(bool includeOne)
        {
            if (includeOne)
            {
                return this.genrand_real1();
            }
            return this.genrand_real2();
        }

        /// <summary>
        /// Returns a random number greater than 0.0 and less than 1.0.
        /// </summary>
        /// <returns>A random number greater than 0.0 and less than 1.0.</returns>
        public double NextDoublePositive()
        {
            return this.genrand_real3();
        }

        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// </summary>
        /// <returns>A single-precision floating point number greater than or equal to 0.0, 
        /// and less than 1.0.</returns>
        public float NextFloat()
        {
            return (float)this.genrand_real2();
        }

        /// <summary>
        /// Returns a random number greater than or equal to zero, and either strictly
        /// less than one, or less than or equal to one, depending on the value of the
        /// given boolean parameter.
        /// </summary>
        /// <param name="includeOne">
        /// If <c>true</c>, the random number returned will be 
        /// less than or equal to one; otherwise, the random number returned will
        /// be strictly less than one.
        /// </param>
        /// <returns>
        /// If <c>includeOne</c> is <c>true</c>, this method returns a
        /// single-precision random number greater than or equal to zero, and less
        /// than or equal to one. If <c>includeOne</c> is <c>false</c>, this method
        /// returns a single-precision random number greater than or equal to zero and
        /// strictly less than one.
        /// </returns>
        public float NextFloat(bool includeOne)
        {
            if (includeOne)
            {
                return (float)this.genrand_real1();
            }
            return (float)this.genrand_real2();
        }

        /// <summary>
        /// Returns a random number greater than 0.0 and less than 1.0.
        /// </summary>
        /// <returns>A random number greater than 0.0 and less than 1.0.</returns>
        public float NextFloatPositive()
        {
            return (float)this.genrand_real3();
        }

        #endregion

        #region Implemented Interfaces

        #region IRandomNumberGenerator

        /// <summary>
        /// Returns a positive random integer less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">The maximum value. Must be greater than zero.</param>
        /// <returns>A positive random integer less than or equal to <c>maxValue</c>.</returns>
        public int Next(int maxValue)
        {
            return this.Next(0, maxValue);
        }

        /// <summary>
        /// Returns a random integer within the specified range.
        /// </summary>
        /// <param name="minValue">The lower bound.</param>
        /// <param name="maxValue">The upper bound.</param>
        /// <returns>A random integer greater than or equal to <c>minValue</c>, and less than
        /// or equal to <c>maxValue</c>.</returns>
        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                int tmp = maxValue;
                maxValue = minValue;
                minValue = tmp;
            }

            return (int)(Math.Floor((maxValue - minValue + 1) * this.genrand_real1() + minValue));
        }

        #endregion

        #endregion

        // generates a random number on [0,0x7fffffff]-interval

        #region Methods

        private int genrand_int31()
        {
            return (int)(this.genrand_int32() >> 1);
        }

        private uint genrand_int32()
        {
            uint y;
            if (this.mti >= N)
            {
                /* generate N words at one time */
                int kk;

                if (this.mti == N + 1) /* if init_genrand() has not been called, */
                {
                    this.init_genrand(5489U); /* a default initial seed is used */
                }

                for (kk = 0; kk < N - M; kk++)
                {
                    y = (this.mt[kk] & UPPER_MASK) | (this.mt[kk + 1] & LOWER_MASK);
                    this.mt[kk] = this.mt[kk + M] ^ (y >> 1) ^ this.mag01[y & 0x1U];
                }
                for (; kk < N - 1; kk++)
                {
                    y = (this.mt[kk] & UPPER_MASK) | (this.mt[kk + 1] & LOWER_MASK);
                    this.mt[kk] = this.mt[kk + (M - N)] ^ (y >> 1) ^ this.mag01[y & 0x1U];
                }
                y = (this.mt[N - 1] & UPPER_MASK) | (this.mt[0] & LOWER_MASK);
                this.mt[N - 1] = this.mt[M - 1] ^ (y >> 1) ^ this.mag01[y & 0x1U];

                this.mti = 0;
            }

            y = this.mt[this.mti++];

            // Tempering
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680U;
            y ^= (y << 15) & 0xefc60000U;
            y ^= (y >> 18);

            return y;
        }

        // generates a random number on [0,1]-real-interval
        private double genrand_real1()
        {
            return this.genrand_int32() * (1.0 / 4294967295.0);
            // divided by 2^32-1
        }

        // generates a random number on [0,1)-real-interval
        private double genrand_real2()
        {
            return this.genrand_int32() * (1.0 / 4294967296.0);
            // divided by 2^32
        }

        // generates a random number on (0,1)-real-interval
        private double genrand_real3()
        {
            return ((this.genrand_int32()) + 0.5) * (1.0 / 4294967296.0);
            // divided by 2^32
        }

        // generates a random number on [0,1) with 53-bit resolution
        private double genrand_res53()
        {
            uint a = this.genrand_int32() >> 5, b = this.genrand_int32() >> 6;
            return (a * 67108864.0 + b) * (1.0 / 9007199254740992.0);
        }

        private void init_by_array(uint[] init_key, uint key_length)
        {
            this.init_genrand(19650218U);
            int i = 1;
            int j = 0;
            var k = (int)(N > key_length ? N : key_length);
            for (; k > 0; k--)
            {
                this.mt[i] =
                    (uint)((this.mt[i] ^ ((this.mt[i - 1] ^ (this.mt[i - 1] >> 30)) * 1664525U)) + init_key[j] + j);
                /* non linear */
                this.mt[i] &= 0xffffffffU; // for WORDSIZE > 32 machines
                i++;
                j++;
                if (i >= N)
                {
                    this.mt[0] = this.mt[N - 1];
                    i = 1;
                }
                if (j >= key_length)
                {
                    j = 0;
                }
            }
            for (k = N - 1; k > 0; k--)
            {
                this.mt[i] = (uint)((this.mt[i] ^ ((this.mt[i - 1] ^ (this.mt[i - 1] >> 30)) * 1566083941U)) - i);
                /* non linear */
                this.mt[i] &= 0xffffffffU; // for WORDSIZE > 32 machines
                i++;
                if (i >= N)
                {
                    this.mt[0] = this.mt[N - 1];
                    i = 1;
                }
            }

            this.mt[0] = 0x80000000U; // MSB is 1; assuring non-zero initial array
        }

        private void init_genrand(uint s)
        {
            this.mt[0] = s & 0xffffffffU;
            for (this.mti = 1; this.mti < N; this.mti++)
            {
                this.mt[this.mti] =
                    (uint)(1812433253U * (this.mt[this.mti - 1] ^ (this.mt[this.mti - 1] >> 30)) + this.mti);
                // See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier. 
                // In the previous versions, MSBs of the seed affect   
                // only MSBs of the array mt[].                        
                // 2002/01/09 modified by Makoto Matsumoto             
                this.mt[this.mti] &= 0xffffffffU;
                // for >32 bit machines
            }
        }

        #endregion

        // These real versions are due to Isaku Wada, 2002/01/09 added
    }
}
