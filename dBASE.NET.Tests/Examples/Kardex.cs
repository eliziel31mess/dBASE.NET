using dBASE.NET.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dBASE.NET.Tests.Examples
{
    public class Kardex
    {
        [DbfField("TR601")] public string TrabajadorId { set; get; }
        [DbfField("TR602")] public string Mes { set; get; }
        //[DbfField("TR603", PreserveTrailingSpaces = true)] public string Signos { set; get; }
        [DbfField("TR603")] public string Signos { set; get; }
    }
}
