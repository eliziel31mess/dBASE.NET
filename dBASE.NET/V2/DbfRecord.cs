using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dBASE.NET.V2
{
    public class DbfRecord : NET.DbfRecord
    {
        public DbfRecord(List<DbfField> fields) : base(fields)
        {

        }
        /// <summary>
        /// Return the index of the field by it's name.
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <returns></returns>
        public int GetFieldIndex(string fieldName)
        {
            return fields.FindIndex(x => x.Name.Equals(fieldName));
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
                    Data[GetFieldIndex(attribute.Name)] = property.GetValue(obj);
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
                    property.SetValue(obj, Data[GetFieldIndex(attribute.Name)]);
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


    }
}
