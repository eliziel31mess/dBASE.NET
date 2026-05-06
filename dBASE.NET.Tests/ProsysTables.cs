using dBASE.NET.Tests.Examples;
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
            //var tbl = @"W:\RelojW\Actual\P26\RTL02_26.dbf";
            var tbl = @"M:\RelojW\guanajuato\actual_spg\P26\RTL02_26.dbf";
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

        [TestMethod]
        public void RelojCompañias()
        {
            var tbl = @"W:\RelojW\Actual\P26\CMP01_26.dbf";
            dbf = new Dbf(Encoding.Default);
            dbf.Read(tbl);
            var entities = new List<Examples.Compañia>(dbf.GetEntities<Examples.Compañia>());

            string outputDir = @"W:\RelojW";
            string filePath = Path.Combine(outputDir, $"Logo.jpg");
            File.WriteAllBytes(filePath, entities[0].Logotipo);
            Assert.IsNotNull(entities);
        }

        [TestMethod]
        public void NominaExpediente()
        {
            var tbl = @"Y:\ProsysW\proynom\p25\TRA05_25.dbf";
            dbf = new Dbf(Encoding.Default);
            dbf.Read(tbl);
            var entities = new List<Examples.Expediente>(dbf.GetEntities<Examples.Expediente>());

            Assert.IsNotNull(entities);
        }

        [TestMethod]
        public void NominaAcumuladoFiscal()
        {
            var tbl = @"Y:\ProsysW\proynom\p25\NOM04T_25.dbf";
            dbf = new Dbf(Encoding.Default);
            dbf.Read(tbl);
            var entities = new List<Examples.AcumuladoFiscal>(dbf.GetEntities<Examples.AcumuladoFiscal>());

            Assert.IsNotNull(entities);
        }

        [TestMethod]
        public void NominaAcumuladoFiscalTricky()
        {
            var tbl = @"M:\ProsysW\spgnomina\P26\NOM04T_26.dbf";
            dbf = new Dbf(Encoding.Default);
            dbf.Read(tbl);
            var entities = new List<Examples.AcumuladoFiscal>(dbf.GetEntities<Examples.AcumuladoFiscal>());

            Assert.IsNotNull(entities);
        }

        [TestMethod]
        public void NominaAcumuladoFiscalDañado()
        {
            var tbl = @"Y:\ProsysW\NOMINA\P26\NOM04T_26.dbf";
            dbf = new Dbf(Encoding.Default);
            dbf.Read(tbl);

            var clon = dbf.Clone();
            clon.Write(@"Y:\ProsysW\NOMINA\P26\NOM04T_26_recuperado.dbf");
            var entities = new List<Examples.AcumuladoFiscal>(dbf.GetEntities<Examples.AcumuladoFiscal>());

            Assert.IsNotNull(entities);
        }

        [TestMethod]
        public void TablaDañada()
        {
            var tbl = @"K:\Afx\ToFix\DañadosOriginales\NOM03_26.DBF";
            dbf = new Dbf(Encoding.Default);
            dbf.Read(tbl);

            // Clonar la tabla dañada a una nueva y guardarla
            var clon = dbf.Clone();
            clon.Write(@"K:\Afx\ToFix\NOM03_26_recuperado.DBF");

            var entities = new List<Examples.NominaProceso>(dbf.GetEntities<Examples.NominaProceso>());
            Assert.IsNotNull(entities);
        }

        [TestMethod]
        public void CloneTablaDañada()
        {
            var tbl = @"K:\Victoreen\260416\NOM04T_26\NOM04T_26.DBF";
            dbf = new Dbf(Encoding.Default);
            dbf.Read(tbl);

            // Clonar la tabla dañada a una nueva y guardarla
            var clon = dbf.Clone();
            clon.Write(@"K:\Victoreen\260416\NOM04T_26\NOM04T_26_fixed.DBF");
            
            Assert.IsNotNull(clon);
        }

        [TestMethod]
        public void EmpresasTricky()
        {
            var tbl = @"M:\ProsysW\PROSYS.dbf";
            dbf = new Dbf(Encoding.Default);
            dbf.Read(tbl);
            var entities = new List<Examples.Empresa>(dbf.GetEntities<Examples.Empresa>());

            Assert.IsNotNull(entities);
        }
    }
}
