namespace dBASE.NET.Encoders
{
    using System;
    using System.Globalization;
    using System.Text;

    internal class DateEncoder : IEncoder
    {
        private const string format = "yyyyMMdd";

        private static DateEncoder instance;

        private DateEncoder() { }

        public static DateEncoder Instance => instance ?? (instance = new DateEncoder());

        /// <inheritdoc />
        public byte[] Encode(DbfField field, object data, Encoding encoding)
        {
            string text;
            // DateTime.MinValue (01/01/0001) comes from uninitialized structs or
            // corrupted records – treat it as empty, same as null.
            if (data is DateTime dt && dt != DateTime.MinValue)
            {
                text = dt.ToString(format).PadLeft(field.Length, ' ');
            }
            else
            {
                text = field.DefaultValue;
            }

            return encoding.GetBytes(text);
        }

        /// <inheritdoc />
        public object Decode(byte[] buffer, byte[] memoData, Encoding encoding)
        {
            // Corrupted tables may store null bytes (\0) instead of spaces — strip both.
            string text = encoding.GetString(buffer).Trim().Trim('\0');
            if (text.Length == 0) return null;
            if (DateTime.TryParseExact(text, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                return result;
            return null;
        }
    }
}