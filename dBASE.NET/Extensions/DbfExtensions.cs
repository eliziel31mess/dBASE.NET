using dBASE.NET.V2;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dBASE.NET.Extensions
{
    public static class DbfExtensions
    {
        // Maps DBF type character to:
        //   [0] = the actual runtime type dBASE.NET returns (native)
        //   [1..] = additional types that are assignment-compatible via Convert.ChangeType
        private static readonly Dictionary<char, Type[]> _dbfTypeMap = new Dictionary<char, Type[]>
        {
            { 'C', new[] { typeof(string) } },
            { 'L', new[] { typeof(bool) } },
            // dBASE.NET always returns double for N fields; int/long/decimal need Convert.ChangeType
            { 'N', new[] { typeof(double), typeof(int), typeof(long), typeof(decimal), typeof(float) } },
            { 'F', new[] { typeof(double), typeof(float), typeof(decimal) } },
            { 'D', new[] { typeof(DateTime) } },
            { 'T', new[] { typeof(DateTime) } },
            { 'I', new[] { typeof(int), typeof(long) } },
            { 'B', new[] { typeof(double) } },
            { 'M', new[] { typeof(byte[]), typeof(string) } },
            { 'G', new[] { typeof(byte[]) } },
            { 'P', new[] { typeof(byte[]) } },
        };

        // Native runtime type returned by dBASE.NET for each DBF type
        private static readonly Dictionary<char, Type> _dbfNativeType = new Dictionary<char, Type>
        {
            { 'C', typeof(string) },
            { 'L', typeof(bool) },
            { 'N', typeof(double) },
            { 'F', typeof(double) },
            { 'D', typeof(DateTime) },
            { 'T', typeof(DateTime) },
            { 'I', typeof(int) },
            { 'B', typeof(double) },
            { 'M', typeof(byte[]) },
            { 'G', typeof(byte[]) },
            { 'P', typeof(byte[]) },
        };

        /// <summary>
        /// Validates that the C# entity properties decorated with <see cref="DbfFieldAttribute"/>
        /// are compatible with the actual DBF field types. Returns null when everything is correct,
        /// or a descriptive string listing every mismatch found.
        /// </summary>
        public static string ValidateEntityMapping<T>(this dBASE.NET.Dbf dbf) where T : class, new()
        {
            var sb = new StringBuilder();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Index DBF fields by name for quick lookup
            var dbfFieldsByName = new Dictionary<string, dBASE.NET.DbfField>(StringComparer.OrdinalIgnoreCase);
            foreach (var f in dbf.Fields)
                dbfFieldsByName[f.Name] = f;

            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttribute<DbfFieldAttribute>();
                if (attr == null) continue;

                if (!dbfFieldsByName.TryGetValue(attr.Name, out var dbfField))
                {
                    sb.AppendLine($"  [{attr.Name}] -> campo no encontrado en el DBF  (propiedad: {prop.Name} {prop.PropertyType.Name})");
                    continue;
                }

                char dbfType = (char)dbfField.Type;
                Type propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                if (!_dbfTypeMap.TryGetValue(dbfType, out Type[] expected))
                {
                    sb.AppendLine($"  [{attr.Name}] tipo DBF '{dbfType}' desconocido  (propiedad: {prop.Name} {prop.PropertyType.Name})");
                    continue;
                }

                bool compatible = Array.IndexOf(expected, propType) >= 0;
                if (!compatible)
                {
                    string expectedNames = string.Join(" | ", Array.ConvertAll(expected, t => t.Name));
                    sb.AppendLine($"  [ERROR]   [{attr.Name}] DBF tipo '{dbfType}' no es compatible con <{propType.Name}> en '{prop.Name}'. Tipos válidos: <{expectedNames}>");
                }
                else if (_dbfNativeType.TryGetValue(dbfType, out Type nativeType) && nativeType != propType)
                {
                    sb.AppendLine($"  [AVISO]   [{attr.Name}] DBF tipo '{dbfType}' retorna <{nativeType.Name}> en runtime; '{prop.Name}' es <{propType.Name}> (se aplicará Convert.ChangeType)");
                }
            }

            if (sb.Length == 0)
                return null;

            return $"Incompatibilidades en el mapeo de {typeof(T).Name}:{Environment.NewLine}{sb}";
        }

        public static IEnumerable<T> SafeGetEntities<T>(this dBASE.NET.Dbf dbf) where T : class, new()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fieldIndexMap = new Dictionary<PropertyInfo, int>();

            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttribute<DbfFieldAttribute>();
                if (attr == null) continue;

                for (int i = 0; i < dbf.Fields.Count; i++)
                {

                    if (dbf.Fields[i].Name == attr.Name)
                    {
                        fieldIndexMap[prop] = i;
                        break;
                    }
                }
            }

            foreach (dBASE.NET.DbfRecord record in dbf.Records)
            {
                if (record.IsDeleted) continue;
                var entity = new T();
                foreach (var kvp in fieldIndexMap)
                {
                    if (kvp.Key.CanWrite)
                    {
                        var value = record.Data[kvp.Value];
                        var propType = kvp.Key.PropertyType;
                        var underlyingType = Nullable.GetUnderlyingType(propType) ?? propType;
                        if (value == null)
                        {
                            if (propType.IsValueType && Nullable.GetUnderlyingType(propType) == null)
                                value = Activator.CreateInstance(propType);
                        }
                        else if (value.GetType() != underlyingType && underlyingType != typeof(object))
                        {
                            try { value = Convert.ChangeType(value, underlyingType); }
                            catch { value = underlyingType.IsValueType ? Activator.CreateInstance(underlyingType) : null; }
                        }
                        kvp.Key.SetValue(entity, value);
                    }
                }
                yield return entity;
            }
        }
    }
}
