using dBASE.NET.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dBASE.NET.Tests.Examples
{
    public class Expediente
    {
        [DbfField("TR501")] public string Clave { get; set; }
        [DbfField("TR502")] public string Referencia { get; set; }
        [DbfField("TR503")] public string Descripcion { get; set; }
    }
}
