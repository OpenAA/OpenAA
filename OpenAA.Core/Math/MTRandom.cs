namespace OpenAA.Math
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Mersenne Twister 乱数生成器
    /// PHPのmt_rand関数に相当する。
    /// </summary>
    public class MTRandom
        : Random
    {
        private const Int32 N = 624;

        private const Int32 M = 397;

        /// <summary>
        /// Constant vector A.
        /// </summary>
        private const UInt32 MATRIX_A = 0x9908b0df;

        /// <summary>
        /// Most significant w-r bits.
        /// </summary>
        private const UInt32 UPPER_MASK = 0x80000000U;

        /// <summary>
        /// Least significant r bits.
        /// </summary>
        private const UInt32 LOWER_MASK = 0x7fffffffU;

        /// <summary>
        /// The array for the state vector.
        /// </summary>
        private UInt32[] _mt;

        /// <summary>
        /// mti==N+1 means mt[N] is not initialized.
        /// </summary>
        private Int16 _mti;

        public MTRandom()
            : this(unchecked((UInt32)(DateTime.Now.Ticks + Environment.TickCount)))
        {
        }

        /// <summary>
        /// Initializes mt[N] with a seed.
        /// </summary>
        /// <param name="seed">The seed value.</param>
        public MTRandom(UInt32 seed)
        {
            this._mt = new UInt32[N];
            this._mti = N + 1;

            this._mt[0] = seed & 0xffffffffU;

            // See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier.
            // In the previous versions, MSBs of the seed affect
            // only MSBs of the array mt[].
            // 2002/01/09 modified by Makoto Matsumoto
            for (this._mti = 1; this._mti < N; ++this._mti)
            {
                // The algorithm looks to overflow in below expression.
                unchecked
                {
                    this._mt[this._mti]
                        = 1812433253U * (this._mt[this._mti - 1] ^ (this._mt[this._mti - 1] >> 30)) + (UInt32)this._mti;
                }
                // for >32 bit machines
                this._mt[this._mti] &= 0xffffffffU;
            }
        }

        public MTRandom(UInt32[] initKey)
            : this(initKey, initKey.Length)
        {
        }

        /// <summary>
        /// Initialize by an array with array-length.
        /// </summary>
        /// <param name="initKey">The array for initializing keys.</param>
        /// <param name="length">The length of initKey.</param>
        public MTRandom(UInt32[] initKey, Int32 length)
            : this(19650218U)
        {
            Int16 i = 1;
            Int16 j = 0;
            Int16 k = (Int16)(N > length ? N : length);

            for (; k != 0; k--)
            {
                // non linear
                this._mt[i] =
                    unchecked((this._mt[i] ^ ((this._mt[i - 1] ^ (this._mt[i - 1] >> 30)) * 1664525U)) + (UInt32)(initKey[j] + j));
                // for WORDSIZE > 32 machines
                this._mt[i] &= 0xffffffffU;
                i++;
                j++;
                if (i >= N)
                {
                    this._mt[0] = this._mt[N - 1];
                    i = 1;
                }
                if (j >= length)
                {
                    j = 0;
                }
            }

            for (k = N - 1; k != 0; k--)
            {
                this._mt[i] =
                    unchecked((this._mt[i] ^ ((this._mt[i - 1] ^ (this._mt[i - 1] >> 30)) * 1566083941U)) - (UInt32)i);
                // for WORDSIZE > 32 machines
                this._mt[i] &= 0xffffffffU;
                i++;
                if (i >= N)
                {
                    this._mt[0] = this._mt[N - 1];
                    i = 1;
                }
            }

            /* MSB is 1; assuring non-zero initial array */
            this._mt[0] = 0x80000000U;
        }

        /// <summary>
        /// Generates a random number on [0,0xffffffff]-interval
        /// </summary>
        /// <returns>Generated value.</returns>
        protected UInt32 GenerateUInt32()
        {
            UInt32 y;
            /* mag01[x] = x * MATRIX_A  for x=0,1 */
            UInt32[] mag01 = new UInt32[] { 0x0U, MATRIX_A, };

            if (this._mti >= N)
            {
                /* generate N words at one time */
                Int32 kk = 0;

                for (; kk < N - M; ++kk)
                {
                    y = (this._mt[kk] & UPPER_MASK) | (this._mt[kk + 1] & LOWER_MASK);
                    this._mt[kk] = this._mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1U];
                }
                for (; kk < N - 1; ++kk)
                {
                    y = (this._mt[kk] & UPPER_MASK) | (this._mt[kk + 1] & LOWER_MASK);
                    this._mt[kk] = this._mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1U];
                }
                y = (this._mt[N - 1] & UPPER_MASK) | (this._mt[0] & LOWER_MASK);
                this._mt[N - 1] = this._mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1U];
                this._mti = 0;
            }

            // Tempering
            y = _mt[_mti++];
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680U;
            y ^= (y << 15) & 0xefc60000U;
            y ^= (y >> 18);

            return y;
        }

        protected override Double Sample()
        {
            return this.NextDouble(false, true, false);
        }

        #region Next
        public override Int32 Next()
        {
            return this.NextInt32();
        }

        public override Int32 Next(Int32 maxValue)
        {
            return this.NextInt32(maxValue);
        }

        public override Int32 Next(Int32 minValue, Int32 maxValue)
        {
            return this.NextInt32(minValue, maxValue);
        }
        #endregion

        #region NextByte
        public Byte NextByte()
        {
            return this.NextByte(Byte.MaxValue);
        }

        public Byte NextByte(Byte maxValue)
        {
            if (maxValue <= 1)
            {
                if (maxValue < 0)
                {
                    throw new ArgumentOutOfRangeException("maxValue");
                }
                return 0;
            }

            return (Byte)(this.NextDouble() * maxValue);
        }

        public Byte NextByte(Byte minValue, Byte maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException("minValue");
            }
            else if (minValue == maxValue)
            {
                return minValue;
            }
            else
            {
                return (Byte)(this.NextDouble(false, true, false) * ((Double)maxValue - minValue) + minValue);
            }
        }

        public override void NextBytes(Byte[] buffer)
        {
            this.NextBytes(buffer, buffer.Length);
        }

        public virtual void NextBytes(Byte[] buffer, Int32 length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            for (Int32 idx = 0; idx < length; ++idx)
            {
                buffer[idx] = this.NextByte();
            }
        }

        public IEnumerable<Byte> NextBytes()
        {
            while (true)
            {
                yield return this.NextByte();
            }
        }

        public IEnumerable<Byte> NextBytes(Byte maxValue)
        {
            while (true)
            {
                yield return this.NextByte(maxValue);
            }
        }

        public IEnumerable<Byte> NextBytes(Byte minValue, Byte maxValue)
        {
            while (true)
            {
                yield return this.NextByte(minValue, maxValue);
            }
        }
        #endregion

        #region NextSByte
        public SByte NextSByte()
        {
            return this.NextSByte(SByte.MaxValue);
        }

        public SByte NextSByte(SByte maxValue)
        {
            if (maxValue <= 1)
            {
                if (maxValue < 0)
                {
                    throw new ArgumentOutOfRangeException("maxValue");
                }
                return 0;
            }

            return (SByte)(this.NextDouble() * maxValue);
        }

        public SByte NextSByte(SByte minValue, SByte maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException("minValue");
            }
            else if (minValue == maxValue)
            {
                return minValue;
            }
            else
            {
                return (SByte)(this.NextDouble(false, true, false) * ((Double)maxValue - minValue) + minValue);
            }
        }

        public void NextSBytes(SByte[] buffer)
        {
            this.NextSBytes(buffer, buffer.Length);
        }

        public virtual void NextSBytes(SByte[] buffer, Int32 length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            for (Int32 idx = 0; idx < length; ++idx)
            {
                buffer[idx] = this.NextSByte(SByte.MinValue, SByte.MaxValue);
            }
        }

        public IEnumerable<SByte> NextSBytes()
        {
            while (true)
            {
                yield return this.NextSByte();
            }
        }

        public IEnumerable<SByte> NextSBytes(SByte maxValue)
        {
            while (true)
            {
                yield return this.NextSByte(maxValue);
            }
        }

        public IEnumerable<SByte> NextSBytes(SByte minValue, SByte maxValue)
        {
            while (true)
            {
                yield return this.NextSByte(minValue, maxValue);
            }
        }
        #endregion

        #region NextChar
        public Char NextChar()
        {
            return this.NextChar(Char.MaxValue);
        }

        public Char NextChar(Char maxValue)
        {
            if (maxValue <= 1)
            {
                if (maxValue < 0)
                {
                    throw new ArgumentOutOfRangeException("maxValue");
                }
                return (Char)0;
            }

            return (Char)(this.NextDouble() * maxValue);
        }

        public Char NextChar(Char minValue, Char maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException("minValue");
            }
            else if (minValue == maxValue)
            {
                return minValue;
            }
            else
            {
                return (Char)(this.NextDouble(false, true, false) * ((Double)maxValue - minValue) + minValue);
            }
        }

        public void NextChars(Char[] buffer)
        {
            this.NextChars(buffer, buffer.Length);
        }

        public virtual void NextChars(Char[] buffer, Int32 length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            for (Int32 idx = 0; idx < length; ++idx)
            {
                buffer[idx] = this.NextChar();
            }
        }

        public IEnumerable<Char> NextChars()
        {
            while (true)
            {
                yield return this.NextChar();
            }
        }

        public IEnumerable<Char> NextChars(Char maxValue)
        {
            while (true)
            {
                yield return this.NextChar(maxValue);
            }
        }

        public IEnumerable<Char> NextChars(Char minValue, Char maxValue)
        {
            while (true)
            {
                yield return this.NextChar(minValue, maxValue);
            }
        }
        #endregion

        #region NextInt16
        public Int16 NextInt16()
        {
            return this.NextInt16(Int16.MaxValue);
        }

        public Int16 NextInt16(Int16 maxValue)
        {
            if (maxValue <= 1)
            {
                if (maxValue < 0)
                {
                    throw new ArgumentOutOfRangeException("maxValue");
                }
                return 0;
            }

            return (Int16)(this.NextDouble() * maxValue);
        }

        public Int16 NextInt16(Int16 minValue, Int16 maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException("minValue");
            }
            else if (minValue == maxValue)
            {
                return minValue;
            }
            else
            {
                return (Int16)(this.NextDouble(false, true, false) * ((Double)maxValue - minValue) + minValue);
            }
        }

        public IEnumerable<Int16> NextInt16s()
        {
            while (true)
            {
                yield return this.NextInt16();
            }
        }

        public IEnumerable<Int16> NextInt16s(Int16 maxValue)
        {
            while (true)
            {
                yield return this.NextInt16(maxValue);
            }
        }

        public IEnumerable<Int16> NextInt16s(Int16 minValue, Int16 maxValue)
        {
            while (true)
            {
                yield return this.NextInt16(minValue, maxValue);
            }
        }
        #endregion

        #region NextUInt16
        public UInt16 NextUInt16()
        {
            return this.NextUInt16(UInt16.MaxValue);
        }

        public UInt16 NextUInt16(UInt16 maxValue)
        {
            if (maxValue <= 1)
            {
                if (maxValue < 0)
                {
                    throw new ArgumentOutOfRangeException("maxValue");
                }
                return 0;
            }

            return (UInt16)(this.NextDouble() * maxValue);
        }

        public UInt16 NextUInt16(UInt16 minValue, UInt16 maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException("minValue");
            }
            else if (minValue == maxValue)
            {
                return minValue;
            }
            else
            {
                return (UInt16)(this.NextDouble(false, true, false) * ((Double)maxValue - minValue) + minValue);
            }
        }

        public IEnumerable<UInt16> NextUInt16s()
        {
            while (true)
            {
                yield return this.NextUInt16();
            }
        }

        public IEnumerable<UInt16> NextUInt16s(UInt16 maxValue)
        {
            while (true)
            {
                yield return this.NextUInt16(maxValue);
            }
        }

        public IEnumerable<UInt16> NextUInt16s(UInt16 minValue, UInt16 maxValue)
        {
            while (true)
            {
                yield return this.NextUInt16(minValue, maxValue);
            }
        }
        #endregion

        #region NextInt32
        public Int32 NextInt32()
        {
            return this.NextInt32(Int32.MaxValue);
        }

        public Int32 NextInt32(Int32 maxValue)
        {
            if (maxValue <= 1)
            {
                if (maxValue < 0)
                {
                    throw new ArgumentOutOfRangeException("maxValue");
                }
                return 0;
            }

            return (Int32)(this.NextDouble() * maxValue);
        }

        public Int32 NextInt32(Int32 minValue, Int32 maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException("minValue");
            }
            else if (minValue == maxValue)
            {
                return minValue;
            }
            else
            {
                return (Int32)(this.NextDouble(false, true, false) * ((Double)maxValue - minValue) + minValue);
            }
        }

        public IEnumerable<Int32> NextInt32s()
        {
            while (true)
            {
                yield return this.NextInt32();
            }
        }

        public IEnumerable<Int32> NextInt32s(Int32 maxValue)
        {
            while (true)
            {
                yield return this.NextInt32(maxValue);
            }
        }

        public IEnumerable<Int32> NextInt32s(Int32 minValue, Int32 maxValue)
        {
            while (true)
            {
                yield return this.NextInt32(minValue, maxValue);
            }
        }
        #endregion

        #region NextUInt32
        public UInt32 NextUInt32()
        {
            return this.NextUInt32(UInt32.MaxValue);
        }

        public UInt32 NextUInt32(UInt32 maxValue)
        {
            if (maxValue <= 1)
            {
                if (maxValue < 0)
                {
                    throw new ArgumentOutOfRangeException("maxValue");
                }
                return 0;
            }

            return (UInt32)(this.NextDouble() * maxValue);
        }

        public UInt32 NextUInt32(UInt32 minValue, UInt32 maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException("minValue");
            }
            else if (minValue == maxValue)
            {
                return minValue;
            }
            else
            {
                return (UInt32)(this.NextDouble(false, true, false) * ((Double)maxValue - minValue) + minValue);
            }
        }

        public IEnumerable<UInt32> NextUInt32s()
        {
            while (true)
            {
                yield return this.NextUInt32();
            }
        }

        public IEnumerable<UInt32> NextUInt32s(UInt32 maxValue)
        {
            while (true)
            {
                yield return this.NextUInt32(maxValue);
            }
        }

        public IEnumerable<UInt32> NextUInt32s(UInt32 minValue, UInt32 maxValue)
        {
            while (true)
            {
                yield return this.NextUInt32(minValue, maxValue);
            }
        }
        #endregion

        #region NextInt64
        public Int64 NextInt64()
        {
            return this.NextInt64(Int64.MaxValue);
        }

        public Int64 NextInt64(Int64 maxValue)
        {
            if (maxValue <= 1)
            {
                if (maxValue < 0)
                {
                    throw new ArgumentOutOfRangeException("maxValue");
                }
                return 0;
            }

            return (Int64)(this.NextDouble() * maxValue);
        }

        public Int64 NextInt64(Int64 minValue, Int64 maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException("minValue");
            }
            else if (minValue == maxValue)
            {
                return minValue;
            }
            else
            {
                return (Int64)(this.NextDouble(false, true, false) * ((Double)maxValue - minValue) + minValue);
            }
        }

        public IEnumerable<Int64> NextInt64s()
        {
            while (true)
            {
                yield return this.NextInt64();
            }
        }

        public IEnumerable<Int64> NextInt64s(Int64 maxValue)
        {
            while (true)
            {
                yield return this.NextInt64(maxValue);
            }
        }

        public IEnumerable<Int64> NextInt64s(Int64 minValue, Int64 maxValue)
        {
            while (true)
            {
                yield return this.NextInt64(minValue, maxValue);
            }
        }
        #endregion

        #region NextUInt64
        public UInt64 NextUInt64()
        {
            return this.NextUInt64(UInt64.MaxValue);
        }

        public UInt64 NextUInt64(UInt64 maxValue)
        {
            if (maxValue <= 1)
            {
                if (maxValue < 0)
                {
                    throw new ArgumentOutOfRangeException("maxValue");
                }
                return 0;
            }

            return (UInt64)(this.NextDouble() * maxValue);
        }

        public UInt64 NextUInt64(UInt64 minValue, UInt64 maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException("minValue");
            }
            else if (minValue == maxValue)
            {
                return minValue;
            }
            else
            {
                return (UInt64)(this.NextDouble(false, true, false) * ((Double)maxValue - minValue) + minValue);
            }
        }

        public IEnumerable<UInt64> NextUInt64s()
        {
            while (true)
            {
                yield return this.NextUInt64();
            }
        }

        public IEnumerable<UInt64> NextUInt64s(UInt64 maxValue)
        {
            while (true)
            {
                yield return this.NextUInt64(maxValue);
            }
        }

        public IEnumerable<UInt64> NextUInt64s(UInt64 minValue, UInt64 maxValue)
        {
            while (true)
            {
                yield return this.NextUInt64(minValue, maxValue);
            }
        }
        #endregion

        #region NextDecimal
        public Decimal NextDecimal()
        {
            return this.NextDecimal(Decimal.MaxValue);
        }

        public Decimal NextDecimal(Decimal maxValue)
        {
            if (maxValue <= 1)
            {
                if (maxValue < 0)
                {
                    throw new ArgumentOutOfRangeException("maxValue");
                }
                return 0;
            }

            return (Decimal)((Decimal)this.NextDouble() * maxValue);
        }

        public Decimal NextDecimal(Decimal minValue, Decimal maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException("minValue");
            }
            else if (minValue == maxValue)
            {
                return minValue;
            }
            else
            {
                return (Decimal)((Decimal)this.NextDouble(false, true, false) * (maxValue - minValue) + minValue);
            }
        }

        public IEnumerable<Decimal> NextDecimals()
        {
            while (true)
            {
                yield return this.NextDecimal();
            }
        }

        public IEnumerable<Decimal> NextDecimals(Decimal maxValue)
        {
            while (true)
            {
                yield return this.NextDecimal(maxValue);
            }
        }

        public IEnumerable<Decimal> NextDecimals(Decimal minValue, Decimal maxValue)
        {
            while (true)
            {
                yield return this.NextDecimal(minValue, maxValue);
            }
        }
        #endregion

        #region NextSingle
        public Single NextSingle()
        {
            return (Single)this.Sample();
        }

        public virtual Single NextSingle(Boolean isSigned, Boolean isMinClosed, Boolean isMaxClosed)
        {
            return (Single)this.NextDouble(isSigned, isMinClosed, isMaxClosed);
        }

        public IEnumerable<Single> NextSingles()
        {
            while (true)
            {
                yield return this.NextSingle();
            }
        }

        public IEnumerable<Single> NextSingles(Boolean isSigned, Boolean isMinClosed, Boolean isMaxClosed)
        {
            while (true)
            {
                yield return this.NextSingle(isSigned, isMinClosed, isMaxClosed);
            }
        }
        #endregion

        #region NextDouble
        public override Double NextDouble()
        {
            return this.Sample();
        }

        public virtual Double NextDouble(Boolean isSigned, Boolean isMinClosed, Boolean isMaxClosed)
        {
            UInt64 n = this.GenerateUInt32();
            UInt64 m = isSigned ? (UInt64)Int32.MaxValue : (UInt64)UInt32.MaxValue;
            if (!isMinClosed)
            {
                n++;
            }
            if (!isMaxClosed)
            {
                m++;
            }

            unchecked
            {
                if (isSigned)
                {
                    return (Double)((Int32)n) / m;
                }
                else
                {
                    return (Double)n / m;
                }
            }
        }

        public IEnumerable<Double> NextDoubles()
        {
            while (true)
            {
                yield return this.NextDouble();
            }
        }

        public IEnumerable<Double> NextDoubles(Boolean isSigned, Boolean isMinClosed, Boolean isMaxClosed)
        {
            while (true)
            {
                yield return this.NextDouble(isSigned, isMinClosed, isMaxClosed);
            }
        }
        #endregion
    }

}