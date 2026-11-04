using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dBASE.NET.V2
{
    public class Dbf : NET.Dbf
    {
        public Encoding Encoding { get; } = Encoding.ASCII;
        public Dbf(Encoding encoding)
        {
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }
        public List<DbfRecord> Records { get; }
        public DbfRecord CreateRecord()
        {
            DbfRecord record = new DbfRecord(Fields);
            Records.Add(record);
            return record;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DbfRecord CreateRecord<T>(T entity)
        {
            var record = CreateRecord();
            record.FromEntity(entity);
            return record;
        }

        /// <summary>
        /// Add a list of entities to the DBF.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public IEnumerable<DbfRecord> AddEntities<T>(IEnumerable<T> entities)
        {
            var records = new List<DbfRecord>();

            foreach (var entity in entities)
            {
                var record = CreateRecord();
                record.FromEntity(entity);
                records.Add(CreateRecord(entity));
            }

            return records;
        }

        /// <summary>
        /// Get records from the DBF mapped into entities.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetEntities<T>()
        {
            var entities = new List<T>();
            foreach (var record in Records)
            {
                if (record.IsDeleted) continue;
                var entity = (T)Activator.CreateInstance(typeof(T));
                record.ToEntity(entity);
                entities.Add(entity);
            }

            return entities;
        }
    }
}
