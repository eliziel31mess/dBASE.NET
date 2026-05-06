using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            var tbl = "fixtures/Reloj-Dañado/REL06_25.DBF";
            dbf = new Dbf(Encoding.Default);
            dbf.Read(tbl);

            var clon = dbf.Clone();
            clon.Write(@"fixtures/Reloj-Dañado/REL06_25_fixed.DBF");
            Assert.IsNotNull(clon);
        }
    }
}
