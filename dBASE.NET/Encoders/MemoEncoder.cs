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

            int lengthToSkip = index * blockSize + 8;

            // Guard against corrupted memo files (e.g. FoxPro reports "missing or invalid").
            // If the computed offset + length exceeds the actual data buffer, the record is
            // unreadable; return null instead of throwing IndexOutOfRangeException.
            if (lengthToSkip < 0 || length < 0 || lengthToSkip + length > memoData.Length)
                return null;

            byte[] memoBytes = new byte[length];

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
        /// Also guards against false positives when the content is printable text
        /// (e.g. base64 strings that happen to start with "BM").
        ///
        /// Distinguishes real binary files from text by checking for non-printable
        /// bytes immediately after the magic signature. Real binary headers (BMP,
        /// JPEG, etc.) have null bytes / control chars within the first few bytes
        /// past the signature, while base64 text does not.
        /// </summary>
        private static bool IsBinaryContent(byte[] data)
        {
            if (data == null || data.Length < 2) return false;

            int signatureLen = 0;

            // JPEG: FF D8 FF
            if (data.Length >= 3 && data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF)
                signatureLen = 3;
            // PNG: 89 50 4E 47
            else if (data.Length >= 4 && data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47)
                signatureLen = 4;
            // BMP: 42 4D
            else if (data[0] == 0x42 && data[1] == 0x4D)
                signatureLen = 2;
            // GIF: 47 49 46 38
            else if (data.Length >= 4 && data[0] == 0x47 && data[1] == 0x49 && data[2] == 0x46 && data[3] == 0x38)
                signatureLen = 4;
            // PDF: 25 50 44 46
            else if (data.Length >= 4 && data[0] == 0x25 && data[1] == 0x50 && data[2] == 0x44 && data[3] == 0x46)
                signatureLen = 4;
            // ZIP / DOCX / XLSX: 50 4B 03 04
            else if (data.Length >= 4 && data[0] == 0x50 && data[1] == 0x4B && data[2] == 0x03 && data[3] == 0x04)
                signatureLen = 4;

            if (signatureLen == 0) return false;

            // Check the bytes right after the signature for non-printable content.
            // Real binary files have nulls/control chars within the next few bytes;
            // base64 text does not (all chars are printable ASCII).
            int probeLen = Math.Min(data.Length, signatureLen + 16);
            for (int i = signatureLen; i < probeLen; i++)
            {
                byte b = data[i];
                bool isPrintable = (b >= 0x20 && b <= 0x7E) || b == 0x09 || b == 0x0A || b == 0x0D;
                if (!isPrintable)
                    return true;
            }

            return false;
        }
    }
}