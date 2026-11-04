using System;
using System.IO;
using System.Linq;
using dBASE.NET.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dBASE.NET.Tests
{
    [TestClass]
    public class DbfRecordTests
    {
        Dbf dbf;

        [TestInitialize]
        public void testInit()
        {
            dbf = new Dbf();
            dbf.Read("fixtures/83/dbase_83.dbf");
        }

        [TestMethod]
        public void ToStringTest()
        {
            Assert.AreEqual(
                "ID=87,CATCOUNT=2,AGRPCOUNT=0,PGRPCOUNT=0,ORDER=87,CODE=1,NAME=Assorted Petits Fours,THUMBNAIL=graphics/00000001/t_1.jpg,IMAGE=graphics/00000001/1.jpg,PRICE=0,COST=0,DESC=,WEIGHT=5.51,TAXABLE=True,ACTIVE=True",
                dbf.Records[0].ToString(),
                "Default separator and mask");
            Assert.AreEqual(
                "ID=87; CATCOUNT=2; AGRPCOUNT=0; PGRPCOUNT=0; ORDER=87; CODE=1; NAME=Assorted Petits Fours; THUMBNAIL=graphics/00000001/t_1.jpg; IMAGE=graphics/00000001/1.jpg; PRICE=0; COST=0; DESC=; WEIGHT=5.51; TAXABLE=True; ACTIVE=True",
                dbf.Records[0].ToString("; "),
                "Custom separator and default mask");
            Assert.AreEqual(
                "ID->87; CATCOUNT->2; AGRPCOUNT->0; PGRPCOUNT->0; ORDER->87; CODE->1; NAME->Assorted Petits Fours; THUMBNAIL->graphics/00000001/t_1.jpg; IMAGE->graphics/00000001/1.jpg; PRICE->0; COST->0; DESC->; WEIGHT->5.51; TAXABLE->True; ACTIVE->True",
                dbf.Records[0].ToString("; ", "{name}->{value}"),
                "Custom separator and mask");
            Assert.AreEqual(
                "ID; CATCOUNT; AGRPCOUNT; PGRPCOUNT; ORDER; CODE; NAME; THUMBNAIL; IMAGE; PRICE; COST; DESC; WEIGHT; TAXABLE; ACTIVE",
                dbf.Records[0].ToString("; ", "{name}"),
                "Custom separator and mask with only name");
            Assert.AreEqual(
                "87; 2; 0; 0; 87; 1; Assorted Petits Fours; graphics/00000001/t_1.jpg; graphics/00000001/1.jpg; 0; 0; ; 5.51; True; True",
                dbf.Records[0].ToString("; ", "{value}"),
                "Custom separator and mask with only value");
        }

        [TestMethod]
        public void CharacterFieldsPreserveRawPaddingAndTrimOnEntityMappingByDefault()
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".dbf");

            try
            {
                var source = new Dbf();
                source.Fields.Add(new DbfField("NORMAL", DbfFieldType.Character, 8));
                source.Fields.Add(new DbfField("KARDEX", DbfFieldType.Character, 10));
                var record = source.CreateRecord();
                record.Data[0] = "ABC";
                record.Data[1] = "VP  V";
                source.Write(path);

                var dbf = new Dbf();
                dbf.Read(path);

                Assert.AreEqual("ABC     ", dbf.Records[0].Data[0]);
                Assert.AreEqual("VP  V     ", dbf.Records[0].Data[1]);

                var entity = dbf.GetEntities<CharacterWhitespaceEntity>().Single();
                Assert.AreEqual("ABC", entity.NormalText);
                Assert.AreEqual("VP  V     ", entity.KardexSigns);

                var safeEntity = dbf.SafeGetEntities<CharacterWhitespaceEntity>().Single();
                Assert.AreEqual("ABC", safeEntity.NormalText);
                Assert.AreEqual("VP  V     ", safeEntity.KardexSigns);

                var v2Record = new dBASE.NET.V2.DbfRecord(dbf.Fields);
                v2Record.Data[0] = dbf.Records[0].Data[0];
                v2Record.Data[1] = dbf.Records[0].Data[1];
                var v2Entity = new CharacterWhitespaceEntity();
                v2Record.ToEntity(v2Entity);
                Assert.AreEqual("ABC", v2Entity.NormalText);
                Assert.AreEqual("VP  V     ", v2Entity.KardexSigns);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        public class CharacterWhitespaceEntity
        {
            [dBASE.NET.V2.DbfField("NORMAL")]
            public string NormalText { get; set; }

            [dBASE.NET.V2.DbfField("KARDEX", PreserveTrailingSpaces = true)]
            public string KardexSigns { get; set; }
        }
    }
}
