using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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

	}
}
