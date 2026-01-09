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
            string text = Convert.ToString(data, CultureInfo.InvariantCulture);
            if (string.IsNullOrEmpty(text))
            {
                text = field.DefaultValue;
            }
            else
            {
                var parts = text.Split('.');
                if (parts.Length == 2)
                {
                    // Truncate or pad float part.
                    if (parts[1].Length > field.Precision)
                    {
                        parts[1] = parts[1].Substring(0, field.Precision);
                    }
                    else
                    {
                        parts[1] = parts[1].PadRight(field.Precision, '0');
                    }
                }
                else if (field.Precision > 0)
                {
                    // If value has no fractional part, pad it with zeros.
                    parts = new[] { parts[0], new string('0', field.Precision) };
                }

                text = string.Join(".", parts);

                // Pad string with spaces or trim.
                text = text.Length > field.Length
                    ? text.Substring(0, field.Length)
                    : text.PadLeft(field.Length, ' ');
            }

            return encoding.GetBytes(text);
        }

        /// <inheritdoc />
        public object Decode(byte[] buffer, byte[] memoData, Encoding encoding)
        {
            return BitConverter.ToDouble(buffer, 0);
        }
    }
}
