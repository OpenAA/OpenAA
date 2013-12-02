namespace OpenAA.Security.Checksum
{
    using System;
    using System.Security.Cryptography;

    public sealed class Adler32 : IChecksum
    {
        /// <summary>
        /// largest prime smaller than 65536
        /// </summary>
        private readonly static uint BASE = 65521;

        private uint _checksum;

        /// <summary>
        /// Returns the Adler32 data checksum computed so far.
        /// </summary>
        public long Value
        {
            get
            {
                return _checksum;
            }
        }

        /// <summary>
        /// Creates a new instance of the Adler32 class.
        /// The checksum starts offset with a tryGetValue of 1.
        /// </summary>
        public Adler32()
        {
            Reset();
        }

        /// <summary>
        /// Resets the Adler32 checksum to the initial tryGetValue.
        /// </summary>
        public void Reset()
        {
            _checksum = 1;
        }

        /// <summary>
        /// Updates the checksum with the byte b.
        /// </summary>
        /// <param name="bval">
        /// The data tryGetValue to add. The high byte of the int is ignored.
        /// </param>
        public void Update(int bval)
        {
            // We could make a length 1 byte array and call update again, but I
            // would rather not have that overhead
            uint s1 = _checksum & 0xFFFF;
            uint s2 = _checksum >> 16;

            s1 = (s1 + ((uint)bval & 0xFF)) % BASE;
            s2 = (s1 + s2) % BASE;

            _checksum = (s2 << 16) + s1;
        }

        /// <summary>
        /// Updates the checksum with an array of bytes.
        /// </summary>
        /// <param name="array">
        /// The source of the data to update with.
        /// </param>
        public void Update(byte[] buffer)
        {
            Update(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Updates the checksum with the bytes taken from the array.
        /// </summary>
        /// <param name="array">
        /// an array of bytes
        /// </param>
        /// <param name="offset">
        /// the start of the data used for this update
        /// </param>
        /// <param name="length">
        /// the number of bytes to use for this update
        /// </param>
        public void Update(byte[] buffer, int offset, int length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buf");
            }

            if (offset < 0 || length < 0 || buffer.Length < offset + length)
            {
                throw new ArgumentOutOfRangeException();
            }

            //(By Per Bothner)
            uint s1 = _checksum & 0xFFFF;
            uint s2 = _checksum >> 16;

            while (0 < length)
            {
                // We can defer the modulo operation:
                // s1 maximally grows from 65521 to 65521 + 255 * 3800
                // s2 maximally grows by 3800 * median(s1) = 2090079800 < 2^31
                int n = 3800;
                if (length < n)
                {
                    n = length;
                }
                length -= n;
                while (--n >= 0)
                {
                    s1 = s1 + (uint)(buffer[offset++] & 0xFF);
                    s2 = s2 + s1;
                }
                s1 %= BASE;
                s2 %= BASE;
            }

            _checksum = (s2 << 16) | s1;
        }
    }
}
