using dBASE.NET.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dBASE.NET.Tests.Examples
{
    public class Foto
    {
        [DbfField("TL201")] public string Clave { set; get; }
        [DbfField("TL202")] public byte[] FotoBytes { set; get; }
    }
}
