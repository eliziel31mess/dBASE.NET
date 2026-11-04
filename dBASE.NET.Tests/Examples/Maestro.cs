using dBASE.NET.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dBASE.NET.Tests.Examples
{
    public class Maestro
    {
        [DbfField("CP701")] public string Clave { set; get; }
        [DbfField("CP702")] public string Descripcion { get; set; }
        [DbfField("CP703")] public double CampoExtra01 { get; set; }
        [DbfField("CP704")] public double CampoExtra02 { get; set; }
    }
}
