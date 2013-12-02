namespace OpenAA.Security.Checksum
{
    public interface IChecksum
    {
        /// <summary>
        /// Returns the data checksum computed so far.
        /// </summary>
        long Value
        {
            get;
        }

        /// <summary>
        /// Resets the data checksum as if no update was ever called.
        /// </summary>
        void Reset();

        /// <summary>
        /// Adds one byte to the data checksum.
        /// </summary>
        /// <param name = "bval">
        /// the data tryGetValue to add. The high byte of the int is ignored.
        /// </param>
        void Update(int bval);

        /// <summary>
        /// Updates the data checksum with the bytes taken from the array.
        /// </summary>
        /// <param name="array">
        /// array an array of bytes
        /// </param>
        void Update(byte[] buffer);

        /// <summary>
        /// Adds the byte array to the data checksum.
        /// </summary>
        /// <param name = "array">
        /// the array which contains the data
        /// </param>
        /// <param name = "offset">
        /// the offset in the array where the data starts
        /// </param>
        /// <param name = "length">
        /// the length of the data
        /// </param>
        void Update(byte[] buffer, int offset, int length);
    }
}
