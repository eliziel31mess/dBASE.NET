namespace dBASE.NET.Encoders
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class MemoEncoder : IEncoder
    {
        private static MemoEncoder instance;

        private MemoEncoder() { }

        public static MemoEncoder Instance => instance ?? (instance = new MemoEncoder());
        
        // cach different length bytes (for performance)
        Dictionary<int, byte[]> buffers = new Dictionary<int, byte[]>();

        private byte[] GetBuffer(int length) {
            if (!buffers.TryGetValue(length, out var bytes)) {
                var s = new string(' ', length);
                bytes = Encoding.ASCII.GetBytes(s);
                buffers.Add(length, bytes);
            }
            return (byte[])bytes.Clone();
        }

        /// <inheritdoc />
        public byte[] Encode(DbfField field, object data, Encoding encoding)
        {
            // Input data maybe various: int, string, whatever.
            string res = data?.ToString();
            if (string.IsNullOrEmpty(res)) {
                res = field.DefaultValue;
            }
            // Emanuele Bonin
            // 24/03/2025
            int BufferLen = res.Length;

            // consider multibyte should truncate or padding after GetBytes (11 bytes)
            var buffer = GetBuffer(BufferLen);
            var bytes = encoding.GetBytes(res);
            Array.Copy(bytes, buffer, Math.Min(bytes.Length, BufferLen));

            return buffer;
        }

        /// <inheritdoc />
        public object Decode(byte[] buffer, byte[] memoData, Encoding encoding)
        {
            int index = 0;
            // Memo fields of 5+ byts in length store their index in text, e.g. "     39394"
            // Memo fields of 4 bytes store their index as an int.
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

            return findMemo(index, memoData, encoding);
        }

        private static object findMemo(int index, byte[] memoData, Encoding encoding)
        {
            // This is the original implementation of findMemo. It was found that
            // the LINQ methods are orders of magnitude slower than using using array
            // offsets.

            /* UInt16 blockSize = BitConverter.ToUInt16(memoData.Skip(6).Take(2).Reverse().ToArray(), 0);
               int type = (int)BitConverter.ToUInt32(memoData.Skip(index * blockSize).Take(4).Reverse().ToArray(), 0);
               int length = (int)BitConverter.ToUInt32(memoData.Skip(index * blockSize + 4).Take(4).Reverse().ToArray(), 0);
               string text = encoding.GetString(memoData.Skip(index * blockSize + 8).Take(length).ToArray()).Trim();
               return text; */

            // The index is measured from the start of the file, even though the memo file header blocks takes
            // up the first few index positions.
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

            byte[] memoBytes = new byte[length];
            int lengthToSkip = index * blockSize + 8;

            for (int i = lengthToSkip; i < lengthToSkip + length; ++i)
            {
                memoBytes[i - lengthToSkip] = memoData[i];
            }

            if (IsBinaryContent(memoBytes))
                return memoBytes;

            return encoding.GetString(memoBytes).TrimEnd();
        }

        /// <summary>
        /// Detects whether the raw memo bytes contain binary content by checking
        /// well-known file format magic bytes (JPEG, PNG, BMP, GIF, PDF, ZIP, etc.).
        /// </summary>
        private static bool IsBinaryContent(byte[] data)
        {
            if (data == null || data.Length < 2) return false;

            // JPEG: FF D8 FF
            if (data.Length >= 3 && data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF)
                return true;
            // PNG: 89 50 4E 47
            if (data.Length >= 4 && data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47)
                return true;
            // BMP: 42 4D
            if (data[0] == 0x42 && data[1] == 0x4D)
                return true;
            // GIF: 47 49 46 38
            if (data.Length >= 4 && data[0] == 0x47 && data[1] == 0x49 && data[2] == 0x46 && data[3] == 0x38)
                return true;
            // PDF: 25 50 44 46
            if (data.Length >= 4 && data[0] == 0x25 && data[1] == 0x50 && data[2] == 0x44 && data[3] == 0x46)
                return true;
            // ZIP / DOCX / XLSX: 50 4B 03 04
            if (data.Length >= 4 && data[0] == 0x50 && data[1] == 0x4B && data[2] == 0x03 && data[3] == 0x04)
                return true;

            return false;
        }
    }
}