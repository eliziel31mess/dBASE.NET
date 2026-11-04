using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dBASE.NET.Encoders
{
    internal class DoubleEncoder : IEncoder
    {
        private static DoubleEncoder instance;

        private DoubleEncoder() { }

        public static DoubleEncoder Instance => instance ?? (instance = new DoubleEncoder());

        /// <inheritdoc />
        public byte[] Encode(DbfField field, object data, Encoding encoding)
        {
            // DbfFieldType.Double is stored as 8-byte IEEE 754 binary (little-endian),
            // NOT as text. Decode reads it with BitConverter.ToDouble so Encode must
            // write the same binary representation.
            double value = 0.0;
            if (data != null)
            {
                try { value = Convert.ToDouble(data, CultureInfo.InvariantCulture); }
                catch { value = 0.0; }
            }
            return BitConverter.GetBytes(value);
        }

        /// <inheritdoc />
        public object Decode(byte[] buffer, byte[] memoData, Encoding encoding)
        {
            return BitConverter.ToDouble(buffer, 0);
        }
    }
}
