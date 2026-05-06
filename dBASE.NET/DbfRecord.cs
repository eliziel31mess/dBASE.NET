namespace dBASE.NET {
    using dBASE.NET.Encoders;
    using dBASE.NET.V2;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// DbfRecord encapsulates a record in a .dbf file. It contains an array with
    /// data (as an Object) for each field.
    /// </summary>
    public class DbfRecord {
        private const string defaultSeparator = ",";
        private const string defaultMask = "{name}={value}";

        public Dbf ParentDbf;

        public List<DbfField> fields;

        /// <summary>
        /// Indicates whether this record is marked as deleted.
        /// </summary>
        public bool IsDeleted { get; internal set; }

        internal DbfRecord(BinaryReader reader, DbfHeader header, List<DbfField> fields, byte[] memoData, Encoding encoding) {
            this.fields = fields;
            Data = new List<object>();

            // Read record marker.
            byte marker = reader.ReadByte();
            IsDeleted = marker == 0x2A;

            // Read entire record as sequence of bytes.
            // Note that record length includes marker.
            byte[] row = reader.ReadBytes(header.RecordLength - 1);
            if (row.Length == 0)
                throw new EndOfStreamException();

            // Read data for each field.
            int offset = 0;
            foreach (DbfField field in fields) {
                // Copy bytes from record buffer into field buffer.
                byte[] buffer = new byte[field.Length];
                Array.Copy(row, offset, buffer, 0, field.Length);
                offset += field.Length;

                IEncoder encoder = EncoderFactory.GetEncoder(field.Type);
                Data.Add(encoder.Decode(buffer, memoData, encoding));
            }
        }

        /// <summary>
        /// Create an empty record.
        /// </summary>
        internal DbfRecord(List<DbfField> fields) {
            this.fields = fields;
            Data = new List<object>();
            foreach (DbfField field in fields) Data.Add(null);
        }

        public List<object> Data { get; }

        public object this[int index] => Data[index];

        public object this[string name] {
            get {
                int index = fields.FindIndex(x => x.Name.Equals(name));
                if (index == -1) return null;
                return Data[index];
            }
        }

        public object this[DbfField field] {
            get {
                int index = fields.IndexOf(field);
                if (index == -1) return null;
                return Data[index];
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            return ToString(defaultSeparator, defaultMask);
        }

        /// <summary>
        /// Returns a string that represents the current object with custom separator.
        /// </summary>
        /// <param name="separator">Custom separator.</param>
        /// <returns>A string that represents the current object with custom separator.</returns>
        public string ToString(string separator) {
            return ToString(separator, defaultMask);
        }

        /// <summary>
        /// Returns a string that represents the current object with custom separator and mask.
        /// </summary>
        /// <param name="separator">Custom separator.</param>
        /// <param name="mask">
        /// Custom mask.
        /// <para>e.g., "{name}={value}", where {name} is the mask for the field name, and {value} is the mask for the value.</para>
        /// </param>
        /// <returns>A string that represents the current object with custom separator and mask.</returns>
        public string ToString(string separator, string mask) {
            separator = separator ?? defaultSeparator;
            mask = (mask ?? defaultMask).Replace("{name}", "{0}").Replace("{value}", "{1}");

            return string.Join(separator, fields.Select(z => string.Format(mask, z.Name, this[z])));
        }

        internal void Write(BinaryWriter writer, Encoding encoding) {
            // Emanuele Bonin 22/03/2025
            // VFP MemoFile
            string MemoFile;
            bool HasMemo = false;
            FileStream stream = null;
            BinaryWriter Memowriter = null;
            BinaryReader Memoreader = null;
            int UsedBlocks = 0, BlockSize = 0, FreeBlockPointer = 0;
            // Write marker (deleted flag)
            writer.Write((byte)(IsDeleted ? 0x2A : 0x20));

            int index = 0;
            HasMemo = fields.Any(f => f.Type == DbfFieldType.Memo);
            if (HasMemo) {
                // Emanuele Bonin 22/03/2025
                // VFP MemoFile
                MemoFile = Path.ChangeExtension(ParentDbf.DBFPath, "fpt");

                stream = File.Open(MemoFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                Memowriter = new BinaryWriter(stream, Encoding.ASCII);
                Memoreader = new BinaryReader(stream, Encoding.ASCII);

                

                // Read 32-bit integer as big endian
                byte[] bytes = Memoreader.ReadBytes(4); // 0x00 - 0x03 next free block
                Array.Reverse(bytes);
                FreeBlockPointer = BitConverter.ToInt32(bytes, 0);
                Memoreader.BaseStream.Seek(6, SeekOrigin.Begin); // 0x06 - 0x07 block size in bytes
                bytes = Memoreader.ReadBytes(2);
                Array.Reverse(bytes);
                BlockSize = BitConverter.ToInt16(bytes, 0);

            }
            // https://www.vfphelp.com/help/_5WN12PC0N.htm
            foreach (DbfField field in fields) {
                IEncoder encoder = EncoderFactory.GetEncoder(field.Type);

                byte[] buffer = encoder.Encode(field, Data[index], encoding);
                if (field.Type == DbfFieldType.Memo) {

                    if (!buffer.All(b => b == 0x20)) {

                        // Write on memo file
                        // and get the memo position
                        Memowriter.Seek(FreeBlockPointer * BlockSize, SeekOrigin.Begin);
                        Memowriter.Write(BitConverter.GetBytes((int)1).Reverse().ToArray()); // 0x00 - 0x03 Block signature Big Endian
                                                                                             // (indicates the type of data in the block)
                                                                                             // 0 – picture (picture field type)
                                                                                             // 1 – text (memo field type)

                        Memowriter.Write(BitConverter.GetBytes((int)buffer.Length).Reverse().ToArray()); // Length of memo(in bytes) Big Endian
                        Memowriter.Write(buffer);
                        UsedBlocks = (int)Math.Ceiling(buffer.Length / (decimal)BlockSize);
                        // Fill rest of the used blocks with 0x00
                        for (int i = 0; i < UsedBlocks * BlockSize - buffer.Length; i++) {
                            Memowriter.Write((byte)0x00);
                        }
                        buffer = BitConverter.GetBytes((int)FreeBlockPointer);
                        FreeBlockPointer = FreeBlockPointer + UsedBlocks;
                    } else {
                        UsedBlocks = 0;
                        buffer = BitConverter.GetBytes((int)0);
                    }                    
                    
                }
                if (buffer.Length > field.Length)
                    throw new ArgumentOutOfRangeException(nameof(buffer.Length), buffer.Length, "Buffer length has exceeded length of the field.");
                writer.Write(buffer);
                index++;
            }
            if (HasMemo) {
                Memowriter.Seek(0, SeekOrigin.Begin);
                Memowriter.Write(BitConverter.GetBytes((int)FreeBlockPointer).Reverse().ToArray());
                Memowriter.Close();
                Memoreader.Close();
            }
        }

        #region Extends
        /// <summary>
        /// Return the index of the field by it's name.
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <returns></returns>
        public int GetFieldIndex(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName)) return -1;
            // Use OrdinalIgnoreCase so that corrupted tables with mixed-case field
            // names (e.g. "Nt401" instead of "NT401") still map correctly.
            // Also trim both sides to guard against stray whitespace/null chars.
            string normalized = fieldName.Trim();
            int index = fields.FindIndex(x =>
                string.Compare(x.Name.Trim(), normalized, StringComparison.OrdinalIgnoreCase) == 0);
            return index;
        }
        public void FromEntity<T>(T obj)
        {
            var properties = GetDecoratedProperties(obj);

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute(typeof(DbfFieldAttribute)) as DbfFieldAttribute;

                if (attribute == null)
                {
                    throw new InvalidOperationException(
                        $"Property {property.Name} does not have the DbfField attribute!"
                    );
                }

                if (property.CanRead)
                {
                    int fieldIndex = GetFieldIndex(attribute.Name);
                    if (fieldIndex < 0 || fieldIndex >= Data.Count)
                        continue;

                    Data[fieldIndex] = property.GetValue(obj);
                }
            }
        }

        public void ToEntity<T>(T obj)
        {
            var properties = GetDecoratedProperties(obj);

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute(typeof(DbfFieldAttribute)) as DbfFieldAttribute;

                if (attribute == null)
                {
                    throw new InvalidOperationException(
                        $"Property {property.Name} does not have the DbfField attribute!"
                    );
                }

                if (property.CanWrite)
                {
                    int fieldIndex = GetFieldIndex(attribute.Name);
                    if (fieldIndex < 0 || fieldIndex >= Data.Count)
                        continue;

                    property.SetValue(obj, Data[fieldIndex]);
                }
            }
        }

        internal PropertyInfo[] GetDecoratedProperties(object obj)
        {
            var decoratedProperties = new List<PropertyInfo>();

            var allProperties = obj.GetType().GetProperties();

            foreach (var property in allProperties)
            {
                var attributes = property.GetCustomAttributes();
                foreach (var attr in attributes)
                {
                    if (attr is DbfFieldAttribute)
                    {
                        decoratedProperties.Add(property);
                    }
                }
            }

            return decoratedProperties.ToArray();
        }
        #endregion
    }
}
