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
            // integer can be widened losslessly to double/decimal (and float) via Convert.ChangeType
            { 'I', new[] { typeof(int), typeof(long), typeof(double), typeof(decimal), typeof(float) } },
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
        /// are compatible with the actual DBF field types. Returns null when there are no errors;
        /// warnings alone are not considered a failure. Otherwise returns a descriptive string
        /// listing every mismatch (and any warnings) found.
        /// </summary>
        public static string ValidateEntityMapping<T>(this dBASE.NET.Dbf dbf) where T : class, new()
            => ValidateEntityMapping<T>(dbf, out _);

        /// <summary>
        /// Same as <see cref="ValidateEntityMapping{T}(dBASE.NET.Dbf)"/> but also surfaces the
        /// non-blocking warnings so callers can log or inspect them without treating them as a failure.
        /// </summary>
        public static string ValidateEntityMapping<T>(this dBASE.NET.Dbf dbf, out IReadOnlyList<string> warnings) where T : class, new()
        {
            var errors = new List<string>();
            var warningsList = new List<string>();
            warnings = warningsList;
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
                    errors.Add($"  [{attr.Name}] -> campo no encontrado en el DBF  (propiedad: {prop.Name} {prop.PropertyType.Name})");
                    continue;
                }

                char dbfType = (char)dbfField.Type;
                Type propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                if (!_dbfTypeMap.TryGetValue(dbfType, out Type[] expected))
                {
                    errors.Add($"  [{attr.Name}] tipo DBF '{dbfType}' desconocido  (propiedad: {prop.Name} {prop.PropertyType.Name})");
                    continue;
                }

                bool compatible = Array.IndexOf(expected, propType) >= 0;
                if (!compatible)
                {
                    string expectedNames = string.Join(" | ", Array.ConvertAll(expected, t => t.Name));
                    errors.Add($"  [ERROR]   [{attr.Name}] DBF tipo '{dbfType}' no es compatible con <{propType.Name}> en '{prop.Name}'. Tipos válidos: <{expectedNames}>");
                }
                else if (_dbfNativeType.TryGetValue(dbfType, out Type nativeType) && nativeType != propType)
                {
                    warningsList.Add($"  [AVISO]   [{attr.Name}] DBF tipo '{dbfType}' retorna <{nativeType.Name}> en runtime; '{prop.Name}' es <{propType.Name}> (se aplicará Convert.ChangeType)");
                }
            }

            if (errors.Count == 0)
                return null;

            var sb = new StringBuilder($"Incompatibilidades en el mapeo de {typeof(T).Name}:{Environment.NewLine}");
            foreach (var error in errors)
                sb.AppendLine(error);
            if (warningsList.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Avisos (no bloqueantes):");
                foreach (var warning in warningsList)
                    sb.AppendLine(warning);
            }
            return sb.ToString();
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
