using dBASE.NET;
using dBASE.NET.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dBASE.NET.Tests.Examples
{
    public class Empresa
    {
        [DbfField("EMP01")] public string Nombre { set; get; }
        [DbfField("EMP02")] public string NombreLargo { set; get; }
        [DbfField("EMP03")] public string CalleNumero { set; get; }
        [DbfField("EMP04")] public string Colonia { set; get; }
        [DbfField("EMP05")] public string CiudadEstado { set; get; }
        [DbfField("EMP06")] public string CodigoPostal { set; get; }
        [DbfField("EMP07")] public string Telefono { set; get; }
        [DbfField("EMP08")] public string Fax { set; get; }
        [DbfField("EMP09")] public string Rfc { set; get; }
        [DbfField("EMP10")] public string RegistroPatronal { set; get; }
        [DbfField("EMP11")] public string RegistroInfonavit { set; get; }
        [DbfField("EMP12")] public string RepresentanteLegal { set; get; }
        [DbfField("EMP13")] public string RfcRepresentante { set; get; }
        [DbfField("EMP14")] public string Actividad { set; get; }

        [DbfField("EMP15")] public string DirNomina { set; get; }
        [DbfField("EMP16")] public string DirComunes { set; get; }
        [DbfField("EMP17")] public string DirGeneral { set; get; }

        [DbfField("EMP18")] public bool SeparaPersonal { set; get; }
        [DbfField("EMP19")] public string DirPersonal { set; get; }

        // Codificadores
        [DbfField("EMP20")] public string Cod01Desc { set; get; }
        [DbfField("EMP21")] public string Cod02Desc { set; get; }
        [DbfField("EMP22")] public string Cod03Desc { set; get; }
        [DbfField("EMP23")] public string Cod04Desc { set; get; }
        [DbfField("EMP24")] public string Cod05Desc { set; get; }
        [DbfField("EMP25")] public string Cod06Desc { set; get; }
        [DbfField("EMP26")] public string CampoExtraNum01Desc { set; get; }
        [DbfField("EMP27")] public string CampoExtraNum02Desc { set; get; }
        [DbfField("EMP28")] public string CampoExtra01Desc { set; get; }
        [DbfField("EMP29")] public string CampoExtra02Desc { set; get; }
        [DbfField("EMP30")] public string CampoExtra03Desc { set; get; }
        [DbfField("EMP31")] public string CampoExtra04Desc { set; get; }
        [DbfField("EMP32")] public string CampoExtra05Desc { set; get; }

        [DbfField("EMP33")] public double Año { set; get; }
        [DbfField("EMP34")] public string RepresentantePatronal { set; get; }
        [DbfField("EMP35")] public string PuestoRepresentante { set; get; }
        [DbfField("EMP36")] public string RfcDelRepresentante { set; get; }
        [DbfField("EMP37")] public string ClinicaAdscripcion { set; get; }
        [DbfField("EMP38")] public double PrefijoCodigoBarras { set; get; }
        [DbfField("EMP39")] public string Clave { set; get; }
        [DbfField("EMP40")] public bool Autorizado { set; get; }
        [DbfField("EMP41")] public string ClaveSupervisor { set; get; }

        // Memo fields
        [DbfField("EMP42")] public byte[] LogotipoEncabezado { set; get; }
        [DbfField("EMP43")] public byte[] LogotipoEmpresa { set; get; }
        [DbfField("EMP44")] public byte[] LogotipoFondo { set; get; }

        [DbfField("EMP45")] public double UltimoFormatoRecibo { set; get; }
        [DbfField("EMP46")] public double UltimoFormatoBanco { set; get; }
        [DbfField("EMP47")] public bool PendienteRespaldo { set; get; }

        [DbfField("EMP48")] public string CurpRepresentanteLegal { set; get; }
        [DbfField("EMP49")] public bool DesglosaDescansoVacaciones { set; get; }
        [DbfField("EMP50")] public string ParametrosImportacionReloj { set; get; }

        [DbfField("EMP51")] public double TipoEmpresa { set; get; }
        [DbfField("EMP52")] public double EntidadFederativa { set; get; }
        [DbfField("EMP53")] public string CodigoEstablecimiento { set; get; }

        [DbfField("EMP54")] public bool ActivaVencimiento { set; get; }
        [DbfField("EMP55")] public double MesesVencimiento { set; get; }

        [DbfField("EMP56")] public double RegistroHuella { set; get; }
        [DbfField("EMP57")] public double EsquemaEmpresa { set; get; }
        [DbfField("EMP58")] public bool AlmacenaImagenHuella { set; get; }

        [DbfField("EMP59")] public string DirectorioBuzon { set; get; }
        [DbfField("EMP60")] public bool VistaPreviaPdf { set; get; }

        [DbfField("EMP61")] public string NoExterior { set; get; }
        [DbfField("EMP62")] public string NoInterior { set; get; }
        [DbfField("EMP63")] public string Pais { set; get; }
        [DbfField("EMP64")] public string Localidad { set; get; }
        [DbfField("EMP65")] public string Referencia { set; get; }
        [DbfField("EMP66")] public string RegimenFiscal { set; get; }

        [DbfField("EMP67")] public byte[] ImagenFirmaRepresentante { set; get; }
        [DbfField("EMP68")] public bool ActivaRegistroFirma { set; get; }

        [DbfField("EMP69")] public string DirectorioEnlace { set; get; }
        [DbfField("EMP70")] public bool ActivaRegistroUsuario { set; get; }
        [DbfField("EMP71")] public bool ContrasenaSensible { set; get; }

        [DbfField("EMP72")] public string NombreLargoImpresion { set; get; }

        [DbfField("EMP73")] public double RegistroPatronalOpcion { set; get; }
        [DbfField("EMP74")] public double RegimenFiscalNvo { set; get; }
        [DbfField("EMP75")] public double RfcPatronOrigen { set; get; }
        [DbfField("EMP76")] public double EntidadSncf { set; get; }

        [DbfField("EMP77")] public string CurpPersonaFisica { set; get; }
        [DbfField("EMP78")] public bool ActivaFechaAntiguedad { set; get; }

        [DbfField("EMP79")] public string RegistroFonacot { set; get; }
        [DbfField("EMP80")] public bool SalarioMinimoSinRetencion { set; get; }
        [DbfField("EMP81")] public bool AplicaTimbradoRegimenUnificado { set; get; }
        [DbfField("EMP82")] public bool ActivaCapturaVacacionesPendientes { set; get; }
        [DbfField("EMP83")] public bool ActivaPagoPrimaVacacionalAniversario { set; get; }
    }
}
