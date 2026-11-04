using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace dBASE.NET.V2
{
    /// <summary>
    /// Defines a field within an entity
    /// </summary>
    public class DbfFieldAttribute : Attribute
    {
        /// <summary>
        /// Name of the property in the DBF
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets whether trailing spaces in a Character field should be
        /// preserved when mapping this field to an entity property. By default,
        /// trailing spaces are trimmed for compatibility with existing mappings.
        /// </summary>
        public bool PreserveTrailingSpaces { get; set; }

        /// <summary>
        /// Create an attribute to be able to bind to a DBF field.
        /// </summary>
        /// <param name="name">
        ///     The name of the field. Can be omitted, in that case it takes the name of the property.
        /// </param>
        public DbfFieldAttribute([CallerMemberName] string name = null)
        {
            Name = name;
        }

        internal object ApplyReadBehavior(object value, DbfField field)
        {
            if (PreserveTrailingSpaces || field?.Type != DbfFieldType.Character || !(value is string text))
                return value;

            text = text.TrimEnd();
            return text.Length == 0 ? null : text;
        }
    }
}
