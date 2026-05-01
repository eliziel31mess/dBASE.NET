using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace dBASE.NET.Tests
{
	/// <summary>
	/// VisualFoxProWithAI is version 0x31.
	/// </summary>
	[TestClass]
	public class ProsysTables
    {
		private Dbf dbf;

        [TestMethod]
        public void RelojMaestros()
        {
            var tbl = @"W:\RelojW\Actual\P26\cmp07_26.dbf";
            dbf = new Dbf(Encoding.Default);
            dbf.Read(tbl);
            var entities = new List<Examples.Maestro>(dbf.GetEntities<Examples.Maestro>());
            Assert.IsNotNull(entities);
        }

        [TestMethod]
        public void RelojFotos()
        {
            var tbl = @"W:\RelojW\Actual\P26\RTL02_26.dbf";
            dbf = new Dbf(Encoding.Default);
            dbf.Read(tbl);
            //foreach (var record in dbf.Records)
            //{
            //    foreach (var d in record.Data)
            //    {
            //        Console.WriteLine(d);
            //    }
            //}
            var entities = new List<Examples.Foto>(dbf.GetEntities<Examples.Foto>());

            string outputDir = @"W:\RelojW\Actual\Fotos";
            Directory.CreateDirectory(outputDir);

            foreach (var foto in entities)
            {
                if (foto.FotoBytes == null || foto.FotoBytes.Length == 0) continue;
                string filePath = Path.Combine(outputDir, $"{foto.Clave}.jpg");
                File.WriteAllBytes(filePath, foto.FotoBytes);
            }

            Assert.IsNotNull(entities);
        }
    }
}
