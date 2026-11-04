namespace dBASE.NET.Tests.Examples
{
    public class EmpresaDto
    {
        public string Unidad { set; get; }
        public string Nombre { set; get; }
        public string NombreLargo { set; get; }
        public string CalleNumero { set; get; }
        public string Colonia { set; get; }
        public string CiudadEstado { set; get; }
        public string CodigoPostal { set; get; }
        public string Telefono { set; get; }
        public string Fax { set; get; }
        public string Rfc { set; get; }
        public string RegistroPatronal { set; get; }
        public string RegistroInfonavit { set; get; }
        public string RepresentanteLegal { set; get; }
        public string RfcRepresentante { set; get; }
        public string Actividad { set; get; }
        public string DirNomina { set; get; }
        public string DirComunes { set; get; }
        public string DirGeneral { set; get; }
        public bool SeparaPersonal { set; get; }
        public string DirPersonal { set; get; }

        // Codificadores
        public string Cod01Desc { set; get; }
        public string Cod02Desc { set; get; }
        public string Cod03Desc { set; get; }
        public string Cod04Desc { set; get; }
        public string Cod05Desc { set; get; }
        public string Cod06Desc { set; get; }
        public string CampoExtraNum01Desc { set; get; }
        public string CampoExtraNum02Desc { set; get; }
        public string CampoExtra01Desc { set; get; }
        public string CampoExtra02Desc { set; get; }
        public string CampoExtra03Desc { set; get; }
        public string CampoExtra04Desc { set; get; }
        public string CampoExtra05Desc { set; get; }

        public int Año { set; get; }
        public string RepresentantePatronal { set; get; }
        public string PuestoRepresentante { set; get; }
        public string RfcDelRepresentante { set; get; }
        public string ClinicaAdscripcion { set; get; }
        public double PrefijoCodigoBarras { set; get; }
        public string Clave { set; get; }
        public bool Autorizado { set; get; }
        public string ClaveSupervisor { set; get; }

        // Memo fields
        public byte[] LogotipoEncabezado { set; get; }
        public byte[] LogotipoEmpresa { set; get; }
        public byte[] LogotipoFondo { set; get; }

        public double UltimoFormatoRecibo { set; get; }
        public double UltimoFormatoBanco { set; get; }
        public bool PendienteRespaldo { set; get; }

        public string CurpRepresentanteLegal { set; get; }
        public bool DesglosaDescansoVacaciones { set; get; }
        public string ParametrosImportacionReloj { set; get; }

        public double TipoEmpresa { set; get; }
        public double EntidadFederativa { set; get; }
        public string CodigoEstablecimiento { set; get; }

        public bool ActivaVencimiento { set; get; }
        public double MesesVencimiento { set; get; }

        public double RegistroHuella { set; get; }
        public double EsquemaEmpresa { set; get; }
        public bool AlmacenaImagenHuella { set; get; }

        public string DirectorioBuzon { set; get; }
        public bool VistaPreviaPdf { set; get; }

        public string NoExterior { set; get; }
        public string NoInterior { set; get; }
        public string Pais { set; get; }
        public string Localidad { set; get; }
        public string Referencia { set; get; }
        public string RegimenFiscal { set; get; }

        public byte[] ImagenFirmaRepresentante { set; get; }
        public bool ActivaRegistroFirma { set; get; }

        public string DirectorioEnlace { set; get; }
        public bool ActivaRegistroUsuario { set; get; }
        public bool ContrasenaSensible { set; get; }

        public string NombreLargoImpresion { set; get; }

        public double RegistroPatronalOpcion { set; get; }
        public double RegimenFiscalNvo { set; get; }
        public double RfcPatronOrigen { set; get; }
        public double EntidadSncf { set; get; }

        public string CurpPersonaFisica { set; get; }
        public bool ActivaFechaAntiguedad { set; get; }

        public string RegistroFonacot { set; get; }
        public bool SalarioMinimoSinRetencion { set; get; }
        public bool AplicaTimbradoRegimenUnificado { set; get; }
        public bool ActivaCapturaVacacionesPendientes { set; get; }
        public bool ActivaPagoPrimaVacacionalAniversario { set; get; }
    }
}
