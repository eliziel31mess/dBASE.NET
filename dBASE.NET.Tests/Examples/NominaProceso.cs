using dBASE.NET.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dBASE.NET.Tests.Examples
{
    public class NominaProceso
    {
        [DbfField("nn301")] public string Trabajador { get; set; }
        [DbfField("NN302")] public string Concepto { get; set; }
        [DbfField("NN303")] public double HorasDias01 { get; set; }
        [DbfField("NN304")] public double HorasDias02 { get; set; }
        [DbfField("NN305")] public double Importe { get; set; }
        [DbfField("NN306")] public double ParteExenta { get; set; }
        [DbfField("NN307")] public string Contrato { get; set; }
        [DbfField("NN308")] public bool Sindicato { get; set; }
        [DbfField("NN309")] public double FormaPago { get; set; }
        [DbfField("NN310")] public double DiasPercepcionNormal { get; set; }
    }
}
