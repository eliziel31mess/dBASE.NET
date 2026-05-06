using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dBASE.NET.Tests
{
    [TestClass]
    public class TablesToBeFixed
    {
        private Dbf dbf;

        [TestMethod]
        public void RelojAusentismosDañados()
        {
            try
            {
                var archivo = "fixtures/Reloj-Dañado/REL06_25.DBF";
                dbf = new Dbf(Encoding.Default);
                dbf.Read(archivo);

                if (dbf.IsCorrupted)
                {
                    string salida = Path.Combine(
                        Path.GetDirectoryName(archivo),
                        Path.GetFileNameWithoutExtension(archivo) + "_reparado.dbf");

                    dbf.Repair(salida);
                    Console.WriteLine($"[REPARADO] {archivo} -> {salida}");
                    Assert.IsNotNull(dbf);
                }
                else
                {
                    Console.WriteLine($"[OK]       {archivo}");
                    Assert.IsNotNull(dbf);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
            
        }
    }
}
