using dBASE.NET.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dBASE.NET.Tests.Examples
{
    public class AcumuladoFiscal
    {
        [DbfField("NT401")] public string Clave { get; set; }
        [DbfField("NT402")] public string Periodo { get; set; }
        [DbfField("NT408")] public string Serie { get; set; }
        [DbfField("NT409")] public double Folio { get; set; }
        [DbfField("NT410")] public string CadenaOriginal { get; set; }
        [DbfField("NT411")] public string Sello { get; set; }
        [DbfField("NT412")] public string PdfPath { get; set; }
        [DbfField("NT413")] public string XmlPath { get; set; }
        [DbfField("NT415")] public string Uuid { get; set; }
        [DbfField("NT417")] public string FechaTimbrado { get; set; }
        [DbfField("NT419")] public string CodigoBidimencional { get; set; }
        [DbfField("NT420")] public string CadenaOriginalSAT { get; set; }
        [DbfField("NT421")] public string SelloDigital { get; set; }
        [DbfField("NT422")] public string SelloDigitalSAT { get; set; }
    }
}


