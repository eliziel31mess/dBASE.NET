namespace dBASE.NET.Encoders
{
    using System;
    using System.Text;

    internal class BlobEncoder : IEncoder
    {
        private static BlobEncoder instance;

        private BlobEncoder() { }

        public static BlobEncoder Instance => instance ?? (instance = new BlobEncoder());

        /// <inheritdoc />
        public byte[] Encode(DbfField field, object data, Encoding encoding)
        {
            if (data is byte[] raw)
            {
                // Store the blob length as a 4-byte integer pointer (same as memo)
                return BitConverter.GetBytes(raw.Length);
            }
            return new byte[field.Length];
        }

        /// <inheritdoc />
        public object Decode(byte[] buffer, byte[] memoData, Encoding encoding)
        {
            if (memoData == null || memoData.Length == 0) return null;

            int index;
            if (buffer.Length > 4)
            {
                string text = encoding.GetString(buffer).Trim();
                if (text.Length == 0) return null;
                index = Convert.ToInt32(text);
            }
            else
            {
                index = BitConverter.ToInt32(buffer, 0);
                if (index == 0) return null;
            }

            return ReadBlobBytes(index, memoData);
        }

        private static byte[] ReadBlobBytes(int index, byte[] memoData)
        {
            UInt16 blockSize = BitConverter.ToUInt16(new[] { memoData[7], memoData[6] }, 0);
            int length = (int)BitConverter.ToUInt32(
                new[]
                {
                    memoData[index * blockSize + 4 + 3],
                    memoData[index * blockSize + 4 + 2],
                    memoData[index * blockSize + 4 + 1],
                    memoData[index * blockSize + 4 + 0],
                },
                0);

            byte[] blobBytes = new byte[length];
            int offset = index * blockSize + 8;
            Array.Copy(memoData, offset, blobBytes, 0, length);
            return blobBytes;
        }
    }
}
