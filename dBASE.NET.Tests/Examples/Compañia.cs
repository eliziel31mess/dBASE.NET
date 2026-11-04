using dBASE.NET.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dBASE.NET.Tests.Examples
{
    public class Compañia
    {
        [DbfField("CP101")] public string Clave { get; set; }
        [DbfField("CP102")] public string NombreCorto { get; set; }
        [DbfField("CP103")] public string NombreLargo { get; set; }

        [DbfField("CP128")] public byte[] Logotipo { get; set; }
    }
}
