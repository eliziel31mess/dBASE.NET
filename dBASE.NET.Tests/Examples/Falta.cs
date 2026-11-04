using dBASE.NET.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dBASE.NET.Tests.Examples
{
    public class Falta
    {
        [DbfField("RL601")] public string Clave { set; get; }
        [DbfField("RL602")] public DateTime Fecha { set; get; }
        [DbfField("RL603")] public double Dia { set; get; }
        [DbfField("RL604")] public double HrsAusentismo { set; get; }
        [DbfField("RL605")] public string TipoAusentismo { set; get; }
        [DbfField("RL606")] public bool TurnoCompleto { set; get; }
        [DbfField("RL607")] public double HrsDiaFestivo { set; get; }
        [DbfField("RL608")] public bool Ausentismo { set; get; }
        [DbfField("RL609")] public double HrsDescanso { set; get; }
        [DbfField("RL610")] public bool Descanso { set; get; }
    }
}
